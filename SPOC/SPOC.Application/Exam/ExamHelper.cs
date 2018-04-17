using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Newtonsoft.Json;
using newv.common;
using SPOC.Exam.Const;
using SPOC.ExamPaper.Dto;
using SPOC.QuestionBank;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ConvertUtil = SPOC.Common.Helper.ConvertUtil;
using ReturnValue = SPOC.Common.ReturnValue;

namespace SPOC.Exam
{
    public class ExamHelper
    {
        private static string FilterQuestionAnswerForFill(string answer)
        {
            string pattern = "/[^\u4e00-\u9fa5a-zA-Z0-9à-ǜ]/g";
            answer = answer.Replace(pattern, "");
            answer = answer.ToLower();
            return answer;
        }
        /// <summary>
        /// 将考生答案数据转换为系统标准XML
        /// </summary>
        /// <param name="userAnswer"></param>
        /// <param name="gradeScore"></param>
        /// <param name="examGradeUid"></param>
        /// <param name="examUid"></param>
        /// <param name="paperUid"></param>
        /// <param name="endTime"></param>
        /// <param name="noAnswerQuestionNum"></param>
        /// <param name="iExamQuestionRep"></param>
        /// <param name="iExamPapeprNodeQuestionRep"></param>
        /// <param name="iExamJudgePolicyRep"></param>
        /// <returns></returns>
        public static string ConvertExamAnswerXml(string userAnswer, ref decimal gradeScore, Guid examGradeUid, Guid examUid, Guid paperUid, DateTime endTime, int noAnswerQuestionNum,
            IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<ExamPaperNodeQuestion, Guid> iExamPapeprNodeQuestionRep, 
            IRepository<ExamJudgePolicy, Guid> iExamJudgePolicyRep )
        {
            DataTable examAnswerTable = JsonConvert.DeserializeObject<DataTable>(userAnswer);
            StringBuilder answerBuilder = new StringBuilder();
            decimal totalScore = 0.0M;
            if (examAnswerTable != null)
            {
                //先根据paperUid 从数据库中查出 当前试卷中所有的试题
                /*IN适合于外表大而内表小的情况；EXISTS适合于外表小而内表大的情况  In确定给定的值是否与子查询或列表中的值相匹配 Exists指定一个子查询，检测行的存在*/
                //DataTable dtQuestion = DataHelper.Instance.ExecuteDataSet("SELECT * FROM exam_question  eq  WHERE EXISTS (SELECT * FROM exam_paper_node_question qpnq WHERE qpnq.question_uid=eq.question_uid AND qpnq.paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")").Tables[0];
                var questionQueryable = iExamPapeprNodeQuestionRep.GetAll()
                    .Where(a => a.paperUid == paperUid)
                    .Join(iExamQuestionRep.GetAll(), nodeQuestion => nodeQuestion.questionUid, question => question.Id,
                        (nodeQuestion, question) => question);
                //DataTable dtCustomerScore = DataHelper.Instance.ExecuteDataSet("select question_uid,paper_question_score from exam_paper_node_question where  paper_uid=" + StringUtil.QuotedToDBStr(paperUid)).Tables[0];
                var customerScoreQueryable =
                    iExamPapeprNodeQuestionRep.GetAll()
                        .Where(a => a.paperUid == paperUid)
                        .Select(a => new {a.questionUid, a.paperQuestionScore});
                //DataTable dtJudgePolicy = DataHelper.Instance.ExecuteDataSet("select question_type_uid,judge_policy_code,parameter from exam_judge_policy where exam_uid=" + StringUtil.QuotedToDBStr(examUid)).Tables[0];
                var judgePolicyQueryable =
                    iExamJudgePolicyRep.GetAll()
                        .Where(a => a.examUid == examUid)
                        .Select(a => new {a.questionTypeUid, a.judgePolicyCode, a.parameter});

                answerBuilder.Append("<?xml version = \"1.0\" encoding=\"gb2312\" standalone = \"no\"?>");
                answerBuilder.Append("<exam_grade_object>");
                answerBuilder.Append("<exam_answers>");
                var questionFilter = questionQueryable.ToList();
                var customerScoreFilter = customerScoreQueryable;
                var judgePolicyFilter = judgePolicyQueryable;
                List<string> list = new List<string>();
                foreach (DataRow row in examAnswerTable.Rows)
                {
                    if (!list.Contains(row["questionId"].ToString()))
                    {
                        list.Add(row["questionId"].ToString());
                    }
                    else
                    {
                        continue;
                    }

                    answerBuilder.Append("<exam_answer>");
                    answerBuilder.AppendFormat("<question_uid>{0}</question_uid>", row["questionId"]);
                    answerBuilder.AppendFormat("<answer_text>{0}</answer_text>", row["answer"]);
                    string answerText = row["answer"].ToString();
                    //dtQuestion.DefaultView.RowFilter = " question_uid = " + StringUtil.QuotedToDBStr(row["questionId"].ToString());
                    var questionId = new Guid(row["questionId"].ToString());
                    questionFilter = questionQueryable.Where(a => a.Id == questionId).ToList();

                    //dtCustomerScore.DefaultView.RowFilter = " question_uid = " + StringUtil.QuotedToDBStr(row["questionId"].ToString());
                    customerScoreFilter = customerScoreQueryable.Where(a => a.questionUid == questionId);

                    string standardAnswer = string.Empty;
                    Guid questionTypeUid = Guid.Empty;
                    string questionBaseTypeCode = string.Empty;
                    string judgePolicyCode = string.Empty;
                    decimal score = 0.0M;
                    if (questionFilter.Any())
                    {
                        var questionFirst = questionFilter.FirstOrDefault();
                        if (customerScoreFilter.Any())
                        {
                            questionFirst.score = customerScoreFilter.FirstOrDefault().paperQuestionScore;
                        }
                        standardAnswer = questionFirst.standardAnswer;
                        questionBaseTypeCode = questionFirst.questionBaseTypeCode;
                        questionTypeUid = questionFirst.questionTypeUid;
                        score = questionFirst.score;
                    }
                    string judgeResCode = "error";
                    decimal eachOptionScore = 0.0M;

                    #region 重构处理评分策略 减少数据库查询

                    if (judgePolicyQueryable.Any())
                    {
                        //dtJudgePolicy.DefaultView.RowFilter = " question_type_uid=" + StringUtil.QuotedToDBStr(questionTypeUid);
                        judgePolicyFilter = judgePolicyQueryable.Where(a => a.questionTypeUid == questionTypeUid);
                        if (judgePolicyFilter.Any())
                        {
                            var judgePolicyFirst = judgePolicyFilter.FirstOrDefault();
                            eachOptionScore = Convert.ToDecimal(judgePolicyFirst.parameter);
                            judgePolicyCode = judgePolicyFirst.judgePolicyCode;
                        }
                    }
                    #endregion

                    decimal judgeScore = 0.0M;

                    if (answerText != "" && answerText == standardAnswer)//标记为单选
                    {
                        judgeScore = ConvertUtil.ToDecimal(score);//ConvertUtil.ToInt(score); 
                        totalScore += judgeScore;
                        judgeResCode = "right";
                    }
                    else if (questionBaseTypeCode.ToLower() == "fill")
                    {
                        string[] arrStandardAnswer = standardAnswer.Split('|');
                        string[] arrUserAnswer = answerText.Split('|');
                        int answerNum = arrStandardAnswer.Length;
                        if (FilterQuestionAnswerForFill(standardAnswer) == FilterQuestionAnswerForFill(answerText))
                        {
                            judgeScore = ConvertUtil.ToDecimal(score);
                            totalScore += judgeScore;
                            judgeResCode = "right";
                        }
                        else
                        {
                            for (var j = 0; j < arrUserAnswer.Length; j++)
                            {
                                if (j >= arrStandardAnswer.Length) break;
                                bool isMultipleAnswers = false;
                                if (arrStandardAnswer[j].ToLower().IndexOf("&") > -1)
                                {
                                    isMultipleAnswers = true;
                                }

                                if (FilterQuestionAnswerForFill(arrUserAnswer[j]) == FilterQuestionAnswerForFill(arrStandardAnswer[j]) && !isMultipleAnswers)
                                {
                                    judgeScore += (score) / (answerNum);
                                    //  totalScore = judgeScore;
                                    if (judgeScore < 0)
                                    {
                                        judgeScore = 0;
                                    }
                                    if (judgeScore > score)////得分比实际分数高，则强行为实际分数
                                    {
                                        judgeScore = score;
                                    }
                                }
                                else
                                {
                                    //如果不是完全配配，则还要看是否是几个中任一个都行,中间用&分隔
                                    //if (arrStandardAnswer[j].ToLower().IndexOf("&") > -1)
                                    if (isMultipleAnswers)
                                    {
                                        var arrOneAnswer = arrStandardAnswer[j].Split('&');
                                        for (var k = 0; k < arrOneAnswer.Length; k++)
                                        {
                                            //只要是其中一个都得分
                                            if (FilterQuestionAnswerForFill(arrUserAnswer[j]) == FilterQuestionAnswerForFill(arrOneAnswer[k]))
                                            {
                                                judgeScore += (score) / (answerNum);

                                                if (judgeScore < 0)
                                                {
                                                    judgeScore = 0;
                                                }
                                                if (judgeScore > score)////得分比实际分数高，则强行为实际分数
                                                {
                                                    judgeScore = score;
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            totalScore += judgeScore;

                        }
                    }

                    else if (questionBaseTypeCode.ToLower() == "multi")
                    {
                        //多选题考生答案作排序
                        // answerText = this.SortUserExamAnswer(answerText);
                        standardAnswer = GetStandardAnswerFormat(standardAnswer);
                        var multiJudgePolicy = judgePolicyCode;//this.GetQuesitonTypeJudgePolicy(questionTypeUid, examUid);
                        if (multiJudgePolicy != "")
                        {
                            var arrMultiJudgePolicy = multiJudgePolicy.Split('|');
                            judgePolicyCode = arrMultiJudgePolicy[0];
                        }
                        else
                        {
                            judgePolicyCode = "";
                        }

                        string[] arrStandardAnswer;// = new string[4];
                        string[] arrUserAnswer;// = new string[4];
                        //把答案转成数组
                        if (standardAnswer.IndexOf("|") > -1)
                        {
                            arrStandardAnswer = standardAnswer.Split('|');
                        }
                        else
                        {
                            arrStandardAnswer = new string[] { standardAnswer };
                        }
                        if (string.IsNullOrEmpty(answerText))
                        {
                            judgeScore = 0;
                            judgeResCode = "";
                        }
                        if (answerText.IndexOf("|") > -1)
                        {
                            arrUserAnswer = answerText.Split('|');
                        }
                        else
                        {
                            arrUserAnswer = new string[] { answerText };
                        }
                        if (answerText == standardAnswer)
                        {
                            judgeScore = score;
                            totalScore += judgeScore;
                        }

                        #region multi_absolutely_right
                        else if (judgePolicyCode == "multi_absolutely_right")
                        {

                            //答对所有才得分
                            if (arrStandardAnswer.Length != arrUserAnswer.Length)
                            {
                                judgeScore = 0;
                            }
                            else
                            {
                                if (comp(arrStandardAnswer, arrUserAnswer, arrStandardAnswer.Length))
                                {
                                    judgeScore = score;
                                }
                                else
                                {
                                    judgeScore = 0;
                                }
                            }
                            //如果负分则为0,如果超过则为最大分
                            if (judgeScore < 0)
                                judgeScore = 0;
                            else if (judgeScore > score)
                                judgeScore = score;

                            totalScore += judgeScore;
                        }
                        #endregion
                        #region multi_incomplete_right
                        else if (judgePolicyCode == "multi_incomplete_right" && eachOptionScore > 0)
                        {
                            for (var j = 0; j < arrUserAnswer.Length; j++)
                            {
                                //答对一个得一个的分
                                if (standardAnswer.IndexOf(arrUserAnswer[j]) > -1)     //选对一个
                                    judgeScore += eachOptionScore;
                                else
                                    judgeScore = judgeScore - eachOptionScore;   //得错一个扣一个的分
                            }
                            //如果负分则为0,如果超过则为最大分
                            if (judgeScore < 0)
                                judgeScore = 0;
                            else if (judgeScore > score)
                                judgeScore = score;

                            totalScore += judgeScore;
                        }
                        #endregion

                        #region multi_partiel_right_score
                        else if (judgePolicyCode == "multi_partiel_right_score" && eachOptionScore > 0)
                        {
                            for (var j = 0; j < arrUserAnswer.Length; j++)
                            {
                                //答对一个得一个的分
                                if (standardAnswer.IndexOf(arrUserAnswer[j]) > -1)
                                    judgeScore += eachOptionScore;
                                else
                                {
                                    judgeScore = 0; //有错误选项则得0分
                                    break;
                                }
                            }
                            //如果负分则为0,如果超过则为最大分
                            if (judgeScore < 0)
                                judgeScore = 0;
                            else if (judgeScore > score)
                                judgeScore = score;

                            totalScore += judgeScore;
                        }
                        #endregion

                        #region multi_wrong_sub
                        else if (judgePolicyCode == "multi_wrong_sub" && eachOptionScore > 0)
                        {
                            var rightNum = 0;
                            var wrongNum = 0;
                            for (var j = 0; j < arrUserAnswer.Length; j++)
                            {
                                //答对一个得一个的分
                                if (standardAnswer.IndexOf(arrUserAnswer[j]) > -1)     //选对一个
                                    rightNum = rightNum + 1;
                                else
                                    wrongNum = wrongNum + 1;   //得错一个扣一个的分
                            }
                            judgeScore = (score / arrStandardAnswer.Length) * rightNum - eachOptionScore * wrongNum;

                            //如果负分则为0,如果超过则为最大分
                            if (Convert.ToInt32(judgeScore) < 0)
                                judgeScore = 0;
                            else if (judgeScore > score)
                                judgeScore = score;

                            totalScore += judgeScore;
                        }
                        #endregion

                        #region multi_partiel_right
                        else if (judgePolicyCode == "multi_partiel_right")
                        {
                            string[] arrMultiAnswerText = answerText.Split('|');
                            string[] arrMultiAtandardAnswer = standardAnswer.Split('|');
                            var multiFoundError = false;
                            for (var ma = 0; ma < arrMultiAnswerText.Length; ma++)
                            {
                                var multiIsRight = false;
                                for (var mi = 0; mi < arrMultiAtandardAnswer.Length; mi++)
                                {
                                    if (arrMultiAnswerText[ma] == arrMultiAtandardAnswer[mi])
                                    {
                                        multiIsRight = true;
                                        break;
                                    }
                                }
                                if (multiIsRight == false)
                                    multiFoundError = true;
                            }
                            if (!multiFoundError)
                            {
                                judgeScore = score * (Convert.ToDecimal(arrMultiAnswerText.Length) / arrMultiAtandardAnswer.Length);
                            }
                            totalScore += judgeScore;
                        }
                        #endregion
                    }
                    else if (questionBaseTypeCode.ToLower() == "judge")
                    {
                        var judgePolicy = judgePolicyCode;//this.GetQuesitonTypeJudgePolicy(questionTypeUid, examUid);
                        if (judgePolicy != "")
                        {
                            var arrJudgePolicy = judgePolicy.Split('|');
                            judgePolicyCode = arrJudgePolicy[0];
                            if (judgePolicyCode == "judge_right_or_wrong")
                            {
                                //做错倒扣分
                                judgeScore = 0 - score;
                                totalScore += judgeScore;
                                //judge_score = 0 - paper_question_score;
                            }
                        }
                    }
                    if (judgeScore == 0)
                    {
                        if (answerText == "")
                            judgeResCode = "";
                        else
                            judgeResCode = "error";
                    }
                    else if (judgeScore == score)
                    {
                        judgeResCode = "right";
                    }
                    else
                    {
                        judgeResCode = "middle";
                    }
                    answerBuilder.Append("<answer_time>0</answer_time>");
                    answerBuilder.AppendFormat("<judge_score>{0}</judge_score>", judgeScore);
                    answerBuilder.AppendFormat("<judge_result_code>{0}</judge_result_code>", judgeResCode);
                    answerBuilder.Append("<judge_remarks></judge_remarks>");
                    answerBuilder.Append("<is_set_bookmark>N</is_set_bookmark>");
                    answerBuilder.Append("<is_read>Y</is_read>");
                    answerBuilder.Append("</exam_answer>");
                }
                answerBuilder.Append("</exam_answers>");
                answerBuilder.AppendFormat("<grade_score>{0}</grade_score>", totalScore);
                answerBuilder.AppendFormat("<exam_grade_uid>{0}</exam_grade_uid>", examGradeUid);
                answerBuilder.AppendFormat("<last_update_time>{0}</last_update_time>", endTime);
                answerBuilder.AppendFormat("<no_answer_question_num>{0}</no_answer_question_num>", noAnswerQuestionNum);
                answerBuilder.AppendFormat("</exam_grade_object>");
                gradeScore = totalScore;
            }
            return answerBuilder.ToString();
        }

        private static string GetStandardAnswerFormat(string standardAnswer)
        {
            standardAnswer = standardAnswer.ToUpper();
            var newStandardAnswer = "";
            char[] arrCh = standardAnswer.ToCharArray();
            for (int m = 0; m < arrCh.Length; m++)
            {
                var charCode = (short)arrCh[m];
                if (charCode >= 65 && charCode <= 90)
                {
                    newStandardAnswer += arrCh[m] + "|";
                }
            }
            if (newStandardAnswer.Length > 0)
                newStandardAnswer = newStandardAnswer.Substring(0, newStandardAnswer.Length - 1);

            return newStandardAnswer;
        }

        #region 比较2个数组是否相同
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="n"></param>
        public static void sort(string[] a, int n)
        {
            int i, j;
            string t = string.Empty;
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            for (i = 0; i < n; i++)
                for (j = 0; j < n - 1 - i; j++)
                    if ((int)asciiEncoding.GetBytes(a[j])[0] < (int)asciiEncoding.GetBytes(a[j + 1])[0])
                    {
                        t = a[j];
                        a[j] = a[j + 1];
                        a[j + 1] = t;
                    }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="b"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool comp(string[] a, string[] b, int n)//n表示数组长度
        {
            sort(a, n);
            sort(b, n);
            for (int i = 0; i < n; i++)
                if (a[i] != b[i]) return false;//只要有一个元素不等，返回false
            return true;//全部相等，返回true
        }
        #endregion

        #region 提交答卷
        /// <summary>
        /// 提交答卷
        /// </summary>
        /// <param name="examGradeRow"></param>
        /// <param name="isCheckForbitSubmitBeforeTime"></param>
        /// <param name="userAnswer"></param>
        /// <param name="iUnitOfWorkManager"></param>
        /// <param name="iExamExamRep"></param>
        /// <param name="iExamGradeRep"></param>
        /// <param name="iExamAnswerRep"></param>
        /// <param name="iExamPaperNodeQuestionRep"></param>
        /// <param name="iExamQuestionRep"></param>
        /// <param name="iExamPaperNodeRep"></param>
        /// <param name="iExamQuestionTypeRep"></param>
        /// <param name="iExamJudgePolicyRep"></param>
        /// <param name="iExamPaperRep"></param>
        /// <returns></returns>
        public static ReturnValue SubmitPaper(ExamGrade examGradeRow, bool isCheckForbitSubmitBeforeTime, string userAnswer, IUnitOfWorkManager iUnitOfWorkManager, 
            IRepository<ExamExam, Guid> iExamExamRep, IRepository<ExamGrade, Guid> iExamGradeRep, IRepository<ExamAnswer, Guid> iExamAnswerRep,
            IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep, IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<ExamPaperNode, Guid> iExamPaperNodeRep,
            IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep, IRepository<ExamJudgePolicy, Guid> iExamJudgePolicyRep, IRepository<ExamPaper, Guid> iExamPaperRep)
        {
            ReturnValue retValue = new ReturnValue(false, "");
            var examGradeUid = examGradeRow.Id;

            //=========1.获取相关数据===================
            var examUid = examGradeRow.examUid;
            string gradeStatusCode = examGradeRow.gradeStatusCode;
            if (gradeStatusCode != ExamGradeStatusCodeConst.Examing && gradeStatusCode != ExamGradeStatusCodeConst.Pause)
            {
                retValue.HasError = true;
                retValue.Message = "该试卷已提交，无法再次提交！";
                return retValue;
            }

            ExamExam examExamRow = iExamExamRep.FirstOrDefault(a=>a.Id == examUid);
            if (examExamRow == null)
            {
                retValue.HasError = true;
                retValue.Message = "找不到考试记录，提交失败！";
                return retValue;
            }

            //设置了禁止提前提交时间
            int examTime;
            if (isCheckForbitSubmitBeforeTime && examExamRow.forbitSubmitBeforeTime > 0)
            {
                var endTime = examGradeRow.lastUpdateTime;
                examTime = examGradeRow.examTime == null ? 0 : examGradeRow.examTime.Value; 
                examTime += ConvertUtil.ToInt(DateTime.Now.Subtract(examGradeRow.lastUpdateTime).TotalSeconds);
                if (examTime < examExamRow.forbitSubmitBeforeTime)
                {
                    retValue.HasError = true;
                    retValue.Message = string.Format("答卷时间少于{0}分钟不允许提交试卷", ConvertUtil.ToString(examExamRow.forbitSubmitBeforeTime / 60));
                    retValue.ErrorCode = "ForbitSubmit";
                    return retValue;
                }
            }

            if (examExamRow.isRealtimeSaveAnswerToDb == "Y")
            {
                retValue = SaveUserAnswerToDbFromXML(examGradeRow, userAnswer, iUnitOfWorkManager, iExamGradeRep, iExamAnswerRep, iExamPaperNodeQuestionRep);
                if (retValue.HasError)
                {
                    examGradeRow.hasSaveAnswerToDb = "N";
                }
                else
                {
                    retValue = ExamGradeTable.AutoJudgePaper(examExamRow, examGradeRow, iUnitOfWorkManager, iExamPaperRep, iExamQuestionRep, iExamPaperNodeRep, iExamPaperNodeQuestionRep, iExamQuestionTypeRep, iExamJudgePolicyRep, iExamAnswerRep);
                }
            }
            else
            {
                examGradeRow.hasSaveAnswerToDb = "N"; //如果不直接生成答案统计数据, 把标记设置为 N
                decimal totalScore = ConvertUtil.ToDecimal(ExamGradeTable.GetFirstNodeValueFromXML(userAnswer, "grade_score"), (decimal)0);
                decimal paperTotalScore = 0;
                decimal gradeRate = 0;
                paperTotalScore = examGradeRow.paperTotalScore ?? 0m;
                //判断最高得分限制
                decimal markPaperMaxScore = examExamRow.markPaperMaxScore ?? 0m;
                if (markPaperMaxScore > 0)
                {
                    if (totalScore > markPaperMaxScore)
                    {
                        totalScore = markPaperMaxScore;
                    }
                    paperTotalScore = markPaperMaxScore;
                }

                //计算总成绩和通过状态               
                if (paperTotalScore > 0) gradeRate = totalScore * 100 / paperTotalScore;

                //==========2.改更成绩状态================
                examGradeRow.gradeScore = totalScore;
                examGradeRow.externalScore = totalScore;
                examGradeRow.gradeRate = gradeRate;
                if (examExamRow.passGradeRate > 0)
                {
                    examGradeRow.isPass = (gradeRate >= examExamRow.passGradeRate) ? "Y" : "N";
                }
                else
                {
                    examGradeRow.isPass = (totalScore >= examExamRow.passGradeScore) ? "Y" : "N";
                }
            }

            //自动评分(如果实时保存到数据库才评)
            string isNeedJudge = examExamRow.isNeedJudge;

            //闯关竞赛时要看此关是否通过
            if (examExamRow.examClassCode == EnumExamClassCode.Race)
            {
                //闯关竞赛时
                decimal thisGateGradeRate = 0;       //本关得分率
                thisGateGradeRate = GetGateGradeRate(examExamRow.gateQuestionMode, examGradeRow.paperUid, examGradeRow.passGateNum??0, userAnswer, iExamPaperNodeRep, iExamPaperNodeQuestionRep, iExamQuestionRep);

                if (thisGateGradeRate < examExamRow.gatePassGratdeRate)
                {
                    //如果没有过关
                    retValue.Message = string.Format("很报歉，您本关的得分率为[{0}%],但没有达到闯关成功要求得分率[{1}%],闯关失败。至此您已通过的总关数是{2}关。", thisGateGradeRate.ToString("0.##"), examExamRow.gatePassGratdeRate, examGradeRow.passGateNum);
                    retValue.ErrorCode = "RaceFail";

                    //更改状态为结束
                    if (isNeedJudge == "Y")
                    {
                        examGradeRow.gradeStatusCode = EnumExamGradeStatusCode.Submitted;
                    }
                    else
                    {
                        if (examExamRow.gradeReleaseType == EnumExamGradeRelease.ByHuman)
                            examGradeRow.gradeStatusCode = EnumExamGradeStatusCode.Judged;
                        else
                            examGradeRow.gradeStatusCode = EnumExamGradeStatusCode.Release;
                    }
                }
                else
                {
                    //已过关数 +1
                    examGradeRow.passGateNum += 1;
                    if (examGradeRow.passGateNum >= examExamRow.gateNum)        //如果已过关数 >= 总关数，则提交完成
                    {
                        if (isNeedJudge == "Y")
                        {
                            examGradeRow.gradeStatusCode = EnumExamGradeStatusCode.Submitted;
                        }
                        else
                        {
                            if (examExamRow.gradeReleaseType == EnumExamGradeRelease.ByHuman)
                                examGradeRow.gradeStatusCode = EnumExamGradeStatusCode.Judged;
                            else
                                examGradeRow.gradeStatusCode = EnumExamGradeStatusCode.Release;
                        }
                        retValue.Message = string.Format("恭喜您，您本关的得分率为[{0}%],已达到闯关成功要求得分率[{1}%],至此您已闯过全部的关。", thisGateGradeRate, examExamRow.gatePassGratdeRate);
                    }
                    else
                    {
                        retValue.Message = string.Format("恭喜您，您本关的得分率为[{0}%],已达到闯关成功要求得分率[{1}%],闯关成功，请点“下一关”继续闯关。", thisGateGradeRate.ToString("0.##"), examExamRow.gatePassGratdeRate);
                    }
                }
            }
            else
            {
                //非竞赛模式
                if (isNeedJudge == "Y")
                {
                    examGradeRow.gradeStatusCode = EnumExamGradeStatusCode.Submitted;
                }
                else
                {
                    if (examExamRow.gradeReleaseType == EnumExamGradeRelease.ByHuman)
                        examGradeRow.gradeStatusCode = EnumExamGradeStatusCode.Judged;
                    else
                        examGradeRow.gradeStatusCode = EnumExamGradeStatusCode.Release;
                }
            }

            //=============3.更改时间=======================

            //examGradeRow.EndTime = (examGradeRow.LastUpdateTime != "") ? examGradeRow.LastUpdateTime : DateTimeUtil.Now;
            examGradeRow.endTime = examGradeRow.lastUpdateTime;
            examTime = examGradeRow.examTime??0;
            if (examTime == 0)
                examTime = ConvertUtil.ToInt(examGradeRow.endTime.Value.Subtract(examGradeRow.beginTime).TotalSeconds);
            
            examGradeRow.examTime = examTime;
            examGradeRow.paperQuestionUids = "";
            examGradeRow.currentQuestionIndex = 0;

            //提取未答试题数
            if (!string.IsNullOrEmpty(userAnswer))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(userAnswer);
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("exam_grade_object/no_answer_question_num");
                    if (xmlNode != null)
                    {
                        examGradeRow.noAnswerQuestionNum = ConvertUtil.ToInt(xmlNode.InnerText.Trim());
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("获取未答试题数失败：" + ex.ToString());
                }
            }

            //if (newv.data.Config.DatabaseType == newv.data.EnumDatabaseType.Oracle)
            //{
            //    if (examGradeRow.UserAnswer.Length > 1000 && examGradeRow.UserAnswer.Length < 8001)
            //    {
            //        for (int i = 0; i < (8001 - examGradeRow.UserAnswer.Length); i++)
            //        {
            //            examGradeRow.UserAnswer += "   ";
            //        }
            //    }
            //}

            iExamGradeRep.Update(examGradeRow);
            iUnitOfWorkManager.Current.SaveChanges();

            Guid userUid = examGradeRow.userUid;
            //string examtypecode = ExamExamTable.GetFieldValueBy(ExamExamField.ExamUid, examUid, ExamExamField.ExamTypeCode);
            //var examtypecode = iExamExamRep.GetAll().Where(a => a.id.ToString() == examUid).Select(a => a.examTypeCode).FirstOrDefault();
            //if (examtypecode == EnumExamTypeCode.TrainExamination)
            //{
            //    string relativeUid = ExamExamRelativeTable.GetFieldValueBy(ExamExamField.ExamUid, examUid, ExamExamRelativeField.RelativeUid);
            //    if (!string.IsNullOrEmpty(relativeUid))
            //    {
            //        newv.exam.LmsInterfaceCaller.UpdateTrainLessonGrade(relativeUid, userUid);
            //        Hashtable updateDatas = new Hashtable();
            //        updateDatas.Add("exam_type_code", examtypecode);
            //        TmsInterfaceCaller.UpdateTrainLessonGrade(relativeUid, userUid, updateDatas);
            //    }
            //}
            //else if (examtypecode == EnumExamTypeCode.TrainTask)
            //{
            //    string relativeUid = ExamExamRelativeTable.GetFieldValueBy(ExamExamField.ExamUid, examUid, ExamExamRelativeField.RelativeUid);
            //    if (!string.IsNullOrEmpty(relativeUid))
            //    {
            //        Hashtable updateDatas = new Hashtable();
            //        updateDatas.Add("exam_type_code", examtypecode);
            //        TmsInterfaceCaller.UpdateTrainLessonGrade(relativeUid, userUid, updateDatas);
            //    }
            //}

            //计算积分
            //if (examExamRow.isNeedIntegral == "Y" && (examGradeRow.gradeStatusCode == EnumExamGradeStatusCode.Judged || examGradeRow.gradeStatusCode == EnumExamGradeStatusCode.Release))
            //{
            //    decimal integralNum = 0;
            //    ReturnValue integralValue = ExamGradeTable.UpdateExamIntegral(examExamRow, examGradeRow, ref integralNum);
            //    if (integralValue.HasError == true)
            //    {
            //        integralValue.HasError = true;
            //        integralValue.Message = Language.Translate("计算积分错误：") + retValue.Message;
            //        return integralValue;
            //    }
            //}
            retValue.PutValue("ExamExamRow", examExamRow);
            retValue.PutValue("ExamGrade", examGradeRow);
            return retValue;
        }

        #region GetGateGradeRate --通关 中 获取某关的得分率

        /// <summary>
        /// 获取某关的得分率
        /// </summary>
        /// <param name="gateQuestionMode"></param>
        /// <param name="paperUid"></param>
        /// <param name="passGateNum"></param>
        /// <param name="userAnswerXML"></param>
        /// <returns></returns>
        public static decimal GetGateGradeRate(string gateQuestionMode, Guid paperUid, int passGateNum, string userAnswerXML,
            IRepository<ExamPaperNode, Guid> iExamPaperNodeRep, IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep, IRepository<ExamQuestion, Guid> iExamQuestionRep)
        {
            decimal thisGatePaperTotalScore = 0;
            decimal thisGateScore = 0;
            var thisGatePaperNodeUid = Guid.Empty;           //此关大题
            string thisGateQuestionUids = "";            //此关试题Uid
            decimal thisGateGradeRate = 0;
            DataTable dtInfo = null;

            //获取本关的总分
            if (gateQuestionMode == EnumGateQuestionModeCode.OneNode)
            {
                //SearchCondition filter = new SearchCondition();
                //filter.And(ExamPaperNodeField.PaperUidT, paperUid);
                //dtInfo = ExamPaperNodeTable.SelectByFilter(filter, ExamPaperNodeField.ListOrderT);
                var info =
                    iExamPaperNodeRep.GetAll().Where(a => a.paperUid == paperUid).OrderBy(a => a.listOrder);
                if (info.Count() > passGateNum)
                {
                    var obj = info.Skip(passGateNum).Select(a => new {a.totalScore, a.Id }).FirstOrDefault();
                    thisGatePaperTotalScore = obj.totalScore;
                        //ConvertUtil.ToDecimal(dtInfo.Rows[passGateNum][ExamPaperNodeField.TotalScore]);
                    thisGatePaperNodeUid = obj.Id;
                    //dtInfo.Rows[passGateNum][ExamPaperNodeField.PaperNodeUid].ToString();
                }
                //dtInfo.Dispose();

                //string sql = "select " + ExamPaperNodeQuestionField.QuestionUidT + " from exam_paper_node_question";
                //sql += " where exam_paper_node_question.paper_node_uid=" + StringUtil.QuotedToDBStr(thisGatePaperNodeUid);
                //dtInfo = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];
                //for (int i = 0; i < dtInfo.Rows.Count; i++)
                //{
                //    thisGateQuestionUids = thisGateQuestionUids + "," + dtInfo.Rows[i][ExamPaperNodeQuestionField.QuestionUid].ToString();
                //}

                var questionUidQueryable = iExamPaperNodeRep.GetAll()
                    .Where(a => a.Id == thisGatePaperNodeUid)
                    .Select(a=>a.Id).ToList();
                questionUidQueryable.ForEach(a =>
                {
                    thisGateQuestionUids += "," + a;
                });
            }
            else
            {
                //每题一关
                //string sql = "select " + ExamPaperNodeQuestionField.PaperQuestionScoreT + "," + ExamPaperNodeQuestionField.QuestionUidT + "," + ExamQuestionField.QuestionBaseTypeCodeT + " from exam_paper_node_question inner join exam_paper_node on exam_paper_node_question.paper_node_uid=exam_paper_node.paper_node_uid ";
                //sql += " inner join exam_question on exam_paper_node_question.question_uid=exam_question.question_uid ";
                //sql += " where (exam_question.parent_question_uid is null or exam_question.parent_question_uid='') and exam_paper_node.paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + " order by exam_paper_node.list_order,exam_paper_node_question.list_order ";
                //dtInfo = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];

                var info = from nodeQuestion in iExamPaperNodeQuestionRep.GetAll()
                    join node in iExamPaperNodeRep.GetAll() on nodeQuestion.paperNodeUid equals node.Id
                           join question in iExamQuestionRep.GetAll() on nodeQuestion.questionUid equals question.Id
                           where question.parentQuestionUid == Guid.Empty && node.paperUid == paperUid
                    orderby node.listOrder, nodeQuestion.listOrder
                    select
                        new {nodeQuestion.paperQuestionScore, nodeQuestion.questionUid, question.questionBaseTypeCode};
                

                if (info.Count() > passGateNum)
                {
                    var obj = info.Skip(passGateNum).FirstOrDefault();
                    thisGatePaperTotalScore = obj.paperQuestionScore;//ConvertUtil.ToDecimal(dtInfo.Rows[passGateNum][ExamPaperNodeQuestionField.PaperQuestionScore]);
                    thisGateQuestionUids = obj.questionUid.ToString();//dtInfo.Rows[passGateNum][ExamPaperNodeQuestionField.QuestionUid].ToString();
                    if (obj.questionBaseTypeCode == EnumQuestionBaseTypeCode.Compose)
                    {
                        //如果是组合题，需提取下面的子试题
                        //sql = "select " + ExamQuestionField.QuestionUidT + " from " + ExamQuestionTable.TableName;
                        //sql += " where " + ExamQuestionField.ParentQuestionUidT + " = " + StringUtil.QuotedToDBStr(thisGateQuestionUids);
                        //dtInfo = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];
                        
                        //foreach (DataRow dataRow in dtInfo.Rows)
                        //{
                        //    thisGateQuestionUids = thisGateQuestionUids + "," + dataRow[ExamQuestionField.QuestionUid].ToString();
                        //}
                        var pqUid = Guid.Parse(thisGateQuestionUids);
                        var questionUidQueryable =
                            iExamQuestionRep.GetAll()
                                .Where(a => a.parentQuestionUid == pqUid)
                                .Select(a => a.Id);
                        thisGateQuestionUids = "";
                        foreach (var guid in questionUidQueryable)
                        {
                            thisGateQuestionUids += "," + guid;
                        }
                    }
                }
            }
            //dtInfo.Dispose();
            //dtInfo = null;

            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.LoadXml(userAnswerXML);
            }
            catch
            {
                return thisGateGradeRate;
            }

            if (xmlDocument.DocumentElement != null)
            {
                XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("exam_answers/exam_answer");
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlNode node = xmlNodeList[i];
                    string questionUid = xmlNodeList[i].SelectSingleNode("question_uid").InnerText;
                    if (("," + thisGateQuestionUids + ",").IndexOf("," + questionUid + ",") > -1)
                    {
                        thisGateScore = thisGateScore + ConvertUtil.ToDecimal(xmlNodeList[i].SelectSingleNode("judge_score").InnerText);
                    }
                }
                xmlNodeList = null;
            }
            xmlDocument = null;

            if (thisGatePaperTotalScore > 0)
            {
                thisGateGradeRate = 100 * thisGateScore / thisGatePaperTotalScore;
            }
            return thisGateGradeRate;
        }
        #endregion

        /// <summary>
        /// 保存一整块的考生答案到数据库里分题保存
        /// </summary>
        /// <returns></returns>
        public static ReturnValue SaveUserAnswerToDbFromXML(ExamGrade examGradeRow, string userAnswer,IUnitOfWorkManager iUnitOfWorkManager, IRepository<ExamGrade, Guid> iExamGradeRep,
            IRepository<ExamAnswer, Guid> iExamAnswerRep, IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep)
        {
            ReturnValue retValue = new ReturnValue();
            XmlNode tempXmlNode;
            var examGradeUid = Guid.Empty;

            string userAnswerXML = userAnswer;
            if (string.IsNullOrEmpty(userAnswerXML))
            {
                examGradeRow.hasSaveAnswerToDb = "Y";
                iExamGradeRep.Update(examGradeRow);
                iUnitOfWorkManager.Current.SaveChanges();
                return retValue;
            }

            XmlDocument xmlDocument = new XmlDocument();
            try
            {

                xmlDocument.LoadXml(userAnswerXML);
            }
            catch (Exception e)
            {
                try
                {
                    userAnswerXML = ReplaceLowOrder(userAnswerXML);
                    xmlDocument.LoadXml(userAnswerXML);
                }
                catch (Exception)
                {
                    retValue.HasError = true;
                    retValue.Message = "考生答案信息格式有误.";
                    return retValue;
                }

            }

            XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("exam_answers/exam_answer");

            tempXmlNode = xmlDocument.DocumentElement.SelectSingleNode("exam_grade_uid");
            if (tempXmlNode != null) examGradeUid = Guid.Parse(tempXmlNode.InnerText);

            if (examGradeUid == Guid.Empty)
            {
                retValue.HasError = true;
                retValue.Message = "考生答案信息格式有误.";
                return retValue;
            }

            //清除考生答案
            iExamAnswerRep.Delete(a=>a.examGradeUid == examGradeUid);
            iUnitOfWorkManager.Current.SaveChanges();
            //重新生成考生答案
            string questionUid = string.Empty;
            string answerText = string.Empty;
            decimal judgeScore = 0;
            string judgeResultCode = string.Empty;
            ExamAnswer examAnswerRow = null;
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                questionUid = string.Empty;
                answerText = string.Empty;
                judgeScore = 0;
                judgeResultCode = string.Empty;
                tempXmlNode = xmlNode.SelectSingleNode("question_uid");
                if (tempXmlNode != null) questionUid = tempXmlNode.InnerText;


                tempXmlNode = xmlNode.SelectSingleNode("answer_text");
                if (tempXmlNode != null) answerText = tempXmlNode.InnerText;


                tempXmlNode = xmlNode.SelectSingleNode("judge_score");
                if (tempXmlNode != null) judgeScore = ConvertUtil.ToDecimal(tempXmlNode.InnerText);


                tempXmlNode = xmlNode.SelectSingleNode("judge_result_code");
                if (tempXmlNode != null) judgeResultCode = tempXmlNode.InnerText;

                examAnswerRow = new ExamAnswer();
                examAnswerRow.examGradeUid = examGradeUid;
                examAnswerRow.questionUid = new Guid(questionUid);
                examAnswerRow.answerText = answerText;
                examAnswerRow.judgeScore = judgeScore;
                examAnswerRow.judgeResultCode = judgeResultCode;
                iExamAnswerRep.Insert(examAnswerRow);
                iUnitOfWorkManager.Current.SaveChanges();
            }

            //将没有回答的题目也写入答案表
            //SearchCondition filter = new SearchCondition();
            //filter.And(ExamAnswerField.ExamGradeUid, examGradeUid);
            //DataTable dtExamAnswer = ExamAnswerTable.SelectByFilter(filter);
            var examAnswerQueryable = iExamAnswerRep.GetAll().Where(a => a.examGradeUid == examGradeUid);
            Hashtable ht = new Hashtable();
            var examPaperQuestionUid = Guid.Empty;
            foreach (var examAnswer in examAnswerQueryable)
            {
                examPaperQuestionUid = examAnswer.questionUid;
                if (!ht.ContainsKey(examPaperQuestionUid))
                {
                    ht.Add(examPaperQuestionUid, examPaperQuestionUid);
                } 
            }
            
            //filter = new SearchCondition();
            //filter.And(ExamPaperNodeField.PaperUid, examGradeRow.PaperUid);
            //DataTable dtExamPaperNodeQuestion = ExamPaperNodeQuestionTable.SelectByFilter(filter);
            var nodeQuestionQueryable =
                iExamPaperNodeQuestionRep.GetAll().Where(a => a.paperUid == examGradeRow.paperUid);
            foreach (var nodeQuestion in nodeQuestionQueryable)
            {
                examPaperQuestionUid = nodeQuestion.questionUid;
                if (!ht.ContainsKey(examPaperQuestionUid))
                {
                    examAnswerRow = new ExamAnswer();
                    examAnswerRow.examGradeUid = examGradeUid;
                    examAnswerRow.questionUid = examPaperQuestionUid;
                    examAnswerRow.answerText = string.Empty;
                    examAnswerRow.judgeScore = 0;
                    examAnswerRow.judgeResultCode = string.Empty;
                    iExamAnswerRep.Insert(examAnswerRow);
                    iUnitOfWorkManager.Current.SaveChanges();

                    ht.Add(examPaperQuestionUid, examPaperQuestionUid);
                }
            }
            //dtExamAnswer = null;
            //dtExamPaperNodeQuestion = null;

            examGradeRow.hasSaveAnswerToDb = "Y";
            return new ReturnValue();
        }

        public static string ReplaceLowOrder(string tmp)
        {
            StringBuilder info = new StringBuilder();
            foreach (char cc in tmp)
            {
                int ss = (int)cc;
                if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                    info.AppendFormat(" ", ss);//&#x{0:X}; 
                else info.Append(cc);
            }
            return info.ToString();
        } 
        #endregion
    }

    class ExamGradeTable
    {
        
        /// <summary>
        /// 自动评分(更新到库中)
        /// </summary>
        /// <param name="examExamRow"></param>
        /// <param name="examGradeRow"></param>
        /// <param name="iUnitOfWorkManager"></param>
        /// <param name="iExamPaperRep"></param>
        /// <param name="iExamQuestionRep"></param>
        /// <param name="iExamPaperNodeRep"></param>
        /// <param name="iExamPaperNodeQuestionRep"></param>
        /// <param name="iExamQuestionTypeRep"></param>
        /// <param name="iExamJudgePolicyRep"></param>
        /// <param name="iExamAnswerRep"></param>
        /// <returns></returns>
        public static ReturnValue AutoJudgePaper(ExamExam examExamRow, ExamGrade examGradeRow, IUnitOfWorkManager iUnitOfWorkManager,
            IRepository<ExamPaper, Guid> iExamPaperRep, IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<ExamPaperNode, Guid> iExamPaperNodeRep,
            IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep, IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep,
            IRepository<ExamJudgePolicy, Guid> iExamJudgePolicyRep, IRepository<ExamAnswer, Guid> iExamAnswerRep)
        {

            return AutoJudgePaper(examExamRow, examGradeRow, null, iExamPaperRep, iExamQuestionRep,iExamPaperNodeRep,iExamPaperNodeQuestionRep,iExamQuestionTypeRep,iExamJudgePolicyRep,iExamAnswerRep);
        }


        
        /// <summary>
        /// 自动评分(不更新examGradeRow对象到数据库中)
        /// </summary>
        /// <param name="examExamRow"></param>
        /// <param name="examGradeRow"></param>
        /// <param name="updateQuestionUid"></param>
        /// <param name="iUnitOfWorkManager"></param>
        /// <param name="iExamPaperRep"></param>
        /// <param name="iExamQuestionRep"></param>
        /// <param name="iExamPaperNodeRep"></param>
        /// <param name="iExamPaperNodeQuestionRep"></param>
        /// <param name="iExamQuestionTypeRep"></param>
        /// <param name="iExamJudgePolicyRep"></param>
        /// <param name="iExamAnswerRep"></param>
        /// <returns></returns>
        public static ReturnValue AutoJudgePaper(ExamExam examExamRow, ExamGrade examGradeRow, Guid updateQuestionUid, IUnitOfWorkManager iUnitOfWorkManager,
            IRepository<ExamPaper, Guid> iExamPaperRep, IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<ExamPaperNode, Guid> iExamPaperNodeRep,
            IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep, IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep,
            IRepository<ExamJudgePolicy, Guid> iExamJudgePolicyRep, IRepository<ExamAnswer, Guid> iExamAnswerRep)
        {
            ReturnValue retValue = new ReturnValue();

            var examGradeUid = examGradeRow.Id;

            var paperUid = examGradeRow.paperUid;
            //string paperQuestionTableName = ExamPaperNodeQuestionTable.GetSplitTableName(paperUid);
            //ExamPaperRow examPaperRow = ExamPaperTable.CreateRowBy(paperUid);
            var examPaperRow = iExamPaperRep.FirstOrDefault(examGradeRow.paperUid);
            if (examPaperRow == null)
            {
                retValue.HasError = true;
                retValue.Message = "找不到试卷信息，提交失败！";
                return retValue;
            }

            //是否做错题扣分
            bool isDeductScore = examExamRow.isDeductScoreWhenError == "Y" && examExamRow.examDoModeCode == "question";

            //============2.自动评分=========
            string sql = "";

            //取得题型列表
//            sql = @"select distinct " + ExamQuestionTypeTable.GetAllFieldSelectString(true) + @" from " + paperQuestionTableName + @"
//                    inner join exam_question on " + paperQuestionTableName + @".question_uid=exam_question.question_uid 
//                    inner join exam_question_type on exam_question_type.question_type_uid=exam_question.question_type_uid 
//                    where " + paperQuestionTableName + @".paper_uid=" + StringUtil.QuotedToDBStr(paperUid);
//            if (updateQuestionUid != null)
//            {
//                sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
//            }

            var questionTypeQueryable = from nodeQuestion in iExamPaperNodeQuestionRep.GetAll()
                join questionType in iExamQuestionTypeRep.GetAll() on nodeQuestion.Question.questionTypeUid equals questionType.Id
                where nodeQuestion.paperUid == examGradeRow.paperUid && (updateQuestionUid != Guid.Empty ? nodeQuestion.questionUid == updateQuestionUid : true)
                select questionType;

            //DataTable dtQuestionType = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];
            //ExamQuestionType examQuestionTypeRow;
            DataTable dtExamAnswer;
            string[] arrUserAnswer;
            string[] arrStandardAnswer;
            string[] arrSelectAnswerScore;
            string standardAnswer;
            string userAnswer;
            Guid questionUid;
            decimal paperQuestionScore;
            string selectAnswerScore;
            int answerNum;
            decimal judgeScore = 0;
            string judgeResultCode = "";
            decimal eachOptionScore = 0m;
            //string examAnswerTableName = ExamAnswerTable.GetSplitTableName(examGradeUid);
            //Hashtable htJudgePara;
            //for (int questionTypeCount = 0; questionTypeCount < dtQuestionType.Rows.Count; questionTypeCount++)
            foreach (var examQuestionTypeRow in questionTypeQueryable)
            {
                //examQuestionTypeRow = new ExamQuestionType();
                //examQuestionTypeRow.AssignByDataRow(dtQuestionType.Rows[questionTypeCount]);
                if (examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Single || examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Multi || examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Judge)
                {
                    #region 对单选、多选、判断进行自动评分

                    bool isJudgeByRightOrWrong = false;
                    if (examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Judge)
                    {
                        //ExamJudgePolicy examJudgePolicy = ExamJudgePolicyTable.CreateRowBy(examExamRow.ExamUid, examQuestionTypeRow.QuestionTypeUid);
                        var examJudgePolicy =
                            iExamJudgePolicyRep.FirstOrDefault(
                                a => a.examUid == examExamRow.Id && a.questionTypeUid == examQuestionTypeRow.Id);
                        if (examJudgePolicy != null && examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Judge && examJudgePolicy.judgePolicyCode == EnumExamJudgePolicy.JudgeByRightOrWrong)
                        {
                            isJudgeByRightOrWrong = true;
                        }
                    }
                    //正确的得分
                    //sql = " Update " + examAnswerTableName + " set " + examAnswerTableName + ".judge_result_code='" + EnumJudgeResultCode.Right + "'," + examAnswerTableName + ".judge_score=" + paperQuestionTableName + ".paper_question_score";
                    //sql += " from " + examAnswerTableName + " inner join " + paperQuestionTableName + " on (" + examAnswerTableName + ".question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                    //sql += "  inner join exam_question on " + examAnswerTableName + ".question_uid=exam_question.question_uid ";
                    //sql += " where " + examAnswerTableName + ".answer_text=exam_question.standard_answer and exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                    //if (updateQuestionUid != null)
                    //{
                    //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                    //}

                    //DataHelper.Instance.ExecuteNonQuery(sql); 
                    var answerQueryable = from answer in iExamAnswerRep.GetAll()
                        join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                            nodeQuestion.questionUid
                        where
                            nodeQuestion.paperUid == paperUid &&
                            answer.answerText == answer.Question.standardAnswer
                            && answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                            answer.examGradeUid == examGradeUid
                            && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                        select new {answer, nodeQuestion.paperQuestionScore};

                    foreach (var a in answerQueryable)
                    {
                        a.answer.judgeResultCode = EnumJudgeResultCode.Right;
                        a.answer.judgeScore = a.paperQuestionScore;
                        iExamAnswerRep.UpdateAsync(a.answer);
                    }

                    iUnitOfWorkManager.Current.SaveChanges();
                    
                    var answerQueryable2 = from answer in iExamAnswerRep.GetAll()
                        join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                            nodeQuestion.questionUid
                        where
                            nodeQuestion.paperUid == paperUid &&
                            answer.answerText != answer.Question.standardAnswer
                            && answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                            answer.examGradeUid == examGradeUid
                            && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                        select new {answer, nodeQuestion.paperQuestionScore};
                    if (!isDeductScore)
                    {
                        if (isJudgeByRightOrWrong)
                        {
                            //做错题倒扣分
                            //sql = " Update " + examAnswerTableName + " set judge_result_code='" + EnumJudgeResultCode.Error + "',judge_score=(0-" + paperQuestionTableName + ".paper_question_score)";
                            
                            foreach (var a in answerQueryable2)
                            {
                                a.answer.judgeResultCode = EnumJudgeResultCode.Error;
                                a.answer.judgeScore = 0 - a.paperQuestionScore;
                                iExamAnswerRep.UpdateAsync(a.answer);
                            }
                        }
                        else
                        {
                            //不正确的不得分
                            //sql = " Update " + examAnswerTableName + " set judge_result_code='" + EnumJudgeResultCode.Error + "',judge_score=0";

                            foreach (var a in answerQueryable2)
                            {
                                a.answer.judgeResultCode = EnumJudgeResultCode.Error;
                                a.answer.judgeScore = 0;
                                iExamAnswerRep.UpdateAsync(a.answer);
                            }
                        }
                        //sql += " from " + examAnswerTableName + " inner join " + paperQuestionTableName + " on (" + examAnswerTableName + ".question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                        //sql += " inner join exam_question on " + examAnswerTableName + ".question_uid=exam_question.question_uid ";
                        //sql += " where " + examAnswerTableName + ".answer_text<>exam_question.standard_answer and exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                        
                        //if (updateQuestionUid != null)
                        //{
                        //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                        //}
                        //DataHelper.Instance.ExecuteNonQuery(sql);
                    }
                    else
                    {
                        //做错题要扣分
                        //sql = " Update " + examAnswerTableName + " set judge_result_code='" + EnumJudgeResultCode.Error + "',judge_score=(0-" + paperQuestionTableName + ".paper_question_score)";
                        //sql += " from " + examAnswerTableName + " inner join " + paperQuestionTableName + " on (" + examAnswerTableName + ".question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                        //sql += "  inner join exam_question on " + examAnswerTableName + ".question_uid=exam_question.question_uid ";
                        //sql += " where " + examAnswerTableName + ".answer_text<>exam_question.standard_answer and exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                        //if (updateQuestionUid != null)
                        //{
                        //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                        //}
                        //DataHelper.Instance.ExecuteNonQuery(sql);

                        foreach (var a in answerQueryable2)
                        {
                            a.answer.judgeResultCode = EnumJudgeResultCode.Error;
                            a.answer.judgeScore = 0 - a.paperQuestionScore;
                            iExamAnswerRep.UpdateAsync(a.answer);
                        }
                    }
                    iUnitOfWorkManager.Current.SaveChanges();
                
                    #endregion

                    //对多选题进行评分策略进行评分
                    #region 对多选题进行评分策略进行评分
                    if (examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Multi)
                    {
                        //提取评分策略
                        //ExamJudgePolicyRow multiJudgePolicy = ExamJudgePolicyTable.CreateRowBy(examExamRow.ExamUid, examQuestionTypeRow.QuestionTypeUid);
                        var multiJudgePolicy =
                            iExamJudgePolicyRep.FirstOrDefault(
                                a => a.examUid == examExamRow.Id && a.questionTypeUid == examQuestionTypeRow.Id);
                        //不完全正确时(有错误选项,也有正确选项),每项的得扣分
                        if (multiJudgePolicy != null && multiJudgePolicy.judgePolicyCode == EnumExamJudgePolicy.MultiByIncompleteRight)
                        {
                            decimal optionScore = ConvertUtil.ToDecimal(multiJudgePolicy.parameter, 0);
                            if (optionScore > 0)
                            {
                                //sql = "select " + ExamAnswerTable.GetAllFieldSelectString(true) + "," + paperQuestionTableName + ".paper_question_score,exam_question.standard_answer,exam_question.answer_num,exam_question.select_answer_score ";
                                //sql += " from " + examAnswerTableName + " exam_answer inner join " + paperQuestionTableName + " on (exam_answer.question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                                //sql += "  inner join exam_question on exam_answer.question_uid=exam_question.question_uid ";
                                //sql += " where exam_answer.answer_text<>'' and exam_answer.judge_result_code=" + StringUtil.QuotedToDBStr(EnumJudgeResultCode.Error) + " and exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and exam_answer.exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                                //if (updateQuestionUid != null)
                                //{
                                //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                                //}
                                //dtExamAnswer = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];

                                var answerQueryable3 = from answer in iExamAnswerRep.GetAll()
                                    join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                                        nodeQuestion.questionUid
                                    where
                                        nodeQuestion.paperUid == paperUid &&
                                        !string.IsNullOrEmpty(answer.answerText) &&
                                        answer.judgeResultCode == EnumJudgeResultCode.Error &&
                                        answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                                        answer.examGradeUid == examGradeUid
                                        && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                                    select new
                                    {
                                        answer,
                                        nodeQuestion.paperQuestionScore,
                                        answer.Question.standardAnswer,
                                        answer.Question.selectAnswerScore,
                                        answer.Question.answerNum
                                    };

                                //for (int i = 0; i < dtExamAnswer.Rows.Count; i++)
                                foreach(var examAnswer in answerQueryable3)
                                {
                                    standardAnswer = examAnswer.standardAnswer;
                                    standardAnswer = QuestionUtil.FormatStanderdAnswerForMulti(standardAnswer);
                                    userAnswer = examAnswer.answer.answerText;
                                    questionUid = examAnswer.answer.questionUid;// dtExamAnswer.Rows[i]["question_uid"].ToString();
                                    paperQuestionScore = examAnswer.paperQuestionScore;
                                    judgeScore = 0;
                                    judgeResultCode = EnumJudgeResultCode.Right;
                                    //计算得分
                                    arrStandardAnswer = standardAnswer.Split('|');
                                    arrUserAnswer = userAnswer.Split('|');
                                    for (int j = 0; j < arrUserAnswer.Length; j++)
                                    {
                                        //答对一个得一个的分
                                        if (standardAnswer.IndexOf(arrUserAnswer[j]) > -1)     //选对一个
                                            judgeScore += optionScore;
                                        else
                                            judgeScore = judgeScore - optionScore;   //得错一个扣一个的分
                                    }
                                    //如果负分则为0,如果超过则为最大分
                                    if (judgeScore < 0)
                                        judgeScore = 0;
                                    else if (judgeScore > paperQuestionScore)
                                        judgeScore = paperQuestionScore;


                                    //处理做错题扣分的情况,当前试题分数为0时,说明答案不正确
                                    if (judgeScore == 0 && isDeductScore)
                                    {
                                        judgeScore = 0 - paperQuestionScore;
                                    }

                                    if (judgeScore <= 0)
                                        judgeResultCode = EnumJudgeResultCode.Error;
                                    else if (judgeScore.ToString("#.##") == paperQuestionScore.ToString("#.##"))
                                        judgeResultCode = EnumJudgeResultCode.Right;
                                    else
                                        judgeResultCode = EnumJudgeResultCode.Middle;
                                    //sql = " Update " + examAnswerTableName + " set " + examAnswerTableName + ".judge_result_code='" + judgeResultCode + "'," + examAnswerTableName + ".judge_score=" + judgeScore.ToString();
                                    //sql += " where " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid) + " and " + examAnswerTableName + ".question_uid=" + StringUtil.QuotedToDBStr(questionUid);
                                    //DataHelper.Instance.ExecuteNonQuery(sql);

                                    var answerQueryable4 =
                                        iExamAnswerRep.GetAll()
                                            .Where(
                                                a =>
                                                    a.examGradeUid == examGradeUid &&
                                                    a.questionUid == questionUid);
                                    foreach (var a in answerQueryable4)
                                    {
                                        a.judgeResultCode = judgeResultCode;
                                        a.judgeScore = judgeScore;
                                        iExamAnswerRep.UpdateAsync(a);
                                    }
                                    iUnitOfWorkManager.Current.SaveChanges();
                                }
                            }
                        }
                        else if (multiJudgePolicy != null && multiJudgePolicy.judgePolicyCode == EnumExamJudgePolicy.MultiByPartialRightScore)
                        {
                            decimal optionScore = ConvertUtil.ToDecimal(multiJudgePolicy.parameter, 0);
                            if (optionScore > 0)
                            {
                                //sql = "select " + ExamAnswerTable.GetAllFieldSelectString(true) + "," + paperQuestionTableName + ".paper_question_score,exam_question.standard_answer,exam_question.answer_num,exam_question.select_answer_score ";
                                //sql += " from " + examAnswerTableName + " exam_answer inner join " + paperQuestionTableName + " on (exam_answer.question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                                //sql += "  inner join exam_question on exam_answer.question_uid=exam_question.question_uid ";
                                //sql += " where exam_answer.answer_text<>'' and exam_answer.judge_result_code=" + StringUtil.QuotedToDBStr(EnumJudgeResultCode.Error) + " and exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and exam_answer.exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                                //if (updateQuestionUid != null)
                                //{
                                //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                                //}
                                //dtExamAnswer = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];

                                var answerQueryable3 = from answer in iExamAnswerRep.GetAll()
                                                       join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                                                           nodeQuestion.questionUid
                                                       where
                                                           nodeQuestion.paperUid == paperUid &&
                                                           !string.IsNullOrEmpty(answer.answerText) &&
                                                           answer.judgeResultCode == EnumJudgeResultCode.Error &&
                                                           answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                                                           answer.examGradeUid == examGradeUid
                                                           && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                                                       select new
                                                       {
                                                           answer,
                                                           nodeQuestion.paperQuestionScore,
                                                           answer.Question.standardAnswer,
                                                           answer.Question.selectAnswerScore,
                                                           answer.Question.answerNum
                                                       };
                                //for (int i = 0; i < dtExamAnswer.Rows.Count; i++)
                                foreach (var examAnswer in answerQueryable3)
                                {
                                    standardAnswer = examAnswer.standardAnswer;
                                    standardAnswer = QuestionUtil.FormatStanderdAnswerForMulti(standardAnswer);
                                    userAnswer = examAnswer.answer.answerText;
                                    questionUid = examAnswer.answer.questionUid;// dtExamAnswer.Rows[i]["question_uid"].ToString();
                                    paperQuestionScore = examAnswer.paperQuestionScore;
                                    judgeScore = 0;
                                    judgeResultCode = EnumJudgeResultCode.Right;
                                    //计算得分
                                    arrStandardAnswer = standardAnswer.Split('|');
                                    arrUserAnswer = userAnswer.Split('|');

                                    for (int j = 0; j < arrUserAnswer.Length; j++)
                                    {
                                        //答对一个得一个的分
                                        if (standardAnswer.IndexOf(arrUserAnswer[j]) > -1)     //选对一个
                                            judgeScore += optionScore;
                                        else
                                        {
                                            judgeScore = 0;
                                            break;
                                        }
                                    }
                                    //如果负分则为0,如果超过则为最大分
                                    if (judgeScore < 0)
                                        judgeScore = 0;
                                    else if (judgeScore > paperQuestionScore)
                                        judgeScore = paperQuestionScore;


                                    //处理做错题扣分的情况,当前试题分数为0时,说明答案不正确
                                    if (judgeScore == 0 && isDeductScore)
                                    {
                                        judgeScore = 0 - paperQuestionScore;
                                    }

                                    if (judgeScore <= 0)
                                        judgeResultCode = EnumJudgeResultCode.Error;
                                    else if (judgeScore.ToString("#.##") == paperQuestionScore.ToString("#.##"))
                                        judgeResultCode = EnumJudgeResultCode.Right;
                                    else
                                        judgeResultCode = EnumJudgeResultCode.Middle;
                                    //sql = " Update " + examAnswerTableName + " set " + examAnswerTableName + ".judge_result_code='" + judgeResultCode + "'," + examAnswerTableName + ".judge_score=" + judgeScore.ToString();
                                    //sql += " where " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid) + " and " + examAnswerTableName + ".question_uid=" + StringUtil.QuotedToDBStr(questionUid);
                                    //DataHelper.Instance.ExecuteNonQuery(sql);
                                    var answerQueryable4 =
                                        iExamAnswerRep.GetAll()
                                            .Where(
                                                a =>
                                                    a.examGradeUid == examGradeUid &&
                                                    a.questionUid == questionUid);
                                    foreach (var a in answerQueryable4)
                                    {
                                        a.judgeResultCode = judgeResultCode;
                                        a.judgeScore = judgeScore;
                                        iExamAnswerRep.UpdateAsync(a);
                                    }
                                    iUnitOfWorkManager.Current.SaveChanges();
                                }
                            }
                        }
                        else if (multiJudgePolicy != null && multiJudgePolicy.judgePolicyCode == EnumExamJudgePolicy.MultiByWrongSub)
                        {
                            decimal optionScore = ConvertUtil.ToDecimal(multiJudgePolicy.parameter, 0);
                            if (optionScore > 0)
                            {
                                //sql = "select " + ExamAnswerTable.GetAllFieldSelectString(true) + "," + paperQuestionTableName + ".paper_question_score,exam_question.standard_answer,exam_question.answer_num,exam_question.select_answer_score ";
                                //sql += " from " + examAnswerTableName + " exam_answer inner join " + paperQuestionTableName + " on (exam_answer.question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                                //sql += "  inner join exam_question on exam_answer.question_uid=exam_question.question_uid ";
                                //sql += " where exam_answer.answer_text<>'' and exam_answer.judge_result_code=" + StringUtil.QuotedToDBStr(EnumJudgeResultCode.Error) + " and exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and exam_answer.exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                                //if (updateQuestionUid != null)
                                //{
                                //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                                //}
                                //dtExamAnswer = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];
                                var answerQueryable3 = from answer in iExamAnswerRep.GetAll()
                                                       join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                                                           nodeQuestion.questionUid
                                                       where
                                                           nodeQuestion.paperUid == paperUid &&
                                                           !string.IsNullOrEmpty(answer.answerText) &&
                                                           answer.judgeResultCode == EnumJudgeResultCode.Error &&
                                                           answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                                                           answer.examGradeUid == examGradeUid
                                                           && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                                                       select new
                                                       {
                                                           answer,
                                                           nodeQuestion.paperQuestionScore,
                                                           answer.Question.standardAnswer,
                                                           answer.Question.selectAnswerScore,
                                                           answer.Question.answerNum
                                                       };
                                //for (int i = 0; i < dtExamAnswer.Rows.Count; i++)
                                foreach (var examAnswer in answerQueryable3)
                                {
                                    standardAnswer = examAnswer.standardAnswer;
                                    standardAnswer = QuestionUtil.FormatStanderdAnswerForMulti(standardAnswer);
                                    userAnswer = examAnswer.answer.answerText;
                                    questionUid = examAnswer.answer.questionUid;// dtExamAnswer.Rows[i]["question_uid"].ToString();
                                    paperQuestionScore = examAnswer.paperQuestionScore;
                                    judgeScore = 0;
                                    judgeResultCode = EnumJudgeResultCode.Right;
                                    arrStandardAnswer = standardAnswer.Split('|');
                                    arrUserAnswer = userAnswer.Split('|');

                                    int rightNum = 0;
                                    int wrongNum = 0;
                                    for (int j = 0; j < arrUserAnswer.Length; j++)
                                    {
                                        if (standardAnswer.IndexOf(arrUserAnswer[j]) > -1)     //选对一个
                                            rightNum = rightNum + 1;
                                        else
                                            wrongNum = wrongNum + 1;   //得错一个扣一个的分
                                    }

                                    judgeScore = (paperQuestionScore / arrStandardAnswer.Length) * rightNum - optionScore * wrongNum;

                                    //如果负分则为0,如果超过则为最大分
                                    if (judgeScore < 0)
                                        judgeScore = 0;
                                    else if (judgeScore > paperQuestionScore)
                                        judgeScore = paperQuestionScore;


                                    //处理做错题扣分的情况,当前试题分数为0时,说明答案不正确
                                    if (judgeScore == 0 && isDeductScore)
                                    {
                                        judgeScore = 0 - paperQuestionScore;
                                    }

                                    if (judgeScore <= 0)
                                        judgeResultCode = EnumJudgeResultCode.Error;
                                    else if (judgeScore.ToString("#.##") == paperQuestionScore.ToString("#.##"))
                                        judgeResultCode = EnumJudgeResultCode.Right;
                                    else
                                        judgeResultCode = EnumJudgeResultCode.Middle;
                                    //sql = " Update " + examAnswerTableName + " set " + examAnswerTableName + ".judge_result_code='" + judgeResultCode + "'," + examAnswerTableName + ".judge_score=" + judgeScore.ToString();
                                    //sql += " where " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid) + " and " + examAnswerTableName + ".question_uid=" + StringUtil.QuotedToDBStr(questionUid);
                                    //DataHelper.Instance.ExecuteNonQuery(sql);

                                    var answerQueryable4 =
                                        iExamAnswerRep.GetAll()
                                            .Where(
                                                a =>
                                                    a.examGradeUid == examGradeUid &&
                                                    a.questionUid == questionUid);
                                    foreach (var a in answerQueryable4)
                                    {
                                        a.judgeResultCode = judgeResultCode;
                                        a.judgeScore = judgeScore;
                                        iExamAnswerRep.UpdateAsync(a);
                                    }
                                    iUnitOfWorkManager.Current.SaveChanges();

                                }
                            }
                        }

                        else if (multiJudgePolicy != null && multiJudgePolicy.judgePolicyCode == EnumExamJudgePolicy.MultiByPartialRight)
                        {
                            //部分正确(没有错误选项),按正确选项的比率得分, 用于多选题
                            //sql = "select " + ExamAnswerTable.GetAllFieldSelectString(true) + "," + paperQuestionTableName + ".paper_question_score,exam_question.standard_answer,exam_question.answer_num,exam_question.select_answer_score ";
                            //sql += " from " + examAnswerTableName + " exam_answer inner join " + paperQuestionTableName + " on (exam_answer.question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                            //sql += "  inner join exam_question on exam_answer.question_uid=exam_question.question_uid ";
                            //sql += " where exam_answer.answer_text<>'' and exam_answer.judge_result_code=" + StringUtil.QuotedToDBStr(EnumJudgeResultCode.Error) + " and exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and exam_answer.exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                            //if (updateQuestionUid != null)
                            //{
                            //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                            //}

                            //dtExamAnswer = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];
                            var answerQueryable3 = from answer in iExamAnswerRep.GetAll()
                                                   join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                                                       nodeQuestion.questionUid
                                                   where
                                                       nodeQuestion.paperUid == paperUid &&
                                                       !string.IsNullOrEmpty(answer.answerText) &&
                                                       answer.judgeResultCode == EnumJudgeResultCode.Error &&
                                                       answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                                                       answer.examGradeUid == examGradeUid
                                                       && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                                                   select new
                                                   {
                                                       answer,
                                                       nodeQuestion.paperQuestionScore,
                                                       answer.Question.standardAnswer,
                                                       answer.Question.selectAnswerScore,
                                                       answer.Question.answerNum
                                                   };
                            //for (int i = 0; i < dtExamAnswer.Rows.Count; i++)
                            foreach (var examAnswer in answerQueryable3)
                            {
                                standardAnswer = examAnswer.standardAnswer;
                                standardAnswer = QuestionUtil.FormatStanderdAnswerForMulti(standardAnswer);
                                userAnswer = examAnswer.answer.answerText;
                                questionUid = examAnswer.answer.questionUid;// dtExamAnswer.Rows[i]["question_uid"].ToString();
                                paperQuestionScore = examAnswer.paperQuestionScore;
                                judgeScore = 0;
                                judgeResultCode = EnumJudgeResultCode.Right;
                                arrStandardAnswer = standardAnswer.Split('|');
                                arrUserAnswer = userAnswer.Split('|');
                                bool multiFoundError = false;
                                for (int ma = 0; ma < arrUserAnswer.Length; ma++)
                                {
                                    bool multiIsRight = false;
                                    for (int mi = 0; mi < arrStandardAnswer.Length; mi++)
                                    {
                                        if (arrUserAnswer[ma] == arrStandardAnswer[mi])
                                        {
                                            multiIsRight = true;
                                            break;
                                        }
                                    }
                                    if (!multiIsRight)
                                    {
                                        multiFoundError = true;
                                        break;
                                    }
                                }
                                if (!multiFoundError)
                                {
                                    judgeScore = paperQuestionScore * arrUserAnswer.Length / arrStandardAnswer.Length;
                                }

                                //处理做错题扣分的情况,当前试题分数为0时,说明答案不正确
                                if (judgeScore == 0 && isDeductScore)
                                {
                                    judgeScore = 0 - paperQuestionScore;
                                }

                                if (judgeScore <= 0)
                                    judgeResultCode = EnumJudgeResultCode.Error;
                                else if (judgeScore.ToString("#.##") == paperQuestionScore.ToString("#.##"))
                                    judgeResultCode = EnumJudgeResultCode.Right;
                                else
                                    judgeResultCode = EnumJudgeResultCode.Middle;
                                //sql = " Update " + examAnswerTableName + " set " + examAnswerTableName + ".judge_result_code='" + judgeResultCode + "'," + examAnswerTableName + ".judge_score=" + judgeScore.ToString();
                                //sql += " where " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid) + " and " + examAnswerTableName + ".question_uid=" + StringUtil.QuotedToDBStr(questionUid);
                                //DataHelper.Instance.ExecuteNonQuery(sql);

                                var answerQueryable4 =
                                    iExamAnswerRep.GetAll()
                                        .Where(
                                            a =>
                                                a.examGradeUid == examGradeUid &&
                                                a.questionUid == questionUid);
                                foreach (var a in answerQueryable4)
                                {
                                    a.judgeResultCode = judgeResultCode;
                                    a.judgeScore = judgeScore;
                                    iExamAnswerRep.UpdateAsync(a);
                                }
                                iUnitOfWorkManager.Current.SaveChanges();
                            }
                        }

                    }
                    #endregion
                }
                else if (examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.JudgeCorrect)
                {
                    #region 对测评单选题评分
                    //sql = "select " + ExamAnswerTable.GetAllFieldSelectString(true) + "," + paperQuestionTableName + ".paper_question_score,exam_question.standard_answer,exam_question.answer_num,exam_question.select_answer_score ";
                    //sql += " from " + examAnswerTableName + " exam_answer inner join " + paperQuestionTableName + " on (exam_answer.question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                    //sql += "  inner join exam_question on exam_answer.question_uid=exam_question.question_uid ";
                    //sql += " where exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and exam_answer.exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                    //if (updateQuestionUid != null)
                    //{
                    //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                    //}

                    //dtExamAnswer = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];
                    var answerQueryable3 = from answer in iExamAnswerRep.GetAll()
                                           join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                                               nodeQuestion.questionUid
                                           where
                                               nodeQuestion.paperUid == paperUid &&
                                               answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                                               answer.examGradeUid == examGradeUid
                                               && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                                           select new
                                           {
                                               answer,
                                               nodeQuestion.paperQuestionScore,
                                               answer.Question.standardAnswer,
                                               answer.Question.selectAnswerScore,
                                               answer.Question.answerNum
                                           };
                    //for (int i = 0; i < dtExamAnswer.Rows.Count; i++)
                    foreach (var examAnswer in answerQueryable3)
                    {
                        standardAnswer = examAnswer.standardAnswer;
                        standardAnswer = QuestionUtil.FormatStanderdAnswerForMulti(standardAnswer);
                        userAnswer = examAnswer.answer.answerText;
                        questionUid = examAnswer.answer.questionUid;// dtExamAnswer.Rows[i]["question_uid"].ToString();
                        paperQuestionScore = examAnswer.paperQuestionScore;
                        judgeScore = 0;
                        judgeResultCode = EnumJudgeResultCode.Right;


                        //计算得分
                        if (string.IsNullOrEmpty(userAnswer))
                        {
                            judgeScore = 0;
                            judgeResultCode = EnumJudgeResultCode.Error;
                        }
                        else if (userAnswer == standardAnswer)
                        {
                            judgeScore = paperQuestionScore;
                            judgeResultCode = EnumJudgeResultCode.Right;
                        }
                        else if (userAnswer != standardAnswer)
                        {
                            judgeScore = paperQuestionScore / 2;
                            judgeResultCode = EnumJudgeResultCode.Middle;
                        }

                        //处理做错题扣分的情况,当前试题分数为0时,说明答案不正确
                        if (isDeductScore)
                        {
                            judgeScore = 0 - paperQuestionScore;
                        }

                        //sql = " Update " + examAnswerTableName + " set " + examAnswerTableName + ".judge_result_code='" + judgeResultCode + "'," + examAnswerTableName + ".judge_score=" + judgeScore.ToString();
                        //sql += " where " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid) + " and " + examAnswerTableName + ".question_uid=" + StringUtil.QuotedToDBStr(questionUid);
                        //DataHelper.Instance.ExecuteNonQuery(sql);
                        var answerQueryable4 =
                                    iExamAnswerRep.GetAll()
                                        .Where(
                                            a =>
                                                a.examGradeUid == examGradeUid &&
                                                a.questionUid == questionUid);

                        foreach (var a in answerQueryable4)
                        {
                            a.judgeResultCode = judgeResultCode;
                            a.judgeScore = judgeScore;
                            iExamAnswerRep.UpdateAsync(a);
                        }
                        iUnitOfWorkManager.Current.SaveChanges();
                    }
                    #endregion
                }
                else if (examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Fill)
                {
                    #region 对填空题评分(按空计分)
                    //sql = "select " + ExamAnswerTable.GetAllFieldSelectString(true) + "," + paperQuestionTableName + ".paper_question_score,exam_question.standard_answer,exam_question.answer_num ";
                    //sql += " from " + examAnswerTableName + " exam_answer inner join " + paperQuestionTableName + " on (exam_answer.question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                    //sql += "  inner join exam_question on exam_answer.question_uid=exam_question.question_uid ";
                    //sql += " where exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and exam_answer.exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                    //if (updateQuestionUid != null)
                    //{
                    //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                    //}

                    //dtExamAnswer = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];
                    var answerQueryable3 = from answer in iExamAnswerRep.GetAll()
                                           join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                                               nodeQuestion.questionUid
                                           where
                                               nodeQuestion.paperUid == paperUid &&
                                               answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                                               answer.examGradeUid == examGradeUid
                                               && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                                           select new
                                           {
                                               answer,
                                               nodeQuestion.paperQuestionScore,
                                               answer.Question.standardAnswer,
                                               answer.Question.selectAnswerScore,
                                               answer.Question.answerNum
                                           };
                    //for (int i = 0; i < dtExamAnswer.Rows.Count; i++)
                    foreach (var examAnswer in answerQueryable3)
                    {
                        standardAnswer = examAnswer.standardAnswer;
                        standardAnswer = QuestionUtil.FormatStanderdAnswerForMulti(standardAnswer);
                        userAnswer = examAnswer.answer.answerText;
                        questionUid = examAnswer.answer.questionUid;// dtExamAnswer.Rows[i]["question_uid"].ToString();
                        paperQuestionScore = examAnswer.paperQuestionScore;
                        judgeScore = 0;
                        judgeResultCode = EnumJudgeResultCode.Right;
                        
                        //支持按空计分,每空用|分格
                        arrStandardAnswer = standardAnswer.Split('|');
                        arrUserAnswer = userAnswer.Split('|');
                        answerNum = arrStandardAnswer.Length;

                        if (FilterQuestionAnswerForFill(userAnswer) == FilterQuestionAnswerForFill(standardAnswer))
                        {
                            judgeScore = paperQuestionScore;
                        }
                        else
                        {
                            for (int j = 0; j < arrUserAnswer.Length; j++)
                            {
                                if (j >= arrStandardAnswer.Length) break;
                                if (FilterQuestionAnswerForFill(arrUserAnswer[j]) == FilterQuestionAnswerForFill(arrStandardAnswer[j]))
                                {
                                    judgeScore += paperQuestionScore / answerNum;
                                }
                                else
                                {
                                    //如果不是完全配配，则还要看是否是几个中任一个都行,中间用&分隔
                                    if (arrStandardAnswer[j].ToLower().IndexOf("&") > -1)
                                    {
                                        string[] arrOneAnswer = arrStandardAnswer[j].Split('&');
                                        for (int k = 0; k < arrOneAnswer.Length; k++)
                                        {
                                            //只要是其中一个都得分
                                            if (FilterQuestionAnswerForFill(arrUserAnswer[j]) == FilterQuestionAnswerForFill(arrOneAnswer[k]))
                                            {
                                                judgeScore += paperQuestionScore / answerNum;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (judgeScore == 0)
                            judgeResultCode = EnumJudgeResultCode.Error;
                        else if (judgeScore.ToString("#.##") == paperQuestionScore.ToString("#.##"))
                            judgeResultCode = EnumJudgeResultCode.Right;
                        else
                            judgeResultCode = EnumJudgeResultCode.Middle;

                        //处理做错题扣分的情况,当前试题分数为0时,说明答案不正确
                        if (isDeductScore)
                        {
                            judgeScore = 0 - paperQuestionScore;
                        }

                        //sql = " Update " + examAnswerTableName + " set " + examAnswerTableName + ".judge_result_code='" + judgeResultCode + "'," + examAnswerTableName + ".judge_score=" + judgeScore.ToString();
                        //sql += " where " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid) + " and " + examAnswerTableName + ".question_uid=" + StringUtil.QuotedToDBStr(questionUid);
                       // DataHelper.Instance.ExecuteNonQuery(sql);
                        var answerQueryable4 =
                                    iExamAnswerRep.GetAll()
                                        .Where(
                                            a =>
                                                a.examGradeUid == examGradeUid &&
                                                a.questionUid == questionUid);
                        foreach (var a in answerQueryable4)
                        {
                            a.judgeResultCode = judgeResultCode;
                            a.judgeScore = judgeScore;
                            iExamAnswerRep.UpdateAsync(a);
                        }
                        
                        iUnitOfWorkManager.Current.SaveChanges();
                    }
                    dtExamAnswer = null;
                    #endregion
                }
                else if (examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.EvaluationSingle)
                {
                    #region 对测评单选题评分
                    //sql = "select " + ExamAnswerTable.GetAllFieldSelectString(true) + "," + paperQuestionTableName + ".paper_question_score,exam_question.standard_answer,exam_question.answer_num,exam_question.select_answer_score ";
                    //sql += " from " + examAnswerTableName + " exam_answer inner join " + paperQuestionTableName + " on (exam_answer.question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                    //sql += "  inner join exam_question on exam_answer.question_uid=exam_question.question_uid ";
                    //sql += " where exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and exam_answer.exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                    //if (updateQuestionUid != null)
                    //{
                    //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                    //}
                    //dtExamAnswer = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];
                    var answerQueryable3 = from answer in iExamAnswerRep.GetAll()
                                           join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                                               nodeQuestion.questionUid
                                           where
                                               nodeQuestion.paperUid == paperUid &&
                                               answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                                               answer.examGradeUid == examGradeUid
                                               && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                                           select new
                                           {
                                               answer,
                                               nodeQuestion.paperQuestionScore,
                                               answer.Question.standardAnswer,
                                               answer.Question.selectAnswerScore,
                                               answer.Question.answerNum
                                           };
                    //for (int i = 0; i < dtExamAnswer.Rows.Count; i++)
                    foreach (var examAnswer in answerQueryable3)
                    {
                        userAnswer = examAnswer.answer.answerText;
                        questionUid = examAnswer.answer.questionUid;// dtExamAnswer.Rows[i]["question_uid"].ToString();
                        judgeScore = 0;
                        judgeResultCode = EnumJudgeResultCode.Right;


                        //计算得分
                        if (string.IsNullOrEmpty(userAnswer))
                        {
                            judgeScore = 0;
                            judgeResultCode = EnumJudgeResultCode.Error;
                        }
                        else
                        {
                            arrSelectAnswerScore = examAnswer.selectAnswerScore.Split('|');

                            int index = ConvertUtil.ToInt(QuestionUtil.AnswerCharsToNumbers(userAnswer));
                            if (index >= 0 && index < arrSelectAnswerScore.Length)
                            {
                                judgeScore = ConvertUtil.ToDecimal(arrSelectAnswerScore[index], 0);
                            }

                            if (judgeScore < 0)
                                judgeScore = 0;
                        }

                        //sql = " Update " + examAnswerTableName + " set " + examAnswerTableName + ".judge_result_code='" + judgeResultCode + "'," + examAnswerTableName + ".judge_score=" + judgeScore.ToString();
                        //sql += " where " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid) + " and " + examAnswerTableName + ".question_uid=" + StringUtil.QuotedToDBStr(questionUid);
                        //DataHelper.Instance.ExecuteNonQuery(sql);
                        var answerQueryable4 =
                                    iExamAnswerRep.GetAll()
                                        .Where(
                                            a =>
                                                a.examGradeUid == examGradeUid &&
                                                a.questionUid == questionUid);
                        foreach (var a in answerQueryable4)
                        {
                            a.judgeResultCode = judgeResultCode;
                            a.judgeScore = judgeScore;
                            iExamAnswerRep.UpdateAsync(a);
                        }
                        iUnitOfWorkManager.Current.SaveChanges();
                    }
                    #endregion
                }
                else if (examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.EvaluationMulti)
                {
                    #region 对测评多选题评分
                    //sql = "select " + ExamAnswerTable.GetAllFieldSelectString(true) + "," + paperQuestionTableName + ".paper_question_score,exam_question.standard_answer,exam_question.answer_num,exam_question.select_answer_score ";
                    //sql += " from " + examAnswerTableName + " exam_answer inner join " + paperQuestionTableName + " on (exam_answer.question_uid=" + paperQuestionTableName + ".question_uid and " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")";
                    //sql += "  inner join exam_question on exam_answer.question_uid=exam_question.question_uid ";
                    //sql += " where exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " and exam_answer.exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
                    //if (updateQuestionUid != null)
                    //{
                    //    sql += " and exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid);
                    //}
                    //dtExamAnswer = DataHelper.Instance.ExecuteDataSet(sql).Tables[0];
                    var answerQueryable3 = from answer in iExamAnswerRep.GetAll()
                                           join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                                               nodeQuestion.questionUid
                                           where
                                               nodeQuestion.paperUid == paperUid &&
                                               answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                                               answer.examGradeUid == examGradeUid
                                               && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                                           select new
                                           {
                                               answer,
                                               nodeQuestion.paperQuestionScore,
                                               answer.Question.standardAnswer,
                                               answer.Question.selectAnswerScore,
                                               answer.Question.answerNum
                                           };
                    //for (int i = 0; i < dtExamAnswer.Rows.Count; i++)
                    foreach (var examAnswer in answerQueryable3)
                    {
                        userAnswer = examAnswer.answer.answerText;
                        questionUid = examAnswer.answer.questionUid;// dtExamAnswer.Rows[i]["question_uid"].ToString();
                        judgeScore = 0;
                        judgeResultCode = EnumJudgeResultCode.Right;

                        //计算得分
                        if (string.IsNullOrEmpty(userAnswer))
                        {
                            judgeScore = 0;
                            judgeResultCode = EnumJudgeResultCode.Error;
                        }
                        else
                        {
                            arrUserAnswer = userAnswer.Split('|');
                            arrSelectAnswerScore = examAnswer.selectAnswerScore.Split('|');
                            for (int m = 0; m < arrUserAnswer.Length; m++)
                            {
                                int index = ConvertUtil.ToInt(QuestionUtil.AnswerCharsToNumbers(arrUserAnswer[m]));
                                if (index >= 0 && index < arrSelectAnswerScore.Length)
                                {
                                    judgeScore += ConvertUtil.ToDecimal(arrSelectAnswerScore[index], 0);
                                }
                            }
                            if (judgeScore < 0)
                                judgeScore = 0;
                        }

                        //sql = " Update " + examAnswerTableName + " set " + examAnswerTableName + ".judge_result_code='" + judgeResultCode + "'," + examAnswerTableName + ".judge_score=" + judgeScore.ToString();
                        //sql += " where " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid) + " and " + examAnswerTableName + ".question_uid=" + StringUtil.QuotedToDBStr(questionUid);
                        //DataHelper.Instance.ExecuteNonQuery(sql);
                        var answerQueryable4 =
                                    iExamAnswerRep.GetAll()
                                        .Where(
                                            a =>
                                                a.examGradeUid == examGradeUid &&
                                                a.questionUid == questionUid);
                        
                        foreach (var a in answerQueryable4)
                        {
                            a.judgeResultCode = judgeResultCode;
                            a.judgeScore = judgeScore;
                            iExamAnswerRep.UpdateAsync(a);
                        }
                        iUnitOfWorkManager.Current.SaveChanges();
                    }
                    #endregion
                }
                else if (examQuestionTypeRow.questionBaseTypeCode == EnumQuestionBaseTypeCode.Answer)
                {
                    #region 对问答题评分 lopping 2011-09-08
                    //StringBuilder sqlBuilder = new StringBuilder();
                    //sqlBuilder.AppendLine("SELECT " + ExamAnswerTable.GetAllFieldSelectString(true) + "," + paperQuestionTableName + ".paper_question_score,exam_question.standard_answer,exam_question.answer_num,exam_question.select_answer,exam_question.select_answer_score ");
                    //sqlBuilder.AppendLine(" FROM " + examAnswerTableName + " exam_answer INNER JOIN " + paperQuestionTableName + " ON (exam_answer.question_uid=" + paperQuestionTableName + ".question_uid AND " + paperQuestionTableName + ".paper_uid=" + StringUtil.QuotedToDBStr(paperUid) + ")");
                    //sqlBuilder.AppendLine("  INNER JOIN exam_question ON exam_answer.question_uid=exam_question.question_uid ");
                    //sqlBuilder.AppendLine(" WHERE exam_question.question_type_uid=" + StringUtil.QuotedToDBStr(examQuestionTypeRow.QuestionTypeUid) + " AND exam_answer.exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid));
                    //if (updateQuestionUid != null)
                    //{
                    //    sqlBuilder.AppendLine(" AND exam_question.question_uid=" + StringUtil.QuotedToDBStr(updateQuestionUid));
                    //}
                    //dtExamAnswer = DataHelper.Instance.ExecuteDataSet(sqlBuilder.ToString()).Tables[0];
                    var answerQueryable3 = from answer in iExamAnswerRep.GetAll()
                                           join nodeQuestion in iExamPaperNodeQuestionRep.GetAll() on answer.questionUid equals
                                               nodeQuestion.questionUid
                                           where
                                               nodeQuestion.paperUid == paperUid &&
                                               answer.Question.questionTypeUid == examQuestionTypeRow.Id &&
                                               answer.examGradeUid == examGradeUid
                                               && (updateQuestionUid == Guid.Empty || answer.questionUid == updateQuestionUid)
                                           select new
                                           {
                                               answer,
                                               nodeQuestion.paperQuestionScore,
                                               answer.Question.standardAnswer,
                                               answer.Question.selectAnswerScore,
                                               answer.Question.selectAnswer,
                                               answer.Question.answerNum
                                           };
                    //for (int i = 0; i < dtExamAnswer.Rows.Count; i++)
                    foreach (var examAnswer in answerQueryable3)
                    {
                        standardAnswer = examAnswer.standardAnswer;
                        questionUid = examAnswer.answer.questionUid;
                        userAnswer = examAnswer.answer.answerText;
                        string[] arrSelectAnswer = examAnswer.selectAnswer.Split('|');
                        selectAnswerScore = examAnswer.selectAnswerScore;
                        arrSelectAnswerScore = selectAnswerScore.Split('|');
                        paperQuestionScore = examAnswer.paperQuestionScore;
                        if (FilterQuestionAnswerForFill(userAnswer) == FilterQuestionAnswerForFill(standardAnswer))
                        {
                            judgeScore = paperQuestionScore;
                        }
                        else
                        {
                            if (arrSelectAnswerScore.Length > 0)
                            {
                                for (int j = 0; j < arrSelectAnswer.Length; j++)
                                {
                                    if (FilterQuestionAnswerForAnswer(userAnswer, arrSelectAnswer[j]))
                                    {
                                        decimal score = paperQuestionScore * (ConvertUtil.ToDecimal(arrSelectAnswerScore[j]) / 100);
                                        judgeScore = judgeScore + score;
                                    }
                                }
                                if (judgeScore < 0)//出现负数分值，则归零
                                {
                                    judgeScore = 0;
                                }
                                if (judgeScore > paperQuestionScore)//比实际分数还大，则为实际分数
                                {
                                    judgeScore = paperQuestionScore;
                                }
                            }
                            else
                            {
                                judgeScore = 0;
                            }
                        }
                        if (judgeScore == 0)
                            judgeResultCode = EnumJudgeResultCode.Error;
                        else if (judgeScore.ToString("#.##") == paperQuestionScore.ToString("#.##"))
                            judgeResultCode = EnumJudgeResultCode.Right;
                        else
                            judgeResultCode = EnumJudgeResultCode.Middle;
                        
                        //sqlBuilder.AppendLine(" Update " + examAnswerTableName + " SET " + examAnswerTableName + ".judge_result_code='" + judgeResultCode + "'," + examAnswerTableName + ".judge_score=" + judgeScore.ToString());
                        //sqlBuilder.AppendLine(" WHERE " + examAnswerTableName + ".exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid) + " AND " + examAnswerTableName + ".question_uid=" + StringUtil.QuotedToDBStr(questionUid));
                        //DataHelper.Instance.ExecuteNonQuery(sqlBuilder.ToString());
                        var answerQueryable4 =
                                     iExamAnswerRep.GetAll()
                                         .Where(
                                             a =>
                                                 a.examGradeUid == examGradeUid &&
                                                 a.questionUid == questionUid);
                        
                        foreach (var a in answerQueryable4)
                        {
                            a.judgeResultCode = judgeResultCode;
                            a.judgeScore = judgeScore;
                            iExamAnswerRep.UpdateAsync(a);
                        }
                        iUnitOfWorkManager.Current.SaveChanges(); 
                    }
                    #endregion
                }
            }

            //求总分
            //sql = "select sum(Judge_Score) from " + examAnswerTableName + " where exam_grade_uid=" + StringUtil.QuotedToDBStr(examGradeUid);
            decimal paperTotalScore = examGradeRow.paperTotalScore ?? 0m;
            decimal totalScore = 0m;
            var judgeScoreQueryable =
                iExamAnswerRep.GetAll().Where(a => a.examGradeUid == examGradeUid).Select(a => a.judgeScore);
            if (judgeScoreQueryable.Any())
            {
                totalScore = judgeScoreQueryable.Sum();
            }

            //判断最高得分限制
            decimal markPaperMaxScore = examExamRow.markPaperMaxScore ?? 0m;
            if (markPaperMaxScore > 0)
            {
                if (totalScore > markPaperMaxScore)
                {
                    totalScore = markPaperMaxScore;
                }
                paperTotalScore = markPaperMaxScore;
            }

            decimal grade_rate = 0;
            if (paperTotalScore > 0) grade_rate = totalScore * 100 / paperTotalScore;


            //==========2.改更成绩状态================
            examGradeRow.gradeScore = totalScore;
            examGradeRow.externalScore = totalScore;
            examGradeRow.gradeRate = grade_rate;
            if (examExamRow.passGradeRate > 0)
            {
                examGradeRow.isPass = (grade_rate >= examExamRow.passGradeRate) ? "Y" : "N";
            }
            else
            {
                examGradeRow.isPass = (totalScore >= examExamRow.passGradeScore) ? "Y" : "N";
            }

            return retValue;
        }

        /// <summary>
        /// 对比用户答案和得分点 lopping 2011-09-08
        /// </summary>
        /// <param name="userAnswer"></param>
        /// <param name="standAnswer"></param>
        /// <returns></returns>
        public static bool FilterQuestionAnswerForAnswer(string userAnswer, string selectAnswer)
        {
            Regex regex = new Regex(@"[\ |\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\-|\+|\=|\||\\|\[|\]|\{|\}|\;|\:|\""|\'|\,|\<|\.|\“|\”|\：|\？|\‘|\’|\《|\》|\（|\）|\，|\。|\、|\；|\>|\/|\?|_]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            userAnswer = regex.Replace(userAnswer, "");
            selectAnswer = regex.Replace(selectAnswer, "");
            string pattern = @"/[^\u4e00-\u9fa5a-zA-Z0-9]/g";
            userAnswer = userAnswer.Replace(pattern, "");
            userAnswer = userAnswer.ToLower();
            selectAnswer = selectAnswer.Replace(pattern, "");
            selectAnswer = selectAnswer.ToLower();
            if (userAnswer.IndexOf(selectAnswer) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 过滤答案中的标点符号,只保留中文,字母及数字
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        private static string FilterQuestionAnswerForFill(string answer)
        {
            //中文|小写字符|大写字符|数字
            string regexText = "[^\u4e00-\u9fa5a-zA-Z0-9]";
            Regex regex = new Regex(regexText, RegexOptions.IgnoreCase);
            //answer = regex.Replace(answer, "");
            answer = answer.Replace("|", "");//不处理特殊字符
            answer = answer.ToLower();

            return answer;
        }

        /// <summary>
        /// 从XML中获取第一个节点的值
        /// </summary>
        /// <param name="userAnswerXML"></param>
        /// <param name="nodeTag"></param>
        /// <returns></returns>
        public static string GetFirstNodeValueFromXML(string xmlText, string nodeTag)
        {
            string nodeValue = "";

            string beginTag = "<" + nodeTag + ">";
            string endTag = "</" + nodeTag + ">";

            int beginTagPos = xmlText.IndexOf(beginTag);
            int endTagPos = xmlText.IndexOf(endTag);
            if (beginTagPos > -1 && endTagPos > -1 && endTagPos > beginTagPos)
            {
                nodeValue = xmlText.Substring(beginTagPos + beginTag.Length, endTagPos - beginTagPos - beginTag.Length);
            }
            return nodeValue;
        }
    }

    #region 枚举常量
    class EnumExamJudgePolicy
    {
        /// <summary>
        /// 完全正确才得分,用于多选题
        /// </summary>
        public static readonly string MultiByAbsolutelyRight = "multi_absolutely_right";

        /// <summary>
        /// 部分正确(没有错误选项),按正确选项的比率得分, 用于多选题
        /// </summary>
        public static readonly string MultiByPartialRight = "multi_partiel_right";

        /// <summary>
        /// 部分正确(没有错误选项),由用户手工指定每个正确选项的得分。
        /// </summary>
        public static readonly string MultiByPartialRightScore = "multi_partiel_right_score";

        /// <summary>
        /// 不完全正确时(有错误选项,也有正确选项),每项的得扣分, 用于多选题
        /// </summary>
        public static readonly string MultiByIncompleteRight = "multi_incomplete_right";


        /// <summary>
        /// 不完全正确时，答错扣分（每项扣分少于每项得分，总扣分减去得分不小于零）
        /// </summary>
        public static readonly string MultiByWrongSub = "multi_wrong_sub";



        /// <summary>
        /// 答对得分,不答不得分,答错扣分,用于判断题
        /// </summary>
        public static readonly string JudgeByRightOrWrong = "judge_right_or_wrong";

    }
    /// <summary>
    /// 考试分类代码
    /// </summary>
    class EnumExamClassCode
    {
        /// <summary>
        /// 考试
        /// </summary>
        public const string Examination = "exam";
        /// <summary>
        /// 作业
        /// </summary>
        public const string Task = "task";

        /// <summary>
        /// 竞赛
        /// </summary>
        public const string Race = "race";

        /// <summary>
        /// 补考
        /// </summary>
        public const string Makeup = "makeup";


    }
    class EnumExamGradeStatusCode
    {
        public const string Examing = "examing";
        public const string Pause = "pause";
        public const string Submitted = "submitted";
        public const string Judging = "judging";
        public const string Judged = "judged";
        public const string Release = "release";
    }
    class EnumExamGradeRelease
    {
        public const string ByTime = "by_time";
        public const string AfterDay = "after_day";
        public const string ByHuman = "by_human";
    }
    /// <summary>
    /// 考试类型代码
    /// </summary>
    class EnumExamTypeCode
    {
        /// <summary>
        /// 考试安排
        /// </summary>
        public const string Examination = "exam_normal";
        /// <summary>
        /// 培训计划考试
        /// </summary>
        public const string TrainExamination = "exam_train";

        /// <summary>
        /// 认证考试
        /// </summary>
        public const string TrainCer = "exam_cer";

        /// <summary>
        /// 作业
        /// </summary>
        public const string Task = "task_normal";
        /// <summary>
        /// 培训计划作业
        /// </summary>
        public const string TrainTask = "task_train";

        public const string Race = "race";
    }
    class EnumGateQuestionModeCode
    {
        public const string OneQuestion = "one_question";       //每关一题
        public const string OneNode = "one_node";               //每关一大题
    } 
    #endregion

}