using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Castle.Core.Logging;
using SPOC.Category;
using SPOC.Common.Cookie;
using SPOC.Common.Helper;
using SPOC.Exam;
using SPOC.User.Dto.StudentInfo;
using SPOC.User.Dto.Teacher;

namespace SPOC.User
{
   internal  class ImportUserInfoHelper
    {

  
        private readonly string _savePath;
        private readonly Guid _departmentUid;
        private readonly Guid _subjectUid;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;
        private readonly ILogger _iLogger;

        private readonly IUserInfoApiService _userInfoApiService;

        private readonly IUserInfoService _userInfoService;
        private const string Pattern = @"(?isx)
                      <({0})\b[^>]*>                  #开始标记“<tag...>”
                          (?>                         #分组构造，用来限定量词“*”修饰范围
                              <\1[^>]*>  (?<Open>)    #命名捕获组，遇到开始标记，入栈，Open计数加1
                          |                           #分支结构
                              </\1>  (?<-Open>)       #狭义平衡组，遇到结束标记，出栈，Open计数减1
                          |                           #分支结构
                              (?:(?!</?\1\b).)*       #右侧不为开始或结束标记的任意字符
                          )*                          #以上子串出现0次或任意多次
                          (?(Open)(?!))               #判断是否还有'OPEN'，有则说明不配对，什么都不匹配
                      </\1>                           #结束标记“</tag>”
                     ";
        public ImportUserInfoHelper(string rootPath )
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            _savePath = Path.Combine(rootPath, "temp", cookie.Id.ToString());

            
            try
            {
                if (!Directory.Exists(_savePath))
                {
                    Directory.CreateDirectory(_savePath);    
                }
            }
            catch (Exception)
            {
                throw new UserFriendlyException("无法创建临时存储文件夹");
            }

            _iLogger = new NullLogger();
        }

        public ImportUserInfoHelper(IUserInfoApiService userInfoApiService, IUserInfoService userInfoService) {
            _userInfoApiService = userInfoApiService;
            _userInfoService = userInfoService;
        }
      
        #region import excel

        public string EnterCharEconde(string content)
        {
            var bracketsCharBegin = "$BracketCharBegin$";
            var bracketsCharEnd = "$BracketCharEnd$";
            var bracketBeginFlag = "$BracketBegin$";
            var bracketEndFlag = "$BracketEnd$";

            content = content.Replace("\\{", bracketsCharBegin);
            content = content.Replace("\\}", bracketsCharEnd);
            content = content.Replace("{\r\n", bracketBeginFlag);
            content = content.Replace("\r\n}", bracketEndFlag);
            content = content.Replace("}", bracketEndFlag);
            //中文的括号也是同样的
            content = content.Replace("\\｛", bracketsCharBegin);
            content = content.Replace("\\｝", bracketsCharEnd);
            content = content.Replace("｛\r\n", bracketBeginFlag);
            content = content.Replace("\r\n｝", bracketEndFlag);
            content = content.Replace("｝", bracketEndFlag);
            var sNewContent = string.Empty;
            var sLeftContent = content; //剩余的
            //找到开头
            var nBeginPos = sLeftContent.IndexOf(bracketBeginFlag, StringComparison.Ordinal);
            //找到结尾
            var nEndPos = sLeftContent.IndexOf(bracketEndFlag, nBeginPos + 1, StringComparison.Ordinal);
            while (nBeginPos > -1 && nEndPos > -1)
            {
                //把前部分保存起来
                sNewContent = sNewContent + sLeftContent.Substring(0, nBeginPos + bracketBeginFlag.Length);
                //中间内容部分
                var middleContent = sLeftContent.Substring(nBeginPos + bracketBeginFlag.Length,
                    nEndPos + bracketEndFlag.Length - nBeginPos - bracketBeginFlag.Length); //中间部分内容
                //中间内容部分的回车换掉
                middleContent = middleContent.Replace("\r\n", "<br/>");
                //把转换后的内容保存起来
                sNewContent = sNewContent + middleContent;
                //把前部分去掉
                sLeftContent = sLeftContent.Substring(nEndPos + bracketEndFlag.Length);

                //找到开头
                nBeginPos = sLeftContent.IndexOf(bracketBeginFlag, StringComparison.Ordinal);
                //找到结尾
                nEndPos = sLeftContent.IndexOf(bracketEndFlag, nBeginPos + 1, StringComparison.Ordinal);
            }
            //把后面的接上去
            sNewContent = sNewContent + sLeftContent;
            return sNewContent;
        }

