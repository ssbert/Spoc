using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SPOC.Common.File
{
    public class ImageUploadHelper
    {
        //   public static int DefaultHeight;//压缩图的高度


        /// <summary>
        /// 图片上传，返回上传图片的路径
        /// </summary>
        /// <param name="fileNamePath">上传文件的路径</param>
        /// <param name="files">文件</param>
        /// <param name="DefaultHeight">上传文件压缩图的高度</param>
        ///  <param name="savePath">返回的源图片的保存路径</param>
        ///  <param name="folderName">縮略圖新增文件夾的名稱</param>
        ///  <param name="checlFileSize">上传图片限制的最大值（单位KB）</param>
        /// <returns></returns>
        public string UpLoadImage(string serverPath, string fileNamePath, HttpPostedFileBase files, int DefaultHeight, string toPath, string savePath, string folderName, int checlFileSize, int checkImageHigh, int checkImageWidth)
        {


            //   string toFilePath = Path.Combine(serverPath, @"Content\images\ItemMean\");
            string toFilePath = Path.Combine(serverPath, toPath);
            //获取要保存的文件信息
            FileInfo file = new FileInfo(fileNamePath);
            //获得文件扩展名
            string fileNameExt = file.Extension;

            //验证合法的文件
            if (CheckImageExt(fileNameExt, files, checlFileSize, checkImageHigh, checkImageWidth))
            {
                //生成将要保存的随机文件名
                string fileName = GetImageName() + fileNameExt;

                //获得要保存的文件路径
                string serverFileName = toFilePath + fileName;
                //物理完整路径                    
                string toFileFullPath = serverFileName; //HttpContext.Current.Server.MapPath(toFilePath);

                //将要保存的完整文件名                
                string toFile = toFileFullPath;//+ fileName;

                ///创建WebClient实例       
                WebClient myWebClient = new WebClient();
                //设定windows网络安全认证   方法1
                myWebClient.Credentials = CredentialCache.DefaultCredentials;
                ////设定windows网络安全认证   方法2
                files.SaveAs(toFile);


                //上传成功后网站内源图片相对路径
                string relativePath = System.Web.HttpContext.Current.Request.ApplicationPath
                                      + string.Format(savePath + "{0}", fileName);

                /*
                  比例处理
                  微缩图高度（DefaultHeight属性值为 400）
                */

                System.Drawing.Image img = System.Drawing.Image.FromFile(toFile);
                int width = img.Width;
                int height = img.Height;
                float ratio = (float)width / height;

                //微缩图高度和宽度
                int newHeight = height <= DefaultHeight ? height : DefaultHeight;
                int newWidth = height <= DefaultHeight ? width : Convert.ToInt32(DefaultHeight * ratio);

                FileInfo generatedfile = new FileInfo(toFile);
                //  string tempName = generatedfile.Name.Split('.')[0] + "_Thumb";
                string tempName = generatedfile.Name.Split('.')[0].ToString();

                string newFileName = tempName + "." + generatedfile.Name.Split('.')[1].ToString();

                string addFolderNametoSavePath = folderName == "" ? "" : @"\" + folderName;

                string deskPath = generatedfile.DirectoryName + addFolderNametoSavePath;

                if (!Directory.Exists(deskPath))
                {
                    Directory.CreateDirectory(deskPath);
                }

                string newFilePath = Path.Combine(generatedfile.DirectoryName + addFolderNametoSavePath, newFileName);

                PictureHandler.CreateThumbnailPicture(toFile, newFilePath, newWidth, newHeight);
                string addFolderName = folderName == "" ? "" : folderName + "/";
                string thumbSavePath = savePath + addFolderName;

                //string thumbRelativePath = System.Web.HttpContext.Current.Request.ApplicationPath
                //                      + string.Format(savePath + "{0}", newFileName);
                string thumbRelativePath = System.Web.HttpContext.Current.Request.ApplicationPath
                                      + string.Format(thumbSavePath + "{0}", newFileName);

                //返回原图和微缩图的网站相对路径

                relativePath = string.Format("{0},{1}", relativePath, thumbRelativePath);
                // int id = FilePathAd(relativePath);
                int id = 1;
                string returnPath = string.Format("{0},{1},{2}", relativePath, thumbRelativePath, id);
                return returnPath;

            }
            else
            {
                return "文件格式非法，请上传格式为gif,jpg ,大小为" + checkImageHigh + "*" + checkImageWidth + "且不超过" + checlFileSize + "k的图片。";
            }



            //  return "";
        }
        /// <summary>
        /// 移动端API上传图片
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static ReturnValue MobileUploadImage(FileStream fileStream)
        {
            var retValue = new ReturnValue();
            try
            {
                var file = Image.FromStream(fileStream);
                const string fileExt = "jpg"; //文件扩展名，不含“.”
                string newFileName = Guid.NewGuid() + "." + fileExt; //随机生成新的文件名
                string newThumbnailFileName = "thumb_" + newFileName; //随机生成缩略图文件名
                string upLoadPath = "/files/chat/images/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";//上传原图目录相对路径
                var rootPath = HttpContext.Current.Server.MapPath(upLoadPath);// HttpContext.Current.Request.PhysicalApplicationPath.Replace("\\", "/").TrimEnd('/').ToLower();
                //是否存在存放缩略图和原图的文件夹 没有则创建
                FilePathUtil.CreateDirectory(rootPath);
             
                #region 保存文件
                file.Save(rootPath+ newFileName);
                #endregion
               
                #region 图片剪裁
                PictureHandler.CreateThumbnailPicture(rootPath + newFileName, rootPath + newThumbnailFileName, 100, 100);
                #endregion
     
                retValue.Message = "操作成功";
                retValue.PutValue("imgUrl", AppConfiguration.FileServerWebRootPath + "/files/chat/images/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + newThumbnailFileName);
                //处理完毕，返回JOSN格式的文件信息
                return retValue;
            }
            catch
            {
                return new ReturnValue() { HasError = true, Message = "上传过程中发生意外错误！" };
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="distinctPath"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        public static ReturnValue UploadFile(Stream inputStream, string distinctPath, string newFileName)
        {
            var retValue = new ReturnValue();
            try
            {
                string storedPath = "";			//存储路径
                string fullFileName = "";
                FilePathUtil.CreateDirectory(distinctPath);
                inputStream.Flush();
                inputStream.Seek(0, SeekOrigin.Begin);
                var input = new byte[inputStream.Length];
                inputStream.Read(input, 0, (int)inputStream.Length);
                storedPath = AppConfiguration.WebServerFileRootPath.Replace("\\", "/").TrimEnd('/') + "/" + distinctPath.TrimStart('/');
                fullFileName = storedPath + "/" + newFileName;
                var fs = new FileStream(fullFileName, FileMode.Create, FileAccess.Write);
                fs.Write(input, 0, input.Length);
                fs.Flush();
                fs.Close();

            }
            catch (Exception ex)
            {
                retValue.HasError = true;
                retValue.Message = ex.Message;
            }
            return retValue;
        }

        private string GetImageName()
        {
            Random rd = new Random();
            StringBuilder serial = new StringBuilder();
            serial.Append(DateTime.Now.ToString("yyyyMMddHHmmssff"));
            serial.Append(rd.Next(0, 999999).ToString());
            return serial.ToString();

        }
        /// <summary>
        /// 检查是否为合法的上传图片
        /// </summary>
        /// <param name="_fileExt">文件的后缀名</param>
        ///   <param name="fileSzie">文件长度</param>
        /// <returns></returns>
        private bool CheckImageExt(string imageExt, HttpPostedFileBase files, int checkFileSzie, int checkHigh, int checkWidth)
        {
            bool b = false;
            try
            {
                string[] allowExt = new string[] { ".gif", ".jpg", ".jpeg", ".bmp" };
                StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
                System.Drawing.Image checkImg = System.Drawing.Image.FromStream(files.InputStream);
                if (files.ContentLength / 1024 <= checkFileSzie && checkImg.Width == checkHigh && checkImg.Width == checkWidth)
                {
                    b = allowExt.Any(c => stringComparer.Equals(c, imageExt));
                }
            }
            catch
            {
                b = false;
            }


            return b;

        }




    }

    public class ImgFormat
    {


        public ImgFormat(List<string> imageExtList, int? fileSize = null, int? height = null, int? width = null)
        {
            this.ImageExtList = imageExtList;
            this.FileSize = fileSize;
            this.Height = height;
            this.Width = width;
        }

        private List<string> imageExtList = new List<string>();

        /// <summary>
        /// 图片格式列表(后缀名列表)
        /// </summary>
        public List<string> ImageExtList { get { return imageExtList; } set { imageExtList = value; } }

        /// <summary>
        /// 图片大小(M)
        /// </summary>
        public int? FileSize { get; set; }

        /// <summary>
        /// 图片高度
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// 宽高是否绝对比较
        /// </summary>
        public bool? Absolute { get; set; }

        /// <summary>
        /// 文件宽度
        /// </summary>
        public int? Width { get; set; }

        public bool CheckImgFormat(HttpPostedFileBase files, out string msg, out System.Drawing.Image checkImg)
        {
            bool b = true;
            try
            {
                msg = "";
                Stream stream = new MemoryStream();
                files.InputStream.CopyTo(stream);
                StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
                // System.Drawing.Image checkImg = System.Drawing.Image.FromStream(stream);
                checkImg = System.Drawing.Image.FromStream(stream);
                if (this.FileSize.HasValue)
                {
                    b = stream.Length / 1024.00 / 1024.00 <= this.FileSize.Value;
                    if (!b) { msg = msg + "大小应小于" + this.FileSize + "M"; }
                }
                if (b && this.Height.HasValue)
                {
                    if (Absolute.HasValue && Absolute.Value == true)
                    {
                        b = checkImg.Height == this.Height;
                        if (!b) { msg = msg + (string.IsNullOrEmpty(msg) ? "" : ",") + "像素高度应为" + this.Height; }
                    }
                    else
                    {
                        b = checkImg.Height <= this.Height;
                        if (!b) { msg = msg + (string.IsNullOrEmpty(msg) ? "" : ",") + "像素高度应小于" + this.Height; }
                    }
                }
                if (b && this.Width.HasValue) //上传图片去掉宽度限制 建议1920*450
                {
                    if (Absolute.HasValue && Absolute.Value == true)
                    {
                        //b = checkImg.Width == this.Width;
                        // if (!b) { msg = msg + (string.IsNullOrEmpty(msg) ? "" : ",") + "像素宽度应为" + this.Width; }
                    }
                    else
                    {
                        // b = checkImg.Width <= this.Width;
                        //if (!b) { msg = msg + (string.IsNullOrEmpty(msg) ? "" : ",") + "像素宽度应小于" + this.Width; }
                    }
                }
                if (b && this.ImageExtList != null && this.ImageExtList.Count > 0)
                {
                    string extStr = Path.GetExtension(files.FileName).ToLower().Trim().Replace(".", "");
                    b = this.ImageExtList.Any(c => stringComparer.Equals(c.ToLower().Trim(), extStr));
                    if (!b)
                    {
                        string imfFormatExtStr = "";
                        this.ImageExtList.ForEach(a => { imfFormatExtStr += a + ","; });
                        msg = msg + (string.IsNullOrEmpty(msg) ? "" : ",") + "格式应为 " + imfFormatExtStr.Substring(0, imfFormatExtStr.Length - 1);
                    }
                }
                msg = b ? "OK" : "图片格式不正确，" + msg;
                stream.Dispose();
                // checkImg.Dispose();
                return b;
            }
            catch
            {
                checkImg = null;
                msg = "图片格式错误！";
                return false;
            }

        }
    }
}
