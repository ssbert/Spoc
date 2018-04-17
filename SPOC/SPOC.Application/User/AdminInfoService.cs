using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Newtonsoft.Json;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.Encrypt;
using SPOC.Common.Enum;
using SPOC.Common.Exam;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.SysSetting.RoleManageDTO;
using SPOC.User.Dto.Admin;
using SPOC.User.Dto.Common;
using SPOC.User.Dto.UserInfo;
using Abp.AutoMapper;
using Abp.Runtime.Validation;

namespace SPOC.User
{
    public class AdminInfoService : SPOCAppServiceBase, IAdminInfoService
    {
        private readonly IRepository<AdminInfo, Guid> _adminInfoRepository;

        private readonly IRepository<UserBase, Guid> _userRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

       
        private readonly IRepository<UserRole, Guid> _iUserRoleRep;
        private readonly IRepository<RoleManage, Guid> _iRoleManageRep;
        private readonly IUserInfoApiService _userInfoApiService;


        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="adminInfoRepository"></param>
        /// <param name="unitOfWorkManager"></param>
        /// <param name="userRepository"></param>
        /// <param name="userInfoApiService"></param>
        /// <param name="iUserRoleRep"></param>
        /// <param name="iRoleManageRep"></param>
        /// <param name="userRepository1"></param>
        public AdminInfoService(
            IRepository<AdminInfo, Guid> adminInfoRepository, 
            IUnitOfWorkManager unitOfWorkManager, 
            IRepository<UserBase, Guid> userRepository,
          
            IUserInfoApiService userInfoApiService,
            IRepository<UserRole, Guid> iUserRoleRep, IRepository<RoleManage, Guid> iRoleManageRep, IRepository<UserBase, Guid> userRepository1)
        {
            _adminInfoRepository = adminInfoRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userInfoApiService = userInfoApiService;
            _iUserRoleRep = iUserRoleRep;
            _iRoleManageRep = iRoleManageRep;
            _userRepository = userRepository1;
        }


