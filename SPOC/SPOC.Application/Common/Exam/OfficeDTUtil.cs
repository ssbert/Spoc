using System;
using SPOC.Common.File;

namespace SPOC.Common.Exam
{
    /// <summary>
    /// OFFICE操作题类别编号
    /// </summary>
    public class EnumOfficeModeCode
    {
        public const string Word = "word";
        public const string Excel = "excel";
        public const string Powerpoint = "powerpoint";
        public const string Visio = "visio";
        public const string Html = "html";
    }

    public class OfficeDTUtil
    {
        //空操作文件名
        private static string EmptFyFileName = "empty";
        //操作题答案存放子路径名称
        private static string ExamAnswerDirectory = "ExamAnswer";
        //操作题答案文件前缀名
        private static string OptAnswerFilePrefix = "Opt_";
        //操作题考试ID文件前缀名
        private static string ExamUidFilePrefix = "Exam_";
        //操作题考试成绩文件前缀名
        private static string ExamGradeUidFilePrefix = "ExamGrade_";

        /// <summary>
        /// 返回保存 Office 答题文件的路径
        /// </summary>
        /// <param name="examUid">考试ID</param>
        /// <param name="UserUid">用户ID</param>
        /// <param name="officeSuffix">文件名</param>
        /// <returns></returns>
        public static string GetOppositeExamOfficeDTFileWebPath(string examUid, string examGradeUid, string questionUid, string officeMode)
        {
            string FilePath = "/" + ExamAnswerDirectory + "/" + ExamUidFilePrefix + examUid + "/" + ExamGradeUidFilePrefix + examGradeUid;
            
            string sPath = AppConfiguration.FileServerFileRootPath + FilePath;
            if (!FilePathUtil.IsPathExist(sPath))
            {
                FilePathUtil.CreateDirectory(sPath);
            }

            string cgi = AppConfiguration.FileServer.FileServerUploadCGI;
            cgi = cgi.Replace(AppConfiguration.FileServer.FileServerHttpHost, String.Empty);
            cgi = String.Format(cgi, FilePath + "/" + OptAnswerFilePrefix + questionUid + getOfficeSuffix(officeMode));

            //string FileUploadPage = "/fileservice/FileUpload.aspx?FilePath=";
            //string cgi = AppConfiguration.FileServerHttpHost;
            //cgi = cgi + FileUploadPage + FilePath + "/" + OptAnswerFilePrefix + questionUid + getOfficeSuffix(officeMode);

            return cgi;
        }

        //返回操作题文件的物理绝对路径
        public static string getAbsoluteExamOfficeDTFilePhysicalPath(string examUid, string examGradeUid, string questionUid, string officeMode)
        {
            return AppConfiguration.FileServerFileRootPath.TrimEnd('/') + "/" + getOppositeExamOfficeDTFileWebPath(examUid, examGradeUid, questionUid, officeMode).TrimStart('/');
        }

        //返回操作题文件的web绝对路径
        public static string getAbsoluteExamOfficeDTFileWebPath(string examUid, string examGradeUid, string questionUid, string officeMode)
        {
            return AppConfiguration.FileServerFileWebPathRoot.TrimEnd('/') + "/" + getOppositeExamOfficeDTFileWebPath(examUid, examGradeUid, questionUid, officeMode).TrimStart('/');
        }

        //返回操作题文件的web路径
        public static string getOppositeExamOfficeDTFileWebPath(string examUid, string examGradeUid, string questionUid, string officeMode)
        {
            string tempExamUid = ExamUidFilePrefix + examUid;
            string tempExamGradeUid = ExamGradeUidFilePrefix + examGradeUid;

            string fileName = OptAnswerFilePrefix + questionUid + getOfficeSuffix(officeMode);

            return "/" + ExamAnswerDirectory + "/" + tempExamUid + "/" + tempExamGradeUid + "/" + fileName;
        }

        //返回操作题文件的web路径
        public static string getOppositeExamOfficeDTFileWebPathWithoutFileName(string examUid, string examGradeUid)
        {
            string tempExamUid = ExamUidFilePrefix + examUid;
            string tempExamGradeUid = ExamGradeUidFilePrefix + examGradeUid;

            return "/" + ExamAnswerDirectory + "/" + tempExamUid + "/" + tempExamGradeUid;
        }

        //返回操作题文件 emply 的 web 绝对路径
        public static string getAbsoluteExamOfficeDTEmptyFileWebPath(string officeMode)
        {
            return AppConfiguration.FileServerFileWebPathRoot.TrimEnd('/') + "/" + getOppositeExamOfficeDTEmptyFileWebPath(officeMode).TrimStart('/');
        }

        //返回操作题文件 emply 的 web 路径
        public static string getOppositeExamOfficeDTEmptyFileWebPath(string officeMode)
        {
            return EmptFyFileName + getOfficeSuffix(officeMode);
        }

        //返回操作题文件的 web 根目录 
        public static string GetOppositeContentFileWebPathRoot(string questionUid)
        {
            return "/" + FilePathUtil.GetOppositeFileWebPathRoot(questionUid, "question");
        }

        //操作题答案文件后缀名
        public static string getOfficeSuffix(string office)
        {
            if (office != EnumOfficeModeCode.Word
                && office != EnumOfficeModeCode.Excel
                && office != EnumOfficeModeCode.Powerpoint
                && office != EnumOfficeModeCode.Visio
                && office != EnumOfficeModeCode.Html)
            {
                throw new ApplicationException(("未知 Office 类型！"));
            }

            if (EnumOfficeModeCode.Word == office)
            {
                return ".doc";
            }
            else if (EnumOfficeModeCode.Excel == office)
            {
                return ".xls";
            }
            else if (EnumOfficeModeCode.Visio == office)
            {
                return ".vsd";
            }
            else if (EnumOfficeModeCode.Powerpoint == office)
            {
                return ".ppt";
            }
            else
            {
                return ".doc"; //默认返回word
            }
        }
    }

   
}
