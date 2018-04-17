using SPOC.Web.Controllers;
using System.Web.Mvc;

namespace SPOC.Web.Areas.Lib.Controllers
{
    public class ManageController : SPOCControllerBase
    {
        // GET: Lib/Manage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StructureEditor()
        {
            ViewBag.language = L("Language");
            return View();
        }
        
        public ActionResult EditLabel()
        {
            return View();
        }

        public ActionResult StructureMap()
        {
            return View();
        }
    }
}