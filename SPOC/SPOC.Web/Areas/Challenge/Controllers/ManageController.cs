using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.Extensions;
using SPOC.Core;
using SPOC.Exam;
using SPOC.QuestionBank;
using SPOC.QuestionBank.Const;
using SPOC.QuestionBank.Dto;
using SPOC.Web.Areas.QuestionBank.Models;
using AutoMapExtensions = SPOC.Common.Extensions.AutoMapExtensions;

namespace SPOC.Web.Areas.Challenge.Controllers
{

    public class ManageController : AsyncController
    {
        private readonly IChallengeQuestionService _iChallengeQuestionService;
        public ManageController(IChallengeQuestionService iChallengeQuestionService)
        {
            _iChallengeQuestionService = iChallengeQuestionService;
        }
        // GET: QuestionBank/Manage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public async Task<ActionResult> Preview(string id)
        {
            var question = await _iChallengeQuestionService.Get(id);

            var model = new QuestionPreviewModel(question.CopyModel<ExamQuestionDto>());
            if (question.questionBaseTypeCode == QuestionTypeConst.Compose)
            {
                var children = await _iChallengeQuestionService.GetChildren(question.Id.ToString());
                if (children.Count > 0)
                {
                    children.ForEach(a=>model.Children.Add(new QuestionPreviewModel(a.CopyModel<ExamQuestionDto>())));
                }
            }

            return await Task.FromResult(View(model));
        }

        public ActionResult CreateFromFile(HttpPostedFileBase file,Guid folderUid)
        {
            try
            {
                var index = file.FileName.LastIndexOf('.');
                var extName = file.FileName.Substring(index + 1);
                ImportResultOutputDto result = new ImportResultOutputDto();
                if (extName == "doc" || extName == "docx")
                {
                    result = _iChallengeQuestionService.CreateFromFile(file.InputStream, folderUid, "word");
                }
                else if (extName == "xls" || extName == "xlsx")
                {
                    result = _iChallengeQuestionService.CreateFromFile(file.InputStream, folderUid, "excel");
                }
                return Json(new {Success = true, Result = result}, JsonRequestBehavior.AllowGet);
            }
            catch (UserFriendlyException e)
            {
                return Json(new {Success = false, Error = new {e.Message}}, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                file.InputStream.Close();
                file.InputStream.Dispose();
            }
        }

        public ActionResult ExportToWord(Guid id)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                return RedirectToAction("Index", "Home");
            }
            var fileName = cookie.Id + "_" + id + ".doc";
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fileroot", "CacheFile", "ExportQuestion", fileName);
            var mime = MimeMapping.GetMimeMapping(fileName);
            if (!System.IO.File.Exists(path))
            {
                return RedirectToAction("Error", "Home");
            }
            return File(path, mime, "挑战题_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc");
        }
    }
}