using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using SPOC.Common.Cookie;
using Castle.MicroKernel.Registration;
using System.Linq.Dynamic;
using Abp.Collections.Extensions;
using SPOC.Common.Pagination;
using SPOC.Lib;
using SPOC.Statement.Dto.Lib;
using SPOC.User;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SPOC.Common.Extensions;

namespace SPOC.Statement
{
    /// <summary>
    /// 标签统计报表实现类
    /// </summary>
    public class LibLabelStatementService : SPOCAppServiceBase, ILibLabelStatementService
    {
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<UserLabelScore, Guid> _iUserLabelScoreRep;
        private readonly IRepository<Label, Guid> _iLabelRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<ClassTeacher, Guid> _iClassTeacherRep;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iClassRep"></param>
        /// <param name="iStudentInfoRep"></param>
        /// <param name="iUserLabelScoreRep"></param>
        /// <param name="iLabelRep"></param>
        /// <param name="iUserBaseRep"></param>
        /// <param name="iClassTeacherRep"></param>
        public LibLabelStatementService(IRepository<Class, Guid> iClassRep, IRepository<ClassStudent, Guid> iClassStudentRep,
            IRepository<UserLabelScore, Guid> iUserLabelScoreRep, IRepository<Label, Guid> iLabelRep,
            IRepository<UserBase, Guid> iUserBaseRep, IRepository<ClassTeacher, Guid> iClassTeacherRep)
        {
            _iClassRep = iClassRep;
            _iClassStudentRep = iClassStudentRep;
            _iUserLabelScoreRep = iUserLabelScoreRep;
            _iLabelRep = iLabelRep;
            _iUserBaseRep = iUserBaseRep;
            _iClassTeacherRep = iClassTeacherRep;
        }

