using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Castle.Core.Logging;
using SPOC.Category;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.Extensions;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.Common.Pagination;
using SPOC.Core.Dto.Challenge;
using SPOC.Core.Dto.CodeCompile;
using SPOC.Exam;
using SPOC.QuestionBank;
using SPOC.QuestionBank.Const;
using SPOC.QuestionBank.Dto;
using SPOC.SqlExecuter;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Json;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Aspose.Words;
using Newtonsoft.Json;
using SPOC.Common;
using SPOC.Common.Http;
using SPOC.Lib;
using SPOC.Lib.Dto;
using DateTimeUtil = SPOC.Common.Helper.DateTimeUtil;

namespace SPOC.Core
{
    /// <summary>
    /// 试卷题库服务
    /// </summary>
    public class ChallengeQuestionService : SPOCAppServiceBase, IChallengeQuestionService
    {
        private readonly IRepository<ChallengeQuestion, Guid> _iChallengeQuestionRep;
        private readonly IRepository<ExamQuestionType, Guid> _iExamQuestionTypeRep;
        private readonly IRepository<NvFolder, Guid> _iNvFolderRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ChallengeGrade, Guid> _iChallengeGradeRep;
        private readonly IRepository<UserBase, Guid> _iUserRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<Major, Guid> _iMajorRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<Label, Guid> _iLabelRep;
        private readonly IRepository<QuestionLabel, Guid> _iQuestionLabelRep;
        private readonly IRepository<QuestionStandardCode, Guid> _iQuestionStandardCodeRep;
        private readonly ILibLabelService _iLibLabelService;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;
        private readonly ICodeComplieService _iCodeComplieService;
        private readonly ISqlExecuter _iSqlExecuter;
        private readonly ILogger _iLogger;
        private string NewMoocApiUrl => L("payUrl").TrimEnd('/') + "/api/";
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChallengeQuestionService(IRepository<ExamQuestionType, Guid> iExamQuestionTypeRepository,
            IRepository<ChallengeQuestion, Guid> iChallengeQuestionRepository,
            IRepository<NvFolder, Guid> iNvFolderRepository, 
            IRepository<TeacherInfo, Guid> iTeacherInfoRep,
            IUnitOfWorkManager iUnitOfWorkManager,  IRepository<UserBase, Guid> iUserRep, IRepository<ChallengeGrade, Guid> iChallengeGradeRep, ICodeComplieService iCodeComplieService, ISqlExecuter iSqlExecuter, IRepository<ClassStudent, Guid> iClassStudentRep, IRepository<Major, Guid> iMajorRep, IRepository<Class, Guid> iClassRep, IRepository<QuestionStandardCode, Guid> iQuestionStandardCodeRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep, ILibLabelService iLibLabelService, IRepository<Label, Guid> iLabelRep)
        {
            _iChallengeQuestionRep = iChallengeQuestionRepository;
            _iExamQuestionTypeRep = iExamQuestionTypeRepository;
            _iNvFolderRep = iNvFolderRepository;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iUnitOfWorkManager = iUnitOfWorkManager;        
            _iUserRep = iUserRep;
            _iChallengeGradeRep = iChallengeGradeRep;
            _iCodeComplieService = iCodeComplieService;
            _iSqlExecuter = iSqlExecuter;
            _iClassStudentRep = iClassStudentRep;
            _iMajorRep = iMajorRep;
            _iClassRep = iClassRep;
            _iQuestionStandardCodeRep = iQuestionStandardCodeRep;
            _iQuestionLabelRep = iQuestionLabelRep;
            _iLibLabelService = iLibLabelService;
            _iLabelRep = iLabelRep;
            _iLogger = new NullLogger();
        }
        #region 后台管理
        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<QuestionItem>> GetPagination(QuestionPaginationInputDto input)
        {
            input.questionText = input.questionText.Trim();
            input.questionCode = input.questionCode.Trim();
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                throw new UserFriendlyException(-1,"登录已过期");
            }
            List<Guid> questionIdList = null;
            if (input.labelFolderIdList.Any())
            {
                questionIdList =  _iQuestionLabelRep.GetAll().WhereIf(input.labelIdList.Any(),a => input.labelIdList.Contains(a.labelId) && a.questionType.Equals("challenge")).Join(_iLabelRep.GetAll().Where(a=> input.labelFolderIdList.Contains(a.folderId)),a=>a.labelId,b=>b.Id,(a,b)=>a)
                    .Select(a => a.questionId).ToList();
            }
            else if (input.labelIdList.Any())
            {
                questionIdList = await _iQuestionLabelRep.GetAll().Where(a => input.labelIdList.Contains(a.labelId) && a.questionType.Equals("challenge"))
                    .Select(a => a.questionId).ToListAsync();
            }
            
            var queryables = _iChallengeQuestionRep.GetAll().AsNoTracking()
                .WhereIf(questionIdList != null, a => questionIdList.Contains(a.Id))
                .WhereIf(!string.IsNullOrEmpty(input.title), a => a.title.Contains(input.title))
                .WhereIf(!string.IsNullOrEmpty(input.questionText), a => a.questionPureText.Contains(input.questionText))
                .WhereIf(!string.IsNullOrEmpty(input.questionCode), a => a.questionCode.Contains(input.questionCode))
                .WhereIf(!string.IsNullOrEmpty(input.questionBaseTypeCode),a => a.questionBaseTypeCode == input.questionBaseTypeCode)
                .WhereIf(input.folderUidList.Any(), a => input.folderUidList.Contains(a.folderUid))
                .WhereIf(!string.IsNullOrEmpty(input.questionStatusCode),a => a.questionStatusCode == input.questionStatusCode)
                .WhereIf(!string.IsNullOrEmpty(input.hardGrade), a => a.hardGrade == input.hardGrade)
                .WhereIf(!string.IsNullOrEmpty(input.operateTypeCode), a => a.operateTypeCode == QuestionTypeConst.Operate && a.operateTypeCode == input.operateTypeCode)
                .WhereIf(input.hasChild == "N", a => a.parentQuestionUid == Guid.Empty)
                .WhereIf(input.hasAnalysis == "Y", a => !string.IsNullOrEmpty(a.questionAnalysis))
                .WhereIf(input.hasAnalysis == "N", a => string.IsNullOrEmpty(a.questionAnalysis))
               // .WhereIf(!cookie.IsAdmin, a=>a.creatorUid == cookie.Id)
                .GroupJoin(input.exceptionIdList, q=>q.Id, id=>id, (q, ids)=>new {question=q, ids})
                .Where(a=>!a.ids.Any())
                .Select(a=>a.question)
                .Join(_iExamQuestionTypeRep.GetAll().AsNoTracking(), q => q.questionTypeUid, qt => qt.Id,
                    (q, qt) => new { question = q, questionType = qt });

            var tempQueryable = from a in queryables
                                join u in _iUserRep.GetAll().AsNoTracking() on a.question.creatorUid equals u.Id
                                select new QuestionItem()
            {
                                    
                Id = a.question.Id,
                title = a.question.title,
                language=a.question.language,
                questionTypeUid = a.question.questionTypeUid,
                questionCode = a.question.questionCode,
                questionTypeName = a.questionType.questionTypeName,
                parentQuestionUid = a.question.parentQuestionUid,
                questionBaseTypeCode = a.question.questionBaseTypeCode,
                questionText = a.question.questionText,
                hardGrade = a.question.hardGrade,
                score = a.question.score,
                outdatedDate = a.question.outdatedDate,
                examTime = a.question.examTime,
                questionStatusCode = a.question.questionStatusCode,
                listOrder = a.question.listOrder,
                createTime = a.question.createTime,
                creatorUid = a.question.creatorUid,
                userLoginName = u.userLoginName,
                userFullName = u.userFullName,
                allowEdit = cookie.Id.Equals(a.question.creatorUid) || cookie.IsAdmin
                                };

