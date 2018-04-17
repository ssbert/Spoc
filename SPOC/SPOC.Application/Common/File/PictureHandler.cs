using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace SPOC.Common.File
{
    public static class PictureHandler
    {
        /// <summary>
        /// 图片微缩图处理，返回缩略图的保存路径
        /// </summary>
        /// <param name="srcPath">源图片</param>
        /// <param name="destPath">目标图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public static string CreateThumbnailPicture(string srcPath, string destPath, int width, int height, HttpPostedFileBase srcFile=null)
        {
            //根据图片的磁盘绝对路径获取 源图片 的Image对象
            //System.Drawing.Image img = System.Drawing.Image.FromFile(srcPath);
            System.Drawing.Image img = srcFile != null ? System.Drawing.Image.FromStream(srcFile.InputStream) : System.Drawing.Image.FromFile(srcPath);
 
            //bmp： 最终要建立的 微缩图 位图对象。
           Bitmap bmp = new Bitmap(width, height);
            

            //g: 绘制 bmp Graphics 对象
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            //为Graphics g 对象 初始化必要参数，很容易理解。
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            //源图片宽和高
            int imgWidth = img.Width;
            int imgHeight = img.Height;

            //绘制微缩图
            g.DrawImage(img, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, imgWidth, imgHeight)
                        , GraphicsUnit.Pixel);

            ImageFormat format = img.RawFormat;
            ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().SingleOrDefault(i => i.FormatID == format.Guid);
            EncoderParameter param = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = param;
            img.Dispose();

            //保存已生成微缩图，这里将GIF格式转化成png格式。
            if (format == ImageFormat.Gif)
            {
                destPath = destPath.ToLower().Replace(".gif", ".png");
                bmp.Save(destPath, ImageFormat.Png);
            }
            else
            {
                if (info != null)
                {
                    bmp.Save(destPath, info, parameters);
                }
                else
                {

                    bmp.Save(destPath, format);
                }
            }

            img.Dispose();
            g.Dispose();
            bmp.Dispose();

            return destPath;
        }

        /// <summary>
        /// 图片微缩图处理,(返回缩略图的Byte)
        /// </summary>
        /// <param name="srcPath">源图片</param>
        /// <param name="destPath">目标图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public static byte[] GetThumbnailPicture(string srcPath, string destPath, int width, int height,   HttpPostedFileBase srcFile = null)
        {
            //根据图片的磁盘绝对路径获取 源图片 的Image对象
            //System.Drawing.Image img = System.Drawing.Image.FromFile(srcPath);

            //重命名名ico图标
            srcPath = "favicon.ico?" + DateTime.Now.ToString("yyyyMMddHHmmss") + "";

            System.Drawing.Image img = srcFile != null ? System.Drawing.Image.FromStream(srcFile.InputStream) : System.Drawing.Image.FromFile(srcPath);

            int w = img.Width;
            int h = img.Height;
            float ratio = (float)w / h;

            //微缩图高度和宽度
            int newHeight = h <= height ? h : height;
            int newWidth = h <= height ? w : Convert.ToInt32(height * ratio);

            //bmp： 最终要建立的 微缩图 位图对象。
            //Bitmap bmp = new Bitmap(width, height);
            Bitmap bmp = new Bitmap(newWidth, newHeight);

            //g: 绘制 bmp Graphics 对象
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            //为Graphics g 对象 初始化必要参数，很容易理解。
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            //源图片宽和高
            int imgWidth =w;
            int imgHeight = h;

            //绘制微缩图
           // g.DrawImage(img, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, imgWidth, imgHeight) , GraphicsUnit.Pixel);
            g.DrawImage(img, new System.Drawing.Rectangle(0, 0, newWidth, newHeight), new System.Drawing.Rectangle(0, 0, imgWidth, imgHeight)
                       , GraphicsUnit.Pixel);

            ImageFormat format = img.RawFormat;
            ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().SingleOrDefault(i => i.FormatID == format.Guid);
            EncoderParameter param = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = param;
          //  img.Dispose();

            ////保存已生成微缩图，这里将GIF格式转化成png格式。
            //if (format == ImageFormat.Gif)
            //{
            //    destPath = destPath.ToLower().Replace(".gif", ".png");
            //    bmp.Save(destPath, ImageFormat.Png);
            //}
            //else
            //{
            //    if (info != null)
            //    {
            //        bmp.Save(destPath, info, parameters);
            //    }
            //    else
            //    {

            //        bmp.Save(destPath, format);
            //    }
            //}

            using (System.IO.MemoryStream ms = new MemoryStream())
            {
                //将图片 保存到内存流中
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                //将内存流 里的 数据  转成 byte 数组 返回
                img.Dispose();
                g.Dispose();
                bmp.Dispose();
                
                return ms.ToArray();
            }

          

          
        }

        public static byte[] GetPictureStream(HttpPostedFileBase srcFile,out int h,out int w)
        {
            Stream stream = new MemoryStream();
            srcFile.InputStream.CopyTo(stream);
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            h = img.Height;
            w = img.Width;
            img.Dispose();

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
           
        }

        /// <summary>
        /// 根据裁剪尺度生成缩略图
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static byte[] MakeThumbnail(string path, int sx, int sy, int width, int height, string savePath = "",float? defaultLen=null)
        {
           
            if (!System.IO.File.Exists(path)) {

                return null;
            }
            Image originalImage = Image.FromFile(path);


            double hProportion = defaultLen.HasValue ? Convert.ToDouble(originalImage.Height) / defaultLen.Value : 1;
            double wProportion = defaultLen.HasValue ? Convert.ToDouble(originalImage.Width) / defaultLen.Value : 1;
            var proportion = wProportion < hProportion ? wProportion : hProportion;

            int x = Convert.ToInt32(sx * proportion);
            int y = Convert.ToInt32(sy * proportion);
            int ow = Convert.ToInt32(originalImage.Width * proportion);
            int oh = Convert.ToInt32(originalImage.Height * proportion);
            width = Convert.ToInt32(width * proportion);
            height = Convert.ToInt32(height * proportion);

         //   width = width > ow ? ow : width;


            //新建一个bmp图片   
            Image bitmap = new Bitmap(width, height);

            //新建一个画板   
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法   
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度   
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充   
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分   
            g.DrawImage(originalImage, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            try
            {
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                if (!string.IsNullOrEmpty(savePath)) {
                    
                    bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
               
                
                
                return ms.GetBuffer();//也可以生成到指定目录，具体方法不介绍了  
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }  
    }
}