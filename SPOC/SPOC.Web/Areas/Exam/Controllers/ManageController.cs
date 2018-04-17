using SPOC.ExamPaper;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SPOC.Common.Cookie;
using SPOC.Common.Helper;
using SPOC.Exam;
using SPOC.Web.Filters;

namespace SPOC.Web.Areas.Exam.Controllers
{
    public class ManageController : AsyncController
    {
        private readonly IExamPaperService _iExamPaperService;
        private readonly IExamExamService _iExamExamService;
        public ManageController(IExamPaperService iExamPaperService, IExamExamService iExamExamService)
        {
            _iExamPaperService = iExamPaperService;
            _iExamExamService = iExamExamService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditTask()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult GradeManage()
        {
            return View();
        }

        public ActionResult UserGrade()
        {
            return View();
        }

        public ActionResult Judge()
        {
            return View();
        }

        [UserAuthorization]
        public async Task<ActionResult> UserExamPreview(string examGradeUid, string filterType)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var examGradeId = examGradeUid.TryParseGuid();
            if (!cookie.IsAdmin && !await _iExamExamService.CheckShowUserExamPreview(examGradeId))
            {
                return RedirectToAction("Index", "ExamTask");
            }
            
            var outputDto = await _iExamPaperService.GetUserPaperView(new Guid(examGradeUid), filterType);

            ViewBag.filterType = filterType;
            return await Task.FromResult(View(outputDto));
        }

        [UserAuthorization]
        public async Task<ActionResult> JudgeExam(string examGradeUid, string filterType)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var examGradeId = examGradeUid.TryParseGuid();
            if (!cookie.IsAdmin && !await _iExamExamService.CheckShowUserExamPreview(examGradeId))
            {
                return RedirectToAction("Index", "ExamTask");
            }

            var outputDto = await _iExamPaperService.GetUserPaperView(new Guid(examGradeUid), filterType, true);

            ViewBag.filterType = filterType;
            return await Task.FromResult(View(outputDto));
        }
    }
}