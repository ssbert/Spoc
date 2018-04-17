using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Newtonsoft.Json;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.Encrypt;
using SPOC.Common.Enum;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.SysSetting;
using SPOC.User.Dto.Common;
using SPOC.User.Dto.Teacher;
using SPOC.User.Dto.UserInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Runtime.Validation;
using SPOC.Common.Cookie;

namespace SPOC.User
{
    [DisableValidation]
    public class TeacherInfoService : SPOCAppServiceBase, ITeacherInfoService
    {
        private static string Pattern = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly IRepository<TeacherInfo, Guid> _teacherInfoRepository;
        private readonly IRepository<ClassTeacher, Guid> _classTeacherRep;
        private readonly IRepository<UserBase, Guid> _userRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Class, Guid> _classRep;
        private readonly IUserInfoApiService _userInfoApiService;
        private readonly ISiteSetService _iSiteSetService;
        private readonly IUserInfoService _userInfoService;
        private readonly IRepository<UserRole, Guid> _iUserRoleRep;
        private readonly IRepository<RoleManage, Guid> _iRoleManageRep;
        private readonly IDepartmentService _iDepartmentService;

        public TeacherInfoService(
            IRepository<TeacherInfo, Guid> teacherInfoRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserBase, Guid> userRepository,
            IUserInfoApiService userInfoApiService,
             IUserInfoService userInfoService,
            ISiteSetService iSiteSetService,
            IRepository<UserRole, Guid> iUserRoleRep, 
            IRepository<RoleManage, Guid> iRoleManageRep, IRepository<ClassTeacher, Guid> classTeacherRep, IRepository<Class, Guid> classRep, IDepartmentService iDepartmentService)
        {
            _teacherInfoRepository = teacherInfoRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;  
            _userInfoApiService = userInfoApiService;
            _iSiteSetService = iSiteSetService;
            _userInfoService = userInfoService;
            _iUserRoleRep = iUserRoleRep;
            _iRoleManageRep = iRoleManageRep;
            _classTeacherRep = classTeacherRep;
            _classRep = classRep;
            _iDepartmentService = iDepartmentService;
        }

