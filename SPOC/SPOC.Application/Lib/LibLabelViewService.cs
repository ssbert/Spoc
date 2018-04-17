using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Pagination;
using SPOC.Core;
using SPOC.Exam;
using SPOC.Exercises;
using SPOC.Lib.Dto;
using SPOC.User;

namespace SPOC.Lib
{
    /// <summary>
    /// 知识点前台展示相关接口
    /// </summary>
    public class LibLabelViewService:SPOCAppServiceBase, ILibLabelViewService
    {
        private readonly IRepository<UserLabelScore, Guid> _iUserLabelScoreRep;
        private readonly IRepository<Label, Guid> _iLabelRep;
        private readonly IRepository<UserAnswerRecords, Guid> _iUserAnswerRecordsRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IRepository<ChallengeQuestion, Guid> _iChallengeQuestionRep;
        private readonly IRepository<ChallengeGrade, Guid> _iChallengeGradeRep;
        private readonly IRepository<QuestionLabel, Guid> _iQuestionLabelRep;
        private readonly IRepository<ExamTask, Guid> _iExamTaskRep;
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<ExamProgramResult, Guid> _iExamProgramResultRep;
        private readonly IRepository<ExamUserAnswer, Guid> _iExamUserAnswerRep;
        private readonly IRepository<Exercise, Guid> _iExerciseRep;
        private readonly IRepository<ExerciseAnswer, Guid> _iExerciseAnswerRep;
        private readonly IRepository<ExerciseRecord, Guid> _iExerciseRecordRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public LibLabelViewService(IRepository<UserLabelScore, Guid> iUserLabelScore, IRepository<Label, Guid> iLabelRep, 
            IRepository<UserAnswerRecords, Guid> iUserAnswerRecordsRep, IRepository<ExamQuestion, Guid> iExamQuestionRep, 
            IRepository<ChallengeQuestion, Guid> iChallengeQuestionRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep, 
            IRepository<ExamTask, Guid> iExamTaskRep, IRepository<ExamExam, Guid> iExamExamRep, 
            IRepository<ExamGrade, Guid> iExamGradeRep, IRepository<Exercise, Guid> iExerciseRep, 
            IRepository<ExerciseRecord, Guid> iExerciseRecordRep, IRepository<ExamUserAnswer, Guid> iExamUserAnswerRep, 
            IRepository<ExerciseAnswer, Guid> iExerciseAnswerRep, IRepository<ChallengeGrade, Guid> iChallengeGradeRep, IRepository<TeacherInfo, Guid> iTeacherInfoRep, IRepository<ExamProgramResult, Guid> iExamProgramResultRep)
        {
            _iLabelRep = iLabelRep;
            _iUserAnswerRecordsRep = iUserAnswerRecordsRep;
            _iExamQuestionRep = iExamQuestionRep;
            _iChallengeQuestionRep = iChallengeQuestionRep;
            _iQuestionLabelRep = iQuestionLabelRep;
            _iExamTaskRep = iExamTaskRep;
            _iExamExamRep = iExamExamRep;
            _iExamGradeRep = iExamGradeRep;
            _iExerciseRep = iExerciseRep;
            _iExerciseRecordRep = iExerciseRecordRep;
            _iExamUserAnswerRep = iExamUserAnswerRep;
            _iExerciseAnswerRep = iExerciseAnswerRep;
            _iChallengeGradeRep = iChallengeGradeRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iExamProgramResultRep = iExamProgramResultRep;
            _iUserLabelScoreRep = iUserLabelScore;
        }

        #endregion


        /// <summary>
        /// 获取自己的标签积分数据
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<Guid, int?>> GetSelfLabelScore()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            
            return await GetUserLabelScore(cookie.Id);
        }

        /// <summary>
        /// 获取用户的标签积分数据
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<Guid, int?>> GetUserLabelScore(Guid userId)
        {
            var result = await (from label in _iLabelRep.GetAll()
                join labelScore in _iUserLabelScoreRep.GetAll() on label.Id equals labelScore.LabelId into tempTable
                from temp in tempTable.DefaultIfEmpty()
                where temp.UserId == userId
                select new
                {
                    label.Id,
                    score = temp == null ? (int?)null : temp.Score
                }).ToDictionaryAsync(a => a.Id, a => a.score);

            return result;
        }

