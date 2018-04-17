using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using NPOI.SS.Formula.Functions;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using SPOC.Exam;
using SPOC.Exercises;
using SPOC.Exercises.Dto;
using SPOC.QuestionBank.Dto;
using SPOC.Statement.Dto.Exercise;
using SPOC.User;

namespace SPOC.Statement
{
    /// <summary>
    /// 练习报表服务接口实现
    /// </summary>
    public class ExerciseStatementService:SPOCAppServiceBase, IExerciseStatementService
    {
        private readonly IRepository<Exercise, Guid> _iExerciseRep;
        private readonly IRepository<ExerciseRecord, Guid> _iExerciseRecordRep;
        private readonly IRepository<ExerciseClass, Guid> _iExerciseClassRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<ClassTeacher, Guid> _iClassTeacherRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<Faculty, Guid> _iFacultyRep;
        private readonly IRepository<Major, Guid> _iMajorRep;
        private readonly IRepository<ExerciseAnswer, Guid> _iExerciseAnswerRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        /// <summary>
        /// 效率排序条件结构
        /// </summary>
        private class EfficiencySortCondition
        {
            /// <summary>
            /// 是否通过
            /// </summary>
            public bool IsPass { get; set; }
            /// <summary>
            /// 开始时间
            /// </summary>
            public DateTime? BeginTime { get; set; }
            /// <summary>
            /// 结束时间
            /// </summary>
            public DateTime? EndTime { get; set; }
            /// <summary>
            /// 用时
            /// </summary>
            public int UseTime { get; set; }
            /// <summary>
            /// 练习次数
            /// </summary>
            public int ExerciseCount { get; set; }
        }
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseStatementService(IRepository<Exercise, Guid> iExerciseRep, IRepository<ExerciseRecord, Guid> iExerciseRecordRep,
            IRepository<ExerciseClass, Guid> iExerciseClassRep, IRepository<UserBase, Guid> iUserBaseRep, 
            IRepository<ClassTeacher, Guid> iClassTeacherRep, IRepository<ClassStudent, Guid> iClassStudentRep, 
            IRepository<Class, Guid> iClassRep, IRepository<Faculty, Guid> iFacultyRep, 
            IRepository<Major, Guid> iMajorRep, IRepository<ExerciseAnswer, Guid> iExerciseAnswerRep,
            IRepository<ExamQuestion, Guid> iExamQuestionRep)
        {
            _iExerciseRep = iExerciseRep;
            _iExerciseRecordRep = iExerciseRecordRep;
            _iExerciseClassRep = iExerciseClassRep;
            _iUserBaseRep = iUserBaseRep;
            _iClassTeacherRep = iClassTeacherRep;
            _iClassStudentRep = iClassStudentRep;
            _iClassRep = iClassRep;
            _iFacultyRep = iFacultyRep;
            _iMajorRep = iMajorRep;
            _iExerciseAnswerRep = iExerciseAnswerRep;
            _iExamQuestionRep = iExamQuestionRep;
        }
        #endregion

        /// <summary>
        /// 获取练习报表分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExerciseStatementItem>> GetPagination(ExerciseStatementPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var classIdList = input.ClassIdList;
            if (!classIdList.Any())
            {
                if (cookie.IsAdmin)
                {
                    classIdList = await _iExerciseClassRep.GetAll().Select(a => a.ClassId).ToListAsync();
                }
                else
                {
                    classIdList = await _iClassTeacherRep.GetAll().Where(a => a.UserId == cookie.Id)
                        .Select(a => a.ClassId).ToListAsync();
                }
            }

            var hasTimeRange = input.BeginTime.HasValue || input.EndTime.HasValue;
            var timeRangeIdList = new List<Guid>();
            if (hasTimeRange)
            {
                timeRangeIdList = await (from exercise in _iExerciseRep.GetAll()
                    where (!input.BeginTime.HasValue || exercise.CreateTime >= input.BeginTime)
                          && (!input.EndTime.HasValue || exercise.CreateTime <= input.EndTime)
                    select exercise.Id).Distinct().ToListAsync();
            }