            tempQueryable = !string.IsNullOrEmpty(input.sort) ? tempQueryable.OrderBy(input.sort + " " + input.order) : tempQueryable.OrderByDescending(a => a.createTime);

            var result= await Task.FromResult(new PaginationOutputDto<QuestionItem>()
            {
                rows = tempQueryable.Skip(input.skip).Take(input.pageSize).ToList(),
                total = tempQueryable.Count()
            });
        
            return result;
        }

        /// <summary>
        /// 创建一条新的试题数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ExamQuestionDto> Create(ChallengeQuestionInputDto input)
        {
            //TODO:需要添加验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }
            if (!_iNvFolderRep.GetAll().Any(a => a.Id == input.folderUid))
            {
                throw new UserFriendlyException("无效的试题分类");
            }
            var questionType = _iExamQuestionTypeRep.GetAll().FirstOrDefault(a => a.questionBaseTypeCode == input.questionBaseTypeCode);
            if (questionType == null)
            {
                throw new UserFriendlyException("无效的试题题型");
            }
            if (input.parentQuestionUid != Guid.Empty && !_iChallengeQuestionRep.GetAll().Any(a => a.Id == input.parentQuestionUid && a.questionBaseTypeCode == QuestionTypeConst.Compose))
            {
                throw new UserFriendlyException("无效的父试题");
            }
            
            if (input.questionBaseTypeCode == QuestionTypeConst.Operate &&
                !OperateTypeConst.HasConstValue(input.operateTypeCode))
            {
                throw new UserFriendlyException("无效的操作类型");
            }

            if (input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.questionCode))
                {
                    throw new UserFriendlyException("自定义试题编号不能为空");
                }

