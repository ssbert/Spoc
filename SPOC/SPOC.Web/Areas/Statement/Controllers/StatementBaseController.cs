using System;
using System.IO;
using System.Web.Mvc;
using Abp.UI;
using NPOI;
using SPOC.Web.Controllers;

namespace SPOC.Web.Areas.Statement.Controllers
{
    public class StatementBaseController : SPOCControllerBase
    {
        /// <summary>
        /// 检测目录是否存在，不存在则创建
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <param name="id"></param>
        /// <param name="workbook"></param>
        protected static void CreateStatementTempFile(string path, Guid id, POIXMLDocument workbook)
        {
            var dirs = path.Split('\\');
            var newPath = dirs[0];

            for (var i = 1; i < dirs.Length; i++)
            {
                newPath = Path.Combine(newPath, dirs[i]);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            //清理创建时间大于1小时的
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles();
            foreach (var fileInfo in files)
            {
                if ((DateTime.Now - fileInfo.CreationTime).TotalHours > 1)
                {
                    fileInfo.Delete();
                }
            }
            var filePath = Path.Combine(path, id + ".xlsx");
            var fs = System.IO.File.Create(filePath);
            workbook.Write(fs);
            fs.Close();
        }

        #region 下载导出文件

        public FileResult Download(Guid id, string fileName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement", id + ".xlsx");
            if (!System.IO.File.Exists(path))
            {
                throw new UserFriendlyException("无效的文件");
            }
            var fs = System.IO.File.OpenRead(path);
            return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }

        #endregion
    }
}