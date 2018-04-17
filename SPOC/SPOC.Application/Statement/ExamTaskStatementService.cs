using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using SPOC.Exam;
using SPOC.Statement.Dto;
using SPOC.User;

namespace SPOC.Statement
{
    /// <summary>
    /// 考试报表服务接口实现
    /// </summary>
    public class ExamTaskStatementService:ApplicationService, IExamTaskStatementService
    {
        private readonly IRepository<ExamTask, Guid> _iExamTaskRep;
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<ExamTaskClass, Guid> _iExamTaskClassRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<ClassTeacher, Guid> _iClassTeacherRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<Faculty, Guid> _iFacultyRep;
        private readonly IRepository<Major, Guid> _iMajorRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamTaskStatementService(IRepository<ExamTask, Guid> iExamTaskRep, IRepository<ExamExam, Guid> iExamExamRep, 
            IRepository<ExamGrade, Guid> iExamGradeRep, IRepository<ClassStudent, Guid> iClassStudentRep, 
            IRepository<ExamTaskClass, Guid> iExamTaskClassRep, IRepository<ClassTeacher, Guid> iClassTeacherRep, 
            IRepository<UserBase, Guid> iUserBaseRep, IRepository<Faculty, Guid> iFacultyRep, 
            IRepository<Major, Guid> iMajorRep, IRepository<Class, Guid> iClassRep)
        {
            _iExamTaskRep = iExamTaskRep;
            _iExamExamRep = iExamExamRep;
            _iExamGradeRep = iExamGradeRep;
            _iClassStudentRep = iClassStudentRep;
            _iExamTaskClassRep = iExamTaskClassRep;
            _iClassTeacherRep = iClassTeacherRep;
            _iUserBaseRep = iUserBaseRep;
            _iFacultyRep = iFacultyRep;
            _iMajorRep = iMajorRep;
            _iClassRep = iClassRep;
        }
        #endregion
        /// <summary>
        /// 获取考试报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExamTaskStatementItem>> GetPagination(ExamTaskStatementPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var classIdList = input.ClassIdList;
            if (!classIdList.Any())
            {
                if (cookie.IsAdmin)
                {
                    classIdList = await _iExamTaskClassRep.GetAll().Select(a => a.ClassId).ToListAsync();
                }
                else
                {
                    classIdList = await _iClassTeacherRep.GetAll().Where(a => a.UserId == cookie.Id)
                        .Select(a => a.ClassId).ToListAsync();
                }
            }

            var hasTimeRange = input.BeginTime.HasValue || input.EndTime.HasValue;
            var timeRangeTaskIdList = new List<Guid>();
            if (hasTimeRange)
            {
                timeRangeTaskIdList = await (from exam in _iExamExamRep.GetAll()
                    where (!input.BeginTime.HasValue || exam.BeginTime >= input.BeginTime)
                          && (!input.EndTime.HasValue || exam.EndTime <= input.EndTime)
                          && exam.examTypeCode == "exam_normal"
                    select exam.TaskId).Distinct().ToListAsync();
            }
            
            var examTasks = from examTask in _iExamTaskRep.GetAll()
                join examTaskClass in _iExamTaskClassRep.GetAll() on examTask.Id equals examTaskClass.Id
                join user in _iUserBaseRep.GetAll() on examTask.CreatorId equals user.Id
                where classIdList.Contains(examTaskClass.ClassId)
                && (!hasTimeRange || timeRangeTaskIdList.Contains(examTask.Id))
                && (!input.CreateBeginTime.HasValue || input.CreateBeginTime <= examTask.CreateTime)
                && (!input.CreateEndTime.HasValue || input.CreateEndTime >= examTask.CreateTime)
                && (string.IsNullOrEmpty(input.Code) || examTask.Code == input.Code)
                && (string.IsNullOrEmpty(input.Title) || examTask.Title.Contains(input.Title))
                && (string.IsNullOrEmpty(input.UserFullName) || user.userFullName.Contains(input.UserFullName))
                && (string.IsNullOrEmpty(input.UserLoginName) || user.userLoginName.Contains(input.UserLoginName))
                select examTask;
            var examTaskList = await examTasks.Distinct().ToListAsync();
            
