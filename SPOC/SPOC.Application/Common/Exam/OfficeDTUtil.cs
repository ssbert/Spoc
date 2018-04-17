using System;
using SPOC.Common.File;

namespace SPOC.Common.Exam
{
    /// <summary>
    /// OFFICE�����������
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
        //�ղ����ļ���
        private static string EmptFyFileName = "empty";
        //������𰸴����·������
        private static string ExamAnswerDirectory = "ExamAnswer";
        //��������ļ�ǰ׺��
        private static string OptAnswerFilePrefix = "Opt_";
        //�����⿼��ID�ļ�ǰ׺��
        private static string ExamUidFilePrefix = "Exam_";
        //�����⿼�Գɼ��ļ�ǰ׺��
        private static string ExamGradeUidFilePrefix = "ExamGrade_";

        /// <summary>
        /// ���ر��� Office �����ļ���·��
        /// </summary>
        /// <param name="examUid">����ID</param>
        /// <param name="UserUid">�û�ID</param>
        /// <param name="officeSuffix">�ļ���</param>
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

        //���ز������ļ����������·��
        public static string getAbsoluteExamOfficeDTFilePhysicalPath(string examUid, string examGradeUid, string questionUid, string officeMode)
        {
            return AppConfiguration.FileServerFileRootPath.TrimEnd('/') + "/" + getOppositeExamOfficeDTFileWebPath(examUid, examGradeUid, questionUid, officeMode).TrimStart('/');
        }

        //���ز������ļ���web����·��
        public static string getAbsoluteExamOfficeDTFileWebPath(string examUid, string examGradeUid, string questionUid, string officeMode)
        {
            return AppConfiguration.FileServerFileWebPathRoot.TrimEnd('/') + "/" + getOppositeExamOfficeDTFileWebPath(examUid, examGradeUid, questionUid, officeMode).TrimStart('/');
        }

        //���ز������ļ���web·��
        public static string getOppositeExamOfficeDTFileWebPath(string examUid, string examGradeUid, string questionUid, string officeMode)
        {
            string tempExamUid = ExamUidFilePrefix + examUid;
            string tempExamGradeUid = ExamGradeUidFilePrefix + examGradeUid;

            string fileName = OptAnswerFilePrefix + questionUid + getOfficeSuffix(officeMode);

            return "/" + ExamAnswerDirectory + "/" + tempExamUid + "/" + tempExamGradeUid + "/" + fileName;
        }

        //���ز������ļ���web·��
        public static string getOppositeExamOfficeDTFileWebPathWithoutFileName(string examUid, string examGradeUid)
        {
            string tempExamUid = ExamUidFilePrefix + examUid;
            string tempExamGradeUid = ExamGradeUidFilePrefix + examGradeUid;

            return "/" + ExamAnswerDirectory + "/" + tempExamUid + "/" + tempExamGradeUid;
        }

        //���ز������ļ� emply �� web ����·��
        public static string getAbsoluteExamOfficeDTEmptyFileWebPath(string officeMode)
        {
            return AppConfiguration.FileServerFileWebPathRoot.TrimEnd('/') + "/" + getOppositeExamOfficeDTEmptyFileWebPath(officeMode).TrimStart('/');
        }

        //���ز������ļ� emply �� web ·��
        public static string getOppositeExamOfficeDTEmptyFileWebPath(string officeMode)
        {
            return EmptFyFileName + getOfficeSuffix(officeMode);
        }

        //���ز������ļ��� web ��Ŀ¼ 
        public static string GetOppositeContentFileWebPathRoot(string questionUid)
        {
            return "/" + FilePathUtil.GetOppositeFileWebPathRoot(questionUid, "question");
        }

        //��������ļ���׺��
        public static string getOfficeSuffix(string office)
        {
            if (office != EnumOfficeModeCode.Word
                && office != EnumOfficeModeCode.Excel
                && office != EnumOfficeModeCode.Powerpoint
                && office != EnumOfficeModeCode.Visio
                && office != EnumOfficeModeCode.Html)
            {
                throw new ApplicationException(("δ֪ Office ���ͣ�"));
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
                return ".doc"; //Ĭ�Ϸ���word
            }
        }
    }

   
}