        public EasyUiListResultDto<AdminInfoDto> GetAdminInfoByGuid(AdminInfoInputDto input)
        {
            input = StringHelper.TrimStr<AdminInfoInputDto>(input);       
            var adminQuery = _adminInfoRepository.GetAll().Where(a => a.isDel == false).ToList();
            var userQuery = _userRepository.GetAll().Where(a => a.identity == 3)
            //.WhereIf(!string.IsNullOrWhiteSpace(input.userLoginName), s => s.userLoginName.Contains(input.userLoginName))
            .WhereIf(!string.IsNullOrWhiteSpace(input.userLoginName), s => !string.IsNullOrEmpty(s.userLoginName) && s.userLoginName.ToLower().Contains(input.userLoginName.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(input.userMobile), s => !string.IsNullOrEmpty(s.userMobile) && s.userMobile.Contains(input.userMobile))
             .WhereIf(!string.IsNullOrWhiteSpace(input.userEmail), s => !string.IsNullOrEmpty(s.userEmail) && s.userEmail.Contains(input.userEmail))
              .WhereIf(!string.IsNullOrWhiteSpace(input.userFullName), s => !string.IsNullOrEmpty(s.userFullName) && s.userFullName.ToLower().Contains(input.userFullName.ToLower()));


            var query =
               from a in  adminQuery join b in userQuery on  a.userId equals b.Id 
            
                    
               select new AdminInfoDto()
               {
                   Id = a.Id,
                   userId = a.userId,
                   userLoginName = b.userLoginName,
                   userPassWord = b.userPassWord,
                   userFullName = b.userFullName,
                   userMobile = b.userMobile,
                   userEmail = b.userEmail,
                   userGender = b.userGender,
                   userBirthday = b.userBirthday,
                   userIdcard = b.userIdcard,
                   userNational = b.userNational,
                   userPolitical = b.userPolitical,
                   adminInviteCode = a.adminInviteCode,
                   recentLoginTime = b.loginTime.ToString(),
                   recentLoginIpAddress = a.recentLoginIpAddress,
                   isDel = a.isDel,
                   isAdmin = a.isAdmin,
                   adminEnbleFlag = a.adminEnbleFlag,
                   createTime = a.createTime.ToString(),
                   updateTime = a.updateTime.ToString()

                    , loginIp=b.loginIp,
                     loginTime=b.loginTime
                     ,
                   approvalStatus = b.approvalStatus,
                   newMessageNum = b.newMessageNum,
                   newNotificationNum = b.newNotificationNum
                     ,
                  
                   about = b.about,
                   signature = b.signature,
                   largeAvatar = b.largeAvatar,
                   mediumAvatar = b.mediumAvatar,
                   smallAvatar = b.smallAvatar
      
               };

            var totalQuery = query;
         //   var totalQuery = string.IsNullOrEmpty(input.department) ? query: query.Join(_user2DepartmentRepository.GetAll().Where(s => input.department.Contains(s.DepartmentId.ToString())), a => a.userId, b => b.UserId, (a, b) => a);
            var res = string.IsNullOrWhiteSpace(input.OrderExpression) ? totalQuery.OrderByDescending(a => DateTime.Parse(a.createTime ?? "1911-01-01")).Skip(input.Skip).Take(input.PageSize).ToList() : totalQuery.OrderBy(input.OrderExpression).Skip(input.Skip).Take(input.PageSize).ToList();
        

            return new EasyUiListResultDto<AdminInfoDto>
            {
                  total = totalQuery.LongCount(),
                
               // rows = query.OrderBy(a=>a.createTime).Skip(input.Skip).Take(input.PageSize).ToList()

                rows = res

                 
            }; 
        }


        public AdminInfoDto GetAdminInfoDtoByQueryInput(UserInfoQueryInputDto model) {
           


            var adminQuery = _adminInfoRepository.GetAll()
                .WhereIf(model.id!=Guid.Empty,a=>a.Id==model.id)
                .Where(a => a.isDel == false).ToList();
            var userQuery = _userRepository.GetAll().Where(a => a.identity == 3)
            .WhereIf(model.userId != Guid.Empty, s => s.Id == model.userId)
            .WhereIf(!string.IsNullOrWhiteSpace(model.userName), s => s.userLoginName == model.userName);
             

       //     var prmissionInfoList = _adminPermissionInfoRepository.GetAllList();

            var query =
               from a in adminQuery
               join b in userQuery on a.userId equals b.Id

               select new AdminInfoDto()
               {
                   Id = a.Id,
                   userId = a.userId,
                   userLoginName = b.userLoginName,
                   userPassWord = b.userPassWord,
                   userFullName = b.userFullName,
                   userMobile = b.userMobile,
                   userEmail = b.userEmail,
                   userGender = b.userGender,
                   userBirthday = b.userBirthday,
                   userIdcard = b.userIdcard,
                   userNational = b.userNational,
                   userPolitical = b.userPolitical,
                   adminInviteCode = a.adminInviteCode,
                   recentLoginTime = a.recentLoginTime.ToString(),
                   recentLoginIpAddress = a.recentLoginIpAddress,
                   isDel = a.isDel,
                   adminEnbleFlag = a.adminEnbleFlag,
                   createTime = a.createTime.ToString(),
                   updateTime = a.updateTime.ToString()

                    ,
                   loginIp = b.loginIp,
                   loginTime = b.loginTime
                     ,
                   approvalStatus = b.approvalStatus,
                   newMessageNum = b.newMessageNum,
                   newNotificationNum = b.newNotificationNum
                     ,
               
                   about = b.about,
                   largeAvatar = b.largeAvatar,
                   mediumAvatar = b.mediumAvatar,
                   smallAvatar = b.smallAvatar

                  

               };
            var res = query.FirstOrDefault();
            return res;

        }

       /// <summary>
       /// 管理员初始化数据
       /// </summary>
       /// <returns></returns>
        public async Task AddDefaultAdmin() {
            Logger.Info("Creating defalutAdmin");

           
            UserBase _user = new UserBase() ;
            _user.Id = Guid.NewGuid();
            _user.identity = 3;
            _user.userPassWord = sha1Encrypt.getSHA1Value("123456");
            AdminInfo admin = new AdminInfo() ;
            admin.Id=Guid.NewGuid();
            admin.userId = _user.Id;
            admin.createTime = DateTime.Now;
            admin.updateTime = DateTime.Now;
            admin.isDel = false;

            UserRegisterModel reginModel = _user.MapTo<UserInfoDto>().GetUserRegister();
            reginModel.PassWord = "123456";
            var apiRes = _userInfoApiService.Register(null, reginModel).Result;//调用新课网的接口，将用户添加到新课网中
            if (!apiRes.IsSuccess)
            {
                throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,apiRes.ErrMsg ?? "新增管理员失败");
            } 


            await _userRepository.InsertAsync(_user);
            await _adminInfoRepository.InsertAsync(admin);
        }

