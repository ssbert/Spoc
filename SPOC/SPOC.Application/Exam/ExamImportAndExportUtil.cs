using Abp.AutoMapper;
using Abp.Domain.Repositories;
using newv.common;
using SPOC.Category;
using SPOC.Common.Extensions;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.ExamPaper.Dto;
using SPOC.QuestionBank;
using SPOC.QuestionBank.Const;
using SPOC.QuestionBank.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using SPOC.Core;
using SPOC.Lib;
using ConvertUtil = SPOC.Common.Helper.ConvertUtil;
using ReturnValue = SPOC.Common.ReturnValue;
using StringUtil = SPOC.Common.Helper.StringUtil;

namespace SPOC.Exam
{
    public class ExamImportAndExportUtil
    {
        /// <summary>
        /// 内容进行回车换的编码
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="newLineText"></param>
        /// <param name="forceReplace">强制替换</param>
        /// <returns></returns>
        public static string ReplaceExportTextParagraph(string sText, string newLineText,bool forceReplace=false)
        {
            if (sText != null)
            {
                sText = sText.Replace("\r\n", newLineText);
                bool needReplace = false;
                if (sText.IndexOf(newLineText, StringComparison.Ordinal) > -1)
                {
                    needReplace = true;
                }
                else if (newLineText == "<br>")
                {
                    if (sText.IndexOf("<br/>", StringComparison.Ordinal) > -1)
                    {
                        needReplace = true;
                    }
                }

                if (needReplace || forceReplace)
                {
                    string sBracketBeginFlag = "{" + newLineText;
                    string sBracketEndFlag = newLineText + "}";
                    sText = sText.Replace("{", "\\{");
                    sText = sText.Replace("}", "\\}");
                    sText = sText.Replace("｛", "\\｛");
                    sText = sText.Replace("｝", "\\｝");
                    sText = sBracketBeginFlag + sText + sBracketEndFlag;
                }
               
                return sText;
            }
            else
            {
                return "";
            }
        }

        #region 试题导入与导出

