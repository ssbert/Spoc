using Abp.Domain.Repositories;
using SPOC.Common.Encrypt;
using SPOC.Common.Exam;
using SPOC.Common.File;
using SPOC.Exam;
using SPOC.ExamPaper.Dto;
using SPOC.ExamPaper.Struct;
using SPOC.QuestionBank;
using SPOC.QuestionBank.Const;
using SPOC.QuestionBank.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using newv.common;
using SPOC.Common.Extensions;
using StringUtil = SPOC.Common.Helper.StringUtil;

namespace SPOC.ExamPaper
{
    public class PaperUtil
    {
        private readonly IRepository<ExamPaperNodeQuestion, Guid> _iExamPaperNodeQuestionRep;
        private readonly IRepository<ExamQuestionType, Guid> _iExamQuestionTypeRep;
        private readonly IRepository<ExamExam, Guid> _iExamRep;
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<ExamAnswer, Guid> _iExamAnswerRep;
        private readonly IRepository<ExamProgramResult, Guid> _iExamProgramResultRep;

        public PaperUtil(IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep)
        {
            _iExamPaperNodeQuestionRep = iExamPaperNodeQuestionRep;
        }

        public PaperUtil(IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep,
            IRepository<ExamExam, Guid> iExamRep, IRepository<ExamGrade, Guid> iExamGradeRep, IRepository<ExamAnswer, Guid> iExamAnswerRep, 
            IRepository<ExamProgramResult, Guid> iExamProgramResultRep)
        {
            _iExamQuestionTypeRep = iExamQuestionTypeRep;
            _iExamGradeRep = iExamGradeRep;
            _iExamRep = iExamRep;
            _iExamAnswerRep = iExamAnswerRep;
            _iExamProgramResultRep = iExamProgramResultRep;
        }

        public string TranslatePaperQuestionObjectToXml(ExamPaperNodeQuestion examPaperNodeQuestionRow, ExamQuestionDto examQuestionRow)
        {
            char[] spliter = "|".ToCharArray();
            var sb = new StringBuilder();
            
            string oppositePath = "/fileroot/" + GetOppositeFileWebPathRoot(examPaperNodeQuestionRow.questionUid.ToString(), "question") + "/";

            //试卷里的试题信息
            sb.AppendLine("                    <" + "question_uid" + ">" + examPaperNodeQuestionRow.questionUid + "</" + "question_uid" + ">");
            sb.AppendLine("                    <" + "paper_question_score" + ">" + examPaperNodeQuestionRow.paperQuestionScore.ToString("0.##") + "</" + "paper_question_score" + ">");
            //如果两者时间不相等，证明是在题库管理中修改了试题的答题时间，这时也应该修改试题在试卷中的节点试题的答题时间，使其同步起来
            if (examPaperNodeQuestionRow.paperQuestionExamTime != examQuestionRow.examTime)
            {
                examPaperNodeQuestionRow.paperQuestionExamTime = examQuestionRow.examTime;
                _iExamPaperNodeQuestionRep.UpdateAsync(examPaperNodeQuestionRow);
            }
            sb.AppendLine("                    <" + "paper_question_exam_time" + ">" + examPaperNodeQuestionRow.paperQuestionExamTime + "</" + "paper_question_exam_time" + ">");
            sb.AppendLine("                    <" + "list_order" + ">" + examPaperNodeQuestionRow.listOrder + "</" + "list_order" + ">");

            //题库中的试题信息
            string questionText = examQuestionRow.questionText;
            //替换字符
            questionText = StringUtil.ReplaceSpaces2Html(questionText);

            questionText = GetContentTextWithFilePath(questionText, oppositePath);

            sb.AppendLine("                    <" + "question_text" + "><![CDATA[" + questionText + "]]></" + "question_text" + ">");
            sb.AppendLine("                    <" + "question_base_type_code" + ">" + examQuestionRow.questionBaseTypeCode + "</" + "question_base_type_code" + ">");
            sb.AppendLine("                    <" + "question_type_uid" + ">" + examQuestionRow.questionTypeUid + "</" + "question_type_uid" + ">");
            sb.AppendLine("                    <" + "is_answer_by_html" + ">" + examQuestionRow.isAnswerByHtml + "</" + "is_answer_by_html" + ">");
            sb.AppendLine("                    <" + "is_only_upload_file" + ">" + examQuestionRow.isOnlyUploadFile + "</" + "is_only_upload_file" + ">");
            sb.AppendLine("                    <" + "operate_type_code" + ">" + examQuestionRow.operateTypeCode + "</" + "operate_type_code" + ">");
            sb.AppendLine("                    <" + "hard_grade" + ">" + examQuestionRow.hardGrade + "</" + "hard_grade" + ">");
            if (examQuestionRow.questionBaseTypeCode == "fill")
            {
                sb.AppendLine("                    <" + "in_order" + ">" + (examQuestionRow.inOrder?"true":"false") + "</" + "in_order" + ">");
            }
            if (examQuestionRow.questionBaseTypeCode == "answer")
            {
                sb.AppendLine("                    <" + "select_answer" + ">" + examQuestionRow.selectAnswer + "</" + "select_answer" + ">");
            }

            if (examQuestionRow.questionBaseTypeCode == "program" || examQuestionRow.questionBaseTypeCode == "program_fill")
            {
                sb.AppendLine("                    <" + "language" + ">" + examQuestionRow.language + "</" + "language" + ">");
            }

            if (examQuestionRow.questionBaseTypeCode == "program_fill")
            {
                sb.AppendLine("                    <" + "preinstall_code" + ">" + Uri.EscapeUriString(examQuestionRow.PreinstallCode) + "</" + "preinstall_code" + ">");
            }

            sb.AppendLine("                    <" + "select_answer_score" + ">" + examQuestionRow.selectAnswerScore + "</" + "select_answer_score" + ">");
            sb.AppendLine("                    <" + "question_type_uid" + ">" + examQuestionRow.questionTypeUid + "</" + "question_type_uid" + ">");
            sb.AppendLine("                    <question_no>" + examQuestionRow.questionCode + "</question_no>");
            sb.AppendLine("                    <question_order>" + examQuestionRow.listOrder + "</question_order>");

            string standardAnswer = examQuestionRow.standardAnswer;
            standardAnswer = GetContentTextWithFilePath(standardAnswer, oppositePath);
            if (!string.IsNullOrEmpty(standardAnswer))
                standardAnswer = EasyCryptoUnit.Encode(standardAnswer);
            sb.AppendLine("                    <" + "standard_answer" + "><![CDATA[" + standardAnswer + "]]></" + "standard_answer" + ">");
            string questionAnalysis = examQuestionRow.questionAnalysis;
            questionAnalysis = GetContentTextWithFilePath(questionAnalysis, oppositePath);
            sb.AppendLine("                    <" + "question_analysis" + "><![CDATA[" + questionAnalysis + "]]></" + "question_analysis" + ">");


            //如果是单选多选判断题则把选项列出来

            if (examQuestionRow.questionBaseTypeCode == "judge" || examQuestionRow.questionBaseTypeCode == "judge_correct")
            {
                sb.AppendLine("                    <select_answers>");
                sb.AppendLine("                        <select_answer>");
                sb.AppendLine("                            <select_answer_text>" + ("错误") + "</select_answer_text>");
                sb.AppendLine("                            <select_answer_value>N</select_answer_value>");
                sb.AppendLine("                        </select_answer>");

                sb.AppendLine("                        <select_answer>");
                sb.AppendLine("                            <select_answer_text>" + ("正确") + "</select_answer_text>");
                sb.AppendLine("                            <select_answer_value>Y</select_answer_value>");
                sb.AppendLine("                        </select_answer>");
                sb.AppendLine("                    </select_answers>");
            }
            else if (examQuestionRow.questionBaseTypeCode == QuestionTypeConst.EvaluationMulti || examQuestionRow.questionBaseTypeCode == QuestionTypeConst.EvaluationSingle || examQuestionRow.questionBaseTypeCode == QuestionTypeConst.Multi || examQuestionRow.questionBaseTypeCode == QuestionTypeConst.Single)
            {
                sb.AppendLine("                    <select_answers>");
                string selectAnswer = examQuestionRow.selectAnswer;

                selectAnswer = StringUtil.ReplaceSpaces2Html(selectAnswer);
                selectAnswer = GetContentTextWithFilePath(selectAnswer, oppositePath);

                string[] arrSelectAnswer = selectAnswer.Split(spliter);

                int k = 0;
                foreach (string sAnswer in arrSelectAnswer)
                {

                    var upChar = ((Char)(65 + k)).ToString();
                    sb.AppendLine("                        <select_answer>");
                    sb.AppendLine("                            <select_answer_text><![CDATA[" + sAnswer + "]]></select_answer_text>");
                    sb.AppendLine("                            <select_answer_value>" + upChar + "</select_answer_value>");
                    sb.AppendLine("                        </select_answer>");
                    k++;
                }
                sb.AppendLine("                    </select_answers>");
            }
            return sb.ToString();
        }
        public string GetOppositeFileWebPathRoot(string recordUid, string recordType)
        {
            string oppositePath = recordType + "/" + recordUid;
            return oppositePath;
        }
        public string GetContentTextWithFilePath(string contentText, string absoluteContentFileWebPathRoot, bool changeEnterAsBr = false)
        {
            if (contentText == null)
                return "";
            if (contentText.Length > 7)
            {
                if (contentText.Substring(0, 3).ToUpper() == "<P>" && contentText.Substring(contentText.Length - 4, 4).ToUpper() == "</P>" && contentText.ToUpper().IndexOf("<P>", 3, StringComparison.Ordinal) == -1)
                    contentText = contentText.Substring(3, contentText.Length - 7);
            }

            if (contentText == "" || absoluteContentFileWebPathRoot == "")
                return contentText;

            //顺便在这里处理回车的问题
            //回车有两种，一种是HTML中的回车，一般是在>后回车，这种不需要转换
            //另一种是内容本身是回车，这种要转换成<br/>
            if (changeEnterAsBr)
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
                contentText = contentText.Replace("<source>./../../", "<source>" + GetWebFileRoot() + "/");
            else
                contentText = contentText.Replace("<source>./", "<source>" + absoluteContentFileWebPathRoot);

            ////不能作 html 符号的处理，否则会影响到 html 的正常显示结果.
            //contentText = contentText.Replace("<", "＜");
            //contentText = contentText.Replace(">", "＞");
            return contentText;
        }
        public string GetWebFileRoot()
        {
            var context = System.Web.HttpContext.Current;
            string urlSuffix = string.Empty;

            if (context.Request.Url.IsDefaultPort == false)
            {
                if (context.Request.ApplicationPath != null)
                    urlSuffix = context.Request.Url.Host + ":" + context.Request.Url.Port.ToString() + context.Request.ApplicationPath.Replace("\\", "/").TrimEnd('/');
            }
            else if (context.Request.ApplicationPath != null)
                urlSuffix = context.Request.Url.Host + context.Request.ApplicationPath.Replace("\\", "/").TrimEnd('/');
            return @"http://" + urlSuffix.ToLower();
        }
        public string GetPaperQuestionViewForPaper(PaperViewBuildInfo info)
        {

            #region GetPaperContentsViewForPaperNode
            StringBuilder sbReturn = new StringBuilder();


            //试卷基本信息
            bool isSingleAsMulti = (info.ExamPaperRow.isSingleAsMulti == "Y") ? true : false;

            for (int m = 0; m < info.ExamPaperNodeRowCollection.Count; m++)
            {
                var examPaperNodeRow = info.ExamPaperNodeRowCollection[m];
                var nodeViewBuilInfo = new NodeViewBuildInfo
                {
                    ViewType = info.ViewType,
                    PaperNodeNumber = m + 1,
                    ExamPaperRow = info.ExamPaperRow,
                    ExamPaperNodeRow = examPaperNodeRow,
                    ExamPaperNodeQuestionRowCollection = info.ExamPaperNodeQuestionRowCollection,
                    ExamQuestionRowCollection = info.ExamQuestionRowCollection,
                    UserAnswerDataTable = info.UserAnswerDataTable,
                    ExamGradeUid = info.ExamGradeUid,
                    IsMixOrder = info.IsMixOrder,
                    IsSingleAsMulti = isSingleAsMulti,
                    QuestionIndex = info.QuesionIndex,
                    IsAllowModifyObjectAnswer = info.IsAllowModifyObjectAnswer
                };
                string thisPaperNodeHtml = GetPaperQuestionViewForPaperNode(nodeViewBuilInfo);
                sbReturn.Append(thisPaperNodeHtml);
            }
            return sbReturn.ToString();
            #endregion
        }


