using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Abp.Web.Models;
using SPOC.Common.EasyUI;
using SPOC.SysSetting;
using SPOC.User;
using SPOC.User.Dto.Admin;
using SPOC.Web.Controllers;
using SPOC.Web.Filters;

namespace SPOC.Web.Areas.User.Controllers
{
    [DontWrapResult]
    [UserAuthorization]
    public class AdminController : SPOCControllerBase
    {
        private readonly IAdminInfoService _adminGuidService;

        private readonly IRoleManageService _roleManageService;

        private readonly IUserInfoService _userInfoService;

        private readonly IUserInfoApiService _iUserInfoApiService;

        public AdminController(IRoleManageService roleManageService, IAdminInfoService adminGuidService,  IUserInfoService iUserInfoService, IUserInfoApiService iUserInfoApiService)
            //public AdminController(IAdminInfoService adminGuidService )
        {
            _adminGuidService = adminGuidService;
           _roleManageService = roleManageService;
           _userInfoService = iUserInfoService;
           _iUserInfoApiService = iUserInfoApiService;
        }



        // GET: User/Admin
        public ActionResult Index()
        {
            ViewBag.usernameContainEN = BaseSiteSetDto.usernameContainEN;
            return View();
        }

        public JsonResult Get() {
            var provider = new EasyUIProvider();
            var pager = provider.GetPager(Request);
            var filter = new AdminInfoInputDto();
            if (!string.IsNullOrEmpty(pager.Filter))
            {
                filter = provider.DeserializeObject<AdminInfoInputDto>(pager.Filter);
            }
            filter.PageSize = pager.PageSize;
            filter.CurrentPage = pager.CurrentPage - 1;
            filter.SortOrder = pager.SortOrder;
            filter.SortCloumnName = pager.SortCloumnName;
            var grid = _adminGuidService.GetAdminInfoByGuid(filter);
            
            return Json(grid);
        }

 

        public JsonResult AdminNameCheck(string type, string checkname, string oldName = "")
        {
            return Json(_adminGuidService.CheckNameExit(checkname, type, oldName), JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <returns></returns>
        public ActionResult Recovery()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            try
            {
                string userName = Request.Params["user_login_name"].ToString().Trim();
                string mobile = Request.Params["user_mobile"].ToString().Trim();
                string email = Request.Params["user_email"].ToString().Trim();
                var result = _iUserInfoApiService.UserSearch(null, userName, email, mobile);
                if (result.Result.IsSuccess)
                {
                    UserBase model = new UserBase();
                    model.userLoginName = result.Result.Data.InKey.loginname;
                    model.userFullName = result.Result.Data.InKey.truename;
                    model.userMobile = result.Result.Data.InKey.mobile;
                    model.userEmail = result.Result.Data.InKey.email;
                    model.newMoocUserId = string.IsNullOrEmpty(result.Result.Data.InKey.id) ? Guid.Empty : Guid.Parse(result.Result.Data.InKey.id);
                    model.userGender = result.Result.Data.InKey.gender;
                    model.userPassWord = "6ED5833CF35286EBF8662B7B5949F0D742BBEC3F";
                    model.Id = Guid.NewGuid();
                    model.identity = 3;
                    model.userEnbleFlag = false;

                    CreateAdminInfoInputDto techerDto = new CreateAdminInfoInputDto()
                    {
                        userId = model.Id
                    };
                    _adminGuidService.RecoverInsertAdmin(techerDto);
                    _userInfoService.RecoverInsertUsers(model);
                    list.Add("msg", "ok");
                }
                else
                {
                    list.Add("msg", "notExist");
                    list.Add("ErrMsg", result.Result.ErrMsg);
                    //msg = result.Result.ErrMsg;
                }

            }
            catch (Exception ex)
            {
                list.Add("msg", "fiale");
                //iLogger.Error(ex.ToString());
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}