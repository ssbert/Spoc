using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.ExamPaper;
using SPOC.ExamPaper.Dto;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPOC.Web.Areas.ExamPaper.Controllers
{
    public class ManageController : AsyncController
    {
        private readonly IExamPaperService _iExamPaperService;
        public ManageController(IExamPaperService iExamPaperService)
        {
            _iExamPaperService = iExamPaperService;
        }
        // GET: ExamPaper/Manager
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(string id, string paperTypeCode)
        {
            if (paperTypeCode == "random")
            {
                return View("EditRandomV2");
            }
            return View("EditV2");
        }

        public async Task<ActionResult> Preview(string paperUid, string policyUid, string examUid, string policyNodeUid)
        {
            var inputDto = new PaperPreviewInputDto
            {
                examUid = string.IsNullOrEmpty(examUid) ? Guid.Empty : new Guid(examUid),
                paperUid = string.IsNullOrEmpty(paperUid) ? Guid.Empty : new Guid(paperUid),
                policyNodeUid = string.IsNullOrEmpty(policyNodeUid) ? Guid.Empty : new Guid(policyNodeUid),
                policyUid = string.IsNullOrEmpty(policyUid) ? Guid.Empty : new Guid(policyUid)
            };
            var outputDto = await _iExamPaperService.GetPaperPreview(inputDto);
            return await Task.FromResult(View(outputDto));
        }

        public async Task<ActionResult> PreviewByExamId(Guid examId)
        {
            var outputDto = await _iExamPaperService.GetExamPreview(examId);
            return await Task.FromResult(View("Preview", outputDto));
        }

        public ActionResult CreateFromFile(HttpPostedFileBase file, Guid questionFolderUid, Guid paperFolderUid)
        {
            try
            {
                var index = file.FileName.LastIndexOf('.');
                var extName = file.FileName.Substring(index + 1);
                ImportResultOutputDto result = new ImportResultOutputDto();
                if (extName == "doc" || extName == "docx")
                {
                    result = _iExamPaperService.CreateFromFile(file.InputStream, questionFolderUid, paperFolderUid);
                }
               
                return Json(new { Success = true, Result = result }, JsonRequestBehavior.AllowGet);
            }
            catch (UserFriendlyException e)
            {
                return Json(new { Success = false, Error = new { e.Message } }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                file.InputStream.Close();
                file.InputStream.Dispose();
            }
        }

        public async Task<ActionResult> ExportToWord(Guid id)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                return RedirectToAction("Index", "Home");
            }
            await _iExamPaperService.ExportToWord(id);
            var fileName = cookie.Id + "_" + id + ".doc";
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fileroot", "CacheFile", "ExportPaper", fileName);
            var mime = MimeMapping.GetMimeMapping(fileName);
            if (!System.IO.File.Exists(path))
            {
                return RedirectToAction("Error", "Home");
            }
            return File(path, mime, "试卷_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc");
        }

        public async Task ExportPaperPreviewHtml(Guid id)
        {
            var html = await _iExamPaperService.ExportPaperPreviewHtml(id);
            var response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.Buffer = false;
            response.Charset = "utf-8";
            response.AppendHeader("Content-Disposition", "attachment;filename=ExportManulPaper" + DateTime.Now.Ticks + ".doc");
            response.ContentEncoding = Encoding.GetEncoding("utf-8");//设置输出流为简体中文   
            response.HeaderEncoding = Encoding.GetEncoding("utf-8");//设置输出流为简体中文   
            response.ContentType = "application/ms-word";
            response.Write("<meta http-equiv=Content-Type content=text/html;charset=UTF-8>");
            response.Write(html);
            response.Flush();
            response.End();
        }
    }
}