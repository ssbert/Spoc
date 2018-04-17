using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPOC.Web.Filters;

namespace SPOC.Web.Areas.User.Controllers
{
    [UserAuthorization]
    public class DepartmentController : Controller
    {
        // GET: User/Department/Faculty
        public ActionResult Faculty()
        {
            return View();
        }
        public ActionResult Major()
        {
            return View();
        }
        public ActionResult Class()
        {
            return View();
        }
        public ActionResult AdministrativeClass()
        {
            return View();
        }
        public ActionResult TeacherSetting(string id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult AddTeachers(string id)
        {
            ViewBag.relationId = id;
            return View();
        }
    }
}