                if (_iChallengeQuestionRep.GetAll().Any(a => a.questionCode == input.questionCode))
                {
                    throw new UserFriendlyException("已有相同的试题编号");
                }
            }

            #endregion

            if (!input.isCustomCode)
            {
                input.questionCode = CreateNewCode();
            }

            var entity = input.MapTo<ChallengeQuestion>();

            entity.createTime = entity.modifyTime = DateTime.Now;
            entity.Id = Guid.NewGuid();
            entity.modifier = cookie.Id;
            entity.creatorUid = cookie.Id;
            entity.language = L("Language");
            entity.questionTypeUid = questionType.Id;
            entity.questionPureText = QuestionUtil.RemoveHtmlTag(entity.questionText);
            #region 图片处理
            var host = "http://" + HttpContext.Current.Request.Url.Authority;
            string rootPath = HttpContext.Current.Server.MapPath("~/");
            Func<string, string, string, string> changeImgPath = (path, content, id) =>
            {
                var reg = new Regex("<img.*?>");
                var reg2 = new Regex("src=\".*?\"");
                var matches = reg.Matches(content);
                foreach (Match match in matches)
                {
                    var value = match.Value.Replace(host, "");
                    content = content.Replace(match.Value, value);

                    var srcMatch = reg2.Match(value);
                    var src = srcMatch.Value.Substring(6, srcMatch.Value.Length - 7);
                    src = src.Replace("/", "\\");
                    var srcPath = rootPath + src;
                    if (File.Exists(srcPath))
                    {
                        var dirPath = Path.Combine(rootPath, "fileroot", "question", id);
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        var index = srcPath.LastIndexOf("\\", StringComparison.Ordinal);
                        var fileName = srcPath.Substring(index + 1);
                        var targetPath = Path.Combine(dirPath, fileName);
                        if (!File.Exists(targetPath))
                        {
                            File.Copy(srcPath, targetPath);
                        }
                        content = content.Replace(srcMatch.Value, "src=\"./" + fileName + "\"");
                    }
                }
                return content;
            };
            var idStr = entity.Id.ToString();
            var fileRootPath = FilePathUtil.GetOppositeFileWebPathRoot(idStr, "question");
            entity.questionText = changeImgPath(fileRootPath, entity.questionText, idStr);
            entity.questionAnalysis = changeImgPath(fileRootPath, entity.questionAnalysis, idStr);
            entity.standardAnswer = changeImgPath(fileRootPath, entity.standardAnswer, idStr);
            entity.selectAnswer = changeImgPath(fileRootPath, entity.selectAnswer, idStr);
            #endregion
            //TODO:缺少创建者与修改者的赋值
            //UserSession loginUserSession=(UserSession) Session[Session.SessionID]

            await _iChallengeQuestionRep.InsertAsync(entity);

            if (input.parentQuestionUid != Guid.Empty)
            {
                var parentScoreQuery =
                    _iChallengeQuestionRep.GetAll()
                        .Where(a => a.parentQuestionUid == input.parentQuestionUid)
                        .Select(a => a.score);
                var parentScore = parentScoreQuery.Any() ? parentScoreQuery.Sum() : 0m;
                parentScore += input.score;
                await _iChallengeQuestionRep.UpdateAsync(input.parentQuestionUid, q =>
                {
                    q.score = parentScore;
                    return Task.FromResult(q);
                });
            }
            var questionInput = new QuestionToCloudDto
            {
                ChallengeQuestion = entity,
                QuestionStandardCodes = new List<QuestionStandardCode>(),
                QuestionLabels = new List<QuestionLabel>()
            };
            //处理标准答案
            if (!string.IsNullOrWhiteSpace(input.standardCode))
            {
                var standardCode = new QuestionStandardCode
                {
                    Id = Guid.NewGuid(),
                    questionId = entity.Id,
                    code = input.standardCode,
                    isDefault = 1,
                    modifyTime = DateTime.Now,
                    type = "challenge"
                };
                questionInput.QuestionStandardCodes.Add(standardCode);
                _iQuestionStandardCodeRep.Insert(standardCode);
            }
            //处理主标签信息
            if (input.label != null)
            {
                foreach (var label in input.label)
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        labelId = label,
                        labelType = 1,
                        questionId = entity.Id,
                        questionType = "challenge"
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                   
                }
               
            }
            //处理辅标签信息
            if (input.seclabel != null)
            {
                foreach (var label in input.seclabel)
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        labelId = label,
                        labelType = 0,
                        questionId = entity.Id,
                        questionType = "challenge"
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                }

            }
            var success = await CompileQuestionPublish(questionInput);
            if (!success)
                throw new UserFriendlyException("编程题云端存储保存失败");
            var dto = entity.MapTo<ExamQuestionDto>();
            dto.questionText = FilePathUtil.GetContentTextWithFilePath(idStr, "question", dto.questionText, false);
            dto.questionAnalysis = FilePathUtil.GetContentTextWithFilePath(idStr, "question", dto.questionAnalysis, false);
            dto.standardAnswer = FilePathUtil.GetContentTextWithFilePath(idStr, "question", dto.standardAnswer, false);
            dto.selectAnswer = FilePathUtil.GetContentTextWithFilePath(idStr, "question", dto.selectAnswer, false);
            dto.label = input.label;
            dto.secLabel = input.seclabel;
            return await Task.FromResult(dto);
        }

        /// <summary>
        /// 更新一条试题数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Update(ChallengeQuestionInputDto input)
        {
            //TODO:需要添加验证
            var cookie = CookieHelper.GetLoginInUserInfo();

            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }
            if (!_iChallengeQuestionRep.GetAll().Any(a => a.Id == input.Id))
            {
                throw new UserFriendlyException("无效的试题");
            }

            if (!_iNvFolderRep.GetAll().Any(a => a.Id == input.folderUid))
            {
                throw new UserFriendlyException("无效的试题分类");
            }

            if (!_iExamQuestionTypeRep.GetAll()
                    .Any(a => a.Id == input.questionTypeUid && a.questionBaseTypeCode == input.questionBaseTypeCode))
            {
                throw new UserFriendlyException("无效的试题题型");
            }
            if (input.parentQuestionUid != Guid.Empty && !_iChallengeQuestionRep.GetAll().Any(a => a.Id == input.parentQuestionUid && a.questionBaseTypeCode == QuestionTypeConst.Compose))
            {
                throw new UserFriendlyException("无效的父试题");
            }

            if (input.questionBaseTypeCode == QuestionTypeConst.Operate &&
                !OperateTypeConst.HasConstValue(input.operateTypeCode))
            {
                throw new UserFriendlyException("无效的操作类型");
            }

            if (input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.questionCode))
                {
                    throw new UserFriendlyException("自定义试题编号不能为空");
                }

                if (_iChallengeQuestionRep.GetAll().Any(a => a.questionCode == input.questionCode && a.Id != input.Id))
                {
                    throw new UserFriendlyException("已有相同的试题编号");
                }
            }
            #endregion

            var entity = _iChallengeQuestionRep.Get(input.Id);
            var scoreChanded = input.score != entity.score;
            var score = scoreChanded ? input.score - entity.score : 0m;//计算以增加分数计算基准
            if (entity.isCustomCode && !input.isCustomCode)
            {
                input.questionCode = CreateNewCode();
            }
            try
            {
                input.MapTo(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            entity.modifyTime = DateTime.Now;
            entity.questionPureText = QuestionUtil.RemoveHtmlTag(entity.questionText);
            #region 图片处理
            var host = "http://" + HttpContext.Current.Request.Url.Authority;
            string rootPath = HttpContext.Current.Server.MapPath("~/");
            Func<string, string, string, string> changeImgPath = (path, content, id) =>
            {
                var reg = new Regex("<img.*?>");
                var reg2 = new Regex("src=\".*?\"");
                var matches = reg.Matches(content);
                foreach (Match match in matches)
                {
                    var value = match.Value.Replace(host, "");
                    content = content.Replace(match.Value, value);

                    var srcMatch = reg2.Match(value);
                    var src = srcMatch.Value.Substring(6, srcMatch.Value.Length - 7);
                    src = src.Replace("/", "\\");
                    var srcPath = rootPath + src;
                    if (File.Exists(srcPath))
                    {
                        var dirPath = Path.Combine(rootPath, "fileroot", "question", id);
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        var index = srcPath.LastIndexOf("\\", StringComparison.Ordinal);
                        var fileName = srcPath.Substring(index + 1);
                        var targetPath = Path.Combine(dirPath, fileName);
                        if (!File.Exists(targetPath))
                        {
                            File.Copy(srcPath, targetPath);
                        }
                        content = content.Replace(srcMatch.Value, "src=\"./" + fileName + "\"");
                    }
                }
                return content;
            };
            var idStr = entity.Id.ToString();
            var fileRootPath = FilePathUtil.GetOppositeFileWebPathRoot(idStr, "question");
            entity.questionText = changeImgPath(fileRootPath, entity.questionText, idStr);
            entity.questionAnalysis = changeImgPath(fileRootPath, entity.questionAnalysis, idStr);
            entity.standardAnswer = changeImgPath(fileRootPath, entity.standardAnswer, idStr);
            entity.selectAnswer = changeImgPath(fileRootPath, entity.selectAnswer, idStr);
            #endregion
            //TODO:缺少创建者与修改者的赋值
            //UserSession loginUserSession=(UserSession) Session[Session.SessionID]

            await _iChallengeQuestionRep.UpdateAsync(entity);

            if (scoreChanded && input.parentQuestionUid != Guid.Empty)
            {
                var parentScore =
                    _iChallengeQuestionRep.GetAll()
                        .Where(a => a.parentQuestionUid == input.parentQuestionUid)
                        .Select(a => a.score)
                        .Sum();
                parentScore += score;
                await _iChallengeQuestionRep.UpdateAsync(input.parentQuestionUid, q =>
                {
                    q.score = parentScore;
                    return Task.FromResult(q);
                });
            }
            var questionInput = new QuestionToCloudDto
            {
                ChallengeQuestion = entity,
                QuestionStandardCodes = _iQuestionStandardCodeRep.GetAllList(a => a.isDefault == 0 && a.questionId.Equals(entity.Id)),
                QuestionLabels = new List<QuestionLabel>()
            };
            //处理标准答案
            _iQuestionStandardCodeRep.Delete(a=>a.isDefault==1 && a.questionId.Equals(entity.Id));
            if (!string.IsNullOrWhiteSpace(input.standardCode))
            {
                var standardCode = new QuestionStandardCode
                {
                    Id = Guid.NewGuid(),
                    questionId = entity.Id,
                    code = input.standardCode,
                    isDefault = 1,
                    modifyTime = DateTime.Now,
                    type = "challenge"
                };
                questionInput.QuestionStandardCodes.Add(standardCode);
                _iQuestionStandardCodeRep.Insert(standardCode);
            }
            //处理主标签信息
            _iQuestionLabelRep.Delete(a=>a.questionId.Equals(entity.Id));
            if (input.label != null)
            {
                foreach (var label in input.label)
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        labelId = label,
                        labelType = 1,
                        questionId = entity.Id,
                        questionType = "challenge"
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                }

            }
            //处理辅标签信息
            if (input.seclabel != null)
            {
                foreach (var label in input.seclabel)
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        labelId = label,
                        labelType = 0,
                        questionId = entity.Id,
                        questionType = "challenge"
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                }

            }
            var success = await CompileQuestionPublish(questionInput);
            if (!success)
                throw new UserFriendlyException("编程题同步云端失败");
        }
        /// <summary>
        /// 挑战题发布新课云
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CompileQuestionPublish(QuestionToCloudDto question)
        {

            var jsonValue = JsonValue.Parse(JsonConvert.SerializeObject(question));
            var apiContent = System.Web.HttpUtility.UrlEncode(jsonValue.ToString(),
                Encoding.UTF8);
            var url = NewMoocApiUrl + "/lib/question?sign=";
            var result = await HttpHelper.PostResponseSerializeData<ApiResponseResult<dynamic>>(url, apiContent);
            return result.IsSuccess;
        }
        /// <summary>
        /// 根据Id字符串删除对应的试题，id用逗号“,”分隔
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<string> Delete(string ids)
        {
            var idArray = ids.Split(',').Select(a=>new Guid(a)).ToArray();
            var entityList = new List<ChallengeQuestion>();
            var childList = new List<ChallengeQuestion>();
            var cookie = CookieHelper.GetLoginInUserInfo();
            //TODO:需要补完验证
            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }
            var skipCount=0;
            foreach (var uid in idArray)
            {
                //有答题记录的试题跳过不删除
                if (_iChallengeGradeRep.GetAll().Any(a => a.questionId.Equals(uid)))
                {
                    skipCount++;
                    continue;
                }
                var entity = _iChallengeQuestionRep.Get(uid);
                if (entity == null)
                {
                    throw new UserFriendlyException("无效的试题");
                }

                entityList.Add(entity);

                if (entity.questionBaseTypeCode == QuestionTypeConst.Compose)
                {
                    var children = _iChallengeQuestionRep.GetAll().Where(a => a.parentQuestionUid == entity.Id).ToList();
                    childList.AddRange(children);
                }
            }
           
            #endregion

            var dic = new Dictionary<Guid, List<ChallengeQuestion>>();

            foreach (var challengeQuestion in childList)
            {
                List<ChallengeQuestion> list;
                if (dic.ContainsKey(challengeQuestion.parentQuestionUid))
                {
                    list = dic[challengeQuestion.parentQuestionUid];
                }
                else
                {
                    list = new List<ChallengeQuestion>();
                    dic.Add(challengeQuestion.parentQuestionUid, list);
                }
                list.Add(challengeQuestion);
                await _iChallengeQuestionRep.DeleteAsync(challengeQuestion);
            }

            foreach (var examQuestion in entityList)
            {
                await _iChallengeQuestionRep.DeleteAsync(examQuestion);
                _iQuestionStandardCodeRep.Delete(a=>a.questionId.Equals(examQuestion.Id));
                _iQuestionLabelRep.Delete(a => a.questionId.Equals(examQuestion.Id));
            }

            foreach (var kv in dic)
            {
                if (entityList.Any(a => a.Id == kv.Key))
                {
                    continue;
                }
                var score = kv.Value.Select(q => q.score).Sum();
                await _iChallengeQuestionRep.UpdateAsync(kv.Key, q =>
                {
                    q.score -= score;
                    return Task.FromResult(q);
                });
            }
            return skipCount==0?$"成功删除{idArray.Length-skipCount}道题": $"成功删除{idArray.Length - skipCount}道题,其中{skipCount}道题存在答题记录已跳过";
        }

        /// <summary>
        /// 根据id获取试题信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ChallengeOutDto> Get(string id)
        {
            var entity = _iChallengeQuestionRep.Get(new Guid(id));
            #region 验证

            if (entity == null)
            {
                throw new UserFriendlyException("无效的试题id");
            }
            #endregion

            var result = entity.MapTo<ChallengeOutDto>();
            result.standardCode = "";
            //参考答案
            var qsc= _iQuestionStandardCodeRep.FirstOrDefault(a => a.questionId.Equals(entity.Id) && a.isDefault == 1);
            result.standardCode =qsc?.code;
            //标签信息
            var labels = _iQuestionLabelRep.GetAll().Where(l => l.questionId.Equals(entity.Id)).ToList();
            result.label = labels.Where(l => l.labelType == 1).Select(l => l.labelId).ToList();
            result.secLabel = labels.Where(l => l.labelType == 0).Select(l => l.labelId).ToList();
            result.questionText = FilePathUtil.GetContentTextWithFilePath(result.Id.ToString(), "question", result.questionText, false);
            result.questionAnalysis = FilePathUtil.GetContentTextWithFilePath(result.Id.ToString(), "question", result.questionAnalysis, false);
            result.standardAnswer = FilePathUtil.GetContentTextWithFilePath(result.Id.ToString(), "question", result.standardAnswer, false);
            result.selectAnswer = FilePathUtil.GetContentTextWithFilePath(result.Id.ToString(), "question", result.selectAnswer, false);
            return await Task.FromResult(result);
        }
        
        /// <summary>
        /// 根据id获取子试题
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<QuestionItem>> GetChildPagination(QuestionPaginationInputDto input)
        {
            var entity = _iChallengeQuestionRep.Get(input.parentQuestionUid);
            #region 验证

            if (entity == null)
            {
                throw new UserFriendlyException("无效的试题id");
            }

            if (entity.questionBaseTypeCode != QuestionTypeConst.Compose)
            {
                throw new UserFriendlyException("不是组合题的id，不能获取子试题");
            }
            #endregion

            var rows = _iChallengeQuestionRep.GetAll().Where(a => a.parentQuestionUid == input.parentQuestionUid).OrderBy(a=>a.listOrder);
            return await Task.FromResult(new PaginationOutputDto<QuestionItem>()
            {
                rows = rows.Skip(input.skip).Take(input.pageSize).Select(a => new QuestionItem()
                {
                    Id = a.Id,
                    questionTypeUid = a.questionTypeUid,
                    questionCode = a.questionCode,
                    questionTypeName = a.QuestionType.questionTypeName,
                    parentQuestionUid = a.parentQuestionUid,
                    questionBaseTypeCode = a.questionBaseTypeCode,
                    questionText = a.questionText,
                    hardGrade = a.hardGrade,
                    score = a.score,
                    outdatedDate = a.outdatedDate, 
                    examTime = a.examTime,
                    questionStatusCode = a.questionStatusCode,
                    listOrder = a.listOrder
                }).ToList(),
                total = rows.Count()
            });
        }

        public async Task<List<ChallengeQuestion>> GetChildren(string parentUid)
        {
            var uid = new Guid(parentUid);
            var entity = _iChallengeQuestionRep.Get(uid);
            #region 验证

            if (entity == null)
            {
                throw new UserFriendlyException("无效的试题id");
            }

            if (entity.questionBaseTypeCode != QuestionTypeConst.Compose)
            {
                throw new UserFriendlyException("不是组合题的id，不能获取子试题");
            }
            #endregion

            var rows = _iChallengeQuestionRep.GetAll().Where(a => a.parentQuestionUid == uid).OrderBy(a => a.listOrder);

            return await Task.FromResult(rows.ToList());
        }

        public async Task<int> GetQuestionNum(QuestionNumInputDto input)
        {
             var count = _iChallengeQuestionRep.GetAll()
                 .Where(a=>a.parentQuestionUid == Guid.Empty && a.questionStatusCode == input.questionStatusCode)
                 .WhereIf(input.questionTypeUid != Guid.Empty, a=>a.questionTypeUid == input.questionTypeUid)
                .WhereIf(input.folderUids.Count > 0, a => input.folderUids.Contains(a.folderUid))
                .WhereIf(!string.IsNullOrEmpty(input.hardGrade), a => a.hardGrade == input.hardGrade)
                .Count();
            return await Task.FromResult(count);
        }
