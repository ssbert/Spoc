using System.Web.Mvc;

namespace SPOC.Web.Areas.Exam.Controllers
{
    public class ComponentController : Controller
    {
        // GET: Exam/Component
        public ActionResult ExamQuestionSelector()
        {
            return View();
        }

        public ActionResult ExamPaperSelector()
        {
            return View();
        }

        public ActionResult QuestionSingleSelector()
        {
            return View();
        }
    }
}