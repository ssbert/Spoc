using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Extensions;
using SPOC.Common.Helper;
using SPOC.Common.Pagination;
using SPOC.Exam.ViewDto;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试排行榜服务接口实现
    /// </summary>
    public class ExamRankingViewService : ApplicationService, IExamRankingViewService
    {
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<ExamTask, Guid> _iExamTaskRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<Faculty, Guid> _iFacultyRep;
        private readonly IRepository<Major, Guid> _iMajorRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IExamExamService _iExamExamService;
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamRankingViewService(IRepository<ExamExam, Guid> iExamExamRep, IRepository<ExamGrade, Guid> iExamGradeRep,
            IRepository<UserBase, Guid> iUserBaseRep, IRepository<ClassStudent, Guid> iClassStudentRep, IRepository<ExamTask, Guid> iExamTaskRep, IRepository<Faculty, Guid> iFacultyRep, IRepository<Major, Guid> iMajorRep, IRepository<Class, Guid> iClassRep, IExamExamService iExamExamService)
        {
            _iExamExamRep = iExamExamRep;
            _iExamGradeRep = iExamGradeRep;
            _iUserBaseRep = iUserBaseRep;
            _iClassStudentRep = iClassStudentRep;
            _iExamTaskRep = iExamTaskRep;
            _iFacultyRep = iFacultyRep;
            _iMajorRep = iMajorRep;
            _iClassRep = iClassRep;
            _iExamExamService = iExamExamService;
        }

        #endregion
        /// <summary>
        /// 考试排行分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExamRankingViewItem>> GetRankingPagination(RankingPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                return new PaginationOutputDto<ExamRankingViewItem>();
            }
            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == cookie.Id).OrderByDescending(a=>a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                throw new UserFriendlyException("没有班级");
            }

            var list = await GetRankingList(input.ExamId, classId, input.ExamTypeCode);
            return new PaginationOutputDto<ExamRankingViewItem>
            {
                rows = list.Skip(input.skip).Take(input.pageSize).ToList(),
                total = list.Count
            };
        }

        /// <summary>
        /// 获取某人考试排行榜
        /// </summary>
        /// <param name="examId"></param>
        /// <param name="userId"></param>
        /// <param name="examTypeCode"></param>
        /// <returns></returns>
        public async Task<ExamRankingViewItem> GetRanking(Guid examId, Guid userId, string examTypeCode)
        {
            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == userId).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                throw new UserFriendlyException("没有班级");
            }
            var list = await GetRankingList(examId, classId, examTypeCode);
            return list.FirstOrDefault(a => a.UserId == userId);
        }
        #region 前台考试排行榜
        /// <summary>
        /// 获取用户的考试记录列表 按提交考试先后顺序排
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SelectListItem>> RecentExamList()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException(-1, "未登录");
            }
           
            var query = await (from eg in _iExamGradeRep.GetAll()
                               join ee in _iExamExamRep.GetAll() on eg.examUid equals ee.Id
                               join et in _iExamTaskRep.GetAll() on ee.TaskId equals et.Id
                               where eg.userUid.Equals(cookie.Id) && eg.gradeStatusCode.Equals("release") &&  eg.IsCompiled &&
                                     ee.examTypeCode.Equals("exam_normal")
                               orderby eg.lastUpdateTime descending
                               select new SelectListItem
                               {
                                   Text = et.Title,
                                   Value = ee.Id.ToString()
                               }).ToListAsync();
            query = query.DistinctBy(a=>new {a.Value}).ToList();
            var firstOrDefault = query.FirstOrDefault();
            if (firstOrDefault != null) firstOrDefault.Selected = true;
            return query;
        }
        /// <summary>
        /// 获取当前用户所在班级指定考试的排行榜
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public async Task<LeaderboardViewDto> LeaderboardView(string examId)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException(-1, "未登录");
            }
            //拉取考试成绩
            await _iExamExamService.CheckCompiled(cookie.Id, examId.TryParseGuid());

            var examUid = examId.TryParseGuid();
            var students = from s in _iClassStudentRep.GetAll()
                join u in _iUserBaseRep.GetAll() on s.UserId equals u.Id
                where u.approvalStatus == "approved"
                select new {s.UserId, s.ClassId};
            //总成绩List
            var gradeList =await (from g in _iExamGradeRep.GetAll()
                             join s in students on g.userUid equals s.UserId
                             where g.examUid == examUid && g.gradeStatusCode.Equals("release") && g.IsCompiled
                                  group g.gradeScore by g.userUid)
                .Select(a => a.Max())  
               // .Distinct()
                .OrderByDescending(a => a)
                .ToListAsync();
            //用户当前所在的教学班级
            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == cookie.Id).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            //班级成绩
            var classGrade = await (
                from s in students 
                join u in _iUserBaseRep.GetAll() on s.UserId equals u.Id
                join classes in _iClassRep.GetAll() on s.ClassId equals classes.Id
                join major in _iMajorRep.GetAll() on classes.majorId equals major.Id
                join faculty in _iFacultyRep.GetAll() on classes.facultyId equals faculty.Id
                join g in _iExamGradeRep.GetAll().Where(g=>g.examUid == examUid) on s.UserId equals g.userUid into gradeTempTable
                from gradeTemp in gradeTempTable.DefaultIfEmpty()
                where  s.ClassId.Equals(classId) //&& gradeTemp.gradeStatusCode.Equals("release") && gradeTemp.IsCompiled
                                    group new
                {
                    u.Id,
                    gradeScore= gradeTemp ==null?-1: (gradeTemp.gradeStatusCode.Equals("release") && gradeTemp.IsCompiled?gradeTemp.gradeScore:-1), //-1分表示未参加考试或成绩未刷新 不计算到统计报表
                    u.userFullName,
                    className = classes.name,
                    majorName = major.name,
                    facultyName = faculty.name
                } by s.UserId
                into g
                select new GradeRankItem
                {
                    UserId = g.Key,
                    Score = g.Max(a => a.gradeScore),
                    ClassName = g.Max(a => a.className),
                    UserFullName = g.Max(a => a.userFullName),
                    MajorName = g.Max(a => a.majorName),
                    FacultyName = g.Max(a => a.facultyName)
                }).OrderByDescending(a=>a.Score).ToListAsync();
            //排名统计
            var classGrades= classGrade.Select(g => g.Score).OrderByDescending(g => g).ToList();
            classGrade.ForEach(a =>
            {
                a.Ranking = a.Score==-1?-1: gradeList.IndexOf(a.Score) + 1;
                a.RankingInClass = a.Score == -1 ? -1 : classGrades.IndexOf(a.Score) + 1;
            });
            //当前用户得分排名信息
            var userScore = classGrade.Where(a => a.UserId.Equals(cookie.Id)).Select(a=>new UserGradeRank{
                Level = a.Score >= 0 && a.Score < 50?0:(a.Score >= 50 && a.Score < 60?1:(a.Score >= 60 && a.Score < 70?2:(a.Score >= 70 && a.Score < 80?3:(a.Score >= 80 && a.Score < 90? 4:(a.Score >= 90 && a.Score <= 100 ? 5 : 6))))),
                UserId =cookie.Id,
                Score = a.Score,
                Ranking = a.Ranking,
                RankingInClass =a.RankingInClass
            }).FirstOrDefault();
           
            var result = new LeaderboardViewDto { UserGradeRank= userScore ,GradeRankList = classGrade};
            //分数等级统计
            var levelView = new LeaderboardLevelViewDto
            {
                Level0 = classGrade.Count(a => a.Score >= 0 && a.Score < 50),
                Level1 = classGrade.Count(a => a.Score >= 50 && a.Score < 60),
                Level2 = classGrade.Count(a => a.Score >= 60 && a.Score < 70),
                Level3 = classGrade.Count(a => a.Score >= 70 && a.Score < 80),
                Level4 = classGrade.Count(a => a.Score >= 80 && a.Score < 90),
                Level5 = classGrade.Count(a => a.Score >= 90 && a.Score <= 100),
                Total= classGrade.Count(a=>a.Score>-1)
            };
            result.LevelView = levelView;
            result.ClassName = classGrade.FirstOrDefault()?.ClassName;
            return result;
        }
        #endregion
        /// <summary>
        /// 获取考试排行榜
        /// </summary>
        /// <param name="examId"></param>
        /// <param name="classId"></param>
        /// <param name="examTypeCode"></param>
        /// <returns></returns>
        private async Task<List<ExamRankingViewItem>> GetRankingList(Guid examId, Guid classId, string examTypeCode)
        {
            var list = new List<ExamRankingViewItem>();
            var students = _iClassStudentRep.GetAll().Where(a => a.ClassId == classId)
                .Join(_iUserBaseRep.GetAll().Where(a=>a.approvalStatus== "approved"), student => student.UserId, user => user.Id,
                    (s, u) => new { s.UserId, UserLoginName = u.userLoginName, UserFullName = u.userFullName });
           
            var retestUserIdList = new List<Guid>();
            var exam = await _iExamExamRep.GetAll().Where(a => a.Id == examId).FirstAsync();
            var exams = _iExamExamRep.GetAll().Where(a => a.TaskId == exam.TaskId && a.examTypeCode == examTypeCode);
            if (examTypeCode == "exam_retest")
            {
                var mainExam = await _iExamExamRep.FirstOrDefaultAsync(a => a.TaskId == exam.TaskId && a.examTypeCode == "exam_normal");
                if (mainExam == null)
                {
                    throw new UserFriendlyException("考试信息丢失");
                }
                //班级所有学生ID
                var allUserIdList = await students.Select(a => a.UserId).ToListAsync();

                //所有补考
                var retestList = await _iExamExamRep.GetAll()
                    .Where(a => a.TaskId == exam.TaskId && a.examTypeCode == "exam_retest")
                    .OrderBy(a => a.createTime)
                    .ToListAsync();
                //当前补考之前的所有补考
                if (retestList.Count > 1)
                {
                    retestList = retestList.Where(a => a.createTime < exam.createTime).ToList();
                }

                //正考未开始
                if (mainExam.BeginTime.HasValue && mainExam.BeginTime > DateTime.Now)
                {
                    return new List<ExamRankingViewItem>();
                }

                async Task<List<Guid>> GetRetestUserIdList(ExamExam targetExam, List<Guid> studentUserIdList)
                {
                    //进行中
                    if (!targetExam.EndTime.HasValue || targetExam.EndTime > DateTime.Now)
                    {
                        return await _iExamGradeRep.GetAll()
                            .Where(a => a.examUid == targetExam.Id && studentUserIdList.Contains(a.userUid))
                            .GroupBy(a => a.userUid)
                            .Where(a => mainExam.maxExamNum.HasValue && mainExam.maxExamNum != 0 && a.Count() >= mainExam.maxExamNum && a.All(b => b.isPass == "N"))
                            .Select(a => a.Key)
                            .ToListAsync();
                    }
                    //已过期
                    //通过考试的学生ID
                    var passUserIdList = await _iExamGradeRep.GetAll()
                        .Where(a => a.examUid == targetExam.Id && studentUserIdList.Contains(a.userUid) && a.isPass == "Y")
                        .Select(a => a.userUid)
                        .Distinct()
                        .ToListAsync();
                    //通过补集操作得到需要补考的学生
                    return studentUserIdList.Where(a => !passUserIdList.Contains(a)).ToList();
                }

                retestUserIdList = await GetRetestUserIdList(mainExam, allUserIdList);
                foreach (var examItem in retestList)
                {
                    retestUserIdList = await GetRetestUserIdList(examItem, retestUserIdList);
                }
            }

            var grades = _iExamGradeRep.GetAll().Where(a => a.examUid == examId);

            var userGradeDic = await (from s in students
                    join g in grades on s.UserId equals g.userUid into gradeTempTable
                    from gradeTemp in gradeTempTable.DefaultIfEmpty()
                    where examTypeCode != "exam_retest" || retestUserIdList.Contains(s.UserId)
                    group gradeTemp by s)
                .ToDictionaryAsync(a => a.Key, a => a.ToList());

            //缺考
            var missList = new List<ExamRankingViewItem>();

            foreach (var kv in userGradeDic)
            {
                var item = new ExamRankingViewItem
                {
                    UserId = kv.Key.UserId,
                    UserName = string.IsNullOrEmpty(kv.Key.UserFullName) ? kv.Key.UserLoginName : kv.Key.UserFullName,
                    ExamTypeCode = examTypeCode
                };
                var userGrade = kv.Value.Where(a => a != null && a.Id != Guid.Empty).OrderByDescending(a => a.gradeScore)
                    .FirstOrDefault();
                if (userGrade == null)
                {
                    missList.Add(item);
                }
                else
                {
                    item.GradeId = userGrade.Id;
                    item.GradeScore = userGrade.gradeScore ?? 0;
                    item.IsPass = userGrade.isPass == "Y";
                    list.Add(item);
                }
            }
            var scoreOrder = list.Select(a => a.GradeScore ?? 0).Distinct().OrderByDescending(a => a).ToList();
            if (list.Any())
            {

                foreach (var item in list)
                {
                    item.Ranking = scoreOrder.IndexOf(item.GradeScore ?? 0) + 1;
                }
                list = list.OrderBy(a => a.Ranking).ThenBy(a => a.UserId).ToList();
            }

            if (missList.Any())
            {
                foreach (var item in missList)
                {
                    item.Ranking = scoreOrder.Count + 1;
                }
                missList = missList.OrderBy(a => a.UserId).ToList();
            }
            list.AddRange(missList);
            return list;
        }
    }
}