using System.Web.Mvc;

namespace SPOC.Web.Areas.Category.Controllers
{
    public class ManageController : Controller
    {
        // GET: Category/Manager
        public ActionResult Index(string code)
        {
            ViewBag.code = code;
            return View();
        }

        public ActionResult CategoryType()
        {
            return View();
        }
    }
}