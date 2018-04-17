using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Exam;
using SPOC.ExamPaper.Dto;
using SPOC.QuestionBank.Const;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;

namespace SPOC.ExamPaper
{
    public class ExamPaperNodeQuestionService:ApplicationService, IExamPaperNodeQuestionService
    {
        private readonly IRepository<ExamPaperNodeQuestion, Guid> _iExamPaperNodeQuestionRep;
        private readonly IRepository<ExamPaperNode, Guid> _iExamPaperNodeRep;
        private readonly IRepository<Exam.ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IExamPaperNodeService _iExamPaperNodeService;
        private readonly IExamPaperService _iExamPaperService;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;

        public ExamPaperNodeQuestionService(IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep, 
            IRepository<ExamPaperNode, Guid> iExamPaperNodeRep, 
            IRepository<Exam.ExamPaper, Guid> iExamPaperRep, 
            IRepository<TeacherInfo, Guid> iTeacherInfoRep, 
            IRepository<ExamQuestion, Guid> iExamQuestionRep,
            IExamPaperNodeService iExamPaperNodeService,
            IExamPaperService iExamPaperService, 
            IUnitOfWorkManager iUnitOfWorkManager)
        {
            _iExamPaperNodeQuestionRep = iExamPaperNodeQuestionRep;
            _iExamPaperNodeRep = iExamPaperNodeRep;
            _iExamPaperRep = iExamPaperRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iExamQuestionRep = iExamQuestionRep;
            _iExamPaperNodeService = iExamPaperNodeService;
            _iExamPaperService = iExamPaperService;
            _iUnitOfWorkManager = iUnitOfWorkManager;
        }

        public async Task Create(ExamPaperNodeQuestionCreateInputDto input)
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

            if (!_iExamPaperNodeRep.GetAll().Any(a => a.Id == input.paperNodeUid))
            {
                throw new UserFriendlyException("无效的试题大题");
            }

            if (!_iExamPaperRep.GetAll().Any(a => a.Id == input.paperUid))
            {
                throw new UserFriendlyException("无效的试卷");
            }

            var questionList = new List<ExamQuestion>();
            input.questionUidList.ForEach(a =>
            {
                var entity = _iExamQuestionRep.Get(a);
                if (entity == null)
                {
                    throw new UserFriendlyException("无效的试题");
                }
                questionList.Add(entity);
            });
            #endregion

