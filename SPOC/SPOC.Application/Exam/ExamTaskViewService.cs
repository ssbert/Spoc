using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Pagination;
using SPOC.Exam.ViewDto;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试任务视图接口实现
    /// </summary>
    public class ExamTaskViewService:ApplicationService, IExamTaskViewService
    {
        private readonly IRepository<ExamTask, Guid> _iExamTaskRep;
        private readonly IRepository<ExamTaskClass, Guid> _iExamTaskClassRep;
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;

        private readonly IExamExamService _iExamExamService;

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamTaskViewService(IRepository<ExamTask, Guid> iExamTaskRep, IRepository<ExamTaskClass, Guid> iExamTaskClassRep, 
            IRepository<ExamExam, Guid> iExamExamRep, IRepository<ExamPaper, Guid> iExamPaperRep, IRepository<ExamGrade, Guid> iExamGradeRep,
            IRepository<ClassStudent, Guid> iClassStudentRep, IExamExamService iExamExamService, IRepository<UserBase, Guid> iUserBaseRep)
        {
            _iExamTaskRep = iExamTaskRep;
            _iExamTaskClassRep = iExamTaskClassRep;
            _iExamExamRep = iExamExamRep;
            _iExamPaperRep = iExamPaperRep;
            _iExamGradeRep = iExamGradeRep;
            _iClassStudentRep = iClassStudentRep;
            _iExamExamService = iExamExamService;
            _iUserBaseRep = iUserBaseRep;
        }
        #endregion

        /// <summary>
        /// 获取视图分页数据项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExamTaskViewItem>> GetPagination(PaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                return new PaginationOutputDto<ExamTaskViewItem>();
            }
            //拉取考试成绩
            await _iExamExamService.CheckCompiled(cookie.Id,Guid.Empty);

            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == cookie.Id).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                return new PaginationOutputDto<ExamTaskViewItem>();
            }
            //考试任务
            var examTasks = from etc in _iExamTaskClassRep.GetAll()
                join task in _iExamTaskRep.GetAll() on etc.Id equals task.Id
                where etc.ClassId == classId
                select task;
            //考试
            var exams = from exam in _iExamExamRep.GetAll()
                join task in examTasks on exam.TaskId equals task.Id
                select exam;
            //成绩
            var gradeList = await(from grade in _iExamGradeRep.GetAll()
                join exam in exams on grade.examUid equals exam.Id
                where grade.gradeStatusCode == "release" && grade.userUid == cookie.Id
                select grade).ToListAsync();
            //试卷
            var paperList = await (from paper in _iExamPaperRep.GetAll()
                join exam in exams on paper.Id equals exam.paperUid
                select paper).ToListAsync();

            var examTaskList = await examTasks.ToListAsync();
            var examList = await exams.ToListAsync();

            //筛选出主考
            var examNoramlList = examList.Where(a => a.examTypeCode == "exam_normal").ToList();
            //需要加入显示列表的补考
            var examReTestList = new List<ExamExam>();
            foreach (var examNoraml in examNoramlList)
            {
                //未开始的
                if (examNoraml.BeginTime.HasValue && examNoraml.BeginTime.Value > DateTime.Now)
                {
                    continue;
                }
                //找到最好成绩
                var grade = gradeList.Where(a => a.examUid == examNoraml.Id)
                    .OrderByDescending(a => a.gradeScore)
                    .FirstOrDefault();

                //找到考试次数
                var examCount = gradeList.Count(a => a.examUid == examNoraml.Id);
                
                decimal passScore = 0;
                if (examNoraml.passGradeType == "passGradeScore")
                {
                    passScore = examNoraml.passGradeScore ?? 0;
                }
                else
                {
                    if (examNoraml.passGradeRate.HasValue)
                    {
                        //找到试卷
                        var paper = paperList.FirstOrDefault(a => a.Id == examNoraml.paperUid);
                        if (paper != null)
                        {
                            passScore = paper.totalScore * examNoraml.passGradeRate.Value / 100;
                        }
                    }
                }
                if (grade != null && grade.gradeScore.HasValue && grade.gradeScore.Value >= passScore)
                {
                    continue;//不需要补考
                }
                //已结束的
                if (examNoraml.EndTime.HasValue && examNoraml.EndTime.Value <= DateTime.Now)
                {
                    var retestList = examList
                        .Where(a => a.examTypeCode == "exam_retest" && a.TaskId == examNoraml.TaskId).ToList();
                    if (retestList.Any())
                    {
                        examReTestList.AddRange(ExamReTestListFilter(retestList, gradeList));
                    }
                }
                else //未结束的
                {
                    //已消耗完考试次数的
                    if (examNoraml.maxExamNum.HasValue && examNoraml.maxExamNum.Value != 0 &&
                        examNoraml.maxExamNum.Value <= examCount)
                    {
                        var retestList = examList
                            .Where(a => a.examTypeCode == "exam_retest" && a.TaskId == examNoraml.TaskId).ToList();
                        if (retestList.Any())
                        {
                            examReTestList.AddRange(ExamReTestListFilter(retestList, gradeList));
                        }
                    }
                }
            }

            //展示出的考试列表为主考+补考
            var allExamList = new List<ExamExam>();
            allExamList.AddRange(examNoramlList);
            allExamList.AddRange(examReTestList);
            var finishedList = new List<ExamExam>();
            //正在进行中且有结束时间的考试
            var result1 = FinishExamListFilter(allExamList, gradeList, a =>
                a.BeginTime.HasValue && a.EndTime.HasValue &&
                a.BeginTime <= DateTime.Now && a.EndTime > DateTime.Now);
            var examList1 = result1.Item1.OrderByDescending(a => a.EndTime).ToList();
            if (result1.Item2.Any())
            {
                finishedList.AddRange(result1.Item2);
            }
            //正在进行中且没有结束时间的考试
            var result2 = FinishExamListFilter(allExamList, gradeList, a => a.BeginTime.HasValue && !a.EndTime.HasValue && a.BeginTime <= DateTime.Now);
            var examList2 = result2.Item1.OrderByDescending(a=>a.BeginTime).ToList();
            if (result2.Item2.Any())
            {
                finishedList.AddRange(result2.Item2);
            }
            //没有开始时间与结束时间的考试
            var result3 = FinishExamListFilter(allExamList, gradeList, a => !a.BeginTime.HasValue && !a.EndTime.HasValue);
            var examList3 = result3.Item1.OrderByDescending(a => a.createTime).ToList();
            if (result3.Item2.Any())
            {
                finishedList.AddRange(result3.Item2);
            }
            //未开始的考试
            var examList4 = allExamList.Where(a => a.BeginTime.HasValue && a.BeginTime > DateTime.Now)
                .OrderByDescending(a => a.BeginTime).ToList();
            //已结束的考试
            var result5 = FinishExamListFilter(allExamList, gradeList, a => a.EndTime.HasValue && a.EndTime <= DateTime.Now);
            var examList5 = result5.Item1.OrderByDescending(a => a.EndTime).ToList();
            if (result5.Item2.Any())
            {
                finishedList.AddRange(result5.Item2);
            }
            if (examList5.Any())
            {
                finishedList.AddRange(examList5);
            }

            var orderList = new List<ExamExam>();
            if (examList1.Any())
            {
                orderList.AddRange(examList1);
            }
            if (examList2.Any())
            {
                orderList.AddRange(examList2);
            }
            if (examList3.Any())
            {
                orderList.AddRange(examList3);
            }
            if (examList4.Any())
            {
                orderList.AddRange(examList4);
            }
            if (finishedList.Any())
            {
                finishedList = finishedList.OrderByDescending(a => a.createTime).ToList();
                orderList.AddRange(finishedList);
            }
            

            var pagination = orderList.Skip(input.skip).Take(input.pageSize);
            var rows = new List<ExamTaskViewItem>();
            foreach (var exam in pagination)
            {
                var task = examTaskList.Find(a => a.Id == exam.TaskId);
                var paper = paperList.Find(a => a.Id == exam.paperUid);
                var gradeQuery = gradeList.Where(a => a.examUid == exam.Id).ToList();
                var gradeCount = gradeQuery.Count;
                var grade = gradeQuery.OrderByDescending(a => a.gradeScore)
                    .FirstOrDefault();
                var viewItem = new ExamTaskViewItem
                {
                    Id = task.Id,
                    Title = task.Title,
                    CreateTime = task.CreateTime,
                    ExamId = exam.Id,
                    ExamName = exam.ExamName,
                    ExamTime = exam.examTime,
                    IsMainExam = exam.examTypeCode == "exam_normal",
                    BeginTime = exam.BeginTime,
                    EndTime = exam.EndTime,
                    PaperId = paper.Id,
                    PaperName = paper.paperName,
                    QuestionNum = paper.questionNum,
                    TotalScore = paper.totalScore,
                    IsMaxExamCount = exam.maxExamNum.HasValue && exam.maxExamNum.Value != 0 && exam.maxExamNum.Value <= gradeCount,
                    GradeCount = gradeCount
                };
                decimal passScore = 0;
                if (exam.passGradeType == "passGradeScore")
                {
                    passScore = exam.passGradeScore ?? 0;
                }
                else
                {
                    if (exam.passGradeRate.HasValue)
                    {
                        passScore = paper.totalScore * exam.passGradeRate.Value / 100;
                    }
                }
                viewItem.PassGradeScore = passScore;
                if (grade != null)
                {
                    if (grade.gradeScore.HasValue)
                    {
                        viewItem.ExamGradeId = grade.Id;
                        viewItem.GradeScore = grade.gradeScore ?? 0;
                        viewItem.IsPass = grade.isPass == "Y";
                    }
                    else
                    {
                        viewItem.IsPass = false;
                    }
                    viewItem.IsCompiled = grade.IsCompiled;
                }
                rows.Add(viewItem);
            }
            return new PaginationOutputDto<ExamTaskViewItem>
            {
                rows = rows,
                total = orderList.Count
            };
        }

        /// <summary>
        /// 获取用户成绩列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="examTaskId"></param>
        /// <param name="examId"></param>
        /// <returns></returns>
        public async Task<SingleExamGradeViewOutputDto> GetExamGrade(Guid userId, Guid examTaskId, Guid examId)
        {
            var dto = new SingleExamGradeViewOutputDto();
            var examTask = await _iExamTaskRep.FirstOrDefaultAsync(a => a.Id == examTaskId);
            if (examTask == null)
            {
                throw new UserFriendlyException("无效的考试任务！");
            }
            dto.Title = examTask.Title;
            dto.CreateTime = examTask.CreateTime;
            var gradeList = await _iExamGradeRep.GetAll()
                .Where(a => a.examUid == examId && a.userUid == userId && a.gradeStatusCode == "release")
                .OrderByDescending(a=>a.beginTime)
                .Select(a=>new ExamGradeViewItem
                {
                    Id = a.Id,
                    BeginTime = a.beginTime,
                    EndTime = a.endTime,
                    IsCompiled = a.IsCompiled,
                    GradeScore = a.gradeScore
                }).ToListAsync();
            dto.ExamGradeList.AddRange(gradeList);
            return dto;
        }

        /// <summary>
        /// 获取考试任务基础数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExamTaskBaseViewOutputDto> GetBase(Guid id)
        {
            var dto = await (from e in _iExamTaskRep.GetAll()
                join u in _iUserBaseRep.GetAll() on e.CreatorId equals u.Id
                where e.Id == id
                select new ExamTaskBaseViewOutputDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    CreatorId = e.CreatorId,
                    CreatorName = string.IsNullOrEmpty(u.userFullName) ? u.userLoginName : u.userFullName,
                    UserImg = u.smallAvatar
                }).FirstOrDefaultAsync();
            dto.MainExamId = await _iExamExamRep.GetAll().Where(a => a.TaskId == id && a.examTypeCode == "exam_normal")
                .Select(a => a.Id).FirstAsync();
            dto.RetestList = await _iExamExamRep.GetAll().Where(a => a.TaskId == id && a.examTypeCode == "exam_retest")
                .Select(a=>new RetestItem{ExamId = a.Id, Title = a.ExamName, CreateTime = a.createTime})
                .OrderBy(a=>a.CreateTime)
                .ToListAsync();
            return dto;
        }

        /// <summary>
        /// 筛选出
        /// 已结束
        /// 与正在进行中未通过且消耗完考试次数
        /// 与正在进行中未通过且未消耗完考试次数
        /// 或未开始且开始时间最近的
        /// 最优先补考
        /// </summary>
        /// <returns></returns>
        private List<ExamExam> ExamReTestListFilter(List<ExamExam> examList, List<ExamGrade> gradeList)
        {
            //根据创建时间来决定显示顺序
            examList = examList.OrderBy(a => a.createTime).ToList();
            var list = new List<ExamExam>();
            foreach (var exam in examList)
            {
                list.Add(exam);
                //已通过补考
                if (gradeList.Any(a => a.examUid == exam.Id && a.isPass == "Y"))
                {
                    break;
                }

                //不限时间不限次数
                if ((!exam.BeginTime.HasValue || exam.BeginTime.Value <= DateTime.Now) && !exam.EndTime.HasValue &&
                    (!exam.maxExamNum.HasValue || exam.maxExamNum.Value == 0))
                {
                    break;
                }


                //进行中
                if ((!exam.BeginTime.HasValue || exam.BeginTime.Value <= DateTime.Now) &&
                           (!exam.EndTime.HasValue || exam.EndTime.Value > DateTime.Now))
                {
                    //不限次数
                    if (!exam.maxExamNum.HasValue || exam.maxExamNum == 0)
                    {
                        break;
                    }
                    //未出成绩
                    if (gradeList.Any(a => a.examUid == exam.Id && !a.IsCompiled))
                    {
                        break;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 根据表达式筛选出未完成的与完成的考试
        /// </summary>
        /// <param name="examList"></param>
        /// <param name="gradeList"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        private (List<ExamExam>, List<ExamExam>) FinishExamListFilter(List<ExamExam> examList, List<ExamGrade> gradeList, Func<ExamExam, bool> exp)
        {
            var tempList = examList.Where(exp);
            var list = new List<ExamExam>();
            var finishedList = new List<ExamExam>();
            foreach (var e in tempList)
            {
                var grades = gradeList.Where(a => a.examUid == e.Id && a.gradeScore.HasValue).ToList();
                var gradeCount = grades.Count;
                var maxExam = e.maxExamNum.HasValue && e.maxExamNum.Value != 0 &&
                              e.maxExamNum.Value <= gradeCount;
                if (maxExam)
                {
                    finishedList.Add(e);
                }
                else
                {
                    list.Add(e);
                }
            }
            return (list, finishedList);
        }
    }
}