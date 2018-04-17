using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using SPOC.Category;
using SPOC.Common.Cookie;
using SPOC.Exam;
using SPOC.Lib;
using SPOC.PolicyPaper.Dto;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace SPOC.PolicyPaper
{
    /// <summary>
    /// 随机试卷基本信息服务
    /// </summary>
    public class ExamPolicyItemService:ApplicationService, IExamPolicyItemService
    {
        private readonly IRepository<NvFolder, Guid> _iNvFolderRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ExamPolicy, Guid> _iExamPolicyRep;
        private readonly IRepository<ExamPolicyNode, Guid> _iExamPolicyNodeRep;
        private readonly IRepository<ExamPolicyItem, Guid> _iExamPolicyItemRep;
        private readonly IRepository<Exam.ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<ExamQuestionType, Guid> _iExamQuestionTypeRep;
        private readonly IRepository<ExamPolicyItemLabel, Guid> _iExamPolicyItemLabelRep;
        private readonly IRepository<Label, Guid> _iLabelRep;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iExamPolicyItemRep"></param>
        public ExamPolicyItemService(IRepository<ExamPolicyItem, Guid> iExamPolicyItemRep, IRepository<ExamPolicyNode, Guid> iExamPolicyNodeRep, 
            IRepository<TeacherInfo, Guid> iTeacherInfoRep, IRepository<NvFolder, Guid> iNvFolderRep, 
            IRepository<Exam.ExamPaper, Guid> iExamPaperRep, IRepository<ExamPolicy, Guid> iExamPolicyRep, 
            IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep, IUnitOfWorkManager iUnitOfWorkManager, 
            IRepository<ExamPolicyItemLabel, Guid> iExamPolicyItemLabelRep, IRepository<Label, Guid> iLabelRep)
        {
            _iExamPolicyItemRep = iExamPolicyItemRep;
            _iExamPolicyNodeRep = iExamPolicyNodeRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iNvFolderRep = iNvFolderRep;
            _iExamPaperRep = iExamPaperRep;
            _iExamPolicyRep = iExamPolicyRep;
            _iExamQuestionTypeRep = iExamQuestionTypeRep;
            _iUnitOfWorkManager = iUnitOfWorkManager;
            _iExamPolicyItemLabelRep = iExamPolicyItemLabelRep;
            _iLabelRep = iLabelRep;
        }

        public async Task<List<ExamPolicyItemOutputDto>> GetList(Guid policyNodeUid)
        {
            var queryable = _iExamPolicyItemRep.GetAll()
                .Where(a => a.policyNodeUid == policyNodeUid);
            var questionTypeUid = _iExamPolicyNodeRep.GetAll()
                .Where(a => a.Id == policyNodeUid)
                .Select(a => a.questionTypeUid)
                .FirstOrDefault();
            var labelList = await (from itemLabel in _iExamPolicyItemLabelRep.GetAll()
                join label in _iLabelRep.GetAll() on itemLabel.LabelId equals label.Id
                select new { label.Id, label.title }).ToListAsync();

            var questionTypeName = "全部题型";
            if (questionTypeUid != Guid.Empty)
            {
                questionTypeName =
                    _iExamQuestionTypeRep.GetAll()
                        .Where(a => a.Id == questionTypeUid)
                        .Select(a => a.questionTypeName)
                        .FirstOrDefault();
            }
            var items = queryable.MapTo<List<ExamPolicyItemOutputDto>>();
            items.ForEach(item =>
            {
                item.questionTypeUid = questionTypeUid;
                item.questionTypeName = questionTypeName;
                item.labelIdList = labelList.Select(a => a.Id).ToList();
                item.labelList = labelList.Select(a => a.title).ToList();
            });

            return await Task.FromResult(items);
        }

        public async Task<ExamPolicyItemOutputDto> Create(ExamPolicyItemInputDto input)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (!_iExamPolicyNodeRep.GetAll().Any(a => a.Id == input.policyNodeUid))
            {
                throw new UserFriendlyException("无效的大题ID");
            }

            if (!string.IsNullOrEmpty(input.folderUid))
            {
                var folderUids = input.folderUid.Split(',').Select(a=>new Guid(a)).ToArray();
                foreach (var folderUid in folderUids)
                {
                    if (!_iNvFolderRep.GetAll().Any(a => a.Id == folderUid && a.folderTypeCode == "question_bank"))
                    {
                        throw new UserFriendlyException("无效的试题分类");
                    }
                }
            }

            foreach (var labelId in input.labelIdList)
            {
                if (await _iLabelRep.GetAll().AllAsync(a => a.Id != labelId))
                {
                    throw new UserFriendlyException("无效的知识点");
                }
            }
            #endregion

            var entity = input.MapTo<ExamPolicyItem>();
            entity.Id = Guid.NewGuid();
            var items = _iExamPolicyItemRep.GetAll().Where(a => a.policyNodeUid == input.policyNodeUid);
            var max = await items.AnyAsync() ? await items.MaxAsync(a => a.listOrder) : 0;
            entity.listOrder = max + 1;
            await _iExamPolicyItemRep.InsertAsync(entity);
            _iUnitOfWorkManager.Current.SaveChanges();

            if (entity.questionScore > 0 || entity.questionNum > 0)
            {
                await UpdateScoreAndNum(entity.Id, entity.questionScore * entity.questionNum, entity.questionNum);
            }

            foreach (var labelId in input.labelIdList)
            {
                await _iExamPolicyItemLabelRep.InsertAsync(new ExamPolicyItemLabel
                {
                    Id = Guid.NewGuid(),
                    ItemId = entity.Id,
                    LabelId = labelId
                });
            }
            return await Task.FromResult(entity.MapTo<ExamPolicyItemOutputDto>());
        }

        public async Task Update(ExamPolicyItemInputDto input)
        {
            #region 验证
            var cookie = CookieHelper.GetLoginInUserInfo();

            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            var entity = _iExamPolicyItemRep.GetAll().FirstOrDefault(a => a.Id == input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的随机试题策略项");
            }

            if (!_iExamPolicyNodeRep.GetAll().Any(a => a.Id == input.policyNodeUid))
            {
                throw new UserFriendlyException("无效的大题ID");
            }

            if (!string.IsNullOrEmpty(input.folderUid))
            {
                var folderUids = input.folderUid.Split(',').Select(a=>new Guid(a)).ToArray();
                foreach (var folderUid in folderUids)
                {
                    if (!_iNvFolderRep.GetAll().Any(a => a.Id == folderUid && a.folderTypeCode == "question_bank"))
                    {
                        throw new UserFriendlyException("无效的试题分类");
                    }
                }
            }

            foreach (var labelId in input.labelIdList)
            {
                if (await _iLabelRep.GetAll().AllAsync(a => a.Id != labelId))
                {
                    throw new UserFriendlyException("无效的知识点");
                }
            }
            #endregion

            var score = input.questionScore * input.questionNum - entity.questionScore * entity.questionNum;
            var num = input.questionNum - entity.questionNum;
            input.MapTo(entity);
            await _iExamPolicyItemRep.UpdateAsync(entity);
            _iUnitOfWorkManager.Current.SaveChanges();
            if (score != 0 || num != 0)
            {
                await UpdateScoreAndNum(entity.Id, score, num);
            }

            var itemLabelList = await _iExamPolicyItemLabelRep.GetAll().Where(a => a.ItemId == entity.Id).ToListAsync();
            var labelIdList = itemLabelList.Select(a => a.LabelId).ToList();
            if (input.labelIdList.Count == itemLabelList.Count && labelIdList.All(input.labelIdList.Contains))
            {
                return;
            }
            //找出删除的知识点
            var delIdList = labelIdList.Where(a => !input.labelIdList.Contains(a)).ToList();
            if (delIdList.Any())
            {
                await _iExamPolicyItemLabelRep.DeleteAsync(a => a.ItemId == entity.Id && delIdList.Contains(a.LabelId));
            }
            //找出新增的知识点
            var addIdList = input.labelIdList.Where(a => !labelIdList.Contains(a)).ToList();
            foreach (var labelId in addIdList)
            {
                await _iExamPolicyItemLabelRep.InsertAsync(new ExamPolicyItemLabel()
                {
                    Id = Guid.NewGuid(),
                    ItemId = entity.Id,
                    LabelId = labelId
                });
            }
        }

        public async Task Delete(string ids)
        {
            #region 验证
            var cookie = CookieHelper.GetLoginInUserInfo();

            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            var idArray = ids.Split(',');
            var guidList = new List<Guid>();
            var score = 0m;
            var num = 0;
            foreach (var idStr in idArray)
            {
                var guid = new Guid(idStr);
                var obj =
                    _iExamPolicyItemRep.GetAll()
                        .Where(a => a.Id == guid)
                        .Select(a => new {a.questionScore, a.questionNum})
                        .FirstOrDefault();
                if (obj == null)
                {
                    throw new UserFriendlyException("无效的id");
                }
                guidList.Add(guid);
                score -= obj.questionScore * obj.questionNum;
                num -= obj.questionNum;
            }
            #endregion

            foreach (var guid in guidList)
            {
                await _iExamPolicyItemRep.DeleteAsync(guid);
            }
            _iUnitOfWorkManager.Current.SaveChanges();
            if (score != 0 || num != 0)
            {
                await UpdateScoreAndNum(guidList[0], score, num);//因只靠id取policyNode，并不需要policyItem参与，所以只取一个id就够了
            }
        }

        private async Task UpdateScoreAndNum(Guid id, decimal score, int questionNum)
        {
            var queryObj = _iExamPolicyItemRep.GetAll().Where(a => a.Id == id).Select(a => new {a.PolicyNode, a.PolicyNode.Policy}).FirstOrDefault();
            if (queryObj == null)
            {
                return;
            }
            queryObj.Policy.totalScore += score;
            queryObj.Policy.policyTotalScore = queryObj.Policy.totalScore;
            queryObj.Policy.questionNum += questionNum;
            queryObj.PolicyNode.totalScore += score;
            queryObj.PolicyNode.questionNum += questionNum;

            var paper = _iExamPaperRep.Get(queryObj.Policy.Id);
            paper.totalScore = queryObj.Policy.totalScore;
            paper.questionNum = queryObj.Policy.questionNum;

            await _iExamPolicyRep.UpdateAsync(queryObj.Policy);
            await _iExamPolicyNodeRep.UpdateAsync(queryObj.PolicyNode);
            await _iExamPaperRep.UpdateAsync(paper);
        }

        public async Task<List<ExamPolicyItemItem>> GetAllList(ExamPolicyItemListQueryInputDto input)
        {
            var result = await (from item in _iExamPolicyItemRep.GetAll()
                                join node in _iExamPolicyNodeRep.GetAll() on item.policyNodeUid equals node.Id
                                join type in _iExamQuestionTypeRep.GetAll() on item.questionTypeUid equals type.Id into typeTempTable
                                from typeTemp in typeTempTable.DefaultIfEmpty()
                                where node.policyUid == input.PolicyId
                                && (input.NodeId == Guid.Empty || item.policyNodeUid == input.NodeId)
                                && (
                                    (string.IsNullOrEmpty(input.QuestionBaseTypeCode) || (input.QuestionBaseTypeCode != "all" && typeTemp.questionBaseTypeCode == input.QuestionBaseTypeCode))
                                    || 
                                    (input.QuestionBaseTypeCode == "all" && item.questionTypeUid == Guid.Empty)
                                )
                                select new ExamPolicyItemItem
                                {
                                    Id = item.Id,
                                    PolicyNodeUid = item.policyNodeUid,
                                    PolicyNodeName = node.policyNodeName,
                                    QuestionTypeUid = item.questionTypeUid,
                                    QuestionTypeName = typeTemp == null ? "不限题型" : typeTemp.questionTypeName,
                                    FolderUid = item.folderUid,
                                    FolderName = item.folderName,
                                    QuestionScore = item.questionScore,
                                    QuestionNum = item.questionNum,
                                    HardGrade = item.hardGrade,
                                    ListOrder = item.listOrder
                                }).ToListAsync();

            var labelList = await (from itemLabel in _iExamPolicyItemLabelRep.GetAll()
                                   join label in _iLabelRep.GetAll() on itemLabel.LabelId equals label.Id
                                   select new { label.Id, label.title }).ToListAsync();
            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                result = result.OrderBy(a => a.PolicyNodeName).ThenBy(a=>a.ListOrder).ToList();
            }
            else
            {
                result = result.OrderBy(input.OrderExpression).ToList();
            }
            result.ForEach(item =>
            {
                item.LabelIdList = labelList.Select(a => a.Id).ToList();
                item.LabelList = labelList.Select(a => a.title).ToList();
            });

            return result;
        }
        
    }
}