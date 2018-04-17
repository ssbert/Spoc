using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Pagination;
using SPOC.Exam;
using SPOC.Exercises.Dto;
using SPOC.User;
using SPOC.User.Dto.Department;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习服务接口实现
    /// </summary>
    public class ExerciseManageService:ApplicationService, IExerciseManageService
    {
        private readonly IRepository<Exercise, Guid> _iExerciseRep;
        private readonly IRepository<ExerciseClass, Guid> _iExerciseClassRep;
        private readonly IRepository<ExerciseRecord, Guid> _iExerciseRecordRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<ClassTeacher, Guid> _iClassTeacherRep;

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseManageService(IRepository<Exercise, Guid> iExerciseRep, IRepository<ExerciseClass, Guid> iExerciseClassRep, 
            IRepository<ExerciseRecord, Guid> iExerciseRecordRep, IRepository<ExamQuestion, Guid> iExamQuestionRep, 
            IRepository<Class, Guid> iClassRep, IRepository<TeacherInfo, Guid> iTeacherInfoRep, 
            IRepository<UserBase, Guid> iUserBaseRep, IRepository<ClassStudent, Guid> iClassStudentRep, 
            IRepository<ClassTeacher, Guid> iClassTeacherRep)
        {
            _iExerciseRep = iExerciseRep;
            _iExerciseClassRep = iExerciseClassRep;
            _iExerciseRecordRep = iExerciseRecordRep;
            _iExamQuestionRep = iExamQuestionRep;
            _iClassRep = iClassRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iUserBaseRep = iUserBaseRep;
            _iClassStudentRep = iClassStudentRep;
            _iClassTeacherRep = iClassTeacherRep;
        }
        #endregion
        
        /// <summary>
        /// 获取一个练习
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExerciseOutputDto> Get(Guid id)
        {
            var dto = new ExerciseOutputDto();
            var exercise = await _iExerciseRep.FirstOrDefaultAsync(id);
            if (exercise == null)
            {
                return dto;
            }
            exercise.MapTo(dto);
            var question = await _iExamQuestionRep.FirstOrDefaultAsync(exercise.QuestionId);
            if (question != null)
            {
                dto.QuestionText = question.questionPureText;
            }
            dto.Classes = await _iClassRep.GetAll()
                .Join(_iExerciseClassRep.GetAll().Where(a => a.Id == id), cls => cls.Id, etc => etc.ClassId, (cls, etc) => cls)
                .ToDictionaryAsync(a => a.Id, a => a.name);
            return dto;
        }

        /// <summary>
        /// 获取练习分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExerciseItem>> GetPagination(ExercisePaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                return new PaginationOutputDto<ExerciseItem>();
            }

            var isTeacher = await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id);
            if (!cookie.IsAdmin && !isTeacher)
            {
                return new PaginationOutputDto<ExerciseItem>();
            }

            var exercises = from exercise in _iExerciseRep.GetAll()
                where (string.IsNullOrEmpty(input.Title) || exercise.Title.Contains(input.Title)) &&
                      (!input.BeginTime.HasValue || exercise.CreateTime >= input.BeginTime) &&
                      (!input.EndTime.HasValue || exercise.EndTime <= input.EndTime) &&
                      (!input.CreateBeginTime.HasValue || exercise.CreateTime >= input.CreateBeginTime) &&
                      (!input.CreateEndTime.HasValue || exercise.CreateTime <= input.CreateEndTime) &&
                      (cookie.IsAdmin || exercise.CreatorId == cookie.Id)
                select exercise;

            var users = from u in _iUserBaseRep.GetAll()
                where (string.IsNullOrEmpty(input.UserFullName) || u.userFullName.Contains(input.UserFullName)) 
                && (string.IsNullOrEmpty(input.UserLoginName) || u.userLoginName.Contains(input.UserLoginName))
                select u;

            var classes = from c in _iClassRep.GetAll()
                where !input.ClassIds.Any() || input.ClassIds.Contains(c.Id)
                select c;

            var classList = await (from ec in _iExerciseClassRep.GetAll()
                join c in classes on ec.ClassId equals c.Id
                select new { ec.Id, ClassId=c.Id, ClassName = c.name }).ToListAsync();

            var idList = classList.Select(c=>c.Id).Distinct().ToList();

            var query = from e in exercises
                join u in users on e.CreatorId equals u.Id
                where !input.ClassIds.Any() || idList.Contains(e.Id)
                orderby e.CreateTime descending 
                select new ExerciseItem
                {
                    Id = e.Id,
                    Title = e.Title,
                    CreateTime = e.CreateTime,
                    EndTime = e.EndTime,
                    CreatorId = e.CreatorId,
                    UserFullName = u.userFullName,
                    UserLoginName = u.userLoginName
                };

            var rows = await query.Skip(input.skip).Take(input.pageSize).ToListAsync();

            rows.ForEach(r =>
            {
                var classDic= classList.Where(c => c.Id == r.Id).ToDictionary(k => k.ClassId, v => v.ClassName);
                r.Classes = classDic;
            });

            return new PaginationOutputDto<ExerciseItem>
            {
                rows = rows,
                total = await query.CountAsync()
            };
        }
        
        /// <summary>
        /// 创建一个练习
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ExerciseOutputDto> Create(ExerciseInputDto input)
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
            var questionText = await _iExamQuestionRep.GetAll()
                .Where(a => a.Id == input.QuestionId && a.questionBaseTypeCode == EnumQuestionBaseTypeCode.Program)
                .Select(a => a.questionPureText)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(questionText))
            {
                throw new UserFriendlyException("无效的试题");
            }

            #endregion

            var entity = input.MapTo<Exercise>();
            entity.CreatorId = cookie.Id;
            await _iExerciseRep.InsertAsync(entity);

            var dto = entity.MapTo<ExerciseOutputDto>();
            dto.QuestionText = questionText;
            return dto;
        }
        
        /// <summary>
        /// 更新一个练习
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Update(ExerciseInputDto input)
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

            var entity = await _iExerciseRep.FirstOrDefaultAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的练习");
            }

            if (!cookie.IsAdmin && entity.CreatorId != cookie.Id)
            {
                throw new UserFriendlyException("权限不够");
            }

            var questionText = await _iExamQuestionRep.GetAll()
                .Where(a => a.Id == input.QuestionId && a.questionBaseTypeCode == EnumQuestionBaseTypeCode.Program)
                .Select(a => a.questionPureText)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(questionText))
            {
                throw new UserFriendlyException("无效的试题");
            }

            #endregion

            input.MapTo(entity);
            await _iExerciseRep.UpdateAsync(entity);
        }
        
        /// <summary>
        /// 删除一个练习
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(Guid id)
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

            if (!cookie.IsAdmin && await _iExerciseRep.GetAll().AnyAsync(a => a.Id == id && a.CreatorId != cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (await _iExerciseRecordRep.GetAll().AnyAsync(a => a.ExerciseId == id))
            {
                throw new UserFriendlyException("已有练习记录，无法删除");
            }
            #endregion

            await _iExerciseClassRep.DeleteAsync(id);
            await _iExerciseRep.DeleteAsync(id);
        }

        /// <summary>
        /// 发布练习
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Publish(ExerciseClassInputDto input)
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

            if (!cookie.IsAdmin && await _iExerciseRep.GetAll().AnyAsync(a => input.idList.Contains(a.Id) && a.CreatorId != cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (!await _iExerciseRep.GetAll().AnyAsync(a => a.Id == input.TaskId))
            {
                throw new UserFriendlyException("无效的练习");
            }

            if (await _iExerciseClassRep.GetAll().AnyAsync(a => a.Id == input.TaskId && input.idList.Contains(a.ClassId)))
            {
                throw new UserFriendlyException("已有发布过相同信息的练习");
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

            foreach (var classId in input.idList)
            {
                await _iExerciseClassRep.InsertAsync(new ExerciseClass {Id = input.TaskId, ClassId = classId});
            }
        }
        
        /// <summary>
        /// 取消发布考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Unpublish(ExerciseClassInputDto input)
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

            if (!cookie.IsAdmin && await _iExerciseRep.GetAll().AnyAsync(a => input.idList.Contains(a.Id) && a.CreatorId != cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (!await _iExerciseRep.GetAll().AnyAsync(a => a.Id == input.TaskId))
            {
                throw new UserFriendlyException("无效的练习");
            }

            var idList = await _iExerciseClassRep.GetAll()
                .Where(a => a.Id == input.TaskId && input.idList.Contains(a.ClassId))
                .Select(a => a.ClassId)
                .ToListAsync();
            if (idList.Count != input.idList.Count)
            {
                throw new UserFriendlyException("有无效发布练习");
            }

            var classIdList = await (from er in _iExerciseRecordRep.GetAll()
                    join student in _iClassStudentRep.GetAll() on er.UserId equals student.UserId
                    where er.ExerciseId == input.TaskId && input.idList.Contains(student.ClassId)
                    select student.ClassId)
                .Distinct()
                .ToListAsync();

            if (classIdList.Any())
            {
                var classNameList = await _iClassRep.GetAll()
                    .Where(a => classIdList.Contains(a.Id))
                    .Select(a => a.name)
                    .ToListAsync();
                throw new UserFriendlyException($"无法取消发布！\n班级:{string.Join(",", classNameList)}中有学生已有练习数据。");
            }

            #endregion

            foreach (var classId in input.idList)
            {
                await _iExerciseClassRep.DeleteAsync(ec => ec.Id == input.TaskId && ec.ClassId == classId);
            }
        }
        
        /// <summary>
        /// 获取练习发布候选班级
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
            var classIds = await _iExerciseClassRep.GetAll().Where(a=>a.Id == id).Select(a=>a.ClassId).ToListAsync();
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
        /// 获取练习已发布的班级Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Guid>> GetClassIds(Guid id)
        {
            return await _iExerciseClassRep.GetAll().Where(a => a.Id == id).Select(a => a.ClassId).ToListAsync();
        }
    }
}