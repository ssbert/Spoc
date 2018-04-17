using System;
using System.IO;
using Abp.UI;

namespace SPOC.Common.File
{
    /// <summary>
    /// 上传
    /// </summary>
    public class UploadHelper
    {
        private static string _tempDirPath = "";
        private static string _uploadDirPath = "";

        /// <summary>
        /// 上传文件临时目录
        /// </summary>
        public static string TempDirPath
        {
            get
            {
                if (string.IsNullOrEmpty(_tempDirPath))
                {
                    _tempDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "uploadTemp");
                    CreateDir(_tempDirPath);
                }
                return _tempDirPath;
            }
        }

        /// <summary>
        /// 上传文件正式目录
        /// </summary>
        public static string UploadDirPath
        {
            get
            {
                if (string.IsNullOrEmpty(_uploadDirPath))
                {
                    _uploadDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "upload");
                    CreateDir(_uploadDirPath);
                }
                return _uploadDirPath;
            }
        }

        /// <summary>
        /// 创建临时文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        public static void CreateTempFile(string name, Stream stream)
        {
            var path = TempDirPath;
            CleanTempFile(path);
            var filePath = Path.Combine(path, name);
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Close();
                stream.Close();
                stream.Dispose();
            }

        }

        /// <summary>
        /// 清理创建时间大于1小时的临时文件
        /// </summary>
        /// <param name="path"></param>
        public static void CleanTempFile(string path)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles();
            foreach (var fileInfo in files)
            {
                if ((DateTime.Now - fileInfo.CreationTime).TotalHours > 1)
                {
                    fileInfo.Delete();
                }
            }
        }

        /// <summary>
        /// 若文件夹路径不存在则创建
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDir(string path)
        {
            try
            {
                var dirs = path.Split('\\');
                var newPath = "";

                foreach (var dir in dirs)
                {
                    newPath = Path.Combine(newPath, dir);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="tempFileName">临时文件名</param>
        /// <param name="fileName">正式文件名</param>
        public static void CreateFile(string tempFileName, string fileName)
        {
            var tempFilePath = Path.Combine(TempDirPath, tempFileName);
            if (!System.IO.File.Exists(tempFilePath))
            {
                throw new UserFriendlyException("无效的临时文件");
            }
            var uploadFilePath = Path.Combine(UploadDirPath, fileName);
            System.IO.File.Copy(tempFilePath, uploadFilePath);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void DeleteFile(string fileName)
        {
            var uploadFilePath = Path.Combine(UploadDirPath, fileName);
            if (!System.IO.File.Exists(uploadFilePath))
            {
                System.IO.File.Delete(uploadFilePath);
            }
        }
    }
}