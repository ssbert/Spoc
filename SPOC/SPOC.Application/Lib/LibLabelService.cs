using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Category;
using SPOC.Common.Cookie;
using SPOC.Common.Helper;
using SPOC.Common.Pagination;
using SPOC.Lib.Dto;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Json;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Specifications;
using Newtonsoft.Json;
using SPOC.Common;
using SPOC.Common.Dto;
using SPOC.Common.Extensions;
using SPOC.Common.Http;

namespace SPOC.Lib
{
    /// <summary>
    /// 知识点服务
    /// </summary>
    public class LibLabelService : SPOCAppServiceBase, ILibLabelService
    {
        private readonly IRepository<Label, Guid> _iLabelRepository;
        private readonly IRepository<LabelRule, Guid> _iLabelRuleRepository;
        private readonly IRepository<UserBase, Guid> _iUserRep;
        private readonly IRepository<NvFolder, Guid> _iFoldeRepository;
        private readonly IRepository<UserAnswerRecords, Guid> _iUserAnswerRecordsRep;
        private readonly IRepository<UserLabelScore, Guid> _iUserLabelScoreRep;
        private readonly IRepository<QuestionLabel, Guid> _iQuestionLabelRep;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;
        private string NewMoocApiUrl => L("payUrl").TrimEnd('/') + "/api/";
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iLabelRepository"></param>
        /// <param name="iUserRep"></param>
        /// <param name="iFoldeRepository"></param>
        /// <param name="iLabelRuleRepository"></param>
        public LibLabelService(IRepository<Label, Guid> iLabelRepository, IRepository<UserBase, Guid> iUserRep, IRepository<NvFolder, Guid> iFoldeRepository, IRepository<LabelRule, Guid> iLabelRuleRepository, IRepository<UserAnswerRecords, Guid> iUserAnswerRecordsRep, IRepository<UserLabelScore, Guid> iUserLabelScoreRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep, IUnitOfWorkManager iUnitOfWorkManager)
        {
            _iLabelRepository = iLabelRepository;
            _iUserRep = iUserRep;
            _iFoldeRepository = iFoldeRepository;
            _iLabelRuleRepository = iLabelRuleRepository;
            _iUserAnswerRecordsRep = iUserAnswerRecordsRep;
            _iUserLabelScoreRep = iUserLabelScoreRep;
            _iQuestionLabelRep = iQuestionLabelRep;
            _iUnitOfWorkManager = iUnitOfWorkManager;
            _iLabelRuleRepository = iLabelRuleRepository;
        }

