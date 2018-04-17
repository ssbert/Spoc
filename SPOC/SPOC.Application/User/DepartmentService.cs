using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.Core.Logging;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.Extensions;
using SPOC.Common.Helper;
using SPOC.User.Dto.Department;
using SPOC.User.Dto.Teacher;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Domain.Uow;

namespace SPOC.User
{
    /// <summary>
    /// 组织架构业务
    /// </summary>
    public  class DepartmentService: SPOCAppServiceBase, IDepartmentService
    {
        private readonly IRepository<Faculty,Guid> _iFcultyaRep;
        private readonly IRepository<Major, Guid> _iMajorRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<AdministrativeClass, Guid> _iAdministrativeClassRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherRep;
        private readonly IRepository<ClassTeacher, Guid> _iClassTeacherRep;
        private readonly IRepository<StudentInfo, Guid> _iStudentInfoRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private IUnitOfWork _iUnitOfWork;
        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger _logger { get; set; }

        /// <summary>
        /// 构造函数注入
        /// </summary>
        public DepartmentService(IRepository<Faculty, Guid> iFcultyaRep, IRepository<Major, Guid> iMajorRep, IRepository<Class, Guid> iClassRep, IRepository<UserBase, Guid> iUserBaseRep, IRepository<TeacherInfo, Guid> iTeacherRep, IRepository<ClassTeacher, Guid> iClassTeacherRep, IRepository<AdministrativeClass, Guid> iAdministrativeClassRep, IUnitOfWork iUnitOfWork, IRepository<StudentInfo, Guid> iStudentInfoRep, IRepository<ClassStudent, Guid> iClassStudentRep)
        {
            _iFcultyaRep = iFcultyaRep;
            _iMajorRep = iMajorRep;
            _iClassRep = iClassRep;
            _iUserBaseRep = iUserBaseRep;
            _iTeacherRep = iTeacherRep;
            _iClassTeacherRep = iClassTeacherRep;
            _iAdministrativeClassRep = iAdministrativeClassRep;
            _iUnitOfWork = iUnitOfWork;
            _iStudentInfoRep = iStudentInfoRep;
            _iClassStudentRep = iClassStudentRep;
            _logger = new NullLogger();
        }