        public string GetPaperQuestionViewForPaperNode(NodeViewBuildInfo info)
        {
            StringBuilder sbReturn = new StringBuilder();

            var thisPaperNodeUid = info.ExamPaperNodeRow.Id;
            ExamQuestionType examQuestionTypeRow = _iExamQuestionTypeRep.FirstOrDefault(info.ExamPaperNodeRow.questionTypeUid);

            string paperNodeName = info.ExamPaperNodeRow.paperNodeName;
            int nodeQuestionNum = info.ExamPaperNodeRow.questionNum;
            decimal nodeTotalScore = info.ExamPaperNodeRow.totalScore;
            decimal nodeQuestionScore = info.ExamPaperNodeRow.questionScore;
            bool isShowScore = true;
            isShowScore = info.ExamPaperRow.isShowScore == "Y" ? true : false;

            StringBuilder stringBuilder = new StringBuilder();
            //一大题的集合

            //大题标题
            string strTitle = "";
            strTitle = strTitle + "<div  class=\"tableheadtitle2\" onclick=\"jscomFlexObject(document.getElementById('divContentListForType" + thisPaperNodeUid + "'))\">";
            strTitle = strTitle + "<span  class=\"red\">" + StringUtil.NumberToBigNumber(info.PaperNodeNumber) + ".&nbsp;" + paperNodeName + "&nbsp;</span><span>";
            strTitle = strTitle + "（" + ("共") + nodeQuestionNum + ("题");
            if (isShowScore && nodeQuestionScore != 0 && 
                ((examQuestionTypeRow != null &&  examQuestionTypeRow.questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose) || info.ExamPaperNodeRow.Id == Guid.Empty)) //设定显示分数并大题分数不为0并大题类型不为组合题
            {
                strTitle = strTitle + "," + string.Format(("每题{0}分"), nodeQuestionScore.ToString("0.##"));
            }
            if (isShowScore && nodeTotalScore != 0)  //如果试卷设定不显示分数或分数为0时,不显示分数.
                strTitle = strTitle + "," + ("共") + nodeTotalScore.ToString("0.##") + ("分");
            strTitle = strTitle + "）";
            strTitle = strTitle + "</span>";
            strTitle += "<div class=\"buttonarea\"><span class='tagclose' title='点击这里可以展开或收缩本大题'></span></div>";
            strTitle = strTitle + "</div>";

            //开始一大题的试题内容的大表
            stringBuilder.Append("<div id='divContentListForType" + thisPaperNodeUid + "'>\n");
            //大题说明
            if (info.ExamPaperNodeRow.paperNodeDesc != "")
            {
                stringBuilder.Append("<table><tr><td>" + info.ExamPaperNodeRow.paperNodeDesc + "</td></tr></table>\n");
            }

            ExamPaperNodeQuestion examPaperNodeQuestionRow;
            ExamQuestionDto examQuestionRow;
            ExamAnswer examAnswerRow;
            //ExamExerciseAnswerRow examExerciseAnswerRow;

            //取得子集合
            List<ExamPaperNodeQuestion> subExamPaperNodeQuestionRowCollection = new List<ExamPaperNodeQuestion>();
            for (int i = 0; i < info.ExamPaperNodeQuestionRowCollection.Count; i++)
            {
                if (info.ExamPaperNodeQuestionRowCollection[i].paperNodeUid == thisPaperNodeUid) subExamPaperNodeQuestionRowCollection.Add(info.ExamPaperNodeQuestionRowCollection[i]);
            }

            if (subExamPaperNodeQuestionRowCollection.Count == 0)
            {
                return sbReturn.ToString();
            }

            var arrQuestionUid = new string[subExamPaperNodeQuestionRowCollection.Count];

            for (int i = 0; i < subExamPaperNodeQuestionRowCollection.Count; i++)
            {
                arrQuestionUid[i] = subExamPaperNodeQuestionRowCollection[i].questionUid.ToString();
            }

            int[] arrNewContentID = new int[arrQuestionUid.Length];

            //是否打乱显示顺序
            if (info.IsMixOrder == true && arrQuestionUid.Length > 1)
            {
                int[] oldIndex = new int[arrQuestionUid.Length];
                arrQuestionUid = ArrayUtil.GetMixedStringArrayOrder(arrQuestionUid, ref oldIndex);
                //整理编号
                if (info.QuestionIndex != null && info.QuestionIndex.Count > 0)
                {
                    for (int qn = 0; qn < arrQuestionUid.Length; qn++)
                    {
                        if (info.QuestionIndex.ContainsKey(arrQuestionUid[qn]))
                        {
                            info.QuestionIndex[arrQuestionUid[qn]] = (int)info.QuestionIndex[arrQuestionUid[qn]] + qn - oldIndex[qn];
                        }
                    }
                }
            }
            //int questionCount = 0;
            var questionIndex = 1;
            for (int i = 0; i < arrQuestionUid.Length; i++)
            {
                //取得题号
                var questionUid = Guid.Parse(arrQuestionUid[i]);

                //找到试题
                examQuestionRow = info.ExamQuestionRowCollection.FirstOrDefault(a => a.Id == questionUid);
                if (examQuestionRow == null) continue;
                //如果是子试题则略过,因为组合题里会把子试题加上去
                if (examQuestionRow.parentQuestionUid != Guid.Empty)
                    continue;

                examPaperNodeQuestionRow = info.ExamPaperNodeQuestionRowCollection.FirstOrDefault(a => a.paperNodeUid.Equals(thisPaperNodeUid) && a.questionUid == questionUid);

                //questionCount += 1;
                //if (info.QuestionIndex != null && info.QuestionIndex.Count > 0)
                //{
                //    if (info.QuestionIndex.ContainsKey(examQuestionRow.id.ToString()))
                //    {
                //        questionCount = (int)info.QuestionIndex[examQuestionRow.id.ToString()];
                //    }
                //    else
                //    {
                //        questionCount = -1;
                //    }
                //}

                examAnswerRow = null;
                if (info.UserAnswerDataTable != null && info.UserAnswerDataTable.Rows.Count > 0)
                {
                    info.UserAnswerDataTable.DefaultView.RowFilter = "questionUid='" + questionUid + "'";
                    if (info.UserAnswerDataTable.DefaultView.Count > 0)
                    {
                        var row = info.UserAnswerDataTable.DefaultView[0].Row;
                        examAnswerRow = AutoMapExtensions.ToEntity<ExamAnswer>(row);

                        // examAnswerRow.AssignByDataRow(dtUserAnswer.DefaultView[0].Row);
                    }
                }
                if (info.ViewType == EnumPaperViewType.Analyze || info.ViewType == EnumPaperViewType.ExamAnalyze || info.ViewType == EnumPaperViewType.ExerciseQuestionAnalyze)
                    stringBuilder.Append(GetPaperQuestionView(info.ViewType, examPaperNodeQuestionRow, examQuestionRow, examAnswerRow, questionIndex, info.IsMixOrder, info.IsSingleAsMulti, info.UserAnswerDataTable, info.ExamGradeUid, isShowScore, info.IsAllowModifyObjectAnswer));
                else
                    stringBuilder.Append(GetPaperQuestionView(info.ViewType, examPaperNodeQuestionRow, examQuestionRow, examAnswerRow, questionIndex, info.IsMixOrder, info.IsSingleAsMulti, null, info.ExamGradeUid, isShowScore, info.IsAllowModifyObjectAnswer));

                questionIndex++;
                #region 如果是组合题则还要显示子试题
                if (examQuestionRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Compose)
                {
                    var subExamQuestionRowCollection = info.ExamQuestionRowCollection.Where(a => a.parentQuestionUid == questionUid).OrderBy(a=>a.listOrder).ToList();

                    var arrSubQuestionUid = subExamQuestionRowCollection.Select(a=>a.Id.ToString()).ToArray();
                    
                    //是否打乱显示顺序
                    if (info.IsMixOrder && arrSubQuestionUid.Any())
                    {
                        arrSubQuestionUid = ArrayUtil.GetMixedStringArrayOrder(arrSubQuestionUid);
                    }
                    var indexDic = new Dictionary<Guid, int>();
                    for (int j = 0; j < arrSubQuestionUid.Length; j++)
                    {
                        var subQuestionUid = Guid.Parse(arrSubQuestionUid[j]);
                        //找到子试题
                        examQuestionRow = subExamQuestionRowCollection.FirstOrDefault(a => a.Id == subQuestionUid);
                        var childIndex = 0;
                        if (indexDic.ContainsKey(examQuestionRow.parentQuestionUid))
                        {
                            indexDic[examQuestionRow.parentQuestionUid]++;
                            childIndex = indexDic[examQuestionRow.parentQuestionUid];
                        }
                        else
                        {
                            indexDic.Add(examQuestionRow.parentQuestionUid, 0);
                        }
                        examPaperNodeQuestionRow = info.ExamPaperNodeQuestionRowCollection.FirstOrDefault(a => a.paperNodeUid == thisPaperNodeUid && a.questionUid == examQuestionRow.Id);
                        if (examPaperNodeQuestionRow == null)
                        {
                            continue;
                        }
                        examAnswerRow = null;
                        if (info.UserAnswerDataTable != null && info.UserAnswerDataTable.Rows.Count > 0)
                        {
                            info.UserAnswerDataTable.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(examQuestionRow.Id.ToString());
                            if (info.UserAnswerDataTable.DefaultView.Count > 0)
                            {
                                var row = info.UserAnswerDataTable.DefaultView[0].Row;
                                examAnswerRow = AutoMapExtensions.ToEntity<ExamAnswer>(row);
                            }
                        }
                       
                        stringBuilder.Append(GetPaperQuestionView(info.ViewType, examPaperNodeQuestionRow, examQuestionRow, examAnswerRow, childIndex + 1, info.IsMixOrder, info.IsSingleAsMulti, info.UserAnswerDataTable, info.ExamGradeUid, isShowScore, false));
                    }
                }
                #endregion
            }
            //每大题的未尾再加多空行
            stringBuilder.Append("</div>\n");

            sbReturn.Append(strTitle + stringBuilder);
            return sbReturn.ToString();
        }