            var exercises = from exercise in _iExerciseRep.GetAll()
                join exerciseClass in _iExerciseClassRep.GetAll() on exercise.Id equals exerciseClass.Id
                join user in _iUserBaseRep.GetAll() on exercise.CreatorId equals user.Id
                where classIdList.Contains(exerciseClass.ClassId)
                      && (!hasTimeRange || timeRangeIdList.Contains(exercise.Id))
                      && (!input.BeginTime.HasValue || exercise.CreateTime >= input.BeginTime)
                      && (!input.EndTime.HasValue || exercise.CreateTime <= input.EndTime)
                      && (string.IsNullOrEmpty(input.Title) || exercise.Title.Contains(input.Title))
                      && (string.IsNullOrEmpty(input.UserFullName) || user.userFullName.Contains(input.UserFullName))
                      && (string.IsNullOrEmpty(input.UserLoginName) || user.userLoginName.Contains(input.UserLoginName))
                select exercise;

            var exerciseList = await exercises.Distinct().ToListAsync();
            var list = new List<ExerciseStatementItem>();
            foreach (var exercise in exerciseList)
            {
                var item = await GetExerciseItem(exercise, classIdList);
                list.Add(item);
            }

            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                list = list.OrderByDescending(a => a.CreateTime).ToList();
            }
            else
            {
                list = list.OrderBy(input.OrderExpression).ToList();
            }
            return new PaginationOutputDto<ExerciseStatementItem>
            {
                rows = list.Skip(input.skip).Take(input.pageSize).ToList(),
                total = list.Count
            };
        }

        /// <summary>
        /// 获取学生练习相关信息报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExerciseStudentStatementItem>> GetStudentPagination(
            ExerciseStudentStatementPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var classIdList = input.ClassIdList;
            if (!classIdList.Any())
            {
                if (cookie.IsAdmin)
                {
                    classIdList = await _iClassRep.GetAll().Select(a => a.Id).ToListAsync();
                }
                else
                {
                    classIdList = await _iClassTeacherRep.GetAll().Where(a => a.UserId == cookie.Id)
                        .Select(a => a.ClassId).ToListAsync();
                }
            }

            var studentList = await (from s in _iClassStudentRep.GetAll()
                join u in _iUserBaseRep.GetAll() on s.UserId equals u.Id
                join c in _iClassRep.GetAll() on s.ClassId equals c.Id
                join ec in _iExerciseClassRep.GetAll() on s.ClassId equals ec.ClassId
                where u.approvalStatus == "approved" && classIdList.Contains(s.ClassId)
                      && ec.Id == input.ExerciseId
                      && (string.IsNullOrEmpty(input.UserLoginName) || u.userLoginName.Contains(input.UserLoginName))
                      && (string.IsNullOrEmpty(input.UserFullName) || u.userFullName.Contains(input.UserFullName))
                select new { s.UserId, s.ClassId, u.userFullName, u.userLoginName, className = c.name }).ToListAsync();

            var list = new List<ExerciseStudentStatementItem>();
            foreach (var student in studentList)
            {
                var item = new ExerciseStudentStatementItem
                {
                    UserId = student.UserId,
                    UserLoginName = student.userLoginName,
                    UserFullName = student.userFullName,
                    ClassId = student.ClassId,
                    ClassName = student.className,
                    JoinState = await _iExerciseRecordRep.GetAll()
                        .AnyAsync(a => a.UserId == student.UserId && a.ExerciseId == input.ExerciseId)
                        ? 1
                        : 2,
                    PassState = await _iExerciseRecordRep.GetAll().AnyAsync(a =>
                        a.UserId == student.UserId && a.ExerciseId == input.ExerciseId && a.IsPass)
                        ? 1
                        : 2
                };
                list.Add(item);
            }

            list = list.WhereIf(input.PassState != 0, a => a.PassState == input.PassState)
                .WhereIf(input.JoinState != 0, a => a.JoinState == input.JoinState).ToList();

            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                list = list.OrderByDescending(a => a.UserFullName).ToList();
            }
            else
            {
                list = list.OrderBy(input.OrderExpression).ToList();
            }

            var rows = list.Skip(input.skip).Take(input.pageSize).ToList();
            return new PaginationOutputDto<ExerciseStudentStatementItem>
            {
                rows = rows,
                total = list.Count
            };
        }

        /// <summary>
        /// 获取某学生练习记录报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExerciseRecordItem>> GetExerciseRecordPagination(ExerciseRecordStatementPaginationInputDto input)
        {

            var query = _iExerciseRecordRep.GetAll()
                .Where(a => a.ExerciseId == input.ExerciseId && a.UserId == input.UserId && a.EndTime.HasValue)
                .Select(a => new ExerciseRecordItem
                {
                    Id = a.Id,
                    BeginTime = a.BeginTime,
                    EndTime = a.EndTime.Value,
                    IsPass = a.IsPass
                });
            var total = await query.CountAsync();
            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                query = query.OrderByDescending(a => a.BeginTime);
            }
            else
            {
                query = query.OrderBy(input.OrderExpression);
            }
            var rows = await query.Skip(input.skip).Take(input.pageSize).ToListAsync();

            return new PaginationOutputDto<ExerciseRecordItem>
            {
                rows = rows,
                total = total
            };
        }

        /// <summary>
        /// 获取某学生练习的某一次作答数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExerciseAnswerOutputDto> GetExerciseAnswer(Guid id)
        {
            var obj = await (from record in _iExerciseRecordRep.GetAll()
                join answer in _iExerciseAnswerRep.GetAll() on record.Id equals answer.Id
                join exercie in _iExerciseRep.GetAll() on record.ExerciseId equals exercie.Id
                join question in _iExamQuestionRep.GetAll() on exercie.QuestionId equals question.Id
                where record.Id == id
                select new {
                    record.Id,
                    Question = question,
                    UserAnswer = answer.Answer,
                    record.CompiledResults,
                    record.IsPass
                }).FirstOrDefaultAsync();

            var item = new ExerciseAnswerOutputDto
            {
                Id = obj.Id,
                Question = obj.Question.MapTo<ExamQuestionDto>(),
                UserAnswer = obj.UserAnswer,
                CompiledResults = obj.CompiledResults,
                IsPass = obj.IsPass
            };
            if (item != null && item.CompiledResults == null)
            {
                item.CompiledResults = string.Empty;
            }
            return item;
        }

        /// <summary>
        /// 获取某练习的效率排行榜
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExerciseEfficiencyRankingItem>> GetEfficiencyRankingPagination(ExerciseRankingStatementPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var classIdList = await _iExerciseClassRep.GetAll().Where(a => a.Id == input.ExerciseId)
                .Select(a => a.ClassId).ToListAsync();
            var teacherClassIdList = new List<Guid>();
            if (!cookie.IsAdmin)
            {
                teacherClassIdList = await (from ct in _iClassTeacherRep.GetAll()
                    join ec in _iExerciseClassRep.GetAll() on ct.ClassId equals ec.ClassId
                    where ct.UserId == cookie.Id && ec.Id == input.ExerciseId
                    select ct.ClassId).ToListAsync();
            }

            var list = await GetEfficiencyRankingList(input.ExerciseId, classIdList);
            var isPass = input.PassState == 1;
            list = list.WhereIf(input.PassState != 0, a => a.IsPass == isPass)
                .WhereIf(!string.IsNullOrEmpty(input.UserFullName), a => a.UserFullName.Contains(input.UserFullName))
                .WhereIf(!string.IsNullOrEmpty(input.UserLoginName), a => a.UserLoginName.Contains(input.UserLoginName))
                .WhereIf(teacherClassIdList.Any(), a=> teacherClassIdList.Contains(a.ClassId))
                .WhereIf(input.ClassIdList.Any(), a=>input.ClassIdList.Contains(a.ClassId))
                .ToList();

            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                list = list.OrderBy(a => a.Ranking).ToList();
            }
            else
            {
                list = list.OrderBy(input.OrderExpression).ToList();
            }
            
            return new PaginationOutputDto<ExerciseEfficiencyRankingItem>
            {
                total = list.Count,
                rows = list.Skip(input.skip).Take(input.pageSize).ToList()
            };
        }

        /// <summary>
        /// 获取某练习的积极性排行榜
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExerciseEnthusiasmRankingItem>> GetEnthusiasmRankingPagination(ExerciseRankingStatementPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var classIdList = await _iExerciseClassRep.GetAll().Where(a => a.Id == input.ExerciseId)
                .Select(a => a.ClassId).ToListAsync();
            var teacherClassIdList = new List<Guid>();
            if (!cookie.IsAdmin)
            {
                teacherClassIdList = await (from ct in _iClassTeacherRep.GetAll()
                    join ec in _iExerciseClassRep.GetAll() on ct.ClassId equals ec.ClassId
                    where ct.UserId == cookie.Id && ec.Id == input.ExerciseId
                    select ct.ClassId).ToListAsync();
            }

            var list = await GetEnthusiasmRankingList(input.ExerciseId, classIdList);
            var isPass = input.PassState == 1;
            list = list.WhereIf(input.PassState != 0, a => a.IsPass == isPass)
                .WhereIf(!string.IsNullOrEmpty(input.UserFullName), a => a.UserFullName.Contains(input.UserFullName))
                .WhereIf(!string.IsNullOrEmpty(input.UserLoginName), a => a.UserLoginName.Contains(input.UserLoginName))
                .WhereIf(teacherClassIdList.Any(), a => teacherClassIdList.Contains(a.ClassId))
                .WhereIf(input.ClassIdList.Any(), a => input.ClassIdList.Contains(a.ClassId))
                .ToList();
            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                list = list.OrderBy(a => a.Ranking).ToList();
            }
            else
            {
                list = list.OrderBy(input.OrderExpression).ToList();
            }

            return new PaginationOutputDto<ExerciseEnthusiasmRankingItem>
            {
                total = list.Count,
                rows = list.Skip(input.skip).Take(input.pageSize).ToList()
            };
        }

        /// <summary>
        /// 获取某联系的班级排行榜
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<ExerciseClassRankingItem>> GetClassRankingList(ExerciseClassRankingQueryInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var classIdList = await _iExerciseClassRep.GetAll().Where(a => a.Id == input.ExerciseId)
                .Select(a => a.ClassId).ToListAsync();
            var teacherClassIdList = new List<Guid>();
            if (!cookie.IsAdmin)
            {
                teacherClassIdList = await (from ct in _iClassTeacherRep.GetAll()
                    join ec in _iExerciseClassRep.GetAll() on ct.ClassId equals ec.ClassId
                    where ct.UserId == cookie.Id && ec.Id == input.ExerciseId
                    select ct.ClassId).ToListAsync();
            }

            var studentInfos = _iClassStudentRep.GetAll().Where(a => classIdList.Contains(a.ClassId));
            var dic = await (from record in _iExerciseRecordRep.GetAll()
                join student in studentInfos on record.UserId equals student.UserId
                where record.ExerciseId == input.ExerciseId
                group record.IsPass by new
                {
                    student.ClassId,
                    student.UserId,
                }).ToDictionaryAsync(a => a.Key, a => a.Any(b => b));
            var recordDic = dic.Select(a => new {a.Key.UserId, a.Key.ClassId, isPass = a.Value})
                .GroupBy(a=>a.ClassId)
                .ToDictionary(a=>a.Key, a=>a.ToList());

            var classNameDic = await _iClassRep.GetAll().Where(a => classIdList.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.name);
            var list = new List<ExerciseClassRankingItem>();
            
            foreach (var kv in recordDic)
            {
                var item = new ExerciseClassRankingItem
                {
                    ClassId = kv.Key,
                    ClassName = classNameDic[kv.Key],
                    PassNum = kv.Value.Count(a => a.isPass),
                    JoinNum = kv.Value.Count,
                    StudentNum = await _iClassStudentRep.GetAll().Where(a => a.ClassId == kv.Key).CountAsync()
                };
                item.PassRate = (float) item.PassNum / item.StudentNum * 100;
                item.JoinRate = (float) item.JoinNum / item.StudentNum * 100;
                list.Add(item);
            }

            var otherClassIdList = classIdList.Where(a => !recordDic.Keys.Contains(a)).ToList();

            foreach (var classId in otherClassIdList)
            {
                var item = new ExerciseClassRankingItem
                {
                    ClassId = classId,
                    ClassName = classNameDic[classId],
                    StudentNum = await _iClassStudentRep.GetAll().Where(a => a.ClassId == classId).CountAsync()
                };
                list.Add(item);
            }

            var passRateList = list.Select(a => a.PassRate).Distinct().ToList();
            foreach (var item in list)
            {
                item.Ranking = passRateList.IndexOf(item.PassRate) + 1;
            }

            list = list.WhereIf(teacherClassIdList.Any(), a => teacherClassIdList.Contains(a.ClassId))
                .WhereIf(input.ClassIdList.Any(), a => input.ClassIdList.Contains(a.ClassId)).ToList();
            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                list = list.OrderBy(a => a.Ranking).ToList();
            }
            else
            {
                list = list.OrderBy(input.OrderExpression).ToList();
            }

            #region 合计
            var statisticsItem = new ExerciseClassRankingItem
            {
                ClassId = Guid.Empty,
                ClassName = "合计",
                StudentNum = list.Select(a => a.StudentNum).Sum(),
                PassNum = list.Select(a => a.PassNum).Sum(),
                JoinNum = list.Select(a => a.JoinNum).Sum()
            };
            statisticsItem.PassRate = (float)statisticsItem.PassNum / statisticsItem.StudentNum * 100;
            statisticsItem.JoinRate = (float)statisticsItem.JoinNum / statisticsItem.StudentNum * 100;
            list.Add(statisticsItem);
            #endregion

            return list;
        }


        /// <summary>
        /// 考试相关班级树
        /// </summary>
        /// <param name="id">考试任务ID</param>
        /// <returns></returns>
        public async Task<List<SelectListItemDto>> GetExerciseClassTree(Guid id)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            var classIdList = await _iExerciseClassRep.GetAll().Where(a => a.Id == id).Select(a => a.ClassId)
                .ToListAsync();
            if (!cookie.IsAdmin)
            {
                var teacherClassIdList = await _iClassTeacherRep.GetAll().Where(a => a.UserId == cookie.Id)
                    .Select(a => a.ClassId).ToListAsync();
                //取交集
                classIdList = classIdList.Intersect(teacherClassIdList).ToList();
            }

            var classList = await _iClassRep.GetAll().Where(a => classIdList.Contains(a.Id))
                .Select(a => new { a.Id, a.facultyId, a.majorId, a.name }).ToListAsync();
            var ids = classList.Select(a => a.facultyId);
            var facultyList = await _iFacultyRep.GetAll().Where(a => ids.Contains(a.Id))
                .Select(a => new { a.Id, a.name }).ToListAsync();
            ids = classList.Select(a => a.majorId);
            var majorList = await _iMajorRep.GetAll().Where(a => ids.Contains(a.Id))
                .Select(a => new { a.Id, a.name, a.facultyId }).ToListAsync();
            var list = new List<SelectListItemDto>();
            facultyList.ForEach(f =>
            {
                var item = new SelectListItemDto
                {
                    id = f.Id.ToString(),
                    text = f.name,
                    children = majorList.Where(m => m.facultyId == f.Id).Select(m => new SelectListItemDto
                    {
                        id = m.Id.ToString(),
                        text = m.name,
                        children = classList.Where(c => c.majorId == m.Id).Select(c => new SelectListItemDto
                        {
                            id = c.Id.ToString(),
                            text = c.name
                        }).ToList()
                    }).ToList()
                };
                list.Add(item);
            });
            return list;
        }

        private async Task<ExerciseStatementItem> GetExerciseItem(Exercise exercise, List<Guid> classIdList)
        {
            var students = from s in _iClassStudentRep.GetAll()
                join u in _iUserBaseRep.GetAll() on s.UserId equals u.Id
                where u.approvalStatus == "approved"
                select new { userId=s.UserId, classId=s.ClassId };

            var studentIdList = await (from s in students
                where classIdList.Contains(s.classId)
                select s.userId).ToListAsync();

            var creator = await _iUserBaseRep.GetAsync(exercise.CreatorId);
            var exerciseItem = new ExerciseStatementItem
            {
                Id = exercise.Id,
                Title = exercise.Title,
                CreateTime = exercise.CreateTime,
                CreatorId = exercise.CreatorId,
                UserLoginName = creator.userLoginName,
                UserFullName = creator.userFullName,
                StudentNum = studentIdList.Count
            };

            var recordRep = _iExerciseRecordRep.GetAll();
            //参加人数
            exerciseItem.JoinNum = await recordRep
                .Where(a => studentIdList.Contains(a.UserId) && a.ExerciseId == exercise.Id)
                .Select(a => a.UserId).Distinct().CountAsync();
            //未参加人数
            exerciseItem.WithoutNum = exerciseItem.StudentNum - exerciseItem.JoinNum;
            //参加率
            exerciseItem.JoinRate = (float) exerciseItem.JoinNum / exerciseItem.StudentNum;

            //通过人数
            exerciseItem.PassNum = await recordRep.Where(a => a.ExerciseId == exercise.Id && studentIdList.Contains(a.UserId) && a.IsPass)
                .CountAsync();
            //未通过人数
            exerciseItem.FailNum = exerciseItem.StudentNum - exerciseItem.PassNum;
            //通过率
            exerciseItem.PassRate = (float) exerciseItem.PassNum / exerciseItem.StudentNum;

            return exerciseItem;
        }

        /// <summary>
        /// 获取效率排行
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="classIdList"></param>
        /// <returns></returns>
        private async Task<List<ExerciseEfficiencyRankingItem>> GetEfficiencyRankingList(Guid exerciseId, List<Guid> classIdList)
        {
            var studentInfos = _iClassStudentRep.GetAll().Where(a => classIdList.Contains(a.ClassId));
            var users = _iUserBaseRep.GetAll().Where(a => a.approvalStatus == "approved");
            var students = from s in studentInfos
                join u in users on s.UserId equals u.Id
                join cls in _iClassRep.GetAll() on s.ClassId equals cls.Id
                select new
                {
                    s.UserId,
                    UserLoginName = u.userLoginName,
                    UserFullName = u.userFullName,
                    s.ClassId,
                    ClassName = cls.name
                };

            var records = from r in _iExerciseRecordRep.GetAll()
                          join student in students on r.UserId equals student.UserId
                          where r.ExerciseId == exerciseId && r.EndTime.HasValue
                          select r;
            var recordDic = await records.GroupBy(a => a.UserId).ToDictionaryAsync(a => a.Key, a => a.ToList());
            var conditionList = new List<EfficiencySortCondition>();
            var passDic = new Dictionary<Guid, EfficiencySortCondition>();
            var failDic = new Dictionary<Guid, EfficiencySortCondition>();

            foreach (var kv in recordDic)
            {
                var condition = new EfficiencySortCondition { ExerciseCount = kv.Value.Count };
                var record = kv.Value.FirstOrDefault(a => a.IsPass);
                if (record != null)
                {
                    condition.IsPass = true;
                    
                    if (!conditionList.Any(c => c.Equals(condition)))
                    {
                        conditionList.Add(condition);
                    }
                    passDic.Add(kv.Key, condition);
                }
                else
                {
                    condition.IsPass = false;
                    record = kv.Value.OrderBy(a => a.EndTime - a.BeginTime).First();
                    failDic.Add(kv.Key, condition);
                }
                var useTime = record.EndTime - record.BeginTime;
                if (useTime.HasValue)
                {
                    condition.UseTime = Convert.ToInt32(useTime.Value.TotalSeconds);
                }
                condition.BeginTime = record.BeginTime;
                condition.EndTime = record.EndTime;

            }
            conditionList = conditionList.OrderByDescending(a => a.IsPass)
                .ThenBy(a => a.ExerciseCount)
                .ThenBy(a => a.UseTime)
                .ToList();

            var studentList = await students.ToListAsync();
            //总排名
            var sortList = new List<ExerciseEfficiencyRankingItem>();
            //通过的
            foreach (var kv in passDic)
            {
                var student = studentList.Find(a => a.UserId == kv.Key);
                var item = new ExerciseEfficiencyRankingItem
                {
                    UserId = student.UserId,
                    UserFullName = student.UserFullName,
                    UserLoginName = student.UserLoginName,
                    ClassId = student.ClassId,
                    ClassName = student.ClassName,
                    UseTime = kv.Value.UseTime,
                    BeginTime = kv.Value.BeginTime,
                    EndTime = kv.Value.EndTime,
                    ExerciseCount = kv.Value.ExerciseCount,
                    IsPass = true,
                    Ranking = conditionList.IndexOf(kv.Value) + 1
                };
                sortList.Add(item);
            }
            //未通过的
            var failRanking = conditionList.Count + 1;
            foreach (var kv in failDic)
            {
                var student = studentList.Find(a => a.UserId == kv.Key);
                var item = new ExerciseEfficiencyRankingItem
                {
                    UserId = student.UserId,
                    UserFullName = student.UserFullName,
                    UserLoginName = student.UserLoginName,
                    ClassId = student.ClassId,
                    ClassName = student.ClassName,
                    UseTime = kv.Value.UseTime,
                    BeginTime = kv.Value.BeginTime,
                    EndTime = kv.Value.EndTime,
                    ExerciseCount = kv.Value.ExerciseCount,
                    IsPass = false,
                    Ranking = failRanking
                };
                sortList.Add(item);
            }
            //未提交的
            var lastRanking = failDic.Any() ? failRanking + 1 : failRanking;
            var userIdList = recordDic.Keys.ToList();
            var studentList2 = studentList.Where(s => !userIdList.Contains(s.UserId)).ToList();
            foreach (var student in studentList2)
            {
                var item = new ExerciseEfficiencyRankingItem
                {
                    UserId = student.UserId,
                    UserFullName = student.UserFullName,
                    UserLoginName = student.UserLoginName,
                    ClassId = student.ClassId,
                    ClassName = student.ClassName,
                    IsPass = false,
                    Ranking = lastRanking
                };
                sortList.Add(item);
            }

            sortList = sortList.OrderBy(a => a.Ranking).ToList();
            //处理班级排名
            var classDic = sortList.GroupBy(a => a.ClassId).ToDictionary(a => a.Key, a => a.ToList());
            foreach (var kv in classDic)
            {
                var rankingList = kv.Value.OrderBy(a => a.Ranking).Select(a => a.Ranking).Distinct().ToList();
                foreach (var item in kv.Value)
                {
                    item.ClassRanking = rankingList.IndexOf(item.Ranking) + 1;
                }
            }
            return sortList;
        }

        /// <summary>
        /// 获取积极性排行
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="classIdList"></param>
        /// <returns></returns>
        private async Task<List<ExerciseEnthusiasmRankingItem>> GetEnthusiasmRankingList(Guid exerciseId, List<Guid> classIdList)
        {
            var studentInfos = _iClassStudentRep.GetAll().Where(a => classIdList.Contains(a.ClassId));
            var users = _iUserBaseRep.GetAll().Where(a => a.approvalStatus == "approved");
            var students = from s in studentInfos
                join u in users on s.UserId equals u.Id
                join cls in _iClassRep.GetAll() on s.ClassId equals cls.Id
                select new
                {
                    s.UserId,
                    UserLoginName = u.userLoginName,
                    UserFullName = u.userFullName,
                    s.ClassId,
                    ClassName = cls.name
                };

            var records = from r in _iExerciseRecordRep.GetAll()
                          join student in students on r.UserId equals student.UserId
                          where r.ExerciseId == exerciseId && r.EndTime.HasValue
                          select r;

            var rankingDataList = await records.GroupBy(a => a.UserId)
                .Select(a => new
                {
                    UserId = a.Key,
                    BeginTime = a.Select(r => r.BeginTime).Min(),
                    IsPass = a.Any(r => r.IsPass)
                }).ToListAsync();

            var sortList = rankingDataList.Select(a => a.BeginTime).Distinct().ToList();
            sortList.Sort();

            var studentList = await students.ToListAsync();
            var list = new List<ExerciseEnthusiasmRankingItem>();
            //已提交
            foreach (var data in rankingDataList)
            {
                var student = studentList.Find(a => a.UserId == data.UserId);
                var item = new ExerciseEnthusiasmRankingItem
                {
                    Ranking = sortList.IndexOf(data.BeginTime) + 1,
                    UserId = student.UserId,
                    UserFullName = student.UserFullName,
                    UserLoginName = student.UserLoginName,
                    ClassId = student.ClassId,
                    ClassName = student.ClassName,
                    BeginTime = data.BeginTime,
                    IsPass = data.IsPass
                };
                list.Add(item);
            }
            //未提交
            var lastRanking = rankingDataList.Count + 1;
            var idList = rankingDataList.Select(a => a.UserId).ToList();
            var studentList2 = studentList.Where(a => !idList.Contains(a.UserId)).ToList();
            foreach (var student in studentList2)
            {
                var item = new ExerciseEnthusiasmRankingItem
                {
                    Ranking = lastRanking,
                    UserId = student.UserId,
                    UserFullName = student.UserFullName,
                    UserLoginName = student.UserLoginName,
                    ClassId = student.ClassId,
                    ClassName = student.ClassName,
                    IsPass = false
                };
                list.Add(item);
            }
            list = list.OrderBy(a => a.Ranking).ToList();
            //处理班级排名
            var classDic = list.GroupBy(a => a.ClassId).ToDictionary(a => a.Key, a => a.ToList());
            foreach (var kv in classDic)
            {
                var rankingList = kv.Value.OrderBy(a => a.Ranking).Select(a => a.Ranking).Distinct().ToList();
                foreach (var item in kv.Value)
                {
                    item.ClassRanking = rankingList.IndexOf(item.Ranking) + 1;
                }
            }
            return list;
        }
    }
}