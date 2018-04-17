using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Exam;
using SPOC.ExamPaper.Dto;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;

namespace SPOC.ExamPaper
{
    public class ExamPaperNodeService: ApplicationService, IExamPaperNodeService
    {

        private readonly IRepository<ExamPaperNode, Guid> _iExamPaperNodeRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<Exam.ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<ExamQuestionType, Guid> _iExamQuestionTypeRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IRepository<ExamPaperNodeQuestion, Guid> _iExamPaperNodeQuestionRep;
        private readonly IExamPaperService _iExamPaperService;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;

        public ExamPaperNodeService(IRepository<ExamPaperNode, Guid> iExamPaperNodeRep, 
            IRepository<TeacherInfo, Guid> iTeacherInfoRep, 
            IRepository<Exam.ExamPaper, Guid> iExamPaperRep, 
            IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep, 
            IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep,
            IExamPaperService iExamPaperService, IRepository<ExamQuestion, Guid> iExamQuestionRep, 
            IUnitOfWorkManager iUnitOfWorkManager)
        {
            _iExamPaperNodeRep = iExamPaperNodeRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iExamPaperRep = iExamPaperRep;
            _iExamQuestionTypeRep = iExamQuestionTypeRep;
            _iExamPaperNodeQuestionRep = iExamPaperNodeQuestionRep;
            _iExamPaperService = iExamPaperService;
            _iExamQuestionRep = iExamQuestionRep;
            _iUnitOfWorkManager = iUnitOfWorkManager;
        }

        public async Task Delete(string ids)
        {
            var idArray = ids.Split(',').Select(a=>new Guid(a)).ToArray();
            var cookie = CookieHelper.GetLoginInUserInfo();
            #region 验证
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            foreach (var id in idArray)
            {
                if (!_iExamPaperNodeRep.GetAll().Any(a => a.Id == id))
                {
                    throw new UserFriendlyException("无效的试卷大题");
                }
            }
            #endregion

            var score = (decimal)0;
            Guid paperUid = Guid.Empty;
            var questionCount = 0;
            foreach (var uid in idArray)
            {
                var guid = uid;
                questionCount += await _iExamPaperNodeQuestionRep.CountAsync(a => a.paperNodeUid == guid);
                await _iExamPaperNodeQuestionRep.DeleteAsync(a => a.paperNodeUid == guid);
                var entity = _iExamPaperNodeRep.Get(guid);
                score += entity.totalScore;
                paperUid = entity.paperUid;
                await _iExamPaperNodeRep.DeleteAsync(entity);
            }
            await _iExamPaperService.UpdateTotalScoreAndQuestionNum(paperUid, -score, -questionCount);
            _iUnitOfWorkManager.Current.SaveChanges();
            await _iExamPaperService.BuidExamPaper(paperUid);
        }

        public async Task Update(ExamPaperNodeInputDto input)
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

            if (!_iExamPaperNodeRep.GetAll().Any(a => a.Id == input.Id))
            {
                throw new UserFriendlyException("无效的试卷大题");
            }

            if (!_iExamPaperRep.GetAll().Any(a => a.Id == input.paperUid))
            {
                throw new UserFriendlyException("无效的试卷");
            }

            if (input.questionTypeUid != Guid.Empty && !_iExamQuestionTypeRep.GetAll().Any(a => a.Id == input.questionTypeUid))
            {
                throw new UserFriendlyException("无效的试题类型");
            }
            #endregion