        /// <summary>
        /// 班级标签统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ClassLabelGettingItem>> ClassLabelGettingList(ClassLabelGettingInputDto input)
        {
            //标签扣分数 计算状态需要
            var labelDeductPoint = -Convert.ToInt32(BaseSiteSetDto.labelDeductPoint);
            //人数统计
            var totalStudents = (from s in _iClassStudentRep.GetAll().AsNoTracking().WhereIf(input.ClassIdList.Any(),
                    s => input.ClassIdList.Contains(s.ClassId))
                                 join cls in _iClassRep.GetAll().AsNoTracking()
                                 on s.ClassId equals cls.Id
                                 select new
                                 {
                                     s.ClassId
                                 }).Count();

            //用户标签得分
            var userLabelScore = from uls in _iUserLabelScoreRep.GetAll().AsNoTracking()
                                 join stu in _iClassStudentRep.GetAll().AsNoTracking()
                                     .WhereIf(input.ClassIdList.Any(),
                                         s => input.ClassIdList.Contains(s.ClassId)) on uls.UserId equals stu.UserId
                                 group uls by uls.LabelId into g
                                 select new
                                 {
                                     lableId = g.Key,
                                     PassNumber = g.Count(a => a.Score > 0),
                                     UnstableNumber = g.Count(a => a.Score <= 0 && a.Score > labelDeductPoint),
                                     FailNumber = g.Count(a => a.Score == labelDeductPoint)
                                 };
            //根据标签、班级分组统计通过人数 
            var labelGettingQuery = (from label in _iLabelRep.GetAll().AsNoTracking().WhereIf(input.FolderId.Any(), q => input.FolderId.Contains(q.folderId))
                                     join uls in userLabelScore on label.Id equals uls.lableId into temp
                                     from utemp in temp.DefaultIfEmpty()
                                     select new ClassLabelGettingItem
                                     {

                                         LabelId = label.Id,
                                         Title = label.title,
                                         PassNumber = utemp == null ? 0 : utemp.PassNumber,
                                         UnstableNumber = utemp == null ? 0 : utemp.UnstableNumber,
                                         FailNumber = utemp == null ? 0 : utemp.FailNumber,
                                         StudentNumber = totalStudents,
                                         PassRate = utemp == null ? 0 : (decimal)utemp.PassNumber / totalStudents * 100,
                                         NotJoinNumber = utemp == null ? totalStudents : totalStudents - utemp.PassNumber - utemp.UnstableNumber - utemp.FailNumber
                                     });

            labelGettingQuery = labelGettingQuery
                .WhereIf(!string.IsNullOrWhiteSpace(input.Title), s => s.Title.Contains(input.Title))
                .WhereIf(input.LabelId!=null && input.LabelId.Any(), s => input.LabelId.Contains(s.LabelId));
               
            //排序
            labelGettingQuery = !string.IsNullOrEmpty(input.sort) ? labelGettingQuery.OrderBy(input.sort + " " + input.order) : labelGettingQuery.OrderByDescending(a => a.PassRate);
            var totle = await labelGettingQuery.CountAsync();
            var rows = await labelGettingQuery.Skip(input.skip).Take(input.pageSize).ToListAsync();
            return new PaginationOutputDto<ClassLabelGettingItem>
            {
                rows = rows,
                total = totle
            };

        }
        /// <summary>
        /// 班级知识点对比折线图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ClassContrastChartDto> ClassContrast(ClassLabelGettingInputDto input)
        {
            var empGuid = Guid.Empty;
            var chartDto = new ClassContrastChartDto{Legend = new List<string>(),SeriesData = new List<SeriesItem>(),XAxis=new List<string>()};
            //班级人数统计
            var classStudentsQuery = from c in _iClassRep.GetAll().AsNoTracking()
                join s in _iClassStudentRep.GetAll().AsNoTracking() on c.Id equals s.ClassId into temp
                from stemp in temp.DefaultIfEmpty()
                join p in _iUserBaseRep.GetAll().AsNoTracking().Where(s => s.approvalStatus.Equals("approved"))
                on stemp.UserId equals p.Id into temp2
                from ptemp in temp2.DefaultIfEmpty()
                group new { c.Id, c.name, userId = stemp == null ? empGuid : stemp.UserId } by c.Id into g
                select new
                {
                    classId = g.Key,
                    className = g.Min(a => a.name),
                    studens = g.Count(a => !a.userId.Equals(empGuid))
                };
            
            //根据标签、班级分组统计通过人数 
            var labelGettingQuery = (from label in _iLabelRep.GetAll().AsNoTracking()
                join uls in _iUserLabelScoreRep.GetAll().AsNoTracking() on label.Id equals uls.LabelId into temp
                from utemp in temp.DefaultIfEmpty()
                join stu in _iClassStudentRep.GetAll().AsNoTracking() on utemp.UserId equals stu.UserId into temp1
                from stutemp in temp1.DefaultIfEmpty()
                join cls in classStudentsQuery on stutemp.ClassId equals cls.classId into temp2
                from clstemp in temp2.DefaultIfEmpty()
                group new { Score = utemp == null ? -9999 : utemp.Score, labelId = label.Id, label.title, classId = clstemp == null ? empGuid : clstemp.classId, clstemp.className, studens = clstemp == null ? 0 : clstemp.studens } by new { classId = clstemp == null ? empGuid : clstemp.classId, labelId = label.Id }
                into g
                select new ClassContrastItemDto
                {
                    ClassId = g.Key.classId,
                    LabelId = g.Key.labelId,
                    PassNumber = g.Count(a => a.Score > 0),
                    UnstableNumber = g.Count(a => a.Score == 0),
                    FailNumber = g.Count(a => a.Score < 0 && a.Score.Equals(-9999)), //-9999表示未参与答题
                    Title = g.Min(a => a.title),
                    ClassName = g.Min(a => a.className),
                    StudentNumber = g.Max(a => a.studens),
                }).ToList();
            var totalClassStudents = await classStudentsQuery.ToListAsync();
            //未参与
            var notAttendList = new List<ClassContrastItemDto>();
            //搜索统计结果 查询班级为空的数据。班级为空即表示班级内学生全部未参与
            var labels = labelGettingQuery.Select(a => new { a.LabelId, a.Title }).Distinct().ToList();
            //根据统计结果填充未参与答题的班级统计数据
            foreach (var classId in input.ClassIdList)
            {
                //查找不到班级统计数据表示班级为参与
                var classItem = totalClassStudents.FirstOrDefault(a => a.classId.Equals(classId));
                if (classItem == null)
                    continue;
                foreach (var label in labels)
                {
                    //判断班级内学生是否参与标签答题
                    if (!labelGettingQuery.Any(a => a.ClassId.Equals(classId) && a.LabelId.Equals(label.LabelId)))
                    {
                        notAttendList.Add(new ClassContrastItemDto
                        {
                            ClassId = classId,
                            LabelId = label.LabelId,
                            PassNumber = 0,
                            UnstableNumber = 0,
                            FailNumber = 0,
                            NotJoinNumber = classItem.studens,
                            Title = label.Title,
                            ClassName = classItem.className,
                            StudentNumber = classItem.studens
                        });
                    }
                }
            }
            //填充未参与答题的班级数据
            var classLabelGettingItems = labelGettingQuery.Union(notAttendList);
            classLabelGettingItems = classLabelGettingItems.Where(a => !a.ClassId.Equals(empGuid))
                .WhereIf(input.ClassIdList.Any(), s => input.ClassIdList.Contains(s.ClassId))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Title), s => s.Title.Contains(input.Title))
                .WhereIf(input.LabelId != null && input.LabelId.Any(), s => input.LabelId.Contains(s.LabelId));

