using System;
using System.IO;
using System.Text;

namespace SPOC.Common.File
{
    public class FilePathUtil
    {
        /// <summary>
        /// 取得相对的某一用户的临时文件路径
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        public static string GetOppositeUserTempPath(string userUid)
        {
            string oppositePath = "temp/" + userUid;
            return oppositePath;
        }

        public static string GetExamOfficeAnswerFilesPath(string questionUid)
        {
            string oppositePath = "ExamOfficeAnswerFiles/" + questionUid;
            return oppositePath;
        }

        public static string GetQuestionFilesPath(string questionUid)
        {
            string oppositePath = "QuestionFiles/" + questionUid;
            return oppositePath;
        }

        public static string GetUnZipUserTempPath(string userUid)
        {
            string oppositePath = "unzip_temp/" + userUid;
            return oppositePath;
        }

        /// <summary>
        /// 得到某种类型的相对路径
        /// </summary>
        /// <param name="recordUid"></param>
        /// <param name="recordType"></param>
        /// <returns></returns>
        public static string GetOppositeFileWebPathRoot(string recordUid, string recordType)
        {
            string oppositePath = recordType + "/" + recordUid;
            return oppositePath;
        }

        /// <summary>
        /// 得到试题的相关文件的根分类
        /// </summary>
        /// <param name="recordUid"></param>
        /// <returns></returns>
        public static string GetAbsoluteFileWebPathRoot(string recordUid, string recordType)
        {
            return AppConfiguration.FileServerFileWebPathRoot.TrimEnd('/') + "/" + GetOppositeFileWebPathRoot(recordUid, recordType).TrimStart('/');
        }

        /// <summary>
        /// 对试题中的路径进行更换得到全路径
        /// </summary>
        /// <param name="recordUid"></param>
        /// <param name="contentText"></param>
        /// <param name="recordType"></param>
        /// <returns></returns>
        public static string GetContentTextWithFilePath(string recordUid, string recordType, string contentText)
        {
            string contentFileWebPathRoot = GetAbsoluteFileWebPathRoot(recordUid, recordType) + "/";
            return GetContentTextWithFilePath(contentText, contentFileWebPathRoot);
        }

        /// <summary>
        /// 对试题中的路径进行更换得到全路径
        /// </summary>
        /// <param name="recordUid"></param>
        /// <param name="contentText"></param>
        /// <param name="recordType"></param>
        /// <param name="changeEnterAsBr"></param>
        /// <returns></returns>
        public static string GetContentTextWithFilePath(string recordUid, string recordType, string contentText, bool changeEnterAsBr)
        {
            string contentFileWebPathRoot = GetAbsoluteFileWebPathRoot(recordUid, recordType) + "/";
            return GetContentTextWithFilePath(contentText, contentFileWebPathRoot, changeEnterAsBr);
        }

        /// <summary>
        /// 创建绝对路径目录，失败就返回错误信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CreateOppositionPath(string path)
        {
            string errorMessage = string.Empty;
            if (!Directory.Exists(AppConfiguration.FileServerFileRootPath + "/" + path))
            {
                //检查上一级
                if (path.LastIndexOf("/") > 0)
                {
                    string parentPath = path.Substring(0, path.LastIndexOf("/"));
                    errorMessage = CreateOppositionPath(parentPath);
                    if (errorMessage != "") return errorMessage;
                }
                //创建该目录
                try
                {
                    Directory.CreateDirectory(AppConfiguration.FileServerFileRootPath + "\\" + path);
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }
            }



            return errorMessage;
        }

        /// <summary>
        /// 对试题中的路径进行更换得到全路径
        /// </summary>
        public static string GetContentTextWithFilePath(string contentText, string absoluteContentFileWebPathRoot)
        {
            return GetContentTextWithFilePath(contentText, absoluteContentFileWebPathRoot, false);
        }