        /// <summary>
        /// 把试题转化为格式化的文本
        /// </summary>
        /// <param name="dvData">要导出的试题数据</param>
        /// <param name="fomartType"></param>
        /// <returns></returns>
        public static ReturnValue ConvertQuestionToText(IEnumerable<ExamQuestionDto> dvData, string fomartType,
            IDictionary<Guid, ExamQuestionType> examQuestionTypeDic, IEnumerable<NvFolder> nvFolders,
            IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<QuestionStandardCode, Guid> iQuestionStandardCodeRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep, IRepository<Label, Guid> iLabelRep)
        {
            string newline = "\r\n";
            bool isWithFilePath = true;
            if (fomartType == EnumExportTextType.Text)
            {
                newline = "\r\n";
                isWithFilePath = false;
            }
            else
            {
                newline = "<br>";
                isWithFilePath = true;
            }

            //Hashtable keyValueCatch = new Hashtable();
            StringBuilder questionText = new StringBuilder();
            //string oldFolderFullName = string.Empty;
            //string folderFullName = string.Empty;

            //var questionFolder = nvFolders.FirstOrDefault(a => a.folderCode == "question_bank" && a.folderCode != "cloud");

            //string oldParentFolder = questionFolder.folderName;
            bool isWriteParentFolder = false;
            
            //dvData.Sort = NvFolderField.FullFolderName + "," + ExamQuestionField.ListOrder;

            ReturnValue retValue = new ReturnValue();
            try
            {
                //Dictionary<string, string> questionField = new Dictionary<string, string>();
                //Dictionary<string, string> tempField = ExamQuestionTable.GetTableFieldList();
                //questionField.Clear();
                //foreach (string key in tempField.Keys)
                //{
                //    questionField.Add(key, tempField[key]);
                //}

                //=========处理 要导出的列 ==============
                //if (exportFields == null || exportFields.Count < 1)
                //{
                //    exportFields = new Dictionary<string, string>();
                //    foreach (string field in questionField.Keys)
                //    {
                //        if (!exportFields.ContainsKey(field))
                //        {
                //            exportFields.Add(field, questionField[field]);
                //        }
                //    }
                //}

                //=========1. 声明变量 ================================
                //NvExtendSettingRowCollection extendSettingRows = NvExtendSettingTable.GetNvExtendSettingRowCollectionByTableName(ExamQuestionTable.TableName);
                string questionUid = string.Empty;  //试题Uid
                //string questionCode = string.Empty;  //试题编号
                string questionType = string.Empty; //试题类型
                //string questionTypeRelative = string.Empty; //操作题类型
                string isHtml = string.Empty; //是否超文件答题
                //string outDatedTime = string.Empty; // 过期时间
                int answerTime; //答题时间
                string hardGrade = string.Empty; //试题难度
                decimal score;
                //string status = string.Empty;  //状态
                string content = string.Empty; //试题内容
                string selectAnswer = string.Empty; //备选答案
                string selectAnswerScore = string.Empty; //答案选项分数,只用于倒扣分题
                string standardAnswer = string.Empty; //标准答案
                string description = string.Empty;     //答案分析
                //string createTime = string.Empty;  //创建时间
                //string creator = string.Empty;     //创建人
                //string modifyTime = string.Empty;  //最后修改时间
                //string modifier = string.Empty;    //修改人
                string baseQuestionType = string.Empty; //基本试题类型
                string folderName = string.Empty;//试题分类
                decimal listOrder;          //顺序号

                //=========2. 分析数据,开始转化======================

                
                int i = 0;
                StringBuilder sb = new StringBuilder();
                foreach (var dataRow in dvData)
                {
                    sb = new StringBuilder();
                    questionUid = dataRow.Id.ToString();//ConvertUtil.ToString(dataRow[ExamQuestionField.QuestionUid], ""));
                    //questionCode = dataRow.questionCode;//ConvertUtil.ToString(dataRow[ExamQuestionField.QuestionCode], "");
                    questionType = examQuestionTypeDic[dataRow.questionTypeUid].questionTypeName;//ConvertUtil.ToString(dataRow[ExamQuestionTypeField.QuestionTypeName], "");
                    //questionTypeRelative = GetListOptionText(keyValueCatch, ExamQuestionField.OperateTypeCode, ConvertUtil.ToString(dataRow[ExamQuestionField.OperateTypeCode], ""));
                    //folderFullName = GetFullFolderName(dataRow.folderUid);//ConvertUtil.ToString(dataRow[NvFolderField.FullFolderName], "");
                    isHtml = dataRow.isAnswerByHtml;//ConvertUtil.ToString(dataRow[ExamQuestionField.IsAnswerByHtml], "");
                    //outDatedTime = dataRow.outdatedDate.ToString();//ConvertUtil.ToString(dataRow[ExamQuestionField.OutdatedDate], "");
                    answerTime = dataRow.examTime;//ConvertUtil.ToInt(dataRow[ExamQuestionField.ExamTime], -1);
                    hardGrade = dataRow.hardGrade;//ConvertUtil.ToString(dataRow[ExamQuestionField.HardGrade], "");
                    //status = dataRow.questionStatusCode;//GetListOptionText(keyValueCatch, ExamQuestionField.QuestionStatusCode, ConvertUtil.ToString(dataRow[ExamQuestionField.QuestionStatusCode], ""));
                    content = dataRow.questionText;//ConvertUtil.ToString(dataRow[ExamQuestionField.QuestionText], "");
                    selectAnswer = dataRow.selectAnswer;//ConvertUtil.ToString(dataRow[ExamQuestionField.SelectAnswer], "");
                    selectAnswerScore = dataRow.selectAnswerScore;//ConvertUtil.ToString(dataRow[ExamQuestionField.SelectAnswerScore], "");
                    standardAnswer = StringUtil.HtmlEncode(dataRow.standardAnswer); //StringUtil.ReplaceSpaces2Html(dataRow.standardAnswer,true);//ConvertUtil.ToString(dataRow[ExamQuestionField.StandardAnswer], "");
                    description = dataRow.questionAnalysis;//ConvertUtil.ToString(dataRow[ExamQuestionField.QuestionAnalysis], "");
                    //createTime = ConvertUtil.ToString(DateTimeUtil.ConvertToUnixTime(dataRow.createTime), "");//ConvertUtil.ToString(dataRow[ExamQuestionField.CreateTime], ""));
                    //creator = ConvertUtil.ToString(dataRow[ExamQuestionField.Creator], "");
                    //modifyTime = ConvertUtil.ToString(dataRow[ExamQuestionField.ModifyTime], "");
                    //modifier = ConvertUtil.ToString(dataRow[ExamQuestionField.Modifier], "");
                    baseQuestionType = dataRow.questionBaseTypeCode;//ConvertUtil.ToString(dataRow[ExamQuestionField.QuestionBaseTypeCode], "");
                    listOrder = dataRow.listOrder;//ConvertUtil.ToDecimal(dataRow[ExamQuestionField.ListOrder]);
                    score = dataRow.score;//ConvertUtil.ToDecimal(dataRow[ExamQuestionField.Score]);

                    //父试题分类
                    //if (!isWriteParentFolder && !string.IsNullOrEmpty(oldParentFolder))
                    //{
                    //    sb.Append("//[" + "父试题分类" + "]:" + oldParentFolder + newline + newline);
                    //    isWriteParentFolder = true;
                    //}
                    //试题分类,要减去父试题分类部分
                    //if (oldFolderFullName != folderFullName)
                    //{
                    //    oldFolderFullName = folderFullName;

                    //    if (folderFullName.StartsWith(oldParentFolder))
                    //    {
                    //        folderFullName = folderFullName.Substring(oldParentFolder.Length).Trim('/');
                    //    }
                    //    if (exportFields.ContainsKey(NvFolderField.FolderUid))
                    //    {
                    //        sb.Append("[" + questionField[ExamQuestionField.FolderUid] + "]:" + folderFullName + newline + newline);
                    //    }
                    //}
                    //试题分类（移植过来代码后添加的）
                    folderName = nvFolders.First(a => a.Id == dataRow.folderUid).folderName;
                    sb.Append("[试题分类]:" + folderName + newline + newline);
                    //试题内容
                    //if (fomartType == EnumExportTextType.Word)
                    //{
                    //    content = StringUtil.ReplaceSpaces2Html(content, true);
                    //}
                    sb.Append((i + 1) + "." + ReplaceExportTextParagraph(content, newline,true) + newline);		//支持换行
                    if (!string.IsNullOrWhiteSpace(dataRow.title))
                    {
                        sb.Append("标题:" + dataRow.title + newline);
                    }
                    //备选答案
                    if (baseQuestionType == EnumQuestionBaseTypeCode.Single || baseQuestionType == EnumQuestionBaseTypeCode.Multi)
                    {
                        string[] arrSelectAnswer = selectAnswer.Split('|');
                        int j = 0;
                        foreach (string sSelectAnswer in arrSelectAnswer)
                        {
                            string tempSelectAnswer = sSelectAnswer;
                            if (fomartType == EnumExportTextType.Word)
                            {
                                tempSelectAnswer = StringUtil.ReplaceSpaces2Html(sSelectAnswer, true);
                            }
                            else
                            {
                                tempSelectAnswer = sSelectAnswer;
                            }
                            sb.Append(QuestionUtil.AnswerNumbersToChars(j.ToString()) + "." + tempSelectAnswer + newline);	//支持换行
                            j++;
                        }
                    }
                    else if (baseQuestionType == EnumQuestionBaseTypeCode.EvaluationSingle || baseQuestionType == EnumQuestionBaseTypeCode.EvaluationMulti)
                    {
                        string[] arrSelectAnswer = selectAnswer.Split('|');
                        string[] arrSelectAnswerScore = selectAnswerScore.Split('|');
                        int j = 0;
                        foreach (string sSelectAnswer in arrSelectAnswer)
                        {
                            string tempSelectAnswer = sSelectAnswer;
                            //if (fomartType == EnumExportTextType.Word)
                            //{
                            //    tempSelectAnswer = StringUtil.ReplaceSpaces2Html(sSelectAnswer, true);
                            //}
                            sb.Append(QuestionUtil.AnswerNumbersToChars(j.ToString()) + "." + ReplaceExportTextParagraph(tempSelectAnswer, newline));	//支持换行
                            if (j < arrSelectAnswerScore.Length)
                            {
                                sb.Append(" (" + arrSelectAnswerScore[j] + ")" + newline);
                            }
                            else
                            {
                                sb.Append(" (0)" + newline);
                            }
                            j++;
                        }
                    }
                    //屏蔽导出得分点
                    //else if (baseQuestionType == EnumQuestionBaseTypeCode.Answer || baseQuestionType == EnumQuestionBaseTypeCode.Operate || baseQuestionType == EnumQuestionBaseTypeCode.Program || baseQuestionType == EnumQuestionBaseTypeCode.ProgramFill)
                    //{
                    //    if (!string.IsNullOrEmpty(selectAnswer.Trim()))
                    //    {
                    //        string[] arrSelectAnswer = selectAnswer.Split(new[] { "$#$" }, StringSplitOptions.None);
                    //        string[] arrSelectAnswerScore = selectAnswerScore.Split('|');
                    //        int j = 0;
                    //        foreach (string sSelectAnswer in arrSelectAnswer)
                    //        {
                    //            sb.Append(QuestionUtil.AnswerNumbersToChars(j.ToString()) + "." + ExamImportAndExportUtil.ReplaceExportTextParagraph(sSelectAnswer, newline));	//支持换行
                    //            if (j < arrSelectAnswerScore.Length)
                    //            {
                    //                sb.Append(" (" + arrSelectAnswerScore[j] + "%)" + newline);
                    //            }
                    //            else
                    //            {
                    //                sb.Append(" (0)" + newline);
                    //            }
                    //            j++;
                    //        }
                    //    }
                    //}

                    //试题Uid
                    //if (exportFields.ContainsKey(ExamQuestionField.QuestionUid))
                    //{
                    //    sb.Append(questionField[ExamQuestionField.QuestionUid] + ":" + questionUid + newline);
                    //}
                    //试题Uid
                    //if (exportFields.ContainsKey(ExamQuestionField.QuestionCode))
                    //{
                    //    sb.Append(questionField[ExamQuestionField.QuestionCode] + ":" + questionCode + newline);
                    //}
                    //标准答案
                    sb.Append("答案:");

                    if (baseQuestionType == EnumQuestionBaseTypeCode.Judge)
                    {
                        if (standardAnswer == "Y")
                            sb.Append("正确" + newline);
                        else if (standardAnswer == "N")
                            sb.Append("错误" + newline);
                        else
                            sb.Append(newline);
                    }
                    else if (baseQuestionType == EnumQuestionBaseTypeCode.JudgeCorrect)
                    {
                        if (standardAnswer == "Y")
                            sb.Append("正确" + newline);
                        else if (standardAnswer == "N")
                            sb.Append("错误" + newline);
                        else
                            sb.Append(ExamImportAndExportUtil.ReplaceExportTextParagraph(standardAnswer, newline) + newline);
                    }
                    else if (baseQuestionType == EnumQuestionBaseTypeCode.Single || baseQuestionType == EnumQuestionBaseTypeCode.Multi)
                    {
                        sb.Append(standardAnswer + newline);
                    }
                    else
                    {
                        sb.Append(ExamImportAndExportUtil.ReplaceExportTextParagraph(standardAnswer, newline) + newline);
                    }
                    //试题分数
                    sb.Append("分数:" + score.ToString("0.##") + newline);

                    //题型
                    sb.Append("题型:" + questionType + newline);
                    //答题时间
                    TimeSpan timeSpan = new TimeSpan(0, 0, answerTime);
                    DateTime time = DateTime.Parse("00:00:00").Add(timeSpan);
                    sb.Append("答题时间:" + DateTimeUtil.ToTimeStr(time) + newline);
                    //操作题类型
                    //if (baseQuestionType == EnumQuestionBaseTypeCode.Operate && questionTypeRelative != "" && exportFields.ContainsKey(ExamQuestionField.QuestionTypeUid))
                    //{
                    //    sb.Append(questionField[ExamQuestionField.OperateTypeCode] + ":" + questionTypeRelative + newline);
                    //}
                    //是否用超文本答题
                    //if (isHtml != "")// && exportFields.ContainsKey(ExamQuestionField.IsAnswerByHtml))
                    //{
                    //    sb.Append("是否用超文本答题:" + isHtml + newline);
                    //}
                    //过期日期
                    //if (!string.IsNullOrEmpty(outDatedTime) && exportFields.ContainsKey(ExamQuestionField.OutdatedDate))
                    //    sb.Append(questionField[ExamQuestionField.OutdatedDate] + ":" + outDatedTime + newline);
                    //试题难度
                    if (hardGrade != "")// && exportFields.ContainsKey(ExamQuestionField.HardGrade))
                    {
                        sb.Append("难度:" + hardGrade + newline);
                    }
                    //试题分析
                    if (description != "")// && exportFields.ContainsKey(ExamQuestionField.QuestionAnalysis))
                    {
                        //description= StringUtil.ReplaceSpaces2Html(description,true);
                        sb.Append("试题分析:" + ReplaceExportTextParagraph(description, newline,true) + newline);
                    }
                    //编程填空题
                    if (baseQuestionType == EnumQuestionBaseTypeCode.ProgramFill)
                    {
                        var preinstallCode = StringUtil.HtmlEncode(dataRow.PreinstallCode);
                        sb.Append("预设代码:" + ReplaceExportTextParagraph(preinstallCode, newline) + newline);
                    }
                    //编程题含编程填空题 需导出参考代码、标签信息
                    if (baseQuestionType.Contains(EnumQuestionBaseTypeCode.Program))
                    {
                        if (!string.IsNullOrWhiteSpace(dataRow.InputParam))
                        {
                            sb.Append("输入流参数:" + dataRow.InputParam + newline);
                        }
                        if (dataRow.MultiTest)
                        {
                            sb.Append("多次测试:是"  + newline);
                        }
                        if (!string.IsNullOrWhiteSpace(dataRow.param))
                        {
                            sb.Append("程序参数:" + dataRow.param + newline);
                        }
                        var standardCode = iQuestionStandardCodeRep.FirstOrDefault(
                            q => q.isDefault == 1 && q.questionId.Equals(dataRow.Id));
                        if (standardCode != null)
                        {
                            var code= StringUtil.CodeEncode(standardCode.code);
                            sb.Append("参考代码:" + ReplaceExportTextParagraph(code, newline) + newline);
                        }
                        var labelList = (from ql in iQuestionLabelRep.GetAll().AsNoTracking()
                            join l in iLabelRep.GetAll().AsNoTracking() on ql.labelId equals l.Id
                            where ql.questionId == dataRow.Id
                            select new
                            {
                                ql.labelType,
                                l.title
                            }).ToList();
                        var auxiliaryLable = ""; //辅标签
                        var label = "";//主标签
                        labelList.ForEach(l=>
                        {
                            if(l.labelType==0)
                            auxiliaryLable += l.title + ",";
                            else
                            label += l.title + ",";
                           
                        });
                       
                        if (!string.IsNullOrWhiteSpace(auxiliaryLable))
                        {
                            sb.Append("辅标签:" + auxiliaryLable.TrimEnd(',') + newline);
                        }
                        if (!string.IsNullOrWhiteSpace(label))
                        {
                            sb.Append("主标签:" + label.TrimEnd(',') + newline);
                        }

                    }
                    //所属知识点
                    //if (exportFields.ContainsKey(ExamQuestionTable.QuesitonKnowledgeField))
                    //{
                    //    string questionKnowledges = ExamQuestionTable.GetQuestionKnowledgeName(questionUid);
                    //    if (!string.IsNullOrEmpty(questionKnowledges))
                    //    {
                    //        sb.Append(questionField[ExamQuestionTable.QuesitonKnowledgeField] + ":" + questionKnowledges + newline);
                    //    }
                    //}
                    //状态
                    //if (status != "" && exportFields.ContainsKey(ExamQuestionField.QuestionStatusCode))
                    //{
                    //    sb.Append(questionField[ExamQuestionField.QuestionStatusCode] + ":" + status + newline);
                    //}
                    //顺序号
                    sb.Append("试题排序号:" + listOrder + newline);
                    //创建时间
                    //if (createTime != "" && exportFields.ContainsKey(ExamQuestionField.CreateTime))
                    //{
                    //    sb.Append(questionField[ExamQuestionField.CreateTime] + ":" + createTime + newline);
                    //}
                    //创建人
                    //if (creator != "" && exportFields.ContainsKey(ExamQuestionField.Creator))
                    //{
                    //    sb.Append(questionField[ExamQuestionField.Creator] + ":" + creator + newline);
                    //}
                    //最后修改时间
                    //if (modifyTime != "" && exportFields.ContainsKey(ExamQuestionField.ModifyTime))
                    //{
                    //    sb.Append(questionField[ExamQuestionField.ModifyTime] + ":" + modifyTime + newline);
                    //}
                    //最后修改人
                    //if (modifier != "" && exportFields.ContainsKey(ExamQuestionField.Modifier))
                    //{
                    //    sb.Append(questionField[ExamQuestionField.Modifier] + ":" + modifier + newline);
                    //}

                    //扩展属性
                    //string fdValue = string.Empty;
                    //foreach (NvExtendSettingRow extendRow in extendSettingRows)
                    //{
                    //    fdValue = ConvertUtil.ToString(dataRow[extendRow.FieldName], "");
                    //    if (!string.IsNullOrEmpty(fdValue) && exportFields.ContainsKey(extendRow.FieldName))
                    //        sb.Append(questionField[extendRow.FieldName] + ":" + ConvertUtil.ToString(dataRow[extendRow.FieldName], "") + newline);
                    //}
                    sb.Append(newline);
                    string text = sb.ToString();

                    //转换路径
                    if (isWithFilePath)
                        text = FilePathUtil.GetContentTextWithFilePath(questionUid, "question", text);

                    questionText.Append(text);

                    //如果是组合题，则要导出子试题
                    if (baseQuestionType == EnumQuestionBaseTypeCode.Compose)
                    {
                        //DataView dvChild = null;
                        //if (isIncludeChildQuestion)
                        //{
                        //    dvData.RowFilter = ExamQuestionField.ParentQuestionUid + "=" + StringUtil.QuotedToStr(questionUid);
                        //    dvChild = dvData;
                        //}
                        //else
                        //{
                        //    SearchCondition subFilter = new SearchCondition();
                        //    subFilter.And(ExamQuestionField.ParentQuestionUid, questionUid);			//取得子题目
                        //    dvChild = ExamQuestionTable.GetQuestionViewByFilter(subFilter).DefaultView;
                        //}
                        var dvChild = iExamQuestionRep.GetAll().Where(a => a.parentQuestionUid == questionUid.TryParseGuid()).ToList().MapTo<List<ExamQuestionDto>>();
                        questionText.Append("[开始子试题]:" + newline + newline);
                        ReturnValue retValue2 = ConvertQuestionToText(dvChild, fomartType, examQuestionTypeDic, nvFolders, iExamQuestionRep, iQuestionStandardCodeRep,iQuestionLabelRep, iLabelRep);
                        if (retValue2.HasError == false)
                        {
                            questionText.Append(retValue2.ReturnObject.ToString());
                        }
                        questionText.Append(newline + newline);
                        questionText.Append("[结束子试题]:" + newline + newline);
                    }
                    i++;
                }

                if (questionText.Length > 0)
                    questionText.Remove(questionText.Length - 4, 4);
                retValue.HasError = false;
                retValue.ReturnObject = questionText.ToString();
            }
            catch (Exception ex)
            {
                retValue.HasError = true;
                retValue.Message = ex.Message;
            }
            return retValue;
        }