#endregion
        #region 前台首页挑战
        /// <summary>
        /// 获取挑战分类  首页显示目前只显示一级分类
        /// </summary>
        /// <returns></returns>
        public async Task<List<ChallengeFolderDto>> GetChallengeFolder()
        {
            var queryable = _iNvFolderRep.GetAll().Where(a => a.folderTypeCode == "challenge_cpp" && a.folderLevel==1).OrderBy(a=>a.listOrder).Select(a => new ChallengeFolderDto
            {
                id=a.Id,
                name=a.folderName
            });
            return await queryable.ToListAsync();
        }
        /// <summary>
        /// 获取挑战分数已经排名
        /// </summary>
        /// <returns></returns>
        public async Task<PointsRankDto> GetPointsAndRank()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            #region 验证登录
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            #endregion
            //取试题最高挑战分
            var data = await _iChallengeGradeRep.GetAll().GroupBy(a => new {a.userId, a.questionId})
                .Select(g => new {g.Key.userId, g.Key.questionId,score=g.Max(item=> item.score) }).ToListAsync();
            //统计总分
            var statisticalPoints = data.GroupBy(a => a.userId).Select(c => new {c.Key, totlaScore = c.Sum(item=> item.score)}).OrderByDescending(s=>s.totlaScore);
            //分数排名
            var pointList = statisticalPoints.Select(n => n.totlaScore).Distinct().ToList();
            //获取当前用户的分数  points.IndexOf方式获取排名
            var userPoint = statisticalPoints.FirstOrDefault(s => s.Key.Equals(cookie.UserUid.TryParseGuid())); 
            if (userPoint == null)
            {
                return new PointsRankDto
                {
                    points = 0,
                    rank = 0
                };
            }
            return new PointsRankDto { points= userPoint.totlaScore,rank = pointList.IndexOf(userPoint.totlaScore)+1 };
        }
        /// <summary>
        /// 获取挑战列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ChallengeQuestionViewModel> GetChallengeList(ChallengeInputDto input)
        {
            try
            {
                var cookie = CookieHelper.GetLoginInUserInfo();
                var uid = cookie.UserUid.TryParseGuid();
                //取试题最高挑战分
                var statisticalCg = _iChallengeGradeRep.GetAll().Where(c => c.userId.Equals(uid)).GroupBy(a => a.questionId)
                    .Select(g => new { g.Key, score = g.Max(item => item.score), hasPass = g.Max(item => item.isPass) });
                //挑战题提交次数 通过次数统计
                var statisticalData = _iChallengeGradeRep.GetAll().GroupBy(a => a.questionId)
                    .Select(g => new { g.Key, submitCount = g.Count(), passCount = g.Count(item => item.isPass == 1) });
                var query = from c in _iChallengeQuestionRep.GetAll().Where(q=>q.questionStatusCode.Equals("normal"))
                    join f in _iNvFolderRep.GetAll() on c.folderUid equals f.Id into folder
                    from nf in folder.DefaultIfEmpty()
                            join t in statisticalData on c.Id equals t.Key into temp
                    from tt in temp.DefaultIfEmpty()
                    join cg in statisticalCg on c.Id equals cg.Key into tempcg
                    from tcg in tempcg.DefaultIfEmpty()
                    select new ChallengeQuestionViewDto
                    {
                        folderPath = nf==null?"":nf.fullPath,
                        id = c.Id,
                        hard = c.hardGrade,
                        passRate = "0",
                        passNum = tt == null ? 0 : tt.passCount,
                        submitNum = tt == null ? 0 : tt.submitCount,
                        score = c.score,
                        title = c.title,
                        seq = c.listOrder,
                        status = tcg == null ? -1 : tcg.hasPass //-1 未挑战  0未通过 1通过
                    };
                //根据标签过滤
                if (!string.IsNullOrEmpty(input.label))
                {
                    var labelId = input.label.TryParseGuid();
                    query = from lc in _iQuestionLabelRep.GetAll()
                            .Where(l => l.labelId.Equals(labelId) && l.questionType.Equals("challenge"))
                        join q in query on lc.questionId equals q.id
                        select q;
                }
                var result = query.WhereIf(!string.IsNullOrEmpty(input.hard), q => q.hard.Equals(input.hard));
                result = result.WhereIf(!string.IsNullOrEmpty(input.folderId),
                    q => q.folderPath.Contains(input.folderId));
                result = input.isPass=="0" ? result.Where(q => q.status<=0) : result.WhereIf(!string.IsNullOrEmpty(input.isPass), q => q.status.ToString().Equals(input.isPass));
               
                var rows =  result.OrderBy("seq asc").Skip(input.skip).Take(input.pageSize).ToList();
                rows.ForEach(a =>
                {
                    a.passRate = a.submitNum == 0 ? "0" : ((decimal)a.passNum / a.submitNum * 100).ToString("f2");
                    a.hard = a.hard == "1" ? "容易" : (a.hard == "2" ? "中等" : "困难");
                });

                return new ChallengeQuestionViewModel
                {
                    ChallengeList = rows,
                    CurrentPage = input.currentPage,
                    Total = result.Count(),
                    PageSize = 10
                };
            }
            catch (Exception e)
            {
                _iLogger.Error(e.Message);
            }
            return new ChallengeQuestionViewModel();
        }
        /// <summary>
        /// 获取挑战详情
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public async Task<ProblemViewDto> GetProblem(Guid questionId)
        {
            //挑战题提交次数 通过次数统计
            var statistical = _iChallengeGradeRep.GetAll().Where(a=>a.questionId.Equals(questionId)).GroupBy(a => a.questionId)
                .Select(g => new { g.Key, submitCount = g.Count(), passCount = g.Count(item => item.isPass == 1) }).FirstOrDefaultAsync();
            var challenge =await _iChallengeQuestionRep.FirstOrDefaultAsync(questionId);
            var statisticalData= await statistical;
            return new ProblemViewDto
            {
                id= challenge.Id,
                hard = challenge.hardGrade == "1" ? "容易" : (challenge.hardGrade == "2" ? "中等" : "困难"),
                passNum= statisticalData?.passCount??0,
                submitNum = statisticalData?.submitCount??0,
                questionText=  FilePathUtil.GetContentTextWithFilePath(challenge.Id.ToString(), "question", challenge.questionText, false),
                score=challenge.score,
                seq=challenge.listOrder,
                title=challenge.title,
                answerTime=(int)DateTimeUtil.Now,
                hasParam = !string.IsNullOrWhiteSpace(challenge.param),
                hasInputParam = !string.IsNullOrWhiteSpace(challenge.InputParam),
                preinstallCode= Uri.EscapeDataString(challenge.PreinstallCode??"")
            };
        }
      
        /// <summary>
        /// 编译运行挑战
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProblemCompileResultDto> CompileRun(ProblemInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException(-1, "未登录系统或登录已经失效，请重新登录");
            }
          
            var compile = new ComplieOutDto<string>();
            if (string.IsNullOrWhiteSpace(input.Code))
            {
                return new ProblemCompileResultDto {code = 3 ,msg = "空白代码" };
            }
            var qId = input.id.TryParseGuid();
            var question = _iChallengeQuestionRep.FirstOrDefault(qId);
            if (question == null)
                throw new UserFriendlyException(0, "挑战题不存在");
            var result = new ProblemCompileResultDto { param = input.Param, inputParam = input.InputParam };
            //确定实际测试参数 不存在用户输入参数时取试题预设的参数
            input.Param = string.IsNullOrEmpty(input.Param) ? question.param.Replace("|", " ") : input.Param.Replace(Environment.NewLine, " ");
            input.InputParam = string.IsNullOrEmpty(input.InputParam) ? question.InputParam : input.InputParam.Replace(Environment.NewLine, " ");
            //多次测试
            if (question.MultiTest)
            {
               compile.Code = 0;
               var inputParams = input.InputParam.Split('|');
                foreach (var inputParam in inputParams)
                {
                    var compileParam = new CompileInputDto
                    {
                        Code = input.Code,
                        Language = question.language,
                        Param = string.IsNullOrEmpty(input.Param) ? question.param.Replace("|", " ") : input.Param,
                        InputParam = inputParam
                    };
                   var testRun = await _iCodeComplieService.Compile(compileParam);
                    compile.Result += testRun.Result + "|";
                    //遇到测试不通过 直接跳出给出提示
                    if (testRun.Code != 0)
                    {
                        compile = testRun;
                        compile.Result = "";
                        compile.Msg += string.IsNullOrEmpty(compileParam.Param)
                            ? ""
                            : "  Main函数参数:" + compileParam.Param;
                        compile.Msg += string.IsNullOrEmpty(compileParam.InputParam)
                            ? ""
                            : "  输入流参数:" + compileParam.InputParam;
                        break;
                    }

                }
                compile.Result = compile.Result.TrimEnd('|');
            }
            else
            {
                compile = await _iCodeComplieService.Compile(new CompileInputDto
                {
                    Code = input.Code,
                    Language = question.language,
                    Param = string.IsNullOrEmpty(input.Param) ? question.param.Replace("|", " ") : input.Param,
                    InputParam = string.IsNullOrEmpty(input.InputParam) ? question.InputParam : input.InputParam
                });
            }
            
            if (compile.Code == 0) //编译成功
            {
                if (compile.Result.Equals(question.standardAnswer))
                {
                    result.code =1;
                    result.compileResult = compile.Result;
                    result.answer = question.standardAnswer;

                }
                else
                {
                    result.code = 2;
                    result.compileResult = compile.Result;
                    result.answer = question.standardAnswer;
                    result.msg = "Wrong Answer";
                }
            }
            else //编译失败
            {
                result.code = 3;
                result.msg = compile.Msg;
  
            }
            return result;
        }
        //多次编译判分
        private async Task<ComplieOutDto<string>> MultiComplie(string code, string param, string inputParam, string language, int compileTimes = 0)
        {
            var result = await _iCodeComplieService.Compile(new CompileInputDto
            {
                Code = code,
                Language = language,
                Param = param,
                InputParam = inputParam
            });
      
            if (result.Code == 1)//启动进程失败
            {
                //启动进程失败 重新编译
                result=await MultiComplie(code, param, inputParam, language);
            }   
            else if (result.Code == 3)
            {

                //超时 记录次数  小于3次的继续编译一次
                if (compileTimes < 3)
                {
                    compileTimes++;
                    result= await MultiComplie(code, param, inputParam, language, compileTimes);

                }
                else
                    return result;

            }
            return result;
        }
        /// <summary>
        /// 提交挑战
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProblemSubmitResultDto> Submit(ProblemInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException(-1, "未登录系统或登录已经失效，请重新登录");
            }       
            var qId = input.id.TryParseGuid();
            var result = new ProblemSubmitResultDto();
            var compile = new ComplieOutDto<string>();
            if (string.IsNullOrWhiteSpace(input.Code))
            {
                result.code = 3;
                result.msg = "空白代码";
                return result;
            }
            var maxRate = float.Parse(BaseSiteSetDto.maxPointRate) / 100;
            var question = _iChallengeQuestionRep.FirstOrDefault(qId);
            if (question == null)
                throw new UserFriendlyException(0, "挑战题不存在");
            input.Param = question.param.Replace("|", " ");
            input.InputParam = question.InputParam;
           
            var challengeGrade = new ChallengeGrade
            {
                Id = Guid.NewGuid(),
                answer = input.Code,
                createTime = DateTime.Now,
                userId = cookie.UserUid.TryParseGuid(),
                questionId = qId,
                challengeTime = (int)DateTimeUtil.Now-input.AnswerTime, //服务器计算挑战时间
                isPass = 0
            };
            //多次测试
            if (question.MultiTest)
            {
                compile.Code = 0;
                var inputParams = input.InputParam.Split('|');
                foreach (var inputParam in inputParams)
                {
                    var testRun = await MultiComplie(input.Code, input.Param, inputParam,
                        question.language);
                    compile.Result += testRun.Result + "|";              
                    //遇到测试不通过 直接跳出给出提示
                    if (testRun.Code != 0)
                    {
                        compile = testRun;
                        break;
                    }
                }
                //运行成功的去除结果中多余的|
                if (compile.Code == 0)
                    compile.Result = compile.Result.TrimEnd('|');
            }
            else
            {
                compile = await _iCodeComplieService.Compile(new CompileInputDto
                {
                    Code = input.Code,
                    Language = question.language,
                    Param = input.Param,
                    InputParam = input.InputParam
                });
            }
            #region 检测代码是否直接包含输出答案
            bool CheckCodeFunc(Guid questionId, string code, string standardAnswer)
            {
                var returnResult = false; //作答代码校验结果 假设不含作弊代码
                ////检验作答代码是否直接输出答案
                //拆分参考答案检测是否多次输出拼装答案
                var standardAnswers = standardAnswer.Split('\n').Union(new[] { standardAnswer.Replace("\n","\\n") }).ToArray();
                foreach (var answer in standardAnswers)
                {
                    if(string.IsNullOrWhiteSpace(answer))
                        continue;
                    if ((code.Contains("'" + answer + "'") || code.Contains("\"" + answer + "\"")))
                        returnResult= true;

                }
                if (!returnResult)
                    return false;
          
                //检验参考代码是否直接输出答案 二次校验代码
                var standardCodeCheck = true; //参考代码校验结果 第一次校验已默认含作弊代码
                var standardCodes = _iQuestionStandardCodeRep.GetAll().Where(q => q.questionId.Equals(questionId)).Select(a => a.code)
                    .ToList();
                standardCodes.ForEach(c =>
                {
                    foreach (var answer in standardAnswers)
                    {
                        if (string.IsNullOrWhiteSpace(answer))
                            continue;
                        if (c.Contains("'" + answer + "'") || c.Contains("\"" + answer + "\""))
                            standardCodeCheck = false;

                    }
                });
                return standardCodeCheck;
            };
            #endregion
            #region 判分方法

            float ScoreAction(float score)
            {
                var dicAnserScores = new Dictionary<string, string>();
                var selectAnswers = question.selectAnswer.Split(new[] { "$#$" }, StringSplitOptions.None);
                var selectScores = question.selectAnswerScore.Split('|');
                for (int i = 0; i < selectAnswers.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(selectAnswers[i]) && !dicAnserScores.ContainsKey(selectAnswers[i]))
                        dicAnserScores.Add(selectAnswers[i], selectScores[i]);
                }
                dicAnserScores.ForEach(d =>
                {
                    //4:采分点匹配 直接获得采分点设置的得分比率 d.Value设置的是百分比正整数 10%=(d.Value=10)
                    if (challengeGrade.answer.Contains(d.Key))
                    {
                        score += float.Parse(d.Value) / 100;
                    }
                });
                return score> maxRate ? maxRate : score;
            }

            #endregion
            float scoreRate = 0;//得分比率
            if (compile.Code == 0) //编译成功
            {
                scoreRate = (float)0.1; //10%
                if (compile.Result.Equals(question.standardAnswer))
                {
                    if (!question.MultiTest && CheckCodeFunc(question.Id, input.Code, compile.Result))
                    {
                        result.code = 2;
                        challengeGrade.score = question.score * (decimal) ScoreAction(scoreRate);
                        result.msg = $"系统检测疑是作弊代码(如有疑问请联系授课教师)，挑战失败！（得分:{challengeGrade.score:f2}）";
                    }
                    else
                    {
                        result.code = 1;
                        challengeGrade.score = question.score;
                        challengeGrade.isPass = 1;
                        result.msg = $"恭喜你，通过了挑战！（得分:{question.score:f2}）";
                        //获取下一个挑战 已经挑战成功的跳过
                        var exists = (from cq in _iChallengeQuestionRep.GetAll()
                            join cg in _iChallengeGradeRep.GetAll() on cq.Id equals cg.questionId
                            where cg.userId == challengeGrade.userId && cg.isPass == 1
                            select cq).Distinct();
                        var next = _iChallengeQuestionRep.GetAll()
                            .Where(a => !a.Id.Equals(question.Id) && a.questionStatusCode.Equals("normal"))
                            .Where(a => !exists.Any(e => e.Id.Equals(a.Id)) && a.listOrder >= question.listOrder)
                            .OrderBy(a => a.listOrder)
                            .ThenBy(a => a.createTime).FirstOrDefault();
                        if (next != null)
                            result.id = next.Id.ToString();
                    }
                }
                else
                {
                    result.code = 2;
                    challengeGrade.score = question.score * (decimal)ScoreAction(scoreRate);
                    result.msg = $"Wrong Answer，挑战失败！（得分:{challengeGrade.score:f2}）";
                    result.result = compile.Result;
                    result.standardAnswer = question.standardAnswer;
                }
                challengeGrade.result = compile.Result;
            }
            else if (compile.Code == 1) //启动编译服务失败
            {
                throw new UserFriendlyException(1, "编译服务启动失败!请重新提交");
            }
            else if (compile.Code == 3) //超时记录
            {
                throw new UserFriendlyException(1, "代码运行超时，请检查是否有无限循环或递归代码。");
               
            }
            else //编译失败 按判分逻辑给分
            {
                result.code = 3;
                result.msg = compile.Msg;
                challengeGrade.score = question.score * (decimal) ScoreAction(scoreRate);
                result.score = challengeGrade.score;
                challengeGrade.result = compile.Msg;
            }
            await _iChallengeGradeRep.InsertAsync(challengeGrade);
            //创建答题记录
            await _iLibLabelService.CreateUserAnswerRecords(cookie.Id, question.Id, challengeGrade.Id, "challenge", challengeGrade.isPass==1);
            return result;
        }
        /// <summary>
        /// 排行榜分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<RankListViewDto> GetRankPagination(RankInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException(-1, "未登录系统或登录已经失效，请重新登录");
            }
            var uid = cookie.UserUid.TryParseGuid();
            var challengeId = input.id.TryParseGuid();
      
            //sql统计分数排行
            var sql =
                @"SELECT  0 as rank, a.userId,users.userFullName as userName, a.score, a.submitTimes,a.isPass,min(b.challengeTime) challengeTime
                      FROM (SELECT userId  , max(score) score,count(1) submitTimes,max(isPass) isPass FROM challenge_grade 
                 where questionId='{0}' GROUP BY userId  
                      )a  join challenge_grade b on a.userId=b.userId  and a.score=b.score
               join users on a.userId= users.Id   GROUP BY a.userId
               order by score desc,challengeTime asc";
            var statisticalQuery =   _iSqlExecuter.SqlQuery<RankSqlDto>(string.Format(sql, challengeId));

            var userRank = statisticalQuery.FirstOrDefault(a => a.userId.Equals(uid)).MapTo<ChallengeRankDto>();
            //得分+时间 便于排名
            var statisticalTotal =  statisticalQuery.Select(a =>new  { a.score,a.challengeTime}).Distinct().ToList();
            var statisticalData = statisticalQuery.Skip(input.skip).Take(input.pageSize).ToList();
            var rankList = new RankListViewDto
            {
                Total = statisticalQuery.Count(),
                CurrentPage = input.page
            };
            var challengeRankList = new List<ChallengeRankDto>();

            foreach (var data in statisticalData)
            {
                var challengeRank = data.MapTo<ChallengeRankDto>();
                challengeRank.rank = statisticalTotal.IndexOf(new {data.score, data.challengeTime}) + 1;
                challengeRank.time = DateTimeUtil.ConvertTimeStrFromSecond((int) challengeRank.challengeTime);
                challengeRankList.Add(challengeRank);
            }
            rankList.RankList = challengeRankList;
            if (userRank != null)
            {
                userRank.rank = statisticalTotal.IndexOf(new { userRank.score, userRank.challengeTime }) + 1;
                userRank.time = DateTimeUtil.ConvertTimeStrFromSecond((int) userRank.challengeTime);
            }
            rankList.Rank = userRank??new ChallengeRankDto(){rank = 0,time="—",challengeTime=0,isPass = 0,submitTimes=0,userName = cookie.UserName};
            return rankList;
        }
        /// <summary>
        /// 获取我的挑战提交记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SubmissionListViewDto> GetSubmissionPagination(RankInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException(-1, "未登录系统或登录已经失效，请重新登录");
            }
            var uid = cookie.UserUid.TryParseGuid();
            var challengeId = input.id.TryParseGuid();
            var challengeGrades = _iChallengeGradeRep.GetAll()
                .Where(g => g.userId.Equals(uid) && g.questionId.Equals(challengeId)).Select(g => new SubmissionDto
                {
                    id=g.Id,
                    challengeTime=g.challengeTime,
                    code=g.answer,
                    isPass=g.isPass,
                    score = g.score,
                    submiTime = g.createTime
                }).OrderByDescending(g=>g.submiTime);
            var challengeRankList = new SubmissionListViewDto
            {
                Total = challengeGrades.Count(),
                CurrentPage = input.page,
                SubmissionList = await  challengeGrades.Skip(input.skip).Take(input.pageSize).ToListAsync()
            };
            return challengeRankList;
        }

        /// <summary>
        /// 挑战排行总榜
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ChallengeLeaderboardView> ChallengeLeaderboard(RankInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var uid = cookie.UserUid.TryParseGuid();
            //sql统计挑战排行
            var sql =
                @"select CASE WHEN @rowtotal = score  THEN @rownum WHEN @rowtotal := score  THEN @rownum :=@rownum + 1 WHEN @rowtotal = 0 THEN @rownum :=@rownum + 1
                END AS  rank ,t.* from (select  sum(score) as score,a.userId,users.userFullName as userName,users.userLoginName as loginName,class.`name` as className,faculty.`name` as facultyName, major.`name` as majorName,class.`id` as classId from 
               (select (max(score)) score,userId from challenge_grade group by userId,questionId) a
               join users on a.userId= users.Id  left join student_info on a.userId= student_info.userId left join class on student_info.classId=class.id left join faculty on student_info.facultyId=faculty.id
               left join major on student_info.majorId=major.id GROUP BY userId) t ,(SELECT @rownum := 0 ,@rowtotal := NULL) r  order by t.score desc";
             var query = _iSqlExecuter.SqlQuery<ChallengeLeaderboardDto>(sql);
             var userRank = query.FirstOrDefault(a => a.userId.Equals(uid));
            var statisticalQuery = query.WhereIf(!string.IsNullOrWhiteSpace(input.classId),s=> input.classId.Contains(s.classId.ToString())).WhereIf(!string.IsNullOrWhiteSpace(input.userId), s => s.userId.Equals(new Guid(input.userId))).
                WhereIf(!string.IsNullOrWhiteSpace(input.userName), s => s.userName.Contains((input.userName))).
                WhereIf(!string.IsNullOrWhiteSpace(input.loginName), s => s.loginName.Contains((input.loginName)));
            input.skip = (input.page - 1) * input.pageSize;
             var leaderboardView = new ChallengeLeaderboardView
             {
                Total = statisticalQuery.Count(),
                RankList = statisticalQuery.Skip(input.skip).Take(input.pageSize).ToList(),    
               PointsRank =new PointsRankDto{points= userRank?.score ?? 0, rank= userRank?.rank??0}
             };
             return leaderboardView;
        }
        /// <summary>
        /// 获取挑战用户班级列表
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public async Task<List<ChallengeClassSelect>> GetChallengeClassList(string className)
        {
            var query =await (from cg in _iChallengeGradeRep.GetAll()
                join user in _iClassStudentRep.GetAll() on cg.userId equals user.UserId
                join classes in _iClassRep.GetAll() on user.ClassId equals classes.Id
                join major in _iMajorRep.GetAll() on classes.majorId equals major.Id
                              where classes.name.Contains(className)
                              select new
                {
                    majorId = major.Id,
                    classId = classes.Id,
                    majorName = major.name,
                    className = classes.name
                }).Distinct().ToListAsync();
            var majorList = query.Select(a => new {a.majorId, a.majorName}).Distinct().ToList();
            var list = new List<ChallengeClassSelect>();            
            foreach (var major in majorList)
            {
                var select = new ChallengeClassSelect();
                select.text = major.majorName;
                select.children = new List<SelectDto>();
                var classList = query.Where(a => a.majorId.Equals(major.majorId)).ToList();
                classList.ForEach(
                    a => select.children.Add(new SelectDto {id = a.classId.ToString(), text = a.className}));
                list.Add(select);
            }
            return list;
        }
        /// <summary>
        /// 获取挑战用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public  async Task<List<SelectDto>> GetChallengeUserList(string userName)
        {
            var query = await(from cu in _iChallengeGradeRep.GetAll()
                              join user in _iUserRep.GetAll() on cu.userId equals user.Id
                where user.userFullName.Contains(userName)
                              select new SelectDto
                {
                   id= user.Id.ToString(),
                   text = user.userFullName
                }).Distinct().ToListAsync();
          
            return query;
        }
        /// <summary>
        /// 获取用户挑战答题记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<UserAnswerRecordDto>> GetUserAnswerRecord(UserAnswerInputDto input)
        {
            var query = (from cg in _iChallengeGradeRep.GetAll().AsNoTracking()
                join c in _iChallengeQuestionRep.GetAll().AsNoTracking()
                on cg.questionId equals c.Id
                where cg.userId.Equals(input.userId)
                select new UserAnswerRecordDto
                {
                    gradeId = cg.Id,
                    hardGrade = c.hardGrade,
                    isPass = cg.isPass,
                    questionText = c.questionText,
                    title = c.title,
                    score = c.score,
                    userScore = cg.score,
                    createTime=cg.createTime
                }).OrderByDescending(s=>s.createTime);
            var statisticalQuery = query.WhereIf(!string.IsNullOrWhiteSpace(input.title), s => input.title.Contains(s.title)).
                WhereIf(!string.IsNullOrWhiteSpace(input.questionText), s => input.questionText.Contains(s.questionText)).
                WhereIf(!string.IsNullOrWhiteSpace(input.hardGrade), s => s.hardGrade.Equals(input.hardGrade));
            input.skip = (input.page - 1) * input.pageSize;
            
           return await  Task.FromResult( new PaginationOutputDto<UserAnswerRecordDto>
           {
                total = statisticalQuery.Count(),
                rows = statisticalQuery.Skip(input.skip).Take(input.pageSize).ToList(),
               
            });
        }

        #endregion
        public ImportResultOutputDto CreateFromFile(Stream fileStream, Guid folderUid, string fileType)
        {
            
            if (!_iNvFolderRep.GetAll().Any(a => a.Id == folderUid && a.folderTypeCode == "challenge_cpp"))
            {
                throw new UserFriendlyException("无效的分类");
            }
            if (fileType != "word" && fileType != "excel")
            {
                throw new UserFriendlyException("无效的文件格式");
            }
            var result = new ImportResultOutputDto();
            try
            {
                var rootPath = AppConfiguration.WebServerFileRootPath;
                //todo 导入操作
                var helper = new ImportChallengeHelper(rootPath, folderUid, _iNvFolderRep, _iChallengeQuestionRep, _iExamQuestionTypeRep, _iUnitOfWorkManager,_iQuestionStandardCodeRep, L("Language"),NewMoocApiUrl, _iLabelRep, _iQuestionLabelRep);
                int successCount;
                string errMessage;
                if (fileType == "word")
                {
                    helper.ImportFromWord(fileStream, out successCount, out errMessage);
                }
                else
                {
                    helper.ImportFromExcel(fileStream, out successCount, out errMessage);
                }
                result = new ImportResultOutputDto
                {
                    successCount = successCount,
                    errMessage = errMessage
                };
                return result;
            }
            catch (Exception e)
            {
                var guidNum = Guid.NewGuid().GetHashCode();
                result.errMessage = "发生未知错误，请联系管理员，错误编码：["+guidNum +"]";
                _iLogger.Error("[" + guidNum + "]" + e.StackTrace);
            }

            return result;
        }

        public async Task<Guid> ExportToWord(IdListInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }

            var questions = await _iChallengeQuestionRep.GetAll().Where(a => input.idList.Contains(a.Id)).ToListAsync();
            var folders = await _iNvFolderRep.GetAll().Where(a => a.folderTypeCode == "challenge_cpp").ToListAsync();
            var questionTypeDic = await _iExamQuestionTypeRep.GetAll().ToDictionaryAsync(a => a.Id, a => a);
            var reValue = ExamImportAndExportUtil.ConvertQuestionToText(questions.MapTo<List<ExamQuestionDto>>(), EnumExportTextType.Word,
                questionTypeDic, folders, null,_iQuestionStandardCodeRep,_iQuestionLabelRep,_iLabelRep);
            var htmlStr = reValue.ReturnObject.ToString();
            htmlStr = StringUtil.ReplaceEnter2BrWhenNoHtml(htmlStr) + "<br>";

            var doc = new Document();
            var builder = new DocumentBuilder(doc);
            builder.InsertHtml(htmlStr);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fileroot", "CacheFile", "ExportQuestion");
            FilePathUtil.CreateDirectoryIfNotExists(path);
            var dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            //删除超过1天的缓存文件
            foreach (var fileInfo in files)
            {
                if ((DateTime.Now - fileInfo.CreationTime).TotalHours > 24)
                {
                    fileInfo.Delete();
                }
            }

            var guid = Guid.NewGuid();
            path += "\\" + cookie.Id + "_" + guid + ".doc";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            doc.Save(path, SaveFormat.Doc);
            return guid;
        }

        private string CreateNewCode()
        {
            var code = "Q000001";
            var entity = _iChallengeQuestionRep.GetAll().Where(a => !a.isCustomCode).OrderByDescending(a => a.createTime).FirstOrDefault();
            if (entity != null)
            {
                code = entity.questionCode;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (_iChallengeQuestionRep.GetAll().Any(a=>a.questionCode == code));
            }
            return code;
        }

        
    }

}