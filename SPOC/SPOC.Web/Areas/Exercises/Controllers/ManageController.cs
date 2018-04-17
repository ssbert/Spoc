using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPOC.Web.Areas.Exercises.Controllers
{
    public class ManageController : Controller
    {
        // GET: Exercises/Manage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditTask()
        {
            return View();
        }
    }
}