using System;
using System.Collections;
using System.Configuration;
using System.Web;

namespace SPOC.Common.File
{
    public class AppConfiguration
    {
        private static string _webServerWebRoot = null;
        private static string _webServerRootPath = null;
        private static string _webServerFileRootPath = null;
        private static string _webServerFileWebRootPath = null;
        private static string _fileServerLocation = null;
        private static string _fileServerWebRootPath = null;
        private static string _fileServerFileRootPath = null;
        private static string _fileServerFileWebPathRoot = null;
        private static string _fileServerFTPRoot = null;
        private static string _fileServerFTPWebRoot = null;
        private static string _fileServerHttpHost = null;
        private static string _uploadFileUserName = null;
        private static string _uploadFileUserPassword = null;

        private static Hashtable _fileServerDomainMatchList = null;
        private static string _fileServerWebRootPathOfWebServerVisit = null;


        /// <summary>
        /// web.config  用户登录后是否提醒其继续学习上次未完成的课时,Y:表示提醒 N:表示不提醒 2016-03-29 15:08
        /// </summary>
        public static bool IsAlertUnCompleteLesson
        {
            get
            {
                var isAlertUnCompleteLesson = ConfigurationManager.AppSettings["IsAlertUnCompleteLesson"];
                if (isAlertUnCompleteLesson == null) return false;
                return ConfigurationManager.AppSettings["IsAlertUnCompleteLesson"].ToString().ToUpper() == "Y" ? true : false;
            }
        }

        /// <summary>
        /// 重置所有值
        /// </summary>
        public static void ResetData()
        {
            _webServerWebRoot = null;
            _webServerRootPath = null;
            _webServerFileRootPath = null;
            _webServerFileWebRootPath = null;
            _fileServerLocation = null;
            _fileServerWebRootPath = null;
            _fileServerFileRootPath = null;
            _fileServerFileWebPathRoot = null;
            _fileServerFTPRoot = null;
            _fileServerFTPWebRoot = null;
            _fileServerHttpHost = null;
            _uploadFileUserName = null;
            _uploadFileUserPassword = null;
            _fileServerDomainMatchList = null;
            _fileServerWebRootPathOfWebServerVisit = null;
        }

        public static string FileServerHttpHost
        {
            get
            {
                return WebServerWebRoot.Substring(WebServerWebRoot.LastIndexOf("/") + 1);
            }
        }


        /// <summary>
        /// Web服务器根目录的物理路径(以/线分隔，后面没有/线),固定在根目录下
        /// </summary>
        public static string WebServerRootPath
        {
            get
            {
                if (_webServerRootPath == null)
                {
                    _webServerRootPath = HttpContext.Current.Request.PhysicalApplicationPath.Replace("\\", "/").TrimEnd('/').ToLower();
                }
                return _webServerRootPath;
            }
        }

        /// <summary>
        /// Web服务器的文件根目录的物理路径(以/线分隔，后面没有/线),固定在Web下的fileroot下
        /// </summary>
        public static string WebServerFileRootPath
        {
            get
            {
                if (_webServerFileRootPath == null)
                {
                    _webServerFileRootPath = HttpContext.Current.Request.PhysicalApplicationPath.Replace("\\", "/").TrimEnd('/').ToLower() + "/fileroot";
                }
                return _webServerFileRootPath;
            }
        }

        /// <summary>
        /// Web服务器的文件根目录的Web路径(以/线分隔，后面没有/线)
        /// </summary>
        public static string WebServerFileWebRootPath
        {
            get
            {
                //需要每次都重取,不能用内存里的值,因为在有多个IP时,每次请求的URL可能不一样,即WebServerWebRoot可能不一样
                _webServerFileWebRootPath = WebServerWebRoot + "/fileroot";
                return _webServerFileWebRootPath;
            }
        }

        /// <summary>
        /// File服务器的文件根目录的物理路径(以/线分隔，后面没有/线)
        /// </summary>
        public static string FileServerFileRootPath
        {
            get
            {
                if (_fileServerFileRootPath == null)
                {
                    _fileServerFileRootPath = WebServerFileRootPath;

                }
                return _fileServerFileRootPath;
            }
        }

