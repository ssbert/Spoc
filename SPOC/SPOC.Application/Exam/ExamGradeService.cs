using Abp.Application.Services;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using NPOI.HSSF.UserModel;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using SPOC.Exam.GradeDto;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace SPOC.Exam
{
    /// <summary>
    /// 成绩管理接口实现
    /// </summary>
    public class ExamGradeService:ApplicationService, IExamGradeService
    {
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ExamUserAnswer, Guid> _iExamUserAnswerRep;
        private readonly IRepository<ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<UserBase, Guid> _iUserRep;
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<ClassTeacher, Guid> _iClassTeacherRep;
        private readonly IRepository<ExamTaskClass, Guid> _iExamTaskClassRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamGradeService(IRepository<ExamGrade, Guid> iExamGradeRep, IRepository<TeacherInfo, Guid> iTeacherInfoRep,
            IRepository<ExamUserAnswer, Guid> iExamUserAnswerRep, IRepository<ExamPaper, Guid> iExamPaperRep,
            IRepository<UserBase, Guid> iUserRep, IRepository<ExamExam, Guid> iExamExamRep,
            IRepository<ClassTeacher, Guid> iClassTeacherRep, IRepository<ExamTaskClass, Guid> iExamTaskClassRep,
            IRepository<ClassStudent, Guid> iClassStudentRep, IRepository<Class, Guid> iClassRep)
        {
            _iExamGradeRep = iExamGradeRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iExamUserAnswerRep = iExamUserAnswerRep;
            _iExamPaperRep = iExamPaperRep;
            _iUserRep = iUserRep;
            _iExamExamRep = iExamExamRep;
            _iClassTeacherRep = iClassTeacherRep;
            _iExamTaskClassRep = iExamTaskClassRep;
            _iClassStudentRep = iClassStudentRep;
            _iClassRep = iClassRep;
        }

        /// <summary>
        /// 获取考试成绩分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExamGradeItem>> GetPagination(ExamGradePaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var classIdList = input.ClassIds;
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

            var query = (from exam in _iExamExamRep.GetAll()
                         join grade in _iExamGradeRep.GetAll() on exam.Id equals grade.examUid
                         join user in _iUserRep.GetAll() on grade.userUid equals user.Id
                         join classStudent in _iClassStudentRep.GetAll() on user.Id equals classStudent.UserId
                         join cls in _iClassRep.GetAll() on classStudent.ClassId equals cls.Id
                         join paper in _iExamPaperRep.GetAll() on exam.paperUid equals paper.Id
                         where exam.TaskId == input.TaskId
                         && classIdList.Contains(classStudent.ClassId)
                         && (string.IsNullOrEmpty(input.UserLoginName) || user.userLoginName.Contains(input.UserLoginName))
                         && (string.IsNullOrEmpty(input.UserFullName) || user.userFullName.Contains(input.UserFullName))
                         && (!input.BeginTime.HasValue || grade.beginTime >= input.BeginTime)
                         && (!input.EndTime.HasValue || grade.endTime <= input.EndTime)
                         && (string.IsNullOrEmpty(input.ExamTypeCode) || exam.examTypeCode == input.ExamTypeCode)
                         && (string.IsNullOrEmpty(input.GradeStatusCode) || grade.gradeStatusCode == input.GradeStatusCode)
                         && (!input.MinScore.HasValue || grade.gradeScore >= input.MinScore)
                         && (!input.MaxScore.HasValue || grade.gradeScore <= input.MaxScore)
                         select new ExamGradeItem
                         {
                             ExamId = exam.Id,
                             GradeId = grade.Id,
                             UserId = grade.userUid,
                             ClassId = cls.Id,
                             ClassName = cls.name,
                             UserLoginName = user.userLoginName,
                             UserFullName = user.userFullName,
                             ExamName = exam.ExamName,
                             BeginTime = grade.beginTime,
                             EndTime = grade.endTime,
                             Score = grade.gradeScore,
                             TotalScore = paper.totalScore,
                             IsPass = grade.isPass,
                             GradeStatusCode = grade.gradeStatusCode
                         });

            if (string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                query = query.OrderBy(a => a.BeginTime);
            }
            else
            {
                query = query.OrderBy(input.OrderExpression);
            }

            return new PaginationOutputDto<ExamGradeItem>
            {
                total = await query.CountAsync(),
                rows = await query.Skip(input.skip).Take(input.pageSize).ToListAsync()
            };
        }

        /// <summary>
        /// 更新用户成绩
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public async Task Update(List<UserExamGradeInputDto> inputList)
        {
            var dic = new Dictionary<UserExamGradeInputDto, ExamGrade>();
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
            foreach (var input in inputList)
            {
                var entity = await _iExamGradeRep.FirstOrDefaultAsync(a => a.Id == input.Id);
                if (entity == null)
                {
                    throw new UserFriendlyException("无效的成绩Id");
                }
                dic.Add(input, entity);
            }

            #endregion
            
            foreach (var kv in dic)
            {
                var entity = kv.Value;
                var input = kv.Key;
                entity.gradeScore = input.GradeScore;
                entity.isPass = input.IsPass;
                entity.gradeStatusCode = input.GradeStatusCode;
            }
            //这里没写Rep.Update是为了防止收到未修改的数据导致操作报错，所以选择ABP的默认更新操作
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Delete(IdListInputDto input)
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

            await _iExamGradeRep.DeleteAsync(a => input.idList.Contains(a.Id));
        }

        /// <summary>
        /// 根据条件导出学生考试成绩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<MemoryStream> ExportExamGrade(ExportGradePaginationInputDto input)
        {
            input.userLoginName = input.userLoginName.Trim();
            input.userFullName = input.userFullName.Trim();
            var gradeList = _iExamGradeRep.GetAll()
                .Where(a => a.examUid == input.examUid)
                .Join(_iUserRep.GetAll(), g => g.userUid, u => u.Id, (g, u) => new { u.userLoginName, u.userFullName, grade = g })
                .Join(_iExamPaperRep.GetAll(), g => g.grade.paperUid, p => p.Id, (g, p) => new { g.userLoginName, g.userFullName, p.totalScore, g.grade }) 
                .Select(a => new UserExamGradeOutputDto
                {
                    Id = a.grade.Id,
                    userUid = a.grade.userUid,
                    userLoginName = a.userLoginName,
                    userFullName = a.userFullName,
                    paperTotalScore = a.totalScore,
                    beginTime = a.grade.beginTime,
                    endTime = a.grade.endTime,
                    gradeScore = a.grade.gradeScore,
                    isPass = a.grade.isPass,
                    gradeStatusCode = a.grade.gradeStatusCode,
                    examTime = a.grade.examTime
                })
                .WhereIf(!string.IsNullOrEmpty(input.userLoginName), a => a.userLoginName.Contains(input.userLoginName))
                .WhereIf(!string.IsNullOrEmpty(input.userFullName), a => a.userFullName.Contains(input.userFullName))
                .WhereIf(!string.IsNullOrEmpty(input.gradeStatusCode), a => a.gradeStatusCode == input.gradeStatusCode)
                .WhereIf(input.minGradeScore != null, a => a.gradeScore >= input.minGradeScore)
                .WhereIf(input.maxGradeScore != null, a => a.gradeScore <= input.maxGradeScore)
                .ToList();

            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Sheet1");
            var rowHead = sheet.CreateRow(0);
            rowHead.CreateCell(0).SetCellValue("登录名");
            rowHead.CreateCell(1).SetCellValue("姓名");
            rowHead.CreateCell(2).SetCellValue("开始时间");
            rowHead.CreateCell(3).SetCellValue("结束时间");
            rowHead.CreateCell(4).SetCellValue("答卷时间(分)");
            rowHead.CreateCell(5).SetCellValue("成绩");
            rowHead.CreateCell(6).SetCellValue("试卷总分");
            rowHead.CreateCell(7).SetCellValue("是否通过");
            rowHead.CreateCell(8).SetCellValue("状态");

            var rowIndex = 1;
            foreach (var grade in gradeList)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(grade.userLoginName);
                row.CreateCell(1).SetCellValue(grade.userFullName);
                row.CreateCell(2).SetCellValue(grade.beginTime.ToString("yyyy-MM-dd HH:mm:ss"));
                row.CreateCell(3).SetCellValue(grade.endTime == null ? "" : grade.endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                row.CreateCell(4).SetCellValue(grade.examTime == null ? "" : Second2Minutes(grade.examTime.Value));
                row.CreateCell(5).SetCellValue(grade.gradeScore == null ? "" : grade.gradeScore.Value.ToString());
                row.CreateCell(6).SetCellValue(grade.paperTotalScore.ToString());
                row.CreateCell(7).SetCellValue(grade.isPass == "Y" ? "是" : "否");
                row.CreateCell(8).SetCellValue(GradeStatusCodeFormatter(grade.gradeStatusCode));
                rowIndex++;
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            return await Task.FromResult(ms);
        }

        /// <summary>
        /// 用户考试数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int UserExamGradeCount(Guid userId)
        {
            return _iExamGradeRep.GetAll().Count(a => a.userUid == userId && a.endTime != null);
        }

        /// <summary>
        /// 获取用户考试记录列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<UserExamRecordOutputDto>> GetUserExamRecordList(UserExamRecordInputDto input)
        {
            var queryable = from exam in _iExamExamRep.GetAll()
                join grade in _iExamGradeRep.GetAll() on exam.Id equals grade.examUid
                where exam.TaskId == input.TaskId && exam.examTypeCode == "exam_normal"
                      && grade.userUid == input.UserId
                select new UserExamRecordOutputDto
                {
                    Id = grade.Id,
                    BeginTime = grade.beginTime,
                    EndTime = grade.endTime,
                    GradeScore = grade.gradeScore,
                    IsPass = grade.isPass == "Y"
                };
            if (string.IsNullOrEmpty(input.Sort))
            {
                return await queryable.OrderByDescending(a => a.BeginTime).ToListAsync();
            }
            return await queryable.OrderBy(input.OrderExpression).ToListAsync();
        }

        private string Second2Minutes(int second)
        {
            var min = Math.Floor((double)second / 60);
            var sec = second % 60;
            var minStr = "" + min;
            var secStr = "" + sec;
            if (min < 10)
            {
                minStr = "0" + min;
            }
            if (sec < 10)
            {
                secStr = "0" + secStr;
            }
            
            return minStr + ":" + secStr;
        }

        private string GradeStatusCodeFormatter(string code)
        {
            switch (code)
            {
                case "release":
                    return "已发布";
                case "submitted":
                    return "已提交";
                case "judged":
                    return "已评卷";
                case "pause":
                    return "暂停中";
                case "examing":
                    return "考试中";
                case "judging":
                    return "评卷中";
            }
            return code;
        }
    }
}
