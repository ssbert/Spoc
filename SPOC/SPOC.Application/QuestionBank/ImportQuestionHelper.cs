using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Aspose.Words;
using Aspose.Words.Saving;
using HtmlAgilityPack;
using SPOC.Category;
using SPOC.Common.Cookie;
using SPOC.Common.File;
using SPOC.Exam;
using SPOC.QuestionBank.Const;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using newv.common;
using SPOC.Core;
using SPOC.Lib;
using DateTimeUtil = SPOC.Exam.DateTimeUtil;
using Html2TextUtil = SPOC.Common.Helper.htmlparser.Html2TextUtil;
using ReturnValue = SPOC.Common.ReturnValue;

namespace SPOC.QuestionBank
{
    /*这些都是原来旧考试系统代码的移植代码，代码构成有点糟糕，先保证能运行再考虑重构，也许永远不会有重构的机会
     * by leente 2016/09/24
     * */
    internal class ImportQuestionHelper:ImportQuestionBaseHelper
    {
        
        
        public ImportQuestionHelper(string rootPath, Guid folderUid, IRepository<NvFolder, Guid> iNvFolderRep, IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep, IUnitOfWorkManager iUnitOfWorkManager, IRepository<QuestionStandardCode, Guid> iQuestionStandardCodeRep, IRepository<Label, Guid> iLabelRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep,string newMoocApiUrl,string language) : base( rootPath, folderUid, iNvFolderRep, iExamQuestionRep, iExamQuestionTypeRep, iUnitOfWorkManager, iQuestionStandardCodeRep,iLabelRep,iQuestionLabelRep, newMoocApiUrl, language)
        {
        }

        #region import word
        public void ImportFromWord(Stream stream, out int scCount, out string errMsg)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var awd = new Document(stream);
            var fileName = "ImportFromWord";
            
            var filePath = Path.Combine(_savePath, fileName + ".html");
            var opt = new HtmlSaveOptions();
            opt.SaveFormat = SaveFormat.Html;
            opt.Encoding = Encoding.UTF8;
            awd.Save(filePath, opt);

            //修改html里的内容，只保留body内的数据，不包含body
            var document = new HtmlDocument();
            document.Load(filePath, Encoding.UTF8);
            var bodyDom = document.DocumentNode.Descendants("body").FirstOrDefault();
            if (bodyDom == null)
            {
                throw new UserFriendlyException("载入数据异常");
            }

           

            //var divDom = bodyDom.FirstChild;
            //divDom.RemoveChild(divDom.FirstChild);//去水印

            var innerHtml = bodyDom.InnerHtml;
            innerHtml = innerHtml.Replace(" text-align:justify;", "").Replace("&#xa0;", " ");

            File.WriteAllText(filePath, innerHtml, Encoding.UTF8);
            
            var txtFilePath = Path.Combine(_savePath, fileName + ".txt");
            string errorMessage = Html2TextUtil.ConvertHtml2TxtFile(filePath, txtFilePath);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new UserFriendlyException("载入数据异常");
            }

            string content;
            try
            {
                content = File.ReadAllText(txtFilePath, Encoding.UTF8);
                //content = HttpUtility.HtmlDecode(content);
                content = ReplaceImportData(content);
            }
            catch (Exception)
            {
                throw new UserFriendlyException("载入数据异常");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new UserFriendlyException("载入数据异常");
            }
            Hashtable publicProperty = new Hashtable();
            publicProperty.Clear();

