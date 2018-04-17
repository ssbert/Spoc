using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Newtonsoft.Json;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.Encrypt;
using SPOC.Common.Enum;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.SysSetting;
using SPOC.User.Dto.Common;
using SPOC.User.Dto.StudentInfo;
using SPOC.User.Dto.UserInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Runtime.Validation;
using SPOC.SqlExecuter;

namespace SPOC.User
{
    [DisableValidation]
    public class StudentInfoService : ApplicationService, IStudentInfoService
    {
        private readonly IRepository<StudentInfo, Guid> _studentInfoRepository;

        private readonly IRepository<UserBase, Guid> _userRepository;
        private readonly IRepository<Faculty, Guid> _iFcultyaRep;
        private readonly IRepository<Major, Guid> _iMajorRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<AdministrativeClass, Guid> _iAdministrativeClassRep;
        private readonly IRepository<ClassTeacher, Guid> _iClassTeacherRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserInfoApiService _userInfoApiService;
        private readonly ISqlExecuter _iSqlExecuter;
        private readonly IUserInfoService _userInfoService;
        private readonly ISiteSetService _iSiteSetService;



        /// <summary>
        /// 构造函数注入
        /// </summary>
        public StudentInfoService(
            IRepository<StudentInfo, Guid> studentInfoRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserBase, Guid> userRepository,
            IUserInfoApiService userInfoApiService,
            IUserInfoService userInfoService,
            ISiteSetService iSiteSetService,
            IRepository<Faculty, Guid> iFcultyaRep, IRepository<Major, Guid> iMajorRep, IRepository<Class, Guid> iClassRep, IRepository<ClassTeacher, Guid> iClassTeacherRep, IRepository<ClassStudent, Guid> iClassStudentRep, IRepository<AdministrativeClass, Guid> iAdministrativeClassRep, ISqlExecuter iSqlExecuter)
        {
            _studentInfoRepository = studentInfoRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;
            _userInfoApiService = userInfoApiService;
            _userInfoService = userInfoService;
            _iSiteSetService = iSiteSetService;
            _iFcultyaRep = iFcultyaRep;
            _iMajorRep = iMajorRep;
            _iClassRep = iClassRep;
            _iClassTeacherRep = iClassTeacherRep;
            _iClassStudentRep = iClassStudentRep;
            _iAdministrativeClassRep = iAdministrativeClassRep;
            _iSqlExecuter = iSqlExecuter;
        }


        #region 获取学生和学生列表

        public EasyUiListResultDto<StudentInfoDto> GetStudentInfoByGuid(
           StudentInfoInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                throw new UserFriendlyException("没有登录");
            }
            input = StringHelper.TrimStr<StudentInfoInputDto>(input);

