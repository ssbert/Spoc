using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Pagination;
using SPOC.Exam;
using SPOC.PolicyPaper.Dto;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using System.Data.Entity;

namespace SPOC.PolicyPaper
{
    /// <summary>
    /// 随机试卷大题服务
    /// </summary>
    public class ExamPolicyNodeService:ApplicationService, IExamPolicyNodeService
    {
        private readonly IRepository<Exam.ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ExamPolicy, Guid> _iExamPolicyRep;
        private readonly IRepository<ExamPolicyNode, Guid> _iExamPolicyNodeRep;
        private readonly IRepository<ExamPolicyItem, Guid> _iExamPolicyItemRep;
        private readonly IRepository<ExamQuestionType, Guid> _iExamQuestionTypeRep;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;


        public ExamPolicyNodeService(IRepository<ExamPolicyNode, Guid> iExamPolicyNodeRep, IRepository<TeacherInfo, Guid> iTeacherInfoRep, IRepository<Exam.ExamPaper, Guid> iExamPaperRep, IRepository<ExamPolicy, Guid> iExamPolicyRep, IRepository<ExamPolicyItem, Guid> iExamPolicyItemRep, IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep, IUnitOfWorkManager iUnitOfWorkManager)
        {
            _iExamPolicyNodeRep = iExamPolicyNodeRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iExamPaperRep = iExamPaperRep;
            _iExamPolicyRep = iExamPolicyRep;
            _iExamPolicyItemRep = iExamPolicyItemRep;
            _iExamQuestionTypeRep = iExamQuestionTypeRep;
            _iUnitOfWorkManager = iUnitOfWorkManager;
        }

        public async Task<ExamPolicyNodeOutputDto> Create(ExamPolicyNodeInputDto input)
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
            #endregion
            input.Id = Guid.NewGuid();
            var entity = input.MapTo<ExamPolicyNode>();
            var query = _iExamPolicyNodeRep.GetAll().Where(a => a.policyUid == input.policyUid);
            entity.listOrder = query.Any() ? query.Select(a => a.listOrder).Max() + 1: 0;
            await _iExamPolicyNodeRep.InsertAsync(entity);
            return await Task.FromResult(entity.MapTo<ExamPolicyNodeOutputDto>());
        }

        public async Task Update(ExamPolicyNodeInputDto input)
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

            var entity = _iExamPolicyNodeRep.GetAll().FirstOrDefault(a => a.Id == input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的随机试卷大题");
            }

            if (entity.questionNum > 0 && entity.questionTypeUid != input.questionTypeUid)
            {
                throw new UserFriendlyException("已添加策略，无法修改试题类型");
            }
            #endregion

            input.MapTo(entity);

            await _iExamPolicyNodeRep.UpdateAsync(entity);
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

            var idArray = ids.Split(',').Select(a=>new Guid(a)).ToArray();
            foreach (var id in idArray)
            {
                if (!_iExamPolicyNodeRep.GetAll().Any(a => a.Id == id))
                {
                    throw new UserFriendlyException("无效的大题");
                }
            }
            #endregion

            var score = 0m;
            var num = 0;

            foreach (var id in idArray)
            {
                var guid = id;
                await _iExamPolicyNodeRep.DeleteAsync(a => a.Id == guid);
                var itemScoreQueryAble = _iExamPolicyItemRep.GetAll()
                    .Where(a => a.policyNodeUid == guid)
                    .Select(a => a.questionScore);
                if (itemScoreQueryAble.Any())
                {
                    score -= itemScoreQueryAble.Sum();
                    num -= itemScoreQueryAble.Count();
                }
            }
            _iUnitOfWorkManager.Current.SaveChanges();
            if (score != 0 || num != 0)
            {
                await UpdateScoreAndNum(idArray[0], score, num);
            }
        }

        public async Task<ExamPolicyNodeOutputDto> Get(Guid id)
        {
            return await Task.FromResult(_iExamPolicyNodeRep.Get(id).MapTo<ExamPolicyNodeOutputDto>());
        }

        public async Task<PaginationOutputDto<ExamPolicyNodeOutputDto>> GetPagination(ExamPolicyNodePaginationInputDto input)
        {
            var query = _iExamPolicyNodeRep.GetAll().Where(a => a.policyUid == input.policyUid).OrderBy(a=>a.listOrder);
            var questionTypeDic =
                _iExamQuestionTypeRep.GetAll().Select(a => new {a.Id, a.questionTypeName}).ToDictionary(a => a.Id, a=>a.questionTypeName);
            var rows = query.Skip(input.skip).Take(input.pageSize).MapTo<List<ExamPolicyNodeOutputDto>>();
            rows.ForEach(a =>
            {
                if (a.questionTypeUid != Guid.Empty)
                {
                    a.questionTypeName = questionTypeDic[a.questionTypeUid];
                }
            });
            var dto = new PaginationOutputDto<ExamPolicyNodeOutputDto>
            {
                rows = rows,
                total = query.Count()
            };
            return await Task.FromResult(dto);
        }

        /// <summary>
        /// 获取所有大题数据
        /// </summary>
        /// <param name="policyId"></param>
        /// <returns></returns>
        public async Task<List<ExamPolicyNodeOutputDto>> GetList(Guid policyId)
        {
            var query = _iExamPolicyNodeRep.GetAll().Where(a => a.policyUid == policyId).OrderBy(a => a.listOrder);
            var questionTypeDic = await
                _iExamQuestionTypeRep.GetAll().Select(a => new { a.Id, a.questionTypeName }).ToDictionaryAsync(a => a.Id, a => a.questionTypeName);
            var result = query.MapTo<List<ExamPolicyNodeOutputDto>>();
            result.ForEach(a =>
            {
                if (a.questionTypeUid != Guid.Empty)
                {
                    a.questionTypeName = questionTypeDic[a.questionTypeUid];
                }
            });
            return result;
        }

        private async Task UpdateScoreAndNum(Guid id, decimal score, int questionNum)
        {
            var policy =
                _iExamPolicyNodeRep.GetAll()
                    .Where(a => a.Id == id)
                    .Select(a => a.Policy)
                    .FirstOrDefault();
            if (policy == null)
            {
                return;
            }
            policy.totalScore += score;
            policy.policyTotalScore = policy.totalScore;
            policy.questionNum += questionNum;
            

            var paper = _iExamPaperRep.Get(policy.Id);
            paper.totalScore = policy.totalScore;
            paper.questionNum = policy.questionNum;

            await _iExamPolicyRep.UpdateAsync(policy);
            await _iExamPaperRep.UpdateAsync(paper);
        }
    }
}