        /// <summary>
        /// 文件服务器的Web根路径(以/线分隔，后面没有/线)
        /// </summary>
        public static string FileServerWebRootPath
        {
            get
            {
                _fileServerWebRootPath = WebServerWebRoot;
                return _fileServerWebRootPath;
            }
        }

        /// <summary>
        /// 文件服务器的文件的Web根路径(以/线分隔，后面没有/线)
        /// </summary>
        public static string FileServerFileWebPathRoot
        {
            get
            {
                return FileServerWebRootPath + "/fileroot";
            }
        }





        /// <summary>
        /// 从URL中取得域名
        /// </summary>
        /// <param name="webPath"></param>
        /// <returns></returns>
        private static string GetDomainFromWebPath(string webPath)
        {
            try
            {
                webPath = webPath.ToLower();
                webPath = webPath.Substring(webPath.IndexOf("://") + 3);
                int _index = webPath.IndexOf("/");
                if (_index > 0)
                {
                    webPath = webPath.Substring(0, webPath.IndexOf("/"));
                }
                return webPath;
            }
            catch (Exception ex)
            {
                throw new Exception("处理Url出错:" + webPath + ". 错误信息为:" + ex.Message);
            }
        }

        /// <summary>
        /// 当前系统的Web根路径(以/线分隔，后面没有/线)
        /// </summary>
        public static string WebServerWebRoot
        {
            get
            {
                System.Web.HttpContext Context = System.Web.HttpContext.Current;
                string urlSuffix = "";
                if (Context.Request.Url.IsDefaultPort == false)
                    urlSuffix = Context.Request.Url.Host + ":" + Context.Request.Url.Port.ToString() + Context.Request.ApplicationPath.Replace("\\", "/").TrimEnd('/');
                else
                    urlSuffix = Context.Request.Url.Host + Context.Request.ApplicationPath.Replace("\\", "/").TrimEnd('/');

                _webServerWebRoot = @"http://" + urlSuffix.ToLower();


                return _webServerWebRoot;
            }

        }

        /// <summary>
        /// 文件服务器中供上传或拷贝大或多文件时预先存放存文件的路径
        /// </summary>
        public static string FileServerFTPRoot
        {
            get
            {
                if (_fileServerFTPRoot == null)
                {

                    if (_fileServerFTPRoot == null || _fileServerFTPRoot == "") _fileServerFTPRoot = FileServerFileRootPath + "/FTPRoot";
                }
                return _fileServerFTPRoot;
            }
        }

        public static string FileServerFTPWebRoot
        {
            get
            {
                if (_fileServerFTPWebRoot == null)
                {

                    if (_fileServerFTPWebRoot == null || _fileServerFTPWebRoot == "") _fileServerFTPWebRoot = FileServerFileWebPathRoot + "/FTPRoot";
                }
                return _fileServerFTPWebRoot;
            }
        }


        public class FileServer
        {
            /*
             * 文件服务器配置类
             */
            public static String FileServerHostName
            {
                /*
                 * 文件服务器主机名 比如 localhost || edmond  || other
                 * 用于WebOffice写文件路径
                 */
                get
                {

                    int nPos1 = AppConfiguration.FileServerWebRootPath.IndexOf("://");

                    int nAddPos = 0;
                    if (nPos1 > -1)
                    {
                        nAddPos = 3;
                    }

                    int nPos2 = AppConfiguration.FileServerWebRootPath.IndexOf("/", nPos1 + nAddPos); //?
                    if (nPos2 == -1)
                    {
                        return AppConfiguration.FileServerWebRootPath.Substring(nPos1 + nAddPos);
                    }
                    else
                    {
                        return AppConfiguration.FileServerWebRootPath.Substring(nPos1 + nAddPos, nPos2 - nPos1 - nAddPos);
                    }
                }
            }
            public static String FileServerHttpHost
            {
                get
                {
                    return "http://" + AppConfiguration.FileServer.FileServerHostName;
                }
            }


            public static String FileServerUploadCGI
            {
                /*
                 * 上传组件地址
                 */
                get
                {
                    return AppConfiguration.FileServerWebRootPath.TrimEnd('/') + fileServerUploadCGI;
                }
            }
            private static string fileServerUploadCGI = "/fileservice/FileUpload.aspx?FilePath={0}";

        }
    }
}