        /// <summary>
        /// 对试题中的路径进行更换得到全路径
        /// </summary>
        /// <param name="contentText"></param>
        /// <param name="absoluteContentFileWebPathRoot"></param>
        /// <param name="changeEnterAsBr">是否把回车换成<br/></param>
        /// <returns></returns>
        public static string GetContentTextWithFilePath(string contentText, string absoluteContentFileWebPathRoot, bool changeEnterAsBr)
        {
            if (string.IsNullOrEmpty(contentText))
                return "";
            if (contentText.Length > 7)
            {
                if (contentText.Substring(0, 3).ToUpper() == "<P>" && contentText.Substring(contentText.Length - 4, 4).ToUpper() == "</P>" && contentText.ToUpper().IndexOf("<P>", 3) == -1)
                    contentText = contentText.Substring(3, contentText.Length - 7);
            }

            if (contentText == "" || absoluteContentFileWebPathRoot == "")
                return contentText;

            //顺便在这里处理回车的问题
            //回车有两种，一种是HTML中的回车，一般是在>后回车，这种不需要转换
            //另一种是内容本身是回车，这种要转换成<br/>
            if (changeEnterAsBr == true)
            {
                contentText = contentText.Replace(">\r\n", "$HTMLEnter$");      //临时转换成别的以免被替换掉
                contentText = contentText.Replace("\r\n", "<br/>");
                contentText = contentText.Replace("$HTMLEnter$", ">\r\n");      //将临时换掉的换回来
            }

            if (absoluteContentFileWebPathRoot.Substring(absoluteContentFileWebPathRoot.Length - 1, 1) != "/" && absoluteContentFileWebPathRoot.Substring(absoluteContentFileWebPathRoot.Length - 1, 1) != "\\")
                absoluteContentFileWebPathRoot = absoluteContentFileWebPathRoot + "/";
            contentText = contentText.Replace("<IMG", "<img");
            contentText = contentText.Replace("<A", "<a");
            contentText = contentText.Replace("<EMBED", "<embed");
            contentText = contentText.Replace("SRC=\"./", "src=\"./");		//大写变小写
            contentText = contentText.Replace("SRC='./", "src='./");		//大写变小写
            contentText = contentText.Replace("SRC=./", "src=./");			//大写变小写

            contentText = contentText.Replace("HREF=\"./", "href=\"./");	//大写变小写
            contentText = contentText.Replace("HREF='./", "href='./");		//大写变小写
            contentText = contentText.Replace("HREF=./", "href=./");		//大写变小写



            //处理Flash代码的大小写
            contentText = contentText.Replace("<PARAM NAME=\"src\" VALUE=\"./", "<PARAM NAME=\"SRC\" VALUE=\"./");
            contentText = contentText.Replace("<PARAM NAME=\"Src\" VALUE=\"./", "<PARAM NAME=\"SRC\" VALUE=\"./");
            contentText = contentText.Replace("<PARAM NAME=\"movie\" VALUE=\"./", "<PARAM NAME=\"MOVIE\" VALUE=\"./");
            contentText = contentText.Replace("<PARAM NAME=\"Movie\" VALUE=\"./", "<PARAM NAME=\"MOVIE\" VALUE=\"./");
            contentText = contentText.Replace("<PARAM NAME=\"url\" VALUE=\"./", "<PARAM NAME=\"URL\" VALUE=\"./");
            contentText = contentText.Replace("<PARAM NAME=\"Url\" VALUE=\"./", "<PARAM NAME=\"URL\" VALUE=\"./");

            //<PARAM NAME="SRC" VALUE="./
            contentText = contentText.Replace("<PARAM NAME=\"SRC\" VALUE=\"./", "<PARAM NAME=\"SRC\" VALUE=\"" + absoluteContentFileWebPathRoot);

            //<PARAM NAME="URL" VALUE="./(语音视频用)
            contentText = contentText.Replace("<PARAM NAME=\"URL\" VALUE=\"./", "<PARAM NAME=\"URL\" VALUE=\"" + absoluteContentFileWebPathRoot);
            contentText = contentText.Replace("<param name=\"url\" value=\"./", "<param name=\"url\" value=\"" + absoluteContentFileWebPathRoot);

            //<PARAM NAME="MOVIE" VALUE="./(Flash用)
            contentText = contentText.Replace("<PARAM NAME=\"MOVIE\" VALUE=\"./", "<PARAM NAME=\"MOVIE\" VALUE=\"" + absoluteContentFileWebPathRoot);

            //开始更换图片等的路径
            contentText = contentText.Replace("src=\"./", "src=\"" + absoluteContentFileWebPathRoot);
            contentText = contentText.Replace("src='./", "src='" + absoluteContentFileWebPathRoot);
            contentText = contentText.Replace("src=./", "src=" + absoluteContentFileWebPathRoot);

            contentText = contentText.Replace("href=\"./", "href=\"" + absoluteContentFileWebPathRoot);
            contentText = contentText.Replace("href='./", "href='" + absoluteContentFileWebPathRoot);
            contentText = contentText.Replace("href=./", "href=" + absoluteContentFileWebPathRoot);

            //处理Flv播放器中的Flv地址
            if (contentText.Contains("flv"))
                contentText = contentText.Replace("<source>./../../", "<source>" + AppConfiguration.FileServerWebRootPath + "/");
            else
                contentText = contentText.Replace("<source>./", "<source>" + absoluteContentFileWebPathRoot);

            ////不能作 html 符号的处理，否则会影响到 html 的正常显示结果.
            //contentText = contentText.Replace("<", "＜");
            //contentText = contentText.Replace(">", "＞");
            return contentText;
        }