        /// <summary>
        /// 把内容里的回车反编码,并把标致还原
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        public static string EnterCharRevert(string sContent)
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
        /// 把内容里的回车反编码,并把标致去掉
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        public static string EnterCharDecode(string sContent)
        {
            string sBracketsCharBegin = "$BracketCharBegin$";
            string sBracketsCharEnd = "$BracketCharEnd$";
            string sBracketBeginFlag = "$BracketBegin$";
            string sBracketEndFlag = "$BracketEnd$";
            string sEnterFlag = "$Enter$";
            string sNewContent = sContent;

            sNewContent = sNewContent.Replace(sBracketsCharBegin, "{");
            sNewContent = sNewContent.Replace(sBracketsCharEnd, "}");
            sNewContent = sNewContent.Replace(sEnterFlag, "\r\n");
            ////替换第一个开始标记
            //int index = sNewContent.IndexOf(sBracketBeginFlag);
            //if (index == 0)
            //    sNewContent = sNewContent.Substring(sBracketBeginFlag.Length);
            //else if (index > 0)
            //    sNewContent = sNewContent.Substring(0, index) + sNewContent.Substring(index + sBracketBeginFlag.Length);

            ////替换最后一下结束标记
            //index = sNewContent.LastIndexOf(sBracketEndFlag);
            //if (index > -1 && index + sBracketEndFlag.Length == sNewContent.Length)
            //{
            //    sNewContent = sNewContent.Substring(0, sNewContent.Length - sBracketEndFlag.Length);
            //}
            //else if (index > -1)
            //{
            //    sNewContent = sNewContent.Substring(0, index) + sNewContent.Substring(index + sBracketEndFlag.Length);
            //}

            ////替换其它的标记
            //sNewContent = sNewContent.Replace(sBracketBeginFlag, "{\r\n");
            //sNewContent = sNewContent.Replace(sBracketEndFlag, "\r\n}");

            sNewContent = sNewContent.Replace(sBracketBeginFlag, "");
            sNewContent = sNewContent.Replace(sBracketEndFlag, "");
            return sNewContent;
        }