            var score = _iExamPaperNodeRep.GetAll().Where(a => a.Id == input.paperNodeUid).Select(a => a.questionScore).FirstOrDefault();
            var queryable = _iExamPaperNodeQuestionRep.GetAll().Where(a => a.paperNodeUid == input.paperNodeUid);
            var maxListOrder = queryable.Any() ? queryable.Max(a => a.listOrder) : 0;
            var totalScore = 0m;
            if (score == 0)
            {
                totalScore = questionList.Sum(a => a.score);
            }
            else
            {
                totalScore = score * questionList.Count();
            }
            foreach (var examQuestion in questionList)
            {
                maxListOrder++;
                var nodeQuestion = new ExamPaperNodeQuestion()
                {
                    Id = Guid.NewGuid(),
                    dataUpdateTime = DateTime.Now,
                    listOrder = maxListOrder,
                    paperNodeUid = input.paperNodeUid,
                    paperUid = input.paperUid,
                    questionUid = examQuestion.Id,
                    paperQuestionExamTime = examQuestion.examTime,
                    paperQuestionScore = score == 0 ? examQuestion.score : score
                };

                await _iExamPaperNodeQuestionRep.InsertAsync(nodeQuestion);

                #region 组合题处理
                if (examQuestion.questionBaseTypeCode == QuestionTypeConst.Compose)
                {
                    var children =
                        _iExamQuestionRep.GetAll().Where(a => a.parentQuestionUid == examQuestion.Id).ToList();
                    if (score != 0)
                    {
                        var num = children.Count(); //这样做是为了避免平均出过多小数位，忽略的部分由最后一题补足
                        if (num == 0)
                        {
                            throw new UserFriendlyException("试题编号为["+examQuestion.questionCode+"]的组合题没有子试题，无法进行添加！");
                        }
                        var childScore = Math.Round(Convert.ToDecimal(score/num), 2);
                        var lastChildScore = score - (childScore*(num - 1));
                        for (var i = 0; i < num; i++)
                        {
                            var q = children[i];
                            var childeNodeQuestion = new ExamPaperNodeQuestion()
                            {
                                Id = Guid.NewGuid(),
                                dataUpdateTime = DateTime.Now,
                                listOrder = q.listOrder,
                                paperNodeUid = input.paperNodeUid,
                                paperUid = input.paperUid,
                                questionUid = q.Id,
                                paperQuestionExamTime = q.examTime,
                                paperQuestionScore = childScore
                            };
                            if (i == num - 1)
                            {
                                childeNodeQuestion.paperQuestionScore = lastChildScore;
                            }
                            await _iExamPaperNodeQuestionRep.InsertAsync(childeNodeQuestion);
                        }
                    }
                    else
                    {
                        children.ForEach(q =>
                        {
                            var childeNodeQuestion = new ExamPaperNodeQuestion
                            {
                                Id = Guid.NewGuid(),
                                dataUpdateTime = DateTime.Now,
                                listOrder = q.listOrder,
                                paperNodeUid = input.paperNodeUid,
                                paperUid = input.paperUid,
                                questionUid = q.Id,
                                paperQuestionExamTime = q.examTime,
                                paperQuestionScore = q.score
                            };
                            _iExamPaperNodeQuestionRep.InsertAsync(childeNodeQuestion);
                        });
                    }
                }
                #endregion
            }
            await _iExamPaperNodeService.UpdateTotalScoreAndQuestionNum(input.paperNodeUid, totalScore, questionList.Count);
            _iUnitOfWorkManager.Current.SaveChanges();
            await _iExamPaperService.BuidExamPaper(input.paperUid);
        }

        public async Task<List<ExamPaperNodeQuestionOutputDto>> GetList(Guid paperNodeUid)
        {
            var queryable = _iExamPaperNodeQuestionRep.GetAll()
                .Where(a => a.paperNodeUid == paperNodeUid && a.Question.parentQuestionUid == Guid.Empty)
                .Select(a => new ExamPaperNodeQuestionOutputDto()
                {
                    Id = a.Id,
                    listOrder = a.listOrder,
                    paperNodeUid = a.paperNodeUid,
                    paperQuestionExamTime = a.paperQuestionExamTime,
                    paperQuestionScore = a.paperQuestionScore,
                    paperUid = a.paperUid,
                    questionCode = a.Question.questionCode,
                    questionTypeName = a.Question.QuestionType.questionTypeName,
                    questionText = a.Question.questionText,
                    questionUid = a.questionUid
                });

            return await Task.FromResult(queryable.ToList());
        }

        public async Task<List<Guid>> GetIdList(Guid paperNodeUid)
        {
            var queryable =
                _iExamPaperNodeQuestionRep.GetAll()
                    .Where(a => a.paperNodeUid == paperNodeUid && a.Question.parentQuestionUid == Guid.Empty)
                    .Select(a => a.questionUid);
            return await Task.FromResult(queryable.ToList());
        }

        public async Task<ExamPaperNodeQuestionOutputDto> Get(Guid id)
        {
            var entity = _iExamPaperNodeQuestionRep.GetAll().Where(a=>a.Id == id)
                .Select(a=>new {paperNodeQuestion = a, question = a.Question, questionType = a.Question.QuestionType}).FirstOrDefault();

            var dto = new ExamPaperNodeQuestionOutputDto(); 
            entity.question.MapTo(dto);
            entity.questionType.MapTo(dto);
            entity.paperNodeQuestion.MapTo(dto);

            return await Task.FromResult(dto);
        }

        public async Task Update(ExamPaperNodeQuestionInputDto input)
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

            var entity = _iExamPaperNodeQuestionRep.Get(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的试题");
            }

            if (!_iExamPaperNodeRep.GetAll().Any(a => a.Id == input.paperNodeUid))
            {
                throw new UserFriendlyException("无效的试题大题");
            }