        /// <summary>
        /// 得到不含前面系统地址的文本
        /// </summary>
        /// <param name="contentText"></param>
        /// <returns></returns>
        public static string GetContentTextWithNoFilePath(string contentFileWebPathRoot, string contentText)
        {
            return contentText.Replace(contentFileWebPathRoot.TrimEnd('/') + "/", "./");
        }
        /// <summary>
        /// 得到不含前面系统地址的文本
        /// </summary>
        /// <param name="fileRootPath"></param>
        /// <param name="contentText"></param>
        /// <param name="recordUid"></param>
        /// <param name="recordType"></param>
        /// <param name="isDealFileOfFlv">是否处理含有flv文件目录的文本</param>
        /// <returns></returns>
        public static string GetContentTextWithNoFilePath(string fileRootPath, string contentText, string recordUid, string recordType, bool isDealFileOfFlv)
        {
            if (string.IsNullOrEmpty(contentText))
                return contentText;
            if (fileRootPath != FilePathUtil.GetOppositeFileWebPathRoot(recordUid, recordType))
                contentText = contentText.Replace(fileRootPath, FilePathUtil.GetOppositeFileWebPathRoot(recordUid, recordType));

            if (isDealFileOfFlv)
            {
                if (contentText.Contains(".flv"))
                    contentText = GetFlvContentTextWithNoFilePath(AppConfiguration.FileServerWebRootPath, contentText);
            }
            return GetContentTextWithNoFilePath(recordUid, recordType, contentText);
        }

        /// <summary>
        /// 得到不含前面指定内容的根目录地址的文本
        /// </summary>
        /// <param name="contentText"></param>
        /// <returns></returns>
        public static string GetContentTextWithNoFilePath(string recordUid, string recordType, string contentText)
        {
            string contentFileWebPathRoot = GetAbsoluteFileWebPathRoot(recordUid, recordType) + "/";
            return contentText.Replace(contentFileWebPathRoot, "./");
        }

        /// <summary>
        /// 如果有固定域名，给相对地址增加固定域名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlToUseFixDomain(string url)
        {
            if (url.IndexOf("://") > 0)
            {
                return url;
            }
            else
            {

                return url;

            }
        }

        /// <summary>
        /// 得到处理过flv资源文件和其播放器之间的相对位置后的文本
        /// </summary>
        /// <param name="contentFileWebPathRoot"></param>
        /// <param name="contentText"></param>
        /// <returns></returns>
        public static string GetFlvContentTextWithNoFilePath(string webPathRoot, string contentText)
        {
            string[] separator = { "flv" };
            string hrefStr = "<a href";
            string[] strs = contentText.Split(separator, StringSplitOptions.None);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < strs.Length; i++)
            {
                string str = strs[i];
                if (str.LastIndexOf('.') == str.Length - 1 && str.IndexOf(hrefStr) == -1)
                {
                    if (!webPathRoot.Contains("./../../"))
                        str = str.Replace(webPathRoot.TrimEnd('/') + "/", "./../../");
                }
                if (i != strs.Length - 1)
                    str += "flv";
                result.Append(str);
            }
            return result.ToString();
        }