            var rows = classLabelGettingItems.ToList();
            rows.ForEach(a =>
            {
                a.NotJoinNumber = a.StudentNumber - a.PassNumber - a.UnstableNumber - a.FailNumber;
                a.PassRate = a.StudentNumber == 0 ? 0 : (decimal)a.PassNumber / a.StudentNumber * 100;
            });

            //组装折线图数据 
            totalClassStudents.Where(c=> input.ClassIdList.Contains(c.classId)).ForEach(c =>
            {
                chartDto.Legend.Add(c.className);
                //获取当前班级知识点统计数据
                var classData = rows.Where(data => data.ClassId.Equals(c.classId)).ToList();
                //折线图数据填充
                var seriesItem = new SeriesItem(){Name = c.className,Type= "line",Data = new List<decimal>()};
                classData.ForEach(d =>
                {
                    if(!chartDto.XAxis.Contains(d.Title))
                    chartDto.XAxis.Add(d.Title);
                    seriesItem.Data.Add(Math.Round(d.PassRate, 2));
                });
                chartDto.SeriesData.Add(seriesItem);
            });
            
            return chartDto;
        }

        /// <summary>
        /// 某个标签班级下所有学员掌握情况
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<UserLabelGettingItem>> UserLabelGettingList(UserLabelGettingInputDto input)
        {
            var labelDeductPoint = -Convert.ToInt32(BaseSiteSetDto.labelDeductPoint);
            var labelGettingQuery = from stu in _iClassStudentRep.GetAll().AsNoTracking()
                    .WhereIf(input.ClassIdList.Any(), s => input.ClassIdList.Contains(s.ClassId))
                join user in _iUserBaseRep.GetAll().AsNoTracking() on stu.UserId equals user.Id
                join classes in _iClassRep.GetAll().AsNoTracking() on stu.ClassId equals classes.Id
                join uls in _iUserLabelScoreRep.GetAll().AsNoTracking().Where(a=>a.LabelId.Equals(input.LabelId)) on stu.UserId equals uls.UserId into temp
                from utemp in temp.DefaultIfEmpty()
                select new UserLabelGettingItem
                {
                    ClassId = classes.Id,
                    ClassName = classes.name,
                    UserFullName = user.userFullName,
                    UserLoginName = user.userLoginName,
                    UserId = stu.UserId,
                    Status = utemp == null
                        ? -1
                        : (utemp.Score > labelDeductPoint && utemp.Score <= 0
                            ? 2
                            : (utemp.Score == labelDeductPoint ? 1 : 3))
                };
            labelGettingQuery = labelGettingQuery
                .WhereIf(!string.IsNullOrWhiteSpace(input.UserFullName), s => s.UserFullName.Contains(input.UserFullName))
                .WhereIf(!string.IsNullOrWhiteSpace(input.UserLoginName), s => s.UserLoginName.Contains(input.UserLoginName))
                .WhereIf(input.Status!=0, s => s.Status.Equals(input.Status));
            //排序
            labelGettingQuery = !string.IsNullOrEmpty(input.sort) ? labelGettingQuery.OrderBy(input.sort + " " + input.order) : labelGettingQuery.OrderByDescending(a => a.Status);
            var totle = await labelGettingQuery.CountAsync();
            var rows = await labelGettingQuery.Skip(input.skip).Take(input.pageSize).ToListAsync();
            return new PaginationOutputDto<UserLabelGettingItem>
            {
                rows = rows,
                total = totle
            };
        }

        /// <summary>
        /// 学生标签统计报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<StudentLabelGettingItem>> StudentLabelGettingList(StudentLabelGettingInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var labelDeductPoint = -Convert.ToInt32(BaseSiteSetDto.labelDeductPoint);
            var classes = cookie.IsAdmin
                ? _iClassRep.GetAll()
                : _iClassRep.GetAll().Join(_iClassTeacherRep.GetAll().Where(a => a.UserId == cookie.Id), cls => cls.Id,
                    ct => ct.ClassId, (cls, ct) => cls);

            var users = from student in _iClassStudentRep.GetAll()
                join cls in classes on student.ClassId equals cls.Id
                join user in _iUserBaseRep.GetAll() on student.UserId equals user.Id
                select new { user.Id, user.userFullName, user.userLoginName, classId = cls.Id, className = cls.name };

            var labelCount = await _iLabelRep.CountAsync();

            var query = from score in _iUserLabelScoreRep.GetAll()
                join user in users on score.UserId equals user.Id
                group score by user
                into g
                select new StudentLabelGettingItem
                {
                    UserId = g.Key.Id,
                    UserLoginName = g.Key.userLoginName,
                    UserFullName = g.Key.userFullName,
                    ClassId = g.Key.classId,
                    ClassName = g.Key.className,
                    MasterNum = g.Count(s => s.Score > 0),
                    FailNum = g.Count(s => s.Score == labelDeductPoint),
                    UnskilledNum = g.Count(s => s.Score <= 0 && s.Score > labelDeductPoint),
                    EmptyNum = labelCount - g.Count(),
                    MasterRate = (float)g.Count(s => s.Score > 0) / labelCount * 100,
                    LabelNum = labelCount
                };

            if (!string.IsNullOrWhiteSpace(input.OrderExpression))
            {
                query = query.OrderBy(input.OrderExpression);
            }
            else
            {
                query = query.OrderByDescending(a => a.MasterRate);
            }

            var totle = await query.CountAsync();
            var rows = await query.Skip(input.skip).Take(input.pageSize).ToListAsync();

            return new PaginationOutputDto<StudentLabelGettingItem>
            {
                total = totle,
                rows = rows
            };
        }

        /// <summary>
        /// 学生标签掌握熟练度查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<StudentLabelStatementItem>> StudentLabelStatementPagination(
            StudentLabelStatementInputDto input)
        {

            var labelDeductPoint = -Convert.ToInt32(BaseSiteSetDto.labelDeductPoint);

            var userLabelScores = _iUserLabelScoreRep.GetAll().Where(a => a.UserId == input.UserId);
            var result = from label in _iLabelRep.GetAll().AsNoTracking().WhereIf(input.FolderId.Any(), q => input.FolderId.Contains(q.folderId))
                         join labelScore in userLabelScores on label.Id equals labelScore.LabelId into tempTable
                from temp in tempTable.DefaultIfEmpty()
                where (string.IsNullOrEmpty(input.LabelTitle) || label.title.Contains(input.LabelTitle))
                      && (input.Proficiency == 0 || (
                              (input.Proficiency == -1 && temp == null) ||
                              (input.Proficiency == 1 && temp.Score == labelDeductPoint) ||
                              (input.Proficiency == 2 && temp.Score <= 0 && temp.Score > labelDeductPoint) ||
                              (input.Proficiency == 3 && temp.Score > 0)
                          ))
                select new StudentLabelStatementItem
                {
                    LabelId = label.Id,
                    LabelTitle = label.title,
                    Score = temp == null ? (int?)null : temp.Score
                };
            result = result.OrderByDescending(a => a.Score);
            var total = await result.CountAsync();
            var rows = await result.Skip(input.skip).Take(input.pageSize).ToListAsync();
            return new PaginationOutputDto<StudentLabelStatementItem>
            {
                total = total,
                rows = rows
            };
        }
    }
}