        #region 教师管理
        public EasyUiListResultDto<TeacherInfoDto> GetTeacherInfoByGuid(TeacherInfoInputDto input)        {

            input = StringHelper.TrimStr(input);
          
            //教师班级
            var classes = (from ct in _classTeacherRep.GetAll()
                join c in _classRep.GetAll() on ct.ClassId equals c.Id
                group new {ct.UserId, ct.Id, c.name} by ct.UserId  
                 
                ).ToDictionary(a => a.Key,
                a => a
                    .Select(b => new { b.Id,b.UserId,b.name})
                    .Distinct().ToList()).Select(a=>new {userId=a.Key,name=a.Value.Select(c=>c.name).Aggregate((right, left) => right + "," + left)});
            

        
             var query = from a in _teacherInfoRepository.GetAll().Where(a => a.isDel == false)
                .WhereIf(!string.IsNullOrWhiteSpace(input.teacherCode),
                    m => !string.IsNullOrEmpty(m.teacherCode) && m.teacherCode.Contains(input.teacherCode))
                .WhereIf(!string.IsNullOrWhiteSpace(input.teacherTitle),
                    s => !string.IsNullOrEmpty(s.teacherTitle) && s.teacherTitle.Contains(input.teacherTitle))
                .WhereIf(input.teacherJobStatusCode.HasValue, s => s.teacherJobStatusCode == input.teacherJobStatusCode)
                .Where(a => a.isDel == false)

                join b in _userRepository.GetAll().Where(s => s.identity == 2)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_login_name),
                        s =>
                            !string.IsNullOrEmpty(s.userLoginName) &&
                            s.userLoginName.ToLower().Contains(input.user_login_name.ToLower()))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_mobile),
                        s => !string.IsNullOrEmpty(s.userMobile) && s.userMobile.Contains(input.user_mobile))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_gender), s => s.userGender == input.user_gender)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_name),s=> !string.IsNullOrEmpty(s.userFullName) && s.userFullName.Contains(input.user_name))
                    on a.userId equals b.Id
                    join c in classes on a.userId equals  c.userId into temp
                    from ct in temp.DefaultIfEmpty()
                select new TeacherInfoDto()
                {
                    Id = a.Id,
                    user_id = a.userId,
                    user_login_name = b.userLoginName,
                    user_password = b.userPassWord,
                    user_name = b.userFullName,
                    user_mobile = b.userMobile,
                    user_email = b.userEmail,
                    user_gender = b.userGender,
                    teacherCode = a.teacherCode,
                    teacherTitle = a.teacherTitle,
                    teacherJobStatusCode = a.teacherJobStatusCode,
                    teacherProfessionalDirection = a.teacherProfessionalDirection,
                    teacherBirthday = b.userBirthday,
                    teacherIdCode = b.userIdcard,
                    teacherNational = b.userNational,
                    teacherPolitical = b.userPolitical,
                    teacherEntryDate = a.teacherEntryDate,
                    teacherStartworkDate = a.teacherStartworkDate,
                    teacherEduAge = a.teacherEduAge,
                    teacherEducation = a.teacherEducation,
                    teacherAcademicDegree = a.teacherAcademicDegree,
                    teacherGraduateSchool = a.teacherGraduateSchool,
                    teacherGraduateDate = a.teacherGraduateDate,
                    teacherStudyProfessional = a.teacherStudyProfessional,
                    teacherIsAddCourse = a.teacherIsAddCourse,
                    teacherInviteCode = a.teacherInviteCode
                    ,
                    teacherIsRecommend = a.teacherIsRecommend,
                    teacherIsDisplay = a.teacherIsDisplay,
                    teacherPersonalResume = a.teacherPersonalResume,
                    teacherEnbleFlag = a.teacherEnbleFlag,
                    createTime = a.createTime,
                    updateTime = a.updateTime,
                    approvalStatus = b.approvalStatus,
                    newMessageNum = b.newMessageNum,
                    newNotificationNum = b.newNotificationNum
                    ,
                    about = b.about,
                    signature = b.signature,
                    largeAvatar = b.largeAvatar,
                    mediumAvatar = b.mediumAvatar,
                    smallAvatar = b.smallAvatar
                    ,
                    loginIp = b.loginIp,
                    loginTime = b.loginTime,
                    classNames = ct?.name

                };


            var totalQuery =query;
          
            var res = string.IsNullOrWhiteSpace(input.OrderExpression) ? totalQuery.OrderByDescending(a => a.createTime).Skip(input.Skip).Take(input.PageSize).ToList() : totalQuery.OrderBy(input.OrderExpression).Skip(input.Skip).Take(input.PageSize).ToList();

            return new EasyUiListResultDto<TeacherInfoDto>
            {
                total = totalQuery.LongCount(),
                rows = res
            };
        }


        public TeacherInfoDto GetTeacherInfoDtoBuUserId(UserInfoQueryInputDto model)
        {

            var query = from a in _teacherInfoRepository.GetAll().Where(a => a.isDel == false)
             .Where(a => a.isDel == false)
             .WhereIf(model.id != Guid.Empty, g => g.Id == model.id)

                        join b in _userRepository.GetAll().Where(s => s.identity == 2)
                        .WhereIf(model.userId != Guid.Empty, u => u.Id == model.userId)
                         .WhereIf(!string.IsNullOrEmpty(model.userName), u => u.userLoginName == model.userName)
                         on a.userId equals b.Id
                        select new TeacherInfoDto()
                        {
                            Id = a.Id,
                            user_id = a.userId,
                            user_login_name = b.userLoginName,
                            user_password = b.userPassWord,
                            user_name = b.userFullName,
                            user_mobile = b.userMobile,
                            user_email = b.userEmail,
                            user_gender = b.userGender,
                            teacherCode = a.teacherCode,
                            teacherTitle = a.teacherTitle,
                            teacherJobStatusCode = a.teacherJobStatusCode,
                            teacherProfessionalDirection = a.teacherProfessionalDirection,
                            teacherBirthday = b.userBirthday,
                            teacherIdCode = b.userIdcard,
                            teacherNational = b.userNational,
                            teacherPolitical = b.userPolitical,
                            teacherEntryDate = a.teacherEntryDate,
                            teacherStartworkDate = a.teacherStartworkDate,
                            teacherEduAge = a.teacherEduAge,
                            teacherEducation = a.teacherEducation,
                            teacherAcademicDegree = a.teacherAcademicDegree,
                            teacherGraduateSchool = a.teacherGraduateSchool,
                            teacherGraduateDate = a.teacherGraduateDate,
                            teacherStudyProfessional = a.teacherStudyProfessional,
                            teacherIsAddCourse = a.teacherIsAddCourse,
                            teacherInviteCode = a.teacherInviteCode
                        ,
                            teacherIsRecommend = a.teacherIsRecommend,
                            teacherIsDisplay = a.teacherIsDisplay,
                            teacherPersonalResume = a.teacherPersonalResume,
                            teacherEnbleFlag = a.teacherEnbleFlag,
                            createTime = a.createTime,
                            updateTime = a.updateTime,
                            approvalStatus = b.approvalStatus,
                            newMessageNum = b.newMessageNum,
                            newNotificationNum = b.newNotificationNum
                            ,
                            about = b.about,
                            signature = b.signature,
                            largeAvatar = b.largeAvatar,
                            mediumAvatar = b.mediumAvatar,
                            smallAvatar = b.smallAvatar
                              ,
                            loginIp = b.loginIp,
                            loginTime = b.loginTime

                        };

            var res = query.FirstOrDefault();
            if (res != null)
            {
                var classIdList = _classTeacherRep.GetAll().Where(a => a.UserId.Equals(res.user_id))
                    .Select(a => a.ClassId.ToString()).ToList();
                res.classIds = classIdList.Any()? classIdList.Aggregate((right, left) => right + "," + left):"";
            }
            return res;
        }

        public void UpdateTeacherInfo(CreateTeacherInfoInputDto input)
        {
            input =StringHelper.TrimStr<CreateTeacherInfoInputDto>(input);

            input.isDel = false;
            input.teacherEnbleFlag = false;

            //可以直接Logger,它在ApplicationService基类中定义的
            Logger.Info("Updating a row for input: " + input);

            try
            {
                var oldUser = _userRepository.Get(input.user_id);
                UserBase _user = input.GetUser();
                if (!string.IsNullOrEmpty(input.teacherCode))
                {
                    TeacherInfo _teacher = _teacherInfoRepository.GetAll().Where(a => a.userId == _user.Id).FirstOrDefault();
                    if (input.teacherCode != _teacher.teacherCode && _teacherInfoRepository.GetAll().Any(a => a.teacherCode == input.teacherCode))
                    {
                        throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,"教师号已存在！");
                    }
                }
                _user.Id = input.user_id;
                _user.userPassWord = string.IsNullOrWhiteSpace(input.user_password) ? oldUser.userPassWord : sha1Encrypt.getSHA1Value(input.user_password);
                TeacherInfo teacher = input.GetTeacherInfo();
                teacher.Id = input.Id;
                teacher.updateTime = DateTime.Now;

                /*-- 判断手机号码是否改变，如果改变，在新课网上修改手机号码--*/
                if (oldUser.userMobile != input.user_mobile && !string.IsNullOrWhiteSpace(input.user_mobile))
                {
                    var mobileExist = _userInfoApiService.CheckMobile(null, new UserRegisterModel() { RegisterMobile = input.user_mobile }).Result.IsSuccess;
                    if (mobileExist)
                    {
                        ModifyMobile modifry = new ModifyMobile() { newmobile = input.user_mobile, oldmobile = oldUser.userMobile, username = oldUser.userLoginName };
                        var apiRes = _userInfoApiService.ModifryUserMobile(null, modifry).Result;
                        if (!apiRes.IsSuccess)
                        {
                            throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,apiRes.ErrMsg ?? "手机号修改失败");
                            //  throw (new Exception(apiRes.ErrMsg));
                        }
                    }
                }
                var apiNewLoginName = oldUser.userLoginName == input.user_login_name ? "" : input.user_login_name;
               
                ObjHelper.ObjSetValue<UserBase, UserBase>(ref oldUser, ref  _user);

                /*--把信息更新到新课网中--*/
                EditDto editDto = oldUser.MapTo<UserInfoDto>().GetEditDto();
                editDto.newpw = string.IsNullOrWhiteSpace(input.user_password) ? "" : sha1Encrypt.AirEncode(input.user_password ?? "");
                editDto.username = apiNewLoginName;
                var res = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
                if (!res.IsSuccess)
                {
                    throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,res.ErrMsg ?? "信息更新失败");
                    // throw (new Exception(res.ErrMsg));
                }
               
                _teacherInfoRepository.Update(teacher);
                _unitOfWorkManager.Current.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void UpdateTeacherInfoByUser(UpdateTeacherInfoInputDto input)
        {
            input = StringHelper.TrimStr<UpdateTeacherInfoInputDto>(input);
            var userCookie= CookieHelper.GetLoginInUserInfo();
            input.updateTime = DateTime.Now;
            UserBase _user = input.GetUser();
            UserBase baseUser = _userRepository.Get(input.user_id);

            TeacherInfo baseTeacher = _teacherInfoRepository.Get(input.Id);
            if (!string.IsNullOrEmpty(input.teacherCode))
            {
                // TeacherInfo _teacher = _teacherInfoRepository.GetAll().Where(a => a.userId == _user.id).FirstOrDefault();
                if (input.teacherCode != baseTeacher.teacherCode && _teacherInfoRepository.GetAll().Any(a => a.teacherCode == input.teacherCode))
                {
                    throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,"教师号已存在！");
                }
            }


            // input.departmentId=string.IsNullOrEmpty(input.departmentId)?null:input.departmentId;
            _user.userPassWord = string.IsNullOrWhiteSpace(input.user_password) ? baseUser.userPassWord : sha1Encrypt.getSHA1Value(input.user_password);

            /*-- 判断手机号码是否改变，如果改变，在新课网上修改手机号码--*/
            if (baseUser.userMobile != input.user_mobile && !string.IsNullOrWhiteSpace(input.user_mobile))
            {
                var mobileExist = _userInfoApiService.CheckMobile(null, new UserRegisterModel() { RegisterMobile = input.user_mobile }).Result.IsSuccess;
                if (mobileExist)
                {
                    ModifyMobile modifry = new ModifyMobile() { newmobile = input.user_mobile, oldmobile = baseUser.userMobile, username = baseUser.userLoginName };
                    var apiRes = _userInfoApiService.ModifryUserMobile(null, modifry).Result;
                    if (!apiRes.IsSuccess)
                    {
                        throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info, apiRes.ErrMsg ?? "手机号修改失败");
                        // throw (new Exception(apiRes.ErrMsg));
                    }
                }
            }
            var apiNewLoginName = baseUser.userLoginName == input.user_login_name ? "" : input.user_login_name;
            /*----------*/

            var oldUserLoginName = baseUser.userLoginName;
            ObjHelper.ObjSetValue<UserBase, UserBase>(ref baseUser, ref  _user);
            // Abp.Common.ObjHelper.ObjSetValue<TeacherInfo, TeacherInfo>(ref baseTeacher, ref  teacher);
            input.SetTeacherInfo(ref baseTeacher);

            /*--把信息更新到新课网中--*/
            EditDto editDto = baseUser.MapTo<UserInfoDto>().GetEditDto();
            editDto.newpw = string.IsNullOrWhiteSpace(input.user_password) ? "" : sha1Encrypt.AirEncode(input.user_password ?? "");
            editDto.username = apiNewLoginName;
            editDto.oldusername = oldUserLoginName;
            var res = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
            if (!res.IsSuccess)
            {
                throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,res.ErrMsg ?? "信息更新失败" );

            }
            //管理员允许编辑班级
            if (userCookie.IsAdmin)
            {
                //班级教师关联
                var userClassids = _classTeacherRep.GetAll().Where(c => c.UserId.Equals(_user.Id)).Select(a => a.ClassId).ToList();
                //没有班级关联直接删除所有关联班级
                if (string.IsNullOrEmpty(input.classId))
                {
                    _classTeacherRep.Delete(a => a.UserId.Equals(_user.Id));
                    return;
                }
                //根据选中的班级列表删除界面取消的班级
                _classTeacherRep.Delete(
                    a => !input.classId.Contains(a.ClassId.ToString()) && a.UserId.Equals(_user.Id));
                foreach (var cid in input.classId.Split(','))
                {
                    if (!userClassids.Any(c => c.Equals(cid.TryParseGuid())))
                    {
                        _classTeacherRep.Insert(new ClassTeacher
                        {
                            Id = Guid.NewGuid(),
                            ClassId = cid.TryParseGuid(),
                            CreateTime = DateTime.Now,
                            CreateUserId = userCookie.Id,
                            UserId = _user.Id
                        });
                    }
                }
            }
          
            _userRepository.Update(baseUser);
            _teacherInfoRepository.Update(baseTeacher);
            _unitOfWorkManager.Current.SaveChanges();
        }

        public async Task CreateTeacherInfo(CreateTeacherInfoInputDto input)
        {
            try
            {
                input = StringHelper.TrimStr(input);
                var userCookie= CookieHelper.GetLoginInUserInfo();
                Logger.Info("Creating a row for input: " + input);
                if (!string.IsNullOrEmpty(input.teacherCode))
                {
                    if (_teacherInfoRepository.GetAll().Any(a => a.teacherCode == input.teacherCode))
                    {
                        throw new UserFriendlyException(1,"教师号已存在！");
                    }
                }

                input.Id = Guid.NewGuid();
                input.user_id = Guid.NewGuid();
                UserBase _user = input.GetUser();
                _user.userPassWord = sha1Encrypt.getSHA1Value(input.user_password ?? "");
                _user.userEnbleFlag = false;
                _user.smallAvatar = UserInfoImg.GetDefaultUserAvator(_user.userGender);
                _user.isCompleted = false;
                _user.userGender = input.user_gender;
                TeacherInfo teacher = input.GetTeacherInfo();
                teacher.createTime = DateTime.Now;
                teacher.updateTime = DateTime.Now;
                teacher.isDel = false;
                teacher.teacherEnbleFlag = false;
                teacher.teacherInviteCode = await InviteCodeHelper.NewTeacherInviteCode(_teacherInfoRepository);

                UserRegisterModel reginModel = _user.MapTo<UserInfoDto>().GetUserRegister();
                reginModel.PassWord = input.user_password;
                var apiRes = _userInfoApiService.Register(null, reginModel).Result;//调用新课网的接口，将用户添加到新课网中
                if (!apiRes.IsSuccess)
                {
                    if (apiRes.ErrMsg.Trim() == "UserExist")//当该学习在新课网中存在的时候，将该学生信息更新到新课网。
                    {
                        EditDto editDto = _user.MapTo<UserInfoDto>().GetEditDto();
                        editDto.newpw = string.IsNullOrWhiteSpace(input.user_password) ? "" : sha1Encrypt.AirEncode(input.user_password ?? "");
                        var res = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
                        if (!res.IsSuccess)
                        {
                            throw new UserFriendlyException(1,res.ErrMsg ?? "新增教师失败");
                        }
                        else
                        {
                            var obj = JsonConvert.DeserializeObject<UserInfoDto>(res.Data.InKey.ToString());
                            _user.newMoocUserId = obj.Id;//如果新课网新增用户成功，讲新课网用户Id保存到本地
                            //    _user.newMoocUserId = res.Data.InKey.ToString();//如果新课网新增用户成功，讲新课网用户Id保存到本地
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info, apiRes.ErrMsg ?? "新增教师失败");
                    }
                }
                else
                {
                    _user.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey);//如果新课网新增用户成功，讲新课网用户Id保存到本地
                }


                //调用仓储基类的Insert方法把实体保存到数据库中

                await _userRepository.InsertAsync(_user);
                await _teacherInfoRepository.InsertAsync(teacher);
                var role = _iRoleManageRep.FirstOrDefault(a => a.roleGroup == "teacher" && a.isDefault);
                if (role != null)
                {
                    await _iUserRoleRep.InsertAsync(new UserRole { userId = teacher.userId, roleId = role.Id });
                }
                if (userCookie.IsAdmin)
                {
                    //班级教师关联
                    foreach (var cid in input.classId.Split(','))
                    {
                        _classTeacherRep.Insert(new ClassTeacher
                        {
                            Id = Guid.NewGuid(),
                            ClassId = cid.TryParseGuid(),
                            CreateTime = DateTime.Now,
                            CreateUserId = userCookie.Id,
                            UserId = _user.Id
                        });
                    }
                }
                _unitOfWorkManager.Current.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public Task DeleteTeacherInfo(BatchRequestInput input)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteTeacherInfos(List<BatchDeleteRequestInputByUser> inputList)
        {
            Logger.Info("Deleting a row for input: " + inputList);
            List<Guid> userIdList = new List<Guid>();
            foreach (BatchDeleteRequestInputByUser item in inputList)
            {

                TeacherInfo teacher = _teacherInfoRepository.Get(new Guid(item.Id));
                teacher.isDel = true;
                userIdList.Add(teacher.userId);
                _userRepository.Delete(teacher.userId);
                _teacherInfoRepository.Delete(teacher);
               
            }
          
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }


        /// <summary>
        /// 设置推荐教师/aa取消设置推荐教师
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SetTeacherRecommend(CreateTeacherInfoInputDto input)
        {
            TeacherInfo teacher = _teacherInfoRepository.Get(input.Id);
            teacher.teacherIsRecommend = !teacher.teacherIsRecommend;
            _teacherInfoRepository.Update(teacher);
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }

        /// <summary>
        /// 设置首页显示(设置首页显示的同时，将该教师设置为推荐教师)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SetTeacherIsDisplay(CreateTeacherInfoInputDto input)
        {
            TeacherInfo teacher = _teacherInfoRepository.Get(input.Id);
            teacher.teacherIsDisplay = !teacher.teacherIsDisplay;
            _teacherInfoRepository.Update(teacher);
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }
        /// <summary>
        /// 设置禁用用户
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public async Task ActiveTeacherInfo(List<BatchDeleteRequestInputByUser> inputList)
        {
            foreach (var input in inputList)
            {
                var id = string.IsNullOrEmpty(input.Id) ? Guid.Empty : new Guid(input.Id);

                var _teacher = _teacherInfoRepository.Get(id);
                _teacher.teacherEnbleFlag = _teacher.teacherEnbleFlag ? false : true;
                _teacherInfoRepository.Update(_teacher);

                UserBase userBase = _userRepository.Get(_teacher.userId);
                userBase.userEnbleFlag = _teacher.teacherEnbleFlag;
                _userRepository.Update(userBase);
            }
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }

        public TeacherInfoDto GetTeacherInfoByTeacherId(Guid teacherId)
        {

            var query = _teacherInfoRepository.GetAll().Where(a => a.isDel == false && a.Id == teacherId)
              .Join(_userRepository.GetAll().Where(a => a.identity == 2)
             , a => a.userId, b => b.Id, (a, b) => new TeacherInfoDto()
             {
                 Id = a.Id,
                 user_id = a.userId,
                 user_login_name = b.userLoginName,
                 user_password = b.userPassWord,
                 user_name = b.userFullName,
                 user_mobile = b.userMobile,
                 user_email = b.userEmail,
                 user_gender = b.userGender,
                 teacherCode = a.teacherCode,
                 teacherTitle = a.teacherTitle,
                 teacherJobStatusCode = a.teacherJobStatusCode,
                 teacherProfessionalDirection = a.teacherProfessionalDirection,
                 teacherBirthday = b.userBirthday,
                 teacherIdCode = b.userIdcard,
                 teacherNational = b.userNational,
                 teacherPolitical = b.userPolitical,
                 teacherEntryDate = a.teacherEntryDate,
                 teacherStartworkDate = a.teacherStartworkDate,
                 teacherEduAge = a.teacherEduAge,
                 teacherEducation = a.teacherEducation,
                 teacherAcademicDegree = a.teacherAcademicDegree,
                 teacherGraduateSchool = a.teacherGraduateSchool,
                 teacherGraduateDate = a.teacherGraduateDate,
                 teacherStudyProfessional = a.teacherStudyProfessional,
                 teacherIsAddCourse = a.teacherIsAddCourse,
                 teacherInviteCode = a.teacherInviteCode,
                 approvalStatus = b.approvalStatus,
                 newMessageNum = b.newMessageNum,
                 newNotificationNum = b.newNotificationNum
             ,
                 teacherIsRecommend = a.teacherIsRecommend,
                 teacherIsDisplay = a.teacherIsDisplay,
                 teacherPersonalResume = a.teacherPersonalResume,
                 teacherEnbleFlag = a.teacherEnbleFlag,
                 createTime = a.createTime,
                 updateTime = a.updateTime,
                 loginIp = b.loginIp,
                 loginTime = b.loginTime,
                 about = b.about,
                 largeAvatar = b.largeAvatar,
                 mediumAvatar = b.mediumAvatar,
                 smallAvatar = b.smallAvatar
                 // smallAvatar = "/files/Teacher/teacherHeadImg.png"

             });
            return query.FirstOrDefault();

        }

        public TeacherInfoDto GetTeacherInfoByUserId(Guid userId)
        {
            var query = _teacherInfoRepository.GetAll().Where(a => a.isDel == false)
               .Join(_userRepository.GetAll().Where(a => a.identity == 2 && a.Id == userId)
              , a => a.userId, b => b.Id, (a, b) => new TeacherInfoDto()
              {
                  Id = a.Id,
                  user_id = a.userId,
                  user_login_name = b.userLoginName,
                  user_password = b.userPassWord,
                  user_name = b.userFullName,
                  user_mobile = b.userMobile,
                  user_email = b.userEmail,
                  user_gender = b.userGender,
                  teacherCode = a.teacherCode,
                  teacherTitle = a.teacherTitle,
                  teacherJobStatusCode = a.teacherJobStatusCode,
                  teacherProfessionalDirection = a.teacherProfessionalDirection,
                  teacherBirthday = b.userBirthday,
                  teacherIdCode = b.userIdcard,
                  teacherNational = b.userNational,
                  teacherPolitical = b.userPolitical,
                  teacherEntryDate = a.teacherEntryDate,
                  teacherStartworkDate = a.teacherStartworkDate,
                  teacherEduAge = a.teacherEduAge,
                  teacherEducation = a.teacherEducation,
                  teacherAcademicDegree = a.teacherAcademicDegree,
                  teacherGraduateSchool = a.teacherGraduateSchool,
                  teacherGraduateDate = a.teacherGraduateDate,
                  teacherStudyProfessional = a.teacherStudyProfessional,
                  teacherIsAddCourse = a.teacherIsAddCourse,
                  teacherInviteCode = a.teacherInviteCode,
                  approvalStatus = b.approvalStatus,
                  newMessageNum = b.newMessageNum,
                  newNotificationNum = b.newNotificationNum
                    ,
                  loginIp = b.loginIp,
                  loginTime = b.loginTime
              ,
                  teacherIsRecommend = a.teacherIsRecommend,
                  teacherIsDisplay = a.teacherIsDisplay,
                  teacherPersonalResume = a.teacherPersonalResume,
                  teacherEnbleFlag = a.teacherEnbleFlag,
                  createTime = a.createTime,
                  updateTime = a.updateTime,
                  about = b.about,
                  largeAvatar = b.largeAvatar,
                  mediumAvatar = b.mediumAvatar,
                  smallAvatar = b.smallAvatar
                  // smallAvatar = "/files/Teacher/teacherHeadImg.png"
              });
            return query.FirstOrDefault();
        }


   
        /// <summary>
        /// 获取多个教师信息以“|”分割
        /// </summary>
        /// <param name="tids"></param>
        /// <returns></returns>
        public List<TeacherInfoDto> GetTeacherInfoByTIds(string tids)
        {
            List<TeacherInfoDto> tList = new List<TeacherInfoDto>();
            try
            {
                if (string.IsNullOrWhiteSpace(tids))
                {
                    return tList;
                }
                string[] idsArr = GetTeacherArr(tids);

                var teachers = from b in _userRepository.GetAll()
                               join a in _teacherInfoRepository.GetAll() on b.Id equals a.userId
                               where a.isDel == false && b.identity == 2 && idsArr.Contains(b.Id.ToString())
                               select new TeacherInfoDto()
                               {
                                   Id = a.Id,
                                   user_id = a.userId,
                                   user_login_name = b.userLoginName,
                                   user_password = b.userPassWord,
                                   user_name = b.userFullName,
                                   user_mobile = b.userMobile,
                                   user_email = b.userEmail,
                                   user_gender = b.userGender,
                                   teacherCode = a.teacherCode,
                                   teacherTitle = a.teacherTitle,
                                   teacherJobStatusCode = a.teacherJobStatusCode,
                                   teacherProfessionalDirection = a.teacherProfessionalDirection,
                                   teacherBirthday = b.userBirthday,
                                   teacherIdCode = b.userIdcard,
                                   teacherNational = b.userNational,
                                   teacherPolitical = b.userPolitical,
                                   teacherEntryDate = a.teacherEntryDate,
                                   teacherStartworkDate = a.teacherStartworkDate,
                                   teacherEduAge = a.teacherEduAge,
                                   teacherEducation = a.teacherEducation,
                                   teacherAcademicDegree = a.teacherAcademicDegree,
                                   teacherGraduateSchool = a.teacherGraduateSchool,
                                   teacherGraduateDate = a.teacherGraduateDate,
                                   teacherStudyProfessional = a.teacherStudyProfessional,
                                   teacherIsAddCourse = a.teacherIsAddCourse,
                                   teacherInviteCode = a.teacherInviteCode,
                                   approvalStatus = b.approvalStatus,
                                   newMessageNum = b.newMessageNum,
                                   newNotificationNum = b.newNotificationNum
                                   ,
                                   loginIp = b.loginIp,
                                   loginTime = b.loginTime
                                   ,
                                   teacherIsRecommend = a.teacherIsRecommend,
                                   teacherIsDisplay = a.teacherIsDisplay,
                                   teacherPersonalResume = a.teacherPersonalResume,
                                   teacherEnbleFlag = a.teacherEnbleFlag,
                                   createTime = a.createTime,
                                   updateTime = a.updateTime,
                                   about = b.about,
                                   largeAvatar = b.largeAvatar,
                                   mediumAvatar = b.mediumAvatar,
                                   smallAvatar = b.smallAvatar
                                   // smallAvatar = "/files/Teacher/teacherHeadImg.png"
                               };

                if (!teachers.Any())
                {
                    return tList;
                }

                return teachers.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return tList;
        }

        /// <summary>
        /// 获取教师数组
        /// </summary>
        /// <param name="tIds">以“|”分割的教师</param>
        /// <returns></returns>
        private string[] GetTeacherArr(string tIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tIds))
                {
                    return null;
                }
                string[] arr = tIds.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return arr;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isRecommend">"true":推荐教师，"false" 不是推荐教师 ""：全部教师</param>
        /// <returns></returns>
        public TeacherInfoDto GetTeacherInfoList(string isRecommend = "")
        {
            var query = _teacherInfoRepository.GetAll().Where(a => a.isDel == false)
                .WhereIf(!string.IsNullOrWhiteSpace(isRecommend), a => a.teacherIsRecommend == bool.Parse(isRecommend))
               .Join(_userRepository.GetAll().Where(a => a.identity == 2)
              , a => a.userId, b => b.Id, (a, b) => new TeacherInfoDto()
              {
                  Id = a.Id,
                  user_id = a.userId,
                  user_login_name = b.userLoginName,
                  user_password = b.userPassWord,
                  user_name = b.userFullName,
                  user_mobile = b.userMobile,
                  user_email = b.userEmail,
                  user_gender = b.userGender,
                  teacherCode = a.teacherCode,
                  teacherTitle = a.teacherTitle,
                  teacherJobStatusCode = a.teacherJobStatusCode,
                  teacherProfessionalDirection = a.teacherProfessionalDirection,
                  teacherBirthday = b.userBirthday,
                  teacherIdCode = b.userIdcard,
                  teacherNational = b.userNational,
                  teacherPolitical = b.userPolitical,
                  teacherEntryDate = a.teacherEntryDate,
                  teacherStartworkDate = a.teacherStartworkDate,
                  teacherEduAge = a.teacherEduAge,
                  teacherEducation = a.teacherEducation,
                  teacherAcademicDegree = a.teacherAcademicDegree,
                  teacherGraduateSchool = a.teacherGraduateSchool,
                  teacherGraduateDate = a.teacherGraduateDate,
                  teacherStudyProfessional = a.teacherStudyProfessional,
                  teacherIsAddCourse = a.teacherIsAddCourse,
                  teacherInviteCode = a.teacherInviteCode,
                  approvalStatus = b.approvalStatus,
                  newMessageNum = b.newMessageNum,
                  newNotificationNum = b.newNotificationNum
                    ,
                  loginIp = b.loginIp,
                  loginTime = b.loginTime
              ,
                  teacherIsRecommend = a.teacherIsRecommend,
                  teacherIsDisplay = a.teacherIsDisplay,
                  teacherPersonalResume = a.teacherPersonalResume,
                  teacherEnbleFlag = a.teacherEnbleFlag,
                  createTime = a.createTime,
                  updateTime = a.updateTime,
                  about = b.about,
                  largeAvatar = b.largeAvatar,
                  mediumAvatar = b.mediumAvatar,
                  smallAvatar = b.smallAvatar
                  //   smallAvatar = "files/Teacher/teacherHeadImg.png"
              });
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取推荐教师
        /// </summary>
        /// <param name="input">isRecommend:是否是推荐教师</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public List<RecommendTeacherObj> GetRecommendTeacherList(TeacherInfoInputDto input, ref int total)
        {
            List<RecommendTeacherObj> result = new List<RecommendTeacherObj>();
            try
            {
           

                var query = _teacherInfoRepository.GetAll()
                    .Where(a => a.isDel == false &&  a.teacherIsRecommend == input.isRecommend)
                    .Join(_userRepository.GetAll().Where(a => a.identity == 2) , a => a.userId, b => b.Id, (a, b) => new RecommendTeacherObj()
                    {

                        userFullName = b.userLoginName,
                        UserId = a.userId.ToString(),
                        UserName = b.userFullName,
                        teacherIsDisplay = a.teacherIsDisplay,
                        TeacherTitle = a.teacherTitle ?? string.Empty,
                        UserHeadImg = b.smallAvatar ?? string.Empty,
                        About = b.about ?? string.Empty

                    });
                if (!query.Any())
                {
                    return result;
                }

                total = query.Count();
                result = query.ToList();
                result = result.OrderByDescending(a => a.UserName).Skip(input.Skip).Take(input.PageSize).ToList();
                if (input.Skip == 0 && input.PageSize == 0)
                {
                    result = query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

            return result;
        }

        public string BindCmb()
        {

            var list = new List<dynamic>
            {
                new {type = "classId", datas = _iDepartmentService.GetDepartmentTree()},
                new {type = "teacherTitle", datas = GetteacherTitle()},
                new {type = "teacherTitleCreate", datas = GetteacherTitle()},
             
            };
            return JsonConvert.SerializeObject(list);
        }

        //获取标签
        private dynamic GetteacherTitle()
        {
            var list = new List<dynamic>();

            list.Add(new { id = "", text = " ", selected = true });
            list.Add(new { id = "1", text = "教授" });
            list.Add(new { id = "2", text = "副教授" });
            list.Add(new { id = "3", text = "讲师" });
            list.Add(new { id = "4", text = "助教" });

            return list;
        }


        /// <summary>
        /// 获取下拉框数据
        /// </summary>
        /// <param name="datas">数据集</param>
        /// <param name="item">父节点数据</param>
        /// <param name="typeId">下拉框类别</param>
        /// <returns></returns>
        private dynamic GetCmbData(dynamic datas, SelectListItemDto item, string typeId = null)
        {
            Type t = datas[0].GetType();
            var properies = t.GetProperties();
            foreach (var data in datas)
            {
                foreach (var propery in properies)
                {
                    switch (propery.Name)
                    {
                        case "schoolName":  //组织架构
                            if (item.id == data.pid.ToString())
                            {
                                item.children.Add(new SelectListItemDto { id = data.id.ToString(), text = data.schoolName, @checked = typeId == data.id.ToString() });
                            }
                            break;
                        case "name":    //学科、标签
                            if (item.id == data.pid.ToString())
                            {
                                item.children.Add(new SelectListItemDto { id = data.id.ToString(), text = data.name, @checked = typeId == data.id.ToString() });
                            }
                            break;
                        case "parentUid":   //分类
                            if (item.id == data.parentUid.ToString())
                            {
                                item.children.Add(new SelectListItemDto { id = data.id.ToString(), text = data.name, @checked = typeId == data.id.ToString() });
                            }
                            break;
                    }
                }
            }
            foreach (var child in item.children)
            {
                GetCmbData(datas, child);
            }
            return item;
        }
        #endregion
        /// <summary>
        /// 检验值是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">验证类型，新增或更改</param>
        /// <returns></returns>
        public bool CheckNameExit(string name, string type, string oldname = "")
        {
            /* if (type == "insert")
             {
                 return _teacherInfoRepository.GetAll().Join(_userRepository.GetAll().Where(s=>s.identity==2), a => a.userId, b => b.id, (a, b) => b).Any(m => m.userLoginName == name);
             }
             else
             {
                 return _teacherInfoRepository.GetAll().Join(_userRepository.GetAll().Where(d =>d.identity==2&& d.userLoginName != oldname), a => a.userId, b => b.id, (a, b) => b).Any(m => m.userLoginName == name);
             }*/

            if (type == "insert")
            {
                return _userRepository.GetAll().Any(s => s.userLoginName == name);
                // return _studentInfoRepository.GetAll().Join(_userRepository.GetAll().Where(s=>s.identity==1), a => a.userId, b => b.id, (a, b) => b).Any(m => m.userLoginName == name);
            }
            else
            {
                return _userRepository.GetAll().Where(d => d.userLoginName != oldname).Any(m => m.userLoginName == name);
                //   return _studentInfoRepository.GetAll().Join(_userRepository.GetAll().Where(d =>d.identity==1&& d.userLoginName != oldname), a => a.userId, b => b.id, (a, b) => b).Any(m => m.userLoginName == name);
            }
        }

        public string GetTeacherTitle(string id)
        {
            try
            {
                Hashtable ht = new Hashtable();
                ht.Add("1", "教授");
                ht.Add("2", "副教授");
                ht.Add("3", "讲师");
                ht.Add("4", "助教");

                if (ht.ContainsKey(id))
                {
                    return ht[id].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return string.Empty;
        }


        public List<ImportFieldModel> GetTeacherInfoMap()
        {
            bool isSigleMooc = true;
            List<ImportFieldModel> importFieldModelList = new List<ImportFieldModel>()
                   {
                       new ImportFieldModel(){FieldCode="user_login_name",FieldName="登录名",MaxLength=50,OrderNum=1,remark="必填",MustFlag=true,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_password",FieldName="密码",MaxLength=10,OrderNum=4,MustFlag=true,remark="必填",isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_name",FieldName="教师姓名",MaxLength=20,OrderNum=12,MustFlag=true,remark="必填",isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherCode",FieldName="教师号",MaxLength=60,OrderNum=12,remark="必填",MustFlag=true,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_mobile",FieldName="手机号码", MaxLength=50,OrderNum=14,remark="必填",MustFlag=true,isUfoProperty=false},
                       new ImportFieldModel(new string[]{"教授","副教授","讲师","助教"}){FieldCode="teacherTitleCreate", MustFlag=true,FieldName="职称",MaxLength=50,OrderNum=18,remark="必填<br/><br/>教授<br/>副教授<br/>讲师<br/>助教<br/>",isUfoProperty=false},
                       new ImportFieldModel(new string[]{"在职","停职","离职"}){FieldCode="teacherJobStatusCode", MustFlag=true,FieldName="在职状态",MaxLength=50,OrderNum=18,remark="必填<br/><br/>在职<br/>停职<br/>离职",isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_idcard",FieldName="身份证号",MaxLength=50,OrderNum=13,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_admission",FieldName="入学时间",isDateTimeType=true,MaxLength=50,OrderNum=15,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_email",FieldName="邮箱地址",MaxLength=50,OrderNum=16,isUfoProperty=false},
                       new ImportFieldModel(new string[]{"男","女"}){FieldCode="user_gender",FieldName="性别",MaxLength=50,OrderNum=20,remark="男<br/>女",isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_birthday",FieldName="出生日期",isDateTimeType=true,MaxLength=50,OrderNum=100,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_national",FieldName="民族",MaxLength=50,OrderNum=101,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_political",FieldName="政治面貌",MaxLength=50,OrderNum=101,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="user_province",FieldName="籍贯",MaxLength=50,OrderNum=102,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherProfessionalDirection",FieldName="专业方向",MaxLength=50,OrderNum=112,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherEntryDate",FieldName="任职日期",isDateTimeType=true,MaxLength=50,OrderNum=103,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherStartworkDate",FieldName="参加工作日期",isDateTimeType=true,MaxLength=50,OrderNum=104,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherEduAge",FieldName="高校教龄",MaxLength=50,OrderNum=104,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherEducation",FieldName="学历",MaxLength=50,OrderNum=104,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherAcademicDegree",FieldName="学位",MaxLength=50,OrderNum=105,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherGraduateSchool",FieldName="毕业学校",MaxLength=50,OrderNum=106,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherGraduateDate",FieldName="毕业时间",isDateTimeType=true,MaxLength=50,OrderNum=107,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherStudyProfessional",FieldName="所学专业",MaxLength=50,OrderNum=107,isUfoProperty=false},
                       new ImportFieldModel(){FieldCode="teacherPersonalResume",FieldName="个人简历",MaxLength=50,OrderNum=107,isUfoProperty=false},
                   };
            if (isSigleMooc)
            {
                importFieldModelList = importFieldModelList.Where(m => m.isUfoProperty == false).ToList();
            }
            return importFieldModelList;
        }

        /// <summary>
        /// 教师导入模板下载
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public System.IO.Stream GetTeacherInfoExportTemplate(string names)
        {
            if (!string.IsNullOrEmpty(names))
            {
                List<string> fieldName = names.TrimEnd(',').Split(',').ToList();
                return ExcelImportExport.ExportTemplate(GetTeacherInfoMap(), fieldName);
            }
            else
            {
                return null;
            }
        }

        #region 汇入教师+ImportResultOutputDto CreateUserInfoFromFile(Stream fileStream)
        public ImportResultOutputDto CreateTeacherInfoFromFile(Stream fileStream)
        {
            var result = new ImportResultOutputDto();
            try
            {
                var isUsernameContainEN = _iSiteSetService.GetAllSiteSet().usernameContainEN.Trim() == "open";
                var rootPath = AppConfiguration.WebServerFileRootPath;
                var helper = new ImportUserInfoHelper(_userInfoApiService, _userInfoService);
                int successCount;
                string errMsg;

                int s_count = 0;
                ExcelImportExport excel = new ExcelImportExport();
                errMsg = string.Empty;
                List<tacher_info_import> UserInfoList = excel.ExcelImport<tacher_info_import>(fileStream, GetTeacherInfoMap(), out errMsg);

                if (!string.IsNullOrEmpty(errMsg))
                {
                    return new ImportResultOutputDto
                    {
                        successCount = 0,
                        errMessage = errMsg
                    };
                }
                if (UserInfoList == null || UserInfoList.Count == 0)
                {
                    // return 0;
                    return new ImportResultOutputDto
                    {
                        successCount = 0,
                        errMessage = errMsg
                    };
                }

                successCount = 0;
                //插入数据
                if (UserInfoList != null && UserInfoList.Count > 0)
                {



                    UserInfoList.ForEach(m =>
                    {
                        var index = UserInfoList.IndexOf(m);
                        string itemErrMsg = string.Empty;
                        //if (!InsertStu(m, UserInfoList, departmentUid, helper, ref itemErrMsg))
                        if (!helper.CheckTeacherImportDate(m, UserInfoList, ref itemErrMsg, isUsernameContainEN))
                        {
                            errMsg += string.Format("第{0}行数据错误，错误原因：{1} <br/>", (index + 3).ToString(), itemErrMsg);
                        }
                        else
                        {
                            successCount++;
                        }

                    });

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        exportTeacherInsert(UserInfoList,  ref  errMsg);
                    }

                }
                //   _unitOfWorkManager.Current.SaveChanges();


                result = new ImportResultOutputDto
                {
                    //successCount = successCount,
                    successCount = string.IsNullOrEmpty(errMsg) ? successCount : 0,
                    errMessage = errMsg
                };
                return result;
            }
            catch (Exception e)
            {
                var guidNum = Guid.NewGuid().GetHashCode();
                result.errMessage = "发生未知错误，请联系管理员，错误编码：[" + guidNum + "]";
                Logger.Error("[" + guidNum + "]" + e);
            }

            return result;
        }
        #endregion
        private bool exportTeacherInsert(List<tacher_info_import> UserInfoList,  ref string msg)
        {
            try
            {
                var apiInputData = UserInfoList.Select(a => new UserRegisterModel()
                {
                    UserName = a.user_login_name,
                    RegisterMobile = a.user_mobile,
                    RegisterEmail = a.user_email,
                    PassWord = a.user_password ?? "123456",
                    RegisterIpAddress = ""
                }).ToList();
                var apiRes = _userInfoApiService.UserRegisterByList(null, apiInputData).Result;
                if (!apiRes.IsSuccess)
                {
                    msg = apiRes.ErrMsg;
                    return false;
                }

                UserInfoList.ForEach(model =>
                {
                    var userBase = model.GetUserInfo();
                    userBase.Id = Guid.NewGuid();
                    userBase.identity = 2;
                    userBase.userEnbleFlag = false;
                    // userBase.userGender=user

                    var teacher = model.GetTeacherInfo();
                    teacher.Id = Guid.NewGuid();
                    teacher.isDel = false;
                    teacher.createTime = DateTime.Now;
                    teacher.userId = userBase.Id;
                    teacher.teacherInviteCode =  InviteCodeHelper.NewTeacherInviteCode(_teacherInfoRepository).Result;
                    _userRepository.Insert(userBase);
                    //  _studentInfoRepository.Insert(stu);
                    _teacherInfoRepository.Insert(teacher);
                    var role = _iRoleManageRep.FirstOrDefault(a => a.roleGroup == "teacher" && a.isDefault);
                    if (role != null)
                    {
                         _iUserRoleRep.Insert(new UserRole { userId = teacher.userId, roleId = role.Id });
                    }

                });
                _unitOfWorkManager.Current.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }



        public List<TeacherInfoDto> GetTeacherInfoDtoList(List<BatchDeleteRequestInputByUser> inputList, TeacherInfoInputDto input, ref int total)
        {


            var query = from a in _teacherInfoRepository.GetAll().Where(a => a.isDel == false)
               .WhereIf(!string.IsNullOrWhiteSpace(input.teacherCode), m => !string.IsNullOrEmpty(m.teacherCode) && m.teacherCode.Contains(input.teacherCode))
           .WhereIf(!string.IsNullOrWhiteSpace(input.teacherTitle), s => !string.IsNullOrEmpty(s.teacherTitle) && s.teacherTitle.Contains(input.teacherTitle))
             .WhereIf(input.teacherJobStatusCode.HasValue, s => s.teacherJobStatusCode == input.teacherJobStatusCode)
             .Where(a => a.isDel == false)

                        join b in _userRepository.GetAll().Where(s => s.identity == 2)
                            //.WhereIf(!string.IsNullOrWhiteSpace(input.user_login_name), s => s.userLoginName == input.user_login_name)
                      .WhereIf(!string.IsNullOrWhiteSpace(input.user_login_name), s => !string.IsNullOrEmpty(s.userLoginName) && s.userLoginName.ToLower().Contains(input.user_login_name.ToLower()))
                       .WhereIf(!string.IsNullOrWhiteSpace(input.user_mobile), s => !string.IsNullOrEmpty(s.userMobile) && s.userMobile.Contains(input.user_mobile))
                            // .WhereIf(!string.IsNullOrWhiteSpace(input.user_name), s => s.userEmail.Contains(input.user_name))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.user_gender), s => s.userGender == input.user_gender)
                         on a.userId equals b.Id
                      

                        select new TeacherInfoDto()
                        {
                            Id = a.Id,
                            user_id = a.userId,
                            user_login_name = b.userLoginName,
                            user_password = b.userPassWord,
                            user_name = b.userFullName,
                            user_mobile = b.userMobile,
                            user_email = b.userEmail,
                            user_gender = b.userGender,
                            teacherCode = a.teacherCode,
                            teacherTitle = a.teacherTitle,
                            teacherJobStatusCode = a.teacherJobStatusCode,
                            teacherProfessionalDirection = a.teacherProfessionalDirection,
                            teacherBirthday = b.userBirthday,
                            teacherIdCode = b.userIdcard,
                            teacherNational = b.userNational,
                            teacherPolitical = b.userPolitical,
                            teacherEntryDate = a.teacherEntryDate,
                            teacherStartworkDate = a.teacherStartworkDate,
                            teacherEduAge = a.teacherEduAge,
                            teacherEducation = a.teacherEducation,
                            teacherAcademicDegree = a.teacherAcademicDegree,
                            teacherGraduateSchool = a.teacherGraduateSchool,
                            teacherGraduateDate = a.teacherGraduateDate,
                            teacherStudyProfessional = a.teacherStudyProfessional,
                            teacherIsAddCourse = a.teacherIsAddCourse,
                            teacherInviteCode = a.teacherInviteCode
                        ,
                            teacherIsRecommend = a.teacherIsRecommend,
                            teacherIsDisplay = a.teacherIsDisplay,
                            teacherPersonalResume = a.teacherPersonalResume,
                            teacherEnbleFlag = a.teacherEnbleFlag,
                            createTime = a.createTime,
                            updateTime = a.updateTime,
                            approvalStatus = b.approvalStatus,
                            newMessageNum = b.newMessageNum,
                            newNotificationNum = b.newNotificationNum
                            ,
                            about = b.about,
                            signature = b.signature,
                            largeAvatar = b.largeAvatar,
                            mediumAvatar = b.mediumAvatar,
                            smallAvatar = b.smallAvatar
                              ,
                            loginIp = b.loginIp,
                            loginTime = b.loginTime
                            
                        };


            var totalQuery = query;
            //  var totalQuery = string.IsNullOrEmpty(input.department) ? query : query.Join(_user2DepartmentRepository.GetAll().Where(s => input.department.Contains(s.DepartmentId.ToString())), a => a.user_id, b => b.UserId, (a, b) => a);
            var res = string.IsNullOrWhiteSpace(input.OrderExpression) ? totalQuery.OrderByDescending(a => a.createTime).Skip(input.Skip).Take(input.PageSize).ToList() : totalQuery.OrderBy(input.OrderExpression).Skip(input.Skip).Take(input.PageSize).ToList();

            if (inputList.Count > 0)
            {
                query = res.Join(inputList, a => a.user_id.ToString(), b => b.user_id, (a, b) => a);
            }
            total = totalQuery.Count();
            return query.ToList();
        }

        public async Task CreateRecovryTeacherInfo(CreateTeacherInfoInputDto input)
        {
            try
            {
                TeacherInfo model = new TeacherInfo();
                model.Id = Guid.NewGuid();
                model.userId = input.user_id;
                model.createTime = DateTime.Now;
                model.isDel = false;
                model.teacherEnbleFlag = false;
                await _teacherInfoRepository.InsertAsync(model);
                var role = _iRoleManageRep.FirstOrDefault(a => a.roleGroup == "teacher" && a.isDefault);
                if (role != null)
                {
                    await _iUserRoleRep.InsertAsync(new UserRole { userId = model.userId, roleId = role.Id });
                }
                _unitOfWorkManager.Current.SaveChanges();
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

        }

        public async Task AddInviteCode(Guid userId)
        {
            var teacherInfo = await _teacherInfoRepository.FirstOrDefaultAsync(a => a.userId == userId);
            if (teacherInfo == null)
            {
                throw new UserFriendlyException("无效教师");
            }

            if (!string.IsNullOrEmpty(teacherInfo.teacherInviteCode))
            {
                throw new UserFriendlyException("已有推荐码");
            }

            teacherInfo.teacherInviteCode = await InviteCodeHelper.NewTeacherInviteCode(_teacherInfoRepository);

            await _teacherInfoRepository.UpdateAsync(teacherInfo);
        }
    }
}
