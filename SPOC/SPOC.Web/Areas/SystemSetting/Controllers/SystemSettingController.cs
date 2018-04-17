using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.Web.Models;
using Castle.Core.Logging;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.Tools;
using SPOC.SysSetting;
using SPOC.SysSetting.CloudDTO;
using SPOC.SysSetting.MenuDTO;
using SPOC.SysSetting.RoleManageDTO;
using SPOC.SysSetting.SiteSetDTO;
using SPOC.SysSetting.SiteVersionDto;
using SPOC.SysSetting.SystemLogDTO;
using SPOC.SystemSet;
using SPOC.Web.Controllers;

namespace SPOC.Web.Areas.SystemSetting.Controllers
{
    [DontWrapResult]
    public class SystemSettingController : SPOCControllerBase
    {
        private readonly ISiteSetService _iSiteSetService;
        private readonly ICloudService _iCloudService;
        private readonly ILogger _iLogger;

        private readonly IMenuService _iMenuService;

        private readonly IRoleManageService _iRoleManageService;

        private readonly ISystemLogService _iSystemLogService;

        public SystemSettingController(ISiteSetService iSiteSetService, ILogger iLogger,  ISystemLogService iSystemLogService, IMenuService iMenuService, IRoleManageService iRoleManageService, ICloudService iCloudService)
        {
            _iSiteSetService = iSiteSetService;
            _iLogger = iLogger;
            _iSystemLogService = iSystemLogService;
            _iMenuService = iMenuService;
            _iRoleManageService = iRoleManageService;
            _iCloudService = iCloudService;
        }

        #region View

       
 


        public ActionResult UserConfig()
        {
            SiteSetDto ssd = _iSiteSetService.GetAllSiteSet();
            return View(ssd);
        }


        public ActionResult SwitchUser()
        {
            return View();
        }

        public ActionResult MenuConfig()
        {
            return View();
        }

        public ActionResult RoleConfig()
        {
            return View();
        }

        #endregion


        #region siteInfo Modify

