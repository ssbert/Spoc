using System.Web.Mvc;
using SPOC.Web.Filters;

namespace SPOC.Web.Areas.User.Controllers
{
    [UserAuthorization]
    public class RoleController : Controller
    {
        // GET: User/Role
        public ActionResult Index()
        {
            return View();
        }
    }
}