        /// <summary>
        /// 分页获取标签数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<LabelPaginationItem>> GetPagination(LabelPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                throw new UserFriendlyException("登录已过期");
            }
            var queryable = (from l in _iLabelRepository.GetAll()
                             join u in _iUserRep.GetAll() on l.creatorId equals u.Id
                             join f in _iFoldeRepository.GetAll() on l.folderId equals f.Id
                             orderby l.createTime descending
                             select new LabelPaginationItem
                             {
                                 folderName = f.folderName,
                                 folderId = l.folderId,
                                 id = l.Id,
                                 describe = l.describe,
                                 regExpressions = l.regExpressions,
                                 title = l.title,
                                 createTime = l.createTime,
                                 userFullName = u.userFullName,
                                 userLoginName = u.userLoginName
                             }).WhereIf(input.folderId.Any(), q => input.folderId.Contains(q.folderId))
                .WhereIf(!string.IsNullOrWhiteSpace(input.title), q => q.title.Contains(input.title))
                .WhereIf(!string.IsNullOrWhiteSpace(input.userFullName), q => q.userFullName.Contains(input.userFullName))
                .WhereIf(!string.IsNullOrWhiteSpace(input.userLoginName), q => q.userLoginName.Contains(input.userLoginName))
                .WhereIf(input.exceptList.Any(), q => !input.exceptList.Contains(q.id));
            return await Task.FromResult(new PaginationOutputDto<LabelPaginationItem>()
            {
                rows = queryable.Skip(input.skip).Take(input.pageSize).ToList(),
                total = queryable.Count()
            });
        }

        /// <summary>
        /// 获取知识库标签Combobox数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<ComboboxItem>> GetComboboxList()
        {
            var queryList = await _iLabelRepository.GetAll()
                .Select(a => new
                {
                    a.Id,
                    a.title,
                    a.createTime
                }).ToListAsync();
            var result = new List<ComboboxItem>();
            for (var i = 0; i < queryList.Count; i++)
            {
                var item = queryList[i];
                result.Add(new ComboboxItem
                {
                    id = item.Id,
                    text = item.title,
                    seq = i
                });
            }
            return result;
        }

        /// <summary>
        /// 根据ID删除标签
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task Delete(string ids)
        {
            #region 验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            #endregion
            var idArray = ids.Split(',').Select(a => a.TryParseGuid()).ToArray();
            foreach (var id in idArray)
            {
                await _iLabelRepository.DeleteAsync(a => a.Id == id);
            }


        }
        /// <summary>
        /// 获取标签信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<LabelDto> Get(string id)
        {

            var lid = id.TryParseGuid();
            var label = await _iLabelRepository.FirstOrDefaultAsync(lid);
            var labelDto = new LabelDto
            {
                id = label.Id,
                describe = label.describe,
                folderId = label.folderId,
                title = label.title,
                rules=new List<LabelRulesDto>()
            };
            var rules = await _iLabelRuleRepository.GetAll().Where(l => l.labelId.Equals(lid)).OrderBy(a=>a.seq).ToListAsync();
            rules.ForEach(r =>
            {
                labelDto.rules.Add(new LabelRulesDto
                {
                    id = r.Id,
                    describe = r.describe,
                    logic = r.logic,
                    matchText = r.matchText,
                    regExpressions = r.regExpressions
                });
            });
            return labelDto;
        }
        /// <summary>
        /// 新增OR编辑标签信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Guid> CreateOrUpdate(LabelDto input)
        {
            #region 验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            #endregion
            var label = new Label
            {
                Id = Guid.NewGuid(),
                createTime = DateTime.Now,
                creatorId = cookie.Id
            };
            if (input.id != Guid.Empty)
            {
                label = await _iLabelRepository.FirstOrDefaultAsync(input.id);
            }
            label.describe = input.describe;
            label.folderId = input.folderId;
            label.title = input.title;
            if (input.id != Guid.Empty)
                await _iLabelRepository.UpdateAsync(label);
            else
                await _iLabelRepository.InsertAsync(label);
            var labelInput = new LabelInputDto() { Label = label ,LabelRules=new List<LabelRule>()};
            var ruleIds = input.rules.Select(r => r.id);
            //删除当前标签下多余的规则记录   例如之前存在A B 2个规则，界面上删除规则B 只剩余A规则存储在input.rules中
            await _iLabelRuleRepository.DeleteAsync(a => a.labelId.Equals(label.Id) && !ruleIds.Contains(a.Id));
            int seq = 0;
            foreach (var rule in input.rules)
            {
                seq++;
                if (string.IsNullOrWhiteSpace(rule.matchText))
                    continue;
                var labelRule = new LabelRule()
                {
                    Id = Guid.NewGuid(),
                    createTime = DateTime.Now,
                    creatorId = cookie.Id,
                   
                };
                if (rule.id != Guid.Empty)
                {
                    //编辑规则
                    labelRule = await _iLabelRuleRepository.FirstOrDefaultAsync(rule.id);
                }
                labelRule.seq = seq;
                labelRule.describe = rule.describe;
                labelRule.labelId = label.Id;
                labelRule.logic = rule.logic;
                labelRule.matchText = rule.matchText;
                labelRule.regExpressions = rule.regExpressions;
                if (rule.id != Guid.Empty)
                    await _iLabelRuleRepository.UpdateAsync(labelRule);
                else
                    await _iLabelRuleRepository.InsertAsync(labelRule);
                labelInput.LabelRules.Add(labelRule);
            }
            var success = await LabelPublish(labelInput);
            if (!success)
                throw new UserFriendlyException("标签同步云端失败");
            return label.Id;
        }
        /// <summary>
        /// 编译题发布新课云
        /// </summary>
        /// <returns></returns>
        private async Task<bool> LabelPublish(LabelInputDto labelInput)
        {

            var jsonValue = JsonValue.Parse(JsonConvert.SerializeObject(labelInput));
            var apiContent = System.Web.HttpUtility.UrlEncode(jsonValue.ToString(),
                Encoding.UTF8);
            var url = NewMoocApiUrl + "/lib/label?sign=";
            var result = await HttpHelper.PostResponseSerializeData<ApiResponseResult<dynamic>>(url, apiContent);
            return result.IsSuccess;
        }
        /// <summary>
        /// 获取标签数据
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> LoadLabelForChoose()
        {
            var labels = await _iLabelRepository.GetAll().Select(a=>new {a.Id,a.title}).ToListAsync();
            return labels;
        
        }
        /// <summary>
        /// 搜索文字 根据规则匹配标签
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<dynamic> SmartSeachLabel(MatchTextInputDto input)
        {
            var searchText = $"{input.code}{StringUtil.NoHTML(input.questionText)}";
            //正则替换掉代码或者题干中的注释
            searchText = Regex.Replace(searchText, "(?<!:)\\/\\/.*|\\/\\*(\\s|.)*?\\*\\/", "");
            //查询标签规则库
            var labelRules = from l in _iLabelRepository.GetAll()
                join lr in _iLabelRuleRepository.GetAll() on l.Id equals lr.labelId
                select new
                {
                    l.Id,
                    l.title,
                    lr.logic,
                    lr.matchText,
                    lr.regExpressions
                };
            var ruleDic = await labelRules.GroupBy(a => new{a.Id,a.title}).ToDictionaryAsync(a => a.Key, a => a.ToList());
            //返回匹配的标签
            var labels = new List<Guid>();
            //返回匹配上的文字 需要返回到加分项上
            var matchText  = new Dictionary<Guid,List<string>>();
            #region 合并匹配结果
            bool Compare(List<MatchResult> matchResult)
            {
                Expression<Func<bool, bool>> expression = e => false;
                matchResult.ForEach(a =>
                {
                    Expression<Func<bool, bool>> ep = e => a.IsMatch;
                    expression= a.Logic == 0 ? expression.Or(ep) : expression.And(ep);
                });
               return  expression.Compile().Invoke(false);
            }
            #endregion

            ruleDic.ForEach(rule =>
            {
                //标签对应具体规则列表
                var rules = rule.Value;
                string pat = @""; //正则
                var matchResult = new List<MatchResult>();
                //标签规则全部填写了正则 则汇总成一个正则后利用正则匹配
                var matchByRegex = !rules.Any(a => string.IsNullOrWhiteSpace(a.regExpressions));
                rules.ForEach(r =>
                {
                    if (!string.IsNullOrWhiteSpace(r.regExpressions))
                        pat = $"{pat}|{r.regExpressions}";
                    //如果不能用一个正则匹配则单个匹配存入
                    if (!matchByRegex)
                    {
                        Regex rx = new Regex(r.regExpressions, RegexOptions.IgnoreCase);
                        var isMatch = !string.IsNullOrWhiteSpace(r.regExpressions)
                            ? rx.IsMatch(searchText)
                            : searchText.Contains(r.matchText);
                        if (isMatch)
                        {
                            if(matchText.ContainsKey(r.Id))
                                matchText[r.Id].Add(r.matchText);
                            else
                                matchText.Add(r.Id,new List<string>{ r.matchText });
                        }
                        matchResult.Add(new MatchResult
                        {
                            IsMatch = isMatch,
                            Logic = r.logic
                        });
                    }

                });
                //是否正则匹配，否则对比匹配单个规则中的关键字
                if (matchByRegex)
                {
                    Regex rx = new Regex(pat.Trim('|'), RegexOptions.IgnoreCase);
                    if (rx.IsMatch(searchText))
                    {
                        labels.Add(rule.Key.Id);
                        //匹配文字信息
                        rules.ForEach(r =>
                        {
                            if (searchText.Contains(r.matchText))
                            {
                                if (matchText.ContainsKey(r.Id))
                                    matchText[r.Id].Add(r.matchText);
                                else
                                    matchText.Add(r.Id, new List<string> { r.matchText });
                            }
                        });
                    }
                    

                }
                else
                {
                    //合并判断单个规则匹配记录
                    if (Compare(matchResult))
                    {
                        labels.Add(rule.Key.Id);
                    }

                }
            });
            //返回匹配上的标签和 标签对应关键字
            return new {label=labels, matchText };



        }

        /// <summary>
        /// 创建用户作答记录并更新标签积分
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="questionId">试题id</param>
        /// <param name="recordId">作答详情id</param>
        /// <param name="source">来源(exam, exercise, challenge)</param>
        /// <param name="pass">是否通过</param>
        /// <returns></returns>
        public async Task CreateUserAnswerRecords(Guid userId, Guid questionId, Guid recordId, string source, bool pass)
        {
            var questionType = source == "challenge" ? "challenge" : "normal";
            var labelIds = await _iQuestionLabelRep.GetAll()
                .Where(a => a.questionId == questionId && a.labelType == 1 && a.questionType == questionType)
                .Select(a => a.labelId).ToListAsync();
            var labelPoint = Convert.ToInt32(BaseSiteSetDto.labelPoint);
            var labelDeductPoint = -Convert.ToInt32(BaseSiteSetDto.labelDeductPoint);
            foreach (var labelId in labelIds)
            {
                var dto = new UserAnswerRecords
                {
                    Id = Guid.NewGuid(),
                    LabelId = labelId,
                    QuestionId = questionId,
                    UserId = userId,
                    RecordId = recordId,
                    CreateTime = DateTime.Now,
                    Score = pass ? labelPoint : labelDeductPoint,
                    Source = source
                };
                await _iUserAnswerRecordsRep.InsertAsync(dto);
                var userLabelScore = await _iUserLabelScoreRep.GetAll()
                    .Where(a => a.LabelId == labelId && a.UserId == userId).FirstOrDefaultAsync();
                if (userLabelScore == null)
                {
                    userLabelScore = new UserLabelScore
                    {
                        Id = Guid.NewGuid(),
                        LabelId = labelId,
                        UserId = userId,
                        Score = pass ? labelPoint : labelDeductPoint
                    };

                    await _iUserLabelScoreRep.InsertAsync(userLabelScore);
                }
                else
                {
                    userLabelScore.Score += pass ? labelPoint : labelDeductPoint;
                    if (userLabelScore.Score > labelPoint)
                    {
                        userLabelScore.Score = labelPoint;
                    }
                    else if (userLabelScore.Score < labelDeductPoint)
                    {
                        userLabelScore.Score = labelDeductPoint;
                    }
                    await _iUserLabelScoreRep.UpdateAsync(userLabelScore);
                }
            }
            _iUnitOfWorkManager.Current.SaveChanges();
        }
        /// <summary>
        /// 存储匹配结果
        /// </summary>
        private struct MatchResult
        {
            /// <summary>
            /// 是否匹配
            /// </summary>
            public bool IsMatch { get; set; }
          
            /// <summary>
            /// 逻辑关系
            /// </summary>
            public int Logic { get; set; }
        }
    }
}