            string preQuestionUid = string.Empty;
            scCount = 0;
            int errCount = 0;
            errMsg = string.Empty;
            var importContent = string.Empty;
            var contentList = ReplaceHtml(content);
            //对内容进行编码
            contentList = EnterCharEconde(contentList) + "\r\n\r\n";
            int i = 0;
            int nPos = contentList.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            while (nPos > -1)
            {
                try
                {
                    //=============2. 取公共属性 ================
                    var sOneContent = contentList.Substring(0, nPos);
                    if (sOneContent != "" && sOneContent.StartsWith("[") &&
                        (sOneContent.IndexOf("]:", StringComparison.Ordinal) >= 0 ||
                         sOneContent.IndexOf("]：", StringComparison.Ordinal) >= 0))
                    {
                        #region 公共属性

                        sOneContent = sOneContent.Replace("：", ":");
                        string sPropertyName = sOneContent.Substring(1,
                            sOneContent.IndexOf("]:", StringComparison.Ordinal) - 1);
                        string sPropertyValue =
                            sOneContent.Substring(sOneContent.IndexOf("]:", StringComparison.Ordinal) + 2);

                        if (sPropertyName == "试题分类") ////试题目录
                        {
                            if (publicProperty.ContainsKey("folderName"))
                            {
                                publicProperty["folderName"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("folderName", sPropertyValue);
                            }
                        }
                        else if (sPropertyValue != "" && sPropertyName == "过期日期") ////过期时间
                        {
                            if (publicProperty.ContainsKey("outdatedDate"))
                            {
                                publicProperty["outdatedDate"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("outdatedDate", sPropertyValue);
                            }
                        }
                        else if (sPropertyValue != "" && sPropertyName == "试题状态") //状态
                        {
                            if (publicProperty.ContainsKey("questionStatusCode"))
                            {
                                publicProperty["questionStatusCode"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("questionStatusCode", sPropertyValue);
                            }
                        }
                        else if (sPropertyName == "题型") //题型
                        {
                            if (publicProperty.ContainsKey("questionTypeUid"))
                            {
                                publicProperty["questionTypeUid"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("questionTypeUid", sPropertyValue);
                            }
                        }
                        else if (sPropertyName == ("开始子试题")) //开始子试题
                        {
                            if (publicProperty.ContainsKey("parentQuestionUid"))
                            {
                                publicProperty["parentQuestionUid"] = preQuestionUid;
                            }
                            else
                            {
                                publicProperty.Add("parentQuestionUid", preQuestionUid);
                            }
                        }
                        else if (sPropertyName == ("结束子试题")) //结束子试题
                        {
                            if (publicProperty.ContainsKey("parentQuestionUid"))
                            {
                                publicProperty["parentQuestionUid"] = "";
                            }
                            else
                            {
                                publicProperty.Add("parentQuestionUid", "");
                            }
                        }
                        else if (sPropertyValue == "程序参数")
                        {
                            if (publicProperty.ContainsKey("param"))
                            {
                                publicProperty["param"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("param", sPropertyValue);
                            }
                        }
                        else if (sPropertyValue == "输入流参数")
                        {
                            if (publicProperty.ContainsKey("inputParam"))
                            {
                                publicProperty["inputParam"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("inputParam", sPropertyValue);
                            }
                        }
                        else if (sPropertyValue == "多次测试")
                        {
                            if (publicProperty.ContainsKey("multiTest"))
                            {
                                publicProperty["multiTest"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("multiTest", sPropertyValue);
                            }
                        }
                        else if (sPropertyValue == "预设代码")
                        {
                            if (publicProperty.ContainsKey("preinstallCode"))
                            {
                                publicProperty["preinstallCode"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("preinstallCode", sPropertyValue);
                            }
                        }
                        else if (sPropertyValue == "参考答案")
                        {
                            if (publicProperty.ContainsKey("standardCode"))
                            {
                                publicProperty["standardCode"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("standardCode", sPropertyValue);
                            }
                        }
                        else if (sPropertyValue == "主标签")
                        {
                            if (publicProperty.ContainsKey("label"))
                            {
                                publicProperty["label"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("label", sPropertyValue);
                            }
                        }
                        else if (sPropertyValue == "辅标签")
                        {
                            if (publicProperty.ContainsKey("auxiliaryLabel"))
                            {
                                publicProperty["auxiliaryLabel"] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add("auxiliaryLabel", sPropertyValue);
                            }
                        }
                        //else if (sPropertyName == ("父试题分类")) //父试题分类
                        //{
                        //    string newParentFolderUid;
                        //    if (string.IsNullOrEmpty(sPropertyValue))
                        //    {
                        //        newParentFolderUid = string.Empty;
                        //    }
                        //    else
                        //    {
                        //        newParentFolderUid = GetFolderUid("question_bank",
                        //            sPropertyValue, cookie.Id, true).ToString();
                        //    }
                        //    publicProperty["parentUid"] = newParentFolderUid;
                        //}
                        else
                        {
                            if (publicProperty.ContainsKey(sPropertyName))
                            {
                                publicProperty[sPropertyName] = sPropertyValue;
                            }
                            else
                            {
                                publicProperty.Add(sPropertyName, sPropertyValue);
                            }
                        }

                        #endregion
                    }
                    else if (sOneContent != "" && !sOneContent.StartsWith("//"))
                    {
                        Dictionary<string, string> questionField = new Dictionary<string, string>();
                        Dictionary<string, string> tempField = GetTableFieldList();
                        questionField.Clear();
                        foreach (string key in tempField.Keys)
                        {
                            questionField.Add(key, tempField[key]);
                        }
                        importContent = sOneContent;
                        var retValue = ConvertTextToQuestion(sOneContent, questionField, publicProperty, cookie.Id);
                        if (retValue.HasError)
                        {
                            errMsg += $"[{importContent}]导入失败:" + retValue.Message +
                                     Environment.NewLine+ Environment.NewLine;
                            errCount += 1;
                        }
                        else
                        {
                            ExamQuestion quRow = (ExamQuestion) retValue.GetValue("question");
                            preQuestionUid = quRow.Id.ToString();

                            //如果是从Word导入,则处理图片
                            if (retValue.HasError == false)
                            {
                                scCount += 1;

                                string sContentText = quRow.questionText;
                                string selectAnswer = quRow.selectAnswer;
                                string questionAnslysis = quRow.questionAnalysis ?? "";
                                string standardAnswer = quRow.standardAnswer;
                                string sSourceFilePath = FilePathUtil.GetOppositeUserTempPath(cookie.Id.ToString());
                                bool hasCopyContentFile = false;
                                var retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref sContentText, sSourceFilePath);
                                if (retValueCopyFile.HasError)
                                {
                                    if (retValueCopyFile.Message != "")
                                    {
                                        errMsg += $"[{sContentText}]导入成功，但处理文件失败::" + retValue.Message +
                                                  Environment.NewLine + Environment.NewLine;
                                      
                                    }
                                }
                                else
                                {
                                    hasCopyContentFile = true;
                                }
                                bool hasCopySelectAnswerFile = false;
                                retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref selectAnswer, sSourceFilePath);
                                if (retValueCopyFile.HasError == false)
                                {
                                    hasCopySelectAnswerFile = true;
                                }
                                bool hasCopyStandardAnswerFile = false;
                                retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref standardAnswer, sSourceFilePath);
                                if (retValueCopyFile.HasError == false)
                                {
                                    hasCopyStandardAnswerFile = true;
                                }
                                bool hasCopyQuestionAnslysisFile = false;
                                retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref questionAnslysis, sSourceFilePath);
                                if (retValueCopyFile.HasError == false)
                                {
                                    hasCopyQuestionAnslysisFile = true;
                                }

                                if (hasCopyContentFile || hasCopySelectAnswerFile || hasCopyStandardAnswerFile || hasCopyQuestionAnslysisFile)
                                {
                                    quRow.questionText = sContentText;
                                    quRow.selectAnswer = selectAnswer;
                                    quRow.standardAnswer = standardAnswer;
                                    quRow.questionAnalysis = questionAnslysis;
                                    _iExamQuestionRep.UpdateAsync(quRow);
                                }

                            }
                            else
                            {
                                errMsg += $"[{importContent}]导入失败:" + retValue.Message +
                                          Environment.NewLine + Environment.NewLine;
                                errCount += 1;
                            }
                        }
                        i = i + 1;
                    }
                }
                catch (Exception ex)
                {
                    errMsg += $"[{importContent}]导入失败:" + ex.Message +
                              Environment.NewLine + Environment.NewLine;
                    
                    errCount += 1;
                }
                contentList = contentList.Substring(nPos + 4);
                nPos = contentList.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            }
            if (errCount > 0)
            {
                errMsg = errMsg + string.Format(("[提示：]导入试题完毕，{0}个试题导入失败，{1}个试题成功！"), errCount, scCount);
            }
            stream.Close();
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
        /// 把内容里的回车反编码,并把标致还原
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        public  string EnterCharRevert(string sContent)
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

        /// <summary>
        /// 验证试题的格式
        /// </summary>
        /// <param name="sOneContent"></param>
        /// <param name="questionField"></param>
        /// <param name="publicProperty"></param>
        /// <param name="questionIndex"></param>
        /// <returns></returns>
        public ReturnValue CheckQuestionTextFormat(string sOneContent, Dictionary<string, string> questionField, Hashtable publicProperty, int questionIndex)
        {
            ReturnValue reValue = new ReturnValue(false, "");
            ArrayList errorMessage = new ArrayList();

            //处理试题前的序号
            if (sOneContent.IndexOf(".", StringComparison.Ordinal) > -1 &&
                StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf(".", StringComparison.Ordinal))) )
            {
                sOneContent = sOneContent.Substring(sOneContent.IndexOf(".", StringComparison.Ordinal) + 1);
            }
            else if (sOneContent.IndexOf("。", StringComparison.Ordinal) > -1 &&
                     StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf("。", StringComparison.Ordinal))) )
            {
                sOneContent = sOneContent.Substring(sOneContent.IndexOf("。", StringComparison.Ordinal) + 1);
            }
            else if (sOneContent.IndexOf("．", StringComparison.Ordinal) > -1 &&
                     StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf("．", StringComparison.Ordinal))) )
            {
                sOneContent = sOneContent.Substring(sOneContent.IndexOf("．", StringComparison.Ordinal) + 1);
            }
            else if (sOneContent.IndexOf("、", StringComparison.Ordinal) > -1 &&
                     StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf("、", StringComparison.Ordinal))) )
            {
                sOneContent = sOneContent.Substring(sOneContent.IndexOf("、", StringComparison.Ordinal) + 1);
            }
            else if (sOneContent.IndexOf(" ", StringComparison.Ordinal) > -1 &&
                     StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf(" ", StringComparison.Ordinal))) )
            {
                sOneContent = sOneContent.Substring(sOneContent.IndexOf(" ", StringComparison.Ordinal) + 1);
            }
            sOneContent = (questionIndex + 1) + "." + sOneContent;

            //=============3. 处理试题属性 =================
            Dictionary<string, string> properties = GetContentProperty(sOneContent, questionField);
            bool hasText = false;
            bool hasType = false;
            bool hasAnswer = false;
            string questionType = string.Empty;
            foreach (string pkey in properties.Keys)
            {
                if (pkey == questionField["questionTypeUid"])
                {
                    questionType = properties[pkey];
                }

                if (pkey == questionField["questionCode"])
                {
                    var code = properties[pkey];
                    var entity = _iExamQuestionRep.FirstOrDefault(p => p.questionCode == code);
                    if (entity != null)
                    {
                        errorMessage.Add(string.Format("{0}试题编号已存在", code));
                        reValue.HasError = true;
                    }
                }
            }

            //扩展属性


            ExamQuestionType qtRow = null;
            if (string.IsNullOrEmpty(questionType) && publicProperty.ContainsKey("questionTypeName"))
            {
                questionType = publicProperty["questionTypeName"].ToString();
            }
            if (questionType == "")
            {
                errorMessage.Add(("找不到题型属性"));
                reValue.HasError = true;
            }
            else
            {
                qtRow = _iExamQuestionTypeRep.FirstOrDefault(a => a.questionTypeName.Equals(questionType));
                if (qtRow == null)
                {
                    errorMessage.Add(string.Format(("题型'{0}'不支持"), questionType));
                    reValue.HasError = true;
                }
            }
            foreach (string pkey in properties.Keys)
            {
                if (pkey != "序号" && !questionField.ContainsValue(pkey))
                {
                    errorMessage.Add(("未知的属性") + ":" + pkey);
                    reValue.HasError = true;
                }
                else if (pkey == questionField["questionText"])
                {
                    //题干
                    hasText = true;
                    if (string.IsNullOrEmpty(properties[pkey]))
                    {
                        errorMessage.Add(("未找到试题内容"));
                        reValue.HasError = true;
                    }
                }
                else if (pkey == questionField["questionTypeUid"])
                {
                    //题型
                    hasType = true;

                    if (qtRow == null)
                    {
                        errorMessage.Add(("无效的题型") + ":" + properties[pkey]);
                        reValue.HasError = true;
                    }
                }
                else if (pkey == questionField["operateTypeCode"])
                {
                    //操作题类型
                    if (qtRow != null && qtRow.questionBaseTypeCode == QuestionTypeConst.Operate)
                    {
                        string fieldTitle = properties[pkey];
                        string fieldValue = fieldTitle;
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            errorMessage.Add(("无效的属性值") + ":" + fieldTitle);
                            reValue.HasError = true;
                        }
                    }
                }
                else if (questionField.ContainsKey("score") &&
                    pkey == questionField["score"])
                {
                    //分数
                    if (!StringUtil.IsNumber(properties[pkey]))
                    {
                        errorMessage.Add(("无效的属性值") + ":" + properties[pkey]);
                        reValue.HasError = true;
                    }
                }
                else if (pkey == questionField["listOrder"])
                {
                    //顺序号
                    if (!StringUtil.IsNumber(properties[pkey]))
                    {
                        errorMessage.Add(("无效的属性值") + ":" + properties[pkey]);
                        reValue.HasError = true;
                    }
                }
                else if (pkey == questionField["outdatedDate"] ||
                         pkey == questionField["modifyTime"] ||
                         pkey == questionField["createTime"])
                {
                    //过期时间
                    if (!string.IsNullOrEmpty(properties[pkey]) && !DateTimeUtil.IsDateTime(properties[pkey]))
                    {
                        errorMessage.Add(("无效的属性值") + ":" + properties[pkey]);
                        reValue.HasError = true;
                    }
                }
                else if (pkey == questionField["examTime"] &&
                         !string.IsNullOrEmpty(properties[pkey]))
                {
                    //考试时间
                    if (!DateTimeUtil.IsDateTime(properties[pkey].Trim()))
                    {
                        errorMessage.Add(("无效的答题时间") + ":" + properties[pkey]);
                        reValue.HasError = true;
                    }
                }
                else if (pkey == questionField["selectAnswer"])
                {
                    // string selectAnswer = properties[pkey].ToString();
                    if (questionType == "判断题" && properties[pkey].Length > 0)
                    //peace 修改|判断题型 不准填写可选答案 否则提示用户清空填写内容
                    {
                        errorMessage.Add(("判断题不可填写可选答案") + ":" + pkey);
                        reValue.HasError = true;
                    }
                }
                else if (pkey == questionField["standardAnswer"])
                {
                    hasAnswer = true;

                    #region 检查答案的合法性

                    string sStandardAnswer = properties[pkey].Trim();
                    if (qtRow != null)
                    {
                        if (qtRow.questionBaseTypeCode == QuestionTypeConst.Single)
                        {
                            //检查答案是否为空
                            if (string.IsNullOrEmpty(sStandardAnswer))
                            {
                                errorMessage.Add(("答案不能为空"));
                                reValue.HasError = true;
                            }
                            //单选答案长度超过1一定有误
                            else if (sStandardAnswer.Length > 1)
                            {
                                errorMessage.Add(("答案格式不正确"));
                                reValue.HasError = true;
                            }
                            else
                            {
                                string[] arrSelectAnswer = properties[("可选答案")].Split('|');
                                //将答案转化为标准的数字型
                                string temporarySingleAnswer = sStandardAnswer;
                                //定义临时的答案项，因为下面将覆盖，用于输入了汉字转化失败后取出作提示
                                sStandardAnswer = QuestionUtil.AnswerCharsToNumbers(sStandardAnswer);
                                if (string.IsNullOrEmpty(sStandardAnswer)) //输入汉字转化后则为空
                                {
                                    errorMessage.Add(string.Format(("答案'{0}'不在可选范围内"),
                                        temporarySingleAnswer));
                                    reValue.HasError = true;
                                }
                                else if (Convert.ToInt32(sStandardAnswer) >
                                         Convert.ToInt32(arrSelectAnswer.Length) - 1) //判断答案是否在可选范围内
                                {
                                    errorMessage.Add(string.Format(
                                        ("答案'{0}'不在可选范围内"), temporarySingleAnswer));
                                    reValue.HasError = true;
                                }
                            }
                        }
                        else if (qtRow.questionBaseTypeCode == QuestionTypeConst.Multi)
                        {
                            //检查答案是否为空
                            if (string.IsNullOrEmpty(sStandardAnswer))
                            {
                                errorMessage.Add(("答案不能为空"));
                                reValue.HasError = true;
                            }
                            //格式化答案并检查
                            string msg;
                            sStandardAnswer = QuestionUtil.FormatSelectQuestionAnswer(sStandardAnswer,
                                out msg);
                            if (msg != "")
                            {
                                errorMessage.Add(("答案格式不正确"));
                                reValue.HasError = true;
                            }
                            else
                            {
                                string[] arrSelectAnswer = properties[("可选答案")].Split('|');
                                string[] arrAnswer = sStandardAnswer.Split('|');
                                foreach (string answer in arrAnswer)
                                {
                                    var temporaryMultiAnswer = QuestionUtil.AnswerCharsToNumbers(answer); //临时变量
                                    if (!string.IsNullOrEmpty(temporaryMultiAnswer))
                                    {
                                        if (Convert.ToInt32(temporaryMultiAnswer) >
                                            Convert.ToInt32(arrSelectAnswer.Length) - 1)
                                        {
                                            errorMessage.Add(
                                                string.Format(("答案'{0}'不在可选范围内"),
                                                    answer));
                                            reValue.HasError = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (qtRow.questionBaseTypeCode == QuestionTypeConst.Judge)
                        {
                            if (string.IsNullOrEmpty(sStandardAnswer))
                            {
                                errorMessage.Add(("答案不能为空"));
                                reValue.HasError = true;
                            }
                            if (sStandardAnswer != ("错误") &&
                                sStandardAnswer != ("正确"))
                            {
                                errorMessage.Add(("未知的答案") + "'" + sStandardAnswer + "'");
                                reValue.HasError = true;
                            }
                        }
                        else if (qtRow.questionBaseTypeCode == QuestionTypeConst.Fill)
                        {
                            if (string.IsNullOrEmpty(sStandardAnswer))
                            {
                                errorMessage.Add(("答案不能为空"));
                                reValue.HasError = true;
                            }
                            else
                            {
                                int fillBoxCount = QuestionUtil.GetFillInBoxCount(sOneContent);
                                string[] arrStandardAnswer = sStandardAnswer.Split('|');
                                if (fillBoxCount != arrStandardAnswer.Length)
                                {
                                    errorMessage.Add(
                                        string.Format(
                                            ("填空类型答案个数与空数不一致,答案个数为{0},空数为：{1}"),
                                            arrStandardAnswer.Length.ToString(), fillBoxCount.ToString()));
                                    reValue.HasError = true;
                                }
                            }
                        }
                    }

                    #endregion
                }
                else if (pkey == questionField["questionStatusCode"])
                {
                    //试题状态
                    if (!string.IsNullOrEmpty(properties[pkey]))
                    {
                        string sPropertyValue;
                        switch (properties[pkey])
                        {
                            case "正常":
                                sPropertyValue = "normal";
                                break;
                            case "已过期":
                                sPropertyValue = "outdated";
                                break;
                            case "禁用":
                                sPropertyValue = "disabled";
                                break;
                            default:
                                sPropertyValue = "";
                                break;
                        }

                        if (sPropertyValue == "")
                        {
                            errorMessage.Add(("未知的状态") + sPropertyValue);
                            reValue.HasError = true;
                        }
                    }
                }
                else if (pkey == questionField["isAnswerByHtml"])
                {
                    //是否使用超文本答题
                    if (!string.IsNullOrEmpty(properties[pkey]))
                    {
                        string isAnswerByHtml = properties[pkey].ToUpper();
                        if (isAnswerByHtml != "Y" && isAnswerByHtml != "N")
                        {
                            errorMessage.Add(("无效的属性值") + ":" + properties[pkey]);
                            reValue.HasError = true;
                        }
                    }
                }

                else if (pkey == questionField["hardGrade"])
                {
                    if (!string.IsNullOrEmpty(properties[pkey]))
                    {
                        // string ishasgrade = properties[pkey].ToUpper();
                        string ishasgrade = properties[pkey];
                        if (ishasgrade.Length >= 2)
                        {
                            errorMessage.Add(("无效的属性值HardGrade") + ":" + properties[pkey]);
                            reValue.HasError = true;
                        }
                    }
                }

            }
            if (!hasText)
            {
                errorMessage.Add(("未找到试题内容"));
                reValue.HasError = true;
            }
            if (!publicProperty.ContainsKey("questionTypeUid") && !hasType)
            {
                errorMessage.Add(("未找到题型"));
                reValue.HasError = true;
            }
            if (!hasAnswer)
            {
                errorMessage.Add(("未找到试题答案"));
                reValue.HasError = true;
            }


            reValue.PutValue("error", errorMessage);
            return reValue;
        }
        #endregion

        #region import excel
        public void ImportFromExcel(Stream stream, out int scCount, out string errMsg)
        {
            scCount = 0;
            errMsg = string.Empty;
            newv.excel.reader.Excel.Workbook book = new newv.excel.reader.Excel.Workbook(stream);
            if (book.Sheets.Count > 0)
            {
                newv.excel.reader.Excel.Worksheet sheet = book.Sheets[0];
                var content = SaveDataFromWorksheet(sheet);
                if (string.IsNullOrEmpty(content))
                {
                    errMsg = "载入数据异常";
                    return;
                }
                var msg = CheckFormat(ref content); //检测导入数据
                if (!string.IsNullOrEmpty(msg))
                {
                    errMsg = msg;
                    return;
                }

                int successCount = 0;
                ImportQuestion(content, ref msg, ref successCount);
                if (!string.IsNullOrEmpty(msg))
                {
                    errMsg = msg;
                    scCount = successCount;
                    return;
                }

                scCount = successCount;
            }
            else
            {
                errMsg = "未找到数据";
            }
        }

        /// <summary>
        /// 检测导入内容
        /// </summary>
        /// <param name="content">导入内容</param>
        /// <returns>错误信息</returns>
        public string CheckFormat(ref string content)
        {
            string errors = "";
            int i = 0;
            try
            {

                //===============1. 处理试题内容 ==============
                //去掉每行两边的空格和Tab
                content = TrimContentList(content);
                string sQuestionList = content;
                //对内容里的回车进行编码
                sQuestionList = EnterCharEconde(sQuestionList);
                Dictionary<string, string> questionField = new Dictionary<string, string>();
                Dictionary<string, string> tempField = GetTableFieldList();
                questionField.Clear();
                foreach (string key in tempField.Keys)
                {
                    questionField.Add(key, tempField[key]);
                }
                StringBuilder sbNewContentList = new StringBuilder();
                sQuestionList = sQuestionList + "\r\n\r\n";

                Hashtable publicProperty = new Hashtable();
                string parentQuestionUid = string.Empty;

                //==============2. 查检数据格式 ==================
                int nPos = sQuestionList.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                while (nPos > -1)
                {
                    var sOneContent = sQuestionList.Substring(0, nPos).Trim();

                    if (sOneContent != "" && sOneContent.StartsWith("[") &&
                        (sOneContent.IndexOf("]:", StringComparison.Ordinal) >= 0 || sOneContent.IndexOf("]：", StringComparison.Ordinal) >= 0))
                    {
                        //==============2.1 取得公共属性 ==================
                        sOneContent = sOneContent.Replace("：", ":");
                        string sPropertyName = sOneContent.Substring(1, sOneContent.IndexOf("]:", StringComparison.Ordinal) - 1);
                        string sPropertyValue = sOneContent.Substring(sOneContent.IndexOf("]:", StringComparison.Ordinal) + 2);

                        //==============2.2 格式检查====================
                        if (sOneContent.Contains("\n"))
                        {
                            errors = errors +
                                     string.Format(("[错误：]第{0}题前公共属性'{1}'格式不对,可能的原因是此公共属性后没有空行"), (i + 1), sPropertyName) +
                                     Environment.NewLine;

                        }
                        else if (sPropertyValue != "" && sPropertyName == questionField["outdatedDate"])
                        {
                            //过期时间
                            if (DateTimeUtil.IsDateTime(sPropertyValue) == false)
                            {
                                errors = errors +
                                         string.Format(("[错误：]第{0}题前公共属性'过期时间'格式不对，请改为形如：2004-01-02"),
                                             (i + 1)) + Environment.NewLine;

                            }
                        }
                        else if (sPropertyValue != "" && sPropertyName == questionField["questionStatusCode"])
                        {
                            //状态
                            switch (sPropertyValue)
                            {
                                case "正常":
                                    sPropertyValue = "normal";
                                    break;
                                case "已过期":
                                    sPropertyValue = "outdated";
                                    break;
                                case "禁用":
                                    sPropertyValue = "disabled";
                                    break;
                                default:
                                    sPropertyValue = "";
                                    break;
                            }

                            if (sPropertyValue == "")
                            {
                                errors = errors + string.Format(("[错误：]第{0}未知的状态"), (i + 1)) +
                                         Environment.NewLine;

                            }
                        }
                        else if (sPropertyName == questionField["id"])
                        {
                            //题型
                            if (sPropertyValue != "")
                            {
                                var qtRow = _iExamQuestionTypeRep.FirstOrDefault(a => a.questionTypeName.Equals(sPropertyValue));

                                if (qtRow == null)
                                {
                                    errors = errors + string.Format(("[错误：]第{0}题前公共属性值无效:"), (i + 1)) +
                                             sPropertyValue + Environment.NewLine;

                                }
                                else if (qtRow.questionBaseTypeCode == QuestionTypeConst.Compose)
                                {
                                    errors = errors + string.Format(("[错误：]第{0}题前公共属性不可以是:"), (i + 1)) +
                                             sPropertyValue + Environment.NewLine;

                                }
                                else
                                {
                                    if (publicProperty.ContainsKey("id"))
                                    {
                                        publicProperty["id"] = qtRow.Id.ToString();
                                    }
                                    else
                                    {
                                        publicProperty.Add("id", qtRow.Id.ToString());
                                    }
                                }

                                if (publicProperty.ContainsKey("questionTypeName"))
                                {
                                    publicProperty["questionTypeName"] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add("questionTypeName", sPropertyValue);
                                }
                            }
                        }
                        else if (sPropertyName == questionField["hardGrade"])
                        {
                            //难度


                        }
                        else if (sPropertyName == ("开始子试题"))
                        {
                            //开始子试题
                            if (parentQuestionUid.Length > 0)
                            {
                                errors = errors +
                                         string.Format(("[错误：]第{0}题是子试题,下面还还有子试题是不正确的，请检查是否上一个组合题的子试题没有结束标志！"),
                                             (i + 1 - 1)) + Environment.NewLine;

                            }
                        }
                        else if (sPropertyName == ("结束子试题"))
                        {
                            //结束子试题
                        }
                        else if (sPropertyName == ("父试题分类"))
                        {
                            //父试题分类
                        }
                        else if (sPropertyName == ("所属知识点"))
                        {
                            //所属知识点
                        }
                        else
                        {
                            if (!questionField.ContainsValue(sPropertyName))
                            {
                                errors = errors +
                                         string.Format(("[错误：]第{0}题前公共属名称'{1}'不在支持范围内"), (i + 1),
                                             sPropertyName) + Environment.NewLine;

                            }
                        }

                    }
                    else if (sOneContent != "" && !sOneContent.StartsWith("//"))
                    {

                        //验证试题格式包括检查单选、多选、判断题答案格式
                        ReturnValue reValue = CheckQuestionTextFormat(sOneContent, questionField, publicProperty, i);

                        //处理试题前的序号
                        if (sOneContent.IndexOf(".", StringComparison.Ordinal) > -1 &&
                            StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf(".", StringComparison.Ordinal))))
                        {
                            sOneContent = sOneContent.Substring(sOneContent.IndexOf(".", StringComparison.Ordinal) + 1);
                        }
                        else if (sOneContent.IndexOf("。", StringComparison.Ordinal) > -1 &&
                                 StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf("。", StringComparison.Ordinal))))
                        {
                            sOneContent = sOneContent.Substring(sOneContent.IndexOf("。", StringComparison.Ordinal) + 1);
                        }
                        else if (sOneContent.IndexOf("、", StringComparison.Ordinal) > -1 &&
                                 StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf("、", StringComparison.Ordinal))))
                        {
                            sOneContent = sOneContent.Substring(sOneContent.IndexOf("、", StringComparison.Ordinal) + 1);
                        }
                        else if (sOneContent.IndexOf(" ", StringComparison.Ordinal) > -1 &&
                                 StringUtil.IsNumber(sOneContent.Substring(0, sOneContent.IndexOf(" ", StringComparison.Ordinal))))
                        {
                            sOneContent = sOneContent.Substring(sOneContent.IndexOf(" ", StringComparison.Ordinal) + 1);
                        }
                        sOneContent = (i + 1) + "." + sOneContent;

                        //判断知识点的权限

                        if (reValue.HasError)
                        {
                            var errList = (ArrayList)reValue.GetValue("error");
                            //显示错误列表
                            foreach (object err in errList)
                            {
                                errors = errors + string.Format(("[错误：]第{0}题"), (i + 1)) + ":" +
                                         (err) + Environment.NewLine;
                            }
                        }

                        i = i + 1;
                    }
                    if (sOneContent != "" && sOneContent != "\r\n") sbNewContentList.Append(sOneContent + "\r\n\r\n");
                    sQuestionList = sQuestionList.Substring(nPos + 4);
                    nPos = sQuestionList.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                }
                if (!string.IsNullOrEmpty(errors))
                    return errors;
                content = EnterCharRevert(sbNewContentList.ToString());
                if (content != "")
                    content = content.Substring(0, content.Length - 4);



            }
            catch (Exception ex)
            {
                var guidNum = Guid.NewGuid().GetHashCode();
                errors = "[错误：]第" + i + "题，发生未知错误，请联系管理员，错误编码：[" + guidNum + "]";
                _iLogger.Error("[" + guidNum + "]" + ex.ToString()); 
                return errors;
            }
            return errors;
        }

        /// <summary>
        /// 把数据从Worksheet提取试题生成文本并把数据保存在Hidden变量里
        /// </summary>
        public string SaveDataFromWorksheet(newv.excel.reader.Excel.Worksheet sheet)
        {
            #region 读取EXCEL文件

            DataTable dtCacheData = new DataTable();
            newv.excel.reader.Excel.Row excelRow = sheet.Rows[2]; //表格头，第二行（标题行）
            HtmlTableRow tableRow = new HtmlTableRow();
            tableRow.Attributes["class"] = "HeadRow";
            int comCount = excelRow.Cells.LastCol;

            string formatValue;
            for (int i = 0; i <= comCount; ++i)
            {
                formatValue = excelRow.Cells[(byte) i].FormattedValue();
                DataColumn dcData = new DataColumn(formatValue);
                try
                {
                    dtCacheData.Columns.Add(dcData);
                }
                catch
                {
                    Random rd = new Random();
                    dtCacheData.Columns.Add(dcData + rd.Next(0, 100).ToString());
                }

            }
            for (int i = 3; i <= sheet.Rows.LastRow; ++i)
            {

                excelRow = sheet.Rows[(ushort) i];
                if (excelRow != null)
                {
                    DataRow drdata = dtCacheData.NewRow();

                    for (int m = 0; m <= comCount; m++)
                    {
                        var excelCell = excelRow.Cells[(byte) m];
                        formatValue = excelCell != null ? excelCell.FormattedValue() : "";
                        drdata[m] = formatValue;
                    }
                    dtCacheData.Rows.Add(drdata);
                }
            }

            #endregion

            #region 生成文本

            StringBuilder sb = new StringBuilder();
            int flag = 0;
            bool startChildQuestion = false;

            for (int i = 0; i < dtCacheData.Rows.Count; i++)
            {
                //处理子试题
                string childQuestionFieldName = "是否子试题";
                if (dtCacheData.Columns.Contains(childQuestionFieldName) &&
                    dtCacheData.Rows[i][childQuestionFieldName] != null &&
                    dtCacheData.Rows[i][childQuestionFieldName].ToString() == "Y")
                {
                    if (startChildQuestion == false)
                    {
                        sb.Append("[" + "开始子试题" + "]:\r\n\r\n");
                        startChildQuestion = true;
                    }
                }
                else
                {
                    if (startChildQuestion)
                    {
                        sb.Append("[" + ("结束子试题") + "]:\r\n\r\n");
                        startChildQuestion = false;
                    }
                }

                #region 处理试题

                flag++;
                //添加分类
                if (dtCacheData.Columns.Contains("试题分类"))
                {
                    if (dtCacheData.Rows[i]["试题分类"].ToString().Trim().Length > 0)
                    {
                        sb.Append("[" + "试题分类" + "]:" + dtCacheData.Rows[i]["试题分类"].ToString().Trim('\n') + "\r\n\r\n");
                    }
                }

                if (dtCacheData.Columns.Contains("试题分类"))
                {
                    if (dtCacheData.Rows[i]["试题分类"].ToString().Trim().Length > 0)
                    {
                        sb.Append("[试题分类]:" + dtCacheData.Rows[i]["试题分类"].ToString().Trim('\n') + "\r\n\r\n");
                    }
                }

                if (dtCacheData.Columns.Contains("试题内容"))
                {
                    if (dtCacheData.Rows[i]["试题内容"].ToString().Trim().Length > 0)
                    {
                        sb.Append(flag.ToString() + "." + dtCacheData.Rows[i]["试题内容"].ToString().Trim('\n') + "\r\n");
                    }
                }


                #region 可选答案

                for (int n = 0; n < 26; n++)
                {
                    string selectChar = QuestionUtil.AnswerNumbersToChars(n.ToString());
                    if (!dtCacheData.Columns.Contains("可选答案" + selectChar))
                    {
                        continue;
                    }
                    var selectAnswerObj = dtCacheData.Rows[i]["可选答案" + selectChar];
                    if (selectAnswerObj != null && selectAnswerObj.ToString().Trim().Length > 0)
                    {
                        string selectAnswer = selectAnswerObj.ToString().Trim('\n');
                        if (selectAnswer.Contains("\r\n"))
                        {
                            selectAnswer = "{\r\n" + selectAnswer + "\r\n}";
                        }
                        sb.Append(selectChar + "." + selectAnswer + "\r\n");
                    }
                }

                #endregion

                #region//添加标准答案

                if (dtCacheData.Columns.Contains("题型"))
                {

                    if (dtCacheData.Rows[i]["题型"].ToString().Contains("判断题"))
                    {
                        if (dtCacheData.Rows[i]["答案"].ToString().ToUpper().Trim() == "Y")
                        {
                            sb.Append("答案" + ":" + "正确" + "\r\n");
                        }
                        else if (dtCacheData.Rows[i]["答案"].ToString().ToUpper().Trim() == "N")
                        {
                            sb.Append("答案" + ":" + ("错误") + "\r\n");
                        }
                        else
                        {
                            sb.Append("答案" + ":" + dtCacheData.Rows[i]["答案"].ToString().Trim('\n') + "\r\n");
                        }
                    }
                    else
                    {
                        sb.Append("答案" + ":" + dtCacheData.Rows[i]["答案"].ToString().Trim('\n') + "\r\n");

                    }
                }

                #endregion

                for (int c = 0; c < dtCacheData.Columns.Count; c++)
                {
                    string colname = dtCacheData.Columns[c].ColumnName;
                    if (!(colname.Contains("试题分类") || colname.Contains("试题内容") || colname.Contains("答案")))
                    {

                        if (dtCacheData.Rows[i][colname].ToString().Trim().Length > 0)
                        {

                            sb.Append(colname + ":" + dtCacheData.Rows[i][colname].ToString().Trim('\n') + "\r\n");

                        }

                    }
                }
                sb.Append("\r\n");

                #endregion

            }

            if (startChildQuestion)
            {
                sb.Append("[结束子试题]:\r\n\r\n");
            }

            return ReplaceImportData(sb.ToString());

            #endregion
        }

        /// <summary>
        /// 导入试题
        /// </summary>
        /// <param name="content"></param>
        /// <param name="errMsg"></param>
        /// <param name="successCount"></param>
        /// <returns></returns>
        public bool ImportQuestion(string content, ref string errMsg, ref int successCount)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            bool hasUpdate = false;
            //=======1. 初始化数据 ==========
            int scCount = 0;
            int errCount = 0;

            //公共属性
            Hashtable publicProperty = new Hashtable();
            publicProperty.Clear();
            publicProperty.Add("parentUid", "");
            string preQuestionUid = string.Empty;
            try
            {
                string sContentList;
                ReplaceHtml(out sContentList, content);

                //对内容里的回车进行编码
                sContentList = EnterCharEconde(sContentList);
                sContentList = sContentList + "\r\n\r\n";
                int i = 0;
                int nPos = sContentList.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                while (nPos > -1)
                {
                    try
                    {
                        //=============2. 取公共属性 ================
                        var sOneContent = sContentList.Substring(0, nPos);
                        if (sOneContent != "" && sOneContent.StartsWith("[") && (sOneContent.IndexOf("]:", StringComparison.Ordinal) >= 0 || sOneContent.IndexOf("]：", StringComparison.Ordinal) >= 0))
                        {
                            #region 公共属性
                            sOneContent = sOneContent.Replace("：", ":");
                            string sPropertyName = sOneContent.Substring(1, sOneContent.IndexOf("]:", StringComparison.Ordinal) - 1);
                            string sPropertyValue = sOneContent.Substring(sOneContent.IndexOf("]:", StringComparison.Ordinal) + 2);

                            if (sPropertyName == "试题分类")        ////试题目录
                            {
                                if (publicProperty.ContainsKey("folderName"))
                                {
                                    publicProperty["folderName"] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add("folderName", sPropertyValue);
                                }
                            }
                            else if (sPropertyValue != "" && sPropertyName == "过期日期") ////过期时间
                            {
                                if (publicProperty.ContainsKey("outdatedDate"))
                                {
                                    publicProperty["outdatedDate"] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add("outdatedDate", sPropertyValue);
                                }
                            }
                            else if (sPropertyValue != "" && sPropertyName == "试题状态")   //状态
                            {
                                if (publicProperty.ContainsKey("questionStatusCode"))
                                {
                                    publicProperty["questionStatusCode"] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add("questionStatusCode", sPropertyValue);
                                }
                            }
                            else if (sPropertyName == "题型")  //题型
                            {
                                if (publicProperty.ContainsKey("questionTypeUid"))
                                {
                                    publicProperty["questionTypeUid"] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add("questionTypeUid", sPropertyValue);
                                }
                            }
                            else if (sPropertyName == ("开始子试题"))  //开始子试题
                            {
                                if (publicProperty.ContainsKey("parentQuestionUid"))
                                {
                                    publicProperty["parentQuestionUid"] = preQuestionUid;
                                }
                                else
                                {
                                    publicProperty.Add("parentQuestionUid", preQuestionUid);
                                }
                            }
                            else if (sPropertyName == ("结束子试题"))  //结束子试题
                            {
                                if (publicProperty.ContainsKey("parentQuestionUid"))
                                {
                                    publicProperty["parentQuestionUid"] = "";
                                }
                                else
                                {
                                    publicProperty.Add("parentQuestionUid", "");
                                }
                            }
                            //else if (sPropertyName == ("父试题分类"))  //父试题分类
                            //{
                            //    string newParentFolderUid;
                            //    if (string.IsNullOrEmpty(sPropertyValue))
                            //        newParentFolderUid = string.Empty;
                            //    else
                            //    {
                            //        newParentFolderUid =
                            //            GetFolderUid("question_bank", sPropertyValue, cookie.Id, true)
                            //                .ToString();
                            //    }
                            //    publicProperty["parentUid"] = newParentFolderUid;
                            //}
                            else
                            {
                                if (publicProperty.ContainsKey(sPropertyName))
                                {
                                    publicProperty[sPropertyName] = sPropertyValue;
                                }
                                else
                                {
                                    publicProperty.Add(sPropertyName, sPropertyValue);
                                }
                            }
                            #endregion
                        }
                        else if (sOneContent != "" && !sOneContent.StartsWith("//"))
                        {
                            Dictionary<string, string> questionField = new Dictionary<string, string>();
                            Dictionary<string, string> tempField = GetTableFieldList();
                            questionField.Clear();
                            foreach (string key in tempField.Keys)
                            {
                                questionField.Add(key, tempField[key]);
                            }
                            var retValue = ConvertTextToQuestion(sOneContent, questionField, publicProperty, cookie.Id);
                            if (retValue.HasError)
                            {
                                errMsg = errMsg + string.Format(("[错误：]第{0}题导入失败:"), (i + 1)) + retValue.Message + Environment.NewLine;
                                errCount += 1;
                            }
                            else
                            {
                                ExamQuestion quRow = (ExamQuestion)retValue.GetValue("question");
                                preQuestionUid = quRow.Id.ToString();

                                //如果是从Word导入,则处理图片
                                if (retValue.HasError == false)
                                {
                                    scCount += 1;

                                    string sContentText = quRow.questionText;
                                    string selectAnswer = quRow.selectAnswer;
                                    string questionAnslysis = quRow.questionAnalysis ?? "";
                                    string standardAnswer = quRow.standardAnswer;
                                    string sSourceFilePath = FilePathUtil.GetOppositeUserTempPath(cookie.Id.ToString());
                                    bool hasCopyContentFile = false;
                                    var retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref sContentText, sSourceFilePath);
                                    if (retValueCopyFile.HasError)
                                    {
                                        if (retValueCopyFile.Message != "")
                                            errMsg = errMsg + string.Format(("[错误：]第{0}题导入试题内容成功，但处理文件失败:"), (i + 1)) + retValueCopyFile.Message + Environment.NewLine;
                                    }
                                    else
                                    {
                                        hasCopyContentFile = true;
                                    }
                                    bool hasCopySelectAnswerFile = false;
                                    retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref selectAnswer, sSourceFilePath);
                                    if (retValueCopyFile.HasError == false) hasCopySelectAnswerFile = true;
                                    bool hasCopyStandardAnswerFile = false;
                                    retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref standardAnswer, sSourceFilePath);
                                    if (retValueCopyFile.HasError == false) hasCopyStandardAnswerFile = true;
                                    bool hasCopyQuestionAnslysisFile = false;
                                    retValueCopyFile = ExamImportAndExportUtil.CopyContentFiles(quRow.Id, ref questionAnslysis, sSourceFilePath);
                                    if (retValueCopyFile.HasError == false) hasCopyQuestionAnslysisFile = true;

                                    if (hasCopyContentFile || hasCopySelectAnswerFile || hasCopyStandardAnswerFile || hasCopyQuestionAnslysisFile)
                                    {
                                        quRow.questionText = sContentText;
                                        quRow.selectAnswer = selectAnswer;
                                        quRow.standardAnswer = standardAnswer;
                                        quRow.questionAnalysis = questionAnslysis;
                                        _iExamQuestionRep.UpdateAsync(quRow);
                                    }
                                }
                                else
                                {
                                    errMsg = errMsg + string.Format(("[错误：]第{0}题导入失败:"), (i + 1)) + retValue.Message + Environment.NewLine;
                                    errCount += 1;
                                }
                            }
                            i = i + 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        errMsg = errMsg + string.Format(("[错误：]第{0}题导入失败:"), (i + 1).ToString()) + ex.Message + Environment.NewLine;
                        errCount += 1;
                    }
                    sContentList = sContentList.Substring(nPos + 4);
                    nPos = sContentList.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                }

                successCount = scCount;
                if (errCount > 0)
                {
                    errMsg = errMsg + string.Format(("[提示：]导入试题完毕，{0}个试题导入失败，{1}个试题成功！"), errCount, scCount);
                }
                hasUpdate = true;
            }
            catch (Exception ex)
            {
                errMsg = errMsg + ex.Message;
            }
            return hasUpdate;
        }

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
    }

    
}