        public void UpdateAdminInfo(CreateAdminInfoInputDto input)
        {
            throw new NotImplementedException();
        }
        [DisableValidation]
        public async Task CreateAdminInfo(CreateAdminInfoInputDto input)
        {
            try
            {
                Logger.Info("Creating a row for input: " + input);

                input.Id = Guid.NewGuid();
                input.userId = Guid.NewGuid();
                UserBase _user = input.GetUser();
                _user.userPassWord = sha1Encrypt.getSHA1Value(input.userPassWord ?? "");
                _user.userEnbleFlag = false;
                _user.isCompleted = false;
                _user.smallAvatar = UserInfoImg.GetDefaultUserAvator(_user.userGender);
                AdminInfo admin = input.GetAdminInfo();
                admin.createTime = DateTime.Now;
                admin.updateTime = DateTime.Now;
                admin.isDel = false;
                admin.adminEnbleFlag = false;
                admin.isAdmin = input.isAdmin=="1";
                UserRegisterModel reginModel = _user.MapTo<UserInfoDto>().GetUserRegister();
                reginModel.PassWord = input.userPassWord;
                var apiRes = _userInfoApiService.Register(null, reginModel).Result;//调用新课网的接口，将用户添加到新课网中
                if (!apiRes.IsSuccess)
                {
                    if (apiRes.ErrMsg.Trim() == "UserExist")//当该学习在新课网中存在的时候，将该学生信息更新到新课网。
                    {
                        EditDto editDto = _user.MapTo<UserInfoDto>().GetEditDto();
                        editDto.newpw = string.IsNullOrWhiteSpace(input.userPassWord) ? "" : sha1Encrypt.AirEncode(input.userPassWord ?? "");
                        var res = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
                        if (!res.IsSuccess)
                        {
                            throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info, res.ErrMsg ?? "新增管理员失败");
                        }
                        else {
                            var obj = JsonConvert.DeserializeObject<UserInfoDto>(res.Data.InKey.ToString());
                            _user.newMoocUserId = obj.Id;//如果新课网新增用户成功，讲新课网用户Id保存到本地
                           // _user.newMoocUserId = res.Data.InKey.ToString();//如果新课网新增用户成功，讲新课网用户Id保存到本地
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info, apiRes.ErrMsg ?? "新增管理员失败");
                    }


                }
                else {
                    _user.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey);//如果新课网新增用户成功，讲新课网用户Id保存到本地
                }
                
              

                //调用仓储基类的Insert方法把实体保存到数据库中

                await _userRepository.InsertAsync(_user);
                await _adminInfoRepository.InsertAsync(admin);