            var studentQueryable =  _studentInfoRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.user_code),
                    s => !string.IsNullOrEmpty(s.userCode) && s.userCode.Contains(input.user_code))
                //.WhereIf(!string.IsNullOrWhiteSpace(input.department),
                //    s => input.department.Contains(s.classId.ToString()))
                .Where(a => a.isDel == false);
            
            if (!cookie.IsAdmin)
            {   //获取当前用户能查看的教学班学生
                var students = _iClassStudentRep.GetAll()
                    .Join(_iClassTeacherRep.GetAll(), c => c.ClassId, ct => ct.ClassId, (c, ct) => new { c, ct })
                    .Where(@t => @t.ct.UserId == cookie.Id)
                    .Select(@t => @t.c.UserId).ToList();
                studentQueryable= studentQueryable.Where(a => students.Any(s => s.Equals(a.userId)));
            }
            if (!string.IsNullOrWhiteSpace(input.department))
            {
                //根据教学班过滤学生
                var students = _iClassStudentRep.GetAll()
                    .Where(t => input.department.Contains(t.ClassId.ToString()))
                    .Select(t => t.UserId).ToList();
                studentQueryable = studentQueryable.Where(a => students.Any(s => s.Equals(a.userId)));
            }
            //查询教学班级
           var classStudentQuery= _iSqlExecuter.SqlQuery<ClassStudentDto>(@"select userId,GROUP_CONCAT(CAST(classId as CHAR) SEPARATOR ',') as classIds,GROUP_CONCAT(name SEPARATOR ',') as classNames from class_student join  class on class_student.classId=class.id GROUP BY userId");

            var userQueryable = _userRepository.GetAll().Where(a => a.identity == 1 && a.approvalStatus== "approved")
                .WhereIf(!string.IsNullOrWhiteSpace(input.user_login_name), s => !string.IsNullOrEmpty(s.userLoginName) && s.userLoginName.ToLower().Contains(input.user_login_name.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(input.user_idcard), s => !string.IsNullOrEmpty(s.userIdcard) && s.userIdcard.Contains(input.user_idcard))
                .WhereIf(!string.IsNullOrWhiteSpace(input.user_mobile), s => !string.IsNullOrEmpty(s.userMobile) && s.userMobile.Contains(input.user_mobile))
                .WhereIf(!string.IsNullOrWhiteSpace(input.user_email), s => !string.IsNullOrEmpty(s.userEmail) && s.userEmail.Contains(input.user_email))
                .WhereIf(!string.IsNullOrWhiteSpace(input.user_name), s => !string.IsNullOrEmpty(s.userFullName) && s.userFullName.ToLower().Contains(input.user_name.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(input.user_gender), s => s.userGender == input.user_gender);

            var query = from a in studentQueryable
                join b in userQueryable on a.userId equals b.Id
                join faculty in _iFcultyaRep.GetAll() on a.facultyId equals faculty.Id
                into ftemp
                from ft in ftemp.DefaultIfEmpty()
                        join major in _iMajorRep.GetAll() on a.majorId equals major.Id
                into mtemp
                from mt in mtemp.DefaultIfEmpty()
                        join cls in _iAdministrativeClassRep.GetAll() on a.classId equals cls.Id
                into ctemp
                from ct in ctemp.DefaultIfEmpty()
                join cs in classStudentQuery on a.userId equals cs.userId
                into cstemp
                from cst in cstemp.DefaultIfEmpty()
                        select new StudentInfoDto
                {
                    Id = a.Id,
                    user_id = a.userId,
                    user_login_name = b.userLoginName,
                    user_code = a.userCode,
                    user_mobile = b.userMobile,
                    user_email = b.userEmail,
                    user_name = b.userFullName,
                    user_nickname = a.userNickname,
                    user_national = b.userNational,
                    user_political = b.userPolitical,
                    user_password = b.userPassWord,
                    user_province = a.userProvince,
                    user_city = a.userCity,
                    user_birthday = b.userBirthday.HasValue ? Convert.ToDateTime(b.userBirthday.Value.ToShortDateString()) : b.userBirthday,
                    user_gender = b.userGender,
                    user_facultyid = a.facultyId.ToString(),
                    user_faculty = ft!=null? ft.name:"",
                    user_majorid = a.majorId.ToString(),
                    user_major = mt != null ? mt.name : "",
                    user_administrativeclass = ct != null ? ct.name : "",
                    study_form = a.studyForm,
                    user_class = cst != null ? cst.classNames : "",
                    study_flag = a.studyFlag,
                    level = a.level,
                    user_eductional = a.userEductional,
                    user_admission = a.userAdmission,
                    user_grade = a.userGrade,
                    user_dormitory = a.userDormitory,
                    user_idcard = b.userIdcard,
                    user_zipcode = a.userZipcode,
                    user_inviteCode = a.userInviteCode,
                    user_register_inviteCode = a.userRegisterInviteCode,
                    user_enble_flag = a.userEnbleFlag,
                    is_graduation = a.isGraduation,
                    graduation_date = a.graduationDate,
                    create_time = a.createTime,
                    updateTime = a.updateTime,
                    isDel = a.isDel,
                    about = b.about,
                    signature = b.signature,
                    largeAvatar = b.largeAvatar,
                    mediumAvatar = b.mediumAvatar,
                    smallAvatar = b.smallAvatar,
                    loginIp = b.loginIp,
                    loginTime = b.loginTime,
                    approvalStatus = b.approvalStatus,
                    newMessageNum = b.newMessageNum,
                    newNotificationNum = b.newNotificationNum
                };


            // var totalQuery = query;
            var res = string.IsNullOrWhiteSpace(input.OrderExpression)
                ? query.OrderByDescending(a => a.create_time).Skip(input.Skip).Take(input.PageSize).ToList()
                : query.OrderBy(input.OrderExpression).Skip(input.Skip).Take(input.PageSize).ToList();
            
            return new EasyUiListResultDto<StudentInfoDto>
            {
                total = query.Count(),
                rows = res
            };
        }

 

        /// <summary>
        /// 根据studentInfo表Id得到学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StudentInfoDto GetStudentInfo(UserInfoQueryInputDto model)
        {
            var query = from a in _studentInfoRepository.GetAll()
                .Where(a => a.isDel == false)
                .WhereIf(model.id != Guid.Empty, s => s.Id == model.id)
                join b in _userRepository.GetAll().Where(a => a.identity == 1)
                    .WhereIf(model.userId != Guid.Empty, s => s.Id == model.userId)
                    .WhereIf(!string.IsNullOrEmpty(model.userName), u => u.userLoginName == model.userName)
                    on a.userId equals b.Id
                join faculty in _iFcultyaRep.GetAll() on a.facultyId equals faculty.Id
                into ftemp
                from ft in ftemp.DefaultIfEmpty()
                join major in _iMajorRep.GetAll() on a.majorId equals major.Id
                into mtemp
                from mt in mtemp.DefaultIfEmpty()
                join classes in _iAdministrativeClassRep.GetAll() on a.classId equals classes.Id
                into ctemp
                from ct in ctemp.DefaultIfEmpty()
                        select
                    new StudentInfoDto()
                    {
                        Id = a.Id,
                        user_id = a.userId,
                        user_login_name = b.userLoginName,
                        user_code = a.userCode,
                        user_mobile = b.userMobile,
                        user_email = b.userEmail,
                        user_name = b.userFullName,
                        user_nickname = a.userNickname,
                        user_national = b.userNational,
                        user_political = b.userPolitical,
                        user_password = b.userPassWord,
                        user_province = a.userProvince,
                        user_city = a.userCity,
                        user_birthday = b.userBirthday,
                        user_gender = b.userGender,
                        user_facultyid = a.facultyId.Equals(Guid.Empty)?"":a.facultyId.ToString(),          
                        user_majorid  = a.majorId.Equals(Guid.Empty) ? "" : a.majorId.ToString(),                      
                        user_administrativeclassid = a.classId.Equals(Guid.Empty) ? "" : a.classId.ToString(),
                        user_faculty = ft != null ? ft.name : "",
                        user_major = mt != null ? mt.name : "",
                        user_administrativeclass = ct != null ? ct.name : "",
                        study_form = a.studyForm,
                        study_flag = a.studyFlag,
                        level = a.level,
                        user_eductional = a.userEductional,
                        user_admission = a.userAdmission,
                        user_grade = a.userGrade,
                        user_dormitory = a.userDormitory,
                        user_idcard = b.userIdcard,
                        user_zipcode = a.userZipcode,
                        user_inviteCode = a.userInviteCode,
                        user_register_inviteCode = a.userRegisterInviteCode,
                        user_enble_flag = a.userEnbleFlag,
                        is_graduation = a.isGraduation,
                        graduation_date = a.graduationDate,
                        create_time = a.createTime,
                        updateTime = a.updateTime
                        ,
                        isDel = a.isDel
                        ,
                        about = b.about,
                        signature = b.signature,
                        largeAvatar = b.largeAvatar,
                        mediumAvatar = b.mediumAvatar,
                        smallAvatar = b.smallAvatar
                        ,
                        loginIp = b.loginIp,
                        loginTime = b.loginTime,
                        approvalStatus = b.approvalStatus,
                        newMessageNum = b.newMessageNum,
                        newNotificationNum = b.newNotificationNum
                      
                    };
            var res = query.Distinct().ToList().FirstOrDefault();
            var classStudent = _iClassStudentRep.GetAll().Where(a => a.UserId.Equals(res.user_id))
                .OrderByDescending(a => a.CreateTime).FirstOrDefault();
            if(classStudent!=null)
            res.user_classid = classStudent.ClassId.ToString();
            res.user_birthday = res.user_birthday.HasValue
                ? Convert.ToDateTime(res.user_birthday.Value.ToShortDateString())
                : res.user_birthday;
            return res;
        }


        private string GetArrStr<T>(List<T> liet, string splitStr)
        {
            string totalString = null;
            int len = liet.Count;
            for (int i = 0; i < len; i++)
            {
                totalString = totalString + liet[i].ToString() + (i == len - 1 ? "" : splitStr);
            }
            return totalString;
        }

        #endregion

        #region 更新学生+void UpdateStudentInfo(CreateStudentInfoInputDto input)

        public void UpdateStudentInfo(CreateStudentInfoInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                throw new UserFriendlyException("没有登录");
            }
       
            if (!cookie.IsAdmin && cookie.Identity==1)
            {
                throw new UserFriendlyException("没有权限");
            }
            //可以直接Logger,它在ApplicationService基类中定义的
            Logger.Info("Updating a row for input: " + input);

            try
            {
                input = StringHelper.TrimStr(input);

                var oldUser = _userRepository.Get(input.user_id);
                UserBase user = input.GetUser();

                StudentInfo oldStu = _studentInfoRepository.Get(input.Id);
           

                if (!string.IsNullOrEmpty(input.user_code))
                {
                    // var  oldUserCode = _studentInfoRepository.GetAll().Where(a => a.userId == _user.id).FirstOrDefault().userCode;
                    var oldUserCode = oldStu.userCode;
                    if (input.user_code != oldUserCode &&
                        _studentInfoRepository.GetAll().Any(a => a.userCode == input.user_code))
                    {
                        throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,"学号已存在！");
                    }
                }

                user.Id = input.user_id;
                user.userPassWord = string.IsNullOrWhiteSpace(input.user_password)
                    ? oldUser.userPassWord
                    : sha1Encrypt.getSHA1Value(input.user_password);

                StudentInfo student = input.GetStudentInfo();
                student.Id = input.Id;
                student.updateTime = DateTime.Now;

                /*-- 判断手机号码是否改变，如果改变，在新课网上修改手机号码--*/
                if (oldUser.userMobile != input.user_mobile && !string.IsNullOrWhiteSpace(input.user_mobile) )
                {

                    var mobileExist =
                        _userInfoApiService.CheckMobile(null,
                            new UserRegisterModel() {RegisterMobile = input.user_mobile}).Result.IsSuccess;
                    if (mobileExist)
                    {
//当新课网中，新修改手机号不存在时，将该手机号更新到新课网

                        ModifyMobile modifry = new ModifyMobile()
                        {
                            newmobile = input.user_mobile,
                            oldmobile = oldUser.userMobile,
                            username = oldUser.userLoginName
                        };
                        var apiRes = _userInfoApiService.ModifryUserMobile(null, modifry).Result;
                        if (!apiRes.IsSuccess)
                        {
                            throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,apiRes.ErrMsg ?? "修改学生信息失败" );
                        }
                    }
                }
                /*----------*/

                user.approvalStatus = oldUser.approvalStatus;
                var apiNewLoginName = oldUser.userLoginName == input.user_login_name ? "" : input.user_login_name;
                var oldUserLoginName = oldUser.userLoginName;
                ObjHelper.ObjSetValue(ref oldUser, ref user);
                ObjHelper.ObjSetValue(ref oldStu, ref student);

                /*--把信息更新到新课网中--*/
                EditDto editDto = oldUser.MapTo<UserInfoDto>().GetEditDto();
                editDto.newpw = string.IsNullOrWhiteSpace(input.user_password)
                    ? ""
                    : sha1Encrypt.AirEncode(input.user_password ?? "");
                editDto.username = apiNewLoginName;
                editDto.oldusername = oldUserLoginName;
                var res = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
                if (!res.IsSuccess)
                {
                    throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info, res.ErrMsg ?? "信息更新失败");
                }
                //写入教学班级
                if (!string.IsNullOrWhiteSpace(input.user_classid))
                {
                    _iClassStudentRep.Delete(s=>s.UserId.Equals(oldUser.Id));
                    _iClassStudentRep.Insert(new ClassStudent
                    {
                        Id = Guid.NewGuid(),
                        ClassId = input.user_classid.TryParseGuid(),
                        UserId = oldUser.Id,
                        CreateTime = DateTime.Now
                    });
                }
                _userRepository.Update(oldUser);
                _studentInfoRepository.Update(oldStu);
                _unitOfWorkManager.Current.SaveChanges();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info, "信息更新失败");
            }
        }

        #endregion

        #region 创建学生+ Task CreateStudentInfo(CreateStudentInfoInputDto input)

        public async Task CreateStudentInfo(CreateStudentInfoInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                throw new UserFriendlyException("没有登录");
            }
          
            if (!cookie.IsAdmin && cookie.Identity==1)
            {
                throw new UserFriendlyException("没有权限");
            }

            try
            {
                input = StringHelper.TrimStr<CreateStudentInfoInputDto>(input);

                Logger.Info("Creating a row for input: " + input);

                input.Id = Guid.NewGuid();
                input.user_id = Guid.NewGuid();
                UserBase user = input.GetUser();

                if (!string.IsNullOrEmpty(input.user_code))
                {
                    if (_studentInfoRepository.GetAll().Any(a => a.userCode == input.user_code))
                    {
                        throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,"学号号已存在！");
                    }
                }

                user.userEnbleFlag = false;
                user.approvalStatus = "approved";
                user.userPassWord = sha1Encrypt.getSHA1Value(input.user_password ?? "");
                user.identity = 1;
                user.smallAvatar = UserInfoImg.GetDefaultUserAvator(user.userGender);
                user.isCompleted = false;
                StudentInfo student = input.GetStudentInfo();
                student.isDel = false;
                student.createTime = DateTime.Now;
                student.updateTime = DateTime.Now;
                student.userEnbleFlag = 0;
               
                //调用仓储基类的Insert方法把实体保存到数据库中

                UserRegisterModel reginModel = user.MapTo<UserInfoDto>().GetUserRegister();
                reginModel.PassWord = input.user_password;
                var apiRes = _userInfoApiService.Register(null, reginModel).Result; //调用新课网的接口，将用户添加到新课网中
                if (!apiRes.IsSuccess)
                {

                    if (apiRes.ErrMsg.Trim() == "UserExist") //当该学习在新课网中存在的时候，将该学生信息更新到新课网。
                    {
                        EditDto editDto = user.MapTo<UserInfoDto>().GetEditDto();
                        editDto.newpw = string.IsNullOrWhiteSpace(input.user_password)
                            ? ""
                            : sha1Encrypt.AirEncode(input.user_password ?? "");
                        var res = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
                        if (!res.IsSuccess)
                        {
                            throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info, res.ErrMsg ?? "新增学生失败");
                        }
                        else
                        {
                            var obj = JsonConvert.DeserializeObject<UserInfoDto>(res.Data.InKey.ToString());
                            user.newMoocUserId = obj.Id; //如果新课网新增用户成功，讲新课网用户Id保存到本地
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info, apiRes.ErrMsg ?? "新增学生失败");
                    }
                }
                else
                {
                    user.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey)
                        ? Guid.Empty
                        : Guid.Parse(apiRes.Data.InKey); //如果新课网新增用户成功，讲新课网用户Id保存到本地
                }
                //写入教学班级
                if (!string.IsNullOrWhiteSpace(input.user_classid))
                {
                    _iClassStudentRep.Insert(new ClassStudent
                    {
                        Id = Guid.NewGuid(),
                        ClassId = input.user_classid.TryParseGuid(),
                        UserId = user.Id,
                        CreateTime=DateTime.Now
                    });
                }
                _userRepository.Insert(user);
                _studentInfoRepository.Insert(student);

                await _unitOfWorkManager.Current.SaveChangesAsync();

              
            }
            catch (Exception e)
            {

                throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,e.ToString());
            }

        }

        #endregion

        #region 删除学生+Task DeleteStudentInfos(List<BatchDeleteRequestInputByUser> inputList)


        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public async Task DeleteStudentInfos(List<BatchDeleteRequestInputByUser> inputList)
        {
            Logger.Info("Deleting a row for input: " + inputList);
            List<Guid> userIdList = new List<Guid>();
            foreach (BatchDeleteRequestInputByUser item in inputList)
            {
                StudentInfo stu = _studentInfoRepository.Get(new Guid(item.Id));
                stu.isDel = true;
                userIdList.Add(stu.userId);
                _userRepository.Delete(stu.userId);
                _studentInfoRepository.Delete(stu);
            }
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }

        #endregion

        #region 禁用学生+Task ActiveStudentInfo(List<BatchDeleteRequestInputByUser> inputList)

        /// <summary>
        /// 禁用学生
        /// </summary>
        /// <returns></returns>
        public async Task ActiveStudentInfo(List<BatchDeleteRequestInputByUser> inputList)
        {
            foreach (var input in inputList)
            {
                var id = string.IsNullOrEmpty(input.Id) ? Guid.Empty : new Guid(input.Id);
                var _stu = _studentInfoRepository.Get(id);


                _stu.userEnbleFlag = _stu.userEnbleFlag == 1 ? 0 : 1;
                _studentInfoRepository.Update(_stu);

                UserBase userBase = _userRepository.Get(_stu.userId);
                userBase.userEnbleFlag = _stu.userEnbleFlag == 1 ? true : false;
                _userRepository.Update(userBase);
            }
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }

        #endregion

        #region 数据绑定

     

        /// <summary>
        /// 获取下拉框数据
        /// </summary>
        /// <param name="datas">数据集</param>
        /// <param name="item">父节点数据</param>
        /// <param name="typeId">下拉框类别</param>
        /// <returns></returns>
        private dynamic GetCmbData(dynamic datas, SelectListItemDto item, string typeId = null,
            string IsAllowChecked = "true")
        {
            Type t = datas[0].GetType();
            var properies = t.GetProperties();
            foreach (var data in datas)
            {
                foreach (var propery in properies)
                {
                    switch (propery.Name)
                    {
                        case "schoolName": //组织架构
                            if (item.id == data.pid.ToString())
                            {
                                item.children.Add(new SelectListItemDto
                                {
                                    id = data.id.ToString(),
                                    text = data.schoolName,
                                    @checked = typeId == data.id.ToString(),
                                    IsAllowChecked = IsAllowChecked
                                });
                            }
                            break;
                        case "name": //学科、标签
                            if (item.id == data.pid.ToString())
                            {
                                item.children.Add(new SelectListItemDto
                                {
                                    id = data.id.ToString(),
                                    text = data.name,
                                    @checked = typeId == data.id.ToString()
                                });
                            }
                            break;
                        case "parentUid": //分类
                            if (item.id == data.parentUid)
                            {
                                item.children.Add(new SelectListItemDto
                                {
                                    id = data.id.ToString(),
                                    text = data.name,
                                    @checked = typeId == data.id.ToString()
                                });
                            }
                            break;
                    }
                }
            }
            foreach (var child in item.children)
            {
                GetCmbData(datas, child, typeId, IsAllowChecked);
            }
            return item;
        }

        #endregion


        #region 学生信息Excel模板下载

        public List<ImportFieldModel> GetStudentInfoMap()
        {
            bool isSigleMooc = true;
           
            List<ImportFieldModel> importFieldModelList = new List<ImportFieldModel>()
            {
                new ImportFieldModel()
                {
                    FieldCode = "login_name",
                    FieldName = "登录名",
                    MaxLength = 50,
                    OrderNum = 1,
                    remark = "必填",
                    MustFlag = true,
                    isUfoProperty = false
                },
               
                new ImportFieldModel()
                {
                    FieldCode = "user_password",
                    FieldName = "密码",
                    MaxLength = 10,
                    OrderNum = 3,
                    MustFlag = true,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_code",
                    FieldName = "学生学号",
                    MaxLength = 64,
                    OrderNum = 4,
                    MustFlag = true,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_name",
                    FieldName = "学生姓名",
                    MaxLength = 20,
                    OrderNum = 2,
                    MustFlag = true,
                    remark = "必填",
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "exam_num",
                    FieldName = "考生号",
                    MaxLength = 60,
                    OrderNum = 12,
                    remark = "必填",
                    isUfoProperty = true
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_idcard",
                    FieldName = "身份证号",
                    MaxLength = 50,
                    OrderNum = 13,
                    isUfoProperty = false
                },

                new ImportFieldModel()
                {
                    FieldCode = "user_mobile",
                    FieldName = "手机号码",
                    MaxLength = 50,
                    OrderNum = 5,
                   // remark = "必填",
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_admission",
                    FieldName = "入学时间",
                    isDateTimeType = true,
                    MaxLength = 50,
                    OrderNum = 15,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_email",
                    FieldName = "邮箱地址",
                    remark = "必填",
                    MaxLength = 50,
                    OrderNum = 6,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_faculty",
                    FieldName = "院系",
                    MaxLength = 50,
                    OrderNum = 7,
                    remark = "",
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "faculty_code",
                    FieldName = "院系代码",
                    MaxLength = 50,
                    OrderNum = 8,
                    remark = "",
                    isUfoProperty = true
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_major",
                    FieldName = "专业",
                    MaxLength = 50,
                    OrderNum = 9,
                    remark = "",
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "major_code",
                    FieldName = "专业代码",
                    MaxLength = 50,
                    OrderNum =10,
                    remark = "",
                    isUfoProperty = true
                },
                new ImportFieldModel()
                {
                    FieldCode = "administrative_class",
                    FieldName = "行政班级",
                    MaxLength = 50,
                    OrderNum = 11,
                    remark = "",
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_class",
                    FieldName = "教学班级",
                    MaxLength = 50,
                    OrderNum = 12,
                    remark = "",
                    isUfoProperty = false
                },
                // new ImportFieldModel(){FieldCode="user_grade",FieldName="当前年级",MaxLength=50,OrderNum=17,isUfoProperty=false},
                new ImportFieldModel()
                {
                    FieldCode = "user_city",
                    FieldName = "来源地区",
                    MaxLength = 50,
                    OrderNum = 18,
                    isUfoProperty = false
                },
                // new ImportFieldModel(){FieldCode="user_birth",FieldName="出生地区",MaxLength=50,OrderNum=19,isUfoProperty=false},
                new ImportFieldModel(new string[] {"男", "女"})
                {
                    FieldCode = "user_gender",
                    FieldName = "性别",
                    MaxLength = 50,
                    OrderNum = 20,
                    remark = "男<br/>女",
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_birthday",
                    FieldName = "生日",
                    isDateTimeType = true,
                    MaxLength = 50,
                    OrderNum = 100,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_national",
                    FieldName = "民族",
                    MaxLength = 50,
                    OrderNum = 101,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_political",
                    FieldName = "政治面貌",
                    MaxLength = 50,
                    OrderNum = 101,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_province",
                    FieldName = "籍贯",
                    MaxLength = 50,
                    OrderNum = 102,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_city",
                    FieldName = "所在城市",
                    MaxLength = 50,
                    OrderNum = 112,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_nickname",
                    FieldName = "昵称",
                    MaxLength = 50,
                    OrderNum = 103,
                    isUfoProperty = false
                },
                // new ImportFieldModel(new string[]{"正常在籍","待审查","已退学","已毕业"}){FieldCode="study_flag",FieldName="学籍状态",MaxLength=50,OrderNum=104,remark="必填<br/>正常在籍<br/>待审查<br/>已退学<br/>已毕业",isUfoProperty=false},
                new ImportFieldModel(new string[] {"专科", "专升本", "本科"})
                {
                    FieldCode = "level",
                    FieldName = "层次",
                    MaxLength = 50,
                    OrderNum = 104,
                    remark = "专科<br/>专升本<br/>本科",
                    isUfoProperty = false
                },
                new ImportFieldModel(new string[] {"业余", "函授"})
                {
                    FieldCode = "study_form",
                    FieldName = "学习形式",
                    MaxLength = 50,
                    OrderNum = 104,
                    remark = "业余<br/>函授",
                    isUfoProperty = false
                },
                new ImportFieldModel(new string[] {"2.5年制", "3年制", "4年制", "5年制"})
                {
                    FieldCode = "user_eductional",
                    FieldName = "学制",
                    MaxLength = 50,
                    OrderNum = 104,
                    remark = "2.5年制<br/>3年制<br/>4年制<br/>5年制",
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_dormitory",
                    FieldName = "宿舍",
                    MaxLength = 50,
                    OrderNum = 105,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "user_zipcode",
                    FieldName = "邮编",
                    MaxLength = 50,
                    OrderNum = 106,
                    isUfoProperty = false
                },
                new ImportFieldModel()
                {
                    FieldCode = "graduation_date",
                    FieldName = "毕业时间",
                    isDateTimeType = true,
                    MaxLength = 50,
                    OrderNum = 107,
                    isUfoProperty = false
                },
                new ImportFieldModel(new string[] {"", "是", "否"})
                {
                    FieldCode = "is_graduation",
                    FieldName = "毕业与否",
                    MaxLength = 50,
                    OrderNum = 108,
                    remark = "是<br/>否",
                    isUfoProperty = false
                },
                // new ImportFieldModel{FieldCode="user_cooperation",FieldName="教学点",MaxLength=50,OrderNum=109,isUfoProperty=false}
            };
            if (isSigleMooc)
            {
                importFieldModelList = importFieldModelList.Where(m => m.isUfoProperty == false).ToList();
            }
            return importFieldModelList;
        }

        /// <summary>
        /// 学生导入模板下载
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public System.IO.Stream GetStudentInfoExportTemplate(string names)
        {
            if (!string.IsNullOrEmpty(names))
            {
                List<string> fieldName = names.TrimEnd(',').Split(',').ToList();
                return ExcelImportExport.ExportTemplate(GetStudentInfoMap(), fieldName);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 汇入学生+ImportResultOutputDto CreateUserInfoFromFile(Stream fileStream, string departmentUid)

        public ImportResultOutputDto CreateUserInfoFromFile(Stream fileStream, string departmentUid)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                throw new UserFriendlyException("没有登录");
            }

          
            var result = new ImportResultOutputDto();
            try
            {
                var isUsernameContainEn = _iSiteSetService.GetAllSiteSet().usernameContainEN.Trim() == "open";
                var helper = new ImportUserInfoHelper(_userInfoApiService, _userInfoService);
                string errMsg;
                // helper.ImportFromExcel( );

                /**/

                ExcelImportExport excel = new ExcelImportExport();
                List<user_info_import> userInfoList = excel.ExcelImport<user_info_import>(fileStream, GetStudentInfoMap(), out errMsg);
                
                if (!string.IsNullOrEmpty(errMsg))
                {
                    return new ImportResultOutputDto
                    {
                        successCount = 0,
                        errMessage = errMsg
                    };
                }
                if (userInfoList == null || userInfoList.Count == 0)
                {
                    // return 0;
                    return new ImportResultOutputDto
                    {
                        successCount = 0,
                        errMessage = errMsg
                    };
                }

                var successCount = 0;
                //插入数据
                if (userInfoList.Count > 0)
                {
                    userInfoList.ForEach(m =>
                    {
                        var index = userInfoList.IndexOf(m);
                        string itemErrMsg = string.Empty;
                        //if (!InsertStu(m, UserInfoList, departmentUid, helper, ref itemErrMsg))
                        if (!helper.CheckUserInfo(m, userInfoList, ref itemErrMsg, isUsernameContainEn))
                        {
                            errMsg += $"第{(index + 3).ToString()}行数据错误，错误原因：{itemErrMsg} <br/>";
                        }
                        else
                        {
                           
                            var classId = departmentUid.TryParseGuid();
                            //如果模板中填写教学班级则以模板内填写的为准否则导入选中的教学班级
                            if (!string.IsNullOrWhiteSpace(m.user_class))
                            {
                                //判断教学班级是否存在
                                var classModel = _iClassRep.FirstOrDefault(a => a.name.Equals(m.user_class));
                                if (classModel != null)
                                    classId = classModel.Id;
                                else
                                {
                                    errMsg += $"第{(index + 3).ToString()}行数据错误，错误原因：教学班级[{m.user_class}]不存在 <br/>";
                                }
                            }
                            m.user_classid = classId.ToString();
                            successCount++;
                        }

                    });

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        exportStuInsert(userInfoList, departmentUid, cookie.Id, ref errMsg);
                    }

                }
     
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

        private bool exportStuInsert(List<user_info_import> userInfoList, string departId, Guid userId,  ref string msg)
        {
            try
            {
                var apiInputData = userInfoList.Select(a => new UserRegisterModel()
                {
                    UserName = a.login_name,
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

                userInfoList.ForEach(model =>
                {
                    var userBase = model.user_info_import2userBase();
                    userBase.Id = Guid.NewGuid();
                    userBase.smallAvatar = UserInfoImg.GetDefaultUserAvator(userBase.userGender);
                    userBase.userPassWord = sha1Encrypt.getSHA1Value(userBase.userPassWord ?? "");
                    userBase.identity = 1;
                    userBase.userEnbleFlag = false;
                    
                    var stu = model.user_info_import2StudentInfo();
                    stu.Id = Guid.NewGuid();
                    stu.isDel = false;
                    stu.createTime = DateTime.Now;
                    stu.userId = userBase.Id;
                  
                    //如果模板中填写行政班级则写入行政班级
                    if (!string.IsNullOrWhiteSpace(model.administrative_class))
                    {
                        var classModel = _iAdministrativeClassRep.FirstOrDefault(a => a.name.Equals(model.administrative_class));
                        if (classModel != null)
                        {
                            stu.facultyId = classModel.facultyId;
                            stu.majorId = classModel.majorId;
                            stu.classId = classModel.Id;
                        }
                    }
                    //如果模板中填写教学班级则以模板内填写的为准否则导入选中的教学班级
                    if (!string.IsNullOrWhiteSpace(model.user_classid))
                    {
                        _iClassStudentRep.Insert(new ClassStudent
                        {
                            Id = Guid.NewGuid(),
                            ClassId = model.user_classid.TryParseGuid(),
                            UserId = stu.userId,
                            CreateTime = DateTime.Now
                        });
                    }
                 
                    _userRepository.Insert(userBase);
                    _studentInfoRepository.Insert(stu);

                  

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

        private bool InsertStu(user_info_import model, List<user_info_import> UserInfoList, string departId,
            ImportUserInfoHelper excelHelper, ref string msg)
        {
            try
            {
                if (excelHelper.CheckUserInfo(model, UserInfoList, ref msg, false))
                {
                    var userBase = model.user_info_import2userBase();
                    userBase.Id = Guid.NewGuid();
                    userBase.smallAvatar = UserInfoImg.GetDefaultUserAvator(userBase.userGender);
                    userBase.userPassWord = sha1Encrypt.getSHA1Value(userBase.userPassWord ?? "");
                    userBase.identity = 1;
                    userBase.userEnbleFlag = false;

                    var stu = model.user_info_import2StudentInfo();
                    stu.Id = Guid.NewGuid();
                    stu.isDel = false;
                    stu.createTime = DateTime.Now;
                    stu.userId = userBase.Id;

                    _userRepository.Insert(userBase);
                    _studentInfoRepository.Insert(stu);
                 

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                msg = "系统错误。错误信息：" + ex.ToString();
                return false;
            }

        }


        public List<StudentInfoDto> GetStudentInfoDtoList(List<BatchDeleteRequestInputByUser> inputList, StudentInfoInputDto input, ref int total)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                throw new UserFriendlyException("没有登录");
            }

            input = StringHelper.TrimStr<StudentInfoInputDto>(input);
            var studentQueryable = _studentInfoRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.user_code), s => !string.IsNullOrEmpty(s.userCode) && s.userCode.Contains(input.user_code))
                .Where(a => a.isDel == false);
            if (!cookie.IsAdmin)
            {   //获取能查看的班级
                var students = _iClassStudentRep.GetAll()
                    .Join(_iClassTeacherRep.GetAll(), c => c.ClassId, ct => ct.ClassId, (c, ct) => new { c, ct })
                    .Where(@t => @t.ct.UserId == cookie.Id)
                    .Select(@t => @t.c.UserId).ToList();
                studentQueryable = studentQueryable.Where(a => students.Any(s => s.Equals(a.userId)));
            
            }
            //查询教学班级
            var classStudentQuery = _iSqlExecuter.SqlQuery<ClassStudentDto>(@"select userId,GROUP_CONCAT(CAST(classId as CHAR) SEPARATOR ',') as classIds,GROUP_CONCAT(name SEPARATOR ',') as classNames from class_student join  class on class_student.classId=class.id GROUP BY userId");

            var userQueryable = _userRepository.GetAll().Where(a => a.identity == 1)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_login_name), s => !string.IsNullOrEmpty(s.userLoginName) && s.userLoginName.Contains(input.user_login_name))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_idcard), s => !string.IsNullOrEmpty(s.userIdcard) && s.userIdcard.Contains(input.user_idcard))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_mobile), s => !string.IsNullOrEmpty(s.userMobile) && s.userMobile.Contains(input.user_mobile))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_email), s => !string.IsNullOrEmpty(s.userEmail) && s.userEmail.Contains(input.user_email))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_name), s => !string.IsNullOrEmpty(s.userFullName) && s.userFullName.Contains(input.user_name))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.user_gender), s => s.userGender == input.user_gender);

            var query = from a in studentQueryable
                join b in userQueryable on a.userId equals b.Id
                join faculty in _iFcultyaRep.GetAll() on a.facultyId equals faculty.Id
                into ftemp
                from ft in ftemp.DefaultIfEmpty()
                join major in _iMajorRep.GetAll() on a.majorId equals major.Id
                into mtemp
                from mt in mtemp.DefaultIfEmpty()
                        join classes in _iAdministrativeClassRep.GetAll() on a.classId equals classes.Id
                into ctemp
                from ct in ctemp.DefaultIfEmpty()
                join cs in classStudentQuery on a.userId equals cs.userId
                into cstemp
                from cst in cstemp.DefaultIfEmpty()
                        select
                    new StudentInfoDto()
                    {
                        Id = a.Id,
                        user_id = a.userId,
                        user_login_name = b.userLoginName,
                        user_code = a.userCode,
                        user_mobile = b.userMobile,
                        user_email = b.userEmail,
                        user_name = b.userFullName,
                        user_nickname = a.userNickname,
                        user_national = b.userNational,
                        user_political = b.userPolitical,
                        user_password = b.userPassWord,
                        user_province = a.userProvince,
                        user_city = a.userCity,
                        user_birthday = b.userBirthday.HasValue ? Convert.ToDateTime(b.userBirthday.Value.ToShortDateString()) : b.userBirthday,
                        user_gender = b.userGender == "1" ? "男" : (b.userGender == "2" ? "女" : ""),
                        user_facultyid = a.facultyId.ToString(),
                        user_faculty = ft != null ? ft.name : "",
                        user_majorid = a.majorId.ToString(),
                        user_major = mt != null ? mt.name : "",
                        user_administrativeclassid = a.classId.ToString(),
                        user_administrativeclass = ct != null ? ct.name : "",
                        user_class = cst != null ? cst.classNames : "",
                        study_form = a.studyForm,
                        study_flag = a.studyFlag,
                        level = a.level,
                        user_eductional = a.userEductional,
                        user_admission = a.userAdmission,
                        user_grade = a.userGrade,
                        user_dormitory = a.userDormitory,
                        user_idcard = b.userIdcard,
                        user_zipcode = a.userZipcode,
                        user_inviteCode = a.userInviteCode,
                        user_register_inviteCode = a.userRegisterInviteCode,
                        user_enble_flag = a.userEnbleFlag,
                        is_graduation = a.isGraduation,
                        graduation_date = a.graduationDate,
                        create_time = a.createTime,
                        updateTime = a.updateTime
                        ,
                        isDel = a.isDel
                        ,
                        about = b.about,
                        signature = b.signature,
                        largeAvatar = b.largeAvatar,
                        mediumAvatar = b.mediumAvatar,
                        smallAvatar = b.smallAvatar
                        ,
                        loginIp = b.loginIp,
                        loginTime = b.loginTime,
                        approvalStatus = b.approvalStatus,
                        newMessageNum = b.newMessageNum,
                        newNotificationNum = b.newNotificationNum
                    };
            var totalQuery = query;
               
            var res = string.IsNullOrWhiteSpace(input.OrderExpression)
                ? totalQuery.OrderByDescending(a => a.create_time).Skip(input.Skip).Take(input.PageSize).ToList()
                : totalQuery.OrderBy(input.OrderExpression).Skip(input.Skip).Take(input.PageSize);

            if (inputList.Count > 0)
            {
                query = res.Join(inputList, a => a.user_id.ToString(), b => b.user_id, (a, b) => a);
            }
            total = totalQuery.Count();
            return query.ToList();
        }

        public async Task CreateRecovryStudentInfo(StudentInfo model)
        {
            try
            {
                _studentInfoRepository.Insert(model);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        #region 注册用户审核
        public async Task ApplyStudents(ApplyStudent input)
        {
            if (string.IsNullOrWhiteSpace(input.Id))
            {
                return;
            }
            var ids = input.Id.Split(',');

            foreach (string t in ids)
            {
                if(string.IsNullOrEmpty(t))
                    continue;
                Guid gid;
                Guid.TryParse(t, out gid);
                var userBase = await _userRepository.FirstOrDefaultAsync(gid);
                if (userBase != null)
                {
                    userBase.approvalStatus = input.status;
                }
            }
        }

        public EasyUiListResultDto<ApplyStudentOutDto> GetApplyStudentUiList(ApplyStudentInputDto input)
        {
            var result = new EasyUiListResultDto<ApplyStudentOutDto>();
            try
            {
                if (!LoginValidation.IsLogin())
                {
                    throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
                }
                var cookie = CookieHelper.GetLoginInUserInfo();
                var classes = cookie.IsAdmin ? _iClassRep.GetAll() : _iClassRep.GetAll()
                    .Join(_iClassTeacherRep.GetAll(), c => c.Id, ct => ct.ClassId, (c, ct) => new { c, ct })
                    .Where(@t => @t.ct.UserId == cookie.Id)
                    .Select(@t => @t.c);
                //用户 基本信息
                var userQueryable = _userRepository.GetAll().Where(a => a.identity == 1)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.userLoginName),
                        s => !string.IsNullOrEmpty(s.userLoginName) && s.userLoginName.Contains(input.userLoginName))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.userFullName),
                        s => !string.IsNullOrEmpty(s.userFullName) && s.userIdcard.Contains(input.userFullName))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.userMobile),
                        s => !string.IsNullOrEmpty(s.userMobile) && s.userMobile.Contains(input.userMobile))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.userEmail),
                        s => !string.IsNullOrEmpty(s.userEmail) && s.userEmail.Contains(input.userEmail))
                    .Where(s => s.approvalStatus.Equals(input.status));
                //学生
                var studens = _iClassStudentRep.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.classId),
                    s => input.classId.Contains(s.ClassId.ToString()));
              
                var totalClassStudents =
                    from s in _iClassStudentRep.GetAll()
                    join p in _userRepository.GetAll().Where(s => s.approvalStatus.Equals("approved"))
                    on s.UserId equals p.Id
                    join c in _iClassRep.GetAll() on s.ClassId equals c.Id
                    group new { s.ClassId, s.UserId } by s.ClassId into g

                    select new
                    {
                        g.Key,
                        studens = g.Count()
                    };
                //班级用户统计
                var query =
                    from s in studens
                    join p in userQueryable on s.UserId equals p.Id
                    join c in classes on s.ClassId equals c.Id
                    join faculty in _iFcultyaRep.GetAll() on c.facultyId equals faculty.Id
                    join major in _iMajorRep.GetAll() on c.majorId equals major.Id
                    join tcs in totalClassStudents on c.Id equals tcs.Key into temp1
                    from t1 in temp1.DefaultIfEmpty()
                    select new ApplyStudentOutDto
                    {
                        id = p.Id,
                        className = c.name,
                        classStudentName = c.studentNum,
                        facultyName = faculty.name,
                        majorName = major.name,
                        userEmail = p.userEmail,
                        userFullName = p.userFullName,
                        userLoginName = p.userLoginName,
                        userMobile = p.userMobile,
                        studentName = t1?.studens ?? 0,
                        createTime=s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        status = p.approvalStatus
                    };
             
                result.total = query.Count();
                result.rows = query.OrderBy(input.OrderExpression).Skip(input.skip).Take(input.pageSize).ToList();
                
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
            }
            return result;
        }
    #endregion
    }

}