            if (!_iExamPaperRep.GetAll().Any(a => a.Id == input.paperUid))
            {
                throw new UserFriendlyException("无效的试卷");
            }

            #endregion

            var score = entity.paperQuestionScore;
            input.MapTo(entity);
            score = entity.paperQuestionScore - score;
            if (score != 0)
            {
                await _iExamPaperNodeService.UpdateTotalScoreAndQuestionNum(input.paperNodeUid, score, 0);

                if (_iExamPaperNodeQuestionRep.GetAll().Any(q=>q.Id == entity.Id && q.Question.questionBaseTypeCode == QuestionTypeConst.Compose))
                {
                    var children =
                        _iExamQuestionRep.GetAll().Where(a => a.parentQuestionUid == entity.questionUid)
                            .Join(_iExamPaperNodeQuestionRep.GetAll(), q => q.Id, nq => nq.questionUid, (q, nq) => nq)
                            .Where(nq => nq.paperUid == entity.paperUid && nq.paperNodeUid == entity.paperNodeUid)
                            .OrderBy(a=>a.listOrder)
                            .ToList();
                    
                    var num = children.Count();//这样做是为了避免平均出过多小数位，忽略的部分由最后一题补足
                    var childScore = Math.Round(Convert.ToDecimal(input.paperQuestionScore / num), 2);
                    var lastChildScore = input.paperQuestionScore - (childScore * (num - 1));
                    for (var i = 0; i < num; i++)
                    {
                        var nq = children[i];
                        nq.paperQuestionScore = (i == num - 1) ? lastChildScore : childScore;
                        await _iExamPaperNodeQuestionRep.UpdateAsync(nq);
                    }
                }
            }
            await _iExamPaperNodeQuestionRep.UpdateAsync(entity);
            _iUnitOfWorkManager.Current.SaveChanges();
            await _iExamPaperService.BuidExamPaper(entity.paperUid);
        }

        public async Task Delete(Guid nodeUid, string ids)
        {
            var idArray = ids.Split(',').Select(a => new Guid(a)).ToArray();

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

            var node = _iExamPaperNodeRep.GetAll().FirstOrDefault(a => a.Id == nodeUid);
            if (node == null)
            {
                throw new UserFriendlyException("无效的大题id");
            }

            foreach (var idStr in idArray)
            {
                if (!_iExamPaperNodeQuestionRep.GetAll().Any(a => a.Id == idStr))
                {
                    throw new UserFriendlyException("无效的试卷试题");
                }
            }

            #endregion

            decimal score = 0;
            var paperId = Guid.Empty;
            foreach (var uid in idArray)
            {
                var guid = uid;
                var obj = _iExamPaperNodeQuestionRep.GetAll().Where(a => a.Id == guid)
                    .Select(a => new {nodeQuestion = a, question = a.Question}).FirstOrDefault();
                score += obj.nodeQuestion.paperQuestionScore;
                paperId = obj.nodeQuestion.paperUid;
                await _iExamPaperNodeQuestionRep.DeleteAsync(a => a.Id == guid);
                if (obj.question.questionBaseTypeCode != QuestionTypeConst.Compose)
                {
                    continue;
                }
                
                var entity = obj.nodeQuestion;
                
                var children =
                    _iExamQuestionRep.GetAll().Where(a => a.parentQuestionUid == entity.questionUid)
                        .Join(_iExamPaperNodeQuestionRep.GetAll(), q => q.Id, nq => nq.questionUid, (q, nq) => nq)
                        .Where(nq => nq.paperUid == entity.paperUid && nq.paperNodeUid == entity.paperNodeUid)
                        .Select(a => a.Id)
                        .ToList();
                children.ForEach(id =>
                {
                    _iExamPaperNodeQuestionRep.DeleteAsync(nq => nq.Id == id);
                });
            }

            var count = idArray.Count();
            await _iExamPaperNodeService.UpdateTotalScoreAndQuestionNum(node.Id, -score, -count);
            _iUnitOfWorkManager.Current.SaveChanges();
            await _iExamPaperService.BuidExamPaper(paperId);
        }

    }
}