                var role = _iRoleManageRep.FirstOrDefault(a => a.roleGroup == "admin" && a.isDefault);
                if (role != null)
                {
                    await _iUserRoleRep.InsertAsync(new UserRole { userId = admin.userId, roleId = role.Id });
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        /// <summary>
        /// 禁用管理员
        /// </summary>
        /// <returns></returns>
        public async Task ActiveAdminInfo(List<BatchDeleteRequestInputByUser> inputList)
        {
            foreach (var input in inputList)
            {
                var id = string.IsNullOrEmpty(input.Id) ? Guid.Empty : new Guid(input.Id);
                var admin = _adminInfoRepository.Get(id);
               // .Get(id);
                admin.adminEnbleFlag = !admin.adminEnbleFlag;
                _adminInfoRepository.Update(admin);

                UserBase userBase = _userRepository.Get(admin.userId);
                userBase.userEnbleFlag = admin.adminEnbleFlag;
                _userRepository.Update(userBase);
            }
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }

        public Task DeleteAdminInfo(BatchRequestInput input)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAdminInfos(List<BatchDeleteRequestInputByUser> inputList)
        {
            Logger.Info("Deleting a row for input: " + inputList);
            foreach (BatchDeleteRequestInputByUser item in inputList)
            {
                //  await _userRepository.DeleteAsync(new Guid(item.user_id));
                // await _teacherInfoRepository.DeleteAsync(new Guid(item.id));

                AdminInfo admin = _adminInfoRepository.Get(new Guid(item.Id));
                admin.isDel = true;

                _userRepository.Delete(admin.userId);

                _adminInfoRepository.Delete(admin);
              //  _adminInfoRepository.Update(admin);
            }
           
          await  _unitOfWorkManager.Current.SaveChangesAsync();
        }

        public void UpdateAdminInfoByUser(UpdateAdminInfoInputDto input)
        {
            UserBase _user = input.GetUser();
            UserBase baseUser = _userRepository.Get(input.userId);
            _user.userPassWord = string.IsNullOrWhiteSpace(input.userPassWord) ? baseUser.userPassWord : sha1Encrypt.getSHA1Value(input.userPassWord);

            /*-- 判断手机号码是否改变，如果改变，在新课网上修改手机号码--*/
            if (baseUser.userMobile != input.userMobile)
            {
                 var mobileExist = _userInfoApiService.CheckMobile(null, new UserRegisterModel() { RegisterMobile = input.userMobile }).Result.IsSuccess;
                 if (mobileExist)
                 {
                     ModifyMobile modifry = new ModifyMobile() { newmobile = input.userMobile, oldmobile = baseUser.userMobile,username= baseUser.userLoginName };
                     var apiRes = _userInfoApiService.ModifryUserMobile(null, modifry).Result;
                     if (!apiRes.IsSuccess)
                     {
                         throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,apiRes.ErrMsg ?? "手机号修改失败");
                     }
                 }
            }
            var apiNewLoginName = baseUser.userLoginName == input.userLoginName ? "" : input.userLoginName;
            var oldUserLoginName = baseUser.userLoginName;
            /*----------*/
            _user.loginTime = baseUser.loginTime;
            _user.loginIp = baseUser.loginIp;
            ObjHelper.ObjSetValue<UserBase, UserBase>(ref baseUser, ref  _user);

            /*--把信息更新到新课网中--*/
            EditDto editDto = baseUser.MapTo<UserInfoDto>().GetEditDto();
            editDto.newpw = string.IsNullOrWhiteSpace(input.userPassWord) ? "" : sha1Encrypt.AirEncode(input.userPassWord ?? "");
            editDto.username = apiNewLoginName;
            editDto.oldusername = oldUserLoginName;
            var res = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
            if (!res.IsSuccess)
            {
                throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,res.ErrMsg ?? "信息更新失败");
               // throw (new Exception(res.ErrMsg));
            }
            /*-------------------------*/
            var admin = _adminInfoRepository.GetAll().FirstOrDefault(a => a.userId.Equals(input.userId));
            if (admin != null)
            {
                admin.isAdmin = input.isAdmin=="1" || input.isAdmin == "true";
            }



            //  _unitOfWorkManager.Current.SaveChanges();
        }


        public List<JsonTree> GetJsonTree(List<RoleManageDto> perList)
          {

          //    List<RolePermissionDto> perList = _roleManageService.GetAllRolePermission().Result;
              List<JsonTree> childTree = new List<JsonTree>();
              foreach (var item in perList)
              {
                 // childTree.Add(new JsonTree() { text = item.roleName, children = null, id = item.id, ParentId = "xxxx" });
                  childTree.Add(new JsonTree() { text = item.roleName, children = null, id = item.id, ParentId = "xxxx" });
              }

              List<JsonTree> jsonData = new List<JsonTree>();

           /*  if (perList.Count > 0) {
                  jsonData.Add(new JsonTree
                  {
                      id = "xxxx",
                      children = childTree,
                      ParentId = "",
                      text = "权限列表",

                  });
              }
            * */
                  
          //    return jsonData;
              return childTree;
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
                            if (item.id == data.pid)
                            {
                                item.children.Add(new SelectListItemDto { id = data.id.ToString(), text = data.schoolName, @checked = typeId == data.id.ToString() });
                            }
                            break;
                        case "name":    //学科、标签
                            if (item.id == data.pid)
                            {
                                item.children.Add(new SelectListItemDto { id = data.id.ToString(), text = data.name, @checked = typeId == data.id.ToString() });
                            }
                            break;
                        case "parentUid":   //分类
                            if (item.id == data.parentUid)
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


        /// <summary>
        /// 检验值是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">验证类型，新增或更改</param>
        /// <returns></returns>
        public bool CheckNameExit(string name, string type, string oldname = "")
        {
        

            if (type == "insert")
            {
                return _userRepository.GetAll().Any(s => s.userLoginName == name);
               
            }
            else
            {
                return _userRepository.GetAll().Where(d => d.userLoginName != oldname).Any(m => m.userLoginName == name);
               
            }
        }

        public async Task RecoverInsertAdmin(CreateAdminInfoInputDto input)
        {
            try
            {
                AdminInfo model = new AdminInfo();
                model.Id = Guid.NewGuid();
                model.createTime = DateTime.Now;
                model.userId = input.userId;
                model.isDel = false;
                model.adminEnbleFlag = false;
                await _adminInfoRepository.InsertAsync(model);

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            
            }
        }
    }
}
