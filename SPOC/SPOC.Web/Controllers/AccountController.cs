using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Abp.Runtime.Caching;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers;
using Castle.Core.Logging;
using SmartUFO.EntityFramework;
using SPOC.Common;
using SPOC.Common.Cookie;
using SPOC.Common.Encrypt;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.EntityFramework;
using SPOC.SysSetting;
using SPOC.SysSetting.SiteSetDTO;
using SPOC.User;
using SPOC.User.Dto.UserInfo;
using SPOC.Web.Models;

namespace SPOC.Web.Controllers
{
    public class AccountController : SPOCControllerBase
    {
        
        private readonly IUserInfoService _iUserInfoService;
        private readonly IStudentInfoService _iStudentInfoService;
        private readonly IDepartmentService _iDepartmentService;
        private readonly ILogger _iLogger;
        private readonly ICacheManager _cacheManager;
        public AccountController( IUserInfoService iUserInfoService, IDepartmentService iDepartmentService, IStudentInfoService iStudentInfoService, ICacheManager cacheManager)
        { 
            _iUserInfoService = iUserInfoService;
            _iDepartmentService = iDepartmentService;
            _iStudentInfoService = iStudentInfoService;
            _cacheManager = cacheManager;
            _iLogger = new NullLogger();
        }

        #region 登录

        public async Task<ActionResult> Login(string skipUrl = "")
        {
            //判断安装
            if (SPOCConsts.version == L("version"))
            {
                if (L("IsAdminInitialize") == "true")
                {
                    //加入缓存  默认缓存60mins
                    var users = _cacheManager.GetCache("DefaultCache").Get("GetUserBaseByExpre",
                        () => _iUserInfoService.GetUserBaseByExpre(a => a.identity == 3));
                    if (users.Count == 0)
                    {
                        return Redirect("/Account/AdminRegister");
                    }
                }

                if (string.IsNullOrEmpty(skipUrl))
                {
                    skipUrl = "/Home/Index";
                }
                if (CookieHelper.CheckIsLogin())
                {
                    if (CookieHelper.GetLoginInUserInfo().Identity > 1)  //如果是管理员或者老师 直接跳到后台
                        skipUrl = "/AdminHome/Index/";
                    return Redirect(skipUrl);
                }
                var registerDisplay = BaseSiteSetDto.userRegisterDispaly;
                var model = new LoginViewModel
                {

                    SkipUrl = skipUrl,
                    RsaKey = RESEncript.GetEncriptKey(),
                    ModulusKey = RESEncript.GetEncriptModulusKey(),
                    RegisterDisplay = string.IsNullOrEmpty(registerDisplay) || registerDisplay == "true"
                };
                if (model.RegisterDisplay)
                {
                    model.ClassListItems = await _cacheManager.GetCache("ControllerCache").Get("AllClass", () => _iDepartmentService.GetAllClassSelectItem(true));

                    //model.ClassListItems = await _iDepartmentService.GetAllClassSelectItem(true);
                }
                return View(model);
                
            }
            return RedirectToAction("InitDataBase", "Home");
        }

        [HttpPost]
        public JsonResult DoLogin(LoginModel input)
        {
            /* Result.Code
             * 0：正常
             * 1：验证不通过
             * 2：登录失败
             * 99：未知错误
             */

            var result = new ResultModel<object>();
            try
            {
                string key = RESEncript.GetEncriptKey();
                var siteSet = BaseSiteSetDto;
                input.UserLoginName = RESEncript.UserLoginRSADecrypt(input.UserLoginName.Trim(), false, this).Trim();
                input.UserPassWord = RESEncript.UserLoginRSADecrypt(input.UserPassWord, false, this).Trim();

                //本站登录
                string msg = string.Empty;
                var siteLogin = siteSet.siteLogin == "true";
                var loginUser = _iUserInfoService.LoginRequest(input.UserLoginName, input.UserPassWord,
                    Request.UserHostAddress, ref msg, false, false, input.RememberMe, siteLogin);
                if (loginUser == null)
                {
                    result.Msg = msg;
                    result.Code = 2;
                }
                else if (loginUser.identity > 1) //教师与管理员直接跳转到后台
                    result.Result = "admin";
                return Json(result);
            }
            catch (Exception e)
            {
                var errorCode = Guid.NewGuid().GetHashCode();
                result.Code = 99;
                result.Msg = "[" + errorCode + "]\n" + e;
                Logger.Error(result.Msg);
                return Json(result);
            }
        }

