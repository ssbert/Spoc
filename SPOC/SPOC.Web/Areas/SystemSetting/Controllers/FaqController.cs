using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPOC.Common.Helper;

namespace SPOC.Web.Areas.SystemSetting.Controllers
{
    public class FaqController : Controller
    {
        // GET: SystemSetting/Faq
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit(string id)
        {
            ViewBag.id = id;
            return View();
        }
    }
}