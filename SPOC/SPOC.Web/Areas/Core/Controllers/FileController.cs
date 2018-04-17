using SPOC.Common.File;
using SPOC.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPOC.Web.Areas.Core.Controllers
{
    /// <summary>
    /// 文件上传控制器
    /// </summary>
    public class FileController : Controller
    {
        private readonly IUploadFileService _iUploadFileService;
        public FileController(IUploadFileService iUploadFileService)
        {
            _iUploadFileService = iUploadFileService;
        }
        // GET: Core/Upload
        public JsonResult Upload(HttpPostedFileBase file)
        {
            var id = Guid.NewGuid().ToString();
            UploadHelper.CreateTempFile(id, file.InputStream);
            return Json(new { id });
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FileResult> Download(Guid id)
        {
            var uploadFile = await _iUploadFileService.Get(id);
            var mime = MimeMapping.GetMimeMapping(uploadFile.FileName);
            var path = Path.Combine(UploadHelper.UploadDirPath, uploadFile.Id.ToString());
            var fs = System.IO.File.OpenRead(path);
            return File(fs, mime, uploadFile.FileName);
        }
    }
}