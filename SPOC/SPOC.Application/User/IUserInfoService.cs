using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Runtime.Validation;
using SPOC.Common;
using SPOC.User.Dto.UserInfo;

namespace SPOC.User
{
    public interface IUserInfoService : IApplicationService
    {
        Task RemoveLoginSessionId(Guid userId, string sessionId);
        UserCookie GetUserSession(string loginName, string password, string ipAddress, ref string msg, bool isStuLogin = true, bool isMobileLogin = false);

        LoginSuccessData LoginRequest(string loginName, string password, string ipAddress, ref string msg, bool isAdminLogin = false, bool isMobileLogin = false, bool isRemberMe = false, bool isSiteLogin = false);

        UserBase UserRegister(UserRegisterModel model, ref string msg, bool isMobileRegister = false, string mobileIP = "");

        UserBase GetUserBaseByQueryDto(UserInfoQueryInputDto model);
        UserBase GetUserBaseByUserName(string userName);

        List<UserBase> GetUserBaseByEmail(string email);

        List<UserBase> GetUserBaseByPhone(string phoneNumber);

        List<UserBase> GetUserBaseByIdCard(string idCard);

        UserBase GetUserBaseByPhoneById(Guid id);

        bool UserPwdModifyByApi(ModifyPassWord model, ref string msg);

        bool UpdateUserByApi(UserInfoInputDto model, List<string> perLiset, ref string msg);
        Task<ReturnMsg> UserPassWordModify(ModifyPassWord model);

        List<UserBase> GetUserBaseByExpre(Expression<Func<UserBase, bool>> expre);

        bool UpdateUser(UserBase user);

        UserInfoDto GetUserInfoInputDtoById(Guid id);

        bool UpdateUserByUserInfoInputDto(UserInfoInputDto user, List<string> perLiset, ref string msg);

        bool MobileExistCheck(string RegisterMobile);
        [DisableValidation]
        bool CheckNameExit(string name, string type, string oldname = "", bool isRemoteCheck = true);
        [DisableValidation]
        bool CheckMobileExist(string mobile, string type, string oldMobile = "", bool isRemoteCheck = false);
        [DisableValidation]
        bool CheckEmailExist(string email, string type, string oldEmail = "", bool isRemoteCheck = true);
        [DisableValidation]
        bool CheckIdCardExist(string idCard, string type, string oldIdCard = "");


        bool UserNameModify(UserInfoMogifyInputDto model, ref string msg);

        string GetUserBaseById(UserInfoInputDto input);


        Task<string> userForgetPassWord(string userNmae, string urlSrc);

        ReturnMsg UserEmailSetting(UserEmailModifyView model, UserCookie _user, string returnUrl);

        Task<bool> PwdModifyByForget(ModifyPassWord model, bool ignoreoldpw);

        bool EmailModifySetting(UserEmailModifyView model, bool ignoreoldpw, ref string msg);

        ReturnMsg PwdModifry(ModifyPassWord model);

        UserDetailShow GetUserDetailShowDtoById(Guid id);

        Task UserLoginRequset(LoginUser model);



        CenterUser GetCenterUser(string userId);

        string UserSwitch(string userName);

        Task RecoverInsertUsers(UserBase model);

        LoginSuccessData SmsLoginRequset(string mobile,string smsCode, string ipAddress, ref string msg, bool isSiteLogin = false);
        UserBase UserSmsRegister(UserRegisterModel model, ref string msg);

        bool CheckPwd(Guid id, string pwd);
        Task<bool> mobileMpdifry(MobileModifyView model);
        /// <summary>
        /// 检查用户是否超级管理员
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        bool IsSuperAdmin(Guid userId);
    }
}
