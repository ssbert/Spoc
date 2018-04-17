using System;
using System.Web.Mvc;
using Abp.Web.Models;
using SPOC.Common.Encrypt;
using SPOC.SysSetting;
using SPOC.User;
using SPOC.User.Dto.UserInfo;
using SPOC.Web.Filters;

namespace SPOC.Web.Areas.User.Controllers
{
    [DontWrapResult]
    public class UserController : Controller
    {
        #region 构造函数 注入Service

        public UserController(IUserInfoService userInfoService, IStudentInfoService studentInfoService,
            ITeacherInfoService teacherInfoService, IAdminInfoService adminInfoService, ICloudService cloudService)
        {
            _userInfoService = userInfoService;
            _studentInfoService = studentInfoService;
            _teacherInfoService = teacherInfoService;
            _adminInfoService = adminInfoService;

            _cloudService = cloudService;
           
        }

        #endregion

        /// <summary>
        ///     后台登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            var model = new UserLoginViewModel();
            model.IsRememberMe = "true";
            model.loginMsg = "";
            return View(model); //model
        }

        /// <summary>
        ///     后台登录
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(UserLoginViewModel userLogin)
        {
            if (ModelState.IsValid)
                try
                {
                    userLogin.PassWord = RESEncript.UserLoginRSADecrypt(userLogin.PassWord, false, this).Trim();
                    userLogin.UserName = RESEncript.UserLoginRSADecrypt(userLogin.UserName, false, this).Trim();
                    var msg = string.Empty;
                    userLogin.Trim();
                 
                    var loginUser = _userInfoService.LoginRequest(userLogin.UserName, userLogin.PassWord,
                        Request.UserHostAddress, ref msg, true);
                    if (loginUser != null)
                        return Redirect("/AdminHome/Index/");
                    userLogin.loginStatu = "error";
                    userLogin.loginMsg = string.IsNullOrEmpty(msg) ? "登录密码错误或用户不存在或用户被禁用" : msg;
                    ModelState.AddModelError("", string.IsNullOrEmpty(msg) ? "登录密码错误或用户不存在或用户被禁用" : msg);
                }
                catch (Exception ex)
                {
                    userLogin.UserName = "";
                    userLogin.loginStatu = "error";
                    userLogin.loginMsg = "登录密码错误或用户不存在或用户被禁用";
                    ModelState.AddModelError("", "登录密码错误或用户不存在或用户被禁用");
                }
            else
                return Content("请不要恶意禁用浏览器脚本");
            userLogin.PassWord = "";
            return View(userLogin);
        }


        [UserAuthorization]
        public ActionResult ResetPwd()
        {
            return View();
        }

        [UserAuthorization]
        public JsonResult GetUser(UserInfoQueryInputDto model)
        {
            if (model == null)
                return Json(new UserBase());
            var user = _userInfoService.GetUserBaseByQueryDto(model);
            return Json(user);
        }

        /// <summary>
        ///     学生个人明细查看
        /// </summary>
        /// <returns></returns>
        [UserAuthorization]
        public ActionResult StudentInfoDetails(UserInfoQueryInputDto model)
        {
            if (model != null)
                ViewBag.data = model;
            return View();
        }

        [UserAuthorization]
        public ActionResult TeacherInfoDetails(UserInfoQueryInputDto model)
        {
            if (model != null)
                ViewBag.data = model;
            return View();
        }

        [UserAuthorization]
        public ActionResult AdminInfoDetails(UserInfoQueryInputDto model)
        {
            if (model != null)
                ViewBag.data = model;
            return View();
        }

        public ActionResult StudentInfoTableDetails(UserInfoQueryInputDto model)
        {
            if (model != null)
                ViewBag.data = model;
            return View();
        }

        public ActionResult TeacherInfoTableDetails(UserInfoQueryInputDto model)
        {
            var cloud = _cloudService.GetCloud();
            if (cloud != null)
                ViewBag.IsApplyLiveService = cloud.LiveServiceStatus == 1;
            else
                ViewBag.IsApplyLiveService = false;
            if (model != null)
                ViewBag.data = model;
            return View();
        }

        public ActionResult AdminInfoTableDetails(UserInfoQueryInputDto model)
        {
            if (model != null)
                ViewBag.data = model;
            return View();
        }

        /// <summary>
        ///     得到用户信息
        /// </summary>
        /// <param name="id">users表Id</param>
        /// <returns></returns>
        [UserAuthorization]
        public JsonResult GetUserInfoById(UserInfoQueryInputDto model)
        {
            UserBase _userBase = null;
            var identity = !string.IsNullOrEmpty(model.identity) ? int.Parse(model.identity) : -1;
            if (string.IsNullOrEmpty(model.identity))
            {
                _userBase = _userInfoService.GetUserBaseByQueryDto(model);
                identity = _userBase.identity;
                if (_userBase == null)
                    return Json(new UserBase());
            }
            if (identity == 1)
            {
                // return Json(_studentInfoService.GetStudentInfo(model));

                var data = _studentInfoService.GetStudentInfo(model);


                return Json(data);
            }
            if (identity == 2)
            {
                //return Json(_teacherInfoService.GetTeacherInfoDtoBuUserId(model));

                var data = _teacherInfoService.GetTeacherInfoDtoBuUserId(model);

                return Json(data);
            }
            if (identity == 3)
            {
                return Json(_adminInfoService.GetAdminInfoDtoByQueryInput(model));
            }
            return null;
        }

        [HttpPost]
        [UserAuthorization]
        public ActionResult ResetPwd(ModifyPassWord model)
        {
            return View();
        }

        /// <summary>
        ///     检查用户名是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <param name="checkname"></param>
        /// <param name="oldName"></param>
        /// <returns></returns>
        public JsonResult UserNameCheck(string type, string checkname, string oldName = "")
        {
            return Json(_userInfoService.CheckNameExit(checkname, type, oldName), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     检查手机号码是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <param name="checkname"></param>
        /// <param name="oldName"></param>
        /// <returns></returns>
        public JsonResult UserMobileCheck(string type, string checkMobile, string oldMobile = "")
        {
            return Json(_userInfoService.CheckMobileExist(checkMobile, type, oldMobile), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     检查邮箱是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <param name="checkname"></param>
        /// <param name="oldName"></param>
        /// <returns></returns>
        public JsonResult UserEamilCheck(string type, string checkEmail, string oldEmail = "")
        {
            return Json(_userInfoService.CheckEmailExist(checkEmail, type, oldEmail), JsonRequestBehavior.AllowGet);
        }

        #region 变量

        private readonly IUserInfoService _userInfoService;

        private readonly IStudentInfoService _studentInfoService;
        private readonly ICloudService _cloudService;
        private readonly ITeacherInfoService _teacherInfoService;
        private readonly IAdminInfoService _adminInfoService;

        #endregion

        #region 获取列表

        #endregion
    }
}