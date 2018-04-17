using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Castle.Core.Logging;
using Newtonsoft.Json;
using SPOC.Common;
using SPOC.Common.Cookie;
using SPOC.Common.Encrypt;
using SPOC.Common.Enum;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.Common.Mail;
using SPOC.SystemSet;
using SPOC.User.Dto.UserInfo;

namespace SPOC.User
{
    public class UserInfoService : SPOCAppServiceBase, IUserInfoService
    {
        private readonly IRepository<UserBase, Guid> _userRepository;
        private readonly IRepository<StudentInfo, Guid> _studentRepository;
        private readonly IRepository<TeacherInfo, Guid> _teacherRepository;
        private readonly IRepository<AdminInfo, Guid> _adminRepository;
        private readonly IRepository<ClassStudent, Guid> _classStudentRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserInfoApiService _userInfoApiService;
        private readonly IRepository<SiteSet, Guid> _siteSetRepository;
        private readonly IRepository<Notification, Guid> _notificationRepository;
        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger _iLogger { get; set; }
        public UserInfoService(IRepository<UserBase, Guid> userRepository, IRepository<StudentInfo, Guid> studentRepository, IUnitOfWorkManager unitOfWorkManager, ILogger iLogger, IRepository<TeacherInfo, Guid> teacherRepository, IRepository<AdminInfo, Guid> adminRepository, IRepository<SiteSet, Guid> siteSetRepository,  IRepository<Notification, Guid> notificationRepository, IUserInfoApiService userInfoApiService, IRepository<ClassStudent, Guid> classStudentRepository)
        {
            _iLogger = iLogger;
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _teacherRepository = teacherRepository;
            _adminRepository = adminRepository;
            _siteSetRepository = siteSetRepository;
            _notificationRepository = notificationRepository;
            _userInfoApiService = userInfoApiService;
            _classStudentRepository = classStudentRepository;
        }