        /// <summary>
        /// 清除字符串中的特殊字符，主要用于针对文件名的处理
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearSpecialCharForFileName(string str)
        {
            return str.Replace("\\", "＼").Replace("/", "／").Replace(":", "：").Replace("*", "＊").Replace("\"", "＂").Replace("|", "").Replace("<", "＜").Replace(">", "＞").Replace("|", "｜").Replace("?", "？").Replace("&", "＆");
        }
        /// <summary>
        /// 检查文件是否存在,可传相对路径和绝对路径
        /// </summary>
        /// <param name="sSourceFile">文件路径</param>
        /// <returns>是否存在</returns>
        public static bool IsFileExist(string sSourceFile)
        {
            string currentRootPath;
            //如果是否带:则是全路径,否则是相对路径
            if (sSourceFile.IndexOf(":") == -1)
                currentRootPath = GetRootPath();
            else
                currentRootPath = "";
            //共享文件夹
            if (sSourceFile.IndexOf("//") == 0)
                currentRootPath = "";

            try
            {
                if (System.IO.File.Exists(currentRootPath + sSourceFile) == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static string GetRootPath()
        {
            string rootPath = AppConfiguration.WebServerFileRootPath;
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);
            return rootPath + "/";
        }
        /// <summary>
        /// 检查路径是否存在,可传相对路径和绝对路径
        /// </summary>
        /// <param name="sSourceFile">路径名称</param>
        /// <returns>是否存在</returns>
        public static bool IsPathExist(string sSourcePath)
        {
            string currentRootPath;
            //如果是否带:则是全路径,否则是相对路径
            if (sSourcePath.IndexOf(":") == -1)
                currentRootPath = GetRootPath();
            else
                currentRootPath = "";
            //共享文件夹
            if (sSourcePath.IndexOf("//") == 0)
            {
                currentRootPath = "";
            }

            if (sSourcePath.Substring(sSourcePath.Length - 1, 1) == "/" || sSourcePath.Substring(sSourcePath.Length - 1, 1) == "\\")
                sSourcePath = sSourcePath.Substring(0, sSourcePath.Length - 1);

            try
            {
                if (Directory.Exists(currentRootPath + sSourcePath) == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 创建路径
        /// </summary>
        /// <param name="sPath"></param>
        public static bool CreateDirectory(string sPath)
        {

            string currentRootPath;
            //如果是否带:则是全路径,否则是相对路径
            if (sPath.IndexOf(":") == -1)
                currentRootPath = GetRootPath();
            else
                currentRootPath = "";
            //共享文件夹
            if (sPath.IndexOf("//") == 0)
            {
                currentRootPath = "";
            }

            sPath = currentRootPath + sPath;
            sPath = sPath.Replace("/", "\\");
            //目录存在直接返回True
            if (Directory.Exists(sPath) == true)
            {
                return true;
            }
            string sCurrPath = ""; ;
            int nPos = -1;
            nPos = sPath.IndexOf("\\", nPos + 1);
            if (nPos == -1)
            {
                return false;
            }
            nPos = sPath.IndexOf("\\", nPos + 1);
            try
            {
                //网络共享目录方式
                if (sPath.IndexOf("\\") == 0)
                {
                    int i = 0;
                    while (nPos > -1)
                    {
                        if (i > 2)
                        {
                            sCurrPath = sPath.Substring(0, nPos);
                            if (Directory.Exists(sCurrPath) == false)
                            {
                                Directory.CreateDirectory(sCurrPath);
                            }
                        }
                        nPos = sPath.IndexOf("\\", nPos + 1);
                        i++;
                    }


                }
                else
                {
                    while (nPos > -1)
                    {
                        sCurrPath = sPath.Substring(0, nPos);
                        if (Directory.Exists(sCurrPath) == false)
                        {
                            Directory.CreateDirectory(sCurrPath);
                        }
                        nPos = sPath.IndexOf("\\", nPos + 1);
                    }
                }

                sCurrPath = sPath;
                if (Directory.Exists(sPath) == false)
                {
                    Directory.CreateDirectory(sPath);
                }
            }
            catch (Exception e)
            {

//                Logger.GetLogger("CreateDirectory").Error("创建目录失败：" + e.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 执行删除目录的操作(因为怕有唯读文件,所以不能直接删整个目录,要先删文件,再删目录)
        /// </summary>
        /// <param name="path"></param>
        public static void DoDeleteDirectory(string path)
        {
            string[] arrFiles = Directory.GetFiles(path);
            foreach (string filePath in arrFiles)
            {
                DoDeleteFile(filePath);
            }
            arrFiles = null;

            string[] arrDirectories = Directory.GetDirectories(path);
            foreach (string subPath in arrDirectories)
            {
                DoDeleteDirectory(subPath);
            }
            arrDirectories = null;
            Directory.Delete(path);
        }
        /// <summary>
        /// 执行删除文件操作(因为怕有唯读文件,所以要先检查是否唯读)
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        private static void DoDeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                FileInfo fi = new FileInfo(filePath);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                System.IO.File.Delete(filePath);
            }
        }
        /// <summary>
        /// 删除目录下所有文件,无错误就是成功
        /// </summary>
        /// <param name="sSourceFile"></param>
        /// <returns></returns>
        public static void DeleteDirectory(string sPath)
        {
            string currentRootPath;
            currentRootPath = GetRootPath();

            //如果是否带:则是全路径,否则是相对路径
            if (sPath.IndexOf(":") == -1 && sPath.IndexOf("//") != 0)
                sPath = currentRootPath + sPath;
            DoDeleteDirectory(sPath);

        }
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sSourceFile"></param>
        /// <param name="sDestinationFile"></param>
        /// <returns></returns>
        public static ReturnValue CopyFile(string sSourceFile, string sDestinationFile)
        {
            ReturnValue retValue = new ReturnValue(false, "");
            string currentRootPath = GetRootPath(); ;
            //如果是否带:则是全路径,否则是相对路径
            if (sSourceFile.IndexOf(":") == -1 && sSourceFile.IndexOf("//") != 0)
                sSourceFile = currentRootPath + sSourceFile;
            if (sDestinationFile.IndexOf(":") == -1 && sDestinationFile.IndexOf("//") != 0)
                sDestinationFile = currentRootPath + sDestinationFile;

            try
            {
                string sDestinationPath;
                sDestinationFile = sDestinationFile.Replace("/", "\\");
                sDestinationPath = sDestinationFile.Substring(0, sDestinationFile.LastIndexOf("\\"));
                if (Directory.Exists(sDestinationPath) == false)
                {
                    try
                    {
                        Directory.CreateDirectory(sDestinationPath);
                    }
                    catch (Exception ee)
                    {
                        retValue.HasError = true;
                        retValue.Message = string.Format(("创建目录[{0}]不成功！"), sDestinationPath) + ee.Message;
                        return retValue;
                    }
                }
                if (System.IO.File.Exists(sSourceFile) == false)
                {
                    retValue.HasError = true;
                    retValue.Message = ("源文件不存在，请检查！");
                }
                else
                {
                    if (sSourceFile != sDestinationFile)
                    {
                        System.IO.File.Copy(sSourceFile, sDestinationFile, true);
                    }
                }
            }
            catch (Exception e)
            {
                retValue.HasError = true;
                retValue.Message = ("拷贝文件失败:") + e.Message;
            }
            return retValue;
        }

        /// <summary>
        /// 检测目录是否存在，不存在则创建
        /// </summary>
        /// <param name="path">目录路径</param>
        public static void CreateDirectoryIfNotExists(string path)
        {
            var dirs = path.Split('\\');
            var newPath = dirs[0];

            for (var i = 0; i < dirs.Length; i++)
            {
                newPath = Path.Combine(newPath, dirs[1]);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

        }
    }
}