        public JsonResult ModifyPlatformSiteSet(SiteSetDto siteDto)
        {
            try
            {
                List<SiteSetInputDto> inputDtoList = new List<SiteSetInputDto>();
                SiteSetInputDto siteName = new SiteSetInputDto();
                siteName.setKey = "site_name";
                siteName.setValue = siteDto.siteName;
                inputDtoList.Add(siteName);

                SiteSetInputDto siteSlogan = new SiteSetInputDto();
                siteSlogan.setKey = "site_slogan";
                siteSlogan.setValue = siteDto.siteSlogan;
                inputDtoList.Add(siteSlogan);

                SiteSetInputDto siteUrl = new SiteSetInputDto();
                siteUrl.setKey = "site_url";
                siteUrl.setValue = siteDto.siteUrl;
                inputDtoList.Add(siteUrl);

                SiteSetInputDto siteLogo = new SiteSetInputDto();
                siteLogo.setKey = "site_logo";
                siteLogo.setValue = siteDto.siteLogo;
                inputDtoList.Add(siteLogo);

                SiteSetInputDto siteFavicon = new SiteSetInputDto();
                siteFavicon.setKey = "site_favicon";
                siteFavicon.setValue = siteDto.siteFavicon;
                inputDtoList.Add(siteFavicon);

                SiteSetInputDto siteSeoKeyWords = new SiteSetInputDto();
                siteSeoKeyWords.setKey = "site_seo_keywords";
                siteSeoKeyWords.setValue = siteDto.siteSeoKeyWords;
                inputDtoList.Add(siteSeoKeyWords);

                SiteSetInputDto siteSeoKeyWordsDescription = new SiteSetInputDto();
                siteSeoKeyWordsDescription.setKey = "site_seo_description";
                siteSeoKeyWordsDescription.setValue = siteDto.siteSeoKeyWordsDescription;
                inputDtoList.Add(siteSeoKeyWordsDescription);

                SiteSetInputDto siteMasterEmail = new SiteSetInputDto();
                siteMasterEmail.setKey = "site_master_email";
                siteMasterEmail.setValue = siteDto.siteMasterEmail;
                inputDtoList.Add(siteMasterEmail);

                SiteSetInputDto siteCopyright = new SiteSetInputDto();
                siteCopyright.setKey = "site_copyright";
                siteCopyright.setValue = siteDto.siteCopyright;
                inputDtoList.Add(siteCopyright);

                SiteSetInputDto siteIcp = new SiteSetInputDto();
                siteIcp.setKey = "site_icp";
                siteIcp.setValue = siteDto.siteIcp;
                inputDtoList.Add(siteIcp);

                SiteSetInputDto siteQQ = new SiteSetInputDto();
                siteQQ.setKey = "site_qq";
                siteQQ.setValue = siteDto.siteQQ;
                inputDtoList.Add(siteQQ);

                SiteSetInputDto siteAnalytics = new SiteSetInputDto();
                siteAnalytics.setKey = "site_analytics";
                siteAnalytics.setValue = siteDto.siteAnalytics;
                inputDtoList.Add(siteAnalytics);

                _iSiteSetService.ModifySiteSet(inputDtoList);
                CacheStrategy.Remove("SiteSet");
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
            }
            return Json("ok");
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ModifyUserConfigSet(SiteSetDto siteDto)
        {
            try
            {
                List<SiteSetInputDto> inputDtoList = new List<SiteSetInputDto>();

                SiteSetInputDto userRegisterDisplay = new SiteSetInputDto();
                userRegisterDisplay.setKey = "user_register_dispaly";
                userRegisterDisplay.setValue = siteDto.userRegisterDispaly;
                inputDtoList.Add(userRegisterDisplay);

                SiteSetInputDto registerEmailActivationTitle = new SiteSetInputDto();
                registerEmailActivationTitle.setKey = "register_email_activation_title"; 
                registerEmailActivationTitle.setValue = siteDto.registerEmailActivationTitle;
                inputDtoList.Add(registerEmailActivationTitle);

                SiteSetInputDto registerEmailActivationBody = new SiteSetInputDto();
                registerEmailActivationBody.setKey = "register_email_activation_body"; registerEmailActivationBody.setValue = siteDto.registerEmailActivationBody;
                inputDtoList.Add(registerEmailActivationBody);

                SiteSetInputDto registerWelcomeEnabled = new SiteSetInputDto();
                registerWelcomeEnabled.setKey = "register_welcome_enabled";
                registerWelcomeEnabled.setValue = siteDto.registerWelcomeEnabled;
                inputDtoList.Add(registerWelcomeEnabled);

                SiteSetInputDto registerWelcomeSenders = new SiteSetInputDto();
                registerWelcomeSenders.setKey = "register_welcome_sender";
                registerWelcomeSenders.setValue = siteDto.registerWelcomeSender;
                inputDtoList.Add(registerWelcomeSenders);

                SiteSetInputDto registerWelcomeTitle = new SiteSetInputDto();
                registerWelcomeTitle.setKey = "register_welcome_title";
                registerWelcomeTitle.setValue = siteDto.registerWelcomeTitle;
                inputDtoList.Add(registerWelcomeTitle);

                SiteSetInputDto registerWelcomeBody = new SiteSetInputDto();
                registerWelcomeBody.setKey = "register_welcome_body";
                registerWelcomeBody.setValue = siteDto.registerWelcomeBody;
                inputDtoList.Add(registerWelcomeBody);

                SiteSetInputDto firstLoginPerfectUserData = new SiteSetInputDto();
                firstLoginPerfectUserData.setKey = "first_login_perfect_userdata";
                firstLoginPerfectUserData.setValue = siteDto.firstLoginPerfectUserData;
                inputDtoList.Add(firstLoginPerfectUserData);

                SiteSetInputDto allowNologinAccess = new SiteSetInputDto();
                allowNologinAccess.setKey = "allow_nologin_access";
                allowNologinAccess.setValue = siteDto.allowNologinAccess;
                inputDtoList.Add(allowNologinAccess);

                SiteSetInputDto loginLimit = new SiteSetInputDto();
                loginLimit.setKey = "login_limit";
                loginLimit.setValue = siteDto.loginLimit;
                inputDtoList.Add(loginLimit);

                SiteSetInputDto siteLogin = new SiteSetInputDto();
                siteLogin.setKey = "site_Login";
                siteLogin.setValue = siteDto.siteLogin;
                inputDtoList.Add(siteLogin);

                SiteSetInputDto loginEnabled = new SiteSetInputDto();
                loginEnabled.setKey = "login_enabled";
                loginEnabled.setValue = siteDto.loginEnabled;
                inputDtoList.Add(loginEnabled);

                SiteSetInputDto loginWeiboEnabled = new SiteSetInputDto();
                loginWeiboEnabled.setKey = "login_weibo_enabled";
                loginWeiboEnabled.setValue = siteDto.loginWeiboEnabled;
                inputDtoList.Add(loginWeiboEnabled);

                SiteSetInputDto loginWeiboKey = new SiteSetInputDto();
                loginWeiboKey.setKey = "login_weibo_key";
                loginWeiboKey.setValue = siteDto.loginWeiboKey;
                inputDtoList.Add(loginWeiboKey);

                SiteSetInputDto loginWeiboSecret = new SiteSetInputDto();
                loginWeiboSecret.setKey = "login_weibo_secret";
                loginWeiboSecret.setValue = siteDto.loginWeiboSecret;
                inputDtoList.Add(loginWeiboSecret);

                SiteSetInputDto loginQQEnabled = new SiteSetInputDto();
                loginQQEnabled.setKey = "login_qq_enabled";
                loginQQEnabled.setValue = siteDto.loginQQEnabled;
                inputDtoList.Add(loginQQEnabled);

                SiteSetInputDto loginQQKey = new SiteSetInputDto();
                loginQQKey.setKey = "login_qq_key";
                loginQQKey.setValue = siteDto.loginQQKey;
                inputDtoList.Add(loginQQKey);

                SiteSetInputDto loginQQSecret = new SiteSetInputDto();
                loginQQSecret.setKey = "login_qq_secret";
                loginQQSecret.setValue = siteDto.loginQQSecret;
                inputDtoList.Add(loginQQSecret);

                SiteSetInputDto loginRenrenEnabled = new SiteSetInputDto();
                loginRenrenEnabled.setKey = "login_renren_enabled";
                loginRenrenEnabled.setValue = siteDto.loginRenrenEnabled;
                inputDtoList.Add(loginRenrenEnabled);

                SiteSetInputDto loginRenrenKey = new SiteSetInputDto();
                loginRenrenKey.setKey = "login_renren_key";
                loginRenrenKey.setValue = siteDto.loginRenrenKey;
                inputDtoList.Add(loginRenrenKey);

                SiteSetInputDto loginRenrenSecret = new SiteSetInputDto();
                loginRenrenSecret.setKey = "login_renren_secret";
                loginRenrenSecret.setValue = siteDto.loginRenrenSecret;
                inputDtoList.Add(loginRenrenSecret);

                SiteSetInputDto loginVerifyCode = new SiteSetInputDto();
                loginVerifyCode.setKey = "login_verify_code";
                loginVerifyCode.setValue = siteDto.loginVerifyCode;
                inputDtoList.Add(loginVerifyCode);

                SiteSetInputDto usernameContainEN = new SiteSetInputDto();
                usernameContainEN.setKey = "username_contain_EN";
                usernameContainEN.setValue = siteDto.usernameContainEN;
                inputDtoList.Add(usernameContainEN);

                SiteSetInputDto mailerEnabled = new SiteSetInputDto();
                mailerEnabled.setKey = "mailer_enabled";
                mailerEnabled.setValue = siteDto.mailerEnabled;
                inputDtoList.Add(mailerEnabled);


                SiteSetInputDto userForRegisterDispaly = new SiteSetInputDto();
                userForRegisterDispaly.setKey = "user_for_register_dispaly";
                userForRegisterDispaly.setValue = siteDto.userForRegisterDispaly;
                inputDtoList.Add(userForRegisterDispaly);

                SiteSetInputDto userRegisterIsapprove = new SiteSetInputDto();
                userRegisterIsapprove.setKey = "user_register_isapprove";
                userRegisterIsapprove.setValue = siteDto.userRegisterIsapprove;
                inputDtoList.Add(userRegisterIsapprove);

                SiteSetInputDto allowEditAvatarPicture = new SiteSetInputDto();
                allowEditAvatarPicture.setKey = "allow_edit_avatar_picture";
                allowEditAvatarPicture.setValue = siteDto.allowEditAvatarPicture;
                inputDtoList.Add(allowEditAvatarPicture);

                SiteSetInputDto invitationCodeSupport = new SiteSetInputDto();
                invitationCodeSupport.setKey = "invitation_code_support";
                invitationCodeSupport.setValue = siteDto.invitationCodeSupport;
                inputDtoList.Add(invitationCodeSupport);

                SiteSetInputDto invitationCodeNeeded = new SiteSetInputDto();
                invitationCodeNeeded.setKey = "invitation_code_needed";
                invitationCodeNeeded.setValue = siteDto.invitationCodeNeeded;
                inputDtoList.Add(invitationCodeNeeded);

                SiteSetInputDto invitationCodeAllowedShow = new SiteSetInputDto();
                invitationCodeAllowedShow.setKey = "invitation_code_allowed_show";
                invitationCodeAllowedShow.setValue = siteDto.invitationCodeAllowedShow;
                inputDtoList.Add(invitationCodeAllowedShow);

                //SiteSetInputDto registerUserTemrs = new SiteSetInputDto();
                //userRegisterDisplay.setKey = "register_usertemrs";
                //userRegisterDisplay.setValue = siteDto.registerUserTemrs;
                //inputDtoList.Add(registerUserTemrs);

                SiteSetInputDto registerUserTemrs = new SiteSetInputDto();
                registerUserTemrs.setKey = "register_userterms";
                registerUserTemrs.setValue = Server.UrlDecode(Request.Params["editorValue"]);
                inputDtoList.Add(registerUserTemrs);


                SiteSetInputDto isSendEmail = new SiteSetInputDto();
                isSendEmail.setKey = "is_send_email";
                isSendEmail.setValue = siteDto.isSendEmail;
                inputDtoList.Add(isSendEmail);

                SiteSetInputDto mailer_host = new SiteSetInputDto();
                mailer_host.setKey = "mailer_host";
                mailer_host.setValue = siteDto.mailer_host;
                inputDtoList.Add(mailer_host);

                SiteSetInputDto mailer_hostPort = new SiteSetInputDto();
                mailer_hostPort.setKey = "mailer_hostPort";
                mailer_hostPort.setValue = siteDto.mailer_hostPort;
                inputDtoList.Add(mailer_hostPort);

                SiteSetInputDto mailer_username = new SiteSetInputDto();
                mailer_username.setKey = "mailer_username";
                mailer_username.setValue = siteDto.mailer_username;
                inputDtoList.Add(mailer_username);

                SiteSetInputDto mailer_password = new SiteSetInputDto();
                mailer_password.setKey = "mailer_password";
                mailer_password.setValue = siteDto.mailer_password;
                inputDtoList.Add(mailer_password);

                SiteSetInputDto mailer_from = new SiteSetInputDto();
                mailer_from.setKey = "mailer_from";
                mailer_from.setValue = siteDto.mailer_from;
                inputDtoList.Add(mailer_from);

                SiteSetInputDto mailer_name = new SiteSetInputDto();
                mailer_name.setKey = "mailer_name";
                mailer_name.setValue = siteDto.mailer_name;
                inputDtoList.Add(mailer_name);

                SiteSetInputDto allowPasteCode = new SiteSetInputDto();
                allowPasteCode.setKey = "allow_paste_code";
                allowPasteCode.setValue = siteDto.allowPasteCode;
                inputDtoList.Add(allowPasteCode);
                SiteSetInputDto labelPoint = new SiteSetInputDto();
                labelPoint.setKey = "lable_point";
                labelPoint.setValue = siteDto.labelPoint;
                inputDtoList.Add(labelPoint);
                SiteSetInputDto labelDeductPoint = new SiteSetInputDto();
                labelDeductPoint.setKey = "lable_deduct_point";
                labelDeductPoint.setValue = siteDto.labelDeductPoint;
                inputDtoList.Add(labelDeductPoint);
                SiteSetInputDto maxPointRate = new SiteSetInputDto();
                maxPointRate.setKey = "max_point_rate";
                maxPointRate.setValue = siteDto.maxPointRate;
                inputDtoList.Add(maxPointRate);
                _iSiteSetService.ModifySiteSet(inputDtoList);
                CacheStrategy.Remove("SiteSet");
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
            }
            return Json("ok");
        }

        [HttpPost]
        public JsonResult ModifySenateConfigSet(SiteSetDto siteDto)
        {
            try
            {
                List<SiteSetInputDto> inputDtoList = new List<SiteSetInputDto>();

                SiteSetInputDto courseBuyFillUserInfo = new SiteSetInputDto();
                courseBuyFillUserInfo.setKey = "course_buy_fill_userinfo";
                courseBuyFillUserInfo.setValue = siteDto.courseBuyFillUserInfo;
                inputDtoList.Add(courseBuyFillUserInfo);

                SiteSetInputDto courseTeacherModifyPrice = new SiteSetInputDto();
                courseTeacherModifyPrice.setKey = "course_teacher_modify_price";
                courseTeacherModifyPrice.setValue = siteDto.courseTeacherModifyPrice;
                inputDtoList.Add(courseTeacherModifyPrice);

                _iSiteSetService.ModifySiteSet(inputDtoList);

            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
            }
            return Json("ok");
        }

        #endregion

    

        #region system log

        public JsonResult GetAllSysLog()
        {
            EasyUiListResultDto<SystemLogDto> result = new EasyUiListResultDto<SystemLogDto>();
            try
            {
                var provider = new EasyUIProvider();
                var pager = provider.GetPager(Request);
                var filter = new SystemLogInputDto();
                if (!string.IsNullOrEmpty(pager.Filter))
                {
                    filter = provider.DeserializeObject<SystemLogInputDto>(pager.Filter);
                }

                filter.PageSize = pager.PageSize;
                filter.CurrentPage = pager.CurrentPage - 1;
                filter.SortOrder = pager.SortOrder;
                filter.SortCloumnName = pager.SortCloumnName;

                filter.level = Request.Params["sysLog_level"];
                filter.startTime = Request.Params["sysLog_startTime"];

                DateTime dt = DateTime.Now;
                if (!DateTime.TryParse(filter.startTime, out dt))
                {
                    filter.startTime = "";
                }

                filter.endTime = Request.Params["sysLog_endTime"];
                if (!DateTime.TryParse(filter.endTime, out dt))
                {
                    filter.endTime = "";
                }

                filter.module = Request.Params["sysLog_module"];
                filter.operateName = Request.Params["sysLog_opt"];
                filter.userName = Request.Params["sysLog_name"];

                result = _iSystemLogService.GetAllServiceSet(filter);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetErrorLogList(int pageIndex, int pageSize)
        {
            EasyUiListResultDto<SystemLogDto> result = new EasyUiListResultDto<SystemLogDto>();
            try
            {
                var provider = new EasyUIProvider();
                var pager = provider.GetPager(Request);
                var filter = new SystemLogInputDto();
                if (!string.IsNullOrEmpty(pager.Filter))
                {
                    filter = provider.DeserializeObject<SystemLogInputDto>(pager.Filter);
                }

                filter.PageSize = pageSize; //pager.PageSize;
                filter.CurrentPage = pageIndex > 0 ? pageIndex - 1 : pageIndex;
                filter.SortOrder = pager.SortOrder;
                filter.SortCloumnName = pager.SortCloumnName;
                result = _iSystemLogService.GetAllServiceSet(filter);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  menu

        public JsonResult GetAllMenu()
        {
            EasyUiListResultDto<MenuDto> result = new EasyUiListResultDto<MenuDto>();
            try
            {
                var provider = new EasyUIProvider();
                var pager = provider.GetPager(Request);
                var filter = new EasyuiDto();
                if (!string.IsNullOrEmpty(pager.Filter))
                {
                    filter = provider.DeserializeObject<MenuInputDto>(pager.Filter);
                }

                filter.PageSize = pager.PageSize;
                filter.CurrentPage = pager.CurrentPage - 1;
                filter.SortOrder = pager.SortOrder;
                filter.SortCloumnName = pager.SortCloumnName;
                result = _iMenuService.GetAllMenu(filter);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModifyMenu(MenuInputDto input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.id))
                {
                    return Json("faile");
                }

                _iMenuService.ModifyMenu(input);
                return Json(new {code="ok"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {code="faile", msg=ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult InsertMenu(MenuInputDto input)
        {
            try
            {
                _iMenuService.InsertMenu(input);

                return Json(new { code = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = "faile", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTreeMenu()
        {
            var dtoList = new List<MenuDto>();
            try
            {
                dtoList = _iMenuService.TreeMenu();
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
            }

            return Json(dtoList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region   RoleManage

        public JsonResult GetAllRoleManage()
        {
            EasyUiListResultDto<RoleManageDto> result = new EasyUiListResultDto<RoleManageDto>();
            try
            {
                var provider = new EasyUIProvider();
                var pager = provider.GetPager(Request);
                var filter = new RoleManageInputDto();
                if (!string.IsNullOrEmpty(pager.Filter))
                {
                    filter = provider.DeserializeObject<RoleManageInputDto>(pager.Filter);
                }

                filter.PageSize = pager.PageSize;
                filter.CurrentPage = pager.CurrentPage - 1;
                filter.SortOrder = pager.SortOrder;
                filter.SortCloumnName = pager.SortCloumnName;

                filter.roleName = Request.Params["roleName"];
                filter.roleGroup = Request.Params["roleGroup"];
                result = _iRoleManageService.GetAllRoleManageUiList(filter);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModifyRoleManage()
        {
            try
            {
                string id = Request.Params["id"];
                string roleName = Request.Params["roleName"];
                string roleCode = Request.Params["roleCode"];
                string description = Request.Params["description"];
                string permissionId = Request.Params["permissionId"];
                string roleGroup = Request.Params["roleGroup"];
                RoleManageInputDto input = new RoleManageInputDto();
                input.id = id;
                input.roleCode = roleCode;
                input.roleGroup = roleGroup;
                input.roleName = roleName;
                input.description = description;
                input.permissionId = permissionId;
                _iRoleManageService.ModifyRoleManage(input);
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
                return Json("faile", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult InsertRoleManage()
        {
            try
            {
                string roleName = Request.Params["roleName"];
                string roleCode = Request.Params["roleCode"];
                string description = Request.Params["description"];
                string permissionId = Request.Params["permissionId"];
                string roleGroup = Request.Params["roleGroup"];
                RoleManageInputDto input = new RoleManageInputDto();
                input.roleCode = roleCode;
                input.roleName = roleName;
                input.roleGroup = roleGroup;
                input.description = description;
                input.permissionId = permissionId;
                _iRoleManageService.InsertRoleManage(input);

                return Json("ok", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
                return Json("faile", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPermissionTree(string id="")
        {
            try
            {

                List<RoleMenuModel> list = _iRoleManageService.GetPermissionTree(id);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
                return Json("faile");
            }
        }

        public ActionResult RoleUserConfig(string identity,string roleId)
        {
            ViewBag.identity = identity; //角色类型
            ViewBag.roleId = roleId; //角色Id
            return View();
        }

        #endregion

        #region file
        public JsonResult UploadLogoFile()
        {
            try
            {
                HttpPostedFileBase postFile = Request.Files["logoFile"];
                string url = SaveSysFile(postFile);
                return Json(new { msg = "ok", url = url }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
                return Json("faile", JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult UploadFaviconFile()
        {
            try
            {
                HttpPostedFileBase postFile = Request.Files["faviconFile"];
                string url = SaveSysFile(postFile);
                return Json(new { msg = "ok", url = url }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
                return Json("faile", JsonRequestBehavior.AllowGet);
            }
        }

        public string SaveSysFile(HttpPostedFileBase file)
        {
            string path = string.Empty;
            try
            {
                string rootPath = System.Web.HttpContext.Current.Server.MapPath("~/");
                string tempDir = Path.Combine(rootPath, "files", "syssetting");
                if (!System.IO.Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                if (file != null)
                {
                    string name = Guid.NewGuid() + "_" + file.FileName;
                    string filePath = Path.Combine(tempDir, name);
                    file.SaveAs(filePath);
                    // path = "/files/syssetting/" + name;
                    string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
                    string fileUrl = filePath.Replace(tmpRootDir, ""); //转换成相对路径
                    fileUrl = fileUrl.Replace(@"\", @"/");
                    path = "/" + fileUrl;
                }
            }
            catch (Exception ex)
            {
                _iLogger.Error(ex.ToString());
            }
            return path;
        }
        #endregion

        #region Cloud

        public JsonResult GetProvince()
        {
            return this.Json(_iCloudService.GetProvince(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCity(int id)
        {
            return this.Json(_iCloudService.GetCity(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cloud()
        {
            var cloud = _iCloudService.GetCloud();
            return View(cloud);
        }
        public async Task<JsonResult> CloudRegister(CloudDto input)
        {
            var cloud = await _iCloudService.CreateOrUpdateCloud(input);
            var obj = new
            {
                isSuccess = true,
                cloud.AccessKey,
                cloud.SecretKey

            };
            //return View("Cloud", cloud);
            return Json(obj);
        }
        #endregion

    }
}