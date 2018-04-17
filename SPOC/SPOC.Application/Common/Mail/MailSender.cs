using System.Net.Mail;
using System.Text;

namespace SPOC.Common.Mail
{
    public class MailSender
    {
        private static string strHost = ""; ////STMP服务器地址
        private static string strAccount = ""; ////SMTP服务帐号
        private static string strPwd = ""; //发送方邮件密码
        private static string strFrom = ""; //发送方邮件地址
        private static string strName = ""; //发送方名称


        ////STMP服务器地址
        public static string StrHost
        {
            get { return strHost; }
            set { strHost = value; }
        }

        ////SMTP服务帐号
        public static string StrAccount
        {
            get { return strAccount; }
            set { strAccount = value; }
        }

        ////发送方邮件地址
        public static string StrFrom
        {
            get { return strFrom; }
            set { strFrom = value; }
        }

        ////发送方邮件密码
        public static string StrPwd
        {
            get { return strPwd; }
            set { strPwd = value; }
        }

        public static string StrName
        {
            get { return strName; }
            set { strName = value; }
        }

        /**/

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">接收方邮件地址</param>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件正文内容</param>
        /// <returns></returns>
        public static bool sendMail(string to, string title, string content)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; //指定电子邮件发送方式
            smtpClient.Host = strHost;
            //指定SMTP服务器
            smtpClient.Credentials = new System.Net.NetworkCredential(strAccount, strPwd); //用户名和密码
            MailAddress mailFrom = new MailAddress(strFrom, strName, Encoding.UTF8);
            MailAddress mailTo = new MailAddress(to);
            MailMessage mailMessage = new MailMessage(mailFrom, mailTo);
            mailMessage.Subject = title; //主题
            mailMessage.Body = content; //内容
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8; //正文编码
            mailMessage.IsBodyHtml = true; //设置为HTML格式
            mailMessage.Priority = MailPriority.High; //优先级

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        


    }
}
