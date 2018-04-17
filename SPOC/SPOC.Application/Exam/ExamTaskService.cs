using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.Helper;
using SPOC.Common.Pagination;
using SPOC.Exam.Dto;
using SPOC.Exam.Dto.Judge;
using SPOC.User;
using SPOC.User.Dto.Department;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试任务服务接口实现
    /// </summary>
    public class ExamTaskService:ApplicationService, IExamTaskService
    {
        private readonly IRepository<ExamTask, Guid> _iExamTaskRep;
        private readonly IRepository<ExamTaskClass, Guid> _iExamTaskClassRep;
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<ClassTeacher, Guid> _iClassTeacherRep;
        private readonly IRepository<ExamJudge, Guid> _iExamJudgeRep;

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamTaskService(IRepository<ExamTask, Guid> iExamTaskRep, IRepository<ExamTaskClass, Guid> iExamTaskClassRep, 
            IRepository<ExamExam, Guid> iExamExamRep, IRepository<ExamGrade, Guid> iExamGradeRep, IRepository<Class, Guid> iClassRep, 
            IRepository<TeacherInfo, Guid> iTeacherInfoRep, IRepository<UserBase, Guid> iUserBaseRep, 
            IRepository<ClassStudent, Guid> iClassStudentRep, IRepository<ClassTeacher, Guid> iClassTeacherRep,
            IRepository<ExamJudge, Guid> iExamJudgeRep)
        {
            _iExamTaskRep = iExamTaskRep;
            _iExamTaskClassRep = iExamTaskClassRep;
            _iExamExamRep = iExamExamRep;
            _iClassRep = iClassRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iUserBaseRep = iUserBaseRep;
            _iClassStudentRep = iClassStudentRep;
            _iClassTeacherRep = iClassTeacherRep;
            _iExamGradeRep = iExamGradeRep;
            _iExamJudgeRep = iExamJudgeRep;
        }

        #endregion


        /// <summary>
        /// 获取一个考试任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExamTaskOutputDto> Get(Guid id)
        {
            var dto = new ExamTaskOutputDto();
            var task = await _iExamTaskRep.FirstOrDefaultAsync(id);
            if (task == null)
            {
                return dto;
            }
            task.MapTo(dto);
            var exams = await _iExamExamRep.GetAll()
                .Where(a => a.TaskId == id)
                .Select(a => new ExamItem
                {
                    Id = a.Id,
                    ExamCode = a.ExamCode,
                    ExamName = a.ExamName,
                    CreateTime = a.createTime,
                    BeginTime = a.BeginTime,
                    EndTime = a.EndTime,
                    ExamTypeCode = a.examTypeCode,
                    ExamTime = a.examTime,
                    MaxExamNum = a.maxExamNum,
                    PassGradeType = a.passGradeType,
                    PassGradeScore = a.passGradeScore,
                    PassGradeRate = a.passGradeRate
                })
                .OrderBy(a=>a.CreateTime)
                .ToListAsync();
            var classes = await _iClassRep.GetAll()
                .Join(_iExamTaskClassRep.GetAll().Where(a=>a.Id == id), cls => cls.Id, etc => etc.ClassId, (cls, etc) => cls)
                .ToDictionaryAsync(a => a.Id, a => a.name);
            dto.Exams = exams;
            dto.Classes = classes;
            return dto;
        }

        /// <summary>
        /// 按照分页获取考试列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExamTaskItem>> GetPagination(ExamTaskPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                return new PaginationOutputDto<ExamTaskItem>();
            }

            var isTeacher = await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id);
            if (!cookie.IsAdmin && !isTeacher)
            {
                return new PaginationOutputDto<ExamTaskItem>();
            }

            var hasTimeRange = input.BeginTime.HasValue || input.EndTime.HasValue;
            var timeRangeTaskIdList = new List<Guid>();
            if (hasTimeRange)
            {
                timeRangeTaskIdList = await(from exam in _iExamExamRep.GetAll()
                    where (!input.BeginTime.HasValue || exam.BeginTime >= input.BeginTime)
                          && (!input.EndTime.HasValue || exam.EndTime <= input.EndTime)
                    select exam.TaskId).Distinct().ToListAsync();
            }

            var tasks = from task in _iExamTaskRep.GetAll()
                where (string.IsNullOrEmpty(input.Code) || task.Code.Contains(input.Code)) &&
                      (string.IsNullOrEmpty(input.Title) || task.Title.Contains(input.Title)) &&
                      (!input.CreateBeginTime.HasValue || task.CreateTime >= input.CreateBeginTime) &&
                      (!input.CreateEndTime.HasValue || task.CreateTime <= input.CreateEndTime) &&
                      (cookie.IsAdmin || task.CreatorId == cookie.Id )
                select task;

            var users = from u in _iUserBaseRep.GetAll()
                where (string.IsNullOrEmpty(input.UserFullName) || u.userFullName.Contains(input.UserFullName))
                      && (string.IsNullOrEmpty(input.UserLoginName) || u.userLoginName.Contains(input.UserLoginName))
                select u;

            var classRangeTaskIdList = new List<Guid>();
            if (input.ClassIds.Any())
            {
                var classes = from c in _iClassRep.GetAll()
                    where input.ClassIds.Contains(c.Id)
                    select c;
                //取符合发布班级的考试任务Id
                classRangeTaskIdList = await (from taskClass in _iExamTaskClassRep.GetAll()
                    join cls in classes on taskClass.ClassId equals cls.Id
                    select taskClass.Id).Distinct().ToListAsync();
            }
            //取交集
            List<Guid> ids = new List<Guid>();
            if (timeRangeTaskIdList.Any())
            {
                ids.AddRange(timeRangeTaskIdList);
            }

            if (classRangeTaskIdList.Any())
            {
                if (ids.Any())
                {
                    ids = ids.Intersect(classRangeTaskIdList).ToList();
                }
                else
                {
                    ids.AddRange(classRangeTaskIdList);
                }
            }
            
            var query = from task in tasks
                join u in users on task.CreatorId equals u.Id
                orderby task.CreateTime descending 
                where (!hasTimeRange && !input.ClassIds.Any()) || ids.Contains(task.Id)
                select new ExamTaskItem
                {
                    Id = task.Id,
                    Title = task.Title,
                    Code = task.Code,
                    CreatorId = task.CreatorId,
                    CreateTime = task.CreateTime,
                    UserFullName = u.userFullName,
                    UserLoginName = u.userLoginName
                };
            var rows = await query.Skip(input.skip).Take(input.pageSize).ToListAsync();

            //Dictionary<Guid, Dictionary<Guid, string>> classDic;
            var taskIds = rows.Select(a => a.Id).ToList();
            var classDic = (from taskClass in _iExamTaskClassRep.GetAll()
                join cls in _iClassRep.GetAll() on taskClass.ClassId equals cls.Id into clsTempTable
                from clsTemp in clsTempTable.DefaultIfEmpty()
                where taskIds.Contains(taskClass.Id)
                group new { clsTemp.Id, clsTemp.name } by taskClass.Id
            ).ToDictionary(a => a.Key, a => a.ToDictionary(b => b.Id, b => b.name));

            rows.ForEach(a =>
            {
                if (classDic.ContainsKey(a.Id))
                {
                    a.Classes = classDic[a.Id];
                }
            });

            return new PaginationOutputDto<ExamTaskItem>
            {
                rows = rows,
                total = await query.CountAsync()
            };
        }

        /// <summary>
        /// 创建考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ExamTaskOutputDto> Create(ExamTaskInputDto input)
        {
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

            if (input.IsCustomCode)
            {
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new UserFriendlyException("自定义考试编号不能为空");
                }

                if (_iExamTaskRep.GetAll().Any(a => a.Code == input.Code))
                {
                    throw new UserFriendlyException("已有相同的考试编号");
                }
            }

            #endregion

            if (!input.IsCustomCode)
            {
                input.Code = CreateNewCode();
            }

            var task = input.MapTo<ExamTask>();
            task.CreateTime = DateTime.Now;
            task.CreatorId = cookie.Id;

            await _iExamTaskRep.InsertAsync(task);

            return task.MapTo<ExamTaskOutputDto>();
        }

        /// <summary>
        /// 更新考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Update(ExamTaskInputDto input)
        {
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

            var entity = await _iExamTaskRep.FirstOrDefaultAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的考试任务");
            }

            if (!cookie.IsAdmin && entity.CreatorId != cookie.Id)
            {
                throw new UserFriendlyException("权限不够");
            }

            if (input.IsCustomCode)
            {
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new UserFriendlyException("自定义编号不能为空");
                }

                if (_iExamTaskRep.GetAll().Any(a => a.Code == input.Code && a.Id != input.Id))
                {
                    throw new UserFriendlyException("已有相同的考试编号");
                }
            }
            
            #endregion
            
            if (entity.IsCustomCode && !input.IsCustomCode)
            {
                input.Code = CreateNewCode();
            }

            input.MapTo(entity);
            await _iExamTaskRep.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Delete(IdListInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            #region 验证

            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && ! await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (!cookie.IsAdmin && await _iExamTaskRep.GetAll().AnyAsync(a=>input.idList.Contains(a.Id) && a.CreatorId != cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }
            
            #endregion

            var examDic = await _iExamExamRep.GetAll()
                .Where(a => input.idList.Contains(a.TaskId))
                .Select(a => new { a.Id, a.ExamName, a.ExamCode, a.TaskId })
                .ToDictionaryAsync(a => a.Id, a => new { a.ExamName, a.ExamCode, a.TaskId });

            if (examDic.Any())
            {
                var examIds = await _iExamGradeRep.GetAll()
                    .Where(a => examDic.Keys.Contains(a.examUid))
                    .Select(a => a.examUid).ToListAsync();

                //已有考试记录
                if (examIds.Any())
                {
                    var dic = examDic.Where(a => examIds.Contains(a.Key))
                        .GroupBy(a => a.Value.TaskId, a => new { a.Value.ExamCode, a.Value.ExamName })
                        .ToDictionary(a => a.Key, a => a);
                    var keys = dic.Keys.ToList();
                    var taskDic = await _iExamTaskRep.GetAll().Where(a => keys.Contains(a.Id))
                        .Select(a => new { a.Id, a.Code, a.Title })
                        .ToDictionaryAsync(a => a.Id, a => new { a.Code, a.Title });

                    var str = "删除操作失败！\n";
                    foreach (var taskId in dic.Keys)
                    {
                        var task = taskDic[taskId];
                        str = $"考试任务[{task.Code}]{task.Title}中：\n";
                        var examList = dic[taskId];
                        var strArr = new List<string>();
                        foreach (var exam in examList)
                        {
                            strArr.Add($"考试[{exam.ExamCode}]{exam.ExamName}");
                        }
                        str += string.Join(",\n", strArr) + ";\n已有考试记录，不可删除";
                    }
                    throw new UserFriendlyException(str);
                }

                //没有考试
                await _iExamExamRep.DeleteAsync(a => input.idList.Contains(a.TaskId));
                
            }
            await _iExamTaskClassRep.DeleteAsync(a => input.idList.Contains(a.Id));
            await _iExamTaskRep.DeleteAsync(a => input.idList.Contains(a.Id));
        }

        /// <summary>
        /// 发布考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Publish(ExamTaskClassInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            #region 验证

            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (!cookie.IsAdmin && await _iExamTaskRep.GetAll().AnyAsync(a => input.idList.Contains(a.Id) && a.CreatorId != cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (!await _iExamTaskRep.GetAll().AnyAsync(a => a.Id == input.TaskId))
            {
                throw new UserFriendlyException("无效的考试任务");
            }

            if (await _iExamTaskClassRep.GetAll()
                .AnyAsync(a => a.Id == input.TaskId && input.idList.Contains(a.ClassId)))
            {
                throw new UserFriendlyException("已有发布过相同信息的考试任务");
            }

            var idList = await _iClassRep.GetAll()
                .Where(a => input.idList.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();
            if (idList.Count != input.idList.Count)
            {
                throw new UserFriendlyException("有无效的班级");
            }

            #endregion

            var examIdList = await _iExamExamRep.GetAll().Where(a => a.TaskId == input.TaskId).Select(a => a.Id).ToListAsync();

            foreach (var classId in input.idList)
            {
                await _iExamTaskClassRep.InsertAsync(new ExamTaskClass {Id = input.TaskId, ClassId = classId});

                //班级获取相关老师ID
                //var teacherIdList = await _iClassTeacherRep.GetAll().Where(a => a.ClassId == classId).Select(a => a.UserId).ToListAsync();
                //if (teacherIdList.Any())
                //{
                //    foreach(var examId in examIdList)
                //    {
                //        //添加评卷人
                //        await AddJudgeUsers(new AddJudgeInputDto { ExamId = examId, UserIdList = teacherIdList });
                //    }
                //}
            }
        }

        /// <summary>
        /// 取消发布任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Unpublish(ExamTaskClassInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            #region 验证

            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (!cookie.IsAdmin && await _iExamTaskRep.GetAll().AnyAsync(a => input.idList.Contains(a.Id) && a.CreatorId != cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (!await _iExamTaskRep.GetAll().AnyAsync(a => a.Id == input.TaskId))
            {
                throw new UserFriendlyException("无效的考试任务");
            }

            var idList = await _iExamTaskClassRep.GetAll()
                .Where(a => a.Id == input.TaskId && input.idList.Contains(a.ClassId))
                .Select(a => a.ClassId)
                .ToListAsync();
            if (idList.Count != input.idList.Count)
            {
                throw new UserFriendlyException("有无效发布任务");
            }

            var classIdList = await (from exam in _iExamExamRep.GetAll()
                join grade in _iExamGradeRep.GetAll() on exam.Id equals grade.examUid
                join student in _iClassStudentRep.GetAll() on grade.userUid equals student.UserId
                where exam.TaskId == input.TaskId && input.idList.Contains(student.ClassId)
                select student.ClassId)
                .Distinct()
                .ToListAsync();

            if (classIdList.Any())
            {
                var classNameList = await _iClassRep.GetAll()
                    .Where(a => classIdList.Contains(a.Id))
                    .Select(a => a.name)
                    .ToListAsync();
                throw new UserFriendlyException($"无法取消发布！\n班级:{string.Join(",", classNameList)}中有学生已有考试数据。");
            }

            #endregion
            var examIdList = await _iExamExamRep.GetAll().Where(a => a.TaskId == input.TaskId).Select(a => a.Id).ToListAsync();
            foreach (var classId in input.idList)
            {
                await _iExamTaskClassRep.DeleteAsync(etc => etc.Id == input.TaskId && etc.ClassId == classId);
                
                //班级获取相关老师ID
                //var teacherIdList = await _iClassTeacherRep.GetAll().Where(a => a.ClassId == classId).Select(a => a.UserId).ToListAsync();

                //if (teacherIdList.Any())
                //{
                //    //删除评卷人
                //    await _iExamJudgeRep.DeleteAsync(a => examIdList.Contains(a.examUid) && teacherIdList.Contains(a.ownerUid));
                //}
            }
        }

        /// <summary>
        /// 获取考试任务发布候选班级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ClassOutDto>> GetCandidateClasses(Guid id)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var teacherClassIdList = new List<Guid>();
            if (!cookie.IsAdmin)
            {
                teacherClassIdList = await _iClassTeacherRep.GetAll()
                    .Where(a => a.UserId == cookie.Id)
                    .Select(a => a.ClassId)
                    .ToListAsync();
            }
            var classIds = await _iExamTaskClassRep.GetAll().Where(a => a.Id == id).Select(a => a.ClassId).ToListAsync();
            var classes = await _iClassRep.GetAll().Where(a => !classIds.Contains(a.Id) && (cookie.IsAdmin || teacherClassIdList.Contains(a.Id)))
                .Select(a => new ClassOutDto
                {
                    id = a.Id,
                    name = a.name,
                    facultyId = a.facultyId,
                    majorId = a.majorId
                }).ToListAsync();

            return classes;
        }

        /// <summary>
        /// 获取考试任务已发布的班级Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Guid>> GetClassIds(Guid id)
        {
            return await _iExamTaskClassRep.GetAll().Where(a => a.Id == id).Select(a => a.ClassId).ToListAsync();
        }

        /// <summary>
        /// 获取考试任务下所有的补考
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<RetestComboboxItem>> GetRetestList(Guid id)
        {
            var list = await _iExamExamRep.GetAll().Where(a => a.TaskId == id && a.examTypeCode== "exam_retest")
                .Select(a => new RetestComboboxItem
                {
                    TaskId = a.TaskId,
                    ExamId = a.Id,
                    Title = a.ExamName,
                    CreateTime = a.createTime
                }).OrderBy(a => a.CreateTime).ToListAsync();
            return list;
        }

        #region 评卷人设置
        /// <summary>
        /// 添加评卷人
        /// </summary>
        /// <param name="input"></param>
        public async Task AddJudgeUsers(AddJudgeInputDto input)
        {
            foreach (var userId in input.UserIdList)
            {
                await _iExamJudgeRep.InsertAsync(new ExamJudge
                {
                    Id = Guid.NewGuid(),
                    examUid = input.ExamId,
                    ownerUid = userId
                });
            }
        }
        /// <summary>
        /// 删除评卷人
        /// </summary>
        /// <param name="input"></param>
        public async Task DeleteJudgeUsers(IdListInputDto input)
        {
            foreach (var id in input.idList)
            {
                await _iExamJudgeRep.DeleteAsync(id);
            }
        }
        #endregion

        private string CreateNewCode()
        {
            var code = "ET000001";
            var entity = _iExamTaskRep.GetAll().Where(a => !a.IsCustomCode).OrderByDescending(a => a.CreateTime).FirstOrDefault();
            if (entity != null)
            {
                code = entity.Code;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (_iExamTaskRep.GetAll().Any(a => a.Code == code));
            }
            return code;
        }
    }
}