        /// <summary>
        /// 后台弹窗登录请求
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task UserLoginRequset(LoginUser model)
        {

            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {

                throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,"用户名或密码不能为空");
            }
            else
            {
                string errMsg = "";
                // UserSession userSession = null;
                //userSession = this.GetUserSession(model.UserName, model.Password, HttpContext.Current.Request.UserHostAddress, ref errMsg, false);
                //if (userSession != null)
                var loginUser = this.LoginRequest(model.UserName, model.Password, HttpContext.Current.Request.UserHostAddress, ref errMsg, true);
                if (loginUser != null)
                {
                    /*  if (userSession.IsAdmin||userSession.Identity==2)
                      {
                          CookieHelper.SetLoginInUserCookie(userSession, true);
                      }
                      else
                      {
                          throw new UserFriendlyException("用户名或密码错误！", UserFriendlyExceptionCode.Info);
                      }*/
                }
                else
                {
                    throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,string.IsNullOrEmpty(errMsg) ? "密码错误！" : errMsg);
                }
            }
        }


        /// <summary>
        /// 清除用户sessionId
        /// </summary>
        /// <returns></returns>
        public async Task RemoveLoginSessionId(Guid userId, string sessionId)
        {
            var newUser = _userRepository.Get(userId);
            if (newUser != null && newUser.sessionId == sessionId)
            {
                newUser.sessionId = "";
                _userRepository.Update(newUser);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }



        #region 登录得到用户信息+GetUserSession(string loginName, string password, string ipAddress, ref string msg)

        #region old
        /// <summary>
        /// 得到用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ipAddress">记录日志的地址</param>
        /// <returns></returns>
        public UserCookie GetUserSession(string loginName, string password, string ipAddress, ref string msg, bool isStuLogin = true, bool isMobileLogin = false)
        {
            UserBase modelAccount = null;
            ApiResponseResult<ApiUserInfoDto> apiRes = null;

            //   modelAccount = GetUserOtherWay(loginName, "", false);

            apiRes = _userInfoApiService.Login(null, loginName, sha1Encrypt.AirEncode(password ?? ""), "pc").Result;


            if (apiRes.IsSuccess)//当新课网存在用户，而本地不存在的时候，将用户信息保存到本地
            {
                var newUser = _userRepository.GetAll().Where(a => a.newMoocUserId.ToString() == apiRes.Data.InKey.id).FirstOrDefault();
                if (newUser != null)
                {
                    newUser.userLoginName = apiRes.Data.InKey.loginname;
                    newUser.userEmail = apiRes.Data.InKey.email;
                    newUser.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey.id) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey.id);
                    newUser.userFullName = apiRes.Data.InKey.loginname;
                    _unitOfWorkManager.Current.SaveChanges();
                    modelAccount = newUser;
                }
                else
                {
                    if (_userRepository.GetAll().Any(a => a.userMobile == apiRes.Data.InKey.mobile))//判断该用户是否存在于本地，根据手机号查询
                    {
                        modelAccount = GetUserOtherWay(apiRes.Data.InKey.mobile, "", false);
                        modelAccount.userLoginName = apiRes.Data.InKey.loginname;
                        modelAccount.userEmail = apiRes.Data.InKey.email;
                        modelAccount.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey.id) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey.id);
                        modelAccount.userFullName = apiRes.Data.InKey.loginname;
                        _unitOfWorkManager.Current.SaveChanges();
                    }
                    else
                    {
                        UserBase user = new UserBase();
                        user.Id = Guid.NewGuid();
                        user.userLoginName = apiRes.Data.InKey.loginname;
                        user.userMobile = apiRes.Data.InKey.mobile;
                        user.userEmail = apiRes.Data.InKey.email;
                        user.userPassWord = sha1Encrypt.getSHA1Value(password ?? "");
                        user.smallAvatar = UserInfoImg.GetDefaultUserAvator(user.userGender ?? "");
                        user.identity = 1;
                        user.isCompleted = false;
                        user.userEnbleFlag = false;

                        StudentInfo student = new StudentInfo();
                        student.Id = Guid.NewGuid();
                        student.isDel = false;
                        student.createTime = DateTime.Now;
                        student.updateTime = DateTime.Now;
                        student.userId = user.Id;
                        student.userEnbleFlag = 0;

                        _userRepository.Insert(user);
                        _studentRepository.Insert(student);
                        _unitOfWorkManager.Current.SaveChanges();

                        modelAccount = _userRepository.Get(user.Id);
                    }
                }

            }
            else
            {
                msg = apiRes.ErrMsg;
                return null;
            }


            modelAccount.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey.id) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey.id);

            var model = new UserCookie
            {
                Id = modelAccount.Id,
                UserName = modelAccount.userFullName,
                UserUid = modelAccount.Id.ToString(),
                LoginName = modelAccount.userLoginName,
                PassWord = modelAccount.userPassWord,
                IsAdmin = modelAccount.identity == 3,
                UserHeadImg = modelAccount.smallAvatar,
                Identity = modelAccount.identity,
                UserEnbleFlag = modelAccount.userEnbleFlag,
                Birthday = modelAccount.userBirthday.ToString(),
                PhoneNumber = modelAccount.userMobile,
                Gender = modelAccount.userGender,
                LoginIpAddress = modelAccount.loginIp,
                SessionId = modelAccount.sessionId,
                IsCompleted = modelAccount.isCompleted
            };
            //登录成功
            //记录日志

            modelAccount.sessionId = isMobileLogin ? "mobile" : System.Web.HttpContext.Current.Session.SessionID;
            modelAccount.loginTime = DateTime.Now;
            _userRepository.Update(modelAccount);
            _unitOfWorkManager.Current.SaveChanges();

            return model;

        }
        #endregion


        public LoginSuccessData LoginRequest(string loginName, string password, string ipAddress, ref string msg, bool isAdminLogin = false, bool isMobileLogin = false, bool isRemberMe = false, bool isSiteLogin = false)
        {
            ApiResponseResult<ApiUserInfoDto> apiRes = null;
            UserBase modelAccount = null;
            bool isLoginSuccess = false;
            Guid newUserId = Guid.Empty;
            try
            {

                LoginSuccessData successData = null;
                //是否本站登录
                if (isSiteLogin)
                {
                    successData = this.SiteLogin(loginName, password, ref msg, ref isLoginSuccess, isAdminLogin,isRemberMe);
                }
                else
                {
                    // _userInfoApiService.Login 方法有问题未解决，
                    //apiRes = _userInfoApiService.Login(null, loginName, sha1Encrypt.AirEncode(password ?? ""), "pc").Result;

                    var dc = new Dictionary<string, string>()
                    {
                        {"username",loginName},
                        {"password", sha1Encrypt.AirEncode(password ?? "")},
                        {"loginkey", "pc"},
                    };
                    var sign = SignatureHelper.GetSignature(dc);
                    var url = L("payUrl").TrimEnd('/') + "/api/" + "user/UserLogin?sign=" + sign;
                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                    var returnValue = "";
                    using (var http = new HttpClient(handler))
                    {
                        var content = new FormUrlEncodedContent(dc);
                        var postTask = http.PostAsync(url, content);
                        postTask.Wait();
                        var response = postTask.Result;
                        var readTask = response.Content.ReadAsStringAsync();
                        readTask.Wait();
                        returnValue = readTask.Result;
                    }
                    apiRes = JsonConvert.DeserializeObject<ApiResponseResult<ApiUserInfoDto>>(returnValue);

                    if (apiRes.IsSuccess)
                    {
                        successData = new LoginSuccessData()
                        { //获取登录的用户信息
                            email = apiRes.Data.InKey.email,
                            fullName = apiRes.Data.InKey.truename,
                            gender = apiRes.Data.InKey.gender,
                            loginName = apiRes.Data.InKey.loginname,
                            phoneNumber = apiRes.Data.InKey.mobile,
                            idCard = apiRes.Data.InKey.idcard
                        };

                        modelAccount = _userRepository.GetAll().FirstOrDefault(a => a.newMoocUserId.ToString() == apiRes.Data.InKey.id) ??
                                       _userRepository.GetAll().FirstOrDefault(a => a.userLoginName == successData.loginName);//查询本地
                        if (isAdminLogin)//当后台登录时
                        {
                            if (modelAccount == null)
                            {
                                msg = "该账号不存在，请重新输入";
                                isLoginSuccess = false;
                                successData = null;
                            }
                            else if (modelAccount.userEnbleFlag)
                            {
                                msg = "用户被禁用！";
                                isLoginSuccess = false;
                                successData = null;
                            }
                            else if (modelAccount.identity == 2 || modelAccount.identity == 3)
                            {
                                isLoginSuccess = true;
                                successData.identity = modelAccount.identity;
                                successData.userId = modelAccount.Id.ToString();
                                successData.isCompleted = modelAccount.isCompleted;
                                successData.UserHeadImg = modelAccount.smallAvatar;
                                successData.birthday = modelAccount.userBirthday.ToString();
                            }
                            else
                            {
                                msg = "该账号不存在，请重新输入！";
                                isLoginSuccess = false;
                                successData = null;
                            }
                        }
                        else
                        {
                            if (modelAccount == null)
                            {
                                msg = "该账号不存在，请重新输入";
                                isLoginSuccess = false;
                                successData = null;
                            }
                            else if (modelAccount.userEnbleFlag)
                            {
                                msg = "账号被禁用，请联系授课老师！";
                                isLoginSuccess = false;
                                successData = null;
                            }
                            else if (modelAccount.identity==1 && modelAccount.approvalStatus != "approved")
                            {
                                msg = "该账号未审核通过，请联系授课老师";
                                isLoginSuccess = false;
                                successData = null;
                            }
                            else
                            {
                                if (modelAccount == null)
                                {
                                    newUserId = Guid.NewGuid();
                                    successData.identity = 1;
                                    successData.userId = newUserId.ToString();
                                    successData.isCompleted = false;
                                    successData.UserHeadImg = UserInfoImg.GetDefaultUserAvator("");
                                    successData.birthday = "";
                                }
                                else
                                {
                                    successData.identity = modelAccount.identity;
                                    successData.userId = modelAccount.Id.ToString();
                                    successData.isCompleted = modelAccount.isCompleted;
                                    successData.UserHeadImg = modelAccount.smallAvatar;
                                    successData.birthday = modelAccount.userBirthday.ToString();
                                }
                                isLoginSuccess = true;
                            }

                        }
                    }
                    else
                    {
                        msg = apiRes.ErrMsg;
                        isLoginSuccess = false;
                    }
                }
                return isLoginSuccess ? successData : null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString);
                isLoginSuccess = false;
                return null;
            }
            finally
            {
                if(isLoginSuccess)
                LoginManger(apiRes, newUserId, modelAccount, loginName, password, ipAddress, isLoginSuccess, isAdminLogin, isMobileLogin, isRemberMe);//最后处理登录逻辑 

            }
        }

        /// <summary>
        /// 本站登录
        /// </summary>
        /// <param name="longinName"></param>
        /// <param name="pwd"></param>
        /// <param name="msg"></param>
        /// <param name="isLoginSuccess"></param>
        /// <param name="isAdminLogin"></param>
        /// <param name="isSmsLogin">是否短信快捷登录</param>
        /// <returns></returns>
        private LoginSuccessData SiteLogin(string longinName, string pwd, ref string msg, ref bool isLoginSuccess, bool isAdminLogin = false, bool isRemberMe=false)
        {
            LoginSuccessData successData = null;
            UserBase modelAccount = null;
            modelAccount  = _userRepository.GetAll().FirstOrDefault(a => a.userLoginName == longinName || a.userMobile == longinName || a.userEmail == longinName);
            if (isAdminLogin)
            {
                if (modelAccount == null)
                {
                    msg = "该账号不存在，请重新输入！";
                    isLoginSuccess = false;

                }
                else if (modelAccount.userPassWord == sha1Encrypt.getSHA1Value(pwd ?? ""))
                {
                    if (modelAccount.userEnbleFlag)
                    {
                        msg = "账号被禁用，请联系管理员！";
                        isLoginSuccess = false;
                    }

                    else if (modelAccount.identity == 2 || modelAccount.identity == 3)
                    {
                        successData = new LoginSuccessData()
                        { //获取登录的用户信息
                            email = modelAccount.userEmail,
                            fullName = modelAccount.userFullName,
                            gender = modelAccount.userGender,
                            loginName = modelAccount.userLoginName,
                            phoneNumber = modelAccount.userMobile,
                            idCard = modelAccount.userIdcard
                        };
                        isLoginSuccess = true;
                        successData.identity = modelAccount.identity;
                        successData.userId = modelAccount.Id.ToString();
                        successData.isCompleted = modelAccount.isCompleted;
                        successData.UserHeadImg = modelAccount.smallAvatar;
                        successData.birthday = modelAccount.userBirthday.ToString();
                    }
                    else
                    {
                        msg = "该账号不存在，请重新输入！";
                        isLoginSuccess = false;
                        successData = null;
                    }

                }
                else
                {
                    msg = "密码验证不通过！";
                    isLoginSuccess = false;
                    successData = null;
                }



            }
            else
            {
                if (modelAccount == null)
                {
                    msg = "该账号不存在，请重新输入！";
                    isLoginSuccess = false;
                    successData = null;
                }
                else if (modelAccount != null && modelAccount.userEnbleFlag)
                {
                    msg = "账号被禁用，请联系管理员！";
                    isLoginSuccess = false;
                    successData = null;
                }
                else if (modelAccount.userPassWord == sha1Encrypt.getSHA1Value(pwd ?? ""))
                {
                    successData = new LoginSuccessData()
                    { //获取登录的用户信息
                        email = modelAccount.userEmail,
                        fullName = modelAccount.userFullName,
                        gender = modelAccount.userGender,
                        loginName = modelAccount.userLoginName,
                        phoneNumber = modelAccount.userMobile,
                        idCard = modelAccount.userIdcard
                    };
                    msg = "登录成功！";
                    successData.identity = modelAccount.identity;
                    successData.userId = modelAccount.Id.ToString();
                    successData.isCompleted = modelAccount.isCompleted;
                    successData.UserHeadImg = modelAccount.smallAvatar;
                    successData.birthday = modelAccount.userBirthday.ToString();
                    isLoginSuccess = true;
                }
                else
                {
                    msg = "密码验证不通过！";
                    isLoginSuccess = false;
                    successData = null;
                }

            }
            if (isLoginSuccess)
            {
                var loginUserSession = new UserCookie
                {
                    Id = modelAccount.Id,
                    UserName = modelAccount.userFullName,
                    UserUid = modelAccount.Id.ToString(),
                    LoginName = modelAccount.userLoginName,
                    PassWord = modelAccount.userPassWord,
                    IsAdmin = modelAccount.identity == 3,
                    UserHeadImg = modelAccount.smallAvatar,
                    Identity = modelAccount.identity,
                    UserEnbleFlag = modelAccount.userEnbleFlag,
                    Birthday = modelAccount.userBirthday.ToString(),
                    PhoneNumber = modelAccount.userMobile,
                    Gender = modelAccount.userGender,
                    LoginIpAddress = modelAccount.loginIp,
                    SessionId = modelAccount.sessionId,
                    IsCompleted = modelAccount.isCompleted,
                    IsRemberMe = isRemberMe,
                    IsLogin = true
                };
                loginUserSession.IsSuperAdmin = IsSuperAdmin(modelAccount.Id);
                CookieHelper.SetLoginInUserCookie(loginUserSession, isAdminLogin, isRemberMe);

            }
            return isLoginSuccess ? successData : null;
        }

        /// <summary>
        /// 登录之后的处理
        /// </summary>
        /// <param name="apiRes">新课网返回结果</param>
        /// <param name="modelAccount">账户信息</param>
        /// <param name="loginName">登录名</param>
        /// <param name="password">密码</param>
        /// <param name="ipAddress">ip地址</param>
        /// <param name="isLoginSuccess">是否登录成功</param>
        /// <param name="isAdminLogin">是否为后台登录</param>
        /// <param name="isMobileLogin">是否为移动端登录</param>
        /// <returns></returns>
        private async Task LoginManger(ApiResponseResult<ApiUserInfoDto> apiRes, Guid newUserid, UserBase modelAccount, string loginName, string password, string ipAddress, bool isLoginSuccess, bool isAdminLogin = false, bool isMobileLogin = false, bool isRemberMe = false)
        {
            try
            {
                //  UserBase modelAccount = null;
                if (apiRes == null)
                {
                    return;
                }
                if (apiRes.IsSuccess)
                {
                    if (modelAccount == null)//当该用户不存在的时候
                    {
                        UserBase user = new UserBase();
                        user.Id = Guid.NewGuid();
                        user.userLoginName = apiRes.Data.InKey.loginname;
                        user.userMobile = apiRes.Data.InKey.mobile;
                        user.userEmail = apiRes.Data.InKey.email;
                        user.userIdcard = apiRes.Data.InKey.idcard;
                        user.userGender = apiRes.Data.InKey.gender;
                        user.userPassWord = sha1Encrypt.getSHA1Value(password ?? "");
                        user.smallAvatar = UserInfoImg.GetDefaultUserAvator(user.userGender ?? "");
                        user.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey.id) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey.id);
                        user.identity = 1;
                        user.isCompleted = false;
                        user.userEnbleFlag = false;

                        StudentInfo student = new StudentInfo();
                        student.Id = Guid.NewGuid();
                        student.isDel = false;
                        student.createTime = DateTime.Now;
                        student.updateTime = DateTime.Now;
                        student.userId = user.Id;
                        student.userEnbleFlag = 0;

                        _userRepository.Insert(user);
                        _studentRepository.Insert(student);
                        //   _unitOfWorkManager.Current.SaveChangesAsync();
                        _unitOfWorkManager.Current.SaveChanges();

                        modelAccount = _userRepository.Get(user.Id);
                    }
                    else
                    {//当存在该用户的时候
                        modelAccount.userLoginName = apiRes.Data.InKey.loginname;
                        modelAccount.userIdcard = apiRes.Data.InKey.idcard;
                        modelAccount.userGender = apiRes.Data.InKey.gender;
                        modelAccount.userEmail = apiRes.Data.InKey.email;
                        modelAccount.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey.id) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey.id);
                        //modelAccount.userFullName = apiRes.Data.InKey.loginname;
                        //   _unitOfWorkManager.Current.SaveChangesAsync();
                        _userRepository.Update(modelAccount);
                        _unitOfWorkManager.Current.SaveChanges();
                    }
                    if (isLoginSuccess)
                    {
                        modelAccount.loginIp = ipAddress;
                        modelAccount.sessionId = isMobileLogin ? "mobile" : System.Web.HttpContext.Current.Session.SessionID;
                        modelAccount.loginTime = DateTime.Now;
                        _userRepository.Update(modelAccount);
                        await _unitOfWorkManager.Current.SaveChangesAsync();

                        var loginUserSession = new UserCookie
                        {
                            Id = modelAccount.Id,
                            UserName = modelAccount.userFullName,
                            UserUid = modelAccount.Id.ToString(),
                            LoginName = modelAccount.userLoginName,
                            PassWord = modelAccount.userPassWord,
                            IsAdmin = modelAccount.identity == 3,
                            UserHeadImg = modelAccount.smallAvatar,
                            Identity = modelAccount.identity,
                            UserEnbleFlag = modelAccount.userEnbleFlag,
                            Birthday = modelAccount.userBirthday.ToString(),
                            PhoneNumber = modelAccount.userMobile,
                            Gender = modelAccount.userGender,
                            LoginIpAddress = modelAccount.loginIp,
                            SessionId = modelAccount.sessionId,
                            IsCompleted = modelAccount.isCompleted,
                            IsRemberMe = isRemberMe,
                            IsLogin = true
                        };
                        if (!isMobileLogin)
                        {
                            loginUserSession.IsSuperAdmin = IsSuperAdmin(modelAccount.Id);
                            CookieHelper.SetLoginInUserCookie(loginUserSession, isAdminLogin, isRemberMe);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString);
            }

        }


        #endregion

        #region 根据用户名查询用户
        public UserBase GetUserOtherWay(string loginName, string pwd, bool isCheckPwd = true)
        {
            UserBase modelAccount = null;
            Regex phoneRex = new Regex(@"^[1]+[3,5,8,7]+\d{9}");
            Regex emailRex = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            if (phoneRex.IsMatch(loginName))
            {
                modelAccount = OtherWayLogin(loginName, pwd, userLoginType.userMobel, isCheckPwd);
            }
            else if (emailRex.IsMatch(loginName))
            {
                modelAccount = OtherWayLogin(loginName, pwd, userLoginType.userEmail, isCheckPwd);
            }
            else
            {
                modelAccount = OtherWayLogin(loginName, pwd, userLoginType.userName, isCheckPwd);
            }
            return modelAccount;
        }


        public UserBase OtherWayLogin(string name, string password, userLoginType key, bool isCheckPwd = true)
        {
            // var keys = key.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


            Expression<Func<UserBase, bool>> exp = a => false;
            switch (key)
            {
                case userLoginType.userName:
                    if (isCheckPwd)
                    {
                        exp = p => p.userLoginName == name && p.userPassWord == password;
                    }
                    else
                    {
                        exp = p => p.userLoginName == name;
                    }
                    break;
                case userLoginType.userIdCard:
                    if (isCheckPwd)
                    {
                        exp = p => p.userIdcard == name && p.userPassWord == password;
                    }
                    else
                    {
                        exp = p => p.userIdcard == name;
                    }
                    break;
                case userLoginType.userEmail:
                    if (isCheckPwd)
                    {
                        exp = p => p.userEmail == name && p.userPassWord == password;
                    }
                    else
                    {
                        exp = p => p.userEmail == name;
                    }

                    break;
                case userLoginType.userMobel:
                    if (isCheckPwd)
                    {
                        exp = p => p.userMobile == name && p.userPassWord == password;
                    }
                    else
                    {
                        exp = p => p.userMobile == name;
                    }

                    break;
            }
            return _userRepository.GetAll().FirstOrDefault(exp);
        }

        public UserBase MultiWayLogin(string name, string password, string key)
        {
            var keys = key.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var exps = new Expression<Func<UserBase, bool>>[keys.Length];
            for (var i = 0; i < keys.Length; i++)
            {
                switch (keys[i])
                {
                    case "userLoginName":
                        Expression<Func<UserBase, bool>> userName = p => p.userLoginName == name && p.userPassWord == password;
                        exps[i] = userName;
                        break;
                    //case "user_code":
                    //    Expression<Func<UserBase, bool>> userCode = p => p.user_code == name && p.userPassWord == password;
                    //    exps[i] = userCode;
                    //    break;
                    case "userIdcard":
                        Expression<Func<UserBase, bool>> idCard = p => p.userIdcard == name && p.userPassWord == password;
                        exps[i] = idCard;
                        break;
                    case "userEmail":
                        Expression<Func<UserBase, bool>> email = p => p.userEmail == name && p.userPassWord == password;
                        exps[i] = email;
                        break;
                    case "userMobile":
                        Expression<Func<UserBase, bool>> phone = p => p.userMobile == name && p.userPassWord == password;
                        exps[i] = phone;
                        break;
                    //case "admission_number":
                    //    Expression<Func<UserBase, bool>> admission = p => p.admission_number == name && p.userPassWord == password;
                    //    exps[i] = admission;
                    //    break;
                }
            }
            return exps.Select(exp => _userRepository.GetAll().FirstOrDefault(exp)).FirstOrDefault(data => data != null);
        }

        #endregion

        #region 用户注册+UserRegister(UserRegisterModel model,ref string msg)
        public UserBase UserRegister(UserRegisterModel model, ref string msg, bool isMobileRegister = false, string mobileIP = "")
        {
            try
            {
                //用户不填手机号注册的统一以00000000000去新课网注册
                model.RegisterMobile = string.IsNullOrEmpty(model.RegisterMobile) ? "00000000000" : model.RegisterMobile;
                UserBase user = new UserBase();
                user.Id = Guid.NewGuid();
                user.userLoginName = string.IsNullOrEmpty(model.UserName) ? model.RegisterMobile : model.UserName;
                user.userMobile =model.RegisterMobile;
                user.userEmail = model.RegisterEmail;
                user.userFullName = model.UserFullName;
                user.userPassWord = sha1Encrypt.getSHA1Value(model.PassWord ?? "");
                user.smallAvatar = UserInfoImg.GetDefaultUserAvator(user.userGender ?? "");
                user.identity = 1;
                user.userEnbleFlag = false;
                user.loginIp = isMobileRegister ? (mobileIP ?? "mobile") : System.Web.HttpContext.Current.Request.UserHostAddress;
                user.sessionId = isMobileRegister ? "mobile" : System.Web.HttpContext.Current.Session.SessionID;
                user.isCompleted = false;
                user.approvalStatus = "approving"; //学生需要审核
                var apiRes = _userInfoApiService.Register(null, model).Result;

                if (!apiRes.IsSuccess)
                {

                    if (apiRes.ErrMsg.Trim() == "UserExist")//当该学生在新课网中存在的时候，将该用户信息更新到新课网。
                    {
                        if (!this.CheckMobileExist(model.RegisterMobile, "insert", "", true))
                        {
                            msg = model.RegisterType == "student" ? "当前手机号码已在其他平台注册，请用当前手机号码直接登录！" : "手机号码已在其他平台注册，请重新输入！";
                            return null;
                        }
                        if (!this.CheckNameExit(model.UserName, "insert"))
                        {
                            msg = model.RegisterType == "student" ? "当前用户名已在其他平台注册，请用当前用户名直接登录！" : "用户名已在其他平台注册，请重新输入！";
                            return null;
                        }
                        if (!this.CheckEmailExist(model.RegisterEmail, "insert"))
                        {
                            msg = model.RegisterType == "student" ? "当前邮箱已在其他平台注册，请用当前邮箱直接登录！" : "邮箱已在其他平台注册，请重新输入！";
                            return null;
                        }
                        /* EditDto editDto = user.ExMapTo<UserInfoDto>().GetEditDto();
                         editDto.newpw = string.IsNullOrWhiteSpace(model.PassWord) ? "" : sha1Encrypt.AirEncode(model.PassWord);
                         var res = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
                         if (!res.IsSuccess)
                         {
                             msg = res.ErrMsg ?? "注册失败";
                             return null;
                         }
                         else
                         {
                             user.newMoocUserId = res.Data.InKey.ToString();
                         }*/

                    }
                    else
                    {
                        msg = apiRes.ErrMsg;
                        return null;
                    }
                }
                else
                {
                    user.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey);//如果新课网新增用户成功，讲新课网用户Id保存到本地
                }
                if (model.RegisterType == "admin")
                {
                    user.approvalStatus = "approved";
                    user.identity = 3;
                    AdminInfo admin = new AdminInfo();
                    admin.Id = Guid.NewGuid();
                    admin.isDel = false;
                    admin.createTime = DateTime.Now;
                    admin.updateTime = DateTime.Now;
                    admin.userId = user.Id;
                    admin.isAdmin = true; //后台注册的管理员为超级管理员
                    _userRepository.Insert(user);
                    _adminRepository.Insert(admin);
                    _unitOfWorkManager.Current.SaveChanges();
                    return user;
                }
                var classId = model.ClassId.TryParseGuid();
                StudentInfo student = new StudentInfo();
                student.Id = Guid.NewGuid();
                student.isDel = false;
                student.createTime = DateTime.Now;
                student.updateTime = DateTime.Now;
                student.userId = user.Id;
                student.userEnbleFlag = 0;
                _userRepository.Insert(user);
                _studentRepository.Insert(student);
                 //写入教学班级
                if (!string.IsNullOrWhiteSpace(model.ClassId))
                {
                    _classStudentRepository.Insert(new ClassStudent
                    {
                        Id = Guid.NewGuid(),
                        ClassId = classId,
                        UserId = user.Id,
                        CreateTime = DateTime.Now
                    });
                }
                _unitOfWorkManager.Current.SaveChanges();


                return user;
            }
            catch (Exception ex)
            {
                Logger.Error("用户注册：" + ex.ToString());
                return null;
            }

        }

        #endregion

        #region 检查信息是否存在
        public bool MobileExistCheck(string RegisterMobile)
        {
            RegisterMobile = RegisterMobile.Trim();
            bool b = false;
            var res = _userInfoApiService.CheckMobile(null, new UserRegisterModel() { RegisterMobile = RegisterMobile });
            if (res.Result.IsSuccess)
            {
                b = !_userRepository.GetAll().Any(a => a.userMobile == RegisterMobile);
            }
            return b;
        }

        /// <summary>
        /// 检验用户名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">验证类型，新增或更改</param>
        /// <returns></returns>
        public bool CheckNameExit(string name, string type, string oldname = "", bool isRemoteCheck = true)
        {
            name = name.Trim();
            if (type == "update" && name == oldname)
            {
                return true;
            }
            if (isRemoteCheck)
            {
                var res = _userInfoApiService.CheckUserName(null, new UserRegisterModel() { UserName = name }).Result;
                if (!res.IsSuccess)
                {
                    return false;
                }
            }

            return !_userRepository.GetAll().Any(s => s.userLoginName == name);

        }

        /// <summary>
        /// 检验手机是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">验证类型，新增或更改</param>
        /// <returns></returns>
        public bool CheckMobileExist(string mobile, string type, string oldMobile = "", bool isRemoteCheck = false)
        {
            mobile = mobile.Trim();
            if (type == "update" && mobile == oldMobile)
            {
                return true;
            }
            if (isRemoteCheck)
            {
                var responseRes = _userInfoApiService.CheckMobile(null, new UserRegisterModel() { RegisterMobile = mobile });
                var res = responseRes.Result;
                if (!res.IsSuccess)
                {
                    return false;
                }
                //--去掉新课网验证
            }
            return !_userRepository.GetAll().Any(s => s.userMobile == mobile);
        }


        /// <summary>
        /// 检验邮箱是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">验证类型，新增或更改</param>
        /// <returns></returns>
        public bool CheckEmailExist(string email, string type, string oldEmail = "", bool isRemoteCheck = true)
        {
            email = email.Trim();
            if (type == "update" && email == oldEmail)
            {
                return true;
            }
            if (isRemoteCheck)
            {
                var res = _userInfoApiService.CheckEmail(null, new UserRegisterModel() { RegisterEmail = email }).Result;
                if (!res.IsSuccess)
                {
                    return false;
                }
            }
            return !_userRepository.GetAll().Any(s => s.userEmail == email);
        }


        /// <summary>
        /// 检验身份证号码是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">验证类型，新增或更改</param>
        /// <returns></returns>
        public bool CheckIdCardExist(string idCard, string type, string oldIdCard = "")
        {
            if (type == "update" && idCard == oldIdCard)
            {
                return true;
            }
            return !_userRepository.GetAll().Any(s => s.userIdcard == idCard);
        }
        #endregion

        #region 用户名更改+UserNameModify(UserInfoMogifyInputDto model,ref string msg)
        public bool UserNameModify(UserInfoMogifyInputDto model, ref string msg)
        {

            UserBase user = _userRepository.Get(model.Id);
            if (user.userLoginName == model.UserName)
            {
                return true;
            }
            if (!this.CheckNameExit(model.UserName, "Insert"))
            {
                msg = "用户名已存在！";
                return false;
            }
            user.userLoginName = model.UserName;
            EditDto editDto = user.MapTo<UserInfoDto>().GetEditDto();
            editDto.username = model.UserName;
            var apiRes = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
            if (!apiRes.IsSuccess)
            {
                msg = apiRes.ErrMsg;
                return false;
            }
            return UpdateUser(user);

        }
        #endregion

        #region 密码修改+PwdModifry(ModifyPassWord model)
        /// <summary>
        /// 用户安全设置-密码修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ReturnMsg PwdModifry(ModifyPassWord model)
        {
            try
            {
                if (model.userId == Guid.Empty || string.IsNullOrEmpty(model.newPassWord) || string.IsNullOrEmpty(model.oldPassWord) || string.IsNullOrEmpty(model.confirmPwd))
                {
                    return new ReturnMsg { MsgCode = "error", MsgContent = "当前信息不完整" };
                }

                if (model.confirmPwd != model.newPassWord)
                {
                    return new ReturnMsg { MsgCode = "error", MsgContent = "两次输入的密码不一致！" };
                }
                model.userId = model.userId;
                UserBase uBase = this.GetUserBaseByExpre(a => a.Id == model.userId).FirstOrDefault();

                if (uBase.userPassWord != sha1Encrypt.getSHA1Value(model.oldPassWord ?? ""))
                {
                    return new ReturnMsg { MsgCode = "error", MsgContent = "原密码错误" };
                }
                if (uBase.userPassWord == sha1Encrypt.getSHA1Value(model.newPassWord ?? ""))
                {
                    return new ReturnMsg { MsgCode = "error", MsgContent = "新密码不能与原密码相同" };
                }
                model.userMobile = uBase.userMobile;
                model.userName = uBase.userLoginName;
                var b = this.PwdModifyByForget(model, true).Result;
                if (b)
                {
                    return new ReturnMsg { MsgCode = "ok", MsgContent = "修改成功" };
                }
                else
                {
                    return new ReturnMsg { MsgCode = "error", MsgContent = "修改失败" };
                }
            }
            catch (Exception ex)
            {
                return new ReturnMsg { MsgCode = "error", MsgContent = "修改失败" };

            }
        }

        #endregion

        #region 移动端密码修改+UserPwdModifyByApi(ModifyPassWord model, ref string msg)
        public bool UserPwdModifyByApi(ModifyPassWord model, ref string msg)
        {
            UserBase user = _userRepository.GetAll().FirstOrDefault(a => a.userMobile == model.userMobile);
            if (user == null)
            {
                msg = "用户不存在";
                return false;
            }
            EditDto editDto = new EditDto();
            editDto.mobile = model.userMobile;
            editDto.gender = int.Parse(string.IsNullOrEmpty(user.userGender) ? "0" : user.userGender);
            editDto.newpw = sha1Encrypt.AirEncode(model.newPassWord ?? "");
            editDto.Id = user.Id;
            editDto.ignoreoldpw = 0;
            editDto.questionid = 0;

            var apiRes = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
            if (!apiRes.IsSuccess)
            {
                msg = apiRes.ErrMsg ?? "修改密码失败";
                return false;
            }
            user.userPassWord = sha1Encrypt.getSHA1Value(model.newPassWord);
            bool b = UpdateUser(user);
            if (!b)
            {
                msg = "修改密码失败";
                return false;
            }
            return true;
        }
        #endregion

        #region 后台用户密码修改+UserPassWordModify(ModifyPassWord model)
        /// <summary>
        /// 密码修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ReturnMsg> UserPassWordModify(ModifyPassWord model)
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var cookie = CookieHelper.GetLoginInUserInfo();
            if(cookie.Identity==1)
                throw new UserFriendlyException("权限不足");
            var b = true;
            UserBase user = _userRepository.Get(model.userId);
            EditDto editDto = new EditDto();
            editDto.oldusername = user.userLoginName;
            //editDto.truename = user.userFullName;
            editDto.mobile = model.userMobile;
            editDto.gender = int.Parse(string.IsNullOrEmpty(user.userGender) ? "0" : user.userGender);
            editDto.newpw = sha1Encrypt.AirEncode(model.newPassWord ?? "");
            editDto.Id = model.userId;
            editDto.email = user.userEmail;
            editDto.ignoreoldpw = 0; //1表示需要密码校验
            editDto.questionid = 0;
            editDto.oldpw= sha1Encrypt.AirEncode(model.oldPassWord ?? "");
            var apiRes = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
            if (!apiRes.IsSuccess)
            {
                return new ReturnMsg {MsgContent= apiRes.ErrMsg,MsgCode = "-1"};
            }
            user.userPassWord = sha1Encrypt.getSHA1Value(model.newPassWord);
            b = UpdateUser(user);
            if (!b)
            {
                return new ReturnMsg { MsgContent = "修改密码失败", MsgCode = "-1" };

               
            }
            return new ReturnMsg { MsgCode = "1" };
        }
        #endregion

        #region 用户设置邮箱修改+EmailModifySetting(UserEmailModifyView model, bool ignoreoldpw, ref string msg)
        public bool EmailModifySetting(UserEmailModifyView model, bool ignoreoldpw, ref string msg)
        {
            string newEmail = model.newEmail;
            if (!_userRepository.GetAll().Any(a => a.userLoginName == model.userName))
            {
                msg = "当前用户不存在！";
                return false;
            }
            UserBase user = _userRepository.GetAll().FirstOrDefault(a => a.userLoginName == model.userName);
  
            //if (user.userEmail == newEmail)
            //{
            //    msg = "新邮箱不能和当前邮箱相同！";
            //    return false;
            //}
            if (!this.CheckEmailExist(newEmail, "insert"))
            {
                msg = "新邮箱已存在！";
                return false;
            }

            EditDto editDto = new EditDto();
            editDto.oldusername = model.userName;
            editDto.email = newEmail;
            editDto.mobile = user.userMobile;
            editDto.gender = int.Parse(string.IsNullOrEmpty(user.userGender) ? "0" : user.userGender);
            editDto.Id = user.Id;
            editDto.ignoreoldpw = ignoreoldpw ? 0 : 1;
            editDto.questionid = 0;

            var apiRes = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
            if (!apiRes.IsSuccess)
            {
                msg = apiRes.ErrMsg;
                return false;

            }
            user.userEmail = newEmail;
            if (!UpdateUser(user))
            {
                msg = "邮箱修改失败！";
                return false;
            }

            return true;
        }

        #endregion

        #region 忘记密码-密码修改+ PwdModifyByForget(ModifyPassWord model,bool ignoreoldpw)
        public async Task<bool> PwdModifyByForget(ModifyPassWord model, bool ignoreoldpw)
        {

            UserBase user = _userRepository.Get(model.userId);
            EditDto editDto = new EditDto();
            if (!ignoreoldpw)
            {
                editDto.oldpw = sha1Encrypt.AirEncode(model.oldPassWord ?? "");
            }
            //   editDto.username = model.userName;
            editDto.mobile = model.userMobile;
            editDto.gender = int.Parse(string.IsNullOrEmpty(user.userGender) ? "0" : user.userGender);
            editDto.newpw = sha1Encrypt.AirEncode(model.newPassWord ?? "");
            editDto.Id = model.userId;
            editDto.ignoreoldpw = ignoreoldpw ? 0:1;
            editDto.questionid = 0;
            editDto.oldusername = model.userName;
            var apiRes = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
            if (!apiRes.IsSuccess)
            {
                return false;

            }
            user.userPassWord = sha1Encrypt.getSHA1Value(model.newPassWord);
            var b = UpdateUser(user);
            return b;
        }
        #endregion

        #region 得到用户+GetUserBaseByQueryDto(UserInfoQueryInputDto model)
        public UserBase GetUserBaseByQueryDto(UserInfoQueryInputDto model)
        {
            if (model.id != Guid.Empty)
            {
                if (_studentRepository.GetAll().Any(a => a.Id == model.id))
                {
                    return _userRepository.Get(_studentRepository.Get(model.id).userId);
                }

                if (_teacherRepository.GetAll().Any(a => a.Id == model.id))
                {
                    return _userRepository.Get(_teacherRepository.Get(model.id).userId);
                }
                if (_adminRepository.GetAll().Any(a => a.Id == model.id))
                {
                    return _userRepository.Get(_adminRepository.Get(model.id).userId);
                }
            }
            return _userRepository.GetAll().WhereIf(model.userId != Guid.Empty, a => a.Id == model.userId)
                .WhereIf(!string.IsNullOrEmpty(model.userName), a => a.userLoginName == model.userName).FirstOrDefault();
        }
        #endregion

        #region 自定义查询获取用户信息+GetUserBaseByExpre(Expression<Func<UserBase, bool>> expre)
        public List<UserBase> GetUserBaseByExpre(Expression<Func<UserBase, bool>> expre)
        {
            return _userRepository.GetAllList(expre);
        }
        #endregion

        #region 根据用户名获取用户信息+GetUserBaseByUserName
        public UserBase GetUserBaseByUserName(string userName)
        {

            return _userRepository.GetAll().FirstOrDefault(a => a.userLoginName == userName);
        }
        #endregion

        #region 根据邮箱获取用户列表+List<UserBase> GetUserBaseByEmail(string email)
        public List<UserBase> GetUserBaseByEmail(string email)
        {
            return _userRepository.GetAllList(a => a.userEmail == email);
        }
        #endregion

        #region 根据手机号获取用户信息+ List<UserBase> GetUserBaseByPhone
        public List<UserBase> GetUserBaseByPhone(string phoneNumber)
        {
            return _userRepository.GetAllList(a => a.userMobile == phoneNumber);
        }
        #endregion

        #region 根据id获取用户信息+UserBase GetUserBaseByPhoneById(Guid id)
        public UserBase GetUserBaseByPhoneById(Guid id)
        {

            return _userRepository.FirstOrDefault(id);
        }
        #endregion

        #region 根据id获取用户DTO+UserInfoDto GetUserInfoInputDtoById(Guid id)
        public UserInfoDto GetUserInfoInputDtoById(Guid id)
        {
            var res = _userRepository.FirstOrDefault(id);
            return res.MapTo<UserInfoDto>() ?? new UserInfoDto();
        }
        #endregion

        #region 根据ID获取UserDetailShow+UserDetailShow GetUserDetailShowDtoById(Guid id)
        public UserDetailShow GetUserDetailShowDtoById(Guid id)
        {
            var user = _userRepository.Get(id);
            var userDto = user.MapTo<UserInfoDto>() ?? new UserInfoDto();
            var res = userDto.GetUserDetailShowModel();
          
            return res;
        }
        #endregion

        

        #region 根据身份证号码获取用户列表+List<UserBase> GetUserBaseByIdCard(string idCard)
        public List<UserBase> GetUserBaseByIdCard(string idCard)
        {
            return _userRepository.GetAllList(a => a.userIdcard == idCard);
        }
        #endregion

        #region 更新用户+UpdateUser(UserBase user)
        public bool UpdateUser(UserBase user)
        {
            try
            {
                _userRepository.Update(user);
                _unitOfWorkManager.Current.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        #endregion

        #region 根据DTO更新用户+bool UpdateUserByUserInfoInputDto(UserInfoInputDto user,List<string>perLiset,ref string msg)
        public bool UpdateUserByUserInfoInputDto(UserInfoInputDto user, List<string> perLiset, ref string msg)
        {
            try
            {

                UserBase userBase = _userRepository.Get(user.Id);
                if (string.IsNullOrEmpty(userBase.userEmail))
                {
                    if (!string.IsNullOrEmpty(user.userEmail) && !this.CheckEmailExist(user.userEmail, "insert"))
                    {
                        msg = "邮箱已存在！";
                        return false;
                    }
                    else
                    {
                        perLiset.Add("userEmail");
                    }
                }
                if (!string.IsNullOrEmpty(user.userIdcard) && userBase.userIdcard != user.userIdcard && _userRepository.GetAll().Any(a => a.userIdcard == user.userIdcard))
                {
                    msg = "身份证号码已存在！";
                    return false;
                }
              
                EditDto editDto = userBase.MapTo<UserInfoDto>().GetEditDto();
                editDto.username = user.userLoginName;
                editDto.oldusername = userBase.userLoginName;
                var apiRes = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
                if (!apiRes.IsSuccess)
                {
                    msg = apiRes.ErrMsg ?? "修改失败";
                    return false;
                }

                ObjHelper.ObjSetValueByPerList<UserBase, UserInfoInputDto>(ref userBase, ref user, perLiset);
                 userBase.isCompleted = true;
                _userRepository.Update(userBase);
                //修改教师信息表
                if (!string.IsNullOrEmpty(user.teacherPersonalResume))
                {
                    var techerInfo = _teacherRepository.Get(user.teacherId);
                    techerInfo.teacherPersonalResume = user.teacherPersonalResume;
                    techerInfo.updateTime = DateTime.Now;
                    _teacherRepository.Update(techerInfo);
                }
                _unitOfWorkManager.Current.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                throw ex;
                //   return false;
            }

        }
        #endregion

        #region 移动端更改用户+bool UpdateUserByApi(UserInfoInputDto model, List<string> perLiset, ref string msg)
        public bool UpdateUserByApi(UserInfoInputDto model, List<string> perLiset, ref string msg)
        {
            if (string.IsNullOrEmpty(model.userLoginName) || string.IsNullOrEmpty(model.userMobile))
            {
                msg = "信息不完整";
                return false;
            }
            var user = _userRepository.Get(model.Id);
            if (user == null)
            {
                msg = "用户不存在";
                return false;
            }
            if (!string.IsNullOrEmpty(model.userMobile))
            {
                if (!this.CheckMobileExist(model.userMobile, "update", user.userMobile))
                {
                    msg = "手机号码已存在";
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(model.userLoginName))
            {
                if (!this.CheckNameExit(model.userLoginName, "update", user.userLoginName))
                {
                    msg = "用户名已存在";
                    return false;
                }
            }
            if (model.userMobile != user.userMobile)
            {
                var mobileExist = _userInfoApiService.CheckMobile(null, new UserRegisterModel() { RegisterMobile = model.userMobile }).Result.IsSuccess;
                if (mobileExist)
                {//当新课网中，新修改手机号不存在时，将该手机号更新到新课网
                    ModifyMobile modifry = new ModifyMobile() { newmobile = model.userMobile, oldmobile = user.userMobile, username = user.userLoginName };
                    var apiRes = _userInfoApiService.ModifryUserMobile(null, modifry).Result;
                    if (!apiRes.IsSuccess)
                    {
                        msg = "修改手机号失败！";
                        return false;
                    }
                }
            }
            EditDto editDto = new EditDto() { mobile = model.userMobile, username = model.userLoginName, gender = int.Parse(model.userGender ?? "0") };
            var userNameModifryApiRes = _userInfoApiService.ModifryUserByMobile(null, editDto).Result;
            if (!userNameModifryApiRes.IsSuccess)
            {
                msg = userNameModifryApiRes.ErrMsg ?? "修改失败！";
                return false;
            }
            //    List<string> perList = new List<string>() { "userLoginName", "userGender", "userBirthday", "userMobile"};
            ObjHelper.ObjSetValueByPerList<UserBase, UserInfoInputDto>(ref user, ref model, perLiset);
            _unitOfWorkManager.Current.SaveChangesAsync();
            return true;
        }
        #endregion

        #region 返回用户的信息
        /// <summary>
        /// 返回用户的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetUserBaseById(UserInfoInputDto input)
        {
            string userbase = string.Empty;
            try
            {
                UserBase userInfo = _userRepository.FirstOrDefault(d => d.Id == input.Id);

                if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.userLoginName))
                {
                    return userbase;
                }

                return JsonConvert.SerializeObject(userInfo);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return userbase;
        }
        #endregion

        #region 用户邮箱设置 发送邮件+ReturnMsg UserEmailSetting(UserEmailModifyView model, UserSession _user,string returnUrl)
        public ReturnMsg UserEmailSetting(UserEmailModifyView model, UserCookie _user, string returnUrl)
        {
            ReturnMsg returnMsg = null;

            if (_user != null && !string.IsNullOrEmpty(_user.UserUid))
            {
                var uBase = this.GetUserBaseByPhoneById(new Guid(_user.UserUid));

                if (string.IsNullOrEmpty(model.passWord) || string.IsNullOrEmpty(model.newEmail))
                {
                    returnMsg = new ReturnMsg() { MsgCode = "error", MsgContent = "信息不完整，请重新输入" };
                }
                else
                {
                    if (uBase.userPassWord != sha1Encrypt.getSHA1Value(model.passWord ?? ""))
                    {
                        returnMsg = new ReturnMsg { MsgCode = "error", MsgContent = "网站登录密码错误" };
                    }
                    else if (uBase.userEmail == model.newEmail)
                    {
                        returnMsg = new ReturnMsg { MsgCode = "error", MsgContent = "新邮箱不能与当前邮箱相同！" };
                    }
                    else if (!this.CheckEmailExist(model.newEmail, "insert"))
                    {
                        returnMsg = new ReturnMsg { MsgCode = "error", MsgContent = "新邮箱已存在，请重新设置！" };
                    }
                    else
                    {
                        SettingSendEmailAddress();//配置邮件发送相关信息
                        string userName = uBase.userLoginName;
                        string email = model.newEmail;
                        string getstringtime = SymmetricCryptoMethod.EncodeBase64(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        // string WebUrl = subsiteRep.GetSiteWebUrl();
                        string WebUrl = "http://" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
                        string Key = SymmetricCryptoMethod.EncodeBase64(userName + "|" + email);

                        // WebUrl = WebUrl + "/Account/UpdatePwd/?Key=" + Key + "&getstring=" + getstringtime;
                        WebUrl = WebUrl + returnUrl + "?Key=" + Key + "&getstring=" + getstringtime;
                        string Content = "<div>";//内容
                        Content = Content + "<p style='margin-top:20px;'>更改您在SPOC设置的邮箱</p>";
                        Content = Content + "<p style='margin-top:30px;'>&nbsp;&nbsp;&nbsp;&nbsp;您的用户名为:<span style=' color:#F3750F; font-weight:bold; font-size:16px;'>" + userName + "</span></p>";
                        Content = Content + "<p style='margin-top:30px;'>&nbsp;&nbsp;&nbsp;&nbsp;请马上点击以下链接更改您的邮箱:  <a href='" + WebUrl + "'>" + WebUrl + "</a>  </p><br/><br/>";
                        Content = Content + "&nbsp;&nbsp;&nbsp;&nbsp;注意:请您在收到邮件1小时内使用，否则该链接将会失效。<br/><br/>";
                        Content = Content + "&nbsp;&nbsp;&nbsp;&nbsp;我们将一如既往、热忱的为您服务！<br/><br/>";
                        Content = Content + "</div>";
                        try
                        {
                            bool isSuccess = MailSender.sendMail(model.newEmail, "更改邮箱" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Content);
                            if (!isSuccess)
                            {
                                return new ReturnMsg { MsgCode = "error", MsgContent = "发送邮件失败，请检查您设置的新邮箱是否有效或联系管理员查看邮箱配置！" };
                            }
                            else
                            {
                                return new ReturnMsg { MsgCode = "ok", MsgContent = "发送邮件成功，请前往您设置的邮箱地址，激活该邮箱！" };
                            }
                        }
                        catch (Exception ex)
                        {
                            return new ReturnMsg { MsgCode = "error", MsgContent = "发送邮件失败，请检查您设置的新邮箱是否有效或联系管理员查看邮箱配置！" };
                        }


                    }
                }
            }
            return returnMsg;
        }
        #endregion

        #region 用户忘记密码+async Task<string> userForgetPassWord(UserBase user,string urlSrc)
        public async Task<string> userForgetPassWord(string userNmae, string urlSrc)
        {
            try
            {
                //   var userBase = _userRepository.GetAll().Where(a => a.userLoginName == user.userLoginName).FirstOrDefault();
                if (string.IsNullOrEmpty(userNmae))
                {
                    return "请填写你需要找回密码的账号";
                }
                var userBase = GetUserOtherWay(userNmae, "", false);
                if (userBase == null)
                {
                    return "该账户不存在！";
                }
                if (string.IsNullOrEmpty(userBase.userEmail))
                {
                    return "该账户还未设置邮箱，请前往个人设置，设置您的邮箱！";
                }
                /*  if (userBase.userEmail != user.userEmail)
                  {
                      return "用户名和邮箱不一致！";
                  }*/

                SettingSendEmailAddress();//配置邮件发送相关信息
                string userName = userBase.userLoginName;
                string email = userBase.userEmail;
                string getstringtime = SymmetricCryptoMethod.EncodeBase64(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                // string WebUrl = subsiteRep.GetSiteWebUrl();
                string WebUrl = "http://" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
                string Key = SymmetricCryptoMethod.EncodeBase64(userName + "|" + email);

                // WebUrl = WebUrl + "/Account/UpdatePwd/?Key=" + Key + "&getstring=" + getstringtime;
                WebUrl = WebUrl + urlSrc + "?Key=" + Key + "&getstring=" + getstringtime;
                string Content = "<div>";//内容
                Content = Content + "<p style='margin-top:20px;'>找回你在SPOC的密码</p>";
                Content = Content + "<p style='margin-top:30px;'>&nbsp;&nbsp;&nbsp;&nbsp;您的用户名为:<span style=' color:#F3750F; font-weight:bold; font-size:16px;'>" + userBase.userLoginName + "</span></p>";
                Content = Content + "<p style='margin-top:30px;'>&nbsp;&nbsp;&nbsp;&nbsp;请马上点击以下链接找回您的密码:  <a href='" + WebUrl + "'>" + WebUrl + "</a>  </p><br/><br/>";
                Content = Content + "&nbsp;&nbsp;&nbsp;&nbsp;注意:请您在收到邮件1小时内使用，否则该链接将会失效。<br/><br/>";
                Content = Content + "&nbsp;&nbsp;&nbsp;&nbsp;我们将一如既往、热忱的为您服务！<br/><br/>";
                Content = Content + "</div>";
                bool isSuccess = MailSender.sendMail(userBase.userEmail, "找回密码" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Content);
                if (!isSuccess)
                {
                    return "发送邮件失败,请检查您的邮箱地址是否正确或联系管理员检查邮件配置！";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }
        #endregion

        #region 邮箱发送配置+void SettingSendEmailAddress()
        public void SettingSendEmailAddress()
        {



            List<SiteSet> EmailList = _siteSetRepository.GetAllList(a => a.settingGroup == "sys_mailer");


            if (EmailList != null && EmailList.Count > 0)
            {

                SiteSet mailer_host = EmailList.Where(a => a.settingKey == "mailer_host").ElementAt(0); //SMTP服务器地址
                if (mailer_host != null && !string.IsNullOrEmpty(mailer_host.settingValue))
                    MailSender.StrHost = mailer_host.settingValue;

                SiteSet mailer_username = EmailList.Where(a => a.settingKey == "mailer_username").ElementAt(0); //SMTP用户名
                if (mailer_username != null && !string.IsNullOrEmpty(mailer_username.settingValue))
                    MailSender.StrAccount = mailer_username.settingValue;

                SiteSet mailer_password = EmailList.Where(a => a.settingKey == "mailer_password").ElementAt(0); //SMTP密码
                if (mailer_password != null && !string.IsNullOrEmpty(mailer_password.settingValue))
                    MailSender.StrPwd = mailer_password.settingValue;


                SiteSet mailer_from = EmailList.Where(a => a.settingKey == "mailer_from").ElementAt(0); //发信人地址
                if (mailer_from != null && !string.IsNullOrEmpty(mailer_from.settingValue))
                    MailSender.StrFrom = mailer_from.settingValue;

                SiteSet mailer_name = EmailList.Where(a => a.settingKey == "mailer_name").ElementAt(0); //发件人名称
                if (mailer_name != null && !string.IsNullOrEmpty(mailer_name.settingValue))
                    MailSender.StrName = mailer_name.settingValue;

            }

        }
        #endregion

       

       
        #region GetCenterUser
        public CenterUser GetCenterUser(string userId)
        {
            CenterUser user = new CenterUser();
            try
            {
                UserBase uBase = _userRepository.FirstOrDefault(d => d.Id.ToString() == userId);
                if (uBase == null)
                {
                    return user;
                }
                user.UserId = uBase.userIdcard;
                user.UserImageUrl = uBase.smallAvatar;
                user.UserName = string.IsNullOrWhiteSpace(uBase.userFullName) == true ? uBase.userLoginName : uBase.userFullName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return user;
        }
        #endregion


       

       


        public string UserSwitch(string userName)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException(-1, "未登录系统或登录已经失效，请重新登录");
            }
            
            if (string.IsNullOrEmpty(userName))
            {

                return "用户名不能为空!";
            }
            else
            {
                var user = _userRepository.GetAll().FirstOrDefault(a => a.userLoginName == userName);
                if (user == null)
                {
                 
                    return "该用户不存在!";
                }
                else if (user.userEnbleFlag)
                {
                  
                    return "该用户已被禁用!";
                }
                else if (user.identity==1 && user.approvalStatus!= "approved")
                {

                    return "该学生账户未通过审核!";
                }
                else
                {
                    //不是超级管理员 只能切换到角色类型低于自己的用户
                    if (!cookie.IsSuperAdmin && cookie.Identity <= user.identity)
                    {
                        return "您无权切换到该用户!";
                    }
                    var model = new UserCookie
                    {
                        Id = user.Id,
                        UserName = user.userFullName,
                        UserUid = user.Id.ToString(),
                        LoginName = user.userLoginName,
                        PassWord = user.userPassWord,
                        IsAdmin = user.identity == 3,
                        UserHeadImg = user.smallAvatar,
                        Identity = user.identity,
                        UserEnbleFlag = user.userEnbleFlag,
                        Birthday = user.userBirthday.ToString(),
                        PhoneNumber = user.userMobile,
                        Gender = user.userGender,
                        LoginIpAddress = user.loginIp,
                        SessionId = user.sessionId,
                        IsCompleted = user.isCompleted,
                        IsLogin = true
                    };
                    model.IsSuperAdmin = IsSuperAdmin(model.Id);
                    CookieHelper.SetLoginInUserCookie(model);
                    if (user.identity == 3 || user.identity == 2)
                    {
                        return "admin";
                    }
                    else
                    {
                        return "stu";
                    }
                }
            }
        }

        public async Task RecoverInsertUsers(UserBase model)
        {
            try
            {
                _userRepository.Insert(model);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        #region 短信登录
     

        public LoginSuccessData SmsLoginRequset(string mobile, string smsCode, string ipAddress, ref string msg, bool isSiteLogin = false)
        {
            ApiResponseResult<ApiUserInfoDto> apiRes = null;
            UserBase modelAccount = null;
            LoginSuccessData successData = null;
            bool isLoginSuccess = false;
            try
            {
                var sessionMsg = HttpContext.Current.Session[mobile] == null ? "" : HttpContext.Current.Session[mobile].ToString();
                if (sessionMsg.ToLower() != smsCode.ToLower())
                {
                    msg = "验证码错误或已过期,请重新输入！";
                }
                else
                {
                    //清除短信验证码Session
                    HttpContext.Current.Session.Remove(mobile);
                    if (isSiteLogin)//本站登录
                    {
                        //查询本地
                        modelAccount = _userRepository.GetAll().Where(a => a.userMobile == mobile).FirstOrDefault();
                        if (modelAccount == null)
                        {
                            msg = "该手机号码在本站未注册，请注册后进行登录！";
                            successData = null;
                            isLoginSuccess = false;
                        }
                        else
                        {
                            //用户禁用
                            if (modelAccount.userEnbleFlag)
                            {
                                msg = "用户被禁用，请联系管理员！";
                                isLoginSuccess = false;
                                successData = null;
                            }
                            else
                            {
                                successData = new LoginSuccessData();
                                successData.identity = modelAccount.identity;
                                successData.userId = modelAccount.Id.ToString();
                                successData.isCompleted = modelAccount.isCompleted;
                                successData.UserHeadImg = modelAccount.smallAvatar;
                                successData.birthday = modelAccount.userBirthday.ToString();
                                isLoginSuccess = true;
                                var loginUserSession = new UserCookie
                                {
                                    Id = modelAccount.Id,
                                    UserName = modelAccount.userFullName,
                                    UserUid = modelAccount.Id.ToString(),
                                    LoginName = modelAccount.userLoginName,
                                    PassWord = modelAccount.userPassWord,
                                    IsAdmin = modelAccount.identity == 3,
                                    UserHeadImg = modelAccount.smallAvatar,
                                    Identity = modelAccount.identity,
                                    UserEnbleFlag = modelAccount.userEnbleFlag,
                                    Birthday = modelAccount.userBirthday.ToString(),
                                    PhoneNumber = modelAccount.userMobile,
                                    Gender = modelAccount.userGender,
                                    LoginIpAddress = modelAccount.loginIp,
                                    SessionId = modelAccount.sessionId,
                                    IsCompleted = modelAccount.isCompleted,
                                    IsLogin = true

                                };
                                loginUserSession.IsSuperAdmin = IsSuperAdmin(modelAccount.Id);
                                CookieHelper.SetLoginInUserCookie(loginUserSession, false, false);
                            }
                        }
                    }

                    else
                    {
                        //根据手机号查询
                        apiRes = _userInfoApiService.GetUserInfo(null, mobile).Result;
                        if (apiRes.IsSuccess)
                        {
                            successData = new LoginSuccessData()
                            { //获取登录的用户信息
                                email = apiRes.Data.InKey.email,
                                fullName = apiRes.Data.InKey.truename,
                                gender = apiRes.Data.InKey.gender,
                                loginName = apiRes.Data.InKey.loginname,
                                phoneNumber = apiRes.Data.InKey.mobile,
                                idCard = apiRes.Data.InKey.idcard
                            };
                            //查询本地
                            modelAccount = _userRepository.GetAll().Where(a => a.userMobile == apiRes.Data.InKey.mobile).FirstOrDefault();
                            if (modelAccount == null)
                            {
                                successData.identity = 1;
                                successData.userId = Guid.NewGuid().ToString();
                                successData.isCompleted = false;
                                successData.UserHeadImg = UserInfoImg.GetDefaultUserAvator("");
                                successData.birthday = "";
                                isLoginSuccess = true;
                            }
                            else
                            {
                                //用户禁用
                                if (modelAccount.userEnbleFlag)
                                {
                                    msg = "用户被禁用，请联系管理员！";
                                    isLoginSuccess = false;
                                    successData = null;
                                }
                                else
                                {
                                    successData.identity = modelAccount.identity;
                                    successData.userId = modelAccount.Id.ToString();
                                    successData.isCompleted = modelAccount.isCompleted;
                                    successData.UserHeadImg = modelAccount.smallAvatar;
                                    successData.birthday = modelAccount.userBirthday.ToString();
                                    isLoginSuccess = true;
                                }
                            }
                        }
                        else
                        {
                            msg = apiRes.ErrMsg;
                            isLoginSuccess = false;
                            successData = null;
                        }
                    }
                }
                return isLoginSuccess ? successData : null;
            }
            catch (Exception ex)
            {
                isLoginSuccess = false;
                return null;
            }
            finally
            {
                SmsLoginManger(apiRes, modelAccount, mobile, ipAddress, isLoginSuccess);
            }
        }

        public async Task SmsLoginManger(ApiResponseResult<ApiUserInfoDto> apiRes, UserBase modelAccount, string mobile, string ipAddress, bool isLoginSuccess)
        {
            try
            {
                //  UserBase modelAccount = null;
                if (apiRes == null)
                {
                    return;
                }
                if (apiRes.IsSuccess)
                {
                    if (modelAccount == null)//当该用户不存在的时候
                    {
                        UserBase user = new UserBase();
                        user.Id = Guid.NewGuid();
                        user.userLoginName = apiRes.Data.InKey.loginname;
                        user.userMobile = apiRes.Data.InKey.mobile;
                        user.userEmail = apiRes.Data.InKey.email;
                        user.userIdcard = apiRes.Data.InKey.idcard;
                        user.userGender = apiRes.Data.InKey.gender;
                        user.smallAvatar = UserInfoImg.GetDefaultUserAvator(user.userGender ?? "");
                        user.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey.id) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey.id);
                        user.identity = 1;
                        user.isCompleted = false;
                        user.userEnbleFlag = false;

                        StudentInfo student = new StudentInfo();
                        student.Id = Guid.NewGuid();
                        student.isDel = false;
                        student.createTime = DateTime.Now;
                        student.updateTime = DateTime.Now;
                        student.userId = user.Id;
                        student.userEnbleFlag = 0;

                        _userRepository.Insert(user);
                        _studentRepository.Insert(student);
                        //   _unitOfWorkManager.Current.SaveChangesAsync();
                        _unitOfWorkManager.Current.SaveChanges();
                        modelAccount = _userRepository.Get(user.Id);
                    }
                    else
                    {//当存在该用户的时候
                        modelAccount.userLoginName = apiRes.Data.InKey.loginname;
                        modelAccount.userIdcard = apiRes.Data.InKey.idcard;
                        modelAccount.userGender = apiRes.Data.InKey.gender;
                        modelAccount.userEmail = apiRes.Data.InKey.email;
                        modelAccount.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey.id) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey.id);
                        //modelAccount.userFullName = apiRes.Data.InKey.loginname;
                        //   _unitOfWorkManager.Current.SaveChangesAsync();
                        _userRepository.Update(modelAccount);
                        _unitOfWorkManager.Current.SaveChanges();
                    }
                    if (isLoginSuccess)
                    {
                        modelAccount.loginIp = ipAddress;
                        modelAccount.sessionId = System.Web.HttpContext.Current.Session.SessionID;
                        modelAccount.loginTime = DateTime.Now;
                        _userRepository.Update(modelAccount);
                        await _unitOfWorkManager.Current.SaveChangesAsync();
                        var loginUserSession = new UserCookie
                        {
                            Id = modelAccount.Id,
                            UserName = modelAccount.userFullName,
                            UserUid = modelAccount.Id.ToString(),
                            LoginName = modelAccount.userLoginName,
                            PassWord = modelAccount.userPassWord,
                            IsAdmin = modelAccount.identity == 3,
                            UserHeadImg = modelAccount.smallAvatar,
                            Identity = modelAccount.identity,
                            UserEnbleFlag = modelAccount.userEnbleFlag,
                            Birthday = modelAccount.userBirthday.ToString(),
                            PhoneNumber = modelAccount.userMobile,
                            Gender = modelAccount.userGender,
                            LoginIpAddress = modelAccount.loginIp,
                            SessionId = modelAccount.sessionId,
                            IsCompleted = modelAccount.isCompleted,
                            IsLogin = true

                        };
                        loginUserSession.IsSuperAdmin = IsSuperAdmin(modelAccount.Id);
                        CookieHelper.SetLoginInUserCookie(loginUserSession, false, false);
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        #endregion
        #region 检查用户是否超级管理员
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public bool IsSuperAdmin(Guid userId)
        {
            var adminInfo =
                _adminRepository.GetAll().AsNoTracking().FirstOrDefault(a => a.userId.Equals(userId) && a.isAdmin);
            if (adminInfo == null)
                return false;
            else
            {
                return true;
            }
        }

        #endregion
        #region 短信注册

        /// <summary>
        /// 短信注册
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public UserBase UserSmsRegister(UserRegisterModel model, ref string msg)
        {
            try
            {
                var sessionMsg = HttpContext.Current.Session[model.RegisterMobile] == null ? "" : HttpContext.Current.Session[model.RegisterMobile].ToString();
                if (sessionMsg != model.SmsCode)
                {
                    msg = "验证码错误或已过期,请重新输入";
                    return null;
                }
                else
                {
                    //清除短信验证码Session
                    HttpContext.Current.Session.Remove(model.RegisterMobile);

                    UserBase user = new UserBase();
                    user.Id = Guid.NewGuid();
                    user.userMobile = model.RegisterMobile;
                    user.smallAvatar = UserInfoImg.GetDefaultUserAvator(user.userGender ?? "");
                    user.identity = 1;
                    user.userEnbleFlag = false;
                    user.userPassWord = sha1Encrypt.getSHA1Value(model.PassWord ?? "");
                    user.loginIp = System.Web.HttpContext.Current.Request.UserHostAddress;
                    user.sessionId = System.Web.HttpContext.Current.Session.SessionID;
                    user.isCompleted = false;
                    user.userLoginName = model.RegisterMobile;
                    var apiRes = _userInfoApiService.Register(null, model).Result;
                    //新课网注册成功
                    if (apiRes.IsSuccess)
                    {
                        user.newMoocUserId = string.IsNullOrEmpty(apiRes.Data.InKey) ? Guid.Empty : Guid.Parse(apiRes.Data.InKey);
                        //注册本地
                        StudentInfo student = new StudentInfo();
                        student.Id = Guid.NewGuid();
                        student.isDel = false;
                        student.createTime = DateTime.Now;
                        student.updateTime = DateTime.Now;
                        student.userId = user.Id;
                        student.userEnbleFlag = 0;

                        _userRepository.Insert(user);
                        _studentRepository.Insert(student);
                        _unitOfWorkManager.Current.SaveChanges();
                        msg = "注册成功！";
                        return user;
                    }
                    else
                    {
                        if (apiRes.ErrMsg.Trim() == "UserExist")
                        {
                            if (!this.CheckMobileExist(model.RegisterMobile, "insert", "", true))
                            {
                                msg = "当前手机号码已在其他UFO平台注册，请用当前手机号码直接登录！";
                                return null;
                            }
                        }
                        msg = apiRes.ErrMsg;
                        return null;
                    }
                }

            }

            catch (Exception ex)
            {
                Logger.Error("用户注册：" + ex.ToString());
                return null;
            }

        }
        #endregion

        #region 修改手机号
        /// <summary>
        /// 检测密码是否正确
        /// </summary>
        /// <returns></returns>
        public bool CheckPwd(Guid id, string pwd)
        {
            UserBase user = this.GetUserBaseByExpre(a => a.Id == id).FirstOrDefault();
            if (user.userPassWord == sha1Encrypt.getSHA1Value(pwd ?? ""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 修改手机号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> mobileMpdifry(MobileModifyView model)
        {
            UserBase user = this.GetUserBaseByExpre(d => d.userMobile == model.oldMobile).FirstOrDefault();
            ModifyMobile apiInputDto = new ModifyMobile();
            apiInputDto.oldmobile = model.oldMobile;
            apiInputDto.newmobile = model.newMobile;
            var res = _userInfoApiService.ModifryUserMobile(null, apiInputDto);
            if (!res.Result.IsSuccess)
            {
                return false;
            }
            user.userMobile = model.newMobile;
            var b = UpdateUser(user);
            return b;

        }
        #endregion
    }
}