            var entity = _iExamPaperNodeRep.Get(input.Id);
            var questionScore = entity.questionScore;
            input.MapTo(entity);
            var score = 0m;
            if (questionScore != input.questionScore)
            {
                var questionList = _iExamPaperNodeQuestionRep.GetAll().Where(a => a.paperNodeUid == input.Id)
                    .Join(_iExamQuestionRep.GetAll(), nq=>nq.questionUid, q=>q.Id, (nq, q)=>new {nodeQuestion=nq, question=q})
                    .Where(a=>a.question.parentQuestionUid == Guid.Empty).ToList();//where是用来排除组合题里子试题的
                var totalScore = 0m;
                questionList.ForEach(a =>
                {
                    if (entity.questionScore == 0)
                    {
                        score += a.question.score - a.nodeQuestion.paperQuestionScore;
                        a.nodeQuestion.paperQuestionScore = a.question.score;
                        totalScore += a.question.score;
                    }
                    else
                    {
                        score += input.questionScore - a.nodeQuestion.paperQuestionScore;
                        a.nodeQuestion.paperQuestionScore = input.questionScore;    
                        totalScore += input.questionScore;
                    }
                    
                    _iExamPaperNodeQuestionRep.UpdateAsync(a.nodeQuestion);
                });
                entity.totalScore = totalScore;
            }
            if (score != 0)
            {
                await _iExamPaperService.UpdateTotalScoreAndQuestionNum(entity.paperUid, score, 0);
            }
            await _iExamPaperNodeRep.UpdateAsync(entity);
            _iUnitOfWorkManager.Current.SaveChanges();
            await _iExamPaperService.BuidExamPaper(entity.paperUid);
        }

        public async Task<ExamPaperNodeOutputDto> Create(ExamPaperNodeInputDto input)
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

            if (!_iExamPaperRep.GetAll().Any(a => a.Id == input.paperUid))
            {
                throw new UserFriendlyException("无效的试卷");
            }

            if (input.questionTypeUid != Guid.Empty && !_iExamQuestionTypeRep.GetAll().Any(a => a.Id == input.questionTypeUid))
            {
                throw new UserFriendlyException("无效的试题类型");
            }
            #endregion

            var entity = input.MapTo<ExamPaperNode>();
            entity.Id = Guid.NewGuid();
            await _iExamPaperNodeRep.InsertAsync(entity);
            if (entity.totalScore != 0)
            {
                await _iExamPaperService.UpdateTotalScoreAndQuestionNum(input.paperUid, entity.totalScore, 0);
            }
            _iUnitOfWorkManager.Current.SaveChanges();
            await _iExamPaperService.BuidExamPaper(entity.paperUid);
            return await Task.FromResult(entity.MapTo<ExamPaperNodeOutputDto>());
        }

        public async Task<List<ExamPaperNodeOutputDto>> GetList(Guid paperUid)
        {
            var queryable = (from pn in _iExamPaperNodeRep.GetAll()
                join qt in _iExamQuestionTypeRep.GetAll() on pn.questionTypeUid equals qt.Id into qtTempTable
                from qtTemp in qtTempTable.DefaultIfEmpty()
                where pn.paperUid == paperUid
                orderby pn.listOrder
                select new
                {
                    paperNode = pn,
                    questionBaseTypeCode = qtTemp == null ? "" : qtTemp.questionBaseTypeCode,
                    questionTypeName = qtTemp == null ? "综合题" : qtTemp.questionTypeName
                });
            var dtos = new List<ExamPaperNodeOutputDto>();
            foreach (var paperNode in queryable)
            {
                var dto = paperNode.paperNode.MapTo<ExamPaperNodeOutputDto>();
                dto.questionBaseTypeCode = paperNode.questionBaseTypeCode;
                dto.questionTypeName = paperNode.questionTypeName;
                dtos.Add(dto);
            }
            return await Task.FromResult(dtos);
        }

        public async Task<ExamPaperNodeOutputDto> Get(Guid id)
        {
            var entity = (from pn in _iExamPaperNodeRep.GetAll()
                join qt in _iExamQuestionTypeRep.GetAll() on pn.questionTypeUid equals qt.Id
                          where pn.Id == id
                orderby pn.listOrder
                select new
                {
                    paperNode = pn,
                    qt.questionBaseTypeCode,
                }).FirstOrDefault();
            var outputDto = entity.paperNode.MapTo<ExamPaperNodeOutputDto>();
            outputDto.questionBaseTypeCode = entity.questionBaseTypeCode;
            return await Task.FromResult(outputDto);
        }

        public async Task UpdateTotalScoreAndQuestionNum(Guid id, decimal score, int questionNum)
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

            var entity = _iExamPaperNodeRep.Get(id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的试卷大题");
            }
            #endregion 
            entity.totalScore += score;
            entity.questionNum += questionNum;

            await _iExamPaperService.UpdateTotalScoreAndQuestionNum(entity.paperUid, score, questionNum);
            
            await _iExamPaperNodeRep.UpdateAsync(entity);
        }
    }
}