        /// <summary>
        /// 获取用户作答记录数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<UserAnswerRecordsPaginationItem>> GetUserAnswerRecordsPagination(
            UserAnswerRecordsPaginationInput input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }

            var hasSource = string.IsNullOrEmpty(input.Source);
            var queryable = from recorde in _iUserAnswerRecordsRep.GetAll()
                where recorde.LabelId == input.LabelId && (hasSource || recorde.Source == input.Source)
                select new UserAnswerRecordsTemp
                {
                    Id = recorde.Id,
                    QuestionId = recorde.QuestionId,
                    LabelId = recorde.LabelId,
                    UserId = recorde.UserId,
                    RecordId = recorde.RecordId,
                    CreateTime = recorde.CreateTime,
                    Score = recorde.Score,
                    Source = recorde.Source
                };

            if (input.Status == 1)
            {
                queryable = queryable.Where(a => a.Score <= 0);
            }
            else if (input.Status == 2)
            {
                queryable = queryable.Where(a => a.Score > 0);
            }

            IQueryable<UserAnswerRecordsPaginationItem> result;
            if (hasSource)
            {
                result = GetUserAnswerRecordsItemByExam(input.UserId, input, queryable);
                result = result.Concat(GetUserAnswerRecordsItemByExericse(input.UserId, input, queryable));
                result = result.Concat(GetUserAnswerRecordsItemByChallenge(input.UserId, input, queryable));
            }
            else if (input.Source == "exam")
            {
                result = GetUserAnswerRecordsItemByExam(input.UserId, input, queryable);
            }
            else if (input.Source == "exercise")
            {
                result = GetUserAnswerRecordsItemByExericse(input.UserId, input, queryable);
            }
            else if (input.Source == "challenge")
            {
                result = GetUserAnswerRecordsItemByChallenge(input.UserId, input, queryable);
            }
            else
            {
                throw new UserFriendlyException("无效的参数");
            }

            var total = await result.CountAsync();
            var rows = await result.OrderByDescending(a => a.CreateTime).Skip(input.skip).Take(input.pageSize)
                .ToListAsync();

            rows.ForEach(item =>
            {
                item.QuestionText = item.QuestionText.Replace("&nbsp;", "")
                .Replace("&lt;", "<").Replace("&gt;", ">");
            });

            return new PaginationOutputDto<UserAnswerRecordsPaginationItem>()
            {
                rows = rows,
                total = total
            };
        }

        /// <summary>
        /// 获取用户作答详细
        /// </summary>
        /// <param name="recordsId"></param>
        /// <returns></returns>
        public async Task<UserAnswerRecordsQuestion> GetUserAnswerRecordsQuestion(Guid recordsId)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            var records = await (from recorde in _iUserAnswerRecordsRep.GetAll()
                where recorde.Id == recordsId
                select new { recorde.QuestionId, recorde.RecordId, recorde.Source, recorde.UserId}).FirstOrDefaultAsync();
            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id) && cookie.Id != records.UserId)
            {
                throw new UserFriendlyException("没有权限查看");
            }
            UserAnswerRecordsQuestion result;
            if (records.Source == "exam")
            {
                result = await GetRecordsQuestionByExam(records.RecordId, records.QuestionId);
            }
            else if (records.Source == "exercise")
            {
                result = await GetRecordsQuestionByExercise(records.RecordId);
            }
            else if (records.Source == "challenge")
            {
                result = await GetRecordsQuestionByChallenge(records.RecordId);
            }
            else
            {
                throw new UserFriendlyException("无效数据");
            }
            result.Id = recordsId;
            return result;
        }
        /// <summary>
        /// 获取挑战作答记录
        /// </summary>
        /// <param name="challengeGradeId"></param>
        /// <returns></returns>
        public async Task<UserAnswerRecordsQuestion> GetRecordsQuestionByChallenge(Guid challengeGradeId)
        {
            var result = await (from grade in _iChallengeGradeRep.GetAll()
                join question in _iChallengeQuestionRep.GetAll() on grade.questionId equals question.Id
                where grade.Id == challengeGradeId
                select new 
                {
                    grade.isPass,
                    question.MultiTest,
                    QuestionId = question.Id,
                    QuestionText = question.questionText,
                    QuestionBaseTypeCode = question.questionBaseTypeCode,
                    Language = question.language,
                    SelectAnswer = question.selectAnswer,
                    UserAnswer = grade.answer,
                    Title = question.title,
                    Type = "challenge",
                    Result = grade.result
                }).FirstOrDefaultAsync();
            if(result==null)
                return new UserAnswerRecordsQuestion();
            var userRecord= new UserAnswerRecordsQuestion
            {
                IsPass = result.isPass==1,
                QuestionId = result.QuestionId,
                QuestionText = result.QuestionText,
                QuestionBaseTypeCode = result.QuestionBaseTypeCode,
                Language = result.Language,
                SelectAnswer = result.SelectAnswer,
                UserAnswer = result.UserAnswer,
                Title = result.Title,
                Type = "challenge",
                Result = result.Result
            };
            if (result.MultiTest)
            {
                var resultMsg = result.Result.Split('|');
                userRecord.Result = "";
                int i = 0;
                foreach (var msg in resultMsg)
                {
                    i ++;
                    userRecord.Result += $"第{i}次运行结果:\n{msg}\n";
                }
                
            }
            return userRecord;
        }
        #region 私有方法，获取用户作答记录项

        private IQueryable<UserAnswerRecordsPaginationItem> GetUserAnswerRecordsItemByExam(Guid userId,
            UserAnswerRecordsPaginationInput input, 
            IQueryable<UserAnswerRecordsTemp> userAnswerRecords)
        {
            var result = from uar in userAnswerRecords
                join question in _iExamQuestionRep.GetAll() on uar.QuestionId equals question.Id
                join examGrade in _iExamGradeRep.GetAll() on uar.RecordId equals examGrade.Id
                join exam in _iExamExamRep.GetAll() on examGrade.examUid equals exam.Id
                join task in _iExamTaskRep.GetAll() on exam.TaskId equals task.Id
                where uar.Source == "exam" && uar.UserId == userId
                      && (input.LabelId == Guid.Empty || uar.LabelId == input.LabelId)
                      && (string.IsNullOrEmpty(input.QuestionText) ||
                          question.questionPureText.Contains(input.QuestionText))
                      && (string.IsNullOrEmpty(input.Title) || task.Title.Contains(input.Title))
                      && (string.IsNullOrEmpty(input.QuestionBaseTypeCode) ||
                          question.questionBaseTypeCode == input.QuestionBaseTypeCode)
                select new UserAnswerRecordsPaginationItem
                {
                    Id = uar.Id,
                    QuestionId = uar.QuestionId,
                    RecordId = uar.RecordId,
                    Title = task.Title,
                    QuestionText = question.questionPureText,
                    QuestionBaseTypeCode = question.questionBaseTypeCode,
                    Source = uar.Source,
                    Score = uar.Score,
                    CreateTime = uar.CreateTime
                };
            return result;
        }

        private IQueryable<UserAnswerRecordsPaginationItem> GetUserAnswerRecordsItemByExericse(Guid userId,
            UserAnswerRecordsPaginationInput input,
            IQueryable<UserAnswerRecordsTemp> userAnswerRecords)
        {
            var result = from uar in userAnswerRecords
                join question in _iExamQuestionRep.GetAll() on uar.QuestionId equals question.Id
                join record in _iExerciseRecordRep.GetAll() on uar.RecordId equals record.Id
                join exercise in _iExerciseRep.GetAll() on record.ExerciseId equals exercise.Id
                where uar.Source == "exercise" && uar.UserId == userId
                      && (input.LabelId == Guid.Empty || uar.LabelId == input.LabelId)
                      && (string.IsNullOrEmpty(input.QuestionText) ||
                          question.questionPureText.Contains(input.QuestionText))
                      && (string.IsNullOrEmpty(input.Title) || exercise.Title.Contains(input.Title))
                      && (string.IsNullOrEmpty(input.QuestionBaseTypeCode) ||
                          question.questionBaseTypeCode == input.QuestionBaseTypeCode)
                select new UserAnswerRecordsPaginationItem
                {
                    Id = uar.Id,
                    QuestionId = uar.QuestionId,
                    RecordId = uar.RecordId,
                    Title = exercise.Title,
                    QuestionText = question.questionPureText,
                    QuestionBaseTypeCode = question.questionBaseTypeCode,
                    Source = uar.Source,
                    Score = uar.Score,
                    CreateTime = uar.CreateTime
                };
            return result;
        }

        private IQueryable<UserAnswerRecordsPaginationItem> GetUserAnswerRecordsItemByChallenge(Guid userId,
            UserAnswerRecordsPaginationInput input,
            IQueryable<UserAnswerRecordsTemp> userAnswerRecords)
        {
            var result = from uar in userAnswerRecords
                join question in _iChallengeQuestionRep.GetAll() on uar.QuestionId equals question.Id
                where uar.Source == "challenge" && uar.UserId == userId
                      && (input.LabelId == Guid.Empty || uar.LabelId == input.LabelId)
                      && (string.IsNullOrEmpty(input.QuestionText) ||
                          question.questionPureText.Contains(input.QuestionText))
                      && (string.IsNullOrEmpty(input.Title) || question.title.Contains(input.Title))
                      && (string.IsNullOrEmpty(input.QuestionBaseTypeCode) ||
                          question.questionBaseTypeCode == input.QuestionBaseTypeCode)
                select new UserAnswerRecordsPaginationItem
                {
                    Id = uar.Id,
                    QuestionId = uar.QuestionId,
                    RecordId = uar.RecordId,
                    Title = question.title,
                    QuestionText = question.questionPureText,
                    QuestionBaseTypeCode = question.questionBaseTypeCode,
                    Source = uar.Source,
                    Score = uar.Score,
                    CreateTime = uar.CreateTime
                };
            return result;
        }

        #endregion

        #region 私有方法，获取用户试题作答详细

        private async Task<UserAnswerRecordsQuestion> GetRecordsQuestionByExam(Guid examGradeId, Guid questionId)
        {
            var result = await _iExamQuestionRep.GetAll().Where(a => a.Id == questionId)
                .Select(a => new UserAnswerRecordsQuestion
                {
                    QuestionId = questionId,
                    QuestionText = a.questionText,
                    QuestionBaseTypeCode = a.questionBaseTypeCode,
                    Language = a.language,
                    SelectAnswer = a.selectAnswer,
                    Type = "normal"
                }).FirstOrDefaultAsync();
            if (result == null)
                return new UserAnswerRecordsQuestion();
            var examAnswer = await (from grade in _iExamGradeRep.GetAll()
                join exam in _iExamExamRep.GetAll() on grade.examUid equals exam.Id
                join task in _iExamTaskRep.GetAll() on exam.TaskId equals task.Id
                where grade.Id == examGradeId
                select new {grade.userAnswerUid, task.Title}).FirstOrDefaultAsync();

            var answer = await _iExamUserAnswerRep.FirstOrDefaultAsync(examAnswer.userAnswerUid);
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml(answer.userAnswer);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
            var xpath = $"/exam_grade_object/exam_answers/exam_answer[question_uid='{questionId}']";
            var examAnswerNode = xml.SelectSingleNode(xpath);
            var answerText = "";
            if (examAnswerNode != null)
            {
                var answerTextNode = examAnswerNode.SelectSingleNode("answer_text");
                answerText = answerTextNode?.InnerText ?? "";
                if (result.QuestionBaseTypeCode.Contains("program"))
                {
                    answerText = HttpUtility.UrlDecode(answerText);
                }
            }
            var programResult =
                _iExamProgramResultRep.FirstOrDefault(g => g.GradeId.Equals(examGradeId) &&
                                                           g.QuestionId.Equals(questionId));
            if (programResult != null)
            {
                result.Result = programResult.Result;
            }
            var answerRecord= _iUserAnswerRecordsRep.FirstOrDefault(g => g.RecordId.Equals(examGradeId) &&
                                                                         g.QuestionId.Equals(questionId));
            if (answerRecord != null)
            {
                result.IsPass = answerRecord.Score > 0;
            }
            result.UserAnswer = answerText;
            result.Title = examAnswer.Title;
            return result;
        }

        private async Task<UserAnswerRecordsQuestion> GetRecordsQuestionByExercise(Guid exerciseRecordId)
        {
            var exercise = await (from records in _iExerciseRecordRep.GetAll()
                    join e in _iExerciseRep.GetAll() on records.ExerciseId equals e.Id
                    where records.Id == exerciseRecordId
                    select new {e.QuestionId, e.Title})
                .FirstOrDefaultAsync();
            if (exercise == null)
                return new UserAnswerRecordsQuestion();
            var result = await _iExamQuestionRep.GetAll().Where(a => a.Id == exercise.QuestionId)
                .Select(a => new UserAnswerRecordsQuestion
                {
                    QuestionId = exercise.QuestionId,
                    QuestionText = a.questionText,
                    QuestionBaseTypeCode = a.questionBaseTypeCode,
                    Language = a.language,
                    SelectAnswer = a.selectAnswer,
                    Type = "normal",
                   
                }).FirstOrDefaultAsync();
            if(result==null)
                return new UserAnswerRecordsQuestion();
            var answer = await _iExerciseAnswerRep.FirstOrDefaultAsync(exerciseRecordId);
            var record = await _iExerciseRecordRep.FirstOrDefaultAsync(exerciseRecordId);
            if(answer!=null)
            result.UserAnswer = answer.Answer;
            result.Title = exercise.Title;
            if (record != null)
            {
                result.Result = record.CompiledResults;
                result.IsPass = record.IsPass;
            }
            return result;
        }



        #endregion
    }
}