        #endregion


        #region 忘记密码

        public ActionResult ForgetPassword()
        {
            CookieHelper.RemoveUserCookie();
            var model = new ForgetPwdViewModel2();
            return View(model);
        }

        /// <summary>
        /// 忘记密码提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgetPassword(ForgetPwdViewModel2 model)
        {
            var siteSetDto = BaseSiteSetDto;
            if (ModelState.IsValid)
            {
                model.modelTrim();
                try
                {
                    if (siteSetDto.isSendEmail != "open")
                    {
                        model.Msg = "暂不能发送邮件，请联系管理员开启相关配置！";
                        model.Statu = "error";
                        return View(model);
                    }
                    if (Session["checkCode"] == null)
                    {
                        model.reSet();
                        return View(model);
                    }

                    if (Session["checkCode"].ToString().ToLower() != model.ComfrimCode.ToLower())
                    {
                        model.ComfrimCode = "";
                        model.Msg = "验证码不正确";
                        model.Statu = "error";
                        ModelState.AddModelError("", "验证码不正确");
                    }
                    else
                    {
                        var res = _iUserInfoService.userForgetPassWord(model.UserName, "/Account/UpdatePwd/");
                        if (res.Result == "")
                        {
                            model.ComfrimCode = "";
                            model.Msg = "已成功发送邮件至您的邮箱，请按提示进行操作。";
                            model.Statu = "ok";
                        }
                        else
                        {
                            model.ComfrimCode = "";
                            model.Msg = res.Result;
                            model.Statu = "error";
                            ModelState.AddModelError("", "信息有误");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("异常信息，同步地址：" + ex.Message + ":" + (ex.InnerException?.Message ?? ""));
                    model.Msg = "失败，信息有误";
                    model.Statu = "error";
                    model.ComfrimCode = "";
                    ModelState.AddModelError("", "信息有误");
                }
                return View(model);
            }
            else
            {
                return Content("请不要恶意禁用浏览器脚本!");
            }
        }

        #endregion

        #region 验证码

        public ActionResult VCode()
        {

            var v = new VCode();

            byte[] arrImg = v.GetVCode();
            string checkCode = Common.VCode.CodeStr;
            Session["checkCode"] = checkCode;
            return File(arrImg, "image/jpeg");
        }

        #endregion

        


        #region  个人中心设置

        [Filters.UserAuthorization]
        public ActionResult Center(string operation="",string code="",string msg="")
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var updateModel = new UserInfoUpdateModel
            {
                AllowEditEmail = BaseSiteSetDto.isSendEmail == "open",
                AllowSetAvtar = BaseSiteSetDto.allowEditAvatarPicture.ToLower() == "true",
                operation = operation,
                code = code,
                msg = msg,
                AcatarImg = cookie.UserHeadImg
            };
            if (cookie.Identity == 1)
            {
                //学生信息
                var userInfo =
                    _iStudentInfoService.GetStudentInfo(new UserInfoQueryInputDto {userId = cookie.Id});
                updateModel.CurrentEmail = userInfo.user_email;
                updateModel.ClassName = userInfo.user_class;
                updateModel.FacultyName = userInfo.user_faculty;
                updateModel.MajorName = userInfo.user_major;
                updateModel.Mobile = userInfo.user_mobile;
                updateModel.Id = userInfo.user_id;
                updateModel.UserName = userInfo.user_login_name;
                updateModel.UserFullName = userInfo.user_name;


            }
            else
            {
                //教师管理员
                var userInfo = _iUserInfoService.GetUserBaseByPhoneById(cookie.Id);
                updateModel.CurrentEmail = userInfo.userEmail;
                updateModel.AcatarImg = cookie.UserHeadImg;
                updateModel.Mobile = userInfo.userMobile;
                updateModel.Id = userInfo.Id;
                updateModel.UserName = userInfo.userLoginName;
                updateModel.UserFullName = userInfo.userFullName;
            }

           
            return View(updateModel);
        }

        [DontWrapResult]
        [HttpPost]
        public JsonResult BaseSetting(UserInfoUpdateModel model)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            try
            {
                if (ModelState.IsValid)
                {
                    List<string> perList = new List<string>() { "userFullName", "userLoginName" };
                    string errMsg = "";
                    if (_iUserInfoService.UpdateUserByUserInfoInputDto(new UserInfoInputDto{ userFullName = model.UserFullName,Id= cookie.Id,userLoginName = model.UserName }, perList, ref errMsg))
                    { 
                        //更新cookie信息
                         cookie.UserName = model.UserFullName;
                         cookie.LoginName = model.UserName;
                         CookieHelper.UpdateUserCookie(cookie);
      
                        return Json(new { statu = "ok", msg = "保存成功!" });
                    }
                    else
                    {
                        return Json(new { statu = "error", msg = string.IsNullOrEmpty(errMsg) ? "保存失败!" : errMsg });
                    }
                }
                return Json(new { statu = "error", msg = "请不要恶意禁用浏览器脚本" });
            }
            catch (Exception ex)
            {
               
                return Json(new { statu = "error", msg = "保存失败，resson:" + ex.ToString() });
            }
        }
        [DontWrapResult]
        [HttpPost]
        public  JsonResult PassWordSet(UserInfoUpdateModel model)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            try
            {
                if (ModelState.IsValid)
                {
                  
                    var returnMsg =  _iUserInfoService.PwdModifry(new ModifyPassWord
                    {
                        confirmPwd = model.ConfirmPwd,
                        newPassWord = model.NewPwd,
                        userId = cookie.Id,
                        oldPassWord = model.PassWord
                    });
                    if (returnMsg.MsgCode== "ok")
                    {
                        
                        return Json(new { statu = "ok", msg = "密码修改成功!" });
                    }
                    else
                    {
                        return Json(new { statu = "error", msg = string.IsNullOrEmpty(returnMsg.MsgContent) ? "密码修改失败!" : returnMsg.MsgContent });
                    }
                }
                return Json(new { statu = "error", msg = "请不要恶意禁用浏览器脚本" });
            }
            catch (Exception ex)
            {

                return Json(new { statu = "error", msg = "密码修改失败，resson:" + ex.ToString() });
            }
        }
        [DontWrapResult]
        [HttpPost]
        public JsonResult EmailSet(UserInfoUpdateModel model)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            try
            {
                if (ModelState.IsValid)
                {
                    var uBase = _iUserInfoService.GetUserBaseByExpre(a => a.Id ==  cookie.Id).FirstOrDefault();
                    if(uBase==null)
                        return Json(new { statu = "error", msg = "邮箱修改失败！用户信息不存在" });
                    var returnMsg = _iUserInfoService.UserEmailSetting(new UserEmailModifyView{currentEmail= uBase.userEmail,newEmail= model.Email,passWord= model.PassWord,userId= uBase.Id, userMobile=uBase.userMobile,userName= uBase.userLoginName}, cookie, "/Account/UpdateEmail");
                    if (returnMsg.MsgCode == "ok")
                    {

                        return Json(new { statu = "ok", msg = "邮箱修改成功!" });
                    }
                    else
                    {
                        return Json(new { statu = "error", msg = string.IsNullOrEmpty(returnMsg.MsgContent) ? "邮箱修改失败!" : returnMsg.MsgContent });
                    }
                }
                return Json(new { statu = "error", msg = "请不要恶意禁用浏览器脚本" });
            }
            catch (Exception ex)
            {

                return Json(new { statu = "error", msg = "密码修改失败，resson:" + ex.ToString() });
            }
        }

        public ActionResult UpdateEmail(string Key, string getstring)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {    //未登录需要跳转登录
                return RedirectToAction("Login", "Account", new RouteValueDictionary() { { "skipUrl", $"/account/UpdateEmail?Key={Key}&getstring={getstring}" } });
            }
            string msg = "";
            string[] Keystring = SymmetricCryptoMethod.DecodeBase64(Key).Split('|');
            string time = SymmetricCryptoMethod.DecodeBase64(getstring);
            string username = Keystring[0];//用户名
            string email = Keystring[1];//邮箱
            if (DateTime.Now > DateTime.Parse(time).AddHours(1))
            {
                return RedirectToAction("Center", "Account", new RouteValueDictionary() { { "operation", "CheckEmail" }, { "code", "-2" }, { "msg", "链接已超时" } });
               
            }
            var b = _iUserInfoService.EmailModifySetting(new UserEmailModifyView{userName= username,newEmail= email }, true, ref msg);
            if (b)
            {
                return RedirectToAction("Center", "Account", new RouteValueDictionary() { { "operation", "CheckEmail"}, { "code", "1" } });
            }
            else
            {
                return RedirectToAction("Center", "Account", new RouteValueDictionary() { { "operation", "CheckEmail" }, { "code", "-1" }, { "msg", msg } });
            }
          
        }
        [HttpPost]
        public ActionResult AvatarSetting(UserAvtarSet model)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            string srcPath = Server.MapPath(model.AcatarImg);
            try
            {
                string srcVirtualpath = GetFilePath(null, "UserInfo/Avtar", model.AcatarImg);
                byte[] byteArr = Common.File.PictureHandler.MakeThumbnail(srcPath, Convert.ToInt32(model.x1), Convert.ToInt32(model.y1), Convert.ToInt32(model.selectionW), Convert.ToInt32(model.selectionH), Server.MapPath(srcVirtualpath), model.defaultImgLen);

                UserBase user =_iUserInfoService.GetUserBaseByPhoneById(cookie.Id);
                user.smallAvatar = srcVirtualpath;
                if (_iUserInfoService.UpdateUser(user))
                {
                    
                    cookie.UserHeadImg = srcVirtualpath;
                    CookieHelper.UpdateUserCookie(cookie);
                    return RedirectToAction("Center", "Account", new RouteValueDictionary() { { "operation", "AvatarSetting" }, { "code", "1" } });
                }
                else
                {
                    return RedirectToAction("Center", "Account", new RouteValueDictionary() { { "operation", "AvatarSetting" }, { "code", "-1" }, { "msg", "上传失败" } });
                }
            }
            catch (Exception ex)
            {

                _iLogger.Error(ex.ToString());
                return RedirectToAction("Center", "Account", new RouteValueDictionary() { { "operation", "AvatarSetting" }, { "code", "-1" }, { "msg", "上传失败" } });
            }
            finally
            {

                if (System.IO.File.Exists(srcPath))
                    System.IO.File.Delete(srcPath);
            }

        }
        [DontWrapResult]
        public JsonResult UserAvatarUpload()
        {
            try
            {
                ImgFormat imgFormat = new ImgFormat(new List<string>() { "png", "gif", "jpg" });
                imgFormat.FileSize = 2;
                HttpPostedFileBase postFile = Request.Files[0];
                string msg;
                System.Drawing.Image img;
                if (!imgFormat.CheckImgFormat(postFile, out msg, out img))
                {
                    return Json(new { statu = "error", msg = msg }, JsonRequestBehavior.AllowGet);
                }
                string srcVirtualpath = GetFilePath(postFile, "UserInfo/Avtar/Temp");
                postFile.SaveAs(Server.MapPath(srcVirtualpath));

                var res = new { statu = "ok", msg = "", imgH = img.Height, imgW = img.Width, path = srcVirtualpath };
                return Json(res);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.Message);
                var res = new { statu = "error", msg = ex.ToString(), path = "" };
                return Json(res);
            }
            // return "{statu = \"ok\",msg=\"\", path = \"\"}"; 
        }
        public string GetFilePath(HttpPostedFileBase file, string dirName, string fileName = "")
        {
            string path = string.Empty;
            try
            {
                string rootPath = System.Web.HttpContext.Current.Server.MapPath("~/");
                string tempDir = Path.Combine(rootPath, "files", dirName);
                if (!System.IO.Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                string name = "";
                if (file != null)
                {
                    name = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    name = Guid.NewGuid() + "_" + Path.GetFileName(fileName);
                }
                string filePath = Path.Combine(tempDir, name);
                string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath);//获取程序根目录
                string fileUrl = filePath.Replace(tmpRootDir, ""); //转换成相对路径
                fileUrl = fileUrl.Replace(@"\", @"/");
                path = "/" + fileUrl;

            }
            catch (Exception ex)
            {
               _iLogger.Error(ex.Message);
            }
            return path;
        }
        #endregion

        public ActionResult LoginOut()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie.IsLogin)
            {
                _iUserInfoService.RemoveLoginSessionId(cookie.Id, Session.SessionID);
                CookieHelper.RemoveUserCookie();
            }
            return RedirectToAction("Login", "Account");
        }

        [DontWrapResult]
        public JsonResult GetRsaKey()
        {

            return Json(new { encryptKey = RESEncript.GetEncriptKey(), ModulusKey = RESEncript.GetEncriptModulusKey() }, JsonRequestBehavior.AllowGet);
        }

        [Filters.UserAuthorization]
        [DontWrapResult]
        public JsonResult GetAccount()
        {
            UserCookie _user = CookieHelper.GetLoginInUserInfo();
            var user = _iUserInfoService.GetUserBaseByPhoneById(_user.Id);
            return Json(new { id = user.Id.ToString(), userName = user.userLoginName, userMobile = user.userMobile });
        }

        #region 获取用户是否在其他地方登陆+GetloginLimit
        [DontWrapResult]
        public JsonResult GetloginLimit()
        {
            var siteSetDto = BaseSiteSetDto;
            var user = CookieHelper.GetLoginInUserInfo();
            if (siteSetDto.loginLimit == "open" && user != null && !string.IsNullOrEmpty(user.UserUid))
            {
                var checkUser = _iUserInfoService.GetUserBaseByPhoneById(user.Id) ?? new UserBase();
                if (!string.IsNullOrEmpty(checkUser.sessionId) && checkUser.sessionId != Session.SessionID)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        [DontWrapResult]
        public JsonResult Register(RegisterModel userRegister)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(userRegister.EncucriptPassWord))
                    {
                        userRegister.PassWord = RESEncript.UserLoginRSADecrypt(userRegister.EncucriptPassWord, false, this).Trim();
                    }
                    if (!string.IsNullOrEmpty(userRegister.EncucriptUserName))
                    {

                        userRegister.UserName = RESEncript.UserLoginRSADecrypt(userRegister.EncucriptUserName, false, this).Trim();
                    }
                    if (!_iUserInfoService.CheckNameExit(userRegister.UserName, "insert", "", true))
                    {
                        return Json(new { success = false, msg = "注册失败,用户名已存在!" }, JsonRequestBehavior.AllowGet);
                    }
                    userRegister.UserFullName = HttpUtility.UrlDecode(userRegister.UserFullName);
                    if (BaseSiteSetDto.usernameContainEN.Trim() == "open")
                    {
                        var regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d).*$");
                        var b = regex.IsMatch(userRegister.UserName);
                        if (!b)
                        {
                            userRegister.msg = "用户名格式错误，必须包含英文加数字";
                            userRegister.registerStatu = "error";
                            return Json(new { success = false, msg = "用户名格式错误，必须包含英文加数字!" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    string msg = "";
                    userRegister.Trim();
                    userRegister.RegisterIpAddress = Request.UserHostAddress;
                    UserRegisterModel model = userRegister.GetUserRegisterModel();
                    model.UserFullName = userRegister.UserFullName;
                    UserBase registerUser = _iUserInfoService.UserRegister(model, ref msg);

                    if (registerUser != null)
                    {
                        userRegister.msg = "注册成功";
                        userRegister.registerStatu = "ok";
                        //   userRegister.returnUrl = "/StudyPlatform/User/Login";
                        userRegister.returnUrl = "/Home/Index/";

                      
                        return Json(new { success = true, msg = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        userRegister.msg = string.IsNullOrEmpty(msg) ? "注册失败，信息有误" : msg;
                        userRegister.registerStatu = "error";
                        userRegister.UserName = "";
                        ModelState.AddModelError("", string.IsNullOrEmpty(msg) ? "注册失败，信息有误。" : msg);
                        return Json(new { success = false, msg = "注册失败，信息有误。" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {

                    userRegister.msg = "注册失败，信息有误。";
                    userRegister.registerStatu = "error";
                    userRegister.UserName = "";
                    ModelState.AddModelError("", "注册失败，信息有误。");
                    return Json(new { success = false, msg = "注册失败，信息有误。" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, msg = "请不要恶意禁用浏览器脚本!" }, JsonRequestBehavior.AllowGet);

            }

        }


        #region 管理员注册
        
        /// <summary>
        /// 管理员注册
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminRegister()
        {
            //if (L("IsAdminInitialize") == "false" || _iUserInfoService.GetUserBaseByExpre(a => a.identity == 3).Count > 0)
            //{

            //    return Redirect("/Account/Login");
            //}
            var siteSetDto = BaseSiteSetDto;
            AdminRegisterModel userRegister = new AdminRegisterModel() { msg = "", registerStatu = "admin" };
            ViewBag.usernameContainEN = siteSetDto.usernameContainEN;
            return View(userRegister);
        }
        
        /// <summary>
        /// 管理员注册提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AdminRegister(AdminRegisterModel userRegister)
        {
            if (ModelState.IsValid)
            {
                userRegister.RegisterType = "admin";
                try
                {
                    if (!string.IsNullOrEmpty(userRegister.EncucriptPassWord))
                    {
                        userRegister.PassWord = RESEncript.UserLoginRSADecrypt(userRegister.EncucriptPassWord, false, this).Trim();
                    }
                    if (!string.IsNullOrEmpty(userRegister.EncucriptUserName))
                    {
                        userRegister.UserName = RESEncript.UserLoginRSADecrypt(userRegister.EncucriptUserName, false, this).Trim();
                    }

                    if (L("IsAdminInitialize") == "false" || _iUserInfoService.GetUserBaseByExpre(a => a.identity == 3).Count > 0)
                    {
                        userRegister.msg = "该站点已经注册过管理员";
                        userRegister.registerStatu = "error";
                        return View(userRegister);
                    }

                    if (BaseSiteSetDto.usernameContainEN.Trim() == "open")
                    {
                        var regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d).*$");
                        var b = regex.IsMatch(userRegister.UserName);
                        if (!b)
                        {
                            userRegister.msg = "用户名格式错误，必须包含英文加数字";
                            userRegister.registerStatu = "error";
                            return View(userRegister);
                        }
                    }

                    string msg = "";
                    userRegister.Trim();
                    userRegister.RegisterIpAddress = Request.UserHostAddress;
                    UserRegisterModel model = userRegister.GetUserRegisterModel();
                    model.RegisterType = "admin";
                    UserBase registerUser = _iUserInfoService.UserRegister(model, ref msg);
                    if (registerUser != null)
                    {
                        UserCookie userCookie = new UserCookie()
                        {
                            UserName = registerUser.userFullName,
                            UserUid = registerUser.Id.ToString(),
                            LoginName = registerUser.userLoginName,
                            PassWord = registerUser.userPassWord,
                            IsAdmin = true,
                            Identity = registerUser.identity,
                            UserHeadImg = registerUser.smallAvatar,
                            UserEnbleFlag = false,
                            Id = registerUser.Id,
                            IsLogin = true
                        };
                        userCookie.IsSuperAdmin = _iUserInfoService.IsSuperAdmin(registerUser.Id);
                        CookieHelper.SetLoginInUserCookie(userCookie);

                        userRegister.msg = "注册成功";
                        userRegister.registerStatu = "ok";
                        userRegister.returnUrl = "/AdminHome/Index/";

                        //修改XML里面的状态
                        var xml = AppDomain.CurrentDomain.BaseDirectory + "Localization\\" + SPOCConsts.LocalizationSourceName + "\\" + SPOCConsts.LocalizationSourceName + ".xml";
                        XDocument doc = XDocument.Load(xml);
                        foreach (var item in doc.Descendants("text"))
                        {
                            if (item.Attribute("name").Value == "IsAdminInitialize")
                            {
                                item.SetAttributeValue("value", "false");
                            }
                        }
                    }
                    else
                    {
                        userRegister.msg = string.IsNullOrEmpty(msg) ? "注册失败，信息有误" : msg;
                        userRegister.registerStatu = "error";
                        ModelState.AddModelError("", string.IsNullOrEmpty(msg) ? "注册失败，信息有误。" : msg);
                    }
                }
                catch (Exception ex)
                {

                    userRegister.msg = "注册失败，信息有误。";
                    userRegister.registerStatu = "error";
                    ModelState.AddModelError("", "注册失败，信息有误。");
                }

            }
            else
            {
                return Content("请不要恶意禁用浏览器脚本!");
            }
            return View(userRegister);
        }

        #endregion

        #region remoto
        [DontWrapResult]
        public JsonResult UserNameExistCheck(string UserName)
        {
         
            bool b = _iUserInfoService.CheckNameExit(UserName, "insert", "", true);
            return Json(b, JsonRequestBehavior.AllowGet);
        }
        [DontWrapResult]
        public JsonResult UserNameUpdateExistCheck(string UserName)
        {
            UserCookie cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            bool b = _iUserInfoService.CheckNameExit(UserName, "update", cookie.LoginName, true);
            return Json(b, JsonRequestBehavior.AllowGet);
        }
        [DontWrapResult]
        public JsonResult MobileExistCheck(string RegisterMobile)
        {
            bool b = _iUserInfoService.CheckMobileExist(RegisterMobile, "insert");
            return Json(b, JsonRequestBehavior.AllowGet);
        }
        [DontWrapResult]
        public JsonResult MobileExistCheckByRegister(string RegisterMobile)
        {
            bool b = _iUserInfoService.CheckMobileExist(RegisterMobile, "insert", "", true);
            return Json(b, JsonRequestBehavior.AllowGet);
        }
        [DontWrapResult]
        public JsonResult EmailExistCheck(string RegisterEmail)
        {
            bool b = _iUserInfoService.CheckEmailExist(RegisterEmail, "insert", "", true);
            return Json(b, JsonRequestBehavior.AllowGet);
        }
        [DontWrapResult]
        [HttpPost]
        public JsonResult UserNameExistCheckByForgetPwd(string UserName)
        {
            UserName = UserName.Trim();
            bool b = _iUserInfoService.CheckNameExit(UserName, "insert", "", false);
            //  bool b = false;
            return Json(!b, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [DontWrapResult]
        public JsonResult VcodeExistCheckByForgetPwd(string comfrimCode)
        {
            comfrimCode = comfrimCode.Trim().ToLower();
            bool b = false;
            if (Session["checkCode"] != null)
            {
                b = comfrimCode == Session["checkCode"].ToString().ToLower();
            }
            return Json(b, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检测手机号
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns>手机号不存在返回true</returns>
        [DontWrapResult]
        public JsonResult SmsForgetCheckMobile(string Mobile)
        {
            bool b = _iUserInfoService.CheckMobileExist(Mobile, "insert", "", true) == false;
            return Json(b, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}