using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPOC.Web.Filters;

namespace SPOC.Web.Controllers
{
    [UserAuthorization]
    public class TutorialController : SPOCControllerBase
    {
        // GET: Tutorial 默认C++教程
        public ActionResult Index()
        {
            //根据语言配置判断是否可以访问教程
            if (!L("Language").Equals("cpp"))
                return RedirectToAction("Error", "Home");
            return View();
        }
        public ActionResult C()
        {
            if (!L("Language").Equals("c"))
                return RedirectToAction("Error", "Home");
            return View();
        }
        public ActionResult Java()
        {
            if (!L("Language").Equals("java"))
                return RedirectToAction("Error", "Home");
            return View();
        }
        public ActionResult Python()
        {
            if (!L("Language").Equals("python"))
                return RedirectToAction("Error", "Home");
            return View();
        }
        public ActionResult Python3()
        {
            if (!L("Language").Equals("python3"))
               return RedirectToAction("Error", "Home");
            return View();
        }
    }
}