        /// <summary>
        /// 把多行内容里的回车替换掉。多行内容是以单独的一行{开头
        /// 如：
        /// {
        /// 题干第一行
        /// 题干第二行
        /// }
        /// 答案：{
        /// 答案第一行
        /// 答案第二行
        /// }
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        public static string EnterCharEconde(string sContent)
        {
            string sBracketsCharBegin = "$BracketCharBegin$";
            string sBracketsCharEnd = "$BracketCharEnd$";
            string sBracketBeginFlag = "$BracketBegin$";
            string sBracketEndFlag = "$BracketEnd$";

            sContent = sContent.Replace("\\{", sBracketsCharBegin);
            sContent = sContent.Replace("\\}", sBracketsCharEnd);
            sContent = sContent.Replace("{\r\n", sBracketBeginFlag);
            sContent = sContent.Replace("\r\n}", sBracketEndFlag);
            sContent = sContent.Replace("}", sBracketEndFlag);
            //中文的括号也是同样的
            sContent = sContent.Replace("\\｛", sBracketsCharBegin);
            sContent = sContent.Replace("\\｝", sBracketsCharEnd);
            sContent = sContent.Replace("｛\r\n", sBracketBeginFlag);
            sContent = sContent.Replace("\r\n｝", sBracketEndFlag);
            sContent = sContent.Replace("｝", sBracketEndFlag);
            string sNewContent = string.Empty;
            string sLeftContent = sContent;		//剩余的
            //找到开头
            var nBeginPos = sLeftContent.IndexOf(sBracketBeginFlag, StringComparison.Ordinal);
            //找到结尾
            var nEndPos = sLeftContent.IndexOf(sBracketEndFlag, nBeginPos + 1, StringComparison.Ordinal);
            while (nBeginPos > -1 && nEndPos > -1)
            {
                //把前部分保存起来
                sNewContent = sNewContent + sLeftContent.Substring(0, nBeginPos + sBracketBeginFlag.Length);
                //中间内容部分
                var sMiddleContent = sLeftContent.Substring(nBeginPos + sBracketBeginFlag.Length, nEndPos + sBracketEndFlag.Length - nBeginPos - sBracketBeginFlag.Length);			//中间部分内容
                //中间内容部分的回车换掉
                sMiddleContent = sMiddleContent.Replace("\r\n", "<br/>");
                //把转换后的内容保存起来
                sNewContent = sNewContent + sMiddleContent;
                //把前部分去掉
                sLeftContent = sLeftContent.Substring(nEndPos + sBracketEndFlag.Length);

                //找到开头
                nBeginPos = sLeftContent.IndexOf(sBracketBeginFlag, StringComparison.Ordinal);
                //找到结尾
                nEndPos = sLeftContent.IndexOf(sBracketEndFlag, nBeginPos + 1, StringComparison.Ordinal);
            }
            //把后面的接上去
            sNewContent = sNewContent + sLeftContent;
            return sNewContent;
        }