        public string GetPaperQuestionView(string viewType, ExamPaperNodeQuestion examPaperNodeQuestionRow, 
            ExamQuestionDto examQuestionRow, ExamAnswer examAnswerRow, int Content_Index, bool isMixOrder, bool Single_As_Multi, 
            DataTable dtUserAnswer, Guid examGradeUid, bool isShowScore, bool isAllowModifyObjectAnswer)
        {
            StringBuilder stringBuilder = new StringBuilder();

            string questionBaseTypeCode;
            var paperUid = Guid.Empty;

            decimal score = examPaperNodeQuestionRow.paperQuestionScore;
            string selectAnswer;
            string selectScore;
            string standardAnswer;
            string content = "";
            var questionUid = Guid.Empty;
            var parentQuestionUid = Guid.Empty;
            string auestionAnalysis;	//试题提示
            string[] arrSelectAnswer;
            char[] spliter = "|".ToCharArray();
            string lowChar;
            string upChar;
            string[] arrSelectScore;
            string strChecked;
            int[] arrOriginalSelectAnswerIndex = new int[0];
            string answerBackgroundColor = "#ffffff";
            paperUid = examPaperNodeQuestionRow.paperUid;

            questionUid = examQuestionRow.Id;
            parentQuestionUid = examQuestionRow.parentQuestionUid;
            questionBaseTypeCode = examQuestionRow.questionBaseTypeCode;
            auestionAnalysis = examQuestionRow.questionAnalysis;
            auestionAnalysis = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", auestionAnalysis, true);
            selectAnswer = examQuestionRow.selectAnswer;
            selectAnswer = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", selectAnswer, true);
            selectScore = examQuestionRow.selectAnswerScore ?? "";
            selectScore = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", selectScore, true);
            standardAnswer = examQuestionRow.standardAnswer;
            standardAnswer = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", standardAnswer, true);
            content = examQuestionRow.questionText;

            string answerText = "";
            string answerTextScore = "";
            if (examAnswerRow != null)
            {
                answerText = examAnswerRow.answerText;
                answerTextScore = examAnswerRow.judgeScoreText;
            }
            //string examGradeUid = "";
            string judgeResultCode = "";
            decimal judge_score = 0;
            string judgeRemarks = "";
            string judgeUserName = string.Empty;
            if (examAnswerRow != null)
            {
                examGradeUid = examAnswerRow.examGradeUid;
                judgeResultCode = examAnswerRow.judgeResultCode;
                judge_score = (decimal)examAnswerRow.judgeScore;
                judgeRemarks = examAnswerRow.judgeRemarks;
                if (!string.IsNullOrEmpty(examAnswerRow.judgeUserName))
                {
                    judgeUserName = "  " + ("评卷人：") + examAnswerRow.judgeUserName;
                }
            }
            string strAnswer = "";
            Guid examUid = new Guid();
            string isFeedback = "";
            var examGrade = _iExamGradeRep.GetAll().FirstOrDefault(a=>a.Id == examGradeUid);
            if (examGrade != null)
                examUid = examGrade.examUid;
            var examExam = _iExamRep.GetAll().FirstOrDefault(a=>a.Id == examUid);
            if (examExam != null)
                isFeedback = examExam.allowFeedbook;
            #region 根据类型不同拼接html字符串
            switch (viewType)
            {
                case EnumPaperViewType.Exam:		//考试时用
                    strAnswer = GetQuestionView(examPaperNodeQuestionRow, examQuestionRow, false, isMixOrder, Single_As_Multi, Content_Index, answerText, true, ref arrOriginalSelectAnswerIndex, examGradeUid.ToString(), isShowScore, false, false, false);
                    stringBuilder.Append(strAnswer);
                    break;
                case EnumPaperViewType.Exercise:		//练习时用
                    strAnswer = GetQuestionView(examPaperNodeQuestionRow, examQuestionRow, false, isMixOrder, Single_As_Multi, Content_Index, answerText, true, ref arrOriginalSelectAnswerIndex, examGradeUid.ToString(), isShowScore, true, false, false);
                    stringBuilder.Append(strAnswer);
                    string sStandardAnswerText = GetQuestionAnswerText(questionBaseTypeCode, examQuestionRow.standardAnswer, arrOriginalSelectAnswerIndex);
                    sStandardAnswerText = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", sStandardAnswerText);//也要图片
                    if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                    {
                        stringBuilder.Append("<table>\r\n");
                        stringBuilder.Append("    <TR id=\"trStandardAnswer" + questionUid + "\" style=\"display:none\" runat=\"server\">\r\n");
                        stringBuilder.Append("        <TD style=\"HEIGHT: 20px;\" align=\"left\" height=\"20\">" + ("标准答案") + "：<span id=\"hidStandardAnswer" + questionUid + "\" style=\"WIDTH: 32px; HEIGHT: 16px;display:none\" size=\"1\" runat=\"server\">" + StringUtil.ReplaceEnter2BrWhenNoHtml(examQuestionRow.standardAnswer) + "</span><asp:label id=\"lblStandardAnswer\" runat=\"server\">" + sStandardAnswerText.Replace("\r\n", "<BR>") + "</asp:label><input id=\"hidStandardAnswer_" + questionUid + "\" name=\"hidStandardAnswer_" + questionUid + "\" type=\"hidden\" value=\"" + HttpUtility.HtmlEncode(examQuestionRow.standardAnswer) + "\" /></TD>\r\n");//HttpUtility.HtmlEncode(
                        stringBuilder.Append("    </TR>\r\n");
                        stringBuilder.Append("    <TR id=\"trAnswerResultOK" + questionUid + "\" style=\"display:none\" runat=\"server\">\r\n");
                        stringBuilder.Append("        <TD style=\"HEIGHT: 20px\" align=\"left\" height=\"20\">" + ("答题结果") + "：<font color=\"blue\">" + ("正确") + "！</font><span class=\"ImgAnswerRight\"></span></TD>\r\n");
                        stringBuilder.Append("    </TR>\r\n");
                        stringBuilder.Append("    <TR id=\"trAnswerResultNotOK" + questionUid + "\" style=\"display:none\" runat=\"server\">\r\n");
                        stringBuilder.Append("        <TD style=\"HEIGHT: 20px\" align=\"left\" height=\"20\">" + ("答题结果") + "：<font color=\"red\">" + ("错误") + "！</font><span class=\"ImgAnswerWrong\"></span></TD>\r\n");
                        stringBuilder.Append("    </TR>\r\n");
                        if (!string.IsNullOrEmpty(auestionAnalysis))
                        {
                            stringBuilder.Append("    <TR id=\"trAnalysis" + questionUid + "\" style=\"display:none\" runat=\"server\">\r\n");
                            stringBuilder.Append("        <TD style=\"HEIGHT: 20px\" align=\"left\" height=\"20\">" + ("试题解析") + "：<font color=\"red\">" + auestionAnalysis + "</font></TD>\r\n");
                            stringBuilder.Append("    </TR>\r\n");
                        }
                        stringBuilder.Append(" </table>\r\n");
                    }
                    break;
                case EnumPaperViewType.ViewExerciseAnswer:
                    strAnswer = GetQuestionView(examPaperNodeQuestionRow, examQuestionRow, false, isMixOrder, Single_As_Multi, Content_Index, answerText, true, ref arrOriginalSelectAnswerIndex, examGradeUid.ToString(), isShowScore, false, false, false);
                    stringBuilder.Append(strAnswer);
                    strAnswer = GetQuestionAnswerView(examQuestionRow, examAnswerRow, true, true, arrOriginalSelectAnswerIndex, examGradeUid);
                    stringBuilder.Append(strAnswer);
                    if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                    {
                        stringBuilder.Append("<table>\r\n");
                        stringBuilder.Append("    <TR>\r\n");
                        stringBuilder.Append("        <TD style=\"HEIGHT: 20px\" align=\"left\" height=\"20\">" + ("答题结果") + "：" + ((judgeResultCode == EnumJudgeResultCode.Right) ? "<font color=\"blue\">" + ("正确") + "！<span class=\"ImgAnswerRight\"></span></font>" : "<font color=\"red\">" + ("错误") + "！<span class=\"ImgAnswerWrong\"></span></font>") + "</TD>\r\n");
                        stringBuilder.Append("    </TR>\r\n");
                        if (!string.IsNullOrEmpty(auestionAnalysis))
                        {
                            stringBuilder.Append("    <TR>\r\n");
                            stringBuilder.Append("        <TD style=\"HEIGHT: 20px\" align=\"left\" height=\"20\">" + ("试题解析") + "：<font color=\"blue\">" + auestionAnalysis + "</font></TD>\r\n");
                            stringBuilder.Append("    </TR>\r\n");
                        }
                        stringBuilder.Append(" </table>\r\n");
                    }
                    break;
                case EnumPaperViewType.Preview:		//预览试卷时用
                    strAnswer = GetQuestionView(examPaperNodeQuestionRow, examQuestionRow, true, isMixOrder, Single_As_Multi, Content_Index, standardAnswer, true, ref arrOriginalSelectAnswerIndex, examGradeUid.ToString(), isShowScore, false, false, false);
                    stringBuilder.Append(strAnswer);
                    strAnswer = GetQuestionAnswerView(examQuestionRow, examAnswerRow, true, false, arrOriginalSelectAnswerIndex, examGradeUid);
                    stringBuilder.Append(strAnswer);
                    if (auestionAnalysis != "")
                        stringBuilder.Append("<table> <tr><td colspan='2'><font color='blue'>&nbsp;★" + ("答题分析") + "：" + auestionAnalysis + "</font></td></tr></table>\n");
                    if (!string.IsNullOrWhiteSpace(examQuestionRow.standardCode))
                    {
                        var standardCode = examQuestionRow.standardCode.Replace("<", "&lt;").Replace(">", "&gt;");
                        stringBuilder.Append("<table> <tr><td colspan='2'><font color='blue'>&nbsp;★" + ("参考代码") + "：</font><pre><code>" + standardCode + "</code></pre></td></tr></table>\n");
                    }
                    break;
                case EnumPaperViewType.Judge:		//评阅时用
                    strAnswer = GetQuestionView(examPaperNodeQuestionRow, examQuestionRow, false, isMixOrder, Single_As_Multi, Content_Index, answerText, true, ref arrOriginalSelectAnswerIndex, examGradeUid.ToString(), isShowScore, false, false, isAllowModifyObjectAnswer);
                    stringBuilder.Append(strAnswer);
                    strAnswer = GetQuestionAnswerView(examQuestionRow, examAnswerRow, true, true, arrOriginalSelectAnswerIndex, examGradeUid);
                    stringBuilder.Append(strAnswer);
                    if (auestionAnalysis != "")
                        stringBuilder.Append("<table> <tr><td colspan='2'><font color='blue'>&nbsp;★" + ("答题分析") + "：" + auestionAnalysis + "</font></td></tr></table>\n");
                    //评分
                    if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                    {
                        if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Answer || questionBaseTypeCode == EnumQuestionBaseTypeCode.Operate)
                        {
                            //得分点
                            if (!string.IsNullOrEmpty(selectAnswer) && !string.IsNullOrEmpty(selectScore))
                            {
                                arrSelectAnswer = selectAnswer.Split(spliter);
                                arrSelectScore = selectScore.Split(spliter);
                                if (isMixOrder == true)
                                {
                                    arrSelectAnswer = ArrayUtil.GetMixedStringArrayOrder(arrSelectAnswer, ref arrOriginalSelectAnswerIndex);
                                }
                                else
                                {
                                    arrOriginalSelectAnswerIndex = ArrayUtil.GetOrderValueIntArray(arrSelectAnswer.Length);
                                }
                                for (int j = 0; j < arrSelectAnswer.Length; j++)
                                {
                                    stringBuilder.Append("<table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr bgcolor='" + answerBackgroundColor + "'><td width='10px' height='30px' nowrap></td><td width='100%'>");
                                    lowChar = ((Char)(97 + j)).ToString();
                                    upChar = ((Char)(65 + j)).ToString();
                                    if (("|" + answerTextScore + "|").IndexOf("|" + QuestionUtil.AnswerNumbersToChars(arrOriginalSelectAnswerIndex[j].ToString()) + "|") > -1)
                                        strChecked = " checked ";
                                    else
                                        strChecked = "";
                                    stringBuilder.Append("<input type='checkbox' id='Answers" + questionUid + "' name='Answers" + questionUid + "' onclick=\"rad_OnClick('" + arrSelectScore[j].ToString() + "','" + questionUid + "','" + score.ToString() + "')\" title='" + arrSelectScore[j].ToString() + "' value='" + QuestionUtil.AnswerNumbersToChars(arrOriginalSelectAnswerIndex[j].ToString()) + "'" + strChecked + "><font color='blue'>" + QuestionUtil.AnswerNumbersToChars(arrOriginalSelectAnswerIndex[j].ToString()) + "&nbsp;&nbsp;" + arrSelectAnswer[j] + "(" + arrSelectScore[j] + "%" + ")</font>&nbsp;");
                                    stringBuilder.Append("<input type='hidden' id='hidden" + questionUid + j + "' name='hidden" + questionUid + j + "' value='" + QuestionUtil.AnswerNumbersToChars(arrOriginalSelectAnswerIndex[j].ToString()) + "'>");
                                    stringBuilder.Append("   </td>\n</tr>\n</table>\n");
                                }
                                if (isMixOrder == false)
                                {
                                    arrOriginalSelectAnswerIndex = null;
                                }
                            }
                        }
                        string sChecked = "";
                        stringBuilder.Append("<table width=\"100%\"> <tr><td><font color='blue'>" + ("评分结果") + "：");
                        if (judgeResultCode == EnumJudgeResultCode.Right)
                            sChecked = " checked ";
                        else
                            sChecked = "";
                        stringBuilder.Append("<input type='radio' id='radEvaluate" + questionUid + "' name='radEvaluate" + questionUid + "' onclick=\"radEvaluate_OnClick(0,'" + questionUid + "')\" value='" + score.ToString() + "'" + sChecked + "><font color='blue'>" + ("对") + "</font>&nbsp;");

                        decimal errorScore = 0;
                        if (examGradeUid != Guid.Empty)
                        {

                            ExamGrade examGradeRow = _iExamGradeRep.GetAll().FirstOrDefault(a=>a.Id == examGradeUid);
                            ExamExam examRow = null;
                            if (examGradeRow != null)
                                examRow = _iExamRep.GetAll().FirstOrDefault(a=>a.Id == examGradeRow.examUid);
                            if (examRow != null && examRow.isDeductScoreWhenError == "Y" && examRow.examDoModeCode == EnumExamDoModeCode.Question)
                            {
                                errorScore = 0 - score;
                            }
                        }
                        if (judgeResultCode == EnumJudgeResultCode.Error)
                            sChecked = " checked ";
                        else
                            sChecked = "";
                        stringBuilder.Append("<input type='radio' id='radEvaluate" + questionUid + "' name='radEvaluate" + questionUid + "' onclick=\"radEvaluate_OnClick(1,'" + questionUid + "')\" value='" + errorScore.ToString() + "'" + sChecked + "><font color='red'>" + ("错") + "</font>&nbsp;");
                        if (judgeResultCode == EnumJudgeResultCode.Middle)
                            sChecked = " checked ";
                        else
                            sChecked = "";
                        decimal Half_Score = score / 2;
                        stringBuilder.Append("<input type='radio' id='radEvaluate" + questionUid + "' name='radEvaluate" + questionUid + "' onclick=\"radEvaluate_OnClick(2,'" + questionUid + "')\" value='" + Half_Score.ToString() + "'" + sChecked + "><font color='#ffa500'>" + ("部分对") + "</font>&nbsp;");
                        
                        stringBuilder.Append("</font></td></tr>");

                        stringBuilder.Append("<tr><td><font color='blue'>");
                        stringBuilder.Append("" + ("得分") + "：<input type='text' id='txtJudgeScore" + questionUid + "' name='txtJudgeScore" + questionUid + "' class='commoninput' size='10' style='width:100px;' onblur='txtJudgeScore_onblur(this)' value='" + judge_score.ToString("0.##") + "'>&nbsp;");
                        stringBuilder.Append("</font></td></tr>");

                        stringBuilder.Append("<tr><td><font color='blue'>");
                        stringBuilder.Append("" + ("评语") + "：<input type='text' id='txtJudgeDesc" + questionUid + "' name='txtJudgeDesc" + questionUid + "' class='commoninput' size='40' style='width:50%;' value='" + judgeRemarks + "'>");
                        stringBuilder.Append("</font></td></tr></table>\n");
                    }
                    break;
                case EnumPaperViewType.ViewAnswer:		//查看考生答卷时用
                    strAnswer = GetQuestionView(examPaperNodeQuestionRow, examQuestionRow, false, isMixOrder, Single_As_Multi, Content_Index, answerText, false, ref arrOriginalSelectAnswerIndex, examGradeUid.ToString(), isShowScore, false, false, false);
                    stringBuilder.Append(strAnswer);
                    strAnswer = GetQuestionAnswerView(examQuestionRow, examAnswerRow, true, true, arrOriginalSelectAnswerIndex, examGradeUid);
                    stringBuilder.Append(strAnswer);
                    //显示分数
                    string sColor = "Blue";
                    if (judgeResultCode != EnumJudgeResultCode.Right)	//有错
                        sColor = "red";
                    if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                    {
                        stringBuilder.Append("<table>");
                        if (examGradeUid != Guid.Empty)
                        {

                            if (isFeedback == "Y")
                            {
                                stringBuilder.Append(" <tr><td colspan='2' class=\"SearchButton\"><a  href=\"javascript:void(0)\" class=\"button_btg\" onclick=\"javascript:SubmitQuestionFeedback('" + examGradeUid + "','" + examQuestionRow.Id + "')\" >" + ("试题反馈") + "</a></td></tr>\n");
                            }
                        }
                        if (parentQuestionUid == Guid.Empty)
                        {
                            stringBuilder.Append(" <tr><td colspan='2'><font color='" + sColor + "'>&nbsp;★" + ("考生得分") + "：" + judge_score.ToString("0.##") + "&nbsp;" + ("分") + "&nbsp;&nbsp;" + ("评语") + "：" + judgeRemarks + judgeUserName + "</font></td></tr>\n");
                            if (auestionAnalysis != "")
                                stringBuilder.Append(" <tr><td colspan='2'><font color='blue'>&nbsp;★" + ("答题分析") + "：" + auestionAnalysis + "</font></td></tr>\n");

                        }
                        else
                        {
                            stringBuilder.Append(" <tr><td colspan='2'><font color='" + sColor + "'>&nbsp;&nbsp;★" + ("考生得分") + "：" + judge_score.ToString("0.##") + "&nbsp;" + ("分") + "&nbsp;&nbsp;" + ("评语") + "：" + judgeRemarks + judgeUserName + "</font></td></tr>\n");
                            if (auestionAnalysis != "")
                                stringBuilder.Append(" <tr><td colspan='2'><font color='blue'>&nbsp;&nbsp;★" + ("答题分析") + "：" + auestionAnalysis + "</font></td></tr>\n");
                        }
                        stringBuilder.Append("</table>");
                    }
                    break;
                case EnumPaperViewType.ViewAnswerNoGrade:		//查看考生答卷(无成绩)
                    strAnswer = GetQuestionView(examPaperNodeQuestionRow, examQuestionRow, false, isMixOrder, Single_As_Multi, Content_Index, answerText, false, ref arrOriginalSelectAnswerIndex, examGradeUid.ToString(), isShowScore, false, false, false);
                    stringBuilder.Append(strAnswer);
                    strAnswer = GetQuestionAnswerView(examQuestionRow, examAnswerRow, true, true, arrOriginalSelectAnswerIndex, examGradeUid);
                    stringBuilder.Append(strAnswer);

                    if (examGradeUid != Guid.Empty)
                    {
                        stringBuilder.Append("<table>");

                        if (isFeedback == "Y")
                        {
                            stringBuilder.Append(" <tr><td colspan='2' class=\"SearchButton\"><a  href=\"javascript:void(0)\" class=\"button_btg\" onclick=\"javascript:SubmitQuestionFeedback('" + examGradeUid + "','" + examQuestionRow.Id + "')\" >" + ("试题反馈") + "</a></td></tr>\n");
                        }
                        stringBuilder.Append("</table>");
                    }
                    break;
                case EnumPaperViewType.ViewUserAnswer:  //考生查看自己答案时用,不显示标准答案,只显示考生答案,------如果设置了显示分数，则显示考试得分及评语
                    strAnswer = GetQuestionView(examPaperNodeQuestionRow, examQuestionRow, false, isMixOrder, Single_As_Multi, Content_Index, answerText, false, ref arrOriginalSelectAnswerIndex, examGradeUid.ToString(), isShowScore, false, false, false);
                    stringBuilder.Append(strAnswer);
                    strAnswer = GetQuestionAnswerView(examQuestionRow, examAnswerRow, false, true, arrOriginalSelectAnswerIndex, examGradeUid);
                    stringBuilder.Append(strAnswer);
                    if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                    {
                        if (examGradeUid != Guid.Empty)
                        {
                            stringBuilder.Append("<table>");

                            if (isFeedback == "Y")
                            {
                                stringBuilder.Append(" <tr><td colspan='2' class=\"SearchButton\"><a  href=\"javascript:void(0)\" class=\"button_btg\" onclick=\"javascript:SubmitQuestionFeedback('" + examGradeUid + "','" + examQuestionRow.Id + "')\" >" + ("试题反馈") + "</a></td></tr>\n");
                            }
                            stringBuilder.Append("</table>");
                        }
                    }
                    break;
                case EnumPaperViewType.ViewUserAnswerWithAnswer:  //考生查看自己答案时用,不显示标准答案,只显示考生答案,考试得分及评语
                    strAnswer = GetQuestionView(examPaperNodeQuestionRow, examQuestionRow, false, isMixOrder, Single_As_Multi, Content_Index, answerText, false, ref arrOriginalSelectAnswerIndex, examGradeUid.ToString(), isShowScore, false, false, false);
                    stringBuilder.Append(strAnswer);
                    strAnswer = GetQuestionAnswerView(examQuestionRow, examAnswerRow, true, true, arrOriginalSelectAnswerIndex, examGradeUid);
                    stringBuilder.Append(strAnswer);
                    //显示分数
                    string strColor = "Blue";
                    if (judgeResultCode != EnumJudgeResultCode.Right)	//有错
                        strColor = "red";
                    if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                    {
                        stringBuilder.Append("<table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
                        if (parentQuestionUid == Guid.Empty)
                        {
                            stringBuilder.Append(" <tr><td colspan='2'><font color='" + strColor + "'>&nbsp;★" + ("考生得分") + "：" + judge_score.ToString("0.##") + "&nbsp;" + ("分") + "&nbsp;&nbsp;" + ("评语") + "：" + judgeRemarks + judgeUserName + "</font></td></tr>\n");
                            if (auestionAnalysis != "")
                                stringBuilder.Append(" <tr><td colspan='2'><font color='blue'>&nbsp;★" + ("答题分析") + "：" + auestionAnalysis + "</font></td></tr>\n");

                        }
                        else
                        {
                            stringBuilder.Append(" <tr><td colspan='2'><font color='" + strColor + "'>&nbsp;&nbsp;★" + ("考生得分") + "：" + judge_score.ToString("0.##") + "&nbsp;" + ("分") + "&nbsp;&nbsp;" + ("评语") + "：" + judgeRemarks + judgeUserName + "</font></td></tr>\n");
                            if (auestionAnalysis != "")
                                stringBuilder.Append(" <tr><td colspan='2'><font color='blue'>&nbsp;&nbsp;★" + ("答题分析") + "：" + auestionAnalysis + "</font></td></tr>\n");
                        }
                        if (examGradeUid != Guid.Empty)
                        {

                            if (isFeedback == "Y")
                            {
                                stringBuilder.Append(" <tr><td colspan='2' class=\"SearchButton\"><a  href=\"javascript:void(0)\" class=\"button_btg\"  onclick=\"javascript:SubmitQuestionFeedback('" + examGradeUid + "','" + examQuestionRow.Id + "')\">" + ("试题反馈") + "</a></td></tr>\n");
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(examQuestionRow.standardCode))
                        {
                            var standardCode = examQuestionRow.standardCode.Replace("<", "&lt;").Replace(">", "&gt;");
                            stringBuilder.Append(" <tr><td colspan='2'><font color='blue'>&nbsp;★" + ("参考代码") + "：</font><pre><code'>" + standardCode + "</code></pre></td></tr>\n");
                        }
                        stringBuilder.Append("</table>");
                    }
                    break;
                case EnumPaperViewType.Analyze:       //按试题分析时用
                    //stringBuilder.Append(strAnswer);
                    strAnswer = GetContentAnswerAnalyzeView(paperUid.ToString(), Content_Index, questionUid.ToString(), questionBaseTypeCode, content, Convert.ToSingle(score), selectAnswer, standardAnswer, dtUserAnswer);

                    stringBuilder.Append(strAnswer);
                    break;
                case EnumPaperViewType.ExamAnalyze:       //试卷分析时用
                    //stringBuilder.Append(strAnswer);
                    strAnswer = ExamAnalyzeView(paperUid.ToString(), Content_Index, questionUid.ToString(), questionBaseTypeCode, content, Convert.ToSingle(score), selectAnswer, standardAnswer, dtUserAnswer);
                    // strAnswer = GetContentAnswerAnalyzeView(paperUid, Content_Index, questionUid, questionBaseTypeCode, Content, Convert.ToSingle(Score), Select_Answer, Standard_Answer, dtUserAnswer);

                    stringBuilder.Append(strAnswer);
                    break;
                case EnumPaperViewType.ExerciseQuestionAnalyze:       //练习统计时用
                    //stringBuilder.Append(strAnswer);
                    strAnswer = GetContentAnswerAnalyzeView(paperUid.ToString(), Content_Index, questionUid.ToString(), questionBaseTypeCode, content, Convert.ToSingle(score), selectAnswer, standardAnswer, dtUserAnswer);
                    stringBuilder.Append(strAnswer);
                    break;
            }
            #endregion
            return stringBuilder.ToString();
        }

        public string GetQuestionView(ExamPaperNodeQuestion examPaperNodeQuestionRow, ExamQuestionDto examQuestionRow, 
                bool isPreview, bool isMixOrder,
                 bool isSingleAsMulti, int contentIndex,
                 string userAnswer, bool isShowInputBox,
                 ref int[] arrOriginalSelectAnswerIndex,
                 string examGradeUid, bool isShowScore, 
                 bool isShowCheckButton, bool isDisabledObjctQuestionButton,
                bool isAllowModifyObjectAnswer)
        {

            string questionUid = examQuestionRow.Id.ToString();
            var parentQuestionUid = examQuestionRow.parentQuestionUid;
            string questionBaseTypeCode = examQuestionRow.questionBaseTypeCode;
            string content = examQuestionRow.questionText;
            decimal score = examPaperNodeQuestionRow.paperQuestionScore;
            string selectAnswer = examQuestionRow.selectAnswer;
            string selectScore = examQuestionRow.selectAnswerScore ?? "";
            int questionExamTime = examPaperNodeQuestionRow.paperQuestionExamTime;
            string officeMode = examQuestionRow.operateTypeCode;

            if (userAnswer == null) userAnswer = "";
            StringBuilder stringBuilder = new StringBuilder();

            if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Fill)
            {
                //如果不是null,则把填空题的空换成文本框
                if (!string.IsNullOrEmpty(userAnswer))
                {
                    content = QuestionUtil.ReplaceFillInContentExercise(content, questionUid, userAnswer);
                }
            }

            //现固定从题库中取图片之类路径
            content = FilePathUtil.GetContentTextWithFilePath(questionUid, "question", content, false);
            content = GetFilePath(content);
            //现固定从题库中取图片之类路径
            selectAnswer = FilePathUtil.GetContentTextWithFilePath(questionUid, "question", selectAnswer, false);

            //把回车换行替换成<br>
            if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Typing)
            {
                //如果是打字题,处理Html标签.回车换行在换成空格.  Rick 20060830
                content = content.Replace("<", "");
                content = content.Replace(">", "");
                content = StringUtil.ReplaceEnter2BrWhenNoHtml(content);

                selectAnswer = selectAnswer.Replace("<", "");
                selectAnswer = selectAnswer.Replace(">", "");
                selectAnswer = StringUtil.ReplaceEnter2BrWhenNoHtml(selectAnswer);
            }
            else
            {
                content = StringUtil.ReplaceSpaces2Html(content);
                selectAnswer = StringUtil.ReplaceSpaces2Html(selectAnswer);
            }