        public Dictionary<string, string> GetTableFieldList()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                {"id", (GetFieldTitle("id"))},
                {"questionCode", (GetFieldTitle("questionCode"))},
                {"questionText", (GetFieldTitle("questionText"))},
                {"selectAnswer", (GetFieldTitle("selectAnswer"))},
                {"standardAnswer", (GetFieldTitle("standardAnswer"))},
                {"examTime", (GetFieldTitle("examTime"))},
                {"questionAnalysis", (GetFieldTitle("questionAnalysis"))},
                {"outdatedDate", (GetFieldTitle("outdatedDate"))},
                {"isAnswerByHtml", (GetFieldTitle("isAnswerByHtml"))},
                {"listOrder", (GetFieldTitle("listOrder"))},
                {"creator", (GetFieldTitle("creator"))},
                {"createTime", (GetFieldTitle("createTime"))},
                {"modifier", (GetFieldTitle("modifier"))},
                {"modifyTime", (GetFieldTitle("modifyTime"))},
                {"hardGrade", (GetFieldTitle("hardGrade"))},
                {"score", (GetFieldTitle("score"))},
                {"eachOptionScore", (GetFieldTitle("eachOptionScore"))},
                {"folderUid", "试题分类"},
                {"questionTypeUid", "题型"},
                {"operateTypeCode", "操作题类型"},
                {"questionStatusCode", "试题状态"},
                {"questionKnowledge", "所属知识点"}
            };

            return dictionary;
        }

        public string GetFieldTitle(string fieldName)
        {
            switch (fieldName)
            {
                case "id":
                    return "系统编号";
                case "folderUid":
                    return "分类编号";
                case "parentQuestionUid":
                    return "上级试题系统编号";
                case "questionTypeUid":
                    return "题型编号";
                case "source":
                    return "试题来源";
                case "questionBaseTypeCode":
                    return "基本题型编号";
                case "operateTypeCode":
                    return "操作题类型";
                case "examTime":
                    return "答题时间";
                case "questionText":
                    return "试题内容";
                case "hardGrade":
                    return "难度";
                case "score":
                    return "分数";
                case "selectAnswer":
                    return "可选答案";
                case "selectAnswerScore":
                    return "可选答案分数";
                case "eachOptionScore":
                    return "每选项得扣分";
                case "answerNum":
                    return "可选答案个数";
                case "standardAnswer":
                    return "答案";
                case "questionAnalysis":
                    return "试题分析";
                case "outdatedDate":
                    return "过期日期";
                case "hasFile":
                    return "是否有文件";
                case "questionStatusCode":
                    return "试题状态";
                case "isAnswerByHtml":
                    return "是否用超文本答题";
                case "isOnlyUploadFile":
                    return "只允许上传附件";
                case "listOrder":
                    return "试题排序号";
                case "creator":
                    return "创建者";
                case "createTime":
                    return "创建时间";
                case "modifier":
                    return "修改者";
                case "modifyTime":
                    return "修改时间";
                case "approveUserUid":
                    return "审批者ID";
                case "approveUserName":
                    return "审批者名称";
                case "approveTime":
                    return "审批时间";
                case "questionCode":
                    return "试题编号";
                default:
                    return "";
            }
        }
        /// <summary>
        /// 把内容里的回车反编码,并把标致还原
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        public string EnterCharRevert(string sContent)
        {
            string sBracketsCharBegin = "$BracketCharBegin$";
            string sBracketsCharEnd = "$BracketCharEnd$";
            string sBracketBeginFlag = "$BracketBegin$";
            string sBracketEndFlag = "$BracketEnd$";
            string sEnterFlag = "$Enter$";
            string sNewContent = sContent;
            sNewContent = sNewContent.Replace(sBracketsCharBegin, "\\{");
            sNewContent = sNewContent.Replace(sBracketsCharEnd, "\\}");
            sNewContent = sNewContent.Replace(sEnterFlag, "\r\n");
            sNewContent = sNewContent.Replace(sBracketBeginFlag, "{\r\n");
            sNewContent = sNewContent.Replace(sBracketEndFlag, "\r\n}");

            return sNewContent;
        }

        //去掉每行两边的空格和Tab
        private string TrimContentList(string content)
        {
            //对内容里的回车进行编码
            string sQuestionList = EnterCharEconde(content);
            string[] arrLineText = StringUtil.Split(sQuestionList, "\r\n");
            StringBuilder sb = new StringBuilder();
            char[] trimStr = " \t".ToCharArray();
            foreach (string lineText in arrLineText)
            {
                //移除空格和Tab
                sb.Append(lineText.Trim(trimStr) + "\r\n"); //里面包括了移除空格和Tab
            }
            content = EnterCharRevert(sb.ToString());
            if (content != "") content = content.Substring(0, content.Length - 2);
            return content;
        }

       

        /// <summary>
        /// 把数据从Worksheet提取试题生成文本并把数据保存在Hidden变量里
        /// </summary>
        //public string SaveDataFromWorksheet(newv.excel.reader.Excel.Worksheet sheet)
        //{
        //    try
        //    {
        //        #region 读取EXCEL文件

        //        DataTable dtCacheData = new DataTable();
        //        newv.excel.reader.Excel.Row excelRow = sheet.Rows[2]; //表格头，第二行（标题行）
        //        HtmlTableRow tableRow = new HtmlTableRow();
        //        tableRow.Attributes["class"] = "HeadRow";
        //        int comCount = excelRow.Cells.LastCol;

        //        string formatValue;
        //        for (int i = 0; i <= comCount; ++i)
        //        {
        //            formatValue = excelRow.Cells[(byte)i].FormattedValue();
        //            DataColumn dcData = new DataColumn(formatValue);
        //            try
        //            {
        //                dtCacheData.Columns.Add(dcData);
        //            }
        //            catch
        //            {
        //                Random rd = new Random();
        //                dtCacheData.Columns.Add(dcData + rd.Next(0, 100).ToString());
        //            }

        //        }
        //        for (int i = 3; i <= sheet.Rows.LastRow; ++i)
        //        {

        //            excelRow = sheet.Rows[(ushort)i];
        //            if (excelRow != null)
        //            {
        //                DataRow drdata = dtCacheData.NewRow();

        //                for (int m = 0; m <= comCount; m++)
        //                {
        //                    var excelCell = excelRow.Cells[(byte)m];
        //                    formatValue = excelCell != null ? excelCell.FormattedValue() : "";
        //                    drdata[m] = formatValue;
        //                }
        //                dtCacheData.Rows.Add(drdata);
        //            }
        //        }

        //        #endregion

        //        #region 生成文本

        //        StringBuilder sb = new StringBuilder();
        //        int flag = 0;
        //        bool startChildQuestion = false;

        //        for (int i = 0; i < dtCacheData.Rows.Count; i++)
        //        {
        //            //处理子试题
        //            string childQuestionFieldName = "是否子试题";
        //            if (dtCacheData.Columns.Contains(childQuestionFieldName) &&
        //                dtCacheData.Rows[i][childQuestionFieldName] != null &&
        //                dtCacheData.Rows[i][childQuestionFieldName].ToString() == "Y")
        //            {
        //                if (startChildQuestion == false)
        //                {
        //                    sb.Append("[" + "开始子试题" + "]:\r\n\r\n");
        //                    startChildQuestion = true;
        //                }
        //            }
        //            else
        //            {
        //                if (startChildQuestion)
        //                {
        //                    sb.Append("[" + ("结束子试题") + "]:\r\n\r\n");
        //                    startChildQuestion = false;
        //                }
        //            }

        //            #region 处理试题

        //            flag++;
        //            //添加分类
        //            if (dtCacheData.Columns.Contains("试题分类"))
        //            {
        //                if (dtCacheData.Rows[i]["试题分类"].ToString().Trim().Length > 0)
        //                {
        //                    sb.Append("[" + "试题分类" + "]:" + dtCacheData.Rows[i]["试题分类"].ToString().Trim('\n') + "\r\n\r\n");
        //                }
        //            }

        //            if (dtCacheData.Columns.Contains("试题分类"))
        //            {
        //                if (dtCacheData.Rows[i]["试题分类"].ToString().Trim().Length > 0)
        //                {
        //                    sb.Append("[试题分类]:" + dtCacheData.Rows[i]["试题分类"].ToString().Trim('\n') + "\r\n\r\n");
        //                }
        //            }

        //            if (dtCacheData.Columns.Contains("试题内容"))
        //            {
        //                if (dtCacheData.Rows[i]["试题内容"].ToString().Trim().Length > 0)
        //                {
        //                    sb.Append(flag.ToString() + "." + dtCacheData.Rows[i]["试题内容"].ToString().Trim('\n') + "\r\n");
        //                }
        //            }


        //            #region 可选答案

        //            try
        //            {
        //                for (int n = 0; n < 26; n++)
        //                {
        //                   /* string selectChar = QuestionUtil.AnswerNumbersToChars(n.ToString());
        //                    if (dtCacheData.Rows[i]["可选答案" + selectChar].ToString().Trim().Length > 0)
        //                    {
        //                        string selectAnswer = dtCacheData.Rows[i]["可选答案" + selectChar].ToString().Trim('\n');
        //                        if (selectAnswer.Contains("\r\n"))
        //                        {
        //                            selectAnswer = "{\r\n" + selectAnswer + "\r\n}";
        //                        }
        //                        sb.Append(selectChar + "." + selectAnswer + "\r\n");
        //                    }*/
        //                }
        //            }
        //            catch
        //            {
        //                // ignored
        //            }

        //            #endregion

        //            #region//添加标准答案

        //            if (dtCacheData.Columns.Contains("题型"))
        //            {

        //                if (dtCacheData.Rows[i]["题型"].ToString().Contains("判断题"))
        //                {
        //                    if (dtCacheData.Rows[i]["答案"].ToString().ToUpper().Trim() == "Y")
        //                    {
        //                        sb.Append("答案" + ":" + "正确" + "\r\n");
        //                    }
        //                    else if (dtCacheData.Rows[i]["答案"].ToString().ToUpper().Trim() == "N")
        //                    {
        //                        sb.Append("答案" + ":" + ("错误") + "\r\n");
        //                    }
        //                    else
        //                    {
        //                        sb.Append("答案" + ":" + dtCacheData.Rows[i]["答案"].ToString().Trim('\n') + "\r\n");
        //                    }
        //                }
        //                else
        //                {
        //                    sb.Append("答案" + ":" + dtCacheData.Rows[i]["答案"].ToString().Trim('\n') + "\r\n");

        //                }
        //            }

        //            #endregion

        //            for (int c = 0; c < dtCacheData.Columns.Count; c++)
        //            {
        //                string colname = dtCacheData.Columns[c].ColumnName;
        //                if (!(colname.Contains("试题分类") || colname.Contains("试题内容") || colname.Contains("答案")))
        //                {

        //                    if (dtCacheData.Rows[i][colname].ToString().Trim().Length > 0)
        //                    {

        //                        sb.Append(colname + ":" + dtCacheData.Rows[i][colname].ToString().Trim('\n') + "\r\n");

        //                    }

        //                }
        //            }
        //            sb.Append("\r\n");

        //            #endregion

        //        }

        //        if (startChildQuestion)
        //        {
        //            sb.Append("[结束子试题]:\r\n\r\n");
        //        }


        //       // return ReplaceImportData(sb.ToString());
        //        return "";

        //        #endregion
        //    }
        //    catch (Exception)
        //    {
        //        return "";
        //    }
        //}

       
        public void ReplaceHtml(out string sContentList, string html)
        {
            string pattern = @"(?isx)

                      <({0})\b[^>]*>                  #开始标记“<tag...>”

                          (?>                         #分组构造，用来限定量词“*”修饰范围

                              <\1[^>]*>  (?<Open>)    #命名捕获组，遇到开始标记，入栈，Open计数加1

                          |                           #分支结构

                              </\1>  (?<-Open>)       #狭义平衡组，遇到结束标记，出栈，Open计数减1

                          |                           #分支结构

                              (?:(?!</?\1\b).)*       #右侧不为开始或结束标记的任意字符

                          )*                          #以上子串出现0次或任意多次

                          (?(Open)(?!))               #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                      </\1>                           #结束标记“</tag>”

                     ";

            var matchEvaluator = new MatchEvaluator((m) => m.Value.Replace("<br/>", ""));
            sContentList = Regex.Replace(html, string.Format(pattern, Regex.Escape("td")), matchEvaluator);
        }
        #endregion

     


       /// <summary>
       /// 下载Excel模板
       /// </summary>
        public void DownExcelMoudle() 
        { 
        
        }


        private readonly Regex mobileRegex = new Regex(@"^[1]+[3,5,8,7]+\d{9}");
        private readonly Regex emailRegex = new Regex(@"^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
        private readonly Regex userNameRegex = new Regex(@"/^(?=.*[a-zA-Z])(?=.*\d).*$/");
        private readonly Regex pwdRegex = new Regex(@"^[0-9a-zA-Z]\w{4,20}$");
        private readonly Regex idCardRegex = new Regex(@"/(^\d{15}$)|(^\d{18}$)|(^\d{17}[X]|[x]$)/");

        /// <summary>
        /// excel学生导入数据检查
        /// </summary>
        /// <param name="model"></param>
        /// <param name="list"></param>
        /// <param name="msg"></param>
        /// <param name="isUsernameContainEN"></param>
        /// <returns></returns>
        public bool CheckTeacherImportDate(tacher_info_import model, List<tacher_info_import> list, ref string msg, bool isUsernameContainEN)
        {
            DateTime outTime;

            if (string.IsNullOrEmpty(model.user_login_name))
            {
                msg += "登录名不能为空！; ";
            }
            else
            {
                //if (!userNameRegex.IsMatch(model.login_name))
                if (model.user_login_name.Length > 20 || model.user_login_name.Length < 4)
                {
                    msg += "登录名必须大于4个字符或小于50个字符，一个中文算2个字符;  ";
                }
                else if (isUsernameContainEN)
                {
                    if (!userNameRegex.IsMatch(model.user_login_name))
                    {
                        msg += "登录名必须是英文和数字的组合;  ";
                    }
                }

                if (list.Count(a => a.user_login_name == model.user_login_name) > 1)
                {
                    //var uList = list.Where(a => a.login_name == model.login_name).ToList();
                    msg += "登录名与其他数据重复;  ";
                }


            }
            if (string.IsNullOrEmpty(model.user_password))
            {
                msg += "密码不能为空！; ";

            }
            else
            {
                //if (!pwdRegex.IsMatch(model.user_password))
                if (model.user_password.Length > 20 || model.user_password.Length < 5)
                {
                    //msg += "密码格式不正确，(5-20位英文、数字、符号)！;  ";
                    msg += "密码不超过20个字符,最短不能低于5个字符！;  ";
                }
            }
            if (string.IsNullOrEmpty(model.user_mobile))
            {
                msg += "手机号不能为空！; ";
            }
            else
            {
                if (!mobileRegex.IsMatch(model.user_mobile) || model.user_mobile.Length != 11)
                {
                    msg += "手机号码格式不正确！;  ";
                }
                else if (list.Count(a => a.user_mobile == model.user_mobile) > 1)
                {
                    //var uList = list.Where(a => a.login_name == model.login_name).ToList();
                    msg += "手机号码与其他数据重复;  ";
                }
            }
            if (string.IsNullOrEmpty(model.teacherCode))
            {
                msg += "教师号不能为空！;  ";

            }
            else if (list.Count(a => a.teacherCode == model.teacherCode) > 1)
            {
                //var uList = list.Where(a => a.login_name == model.login_name).ToList();
                msg += "学号与其他数据重复;  ";
            }
            else
            {
                if (!string.IsNullOrEmpty(model.user_birthday))
                {
                    try
                    {
                        DateTime.Parse(model.user_birthday);
                    }
                    catch
                    {
                        msg += "生日格式不正确;  ";
                    }
                }
                if (!string.IsNullOrEmpty(model.teacherGraduateDate))
                {
                    try
                    {
                        DateTime.Parse(model.teacherGraduateDate);
                    }
                    catch
                    {
                        msg += "毕业时间格式不正确;  ";
                    }

                }
                if (!string.IsNullOrEmpty(model.teacherEntryDate))
                {
                    try
                    {
                        DateTime.Parse(model.teacherEntryDate);
                    }
                    catch
                    {
                        msg += "任职日期格式不正确;  ";
                    }
                }
                if (!string.IsNullOrEmpty(model.teacherEduAge))
                {
                    try
                    {
                        float.Parse(model.teacherEduAge);
                    }
                    catch
                    {
                        msg += "高校教龄格式不正确;  ";
                    }
                }
                if (!string.IsNullOrEmpty(model.teacherStartworkDate))
                {
                    try
                    {
                        DateTime.Parse(model.teacherStartworkDate);
                    }
                    catch
                    {
                        msg += "参加工作日期格式不正确;  ";
                    }
                }
            }

            if (string.IsNullOrEmpty(msg))
            {
                if (!_userInfoService.CheckNameExit(model.user_login_name, "insert", "", false))
                {
                    msg = "登录名已存在; ";
                }
                if (!_userInfoService.CheckMobileExist(model.user_mobile, "insert", "", false))
                {
                    msg += "手机号已存在; ";
                }
                if (!string.IsNullOrEmpty(model.user_idcard))
                {
                    if (!idCardRegex.IsMatch(model.user_idcard))
                    {
                        msg += "身份证号码格式不正确;  ";
                    }
                    else if (list.Count(a => a.user_idcard == model.user_idcard) > 1)
                    {
                        //var uList = list.Where(a => a.login_name == model.login_name).ToList();
                        msg += "身份证号码与其他数据重复;  ";
                    }
                    else if (!_userInfoService.CheckIdCardExist(model.user_idcard, "insert", ""))
                    {
                        msg += "身份证号已存在;  ";
                    }
                }
                if (!string.IsNullOrEmpty(model.user_email))
                {
                    /* if (!emailRegex.IsMatch(model.user_email)) {
                         msg += "邮箱格式不正确;  ";
                     }
                     else */

                    if (list.Count(a => a.user_email == model.user_email) > 1)
                    {
                        //var uList = list.Where(a => a.login_name == model.login_name).ToList();
                        msg += "邮箱与其他数据重复;  ";
                    }
                    else if (!_userInfoService.CheckEmailExist(model.user_email, "insert", "", false))
                    {
                        msg += "邮箱已存在; ";
                    }
                }
            }
            return string.IsNullOrEmpty(msg) ? true : false;
        }
       

       /// <summary>
       /// excel学生导入数据检查
       /// </summary>
       /// <param name="model"></param>
       /// <param name="list"></param>
       /// <param name="msg"></param>
       /// <param name="isUsernameContainEN"></param>
       /// <returns></returns>
        public bool CheckUserInfo(user_info_import model, List<user_info_import> list, ref string msg, bool isUsernameContainEN)
        {
            DateTime outTime;
           
            if (string.IsNullOrEmpty(model.login_name))
            {
                msg+= "登录名不能为空！; ";
            }
            else {
               //if (!userNameRegex.IsMatch(model.login_name))
                if (model.login_name.Length > 20 || model.login_name.Length<4)
                {
                    msg += "登录名必须大于4个字符或小于20个字符，一个中文算2个字符;  ";
                }
                else if (isUsernameContainEN) {
                    if (!userNameRegex.IsMatch(model.login_name)) {
                        msg += "登录名必须是英文和数字的组合;  ";
                    }
                }
                
                if (list.Count(a => a.login_name == model.login_name) > 1)
                {
                    //var uList = list.Where(a => a.login_name == model.login_name).ToList();
                    msg += "登录名与其他数据重复;  ";
                }


            }
            if (string.IsNullOrEmpty(model.user_password))
            {
                msg += "密码不能为空！; ";

            }
            else {
                //if (!pwdRegex.IsMatch(model.user_password))
                if (model.user_password.Length > 20 || model.user_password.Length<5)
                {
                    //msg += "密码格式不正确，(5-20位英文、数字、符号)！;  ";
                    msg += "密码不超过20个字符,最短不能低于5个字符！;  ";
                }
            }
            if (string.IsNullOrEmpty(model.user_mobile))
            {
                //msg += "手机号不能为空！; ";
            }
            else
            {
                if (!mobileRegex.IsMatch(model.user_mobile) || model.user_mobile.Length != 11)
                {
                    msg += "手机号码格式不正确！;  ";
                }
                else if (list.Count(a => a.user_mobile == model.user_mobile) > 1)
                {
                   
                    msg += "手机号码与其他数据重复;  ";
                }
            }
            if (string.IsNullOrEmpty(model.user_code))
            {
               // msg += "学号不能为空！;  ";

            }
            else if (list.Count(a => a.user_code == model.user_code) > 1)
            {
               
                msg += "学号与其他数据重复;  ";
            }
            else {
                if (!string.IsNullOrEmpty(model.user_birthday))
                {
                    try
                    {
                        DateTime.Parse(model.user_birthday);
                    }
                    catch
                    {
                        msg += "生日格式不正确;  ";
                    }
                }
                if (!string.IsNullOrEmpty(model.graduation_date))
                {
                    try
                    {
                        DateTime.Parse(model.graduation_date);
                    }
                    catch
                    {
                        msg += "毕业时间格式不正确;  ";
                    }

                }
                if (!string.IsNullOrEmpty(model.user_admission))
                {
                    try
                    {
                        DateTime.Parse(model.user_admission);
                    }
                    catch
                    {
                        msg += "入学时间格式不正确;  ";
                    }
                }
            }

            if (string.IsNullOrEmpty(msg))
            {
                if (!_userInfoService.CheckNameExit(model.login_name, "insert","",false))
                {
                    msg = "登录名已存在; ";
                }
                if (!string.IsNullOrEmpty(model.user_mobile) &&
                    !_userInfoService.CheckMobileExist(model.user_mobile, "insert", "", false))
                    msg += "手机号已存在; ";
                if (!string.IsNullOrEmpty(model.user_idcard)   )
                {
                    if (!idCardRegex.IsMatch(model.user_idcard))
                    {
                        msg += "身份证号码格式不正确;  ";
                    }
                    else if (list.Count(a => a.user_idcard == model.user_idcard) > 1)
                    {
                        //var uList = list.Where(a => a.login_name == model.login_name).ToList();
                        msg += "身份证号码与其他数据重复;  ";
                    }
                    else if (!_userInfoService.CheckIdCardExist(model.user_idcard, "insert", "")) {
                        msg += "身份证号已存在;  ";
                    }
                }
                if (!string.IsNullOrEmpty(model.user_email))
                {
                   /* if (!emailRegex.IsMatch(model.user_email)) {
                        msg += "邮箱格式不正确;  ";
                    }
                    else */
                        
                        if (list.Count(a => a.user_email == model.user_email) > 1)
                    {
                        //var uList = list.Where(a => a.login_name == model.login_name).ToList();
                        msg += "邮箱与其他数据重复;  ";
                    }
                    else if (!_userInfoService.CheckEmailExist(model.user_email, "insert", "",false)) { 
                     msg += "邮箱已存在; ";
                    }
                }
            }
            return string.IsNullOrEmpty(msg)? true:false;
        }
       
        
    }

    }