        /// <summary>
        /// 返回试题答案
        /// </summary>
        /// <param name="questionBaseTypeCode"></param>
        /// <param name="sAnswer"></param>
        /// <returns></returns>
        public static string GetStandardAnswerText(string questionBaseTypeCode, string sAnswer)
        {
            sAnswer = sAnswer.Trim();
            if ((questionBaseTypeCode == QuestionTypeConst.Judge || questionBaseTypeCode == QuestionTypeConst.JudgeCorrect) && (sAnswer == "正确" || sAnswer == "Y"))
                return "Y";
            else if ((questionBaseTypeCode == QuestionTypeConst.Judge || questionBaseTypeCode == QuestionTypeConst.JudgeCorrect) && (sAnswer == "错误" || sAnswer == "N"))
                return "N";
            else if (questionBaseTypeCode == QuestionTypeConst.Single || questionBaseTypeCode == QuestionTypeConst.Multi || questionBaseTypeCode == QuestionTypeConst.EvaluationMulti || questionBaseTypeCode == QuestionTypeConst.EvaluationSingle)
            {
                string errorMessage;
                sAnswer = QuestionUtil.FormatSelectQuestionAnswer(sAnswer, out errorMessage);
                return sAnswer;
            }
            else
            {
                return sAnswer;
            }
        }


        /// <summary>
        /// 拷贝文件,源文本如aa<img src="试题.files/aa.jpg"/>bb,要把"试题.files/aa.jpg"文件拷成新文件放入试题文件夹并改为:aa<img src="./6CFA307C-D77F-4011-9B4C-FFF81C86645A.jpg"/>bb
        /// </summary>
        /// <param name="questionUid"></param>
        /// <param name="sContent"></param>
        /// <param name="sSourceFilePath"></param>
        /// <returns></returns>
        public static ReturnValue CopyContentFiles(Guid questionUid, ref string sContent, string sSourceFilePath)
        {
            ReturnValue retValue = new ReturnValue(false, "");
            string returnSourceFiles = "";
            //换成新格式如:aa#ImgFile0000000000bb
            string sNewText = ReplaceLocalElementWithFixString(sContent, "IMG", "./", "#ImgFile", ref returnSourceFiles);
            if (returnSourceFiles == "")
            {
                retValue.HasError = true;
                return retValue;
            }

            string[] arrSourceFileNames = StringUtil.Split(returnSourceFiles, "-|-");
            for (int i = 0; i < arrSourceFileNames.Length; i++)
            {
                string[] arrOneSourceFile = arrSourceFileNames[i].Split('|');
                string sSourceFileName = arrOneSourceFile[0];
                int nLastPointPos = sSourceFileName.LastIndexOf(".", StringComparison.Ordinal);
                string sExtendName = "";
                if (nLastPointPos > -1) sExtendName = sSourceFileName.Substring(nLastPointPos);
                var sDistFileName = Guid.NewGuid() + sExtendName;
                string sDistFilePath = AppConfiguration.FileServerFileRootPath + "/" + FilePathUtil.GetOppositeFileWebPathRoot(questionUid.ToString(), "question");

                retValue = FilePathUtil.CopyFile(sSourceFilePath + "/" + sSourceFileName, sDistFilePath + "/" + sDistFileName);
                if (retValue.HasError == false)
                {
                    //换成新的根目录下的格式如:aa<img src="./6CFA307C-D77F-4011-9B4C-FFF81C86645A.jpg">bb
                    string sNewImgFlag = "<IMG src=\"./" + sDistFileName + "\"";
                    if (arrOneSourceFile.Length == 3)
                    {
                        if (arrOneSourceFile[1] != "") sNewImgFlag = sNewImgFlag + " width=\"" + arrOneSourceFile[1] + "\"";
                        if (arrOneSourceFile[2] != "") sNewImgFlag = sNewImgFlag + " height=\"" + arrOneSourceFile[2] + "\"";
                    }
                    sNewImgFlag = sNewImgFlag + ">";
                    sNewText = sNewText.Replace("#ImgFile" + i.ToString("0000000000"), sNewImgFlag);
                }
                else
                {
                    return retValue;
                }
            }
            sContent = sNewText;
            retValue.HasError = false;
            return retValue;
        }

        //将文件是本地路径的元素更换成固定的的字符串并后面接上10位数找到的顺序编号  并返回文件名列表
        private static string ReplaceLocalElementWithFixString(string sHtml, string sElement, string sLocalUrlStart, string sFixString, ref string returnSourceFiles)
        {
            if (returnSourceFiles == null) 
                returnSourceFiles = "";

            string newContentText = "";

            string sFlag1 = "";
            string sFlag2 = "";
            switch (sElement)
            {
                case "IMG":
                    sFlag1 = "<IMG";
                    sFlag2 = ">";
                    sHtml = sHtml.Replace("<img", "<IMG");
                    break;
                case "EMBED":
                    sFlag1 = "<EMBED";
                    sFlag2 = "</EMBED>";
                    sHtml = sHtml.Replace("<embed", "<EMBED");
                    break;

            }

            int nFoundIndex = 0;

            var nPos1 = sHtml.IndexOf(sFlag1, StringComparison.Ordinal);
            while (nPos1 > -1)
            {
                var nPos2 = sHtml.IndexOf(sFlag2, nPos1 + 1, StringComparison.Ordinal);
                var sElementHtml = sHtml.Substring(nPos1, nPos2 - nPos1 + sFlag2.Length);
                string sFileName = GetPropertityValueFromElementHtml(sElementHtml, "src");
                //if (sFileName.StartsWith(sLocalUrlStart))		//如果是本地路径则要更换
                //{
                    var sFoundIndex = nFoundIndex.ToString("0000000000");
                    newContentText = newContentText + sHtml.Substring(0, nPos1) + sFixString + sFoundIndex;

                    string sWidth = "";
                    string sHeight = "";
                    string sStyle = GetPropertityValueFromElementHtml(sElementHtml, "style").ToLower();
                    if (sStyle != "")
                    {
                        string[] arrStyle = sStyle.Split(';');
                        for (int j = 0; j < arrStyle.Length; j++)
                        {
                            arrStyle[j] = arrStyle[j].Trim();
                            if (arrStyle[j].StartsWith("width:"))
                                sWidth = arrStyle[j].Substring("width:".Length);
                            if (arrStyle[j].StartsWith("height:"))
                                sHeight = arrStyle[j].Substring("height:".Length);
                        }
                    }
                    else
                    {
                        sWidth = GetPropertityValueFromElementHtml(sElementHtml, "width");
                        sHeight = GetPropertityValueFromElementHtml(sElementHtml, "height");
                    }
                    if (sWidth.ToLower().EndsWith("px")) sWidth = sWidth.Substring(0, sWidth.Length - 2);
                    if (sHeight.ToLower().EndsWith("px")) sHeight = sHeight.Substring(0, sHeight.Length - 2);
                    returnSourceFiles = returnSourceFiles + "-|-" + sFileName + "|" + sWidth + "|" + sHeight;
                    sHtml = sHtml.Substring(nPos2 + sFlag2.Length, sHtml.Length - nPos2 - sFlag2.Length);
                    nFoundIndex = nFoundIndex + 1;
                //}
                //else
                //{
                //    newContentText = newContentText + sHtml.Substring(0, nPos2 + sFlag2.Length);
                //    sHtml = sHtml.Substring(nPos2 + sFlag2.Length, sHtml.Length - nPos2 - sFlag2.Length);
                //}
                nPos1 = sHtml.IndexOf(sFlag1, StringComparison.Ordinal);
            }
            newContentText = newContentText + sHtml;

            if (returnSourceFiles != "") returnSourceFiles = returnSourceFiles.Substring(3);
            return newContentText;
        }