        public EasyUiListResultDto<FacultyOutDto> GetFacultyUiList(FacultyInputDto input)
        {
            var result = new EasyUiListResultDto<FacultyOutDto>();
            try
            {

                var facultyList = _iFcultyaRep.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.name), a => a.name.Contains(input.name))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.code), a => a.code.Equals(input.code)).GroupJoin(_iUserBaseRep.GetAll(),f=>f.updateUserId,u=>u.Id,
                        (f, u) => new {f,u }).SelectMany(fl=> fl.u.DefaultIfEmpty(),(fl, u)=>new FacultyOutDto
                    {
                        id= fl.f.Id,
                        name=fl.f.name,
                        updateUser = u!=null? (u.userFullName == "" || u.userFullName == null ? u.userLoginName : u.userFullName) : "",
                        code= fl.f.code,
                        updateTime = fl.f.updateTime.ToString()
                        });
              
                result.total = facultyList.Count();
                result.rows = facultyList.OrderBy(input.OrderExpression).Skip(input.skip).Take(input.pageSize).ToList();
                result.rows.ForEach(a => a.updateTime = DateTimeUtil.ConvertToDateTimeStr(a.updateTime));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return result;
        }

        public async  Task InsertFaculty(FacultyInputDto input)
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var cookie = CookieHelper.GetLoginInUserInfo();
            await _iFcultyaRep.InsertAsync(new Faculty()
            {
                Id=Guid.NewGuid(),
                code = input.code,
                name=input.name,
                updateTime = DateTime.Now,
                updateUserId = new Guid(cookie.UserUid),
            });
        }

        public async Task<int> DeleteFaculty(BatchRequestInput input)
        {
            try
            {
                var successCount = 0;
                if (string.IsNullOrWhiteSpace(input.Id))
                {
                    return successCount;
                }
                var ids = input.Id.Split(',');
               
                foreach (string t in ids)
                {
                    Guid gid;
                    Guid.TryParse(t, out gid);
                    if (_iMajorRep.GetAll().Any(m => m.facultyId.Equals(gid)))
                    {
                        //院系下存在专业信息的不执行删除操作
                        continue;
                    }
                    successCount++;
                    await _iFcultyaRep.DeleteAsync(gid);
                }
                _iUnitOfWork.SaveChanges();
                return successCount;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return 0;
        }

        public void ModifyFaculty(FacultyInputDto input)
        {
            try
            {
                if (!LoginValidation.IsLogin())
                {
                    throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
                }
                var cookie = CookieHelper.GetLoginInUserInfo();
                if (string.IsNullOrWhiteSpace(input.id))
                {
                    return;
                }

                Guid gid;
                Guid.TryParse(input.id, out gid);
                var faculty = _iFcultyaRep.FirstOrDefault(d => d.Id == gid);
                if (faculty == null)
                {
                    return;
                }
                faculty.code = input.code;
                faculty.name = input.name;
                faculty.updateTime = DateTime.Now;
                faculty.updateUserId = new Guid(cookie.UserUid);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new UserFriendlyException("更新院系信息失败");
            }
        }

        public EasyUiListResultDto<MajorOutDto> GetMajorUiList(MajorInputDto input)
        {
            var result = new EasyUiListResultDto<MajorOutDto>();
            try
            {

                var query = (from major in _iMajorRep.GetAll()
                    join faculty in _iFcultyaRep.GetAll() on major.facultyId equals faculty.Id
                    join userBase in _iUserBaseRep.GetAll() on major.updateUserId equals userBase.Id
                    into temp1
                    from t1 in temp1.DefaultIfEmpty()
                             where major.name.Contains(input.name) && major.code.Contains(input.code) 
                             select new MajorOutDto
                    {
                       id= major.Id,
                       name=major.name,
                       code=major.code,
                       facultyId = faculty.Id,
                       facultyName = faculty.name,
                       updateTime = major.updateTime.ToString(),
                       updateUser = t1!=null? (t1.userFullName==""|| t1.userFullName==null ? t1.userLoginName : t1.userFullName) : ""
                             }).WhereIf(input.facultyId!=Guid.Empty,a=>a.facultyId.Equals(input.facultyId));
                result.total = query.Count();
                result.rows = query.OrderBy(input.OrderExpression).Skip(input.skip).Take(input.pageSize).ToList();
                result.rows.ForEach(a => a.updateTime = DateTimeUtil.ConvertToDateTimeStr(a.updateTime));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return result;
        }

        public async Task InsertMajor(MajorInputDto input)
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var cookie = CookieHelper.GetLoginInUserInfo();
            await _iMajorRep.InsertAsync(new Major()
            {
                Id = Guid.NewGuid(),
                code = input.code,
                name = input.name,
                facultyId = input.facultyId,
                updateTime = DateTime.Now,
                updateUserId = new Guid(cookie.UserUid),
            });
        }

        public async Task<int> DeleteMajor(BatchRequestInput input)
        {
            var successCount = 0;
            try
            {             
                if (string.IsNullOrWhiteSpace(input.Id))
                {
                    return successCount;
                }
                var ids = input.Id.Split(',');
                foreach (string t in ids)
                {
                    Guid gid;
                    Guid.TryParse(t, out gid);
                    if (_iClassRep.GetAll().Any(m => m.majorId.Equals(gid)))
                    {
                        //专业下存在班级信息的不执行删除操作
                        continue;
                    }
                    successCount++;
                    await _iMajorRep.DeleteAsync(gid);
                }
            
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return successCount;
        }

        public void ModifyMajor(MajorInputDto input)
        {
            try
            {
                if (!LoginValidation.IsLogin())
                {
                    throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
                }
                var cookie = CookieHelper.GetLoginInUserInfo();
                if (input.id==Guid.Empty)
                {
                    return;
                }
                var major = _iMajorRep.FirstOrDefault(d => d.Id == input.id);
                if (major == null)
                {
                    return;
                }
                major.code = input.code;
                major.name = input.name;
                major.facultyId = input.facultyId;
                major.updateTime = DateTime.Now;
                major.updateUserId = new Guid(cookie.UserUid);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new UserFriendlyException("更新专业信息失败");
            }
        }

        public EasyUiListResultDto<ClassOutDto> GetClassUiList(ClassInputDto input)
        {
            var result = new EasyUiListResultDto<ClassOutDto>();
            try
            {
                var query = (from classs in _iClassRep.GetAll()
                    join faculty in _iFcultyaRep.GetAll() on classs.facultyId equals faculty.Id
                    join major in _iMajorRep.GetAll() on classs.majorId equals major.Id
                             join userBase in _iUserBaseRep.GetAll() on classs.updateUserId equals userBase.Id
                    into temp1
                    from t1 in temp1.DefaultIfEmpty()
                    where  classs.name.Contains(input.name) 
                    select new ClassOutDto
                    {
                        studentNum= classs.studentNum,
                        id = classs.Id,
                        name = classs.name,
                        majorId = major.Id,
                        majorName = major.name,
                        facultyId = faculty.Id,
                        facultyName = faculty.name,
                        updateTime = classs.updateTime.ToString(),
                        updateUser = t1 != null ? (t1.userFullName == "" || t1.userFullName == null ? t1.userLoginName: t1.userFullName) : ""
                        
                    }).WhereIf(input.facultyId!=Guid.Empty,a=>a.facultyId.Equals(input.facultyId))
                    .WhereIf(input.majorId != Guid.Empty, a => a.majorId.Equals(input.majorId));
                result.total = query.Count();
                result.rows = query.OrderBy(input.OrderExpression).Skip(input.skip).Take(input.pageSize).ToList();
                result.rows.ForEach(a => a.updateTime = DateTimeUtil.ConvertToDateTimeStr(a.updateTime));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return result;
        }
        public EasyUiListResultDto<ClassOutDto> GetAdministrativeClassUiList(ClassInputDto input)
        {
            var result = new EasyUiListResultDto<ClassOutDto>();
            try
            {
                var query = (from classs in _iAdministrativeClassRep.GetAll()
                        join faculty in _iFcultyaRep.GetAll() on classs.facultyId equals faculty.Id
                        join major in _iMajorRep.GetAll() on classs.majorId equals major.Id
                        join userBase in _iUserBaseRep.GetAll() on classs.updateUserId equals userBase.Id
                        into temp1
                        from t1 in temp1.DefaultIfEmpty()
                        where classs.name.Contains(input.name)
                        select new ClassOutDto
                        {
                            studentNum = classs.studentNum,
                            id = classs.Id,
                            name = classs.name,
                            majorId = major.Id,
                            majorName = major.name,
                            facultyId = faculty.Id,
                            facultyName = faculty.name,
                            updateTime = classs.updateTime.ToString(),
                            updateUser = t1 != null ? (t1.userFullName == "" || t1.userFullName == null ? t1.userLoginName : t1.userFullName) : ""

                        }).WhereIf(input.facultyId != Guid.Empty, a => a.facultyId.Equals(input.facultyId))
                    .WhereIf(input.majorId != Guid.Empty, a => a.majorId.Equals(input.majorId));
                result.total = query.Count();
                result.rows = query.OrderBy(input.OrderExpression).Skip(input.skip).Take(input.pageSize).ToList();
                result.rows.ForEach(a => a.updateTime = DateTimeUtil.ConvertToDateTimeStr(a.updateTime));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return result;
        }
        public async Task InsertClass(ClassInputDto input)
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (input.classType.Equals("administrative"))
            {
                await _iAdministrativeClassRep.InsertAsync(new AdministrativeClass
                {
                    Id = Guid.NewGuid(),
                    facultyId = input.facultyId,
                    majorId = input.majorId,
                    name = input.name,
                    studentNum = input.studentNum,
                    updateTime = DateTime.Now,
                    updateUserId = new Guid(cookie.UserUid),
                });
            }
            else
            {
                await _iClassRep.InsertAsync(new Class
                {
                    Id = Guid.NewGuid(),
                    facultyId = input.facultyId,
                    majorId = input.majorId,
                    name = input.name,
                    studentNum = input.studentNum,
                    updateTime = DateTime.Now,
                    updateUserId = new Guid(cookie.UserUid),
                });
            }
           
        }

        public async Task<int> DeleteClass(BatchRequestInput input)
        {
            int success = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(input.Id))
                {
                    return success;
                }
                var ids = input.Id.Split(',');
                foreach (string t in ids)
                {
                    Guid gid;
                    Guid.TryParse(t, out gid);
                    if(_iClassStudentRep.GetAll().Any(a=>a.ClassId.Equals(gid)))
                        continue;
                    await _iClassRep.DeleteAsync(gid);
                    await _iClassTeacherRep.DeleteAsync(a=>a.ClassId.Equals(gid));
                    success++;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return success;
        }
        public async Task<int> DeleteAdministrativeClass(BatchRequestInput input)
        {
            int success = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(input.Id))
                {
                    return success;
                }
                var ids = input.Id.Split(',');
                foreach (string t in ids)
                {
                    Guid gid;
                    Guid.TryParse(t, out gid);
                    if(_iStudentInfoRep.GetAll().Any(a=>a.classId.Equals(gid)))
                        continue;
                    await _iAdministrativeClassRep.DeleteAsync(gid);
                    await _iClassTeacherRep.DeleteAsync(a => a.ClassId.Equals(gid));
                    success++;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return success;
        }
        public void ModifyClass(ClassInputDto input)
        {
            try
            {
                if (!LoginValidation.IsLogin())
                {
                    throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
                }
                var cookie = CookieHelper.GetLoginInUserInfo();
                if (input.id==Guid.Empty)
                {
                    return;
                }
                if (input.classType.Equals("administrative"))
                {
                    var classes = _iAdministrativeClassRep.FirstOrDefault(d => d.Id == input.id);
                    if (classes == null)
                    {
                        return;
                    }
                    classes.studentNum = input.studentNum;
                    classes.majorId = input.majorId;
                    classes.name = input.name;
                    classes.facultyId = input.facultyId;
                    classes.updateTime = DateTime.Now;
                    classes.updateUserId = new Guid(cookie.UserUid);
                }
                else
                {
                    var classes = _iClassRep.FirstOrDefault(d => d.Id == input.id);
                    if (classes == null)
                    {
                        return;
                    }
                    classes.studentNum = input.studentNum;
                    classes.majorId = input.majorId;
                    classes.name = input.name;
                    classes.facultyId = input.facultyId;
                    classes.updateTime = DateTime.Now;
                    classes.updateUserId = new Guid(cookie.UserUid);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new UserFriendlyException("更新班级信息失败");
            }
        }

        public EasyUiListResultDto<ClassTeacherOutDto> GetTeachersByClassId(ClassInputDto input)
        {
            var resultList = new EasyUiListResultDto<ClassTeacherOutDto>();
            try
            {
                input.skip = input.rows * (input.page - 1);
                input.pageSize = input.rows;
                var classId = input.id;
                var result = from userBase in _iUserBaseRep.GetAll()
                    join teacherInfo in _iTeacherRep.GetAll() on userBase.Id equals teacherInfo.userId
                    join ct in _iClassTeacherRep.GetAll() on userBase.Id equals
                    ct.UserId
                    where
                    ct.ClassId == classId && !teacherInfo.isDel
                    orderby ct.CreateTime descending
                    select new ClassTeacherOutDto
                    {
                        Id = ct.Id,
                        UserLoginName = userBase.userLoginName,
                        UserFullName = userBase.userFullName,
                        Gender=userBase.userGender,
                        CreateTime = ct.CreateTime.ToString()
                    };
                resultList.total = result.LongCount();
                resultList.rows = result.OrderBy(input.OrderExpression).Skip(input.skip).Take(input.pageSize).ToList();
                resultList.rows.ForEach(a => a.CreateTime = DateTimeUtil.ConvertToDateTimeStr(a.CreateTime));

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                
            }
            return resultList;


        }
        public async Task AddTeachers(RigthTeacherInputDto dto)
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var relationId = dto.relationId;
            foreach (var model in dto.TeacherInputDtos)
            {

                await _iClassTeacherRep.InsertAsync(
                        new ClassTeacher
                        {
                            Id = Guid.NewGuid(),
                            ClassId = relationId.TryParseGuid(),
                            UserId = model.userId.TryParseGuid(),
                            CreateUserId = CookieHelper.GetLoginInUserInfo().Id,
                            CreateTime = DateTime.Now
                        });
               
            }
           
        }

        public async Task DelClassTeacher(BatchRequestInput input)
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            for (int i = 0; i < input.Id.Split(',').Length; i++)
            {
                var id = new Guid(input.Id.Split(',')[i]);
                await _iClassTeacherRep.DeleteAsync(id);
            }
            
        }
        /// <summary>
        /// 备选栏老师列表
        /// </summary>
        /// <returns></returns>
        public EasyUiListResultDto<LeftTeacherInfoOutDto> GetLeftTeachers(LeftTeacherInfoDto dto)
        {
            try
            {
                var cid = dto.RelationId.TryParseGuid();
                //排除当前班级存在的教师
                var users = (from pt in _iClassTeacherRep.GetAll()
                    where pt.ClassId == cid
                    select pt.UserId);
                dto.skip = dto.rows * (dto.page - 1);
                dto.pageSize = dto.rows;
                var result = (from userBase in _iUserBaseRep.GetAll()
                    join teacherInfo in _iTeacherRep.GetAll() on userBase.Id equals teacherInfo.userId
                    where userBase.identity == 2 && !teacherInfo.isDel && !users.Contains(userBase.Id)
                    orderby userBase.userLoginName descending
                    select new LeftTeacherInfoOutDto
                    {
                        UserId = userBase.Id.ToString(),
                        UserLoginName = userBase.userLoginName,
                        UserFullName = userBase.userFullName,
                        Gender = userBase.userGender
                    });
                var selectUserIds =dto.Teachers.Select(t => t.UserId).ToList();
                result = result.WhereIf(!string.IsNullOrEmpty(dto.UserLoginName),
                        p => p.UserLoginName.Contains(dto.UserLoginName.Trim()))
                    .WhereIf(!string.IsNullOrEmpty(dto.UserFullName),
                        p => p.UserFullName.Contains(dto.UserFullName.Trim()))
                    .WhereIf(dto.Teachers.Count != 0,p=> !selectUserIds.Contains(p.UserId)
                    );

                return new EasyUiListResultDto<LeftTeacherInfoOutDto>
                {
                    total = result.LongCount(),
                    rows = result.OrderBy(dto.OrderExpression).Skip(dto.skip).Take(dto.pageSize).ToList()
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return new EasyUiListResultDto<LeftTeacherInfoOutDto>();

        }
        public IEnumerable<object> CmbFaculty()
        {
            var obj =
                _iFcultyaRep.GetAll()
                    .OrderBy(c => c.updateTime)
                    .Select(c => new { c.Id, c.name }).ToList();
            return obj;
        }

        public IEnumerable<object> CmbMajor(string facultyId)
        {
            var obj =
                _iMajorRep.GetAll().WhereIf(!string.IsNullOrEmpty(facultyId),f=>f.facultyId.Equals(new Guid(facultyId)))
                    .OrderBy(c => c.updateTime)
                    .Select(c => new { c.Id, c.name }).ToList();
            return obj;
        }
        public IEnumerable<object> CmbClass(string majorId)
        {
            var obj =
               _iClassRep.GetAll().WhereIf(!string.IsNullOrEmpty(majorId), f => f.majorId.Equals(new Guid(majorId)))
                    .OrderBy(c => c.updateTime)
                    .Select(c => new { c.Id, c.name }).ToList();
            return obj;
        }
        public IEnumerable<object> CmbAdministrativeClass(string majorId)
        {
            var obj =
                _iAdministrativeClassRep.GetAll().WhereIf(!string.IsNullOrEmpty(majorId), f => f.majorId.Equals(new Guid(majorId)))
                    .OrderBy(c => c.updateTime)
                    .Select(c => new { c.Id, c.name }).ToList();
            return obj;
        }
        /// <summary>
        /// 获取组织架构
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItemDto> GetDepartmentTree()
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var cookie = CookieHelper.GetLoginInUserInfo();
            //查询当前登录用户可查看的班级列表
            
            var classes = cookie.IsAdmin ? _iClassRep.GetAll().ToList():_iClassRep.GetAll()
                .Join(_iClassTeacherRep.GetAll(), c => c.Id, ct => ct.ClassId, (c, ct) => new {c, ct})
                .Where(@t => @t.ct.UserId == cookie.Id)
                .Select(@t => @t.c).ToList();
            var permissionIds = classes.Select(a => a.facultyId).ToList();
            var fcultyList = cookie.IsAdmin ? _iFcultyaRep.GetAll().Select(f=> new { f.Id, pid = Guid.Empty, f.name }).ToList() : _iFcultyaRep.GetAll().
                Where(f=> permissionIds.Any(c=>c.Equals(f.Id))).Select(f => new { f.Id, pid = Guid.Empty, f.name }).ToList();
            permissionIds = classes.Select(a => a.majorId).ToList();
            var majorList = cookie.IsAdmin ? _iMajorRep.GetAll().Select(f => new { f.Id, pid = f.facultyId, f.name }).ToList():
                _iMajorRep.GetAll().Where(m=> permissionIds.Any(c => c.Equals(m.Id))).Select(f => new { f.Id, pid = f.facultyId, f.name }).ToList();

            var classList = classes.Select(f => new { f.Id, pid = f.majorId, f.name }).ToList();
            //var departments = fcultyList.Union(majorList).Union(classList);
            var listItmes = new List<SelectListItemDto>();
            fcultyList.ForEach(f =>
            {
                var item = new SelectListItemDto
                {
                    id = f.Id.ToString(),
                    text = f.name.ToString(),
                    children = majorList.Where(m => m.pid.Equals(f.Id)).Select(m => new SelectListItemDto
                    {
                        id = m.Id.ToString(),
                        text = m.name.ToString(),
                        children = classList.Where(c => c.pid.Equals(m.Id)).Select(c => new SelectListItemDto
                        {
                            id = c.Id.ToString(),
                            text = c.name.ToString()
                        }).ToList()
                    }).ToList()
                };
                //移除班级为空的节点
                for (var i = item.children.Count - 1; i >= 0; i--)
                {
                    if (item.children[i].children.Count == 0)
                    {
                        item.children.RemoveAt(i);
                    }
                }
                if (item.children.Count > 0)
                    listItmes.Add(item);
            });
           

            return listItmes;

        }
        /// <summary>
        /// 获取组织架构
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItemDto> GetAllDepartmentTree()
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException(-1,"未登录系统或登录已经失效，请重新登录");
            }

            var classes = _iClassRep.GetAll().ToList() ;

            var fcultyList = _iFcultyaRep.GetAll().Select(f => new {f.Id, pid = Guid.Empty, f.name}).ToList();
           
            var majorList =_iMajorRep.GetAll().Select(f => new { f.Id, pid = f.facultyId, f.name }).ToList() ;

            var classList = classes.Select(f => new { f.Id, pid = f.majorId, f.name }).ToList();
            var listItmes = new List<SelectListItemDto>();
            fcultyList.ForEach(f =>
            {
                var item = new SelectListItemDto
                {
                    id = f.Id.ToString(),
                    text = f.name.ToString(),
                    children = majorList.Where(m => m.pid.Equals(f.Id)).Select(m => new SelectListItemDto
                    {
                        id = m.Id.ToString(),
                        text = m.name.ToString(),
                        children = classList.Where(c => c.pid.Equals(m.Id)).Select(c => new SelectListItemDto
                        {
                            id = c.Id.ToString(),
                            text = c.name.ToString()
                        }).ToList()
                    }).ToList()
                };
                //移除班级为空的节点
                for (var i = item.children.Count - 1; i >= 0; i--)
                {
                    if (item.children[i].children.Count==0)
                    {
                        item.children.RemoveAt(i);
                    }
                }
                if(item.children.Count>0)
                listItmes.Add(item);
            });
           
            return listItmes;

        }
        /// <summary>
        /// 若用户为管理员，则获取所有班级
        /// 若用户为教师，则获取关联的所有班级
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClassOutDto>> GetAllClass()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var idList = new List<Guid>();
            if (!cookie.IsAdmin)
            {
                idList = await _iClassTeacherRep.GetAll()
                    .Where(a => a.UserId == cookie.Id)
                    .Select(a => a.ClassId)
                    .ToListAsync();
            }
            var list = await _iClassRep.GetAll()
                .Where(a=>cookie.IsAdmin || idList.Contains(a.Id))
                .OrderBy(a => a.name)
                .Select(a=>new ClassOutDto
                {
                    id = a.Id,
                    facultyId = a.facultyId,
                    majorId = a.majorId,
                    name = a.name
                })
                .ToListAsync();
            return list;
        }

        /// <summary>
        /// 根据Class的Id列表获取Class的相关信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<ClassOutDto>> GetClassesByIdList(IdListInputDto input)
        {
            var list = await (from c in _iClassRep.GetAll()
                join f in _iFcultyaRep.GetAll() on c.facultyId equals f.Id
                join m in _iMajorRep.GetAll() on c.majorId equals m.Id
                where input.idList.Contains(c.Id)
                orderby c.name
                select new ClassOutDto
                {
                    id = c.Id,
                    facultyId = c.facultyId,
                    facultyName = f.name,
                    majorId = c.majorId,
                    majorName = m.name,
                    name = c.name
                }).ToListAsync();
            return list;
        }
        /// <summary>
        /// 获取所有院校的数据
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<System.Web.Mvc.SelectListItem>> GetAllClassSelectItem(bool loadEmpty)
        {

            var classList = await (_iClassRep.GetAll()
                .Join(_iFcultyaRep.GetAll(), c => c.facultyId, m => m.Id, (c, m) => new System.Web.Mvc.SelectListItem()
                {
                    Selected = false,
                    Text = c.name + "(" + m.name + ")",
                    Value = c.Id.ToString()
                })).ToListAsync();

            if (loadEmpty)
                classList.Insert(0, new System.Web.Mvc.SelectListItem { Text = @"--请选择班级--", Value = "" });
            return classList;

        }
    }
}
