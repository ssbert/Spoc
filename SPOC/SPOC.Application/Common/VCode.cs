using System;
using System.Drawing;
using System.IO;
using System.Web;

namespace SPOC.Common
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class VCode
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public static string CodeStr;
        readonly Random _random = new Random();

        /// <summary>
        /// 构造函数
        /// </summary>
        public VCode()
        {
            CodeStr = GetRandomStr();
        }

        /// <summary>
        /// 生成验证码图片 字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetVCode()
        {
            //   using (Image img = new Bitmap(80, 23))
            using (var img = new Bitmap(70, 28))
            {
                var strCode = CodeStr;
                HttpContext.Current.Session["vcode"] = strCode;
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.Clear(Color.White);
                    g.DrawRectangle(Pens.Black, 0, 0, img.Width - 1, img.Height - 1);
                    DrawPoint(g);
                    g.DrawString(strCode, new Font("微软雅黑", 15), Brushes.Blue, new PointF(2, 2));
                    DrawPoint(g);
                    using (var ms = new MemoryStream())
                    {
                        //将图片 保存到内存流中
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //将内存流 里的 数据  转成 byte 数组 返回

                        return ms.ToArray();
                    }
                }
            }
        }

        string GetRandomStr()
        {

            string str = string.Empty;
            string[] strArr = { "A", "b", "c", "d", "e", "f", "g", "h", "I", "J", "k", "l", "T", "n", "o", "p", "Q", "r", "s", "t", "U", "v", "w", "x", "y", "z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            for (int i = 0; i < 4; i++)
            {
                int index = _random.Next(strArr.Length);
                str += strArr[index];
            }
            return str;
        }

        void DrawPoint(Graphics g)
        {
            Pen[] pens = { Pens.Blue, Pens.Black, Pens.Red, Pens.Green, Pens.Yellow, Pens.YellowGreen, Pens.Tomato };
            var length = 0;
            for (int i = 0; i < 50; i++)
            {
                var p1 = new Point(_random.Next(79), _random.Next(29));
                var p2 = new Point(p1.X - length, p1.Y - length);
                length = _random.Next(5);
                g.DrawLine(pens[_random.Next(pens.Length)], p1, p2);
            }
        }
    }
}