        //从元素的HTML中取得文件名如:<IMG src="./aa.gif">中取出http://localhost/FileRoot/Content/G1/5/aa.gif
        private static string GetPropertityValueFromElementHtml(string sHtml, string sPropertityName)
        {
            string sOldHtml = sHtml;

            sHtml = sHtml.ToLower();
            sPropertityName = sPropertityName.ToLower();

            var sFlag2 = sPropertityName + "=";

            var nPos1 = sHtml.IndexOf(sFlag2, StringComparison.Ordinal);
            var nPos2 = sHtml.IndexOf(">", nPos1 + sFlag2.Length + 1, StringComparison.Ordinal);
            var nPos3 = sHtml.IndexOf("\"", nPos1 + sFlag2.Length + 1, StringComparison.Ordinal);
            var nPos4 = sHtml.IndexOf(" ", nPos1 + sFlag2.Length + 1, StringComparison.Ordinal);
            if (nPos2 > -1 && nPos3 > -1 && nPos3 < nPos2) nPos2 = nPos3;	//如果空格在前面，则以空格结束
            if (nPos2 > -1 && nPos4 > -1 && nPos3 == -1 && nPos4 < nPos2) nPos2 = nPos4;	//如果"在前面，则以空格结束
            string sValue = "";
            if (nPos1 > -1 && nPos2 > -1)
                sValue = sOldHtml.Substring(nPos1 + sFlag2.Length, nPos2 - nPos1 - sFlag2.Length);
            sValue = sValue.Trim('"');
            sValue = sValue.Trim('\'');
            sValue = sValue.TrimEnd(' ');
            return sValue;
        }

        #endregion


        #region 试卷的导入与导出

        public static ReturnValue ConvertPaperObjectToText(ExamPaper paperRow, IEnumerable<ExamPaperNode> nodes, 
            IEnumerable<ExamPaperNodeQuestion> nodeQuestions, IEnumerable<ExamQuestionDto> questions, 
            IDictionary<Guid, ExamQuestionType> questionTypeDic,  IEnumerable<NvFolder> questionFolders, 
            IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<QuestionStandardCode, Guid> iQuestionStandardCodeRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep, IRepository<Label, Guid> iLabelRep)
        {
            string newline = "<br>";
            ReturnValue reValue = new ReturnValue();
            try
            {
                StringBuilder sb = new StringBuilder();
                SearchCondition filter = null;
                DataTable dtInfo = null;

                //====1. 开始导出数据 =============
                sb.Append("[试卷信息]:" + newline);
                sb.Append("试卷名称:" + paperRow.paperName + newline);
                //if (!string.IsNullOrEmpty(paperRow.PaperUid))
                //{
                //    sb.Append(ExamPaperField.GetFieldTitle(ExamPaperField.PaperUid) + ":" + paperRow.PaperUid + newline);
                //}
                //试题编号
                //sb.Append(ExamPaperField.GetFieldTitle(ExamPaperField.PaperCode) + ":" + paperRow.PaperCode + newline);
                if (paperRow.planTotalScore != null && paperRow.planTotalScore > 0)
                {
                    sb.Append("计划分数:" + paperRow.planTotalScore.Value.ToString("0.##") + newline);
                }
                if (!string.IsNullOrEmpty(paperRow.isShowScore))
                {
                    sb.Append("是否显示分数:" + paperRow.isShowScore + newline);
                }
                if (!string.IsNullOrEmpty(paperRow.isSingleAsMulti))
                {
                    sb.Append("单选变为不定项:" + paperRow.isSingleAsMulti + newline);
                }
                //if (!string.IsNullOrEmpty(paperRow.paperClassCode))
                //{
                //    NvFieldOptionRow optionRow = NvFieldOptionTable.CreateRowBy(ExamPaperTable.TableName, ExamPaperField.PaperClassCode, paperRow.PaperClassCode);
                //    if (optionRow != null)
                //    {
                //        sb.Append(ExamPaperField.GetFieldTitle(ExamPaperField.PaperClassCode) + ":" + optionRow.OptionText) + newline;
                //    }
                //}
                sb.Append("试卷类型:通用试卷" + newline);
                sb.Append(newline);

                //======2. 开始写大题信息 =============
                //filter = new SearchCondition();
                //filter.And(ExamPaperNodeField.PaperUid, paperRow.PaperUid);
                //dtInfo = ExamPaperNodeTable.SelectByFilter(filter, ExamPaperNodeField.ListOrder);
                nodes = nodes.OrderBy(a => a.listOrder);
                foreach (var nodeRow in nodes)
                {
                    //ExamPaperNodeRow nodeRow = new ExamPaperNodeRow();
                    //nodeRow.AssignByDataRow(dataRow);
                    //开始写数据
                    sb.Append("[试卷大题信息]:" + newline);
                    sb.Append("试卷大题名称:" + nodeRow.paperNodeName + newline);
                    //if (!string.IsNullOrEmpty(nodeRow.PaperNodeUid))
                    //{
                    //    sb.Append(ExamPaperNodeField.GetFieldTitle(ExamPaperNodeField.PaperNodeUid) + ":" + nodeRow.PaperNodeUid + newline);
                    //}
                    if (nodeRow.questionTypeUid != Guid.Empty)
                    {
                        var questionType = questionTypeDic[nodeRow.questionTypeUid];
                        sb.Append("题型:" + questionType.questionTypeName + newline);
                    }
                    else
                    {
                        sb.Append("题型:" + "不限题型" + newline);
                    }
                    if (nodeRow.planQuestionNum > 0)
                    {
                        sb.Append("计划题目数:" + nodeRow.planQuestionNum.ToString("0.##") + newline);
                    }
                    if (nodeRow.questionScore > 0)
                    {
                        sb.Append("每题分数:" + nodeRow.questionScore.ToString("0.##") + newline);
                    }
                    if (!string.IsNullOrEmpty(nodeRow.paperNodeDesc))
                    {
                        sb.Append("试卷大题说明:" + ExamImportAndExportUtil.ReplaceExportTextParagraph(nodeRow.paperNodeDesc, "<br>") + newline);
                    }
                    sb.Append(newline);

                    //=======3. 开始写试题信息 ===============

                    //DataView dvQuestion = ExamPaperNodeQuestionTable.GetQuestionViewByPaperNodeUid(nodeRow.PaperNodeUid, paperRow.PaperUid).DefaultView;
                    ////处理试题分数
                    //for (int d = 0; d < dvQuestion.Count; d++)
                    //{
                    //    dvQuestion[d][ExamQuestionField.Score] = dvQuestion[d][ExamPaperNodeQuestionField.PaperQuestionScore];
                    //}

                    //处理分数与排序（移植后新增的）
                    var qList = new List<ExamQuestionDto>();
                    var nqList = nodeQuestions.Where(a => a.paperNodeUid == nodeRow.Id);
                    foreach (var nq in nqList)
                    {
                        var q = questions.First(a => a.Id == nq.questionUid);
                        q.listOrder = nq.listOrder;
                        q.score = nq.paperQuestionScore;
                        qList.Add(q);
                    }

                    reValue = ExamImportAndExportUtil.ConvertQuestionToText(qList, EnumExportTextType.Word, questionTypeDic, questionFolders, iExamQuestionRep, iQuestionStandardCodeRep,iQuestionLabelRep, iLabelRep);
                    if (reValue.HasError)
                        return reValue;
                    sb.Append(reValue.ReturnObject.ToString());
                    sb.Append(newline);
                }

                reValue.ReturnObject = sb.ToString();
                reValue.HasError = false;
                return reValue;
            }
            catch (Exception ex)
            {
                reValue.HasError = true;
                reValue.Message = ex.Message;
            }
            return reValue;
        }

        

