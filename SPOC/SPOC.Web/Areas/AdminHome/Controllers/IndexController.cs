using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPOC.Common.Cookie;
using SPOC.SysSetting;
using SPOC.SystemSet;
using SPOC.User;
using SPOC.Web.Filters;

namespace SPOC.Web.Areas.AdminHome.Controllers
{
    [UserAuthorization]
    public class IndexController : Controller
    {
     
        // GET: AdminHome/Home
        [Filters.UserAuthorization]
        public ActionResult Index()
        {
            var user = CookieHelper.GetLoginInUserInfo();
            //如果是学生访问后台 强制其退出系统
            if(user.Identity==1)
                return RedirectToAction("LoginOut", "Account", new { area = "" });
            return View();
        }
    }
}