            //开始写试题内容
            stringBuilder.Append("<div class=\"tableheadtitle\">");
            //试题号
            string sContentIndex = contentIndex.ToString();
            if (sContentIndex == "-1" || sContentIndex == "0")
            {
                sContentIndex = "";
            }
            else
            {
                if (parentQuestionUid == Guid.Empty)//用于处理组合题子试题
                {
                    sContentIndex = sContentIndex + ".";
                }
                else
                {
                    sContentIndex = "(" + contentIndex.ToString() + ")";
                }
            }
            stringBuilder.Append("<input type='hidden' id='hidQuestionUid' name='hidQuestionUid' value='" + questionUid + "'/>");
            stringBuilder.Append("<input type='hidden' id='hidQuestionBaseTypeCode" + questionUid + "' name='hidQuestionBaseTypeCode" + questionUid + "' value='" + questionBaseTypeCode.ToString() + "'/>");

            if (parentQuestionUid == Guid.Empty)//用于处理组合题子试题
            {
                stringBuilder.Append("<span class=\"red\">" + sContentIndex + "&nbsp;</span>");
            }
            else
            {
                stringBuilder.Append("<span class=\"red\">&nbsp;" + sContentIndex + "&nbsp;</span>");
            }

            //如果不是打字题或不显示输入框时都要显示题目
            if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Typing || isShowInputBox == false)
                stringBuilder.Append(content);

            if (isShowScore && score > 0) stringBuilder.Append("&nbsp;（" + score.ToString("0.##") + ("分") + "）");
            if (questionExamTime > 0) stringBuilder.Append("&nbsp;（" + ("答题时限") + "：" + SPOC.Exam.DateTimeUtil.ToTimeStrFromSecond(questionExamTime) + "）");

            stringBuilder.Append("</div>");
            string operation = examQuestionRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Operate ? "exam_operation" + "_opt" : "exam_operation";
            stringBuilder.Append("<table class='" + operation + "'  hidId='" + examQuestionRow.Id + "' id='" + examQuestionRow.Id + "' >\n");

            //显示可选答案
            string[] arrSelectAnswer;
            char[] spliter = "|".ToCharArray();
            string lowChar;
            string upChar;
            string[] arrSelectScore;
            string strChecked;

            //int[] arrOriginalSelectAnswerIndex=new int[0];
            arrOriginalSelectAnswerIndex = null;
            switch (questionBaseTypeCode)
            {
                case EnumQuestionBaseTypeCode.Single:
                case EnumQuestionBaseTypeCode.EvaluationSingle:
                    arrSelectAnswer = selectAnswer.Split(spliter);
                    if (isMixOrder == true)
                    {
                        arrSelectAnswer = ArrayUtil.GetMixedStringArrayOrder(arrSelectAnswer, ref arrOriginalSelectAnswerIndex);
                    }
                    else
                    {
                        arrOriginalSelectAnswerIndex = ArrayUtil.GetOrderValueIntArray(arrSelectAnswer.Length);
                    }
                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        var reg1 = new Regex("^<p>");
                        var reg2 = new Regex("</p>$");
                        var selectAnswerItem = reg1.Replace(arrSelectAnswer[j], "");
                        selectAnswerItem = reg2.Replace(selectAnswerItem, "", 1);
                        stringBuilder.Append("<tr><td><span>");
                        lowChar = ((Char)(97 + j)).ToString();
                        upChar = ((Char)(65 + j)).ToString();
                        if (("|" + userAnswer + "|").IndexOf("|" + QuestionUtil.AnswerNumbersToChars(arrOriginalSelectAnswerIndex[j].ToString()) + "|") > -1)
                            strChecked = " checked ";
                        else
                            strChecked = "";
                        stringBuilder.Append("<input " + (isDisabledObjctQuestionButton ? "disabled" : "") + " type='" + (isSingleAsMulti ? "checkbox" : "radio") + "' id='Answer" + questionUid + "' name='Answer" + questionUid + "' value='" + QuestionUtil.AnswerNumbersToChars(arrOriginalSelectAnswerIndex[j].ToString()) + "'" + strChecked + " onclick=\"jscomCheckedInputQuestionAnswer('Answer" + questionUid + "'," + j + ")\"  onfocus='try{Controller_onfocus(this)}catch(e){}'>" + upChar + ".<span onclick=\"jscomCheckedQuestionAnswer('Answer" + questionUid + "'," + j + ")\">" + selectAnswerItem + "</span>");
                        stringBuilder.Append("   </span></td>\n</tr>\n");
                    }
                    //如果不打乱顺序，则返回的顺序列表为空，这样可以避免多余操作
                    if (isMixOrder == false)
                    {
                        arrOriginalSelectAnswerIndex = null;
                    }

                    break;
                case EnumQuestionBaseTypeCode.Multi:
                case EnumQuestionBaseTypeCode.EvaluationMulti:
                    arrSelectAnswer = selectAnswer.Split(spliter);