        public static ReturnValue CheckPaperTextFormat(string sOneContent, Dictionary<string, string> paperField, IRepository<ExamPaper, Guid> iExamPaperRep)
        {
            ReturnValue reValue = new ReturnValue(false, "");
            ArrayList errorMessage = new ArrayList();

            try
            {
                Hashtable propertyList = GetPaperOrPaperNodeProperty(sOneContent);

                //检查必要的字段
                //试卷名称
                if (!propertyList.ContainsKey(paperField["paperName"]))
                {
                    errorMessage.Add(("未找到属性") + ":" + paperField["paperName"]);
                    reValue.HasError = true;
                }
                foreach (string key in propertyList.Keys)
                {
                    string fieldName = GetDictionaryKeyByTitle(paperField, key);
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        //是否显示分数
                        if (fieldName == "isShowScore")
                        {
                            string keyValue = propertyList[key].ToString().ToUpper().Trim();
                            if (keyValue != "Y" && keyValue != "N")
                            {
                                errorMessage.Add(("无效的属性值,应为'Y'或'N'") + ": " + keyValue);
                                reValue.HasError = true;
                            }
                        }
                        //试卷编号
                        else if (fieldName == "paperCode")
                        {
                            string keyValue = propertyList[key].ToString().Trim();
                            //var db = ContextFactory.GetCurrentContext();
                            //var paper = db.ExamPapers.FirstOrDefault(a => a.paperCode.Equals(keyValue));
                            var paper = iExamPaperRep.GetAll().FirstOrDefault(a => a.paperCode == keyValue);
                            if (!string.IsNullOrEmpty(keyValue) && paper != null)
                            {
                                errorMessage.Add(string.Format(("无效的属性值:{0},存在重复的值."), keyValue));
                                reValue.HasError = true;
                            }
                        }
                        //是否把单选转化为多选
                        else if (fieldName == "isSingleAsMulti")
                        {
                            string keyValue = propertyList[key].ToString().ToUpper().Trim();
                            if (keyValue != "Y" && keyValue != "N")
                            {
                                errorMessage.Add(("无效的属性值,应为'Y'或'N'.") + keyValue);
                                reValue.HasError = true;
                            }
                        }
                        //计划试题总分
                        else if (fieldName == "planTotalScore")
                        {
                            string keyValue = propertyList[key].ToString().ToUpper().Trim();
                            if (!StringUtil.IsNumber(keyValue))
                            {
                                errorMessage.Add(("无效的属性值,数字类型: ") + keyValue);
                                reValue.HasError = true;
                            }
                        }
                        else if (fieldName == "paperClassCode")
                        {
                            string keyValue = propertyList[key].ToString().ToUpper().Trim();
                            if (!string.IsNullOrEmpty(keyValue))
                            {
                                if (keyValue == ("测评试卷").ToUpper()) keyValue = "测评试卷";
                                else if (keyValue == ("通用试卷").ToUpper()) keyValue = "通用试卷";

                                if (!keyValue.Equals("通用试卷") && !keyValue.Equals("测评试卷"))
                                {
                                    errorMessage.Add(("未知的试卷类型: ") + keyValue);
                                    reValue.HasError = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        errorMessage.Add(("未知的属性:") + key);
                        reValue.HasError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                reValue.HasError = true;
                errorMessage.Add(("未知的错误:") + ex.Message);
            }

            reValue.PutValue("errorMessage", errorMessage);
            return reValue;
        }

        public static ReturnValue CheckPaperNodeTextFormat(string sOneContent, Dictionary<string, string> paperNodeField, Hashtable keyValueCatch, IRepository<ExamQuestionType, Guid> iExamQuestionRep)
        {
            ReturnValue reValue = new ReturnValue(false, "");
            ArrayList errorMessage = new ArrayList();

            try
            {
                Hashtable propertyList = GetPaperOrPaperNodeProperty(sOneContent);
                //检查必要的字段
                //试卷名称
                if (!propertyList.ContainsKey(paperNodeField["paperNodeName"]))
                {
                    errorMessage.Add(("未找到属性:") + paperNodeField["paperNodeName"]);
                    reValue.HasError = true;
                }
                foreach (string key in propertyList.Keys)
                {
                    string fieldName = GetDictionaryKeyByTitle(paperNodeField, key);
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        //试题类型
                        if (fieldName == "id")
                        {
                            string typeName = propertyList[key].ToString().Trim();
                            if (typeName == ("不限题型"))
                                continue;
                            //var db = ContextFactory.GetCurrentContext();
                            //var examQuestionType =
                            //    db.ExamQuestionTypes.FirstOrDefault(a => a.question_type_name.Equals(typeName));
                            var examQuestionType = iExamQuestionRep.FirstOrDefault(a => a.questionTypeName == typeName);
                            var typeUid = examQuestionType == null ? Guid.Empty : examQuestionType.Id;
                            if (typeUid == Guid.Empty)
                            {
                                errorMessage.Add(("无效的属性值:") + typeName);
                                reValue.HasError = true;
                            }
                        }
                        //计划试题数量
                        else if (fieldName == "planQuestionNum")
                        {
                            string keyValue = propertyList[key].ToString().Trim();
                            if (!StringUtil.IsNumber(keyValue))
                            {
                                errorMessage.Add(("无效的属性值,数字类型: ") + keyValue);
                                reValue.HasError = true;
                            }
                        }
                        //每题分数
                        else if (fieldName == "questionScore")
                        {
                            string keyValue = propertyList[key].ToString().Trim();
                            if (!StringUtil.IsNumber(keyValue))
                            {
                                errorMessage.Add(("无效的属性值,数字类型: ") + keyValue);
                                reValue.HasError = true;
                            }
                        }
                    }
                    else
                    {
                        errorMessage.Add(("未知的属性:") + key);
                        reValue.HasError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                reValue.HasError = true;
                errorMessage.Add(("未知的错误:") + ex.Message);
            }
            reValue.PutValue("errorMessage", errorMessage);

            return reValue;
        }


        /// <summary>
        /// 将试卷导入者信息带入，取代API
        /// </summary>
        /// <param name="sOneContent"></param>
        /// <param name="paperField"></param>
        /// <param name="papercreatorUid"></param>
        /// <param name="iExamPaperRep"></param>
        /// <remarks></remarks>
        /// <returns></returns>
        public static ExamPaper ConvertTextToPaperRow(string sOneContent, Dictionary<string, string> paperField, Guid papercreatorUid, IRepository<ExamPaper, Guid> iExamPaperRep)
        {
            Hashtable propertyList = GetPaperOrPaperNodeProperty(sOneContent);
            var paperUid = Guid.Empty;
            if (propertyList.ContainsKey(paperField["id"]))
            {
                paperUid = Guid.Parse(propertyList[paperField["id"]].ToString());
            }
            ExamPaper paperRow;
            if (paperUid == Guid.Empty)
            {
                paperRow = new ExamPaper
                {
                    createTime = DateTime.Now,
                    lastUpdateTime = DateTime.Now,
                    creatorUid = papercreatorUid,
                    paperTypeCode = "fix"
                };
            }
            else
            {
                paperRow = iExamPaperRep.FirstOrDefault(a => a.Id == paperUid);
                if (paperRow == null)
                {
                    paperRow = new ExamPaper
                    {
                        createTime = DateTime.Now,
                        creatorUid = papercreatorUid,
                        paperTypeCode = "fix"
                    };
                }
            }
            foreach (string key in propertyList.Keys)
            {
                //试卷编号
                string keyValue;
                if (key == paperField["paperCode"])
                {
                    keyValue = propertyList[key].ToString().Trim();
                    paperRow.paperCode = keyValue;
                }
                //是否显示分数
                if (key == paperField["isShowScore"])
                {
                    keyValue = propertyList[key].ToString().ToUpper().Trim();
                    paperRow.isShowScore = keyValue;
                }
                //是否把单选转化为多选
                else if (key == paperField["isSingleAsMulti"])
                {
                    keyValue = propertyList[key].ToString().ToUpper().Trim();
                    paperRow.isSingleAsMulti = keyValue;
                }
                //试卷名称
                else if (key == paperField["paperName"])
                {
                    keyValue = propertyList[key].ToString().Trim();
                    paperRow.paperName = keyValue;
                }
                //计划试题总分
                else if (key == paperField["planTotalScore"])
                {
                    keyValue = propertyList[key].ToString().Trim();
                    paperRow.planTotalScore = ConvertUtil.ToDecimal(keyValue);
                }
                else if (key == paperField["paperClassCode"])
                {
                    keyValue = propertyList[key].ToString().Trim();
                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        paperRow.paperClassCode = "exam";
                    }
                }
            }
            if (string.IsNullOrEmpty(paperRow.paperCode))
            {
                paperRow.paperCode = CreateNewCode(iExamPaperRep);
            }
            if (string.IsNullOrEmpty(paperRow.isShowScore))
                paperRow.isShowScore = "Y";
            if (string.IsNullOrEmpty(paperRow.isSingleAsMulti))
                paperRow.isSingleAsMulti = "N";
            if (string.IsNullOrEmpty(paperRow.paperClassCode))
                paperRow.paperClassCode = "exam";
            if (paperRow.Id == Guid.Empty)
            {
                paperRow.statusCode = "approved";
            }
            return paperRow;
        }

        public static ExamPaperNode ConvertTextToPaperNodeRow(string sOneContent, Dictionary<string, string> paperNodeField, Guid paperUid, Hashtable keyValueCatch, IRepository<ExamPaperNode, Guid> iExamPaperNodeRep, IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep)
        {
            Hashtable propertyList = GetPaperOrPaperNodeProperty(sOneContent);
            var paperNodeUid = Guid.Empty;
            if (propertyList.ContainsKey(paperNodeField["id"]))
            {
                paperNodeUid = Guid.Parse(propertyList[paperNodeField["id"]].ToString());
            }
            ExamPaperNode paperNodeRow;
            if (paperNodeUid == Guid.Empty)
            {
                paperNodeRow = new ExamPaperNode ();
            }
            else
            {
                var paperNode = iExamPaperNodeRep.FirstOrDefault(a => a.Id == paperNodeUid);
                if (paperNode != null)
                {
                    //如果试卷Uid不相同,那么做为新的试卷大题来处理
                    if (paperUid != Guid.Empty && paperNode.Id.ToString().ToLower() != paperUid.ToString().ToLower())
                    {
                        var paperNodeDto = paperNode.MapTo<ExamPaperNodeDto>();
                        paperNodeDto.Id = Guid.Empty;

                        paperNodeRow = paperNodeDto.MapTo<ExamPaperNode>();
                    }
                    else
                    {
                        paperNodeRow = paperNode;
                    }
                }
                else
                {
                    paperNodeRow = new ExamPaperNode ();
                }
            }
            foreach (string key in propertyList.Keys)
            {
                //题型
                string keyValue;
                if (key == paperNodeField["questionTypeUid"])
                {
                    keyValue = propertyList[key].ToString().Trim();
                    if (keyValue == ("不限题型"))
                    {
                        paperNodeRow.questionTypeUid = Guid.Empty;
                    }
                    else
                    {
                        var value = keyValue;
                        var questionType = iExamQuestionTypeRep.FirstOrDefault(a => a.questionTypeName == value);
                        paperNodeRow.questionTypeUid = questionType == null ? Guid.Empty : questionType.Id;
                    }
                }
                //试卷大题简介
                else if (key == paperNodeField["paperNodeDesc"])
                {
                    keyValue = propertyList[key].ToString().Trim();
                    paperNodeRow.paperNodeDesc = keyValue;
                }
                //试卷大题名称
                else if (key == paperNodeField["paperNodeName"])
                {
                    keyValue = propertyList[key].ToString().Trim();
                    paperNodeRow.paperNodeName = keyValue;
                }
                //计划试题数
                else if (key == paperNodeField["planQuestionNum"])
                {
                    keyValue = propertyList[key].ToString().Trim();
                    paperNodeRow.planQuestionNum = ConvertUtil.ToInt(keyValue);
                }
                //每题分数
                else if (key == paperNodeField["questionScore"])
                {
                    keyValue = propertyList[key].ToString().Trim();
                    paperNodeRow.questionScore = ConvertUtil.ToDecimal(keyValue);
                }
                else if (key == paperNodeField[""])
                {
                    
                }
            }

            return paperNodeRow;
        }

        public static Hashtable GetPaperOrPaperNodeProperty(string sOneContent)
        {
            char[] arrSplit = { '\r', '\n' };
            string[] arrOneContent = sOneContent.Split(arrSplit);
            Hashtable properties = new Hashtable();
            //=======2. 取试题序号 ===============
            for (int i = 0; i < arrOneContent.Length; i++)
            {
                if (arrOneContent[i].StartsWith("//"))
                    continue;

                arrOneContent[i] = arrOneContent[i].Replace("：", ":");
                int index = arrOneContent[i].IndexOf(':');
                if (index > 0)
                {
                    string propertyName = arrOneContent[i].Substring(0, index);
                    var sReturn = arrOneContent[i].Substring(index + 1);
                    sReturn = sReturn.Trim();
                    //对属性内容进行回车反编码
                    sReturn = EnterCharDecode(sReturn);
                    //要检查是否有，防止重复写属性
                    if (!properties.ContainsKey(propertyName)) properties.Add(propertyName, sReturn);

                }
            }

            return properties;
        }

        #endregion

        public static string GetDictionaryKeyByTitle(Dictionary<string, string> dictionary, string title)
        {
            foreach (string key in dictionary.Keys)
            {
                if (title == dictionary[key])
                    return key;
            }
            return "";
        }

        private static string CreateNewCode(IRepository<ExamPaper, Guid> iExamPaperRep)
        {
            var code = "P000001";
            var entity = iExamPaperRep.GetAll().Where(a => !a.isCustomCode).OrderByDescending(a => a.createTime).FirstOrDefault();
            if (entity != null)
            {
                code = entity.paperCode;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (iExamPaperRep.GetAll().Any(a => a.paperCode == code));
            }
            return code;
        }
    }
    public class EnumExportTextType
    {
        /// <summary>
        /// Word格式
        /// </summary>
        public static readonly string Word = "word";

        /// <summary>
        /// 文本格式
        /// </summary>
        public static readonly string Text = "text";
    }
}