            var examTaskItemList = new List<ExamTaskStatementItem>();
            foreach (var examTask in examTaskList)
            {
                var examTaskItem = await GetExamTaskItem(examTask, classIdList);
                examTaskItemList.Add(examTaskItem);
            }

            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                examTaskItemList = examTaskItemList.OrderByDescending(a => a.CreateTime).ToList();
            }
            else
            {
                examTaskItemList = examTaskItemList.OrderBy(input.OrderExpression).ToList();
            }

            return new PaginationOutputDto<ExamTaskStatementItem>
            {
                rows = examTaskItemList.Skip(input.skip).Take(input.pageSize).ToList(),
                total = examTaskList.Count
            };
        }

        /// <summary>
        /// 获取考试成绩报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<GradeRankingItem>> GetGradePagination(GradeRankingPaginationInputDto input)
        {
            try
            {
                var cookie = CookieHelper.GetLoginInUserInfo();
                var classIdList = input.ClassIdList;
                var exam = await _iExamExamRep.GetAll()
                    .Where(a => a.TaskId == input.ExamTaskId && a.examTypeCode == "exam_normal")
                    .FirstAsync();

                //bool examOutTime = exam.EndTime.HasValue && exam.EndTime <= DateTime.Now;

                var examId = exam.Id;

                List<Guid> allClassIdList; //教师相关所有班级
                if (cookie.IsAdmin)
                {
                    allClassIdList = await _iExamTaskClassRep.GetAll().Where(a => a.Id == input.ExamTaskId)
                        .Select(a => a.ClassId)
                        .ToListAsync();
                }
                else
                {
                    allClassIdList = await (from etc in _iExamTaskClassRep.GetAll()
                        join ct in _iClassTeacherRep.GetAll() on etc.ClassId equals ct.ClassId
                        where etc.Id == input.ExamTaskId && ct.UserId == cookie.Id
                        select etc.ClassId).ToListAsync();
                }
                if (!classIdList.Any())
                {
                    classIdList.AddRange(allClassIdList);
                }

                var students = from s in _iClassStudentRep.GetAll()
                    join u in _iUserBaseRep.GetAll() on s.UserId equals u.Id
                    where allClassIdList.Contains(s.ClassId) && u.approvalStatus == "approved"
                               select new
                    {
                        s.UserId,
                        s.ClassId,
                        UserLoginName = u.userLoginName,
                        UserFullName = u.userFullName
                    };
                var grades = _iExamGradeRep.GetAll().Where(a=>a.IsCompiled);

                //总成绩List
                var gradeList = await (from g in grades
                        join s in students on g.userUid equals s.UserId
                        where g.examUid == examId
                        group g.gradeScore by g.userUid)
                    .Select(a => a.Max())
                    .Distinct()
                    .OrderByDescending(a => a)
                    .ToListAsync();
                

                //班级成绩Dic
                var classGradeDic = await (from g in grades
                        join s in students on g.userUid equals s.UserId
                        where g.examUid == examId && classIdList.Contains(s.ClassId)
                        group g by s.ClassId)
                    .ToDictionaryAsync(a => a.Key,
                        a => a.GroupBy(b => b.userUid, b => b.gradeScore)
                            .Select(b => b.Max())
                            .Distinct()
                            .OrderByDescending(b => b).ToList());

                //班级信息Dic
                var classInfoDic = await (from c in _iClassRep.GetAll()
                    join m in _iMajorRep.GetAll() on c.majorId equals m.Id
                    join f in _iFacultyRep.GetAll() on c.facultyId equals f.Id
                    where classIdList.Contains(c.Id)
                    select new
                    {
                        ClassId = c.Id,
                        ClassName = c.name,
                        FacultyId = f.Id,
                        FacultyName = f.name,
                        MajorId = m.Id,
                        MajorName = m.name
                    }).ToDictionaryAsync(a => a.ClassId, a => a);
                //有效学生
                var studentList = await students.Where(a => classIdList.Contains(a.ClassId)
                                                            && (string.IsNullOrEmpty(input.UserFullName) ||
                                                                a.UserFullName.Contains(input.UserFullName))
                                                            && (string.IsNullOrEmpty(input.UserLoginName) ||
                                                                a.UserLoginName.Contains(input.UserLoginName))
                ).ToListAsync();
                var studentIdList = studentList.Select(a => a.UserId).ToList();
                //学生成绩Dic
                var studentGradeDic = await (from g in grades
                        where studentIdList.Contains(g.userUid) && g.examUid == examId
                        group g.gradeScore by g.userUid)
                    .ToDictionaryAsync(a => a.Key, a => a.Max());
                var itemList = new List<GradeRankingItem>();
                foreach (var student in studentList)
                {
                    var classInfo = classInfoDic[student.ClassId];
                    var item = new GradeRankingItem
                    {
                        UserId = student.UserId,
                        ClassId = student.ClassId,
                        UserFullName = student.UserFullName,
                        UserLoginName = student.UserLoginName,
                        ClassName = classInfo.ClassName,
                        FacultyId = classInfo.FacultyId,
                        FacultyName = classInfo.FacultyName,
                        MajorId = classInfo.MajorId,
                        MajorName = classInfo.MajorName
                    };
                    if (classGradeDic.ContainsKey(student.ClassId))
                    {
                        var classGrade = classGradeDic[student.ClassId];
                        if (studentGradeDic.ContainsKey(student.UserId))
                        {
                            item.Score = studentGradeDic[student.UserId];
                            item.RankingInClass = classGrade.IndexOf(item.Score) + 1;
                            item.Ranking = gradeList.IndexOf(item.Score) + 1;
                            item.JoinExam = true;
                        }
                        else
                        {
                            item.RankingInClass = classGrade.Count + 1;
                            item.Ranking = gradeList.Count + 1;
                        }
                    }
                    else
                    {
                        //班里没人考试
                        item.RankingInClass = 1;
                        item.Ranking = gradeList.Count + 1;
                    }
                    
                    itemList.Add(item);
                }
                
                if (string.IsNullOrWhiteSpace(input.OrderExpression))
                {
                    itemList = itemList.OrderBy(a=>a.Ranking).ToList();
                }
                else
                {
                    itemList = itemList.OrderBy(input.OrderExpression).ToList();
                }

                return new PaginationOutputDto<GradeRankingItem>
                {
                    rows = itemList.Skip(input.skip)
                        .Take(input.pageSize)
                        .ToList(),
                    total = studentList.Count
                };
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
                throw;
            }
        }

        /// <summary>
        /// 获取班级成绩报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<ClassRankingItem>> GetClassRankingList(ClassRankingQueryInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var examId = await _iExamExamRep.GetAll()
                .Where(a => a.TaskId == input.ExamTaskId && a.examTypeCode == "exam_normal")
                .Select(a => a.Id).FirstAsync();

            
            var allClassIdList = new List<Guid>();
            var classIdList = await _iExamTaskClassRep.GetAll().Where(a => a.Id == input.ExamTaskId).Select(a => a.ClassId)
                .ToListAsync();
            allClassIdList.AddRange(classIdList);
            var classes = await _iClassRep.GetAll().Where(a => allClassIdList.Contains(a.Id))
                .Select(a => new {a.Id, a.name}).ToListAsync();
            if (!cookie.IsAdmin)
            {
                var teacherClassIdList = await _iClassTeacherRep.GetAll().Where(a => a.UserId == cookie.Id)
                    .Select(a => a.ClassId).ToListAsync();
                //取交集
                classIdList = classIdList.Intersect(teacherClassIdList).ToList();
            }
            if (input.ClassIdList.Any())
            {
                classIdList = classIdList.Intersect(input.ClassIdList).ToList();
            }

            var students = from s in _iClassStudentRep.GetAll()
                join u in _iUserBaseRep.GetAll() on s.UserId equals u.Id
                where u.approvalStatus == "approved"
                select new { s.UserId, s.ClassId };
            //取得所有班的平均分并排名
            var grades = from g in _iExamGradeRep.GetAll()
                join s in students on g.userUid equals s.UserId
                where allClassIdList.Contains(s.ClassId) && g.examUid == examId
                select new {s.ClassId, g.userUid, g.gradeScore, g.isPass, g.IsCompiled};

            var classGradeDic = await grades.Where(a=>a.IsCompiled)
                .GroupBy(g=> new { g.ClassId, g.userUid }, g=>g.gradeScore)
                .Select(g => new {g.Key.ClassId, score = g.Max()})
                .GroupBy(a => a.ClassId, a => a.score)
                .ToDictionaryAsync(a => a.Key, a => new {tatal = a.Sum(), count = a.Count()});

            var classStudentNumDic = await students.Where(a => classIdList.Contains(a.ClassId))
                .GroupBy(a => a.ClassId)
                .ToDictionaryAsync(a => a.Key, a => a.Count());

            var allClassRankingItemList = new List<ClassRankingItem>();
            var totalScore = 0m;
            foreach (var classId in allClassIdList)
            {
                var item = new ClassRankingItem { ClassId = classId, AverageScore = 0 };
                if (classGradeDic.ContainsKey(classId))
                {
                    var obj = classGradeDic[classId];
                    if (obj.count > 0)
                    {
                        var studentsNum = classStudentNumDic[classId];
                        if (obj.tatal.HasValue)
                        {
                            item.AverageScore = studentsNum == 0 ? 0 : obj.tatal / studentsNum;
                            totalScore += obj.tatal.Value;
                        }
                    }
                }
                allClassRankingItemList.Add(item);
            }
            var averageList = allClassRankingItemList.Select(a => a.AverageScore)
                .Distinct()
                .OrderByDescending(a => a)
                .ToList();

           
            //对需要展示的数据进行填充
            var list = new List<ClassRankingItem>();
            foreach (var classId in classIdList)
            {
                var item = allClassRankingItemList.First(a => a.ClassId == classId);
                item.ClassName = classes.First(a => a.Id == classId).name;
                item.StudentNum = classStudentNumDic.ContainsKey(classId) ? classStudentNumDic[classId] : 0;
                item.Ranking = averageList.IndexOf(item.AverageScore) + 1;
                list.Add(item);
                var gradeList = await grades.Where(a => a.ClassId == classId).ToListAsync();
                if (!gradeList.Any())
                {
                    continue;
                }
                var classGradeList = gradeList.GroupBy(g => g.userUid, g => g)
                    .Select(g => g.OrderByDescending(a => a.gradeScore).FirstOrDefault())
                    .ToList();
                item.PassNum = classGradeList.Count(a => a.isPass == "Y");
                item.JoinNum = classGradeList.Count;
                //var compiledNum = classGradeList.Count(a => a.IsCompiled);
                if (item.StudentNum > 0)
                {
                    item.PassRate = (decimal)item.PassNum / item.StudentNum * 100;
                    item.JoinRate = (decimal)item.JoinNum / item.StudentNum * 100;
                }
                item.MaxScore = gradeList.Where(a=>a.IsCompiled).Max(a => a.gradeScore);
                item.MinScore = gradeList.Where(a => a.IsCompiled).Min(a => a.gradeScore);

                item.ScoreSectionNum1 = classGradeList.Count(a => 0 <= a.gradeScore && a.gradeScore <= 49);
                item.ScoreSectionNum2 = classGradeList.Count(a => 50 <= a.gradeScore && a.gradeScore <= 59);
                item.ScoreSectionNum3 = classGradeList.Count(a => 60 <= a.gradeScore && a.gradeScore <= 69);
                item.ScoreSectionNum4 = classGradeList.Count(a => 70 <= a.gradeScore && a.gradeScore <= 79);
                item.ScoreSectionNum5 = classGradeList.Count(a => 80 <= a.gradeScore && a.gradeScore <= 89);
                item.ScoreSectionNum6 = classGradeList.Count(a => 90 <= a.gradeScore && a.gradeScore <= 100);
            }
            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                list = list.OrderBy(a => a.Ranking).ToList();
            }
            else
            {
                list = list.OrderBy(input.OrderExpression).ToList();
            }

            #region 合计
            
            var totalStudentNum = list.Sum(a => a.StudentNum);
            var statisticsItem = new ClassRankingItem
            {
                ClassName = "合计",
                JoinRate = totalStudentNum == 0 ? 0 : (decimal) list.Sum(a=>a.JoinNum) / totalStudentNum * 100,
                PassRate = totalStudentNum == 0 ? 0 : (decimal) list.Sum(a=>a.PassNum) / totalStudentNum * 100,
                MaxScore = list.Max(a=>a.MaxScore),
                MinScore = list.Min(a => a.MinScore),
                AverageScore = totalStudentNum == 0 ? (decimal?) null : totalScore / totalStudentNum,
                PassNum = list.Sum(a=>a.PassNum),
                JoinNum = list.Sum(a=>a.JoinNum),
                StudentNum = list.Sum(a=>a.StudentNum),
                ScoreSectionNum1 = list.Sum(a=>a.ScoreSectionNum1),
                ScoreSectionNum2 = list.Sum(a=>a.ScoreSectionNum2),
                ScoreSectionNum3 = list.Sum(a=>a.ScoreSectionNum3),
                ScoreSectionNum4 = list.Sum(a=>a.ScoreSectionNum4),
                ScoreSectionNum5 = list.Sum(a=>a.ScoreSectionNum5),
                ScoreSectionNum6 = list.Sum(a=>a.ScoreSectionNum6)
            };
            list.Add(statisticsItem);
            #endregion
            return list;
        }

        /// <summary>
        /// 获取补考成绩报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<GradeRankingItem>> GetRetestPagination(
            RetestRankPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            var retestExam = await _iExamExamRep.GetAll().Where(a => a.Id == input.ExamId).FirstAsync();
            var examTaskId = retestExam.TaskId;
            var allClassIdList = new List<Guid>();
            var classIdList = await _iExamTaskClassRep.GetAll().Where(a => a.Id == examTaskId).Select(a => a.ClassId)
                .ToListAsync();
            allClassIdList.AddRange(classIdList);
            if (!cookie.IsAdmin)
            {
                var teacherClassIdList = await _iClassTeacherRep.GetAll().Where(a => a.UserId == cookie.Id)
                    .Select(a => a.ClassId).ToListAsync();
                //取交集
                classIdList = classIdList.Intersect(teacherClassIdList).ToList();
            }
            if (input.ClassIdList.Any())
            {
                classIdList = classIdList.Intersect(input.ClassIdList).ToList();
            }
            //主考ID
            var mainExam = await _iExamExamRep.GetAll()
                .Where(a => a.TaskId == examTaskId && a.examTypeCode == "exam_normal")
                .FirstAsync();
            //正考未开始
            if (mainExam.BeginTime.HasValue && mainExam.BeginTime > DateTime.Now)
            {
                return new PaginationOutputDto<GradeRankingItem>();
            }
            
            var students = from s in _iClassStudentRep.GetAll()
                join u in _iUserBaseRep.GetAll() on s.UserId equals u.Id
                where u.approvalStatus == "approved"
                select new { userId=s.UserId, classId=s.ClassId };

            //班级所有学生ID
            var allUserIdList = await students.Where(a => classIdList.Contains(a.classId))
                .Select(a => a.userId).ToListAsync();

            //所有补考
            var retestList = await _iExamExamRep.GetAll()
                .Where(a => a.TaskId == examTaskId && a.examTypeCode == "exam_retest")
                .OrderBy(a=>a.createTime)
                .ToListAsync();
            //当前补考之前的所有补考
            if (retestList.Count > 1)
            {
                retestList = retestList.Where(a => a.createTime < retestExam.createTime).ToList();
            }

            async Task<List<Guid>> GetRetestUserIdList(ExamExam exam, List<Guid> userIdList)
            {
                //进行中
                if (!exam.EndTime.HasValue || exam.EndTime > DateTime.Now)
                {
                    return await _iExamGradeRep.GetAll()
                        .Where(a => a.examUid == exam.Id && userIdList.Contains(a.userUid))
                        .GroupBy(a => a.userUid)
                        .Where(a => mainExam.maxExamNum.HasValue && mainExam.maxExamNum != 0 && a.Count() >= mainExam.maxExamNum && a.All(b => b.isPass == "N"))
                        .Select(a => a.Key)
                        .ToListAsync();
                }
                //已过期
                //通过考试的学生ID
                var passUserIdList = await _iExamGradeRep.GetAll()
                    .Where(a => a.examUid == exam.Id && userIdList.Contains(a.userUid) && a.isPass == "Y")
                    .Select(a => a.userUid)
                    .Distinct()
                    .ToListAsync();
                //通过补集操作得到需要补考的学生
                return userIdList.Where(a => !passUserIdList.Contains(a)).ToList();
            }

            var retestUserIdList = await GetRetestUserIdList(mainExam, allUserIdList);
            foreach (var exam in retestList)
            {
                retestUserIdList = await GetRetestUserIdList(exam, retestUserIdList);
            }
            var allStudents = from s in students
                join u in _iUserBaseRep.GetAll() on s.userId equals u.Id
                where allClassIdList.Contains(s.classId) && retestUserIdList.Contains(s.userId)
                select new
                {
                    UserId = s.userId,
                    ClassId = s.classId,
                    UserLoginName = u.userLoginName,
                    UserFullName = u.userFullName
                };
            //总成绩List
            var gradeList = await (from g in _iExamGradeRep.GetAll()
                    join s in allStudents on g.userUid equals s.UserId
                    where g.examUid == input.ExamId
                    group g.gradeScore by g.userUid)
                .Select(a => a.Max())
                .Distinct()
                .OrderByDescending(a => a)
                .ToListAsync();

            //班级成绩Dic
            var classGradeDic = await (from g in _iExamGradeRep.GetAll()
                    join s in allStudents on g.userUid equals s.UserId
                    where g.examUid == input.ExamId && classIdList.Contains(s.ClassId)
                    group g by s.ClassId)
                .ToDictionaryAsync(a => a.Key,
                    a => a.GroupBy(b => b.userUid, b => b.gradeScore)
                        .Select(b => b.Max())
                        .Distinct()
                        .OrderByDescending(b => b).ToList());

            //班级信息Dic
            var classInfoDic = await (from c in _iClassRep.GetAll()
                join m in _iMajorRep.GetAll() on c.majorId equals m.Id
                join f in _iFacultyRep.GetAll() on c.facultyId equals f.Id
                where classIdList.Contains(c.Id)
                select new
                {
                    ClassId = c.Id,
                    ClassName = c.name,
                    FacultyId = f.Id,
                    FacultyName = f.name,
                    MajorId = m.Id,
                    MajorName = m.name
                }).ToDictionaryAsync(a => a.ClassId, a => a);

            //有效学生
            var studentList = await allStudents.Where(a => classIdList.Contains(a.ClassId)
                                                        && (string.IsNullOrEmpty(input.UserFullName) ||
                                                            a.UserFullName.Contains(input.UserFullName))
                                                        && (string.IsNullOrEmpty(input.UserLoginName) ||
                                                            a.UserLoginName.Contains(input.UserLoginName))
            ).ToListAsync();
            var studentIdList = studentList.Select(a => a.UserId).ToList();

            //学生成绩Dic
            var studentGradeDic = await (from g in _iExamGradeRep.GetAll()
                    where studentIdList.Contains(g.userUid) && g.examUid == input.ExamId
                    group g.gradeScore by g.userUid)
                .ToDictionaryAsync(a => a.Key, a => a.Max());

            var itemList = new List<GradeRankingItem>();
            foreach (var student in studentList)
            {
                var classInfo = classInfoDic[student.ClassId];
                var item = new GradeRankingItem
                {
                    UserId = student.UserId,
                    ClassId = student.ClassId,
                    UserFullName = student.UserFullName,
                    UserLoginName = student.UserLoginName,
                    ClassName = classInfo.ClassName,
                    FacultyId = classInfo.FacultyId,
                    FacultyName = classInfo.FacultyName,
                    MajorId = classInfo.MajorId,
                    MajorName = classInfo.MajorName
                };
                if (classGradeDic.ContainsKey(student.ClassId))
                {
                    var classGrade = classGradeDic[student.ClassId];
                    if (studentGradeDic.ContainsKey(student.UserId))
                    {
                        item.Score = studentGradeDic[student.UserId];
                        item.RankingInClass = classGrade.IndexOf(item.Score) + 1;
                        item.Ranking = gradeList.IndexOf(item.Score) + 1;
                        if (!item.Score.HasValue)
                        {
                            item.Score = 0;
                        }
                    }
                    else
                    {
                        item.RankingInClass = classGrade.Count + 1;
                        item.Ranking = gradeList.Count + 1;
                    }
                }
                else
                {
                    //班里没人考试
                    item.RankingInClass = 1;
                    item.Ranking = gradeList.Count + 1;
                }

                itemList.Add(item);
            }

            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                itemList = itemList.OrderBy(a => a.Ranking).ToList();
            }
            else
            {
                itemList = itemList.OrderBy(input.OrderExpression).ToList();
            }

            return new PaginationOutputDto<GradeRankingItem>
            {
                rows = itemList.Skip(input.skip)
                    .Take(input.pageSize)
                    .ToList(),
                total = studentList.Count
            };
        }

        /// <summary>
        /// 考试相关班级树
        /// </summary>
        /// <param name="id">考试任务ID</param>
        /// <returns></returns>
        public async Task<List<SelectListItemDto>> GetExamTaskClassTree(Guid id)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            var classIdList = await _iExamTaskClassRep.GetAll().Where(a => a.Id == id).Select(a => a.ClassId)
                .ToListAsync();
            if (!cookie.IsAdmin)
            {
                var teacherClassIdList = await _iClassTeacherRep.GetAll().Where(a => a.UserId == cookie.Id)
                    .Select(a => a.ClassId).ToListAsync();
                //取交集
                classIdList = classIdList.Intersect(teacherClassIdList).ToList();
            }

            var classList = await _iClassRep.GetAll().Where(a => classIdList.Contains(a.Id))
                .Select(a => new {a.Id, a.facultyId, a.majorId, a.name}).ToListAsync();
            var ids = classList.Select(a => a.facultyId);
            var facultyList = await _iFacultyRep.GetAll().Where(a => ids.Contains(a.Id))
                .Select(a => new {a.Id, a.name}).ToListAsync();
            ids = classList.Select(a => a.majorId);
            var majorList = await _iMajorRep.GetAll().Where(a => ids.Contains(a.Id))
                .Select(a => new {a.Id, a.name, a.facultyId}).ToListAsync();
            var list = new List<SelectListItemDto>();
            facultyList.ForEach(f =>
            {
                var item = new SelectListItemDto
                {
                    id = f.Id.ToString(),
                    text = f.name,
                    children = majorList.Where(m=>m.facultyId == f.Id).Select(m=>new SelectListItemDto
                    {
                        id = m.Id.ToString(),
                        text = m.name,
                        children = classList.Where(c=>c.majorId == m.Id).Select(c=>new SelectListItemDto
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

        /// <summary>
        /// 获取学生考试相关信息报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExamTaskStudentStatementItem>> GetStudentPagination(
            ExamTaskStudentStatementPaginationInputDto input)
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

            var students = from s in _iClassStudentRep.GetAll()
                join u in _iUserBaseRep.GetAll() on s.UserId equals u.Id
                join c in _iClassRep.GetAll() on s.ClassId equals c.Id
                join ec in _iExamTaskClassRep.GetAll() on s.ClassId equals ec.ClassId
                where u.approvalStatus == "approved" && classIdList.Contains(s.ClassId)
                && ec.Id == input.ExamTaskId
                && (string.IsNullOrEmpty(input.UserLoginName) || u.userLoginName.Contains(input.UserLoginName))
                && (string.IsNullOrEmpty(input.UserFullName) || u.userFullName.Contains(input.UserFullName))
                select new {userId= s.UserId,classId= s.ClassId, u.userFullName, u.userLoginName, className = c.name };
            

            var studentIdList = await students.Select(a=>a.userId).ToListAsync();

            //只取最大成绩
            var gradeDic = await (from exam in _iExamExamRep.GetAll()
                join grade in _iExamGradeRep.GetAll() on exam.Id equals grade.examUid
                where exam.TaskId == input.ExamTaskId && exam.examTypeCode == "exam_normal" &&
                      studentIdList.Contains(grade.userUid)
                group grade by grade.userUid).ToDictionaryAsync(a => a.Key,
                a => a.OrderByDescending(g => g.gradeScore).FirstOrDefault());
            var grades = gradeDic.Values;
            var list = new List<ExamTaskStudentStatementItem>();
            var studentList = await students.ToListAsync();
            foreach (var student in studentList)
            {
                var grade = grades.FirstOrDefault(a => a.userUid == student.userId);
                var isJoin = grade != null;
                var item = new ExamTaskStudentStatementItem
                {
                    Id = student.userId,
                    UserLoginName = student.userLoginName,
                    UserFullName = student.userFullName,
                    ClassId = student.classId,
                    ClassName = student.className,
                    JoinState = isJoin ? 1 : 2,
                    PassState = isJoin ? (grade.isPass == "Y" ? 1 : 2): 2,
                    SubmitState = isJoin ? (grade.gradeStatusCode == "release" ? 1 : 2) : 0,
                    Score = isJoin ? grade.gradeScore : null
                };
                item.CompileState = isJoin && item.SubmitState == 1 ? (grade.IsCompiled ? 1 : 2) : 0;
                list.Add(item);
            }
            list = list.WhereIf(input.PassState != 0, a => a.PassState == input.PassState)
                .WhereIf(input.JoinState != 0, a => a.JoinState == input.JoinState)
                .WhereIf(input.SubmitState != 0, a => a.SubmitState == input.SubmitState)
                .WhereIf(input.CompileState != 0, a => a.CompileState == input.CompileState).ToList();

            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                list = list.OrderByDescending(a => a.UserFullName).ToList();
            }
            else
            {
                list = list.OrderBy(input.OrderExpression).ToList();
            }

            var rows = list.Skip(input.skip).Take(input.pageSize).ToList();
            return new PaginationOutputDto<ExamTaskStudentStatementItem>
            {
                rows = rows,
                total = list.Count
            };
        }

        private async Task<ExamTaskStatementItem> GetExamTaskItem(ExamTask examTask, List<Guid> classIdList)
        {
            var students = from s in _iClassStudentRep.GetAll()
                join u in _iUserBaseRep.GetAll() on s.UserId equals u.Id
                where u.approvalStatus == "approved"
                select new { userId=s.UserId, classId=s.ClassId };

            var studentIdList = await (from s in students
                where classIdList.Contains(s.classId)
                select s.userId).ToListAsync();

            var creator = await _iUserBaseRep.GetAsync(examTask.CreatorId);

            var examTaskItem = new ExamTaskStatementItem
            {
                Code = examTask.Code,
                TaskId = examTask.Id,
                Title = examTask.Title,
                CreateTime = examTask.CreateTime,
                CreatorId = examTask.CreatorId,
                UserFullName = creator.userFullName,
                UserLoginName = creator.userLoginName
            };
            //只取最大成绩
            var gradeDic = await (from exam in _iExamExamRep.GetAll()
                join grade in _iExamGradeRep.GetAll() on exam.Id equals grade.examUid
                where exam.TaskId == examTask.Id && exam.examTypeCode == "exam_normal" &&
                      studentIdList.Contains(grade.userUid)
                group grade by grade.userUid).ToDictionaryAsync(a => a.Key,
                a => a.OrderByDescending(g => g.gradeScore).FirstOrDefault());
            var grades = gradeDic.Values;
            //总人数
            var studentNum = studentIdList.Count;
            examTaskItem.StudentNum = studentNum;
            //参加考试人数
            examTaskItem.JoinNum = grades.Count();
            //未参加人数
            examTaskItem.WithoutNum = examTaskItem.StudentNum - examTaskItem.JoinNum;
            //已提交，成绩未获取完毕
            examTaskItem.UnCompiledNum = grades.Count(a => a.gradeStatusCode == "release" && !a.IsCompiled);
            //已提交，成绩获取完毕
            examTaskItem.CompiledNum = grades.Count(a => a.gradeStatusCode == "release" && a.IsCompiled);
            //通过人数
            var passNum = grades.Where(a => a.isPass == "Y").GroupBy(a => a.userUid).Count();
            examTaskItem.PassNum = passNum;
            //未通过人数
            examTaskItem.FailNum = examTaskItem.StudentNum - examTaskItem.PassNum;
            //参与率
            examTaskItem.JoinRate = studentNum == 0 ? 0 : (decimal) examTaskItem.JoinNum / studentNum * 100;
            //通过率
            examTaskItem.PassRate = studentNum == 0 ? 0 : (decimal) passNum / studentNum * 100;
            //未提交数量
            examTaskItem.UnSubmitNum = grades.Count(a => a.gradeStatusCode != "release");
            //提交数量
            examTaskItem.SubmitNum = grades.Count(a => a.gradeStatusCode == "release");
            //已有有效分数
            var hasReleaseGrade = grades.Any(a => a.IsCompiled);
            if (examTaskItem.JoinNum > 0 && hasReleaseGrade)
            {
                //最高分
                examTaskItem.MaxScore = grades.Max(a => a.gradeScore);
                //最低分成绩
                if (examTaskItem.WithoutNum == 0 && examTaskItem.UnSubmitNum == 0 && examTaskItem.UnCompiledNum == 0)
                {
                    examTaskItem.MinScore = grades.Min(a => a.gradeScore);
                }
                //平均分
                var maxScoreSum = grades.Sum(a => a.gradeScore);
                examTaskItem.AverageScore = studentNum == 0 ? 0 : maxScoreSum / studentNum;
            }
            //有补考
            examTaskItem.HasReTest = await _iExamExamRep.GetAll()
                .AnyAsync(a => a.TaskId == examTask.Id && a.examTypeCode == "exam_retest");
            //考试ID
            examTaskItem.ExamId = await _iExamExamRep.GetAll()
                .Where(a => a.TaskId == examTask.Id && a.examTypeCode == "exam_normal").Select(a => a.Id)
                .FirstOrDefaultAsync();
            
            return examTaskItem;
        }
    }
}