                    if (isMixOrder == true)
                    {
                        arrSelectAnswer = ArrayUtil.GetMixedStringArrayOrder(arrSelectAnswer, ref arrOriginalSelectAnswerIndex);
                    }
                    else
                    {
                        arrOriginalSelectAnswerIndex = ArrayUtil.GetOrderValueIntArray(arrSelectAnswer.Length);
                    }

                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        var reg1 = new Regex("^<p>");
                        var reg2 = new Regex("</p>$");
                        var selectAnswerItem = reg1.Replace(arrSelectAnswer[j], "");
                        selectAnswerItem = reg2.Replace(selectAnswerItem, "", 1);
                        stringBuilder.Append("<tr><td><span>");
                        lowChar = ((Char)(97 + j)).ToString();
                        upChar = ((Char)(65 + j)).ToString();
                        //例如：  |2|3|  与|3|
                        if (("|" + userAnswer + "|").IndexOf("|" + QuestionUtil.AnswerNumbersToChars(arrOriginalSelectAnswerIndex[j].ToString()) + "|") > -1)
                            strChecked = " checked ";
                        else
                            strChecked = "";
                        stringBuilder.Append("<input " + ((isDisabledObjctQuestionButton == true) ? "disabled" : "") + " type='checkbox' id='Answer" + questionUid + "' name='Answer" + questionUid + "' value='" + QuestionUtil.AnswerNumbersToChars(arrOriginalSelectAnswerIndex[j].ToString()) + "'" + strChecked + " onclick=\"jscomCheckedInputQuestionAnswer('Answer" + questionUid + "'," + j + ")\"  onfocus='try{Controller_onfocus(this)}catch(e){}'>" + upChar + ".<span onclick=\"jscomCheckedQuestionAnswer('Answer" + questionUid + "'," + j + ")\">" + selectAnswerItem + "</span>");
                        stringBuilder.Append("   </span></td>\n</tr>\n");
                    }
                    //stringBuilder.Append("</table>");
                    //如果不打乱顺序，则返回的顺序列表为空，这样可以避免多余操作
                    if (isMixOrder == false)
                    {
                        arrOriginalSelectAnswerIndex = null;
                    }
                    break;

                case EnumQuestionBaseTypeCode.Judge:

                    arrSelectAnswer = selectAnswer.Split(spliter);

                    if (arrSelectAnswer.Length != 2)
                    {
                        arrSelectAnswer = new string[] { ("错误"), ("正确") };
                    }
                    string[] arrSelectAnswerValue = new string[] { "N", "Y" };

                    //判断题不打乱顺序
                    arrOriginalSelectAnswerIndex = ArrayUtil.GetOrderValueIntArray(arrSelectAnswer.Length);

                    for (int j = 0; j < 2; j++)
                    {
                        if (userAnswer == arrSelectAnswerValue[j])
                            strChecked = " checked ";
                        else
                            strChecked = "";
                        stringBuilder.Append("<tr><td><span>");
                        stringBuilder.Append("<input " + ((isDisabledObjctQuestionButton == true) ? "disabled" : "") + " type='radio' id='Answer" + questionUid + "' name='Answer" + questionUid + "' value='" + arrSelectAnswerValue[j] + "'" + strChecked + " onclick=\"jscomCheckedInputQuestionAnswer('Answer" + questionUid + "'," + j.ToString() + ")\"  onfocus='try{Controller_onfocus(this)}catch(e){}'>" + "<span onclick=\"jscomCheckedQuestionAnswer('Answer" + questionUid + "'," + j.ToString() + ")\">" + arrSelectAnswer[j] + "</span>");
                        stringBuilder.Append("   </span></td>\n</tr>\n");
                    }

                    //stringBuilder.Append("</table>");
                    //如果不打乱顺序，则返回的顺序列表为空，这样可以避免多余操作
                    if (isMixOrder == false)
                    {
                        arrOriginalSelectAnswerIndex = null;
                    }
                    break;
                case EnumQuestionBaseTypeCode.JudgeCorrect:
                    stringBuilder.Append("<tr><td><span>");

                    //正确
                    if (userAnswer == "Y")
                        strChecked = " checked ";
                    else
                        strChecked = "";
                    stringBuilder.Append("<input type='radio' id='JudgeCorrect" + questionUid + "' name='JudgeCorrect" + questionUid + "' value='Y'" + strChecked + " onclick=\"JudgeCorrectRightInput('" + questionUid + "')\"  onfocus='try{Controller_onfocus(this)}catch(e){}'>" + "<span onclick=\"document.getElementById('Answer" + questionUid + "').value='';document.getElementById('trAnswer" + questionUid + "').style.display='none';jscomCheckedQuestionAnswer('JudgeCorrect" + questionUid + "',0)\">" + ("正确") + "</span>");
                    //错误
                    if (string.IsNullOrEmpty(userAnswer) || userAnswer == "Y")
                        strChecked = "";
                    else
                        strChecked = " checked ";
                    stringBuilder.Append("<input type='radio' id='JudgeCorrect" + questionUid + "' name='JudgeCorrect" + questionUid + "' value='N'" + strChecked + " onclick=\"JudgeCorrectErrorInput('" + questionUid + "')\"  onfocus='try{Controller_onfocus(this)}catch(e){}'>" + "<span onclick=\"jscomCheckedQuestionAnswer('JudgeCorrect" + questionUid + "',1);document.getElementById('trAnswer" + questionUid + "').style.display='block';\">" + ("错误") + "</span>");

                    stringBuilder.Append("   <span></td>\n</tr>\n");
                    //增加改错文本框
                    if (isShowInputBox)
                    {
                        stringBuilder.Append("<tr id='trAnswer" + questionUid + "'><td><span style='width:100%'>");
                        stringBuilder.Append("<textarea id='Answer" + questionUid + "' name='Answer" + questionUid + "' rows='6' cols='40' style='width:100%'  class='commontextarea' name='Answer" + questionUid + "'></textarea>");
                        stringBuilder.Append("   </span></td>\n</tr>\n");
                    }
                    break;
                case EnumQuestionBaseTypeCode.ProgramFill:
                case EnumQuestionBaseTypeCode.Program:
                    if (!string.IsNullOrEmpty(examQuestionRow.PreinstallCode))
                    {
                        var preinstallCode = examQuestionRow.PreinstallCode
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;");
                        stringBuilder.Append("<tr><td>");
                        stringBuilder.Append("<font color='blue'>&nbsp;★预设代码：</font>");
                        stringBuilder.Append("</td></tr>");
                        stringBuilder.Append("<tr><td><pre><code>");
                        stringBuilder.Append(preinstallCode);
                        stringBuilder.Append("</code></pre></td></tr>");


                    }
                    break;
                case EnumQuestionBaseTypeCode.Fill:
                case EnumQuestionBaseTypeCode.Compose:
                    break;
                case EnumQuestionBaseTypeCode.Operate:	//新加入类型为操作题 目的是要显示office操作钮

                    //选用office工具(Word、Excel、PowerPoint)操作
                    if (examGradeUid != "")
                    {
                        stringBuilder.Append("<tr><td class=\"SearchButton\">");
                        stringBuilder.Append("<a href=\"javascript:void(0)\" onclick=\"OpenContractTextFile('" + examGradeUid.ToString() + "','" + questionUid + "','" + examQuestionRow.operateTypeCode + "')\" id='btnUsingOffice" + questionUid + "' class='button_btgOperate' title='" + ("开始操作") + "'>" + ("开始操作") + "</a>");
                        stringBuilder.Append("<input type='text' id='Answer" + questionUid + "' name='Answer" + questionUid + "' style='display:none' value='" + userAnswer.Replace("'", "") + "'>");
                        stringBuilder.Append(" </td>\n</tr>\n");
                    }
                    // stringBuilder.Append("</table>");
                    break;
                case EnumQuestionBaseTypeCode.Voice:	//语音题
                    if (examGradeUid != "" && isShowInputBox)
                    {
                        //评卷时，将语音题作答功能隐藏掉 Lopping 2012-09-25
                        stringBuilder.Append("<tr style=\"display:none\"><td class=\"SearchButton\">");
                        stringBuilder.Append("<a href=\"javascript:void(0)\" onclick=\"OpenContractVoiceFile('" + examGradeUid.ToString() + "','" + questionUid + "')\" id='btnUsingVoice" + questionUid + "' class='button_btgVoice' title='" + ("开始作答") + "'>" + ("开始作答") + "</a>");
                        stringBuilder.Append("<input type='text' id='Answer" + questionUid + "' name='Answer" + questionUid + "' style='display:none' value='" + userAnswer.Replace("'", "") + "'>");
                        stringBuilder.Append(" </td>\n</tr>\n");
                    }
                    break;
                case EnumQuestionBaseTypeCode.Typing:
                    //如果打字题在显示输入框的条件下(也就是考试或预览时)
                    if (examGradeUid != "")
                    {
                        if (isShowInputBox == true)
                        {
                            stringBuilder.Append("<tr><td class=\"SearchButton\">");
                            //下面改了，加了个 ' 在TypingOpenWindow( 后面
                            stringBuilder.Append("<a href=\"javascript:void(0)\" onclick=\"TypingOpenWindow('" + examGradeUid.ToString() + "','" + questionUid + "'," + questionExamTime.ToString() + ",'" + content.Replace("'", "") + "')\"  id='btnBeginTyping" + questionUid + "' name='btnBeginTyping" + questionUid + "'" + ((userAnswer == "") ? "" : " disabled ") + " class='button_btgTyping' title='" + ("开始打字") + "'>" + ("开始打字") + "</a>");
                            stringBuilder.Append("<input type='text' id='Answer" + questionUid + "' name='Answer" + questionUid + "' style='display:none' value='" + userAnswer.Replace("'", "") + "'>");
                            stringBuilder.Append("</td>\n</tr>\n");
                        }
                    }
                    break;
                default:
                    if (isShowInputBox == true)
                    {
                        userAnswer = QuestionUtil.DecodeSpliter(userAnswer);
                        //=========== add by nick 为增超文本答题功能==============
                        if (examQuestionRow.isAnswerByHtml == "Y")		//使用超文本答题功能
                        {
                            string rootPath = AppConfiguration.WebServerFileRootPath;
                            if (!Directory.Exists(rootPath))
                                Directory.CreateDirectory(rootPath);
                            rootPath = rootPath + "/";
                            string examUid = "";
                            var examGradeId = Guid.Parse(examGradeUid);
                            var examGrade = _iExamGradeRep.GetAll().FirstOrDefault(a=>a.Id == examGradeId);
                            if (examGrade != null)
                                examUid = examGrade.examUid.ToString();
                            string fileRootPath = FilePathUtil.GetOppositeFileWebPathRoot("", "ExamAnswer");
                            fileRootPath += "Exam_" + examUid + "/" + "ExamGrade_" + examGradeUid + "/" + examQuestionRow.Id;
                            if (examGradeUid != "")
                                Directory.CreateDirectory(rootPath + fileRootPath);

                            //string New_User_Answer = FilePathUtil.GetContentTextWithFilePath(examGradeUid, "ExamAnswer", User_Answer, true);
                            string newUserAnswer = FilePathUtil.GetContentTextWithFilePath("Exam_" + examUid + "\\" + "ExamGrade_" + examGradeUid + "\\" + questionUid, "ExamAnswer", userAnswer, true);
                            stringBuilder.Append("<tr><td>");
                            stringBuilder.Append("<textarea rows=6 cols=40 id='Answer" + questionUid + "' name='Answer" + questionUid + "' onmouseout=\"jscomCheckedTextareaQuestionAnswer('Answer" + questionUid + "')\"  class='commontextarea' onfocus='try{Controller_onfocus(this)}catch(e){}'>" + newUserAnswer + "</textarea><span class='htmledit' onclick=\"javascript:OpenHtmlEditor('Answer" + questionUid + "','" + fileRootPath + "')\"></span>");
                            stringBuilder.Append("   </td>\n</tr>\n");
                        }
                        else
                        //=======================================================
                        {
                            stringBuilder.Append("<tr><td>");
                            stringBuilder.Append("<textarea rows=6 cols=40 id='Answer" + questionUid + "' name='Answer" + questionUid + "' onblur=\"jscomCheckedTextareaQuestionAnswer('Answer" + questionUid + "')\" class='commontextarea' onfocus='try{Controller_onfocus(this)}catch(e){}'>" + userAnswer + "</textarea>");
                            stringBuilder.Append("   </td>\n</tr>\n");
                        }
                        // stringBuilder.Append("</table>");
                    }
                    break;
            }

            if (isShowCheckButton && questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                stringBuilder.Append("<tr><td class=\"SearchButton\"><a  href=\"javascript:CheckUserAnswer('" + examQuestionRow.Id + "')\" class='button_btg' title='" + ("检查答案") + "'>★" + ("检查答案") + "</a></td></tr>\r\n");
            stringBuilder.Append("</table>");


            return stringBuilder.ToString();

        }


        /// <summary>
        /// 得到答案的显示形式
        /// </summary>
        #region GetQuestionAnswerText
        public string GetQuestionAnswerText(string questionBaseTypeCode, string answer, int[] arrOriginalSelectAnswerIndex)
        {
            char[] spliter = "|".ToCharArray();

            string sAnswerText = "";
            switch (questionBaseTypeCode)
            {
                case EnumQuestionBaseTypeCode.Single:
                    sAnswerText = QuestionUtil.GetSelectAnswerView(answer, arrOriginalSelectAnswerIndex);
                    break;
                case EnumQuestionBaseTypeCode.Multi:
                    sAnswerText = QuestionUtil.GetSelectAnswerView(answer, arrOriginalSelectAnswerIndex);
                    break;
                case EnumQuestionBaseTypeCode.EvaluationMulti:
                case EnumQuestionBaseTypeCode.EvaluationSingle:
                    sAnswerText = answer;
                    break;
                case EnumQuestionBaseTypeCode.Judge:
                    if (answer == "Y")
                        sAnswerText = ("正确");
                    else if (answer == "N")
                        sAnswerText = ("错误");
                    else
                        sAnswerText = "";
                    break;
                case EnumQuestionBaseTypeCode.JudgeCorrect:
                    if (answer == "Y")
                        sAnswerText = ("正确");
                    else if (string.IsNullOrEmpty(answer))
                        sAnswerText = "";
                    else
                        sAnswerText = ("错误") + ";" + ("改正:") + answer;
                    break;
                case EnumQuestionBaseTypeCode.Fill:
                    sAnswerText = "";
                    string[] arrAnswer = answer.Split(spliter);
                    for (int j = 0; j < arrAnswer.Length; j++)
                    {
                        sAnswerText = sAnswerText + (j + 1).ToString() + ". " + QuestionUtil.DecodeSpliter(arrAnswer[j]) + ";&nbsp;";
                    }
                    break;
                case EnumQuestionBaseTypeCode.Compose:
                    sAnswerText = "none";
                    break;
                default:

                    sAnswerText = answer;
                    break;
            }
            return sAnswerText;
        }
        #endregion

        /// <summary>
        /// 显示试题的答案部分
        /// </summary>
        /// <param name="examAnswer"></param>
        /// <param name="isShowStandardAnswer"></param>
        /// <param name="isShowUserAnswer"></param>
        /// <param name="examQuestionRow"></param>
        /// <param name="arrOriginalSelectAnswerIndex"></param>
        /// <param name="examGradeUid"></param>
        /// <returns></returns>

        #region GetQuestionAnswerView
        public string GetQuestionAnswerView(ExamQuestionDto examQuestionRow, ExamAnswer examAnswer, bool isShowStandardAnswer,
            bool isShowUserAnswer, int[] arrOriginalSelectAnswerIndex, Guid examGradeUid)
        {
            var questionUid = examQuestionRow.Id;
            var parentQuestionUid = examQuestionRow.parentQuestionUid;
            string questionBaseTypeCode = examQuestionRow.questionBaseTypeCode;
            string operateTypeCode = examQuestionRow.operateTypeCode;
            string standardAnswer = examQuestionRow.standardAnswer;
            string isAnswerByHtml = examQuestionRow.isAnswerByHtml;

            string userAnswer = "";
            int examTime = 0;

            if (examAnswer != null)
            {
                examGradeUid = examAnswer.examGradeUid;
                userAnswer = examAnswer.answerText;
                examTime = (int)examAnswer.answerTime;
            }

            var examUid = Guid.Empty;
            var examGrade = _iExamGradeRep.GetAll().FirstOrDefault(a => a.Id == examGradeUid);
            if (examGrade != null)
                examUid = examGrade.examUid;


            //处理回车换行
            standardAnswer = StringUtil.ReplaceEnter2BrWhenNoHtml(standardAnswer ?? "");
            userAnswer = StringUtil.ReplaceEnter2BrWhenNoHtml(userAnswer);

            StringBuilder stringBuilder = new StringBuilder();
            string sAnswerText;
            if (isShowStandardAnswer)
            {
                sAnswerText = GetQuestionAnswerText(questionBaseTypeCode, standardAnswer, arrOriginalSelectAnswerIndex);
                sAnswerText = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", sAnswerText, true);//也要图片
                if (examQuestionRow.questionBaseTypeCode != EnumQuestionBaseTypeCode.Program && examQuestionRow.questionBaseTypeCode != EnumQuestionBaseTypeCode.ProgramFill)
                {
                    var strAnswerDesc = "&nbsp;★" + ("标准答案") + "：";
                    if (sAnswerText != "none")
                    {
                        stringBuilder.Append("<table>");
                        stringBuilder.Append(" <tr><td colspan='2' style='text-align:left'><font color='blue'>" + strAnswerDesc + sAnswerText + "</font></td></tr>\n");
                        stringBuilder.Append("</table>");
                    }
                }
            }

            if (isShowUserAnswer)
            {
                sAnswerText = GetQuestionAnswerText(questionBaseTypeCode, userAnswer, arrOriginalSelectAnswerIndex);
                //处理超文本答题答案显示
                if (isAnswerByHtml == "Y")
                {


                    sAnswerText = FilePathUtil.GetContentTextWithFilePath("Exam_" + examUid + "\\" + "ExamGrade_" + examGradeUid + "\\" + examQuestionRow.Id, "ExamAnswer", sAnswerText, true);

                    //====读取文件====

                    //string fileServerFileWebRootPath = AppConfiguration.FileServerFileWebPathRoot + "/";
                    //string filePath = FilePathUtil.GetOppositeFileWebPathRoot("Exam_" + examUid + "\\" + "ExamGrade_" + examGradeUid + "\\" + examQuestionRow.Id, "ExamAnswer");
                    //string[] files = FileManager.GetFileListFromSpecificPath(FilePath);
                }

                string sExamTime = "";
                int nMinute = 0;
                int nSecond = 0;
                if (examTime > 0)
                {
                    sExamTime = "" + ("答题时间") + ":" + SPOC.Exam.DateTimeUtil.ToTimeStrFromSecond(examTime);
                }
                if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Program && questionBaseTypeCode != EnumQuestionBaseTypeCode.ProgramFill)
                {
                    sAnswerText = HttpUtility.HtmlDecode(sAnswerText);
                }
                else
                {
                    sAnswerText = "<pre>" + HttpUtility.UrlDecode(sAnswerText).Replace("<", "&lt;").Replace(">", "&gt;") + "</pre>";
                }
                if (sAnswerText != "none")
                {
                    stringBuilder.Append("<table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
                    if (parentQuestionUid == Guid.Empty) //用于处理组合题子试题显示
                    {
                        stringBuilder.Append(" <tr><td colspan='2' class=\"SearchButton\"><font color='#660033'>&nbsp;☆" + ("考生答案") + "：");

                        if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Voice)
                        {
                            string wavFile = OfficeDTUtil.getOppositeExamOfficeDTFileWebPathWithoutFileName(examUid.ToString(), examGradeUid.ToString()).Trim('/') + "\\" + questionUid + ".mp3";
                            if (FilePathUtil.IsFileExist(wavFile))
                            {
                                stringBuilder.Append(GetSoundPlayerControl(AppConfiguration.FileServerFileWebPathRoot + "/" + wavFile, examGradeUid + "_" + questionUid));
                            }
                            stringBuilder.Append(sAnswerText + "&nbsp;&nbsp;&nbsp;&nbsp;" + sExamTime + "</font></td></tr>\n");
                        }
                        else if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Operate)
                        {
                            if (operateTypeCode == "html")
                            {
                                stringBuilder.Append(sAnswerText + "&nbsp;&nbsp;&nbsp;&nbsp;" + sExamTime + "</font></td></tr>\n");
                            }
                            else
                            {
                                stringBuilder.Append("<a href=\"javascript:void(0)\" onclick=\"ViewContractTextFile('" + examGradeUid + "','" + questionUid + "','" + examQuestionRow.operateTypeCode + "')\"  id='btnViewUsingOffice' class='button_btg'>" + ("查看操作") + "</a>");
                                stringBuilder.Append("&nbsp;&nbsp;&nbsp;&nbsp;" + sExamTime + "</font></td></tr>\n");
                            }
                        }
                        else
                        {
                            stringBuilder.Append(sAnswerText + "&nbsp;&nbsp;&nbsp;&nbsp;" + sExamTime + "</font></td></tr>\n");
                        }
                    }
                    else
                    {
                        stringBuilder.Append(" <tr><td colspan='2' class=\"SearchButton\"><font color='#660033'>&nbsp;&nbsp;☆" + ("考生答案") + "：");
                        if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Voice)
                        {
                            string wavFile = OfficeDTUtil.getOppositeExamOfficeDTFileWebPathWithoutFileName(examUid.ToString(), examGradeUid.ToString()).Trim('/') + "\\" + questionUid + ".mp3";
                            if (FilePathUtil.IsFileExist(wavFile))
                            {
                                stringBuilder.Append(GetSoundPlayerControl(AppConfiguration.FileServerFileWebPathRoot + "/" + wavFile, examGradeUid + "_" + questionUid));
                            }
                            stringBuilder.Append(sAnswerText + "&nbsp;&nbsp;&nbsp;&nbsp;" + sExamTime + "</font></td></tr>\n");
                        }
                        else if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Operate)
                        {
                            stringBuilder.Append("<a href=\"javascript:void(0)\" onclick=\"ViewContractTextFile('" + examGradeUid + "','" + questionUid + "','" + examQuestionRow.operateTypeCode + "')\" id='btnViewUsingOffice' class='button_btg'>" + ("查看操作") + " </a>");
                            stringBuilder.Append("&nbsp;&nbsp;&nbsp;&nbsp;" + sExamTime + "</font></td></tr>\n");
                        }
                        else
                        {
                            stringBuilder.Append(sAnswerText + "&nbsp;&nbsp;&nbsp;&nbsp;" + sExamTime + "</font></td></tr>\n");
                        }
                    }
                    stringBuilder.Append("</table>");
                }

                //展示编译运行结果与标准答案的对比
                if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Program || questionBaseTypeCode == EnumQuestionBaseTypeCode.ProgramFill)
                {
                    var examProramResult =
                        _iExamProgramResultRep.GetAll().FirstOrDefault(a => a.GradeId == examGradeUid && a.QuestionId == questionUid);
                    var textColor = "blue";
                    if (examQuestionRow.MultiTest)
                    {
                        var inputParams = examQuestionRow.InputParam.Split('|');
                        var standardAnswers = examQuestionRow.standardAnswer.Split('|');
                        var answers = new string[inputParams.Length];
                        if (examProramResult != null)
                        {
                            var results = examProramResult.Result.Split('|');
                            for (var i = 0; i < answers.Length; i++)
                            {
                                if (results.Length <= i)
                                {
                                    answers[i] = "";
                                }
                                else
                                {
                                    answers[i] = results[i];
                                }
                            }
                        }
                        for (int i = 0; i < inputParams.Length; i++)
                        {
                            var inputParam = inputParams[i];
                            var sAnswer = standardAnswers[i];
                            var result = answers[i] ?? "";
                            textColor = sAnswer == result ? "blue" : "red";
                            stringBuilder.Append($"<table style=\"text-color:{textColor};\">");
                            stringBuilder.Append($"<tr><td colspan=\"2\">[第{i+1}次编译运行]</td></tr>");
                            if (!string.IsNullOrEmpty(result))
                            {
                                result = result.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br>");
                            }
                            stringBuilder.Append(ProgramResultFormat(examQuestionRow.param, inputParam, sAnswer, result));
                            stringBuilder.Append("</table>");
                        }
                    }
                    else
                    {
                        if (examProramResult == null || examQuestionRow.standardAnswer != examProramResult.Result)
                        {
                            textColor = "red";
                        }
                        stringBuilder.Append($"<table style=\"text-color:{textColor};\">");
                        stringBuilder.Append(ProgramResultFormat(examQuestionRow.param, examQuestionRow.InputParam,
                            examQuestionRow.standardAnswer, examProramResult == null? "" : examProramResult.Result));
                        stringBuilder.Append("</table>");
                    }
                    
                }

            }

            return stringBuilder.ToString();
        }

        private string ProgramResultFormat(string param, string inputParam, string standardAnswer, string answer)
        {
            var sb = new StringBuilder();
            var isPass = standardAnswer == answer ? "是" : "否";
            if (!string.IsNullOrEmpty(param))
            {
                sb.Append($"<tr><td>程序参数：</td><td>{param.Replace("|", " ")}</td></tr>");
            }
            if (!string.IsNullOrEmpty(inputParam))
            {
                sb.Append($"<tr><td>输入流参数：</td><td>{inputParam}</td></tr>");
            }
            sb.Append($"<tr><td>期望输出：</td><td>{standardAnswer}</td></tr>");
            sb.Append($"<tr><td>执行结果：</td><td>{answer}</td></tr>");
            sb.Append($"<tr><td>是否通过：</td><td>{isPass}</td></tr>");
            return sb.ToString(); 
        }
        #endregion

        /// <summary>
        /// 显示试题的考试分析结果
        /// </summary>
        /// <param name="contentIndex"></param>
        /// <param name="questionUid"></param>
        /// <param name="questionBaseTypeCode"></param>
        /// <param name="score"></param>
        /// <param name="selectAnswer"></param>
        /// <param name="standardAnswer"></param>
        /// <param name="paperUid"></param>
        /// <param name="content"></param>
        /// <param name="dtUserAnswer"></param>
        /// <returns></returns>

        #region GetContentAnswerAnalyzeView
        public string GetContentAnswerAnalyzeView(string paperUid, int contentIndex, string questionUid, string questionBaseTypeCode, 
            string content, float score, string selectAnswer, string standardAnswer, DataTable dtUserAnswer)
        {
            StringBuilder stringBuilder = new StringBuilder();

            //处理回车换行
            content = StringUtil.ReplaceEnter2BrWhenNoHtml(content);
            standardAnswer = StringUtil.ReplaceEnter2BrWhenNoHtml(standardAnswer);
            selectAnswer = StringUtil.ReplaceEnter2BrWhenNoHtml(selectAnswer);

            if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Fill)
            {
                if (standardAnswer != "")
                    content = QuestionUtil.ReplaceFillInContent(content, questionUid, standardAnswer);
                else
                    content = QuestionUtil.ReplaceFillInContent(content, questionUid, "");
            }

            //开始更换图片等的路径
            content = FilePathUtil.GetContentTextWithFilePath(questionUid, "question", content, true);	//图片全部在题库中了


            //开始写试题内容
            stringBuilder.Append("<div style=\"float:left;\" class=\"tableheadtitle\">\n");
            //试题号
            string sContentIndex = contentIndex.ToString();
            if (sContentIndex == "-1" || sContentIndex == "0") sContentIndex = "";
            stringBuilder.Append("<input type='hidden' id='hidquestionUid' name='hidquestionUid' value='" + questionUid + "'>\n");
            stringBuilder.Append("<input type='hidden' id='hidQuestionBaseTypeCode" + questionUid + "' name='hidQuestionBaseTypeCode" + questionUid + "' value='" + questionBaseTypeCode.ToString() + "'>\n");
            stringBuilder.Append(sContentIndex + ".&nbsp;" + content + "&nbsp;<font color='red'>（" + score.ToString() + ("分") + "）</font>");
            stringBuilder.Append("</div>");

            //显示可选答案
            string[] arrSelectAnswer;
            char[] spliter = "|".ToCharArray();
            string upChar;

            dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid);
            int totalAnswerCount = dtUserAnswer.DefaultView.Count;
            dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answer_text=" + StringUtil.QuotedToStr(standardAnswer);
            int rightUserCount = dtUserAnswer.DefaultView.Count;
            double rightPercent = (totalAnswerCount > 0) ? (rightUserCount * 100.0 / totalAnswerCount) : 0;

            switch (questionBaseTypeCode)
            {
                case EnumQuestionBaseTypeCode.Single:
                case EnumQuestionBaseTypeCode.EvaluationSingle:
                    arrSelectAnswer = selectAnswer.Split(spliter);        //保存选择答案数组

                    stringBuilder.Append("<table>\n");
                    stringBuilder.Append("<tr>\n");
                    stringBuilder.Append("  <th width='50%' align='center'>");
                    stringBuilder.Append("		&nbsp;&nbsp;" + ("选项") + "&nbsp;&nbsp;");
                    stringBuilder.Append("  </th>\n");
                    stringBuilder.Append("  <th width='15%' align='center'>&nbsp;&nbsp;" + ("答题人次") + "&nbsp;&nbsp;</th>\n");
                    stringBuilder.Append("  <th width='10%' align='center'>&nbsp;&nbsp;" + ("答题率") + "&nbsp;&nbsp;</th>\n");	//答题率
                    stringBuilder.Append("  <th width='25%' align='center'>&nbsp;&nbsp;" + ("答题率图示") + "&nbsp;&nbsp;</th>\n");	//答题率图示
                    stringBuilder.Append("</tr>\n");

                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        stringBuilder.Append("<tr>\n");

                        //string strChecked;
                        //stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");
                        upChar = ((Char)(65 + j)).ToString();
                        //    if (Standard_Answer == upChar)
                        //        strChecked = " checked ";
                        //    else
                        //        strChecked = "";
                        //    stringBuilder.Append("  <td width='50%'>");
                        //    stringBuilder.Append("		<input type='radio' disabled id='Answer" + questionUid + "' name='Answer" + questionUid + "' value='" + (j).ToString() + "'" + strChecked + ">" + upChar + "." + arrSelect_Answer[j].ToString());
                        //    stringBuilder.Append("  &nbsp;&nbsp;&nbsp;&nbsp;</td>\n");
                        //    dtUserAnswer.DefaultView.RowFilter = "question_uid=" + StringUtil.QuotedToStr(questionUid) + " and answer_text=" + StringUtil.QuotedToStr(upChar);
                        //    stringBuilder.Append("  <td width='15%'>" + dtUserAnswer.DefaultView.Count.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;</td>\n");	//答题人数
                        //    double Percent = (Total_Answer_Count > 0) ? (dtUserAnswer.DefaultView.Count * 100.0 / Total_Answer_Count) : 0;
                        //    stringBuilder.Append("  <td width='10%'>" + Percent.ToString("0.##") + "%&nbsp;&nbsp;&nbsp;&nbsp;</td>\n");	//答题率
                        //    stringBuilder.Append("  <td width='25%'><img src='../../framework/image/Percent_Bar.gif' height='20px' width='" + Percent.ToString("0.##") + "%" + "' /></td>\n");	//答题率图示
                        //    stringBuilder.Append("</tr>\n");

                        stringBuilder.Append("  <td width='50%'>");
                        stringBuilder.Append(upChar + "." + arrSelectAnswer[j]);
                        stringBuilder.Append("  &nbsp;&nbsp;&nbsp;&nbsp;</td>\n");
                        dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answer_text=" + StringUtil.QuotedToStr(upChar);
                        stringBuilder.Append("  <td width='15%'>" + dtUserAnswer.DefaultView.Count.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;</td>\n");	//答题人数
                        double percent = (totalAnswerCount > 0) ? (dtUserAnswer.DefaultView.Count * 100.0 / totalAnswerCount) : 0;
                        stringBuilder.Append("  <td width='10%'>" + percent.ToString("0.##") + "%&nbsp;&nbsp;&nbsp;&nbsp;</td>\n");	//答题率
                        stringBuilder.Append("  <td width='25%'><img src='" + AppConfiguration.FileServerWebRootPath + "/framework/image/Percent_Bar.gif' width=\"" + percent.ToString("0.##") + "%\" height=\"20\" /></td>\n");	//答题率图示
                        stringBuilder.Append("</tr>\n");
                    }

                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("答题总人次") + "：" + totalAnswerCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确人次") + "：" + rightUserCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确答案") + "：" + standardAnswer + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确率") + "：" + rightPercent.ToString("0.##") + "%</td></tr>");

                    stringBuilder.Append("</Table>\n");

                    break;
                case EnumQuestionBaseTypeCode.Multi:
                case EnumQuestionBaseTypeCode.EvaluationMulti:
                    arrSelectAnswer = selectAnswer.Split(spliter);

                    stringBuilder.Append("<table>\n");
                    stringBuilder.Append("<tr>\n");
                    stringBuilder.Append("  <th width='50%' align='center'>");
                    stringBuilder.Append("		&nbsp;&nbsp;" + ("选项") + "&nbsp;&nbsp;");
                    stringBuilder.Append("  </th>\n");
                    stringBuilder.Append("  <th width='15%' align='center'>&nbsp;&nbsp;" + ("答题人次") + "&nbsp;&nbsp;</th>\n");
                    stringBuilder.Append("  <th width='10%' align='center'>&nbsp;&nbsp;" + ("答题率") + "&nbsp;&nbsp;</th>\n");	//答题率
                    stringBuilder.Append("  <th width='25%' align='center'>&nbsp;&nbsp;" + ("答题率图示") + "&nbsp;&nbsp;</th>\n");	//答题率图示
                    stringBuilder.Append("</tr>\n");

                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        stringBuilder.Append("<tr>\n");

                        upChar = ((Char)(65 + j)).ToString();
                        stringBuilder.Append("  <td width='50%'>");

                        stringBuilder.Append(upChar + "." + arrSelectAnswer[j]);
                        stringBuilder.Append("  &nbsp;&nbsp;</td>\n");
                        dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answer_text like '%" + upChar + "%'";
                        stringBuilder.Append("  <td width='15%'>" + dtUserAnswer.DefaultView.Count.ToString() + "&nbsp;&nbsp;</td>\n");	//答题人数
                        double percent = (totalAnswerCount > 0) ? (dtUserAnswer.DefaultView.Count * 100.0 / totalAnswerCount) : 0;
                        stringBuilder.Append("  <td width='10%'>" + percent.ToString("0.##") + "%&nbsp;&nbsp;</td>\n");	//答题率
                        stringBuilder.Append("  <td width='25%'><img src='" + AppConfiguration.FileServerWebRootPath + "/framework/image/Percent_Bar.gif' width=\"" + percent.ToString("0.##") + "%\" height=\"20\" /></td>\n");	//答题率图示
                        stringBuilder.Append("</tr>\n");
                    }

                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("答题总人次") + "：" + totalAnswerCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确人次") + "：" + rightUserCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确答案") + "：" + standardAnswer + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确率") + "：" + rightPercent.ToString("0.##") + "%</td></tr>");
                    stringBuilder.Append("</Table>\n");

                    break;
                case EnumQuestionBaseTypeCode.Judge:
                case EnumQuestionBaseTypeCode.JudgeCorrect:     //2008-4-18 Nick 还不确定
                    arrSelectAnswer = selectAnswer.Split(spliter);
                    string[] realAnswer = new string[] { "N", "Y" };
                    if (arrSelectAnswer.Length != 2)
                        arrSelectAnswer = new[] { ("错误"), ("正确") };

                    stringBuilder.Append("<table>\n");
                    stringBuilder.Append("<tr>\n");
                    stringBuilder.Append("  <th width='50%' align='center'>");
                    stringBuilder.Append("		&nbsp;&nbsp;" + ("选项") + "&nbsp;&nbsp;");
                    stringBuilder.Append("  </th>\n");
                    stringBuilder.Append("  <th width='15%' align='center'>&nbsp;&nbsp;" + ("答题人次") + "&nbsp;&nbsp;</th>\n");
                    stringBuilder.Append("  <th width='10%' align='center'>&nbsp;&nbsp;" + ("答题率") + "&nbsp;&nbsp;</th>\n");	//答题率
                    stringBuilder.Append("  <th width='25%' align='center'>&nbsp;&nbsp;" + ("答题率图示") + "&nbsp;&nbsp;</th>\n");	//答题率图示
                    stringBuilder.Append("</tr>\n");

                    for (int j = 0; j < 2; j++)
                    {
                        stringBuilder.Append("<tr>");
                        stringBuilder.Append("  <td width='50%'>");

                        stringBuilder.Append(arrSelectAnswer[j]);

                        stringBuilder.Append("  </td>\n");
                        string rowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answer_text='" + realAnswer[j] + "'";
                        if (questionBaseTypeCode.Equals(EnumQuestionBaseTypeCode.JudgeCorrect))
                        {
                            if (realAnswer[j] != "Y")
                                rowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answer_text <> 'Y'";
                        }
                        dtUserAnswer.DefaultView.RowFilter = rowFilter;
                        stringBuilder.Append("  <td width='15%'>" + dtUserAnswer.DefaultView.Count + "</td>\n");	//答题人数
                        double percent = (totalAnswerCount > 0) ? (dtUserAnswer.DefaultView.Count * 100.0 / totalAnswerCount) : 0;
                        stringBuilder.Append("  <td width='10%'>" + percent.ToString("0.##") + "%&nbsp;&nbsp;</td>\n");	//答题率
                        stringBuilder.Append("  <td width='25%'><<img src='" + AppConfiguration.FileServerWebRootPath + "/framework/image/Percent_Bar.gif' width=\"" + percent.ToString("0.##") + "%\" height=\"20\" /></td>\n");	//答题率图示
                        stringBuilder.Append("</tr>\n");
                    }
                    string standard;
                    if (standardAnswer == "Y")
                        standard = ("正确");
                    else
                        standard = ("错误");

                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("答题总人次") + "：" + totalAnswerCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确人次") + "：" + rightUserCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确答案") + "：" + standard + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确率") + "：" + rightPercent.ToString("0.##") + "%</td></tr>");

                    stringBuilder.Append("</Table>\n");

                    break;
                case EnumQuestionBaseTypeCode.Fill:
                    arrSelectAnswer = GetFillQuestionAnswer(paperUid, questionUid);

                    stringBuilder.Append("<table>\n");
                    stringBuilder.Append("<tr>\n");
                    stringBuilder.Append("  <th width='50%' align='center'>");
                    stringBuilder.Append("		&nbsp;&nbsp;" + ("答案") + "&nbsp;&nbsp;");
                    stringBuilder.Append("  </th>\n");
                    stringBuilder.Append("  <th width='15%' align='center'>&nbsp;&nbsp;" + ("答题人次") + "&nbsp;&nbsp;</th>\n");
                    stringBuilder.Append("  <th width='10%' align='center'>&nbsp;&nbsp;" + ("答题率") + "&nbsp;&nbsp;</th>\n");	//答题率
                    stringBuilder.Append("  <th width='25%' align='center'>&nbsp;&nbsp;" + ("答题率图示") + "&nbsp;&nbsp;</th>\n");	//答题率图示
                    stringBuilder.Append("</tr>\n");

                    //显示用户填写的答案
                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        stringBuilder.Append("  <td width='50%'>" + arrSelectAnswer[j] + "</td>");
                        dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answer_text=" + StringUtil.QuotedToStr(arrSelectAnswer[j]);
                        stringBuilder.Append(" <td width='15%'>" + dtUserAnswer.DefaultView.Count + "<td>\n");   //答题人数
                        double percent1 = (totalAnswerCount > 0) ? (dtUserAnswer.DefaultView.Count * 100.0 / totalAnswerCount) : 0;
                        stringBuilder.Append(percent1.ToString("0.##") + "%");	//答题率
                        stringBuilder.Append(" <td width='25%'><img src='" + AppConfiguration.FileServerWebRootPath + "/framework/image/Percent_Bar.gif' width=\"" + percent1.ToString("0.##") + "%\" height=\"20\" /></td>\n");	//答题率图示
                        stringBuilder.Append("</tr>\n");
                    }

                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("答题总人次") + "：" + totalAnswerCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确人次") + "：" + rightUserCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确率") + "：" + rightPercent.ToString("0.##") + "%</td></tr>");

                    stringBuilder.Append("</Table>\n");

                    break;
                case EnumQuestionBaseTypeCode.Answer:
                    arrSelectAnswer = GetFillQuestionAnswer(paperUid, questionUid);

                    stringBuilder.Append("<table>\n");
                    stringBuilder.Append("<tr>\n");
                    stringBuilder.Append("  <th width='50%' align='center'>");
                    stringBuilder.Append("		&nbsp;&nbsp;" + ("答案") + "&nbsp;&nbsp;");
                    stringBuilder.Append("  </th>\n");
                    stringBuilder.Append("  <th width='15%' align='center'>&nbsp;&nbsp;" + ("答题人次") + "&nbsp;&nbsp;</th>\n");
                    stringBuilder.Append("  <th width='10%' align='center'>&nbsp;&nbsp;" + ("答题率") + "&nbsp;&nbsp;</th>\n");	//答题率
                    stringBuilder.Append("  <th width='25%' align='center'>&nbsp;&nbsp;" + ("答题率图示") + "&nbsp;&nbsp;</th>\n");	//答题率图示
                    stringBuilder.Append("</tr>\n");

                    //显示用户填写的答案
                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        stringBuilder.Append("  <td width='50%'>" + arrSelectAnswer[j] + "</td>");
                        dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answer_text=" + StringUtil.QuotedToStr(arrSelectAnswer[j]);
                        stringBuilder.Append(" <td width='15%'>" + dtUserAnswer.DefaultView.Count + "<td>\n");   //答题人数
                        double percent1 = (totalAnswerCount > 0) ? (dtUserAnswer.DefaultView.Count * 100.0 / totalAnswerCount) : 0;
                        stringBuilder.Append(percent1.ToString("0.##") + "%");	//答题率
                        stringBuilder.Append(" <td width='25%'><img src='" + AppConfiguration.FileServerWebRootPath + "/framework/image/Percent_Bar.gif' width=\"" + percent1.ToString("0.##") + "%\" height=\"20\" /></td>\n");	//答题率图示
                        stringBuilder.Append("</tr>\n");
                    }

                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("答题总人次") + "：" + totalAnswerCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确人次") + "：" + rightUserCount.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("正确率") + "：" + rightPercent.ToString("0.##") + "%</td></tr>");
                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("正确答案") + "：" + standardAnswer + "</td></tr>");
                    stringBuilder.Append("</Table>\n");

                    break;
                case EnumQuestionBaseTypeCode.Compose:

                    break;
                default:
                    stringBuilder.Append(("参考答案:") + standardAnswer);
                    break;
            }
            return stringBuilder.ToString();

        }
        #endregion

        /// <summary>
        /// 考试试卷分析结果
        /// </summary>
        /// <param name="contentIndex"></param>
        /// <param name="questionUid"></param>
        /// <param name="questionBaseTypeCode"></param>
        /// <param name="score"></param>
        /// <param name="selectAnswer"></param>
        /// <param name="standardAnswer"></param>
        /// <param name="paperUid"></param>
        /// <param name="content"></param>
        /// <param name="dtUserAnswer"></param>
        /// <returns></returns>

        #region ExamAnalyzeView
        public string ExamAnalyzeView(string paperUid, int contentIndex, string questionUid, string questionBaseTypeCode, 
            string content, float score, string selectAnswer, string standardAnswer, DataTable dtUserAnswer)
        {

            StringBuilder stringBuilder = new StringBuilder();

            //处理回车换行
            content = StringUtil.ReplaceEnter2BrWhenNoHtml(content);
            standardAnswer = StringUtil.ReplaceEnter2BrWhenNoHtml(standardAnswer);
            selectAnswer = StringUtil.ReplaceEnter2BrWhenNoHtml(selectAnswer);

            if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Fill)
            {
                if (standardAnswer != "")
                    content = QuestionUtil.ReplaceFillInContent(content, questionUid, standardAnswer);
                else
                    content = QuestionUtil.ReplaceFillInContent(content, questionUid, "");
            }

            //开始更换图片等的路径
            content = FilePathUtil.GetContentTextWithFilePath(questionUid, "question", content, true);	//图片全部在题库中了


            //开始写试题内容
            stringBuilder.Append("<td colspan='2' width='100%'>\n");
            //试题号
            string sContentIndex = contentIndex.ToString();
            if (sContentIndex == "-1" || sContentIndex == "0") sContentIndex = "";
            stringBuilder.Append("<input type='hidden' id='hidquestionUid' name='hidquestionUid' value='" + questionUid + "'>\n");
            stringBuilder.Append("<input type='hidden' id='hidQuestionBaseTypeCode" + questionUid + "' name='hidQuestionBaseTypeCode" + questionUid + "' value='" + questionBaseTypeCode.ToString() + "'>\n");
            stringBuilder.Append(sContentIndex + ".&nbsp;" + content + "&nbsp;<font color='red'>（" + score.ToString() + ("分") + "）</font>");
            stringBuilder.Append("</td></tr>");

            //显示可选答案
            string[] arrSelectAnswer;
            char[] spliter = "|".ToCharArray();
            string lowChar;
            string upChar;

            dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid);
            int totalAnswerCount = dtUserAnswer.DefaultView.Count;
            dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answer_text=" + StringUtil.QuotedToStr(standardAnswer);
            int rightUserCount = dtUserAnswer.DefaultView.Count;
            double rightPercent = (totalAnswerCount > 0) ? (rightUserCount * 100.000 / totalAnswerCount) : 0;



            //ExamAnswerRow examAnswerRow; 
            //examAnswerRow = null;
            //if (dtUserAnswer != null)
            //{
            //    dtUserAnswer.DefaultView.RowFilter = "question_uid=" + StringUtil.QuotedToStr(questionUid);
            //    if (dtUserAnswer.DefaultView.Count > 0)
            //    {
            //        examAnswerRow = new ExamAnswerRow();
            //        examAnswerRow.AssignByDataRow(dtUserAnswer.DefaultView[0].Row);
            //    }
            //} 
            //dtUserAnswer.DefaultView.RowFilter = "question_uid=" + StringUtil.QuotedToStr(questionUid);
            // decimal Total_Answer_Score = examAnswerRow.JudgeScore;

            //单题平均分
            double answerAvg = GetExamUserAnswerAvg(dtUserAnswer.Rows[0]["examUid"].ToString(), paperUid, questionUid);

            //分段人数
            double number = Convert.ToDouble(totalAnswerCount * 0.5);
            //高分段平均分
            double answerUpAvg = GetExamUserAnswerSubAvg(dtUserAnswer.Rows[0]["examUid"].ToString(), paperUid, questionUid, number, "desc");
            //低分段平均分
            double answerDownAvg = GetExamUserAnswerSubAvg(dtUserAnswer.Rows[0]["examUid"].ToString(), paperUid, questionUid, number, "asc");

            //难度系数
            double lNum = Math.Round(1 - answerAvg / Convert.ToDouble(score), 3);

            //区分度
            double dNum = Math.Round(2 * (answerUpAvg - answerDownAvg) / Convert.ToDouble(score), 3);

            switch (questionBaseTypeCode)
            {
                case EnumQuestionBaseTypeCode.Single:
                case EnumQuestionBaseTypeCode.EvaluationSingle:
                    arrSelectAnswer = selectAnswer.Split(spliter);        //保存选择答案数组

                    stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");

                    stringBuilder.Append("<table class='TableList' cellSpacing='0' cellPadding='0' width='100%' align='center' border='1'>\n");
                    stringBuilder.Append("<tr class='HeadRow'>\n");
                    stringBuilder.Append("  <td width='50%' align='center'>");
                    stringBuilder.Append("		&nbsp;&nbsp;" + ("选项") + "&nbsp;&nbsp;");
                    stringBuilder.Append("  </td>\n");
                    stringBuilder.Append("</tr>\n");

                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        stringBuilder.Append("<tr>\n");

                        string strChecked;
                        //stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");
                        lowChar = ((Char)(97 + j)).ToString();
                        upChar = ((Char)(65 + j)).ToString();
                        if (standardAnswer == upChar)
                            strChecked = " checked ";
                        else
                            strChecked = "";
                        stringBuilder.Append("  <td width='50%'>");
                        stringBuilder.Append("		<input type='radio' disabled id='Answer" + questionUid + "' name='Answer" + questionUid + "' value='" + (j).ToString() + "'" + strChecked + ">" + upChar + "." + arrSelectAnswer[j].ToString());
                        stringBuilder.Append("  &nbsp;&nbsp;&nbsp;&nbsp;</td>\n");
                        dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answerText=" + StringUtil.QuotedToStr(upChar);
                        stringBuilder.Append("</tr>\n");
                    }

                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("难度系数") + "：" + lNum + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("区分度") + "：" + dNum + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>");

                    stringBuilder.Append("</Table>\n");
                    stringBuilder.Append("   </td>\n</tr>\n");

                    break;
                case EnumQuestionBaseTypeCode.Multi:
                case EnumQuestionBaseTypeCode.EvaluationMulti:
                    arrSelectAnswer = selectAnswer.Split(spliter);

                    stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");

                    stringBuilder.Append("<table class='TableList' cellSpacing='0' cellPadding='0' width='100%' align='center' border='1'>\n");
                    stringBuilder.Append("<tr class='HeadRow'>\n");
                    stringBuilder.Append("  <td width='50%' align='center'>");
                    stringBuilder.Append("		&nbsp;&nbsp;" + ("选项") + "&nbsp;&nbsp;");
                    stringBuilder.Append("  </td>\n");
                    stringBuilder.Append("</tr>\n");

                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        stringBuilder.Append("<tr>\n");

                        string strChecked;
                        //stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");
                        lowChar = ((Char)(97 + j)).ToString();
                        upChar = ((Char)(65 + j)).ToString();
                        stringBuilder.Append("  <td width='50%'>");
                        if (("|" + standardAnswer + "|").IndexOf("|" + upChar + "|") > -1)
                            strChecked = " checked ";
                        else
                            strChecked = "";

                        stringBuilder.Append("<input type='checkbox' disabled id='Answer" + questionUid + "' name='Answer" + questionUid + "' value='" + (j).ToString() + "'" + strChecked + ">" + upChar + "." + arrSelectAnswer[j]);
                        stringBuilder.Append("  &nbsp;&nbsp;</td>\n");
                        dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answerText like '%" + upChar + "%'";
                        stringBuilder.Append("</tr>\n");
                    }
                    //难度系数
                    //double LNum = Math.Round(1 - Right_Percent / 100, 3);U
                    //区分度
                    //double DNum = Math.Round(Right_Percent * 4 / 100, 3);

                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("难度系数") + "：" + lNum.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("区分度") + "：" + dNum.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>");

                    stringBuilder.Append("</Table>\n");
                    stringBuilder.Append("   </td>\n</tr>\n");

                    break;
                case EnumQuestionBaseTypeCode.Judge:
                case EnumQuestionBaseTypeCode.JudgeCorrect:     //2008-4-18 Nick 还不确定
                    arrSelectAnswer = selectAnswer.Split(spliter);
                    if (arrSelectAnswer.Length != 2)
                        arrSelectAnswer = new string[] { ("错误"), ("正确") };

                    stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");

                    stringBuilder.Append("<table class='TableList' cellSpacing='0' cellPadding='0' width='100%' align='center' border='1'>\n");
                    stringBuilder.Append("<tr class='HeadRow'>\n");
                    stringBuilder.Append("  <td width='50%' align='center'>");
                    stringBuilder.Append("		&nbsp;&nbsp;" + ("选项") + "&nbsp;&nbsp;");
                    stringBuilder.Append("  </td>\n");
                    stringBuilder.Append("</tr>\n");

                    for (int j = 0; j < 2; j++)
                    {
                        string strChecked;
                        if (standardAnswer == (j).ToString())
                            strChecked = " checked ";
                        else
                            strChecked = "";
                        stringBuilder.Append("  <td width='50%'>");
                        //stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");
                        stringBuilder.Append("<input type='radio' disabled id='Answer" + questionUid + "' name='Answer" + questionUid + "' value='" + j.ToString() + "'" + strChecked + ">" + arrSelectAnswer[j].ToString());

                        stringBuilder.Append("  </td>\n");
                        dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answerText='" + (j + 1).ToString() + "'";
                        stringBuilder.Append("</tr>\n");
                    }
                    //难度系数
                    //  LNum = Math.Round(1 - Right_Percent / 100, 3);
                    //区分度
                    //DNum = Math.Round(Right_Percent * 4 / 100, 3);
                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("难度系数") + "：" + lNum + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("区分度") + "：" + dNum + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>");

                    stringBuilder.Append("</Table>\n");
                    stringBuilder.Append("   </td>\n</tr>\n");

                    break;
                case EnumQuestionBaseTypeCode.Fill:
                    arrSelectAnswer = GetFillQuestionAnswer(paperUid, questionUid);

                    stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");

                    stringBuilder.Append("<table class='TableList' cellSpacing='0' cellPadding='0' width='100%' align='center' border='1'>\n");
                    stringBuilder.Append("<tr class='HeadRow'>\n");
                    stringBuilder.Append("  <td width='50%' align='center'>");
                    stringBuilder.Append("		&nbsp;&nbsp;" + ("答案") + "&nbsp;&nbsp;");
                    stringBuilder.Append("  </td>\n");
                    stringBuilder.Append("</tr>\n");

                    //显示用户填写的答案
                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        stringBuilder.Append("  <td width='50%'>" + arrSelectAnswer[j].ToString() + "</td>");
                        dtUserAnswer.DefaultView.RowFilter = "questionUid=" + StringUtil.QuotedToStr(questionUid) + " and answerText=" + StringUtil.QuotedToStr(arrSelectAnswer[j]);
                        stringBuilder.Append("</tr>\n");
                    }

                    stringBuilder.Append("<tr><td colspan=4 height=20>" + ("难度系数") + "：" + lNum.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + ("区分度") + "：" + dNum.ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>");

                    stringBuilder.Append("</Table>\n");
                    stringBuilder.Append("   </td>\n</tr>\n");

                    break;
                case EnumQuestionBaseTypeCode.Compose:

                    break;
                default:
                    stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");
                    stringBuilder.Append(("参考答案:") + standardAnswer);
                    stringBuilder.Append("   </td>\n</tr>\n");
                    stringBuilder.Append("<tr><td width='10px' nowrap><td width='100%'>");
                    stringBuilder.Append(("难度系数:") + lNum.ToString() + ("区分度:") + dNum.ToString());
                    stringBuilder.Append("   </td>\n</tr>\n");
                    break;
            }
            return stringBuilder.ToString();

        }
        #endregion


        public string GetSoundPlayerControl(string sSoundFileName, string objectName)
        {
            string sHtml = "<embed id='" + objectName + "' width=200 height=25 src=\"" + sSoundFileName + "\" controls=controlpanel type=application/vnd.rn-realmedia autostart=false></embed>";
            return sHtml;

            //string sReturnHTML = "";
            //sReturnHTML = "<object id='" + objectName + "' codebase='http://activex.microsoft.com/activex/controls/mimages/player/en/nsmp2inf.cab#Version=5,1,52,701standby=Loading' ";
            //sReturnHTML = sReturnHTML + "		type=application/x-oleobject height=320 width=452 ";
            //sReturnHTML = sReturnHTML + "		classid=CLSID:6BF52A52-394A-11d3-B153-00C04F79FAA6 viewastext>";
            //sReturnHTML = sReturnHTML + "<param name='url' value='" + sSoundFileName + "'>";
            //sReturnHTML = sReturnHTML + "<param name='AudioStream' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='AutoSize' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='AutoStart' value='true'>";
            //sReturnHTML = sReturnHTML + "			<param name='AnimationAtStart' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='AllowScan' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='AllowChangeDisplaySize' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='AutoRewind' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='Balance' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='BaseURL' value>";
            //sReturnHTML = sReturnHTML + "			<param name='BufferingTime' value='5'>";
            //sReturnHTML = sReturnHTML + "			<param name='CaptioningID' value>";
            //sReturnHTML = sReturnHTML + "			<param name='ClickToPlay' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='CursorType' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='CurrentPosition' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='CurrentMarker' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='DefaultFrame' value>";
            //sReturnHTML = sReturnHTML + "			<param name='DisplayBackColor' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='DisplayForeColor' value='16777215'>";
            //sReturnHTML = sReturnHTML + "			<param name='DisplayMode' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='DisplaySize' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='Enabled' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='EnableContextMenu' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='EnablePositionControls' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='EnableFullScreenControls' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='EnableTracker' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='Filename' value>";
            //sReturnHTML = sReturnHTML + "			<param name='InvokeURLs' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='Language' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='Mute' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='PlayCount' value='1'>";
            //sReturnHTML = sReturnHTML + "			<param name='PreviewMode' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='Rate' value='1'>";
            //sReturnHTML = sReturnHTML + "			<param name='SAMILang' value>";
            //sReturnHTML = sReturnHTML + "			<param name='SAMIStyle' value>";
            //sReturnHTML = sReturnHTML + "			<param name='SAMIFileName' value>";
            //sReturnHTML = sReturnHTML + "			<param name='SelectionStart' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='SelectionEnd' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='SendOpenStateChangeEvents' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='SendWarningEvents' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='SendErrorEvents' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='SendKeyboardEvents' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='SendMouseClickEvents' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='SendMouseMoveEvents' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='SendPlayStateChangeEvents' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='ShowCaptioning' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='ShowControls' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='ShowAudioControls' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='ShowDisplay' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='ShowGotoBar' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='ShowPositionControls' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='ShowStatusBar' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='ShowTracker' value='-1'>";
            //sReturnHTML = sReturnHTML + "			<param name='TransparentAtStart' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='VideoBorderWidth' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='VideoBorderColor' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='VideoBorder3D' value='0'>";
            //sReturnHTML = sReturnHTML + "			<param name='Volume' value='600'>";
            //sReturnHTML = sReturnHTML + "			<param name='WindowlessVideo' value='0'>";
            //sReturnHTML = sReturnHTML + "		</object>";

            //return sReturnHTML;
        }

        public string[] GetFillQuestionAnswer(string paperUid, string questionUid)
        {
            List<object> dtTable = GetFillQuestion(paperUid, questionUid);
            string[] arrSelectAnswer = new string[dtTable.Count];

            for (int i = 0; i < dtTable.Count; i++)
            {
                arrSelectAnswer[i] = dtTable[i].ToString();
            }

            return arrSelectAnswer;
        }

        /// <summary>
        /// 返回填空题答案数据集
        /// </summary>
        /// <param name="paperUid">试卷ID</param>
        /// <param name="questionUid">试题ID</param>
        /// <returns></returns>
        public List<object> GetFillQuestion(string paperUid, string questionUid)
        {
            /*
            string sql = string.Format(@"select exam_answer.answer_text from exam_grade 
inner join exam_answer on exam_grade.exam_grade_uid=exam_answer.exam_grade_uid
where exam_grade.paper_uid='{0}' and exam_answer.question_uid='{1}' order by answer_text", paperUid, questionUid);
            */

            var paraperId = Guid.Parse(paperUid);
            var questionId = Guid.Parse(questionUid);
            var result = (from examGrade in _iExamGradeRep.GetAll()
                join examAnswer in _iExamAnswerRep.GetAll() on examGrade.Id equals examAnswer.examGradeUid
                where examGrade.paperUid == paraperId && examAnswer.questionUid== questionId
                orderby examAnswer.answerText
                select examAnswer.answerText).ToList<object>();

            return result;
        }

        /// <summary>
        /// 返回单个题的平均分
        /// </summary>
        /// <param name="examUid">考试ID</param>
        /// <param name="paperUid">试卷ID</param>
        /// <param name="questionUid">试题ID</param>
        /// <returns>单题平均分</returns>
        private double GetExamUserAnswerAvg(string examUid, string paperUid, string questionUid)
        {
            /*
            string sql = string.Format(@"select avg(exam_answer.judge_score) as judge_score from  exam_grade 
inner join exam_answer on exam_grade.exam_grade_uid=exam_answer.exam_grade_uid
where 1=1 and exam_grade.exam_uid='{0}' and exam_grade.paper_uid='{1}' 
and exam_answer.question_uid='{2}'", examUid, paperUid, questionUid);
            */
            var examId = Guid.Parse(examUid);
            var paperId = Guid.Parse(paperUid);
            var questionId = Guid.Parse(questionUid);
            var result = (from examGrade in _iExamGradeRep.GetAll()
                join examAnswer in _iExamAnswerRep.GetAll() on examGrade.Id equals examAnswer.examGradeUid
                where
                    examGrade.Id == examId && examGrade.paperUid == paperId &&
                    examAnswer.questionUid == questionId
                select examAnswer.judgeScore).ToList();

            return result.Count > 0 ? Convert.ToDouble(result.Average()) : Convert.ToDouble(0);
        }
        
        /// <summary>
        /// 返回单个题的高分段的平均分
        /// </summary>
        /// <param name="examUid">考试ID</param>
        /// <param name="paperUid">试卷ID</param>
        /// <param name="questionUid">试题ID</param>
        /// <param name="number">人数</param>
        /// <param name="strdesc">排序</param>
        /// <returns>高分段的平均分</returns>
        public double GetExamUserAnswerSubAvg(string examUid, string paperUid, string questionUid, double number, string strdesc)
        {
            /*
            string sql = string.Format(@"select avg(exam_answer.judge_score) as judge_score from  exam_grade 
inner join exam_answer on exam_grade.exam_grade_uid=exam_answer.exam_grade_uid
where exam_grade.paper_uid='{0}' and exam_answer.question_uid='{1}' AND
exam_grade.exam_grade_uid in (select  exam_grade_uid from (select  exam_grade.exam_grade_uid from exam_grade  inner join
exam_answer  on exam_grade.exam_grade_uid=exam_answer.exam_grade_uid where  exam_uid='{2}' and exam_grade.paper_uid='{0}' and exam_answer.question_uid='{1}'
order by exam_answer.judge_score {3} limit {4}) as t)", paperUid, questionUid, examUid, strdesc, number);

*/
            var examId = Guid.Parse(examUid);
            var paperId = Guid.Parse(paperUid);
            var questionId = Guid.Parse(questionUid);

            var q = _iExamGradeRep.GetAll()
                .Join(_iExamAnswerRep.GetAll(), examGrade => examGrade.Id, examAnswer => examAnswer.examGradeUid,
                    (examGrade, examAnswer) => new {examGrade, examAnswer})
                    .Where(a=>a.examGrade.examUid == examId && a.examGrade.paperUid == paperId && a.examAnswer.questionUid == questionId);
            
            var orderable = strdesc == "desc" ? q.OrderByDescending(a => a.examAnswer.judgeScore) : q.OrderBy(a => a.examAnswer.judgeScore);

            var queryable = orderable.Select(a => a.examGrade.Id).Take((int)number);

            var result = (from examGrade in _iExamGradeRep.GetAll()
                join examAnswer in _iExamAnswerRep.GetAll() on examGrade.Id equals examAnswer.examGradeUid
                where examGrade.paperUid == paperId
                      && examAnswer.questionUid == questionId
                      && queryable.Contains(examGrade.Id)
                select examAnswer.judgeScore).ToList();

            return result.Count > 0 ? Convert.ToDouble(result.Average()) : Convert.ToDouble(0);

        }



        public string Radom()
        {
            Random rad = new Random();
            int value = rad.Next(100000, 1000000);
            return value.ToString();
        }

        public string GetFilePath(string questionText)
        {
            var str = questionText;
            var filepath = "";
            var data = "";
            var old = "";
            var start = str.IndexOf("<vcastr><channel><item><source>");
            if (start <= -1) return questionText;
            var length = ("<vcastr><channel><item><source>").Length;
            var end = str.IndexOf("</source></item></channel></vcastr>");
            filepath = str.Substring(start + length, end - start - length);
            old = "<vcastr><channel><item><source>" + filepath + "</source></item></channel></vcastr>";
            data = "<vcastr><channel><item><source>" + filepath + "?r=" + Radom() + "</source></item></channel></vcastr>";
            str = str.Replace(old, data);
            return str;
        }

        public string ChangeImgUrl(string html, Guid questionId)
        {
            var reg = new Regex(@"<img*.+?>", RegexOptions.IgnoreCase);
            var srcReg = new Regex("src=\"\\./[0-9a-zA-Z\\-]{36}\\.[a-zA-z].+?\"");
            if (!reg.IsMatch(html))
            {
                return html;
            }
            var matchs = reg.Matches(html);
            var host = @"http://" + HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"] + @":" +
                       HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            foreach (Match match in matchs)
            {
                if (!srcReg.IsMatch(match.Value))
                {
                    continue;
                }
                var srcMatch = srcReg.Match(match.Value);
                var srcStr = srcMatch.Value;
                srcStr = srcStr.Remove(0, 7);
                srcStr = srcStr.Remove(srcStr.Length - 1);
                srcStr = host + @"/fileroot/question/" + questionId + @"/" + srcStr;
                var imgStr = match.Value.Replace(srcMatch.Value, "src=\"" + srcStr + "\"");
                html = html.Replace(match.Value, imgStr);
            }
            return html;
        }
    }
}