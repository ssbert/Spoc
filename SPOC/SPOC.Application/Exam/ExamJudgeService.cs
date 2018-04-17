using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.Helper;
using SPOC.Exam.Dto.Judge;
using SPOC.ExamPaper.Dto;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace SPOC.Exam
{
    /// <summary>
    /// 评卷操作接口实现
    /// </summary>
    public class ExamJudgeService : SPOCAppServiceBase, IExamJudgeService
    {
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<ExamAnswer, Guid> _iExamAnswerRep;
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<ExamUserAnswer, Guid> _iExamUserAnswerRep;
        private readonly IRepository<ExamJudgeInfo, Guid> _iExamJudgeInfoRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IRepository<ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<ExamPaperNode, Guid> _iExamPaperNodeRep;
        private readonly IRepository<ExamPaperNodeQuestion, Guid> _iExamPaperNodeQuestionRep;
        private readonly IRepository<ExamJudgePolicy, Guid> _iExamJudgePolicyRep;
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamJudgeService(IRepository<ExamExam, Guid> iExamExamRep, IRepository<ExamGrade, Guid> iExamGradeRep,
            IRepository<ExamUserAnswer, Guid> iExamUserAnswerRep, IRepository<ExamJudgeInfo, Guid> iExamJudgeInfoRep,
            IRepository<ExamAnswer, Guid> iExamAnswerRep, IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep,
            IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<ExamPaper, Guid> iExamPaperRep,
            IRepository<ExamPaperNode, Guid> iExamPaperNodeRep, IRepository<ExamJudgePolicy, Guid> iExamJudgePolicyRep)
        {
            _iExamExamRep = iExamExamRep;
            _iExamGradeRep = iExamGradeRep;
            _iExamUserAnswerRep = iExamUserAnswerRep;
            _iExamJudgeInfoRep = iExamJudgeInfoRep;
            _iExamAnswerRep = iExamAnswerRep;
            _iExamPaperNodeQuestionRep = iExamPaperNodeQuestionRep;
            _iExamQuestionRep = iExamQuestionRep;
            _iExamPaperRep = iExamPaperRep;
            _iExamPaperNodeRep = iExamPaperNodeRep;
            _iExamJudgePolicyRep = iExamJudgePolicyRep;
        }
        #endregion

        /// <summary>
        /// 提交评卷
        /// </summary>
        /// <returns></returns>
        public async Task<JudgeResultOutputDto> SubmitJudge()
        {
            var request = HttpContext.Current.Request;
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var examGradeId = request.Form["examGradeUid"].TryParseGuid();
            if (examGradeId == Guid.Empty)
            {
                throw new UserFriendlyException("无效的成绩");
            }
            var examGrade = await _iExamGradeRep.FirstOrDefaultAsync(examGradeId);
            if (examGrade == null)
            {
                throw new UserFriendlyException("无效的试题信息");
            }

            var questionUidArr = request.Form.GetValues("hidQuestionUid");

            var userAnswerEntity = await _iExamUserAnswerRep.FirstOrDefaultAsync(examGrade.userAnswerUid);
            var userAnswerXML = userAnswerEntity == null ? string.Empty : userAnswerEntity.userAnswer;

            var xmlDocument = new XmlDocument();
            if (!string.IsNullOrEmpty(userAnswerXML) && userAnswerXML.Contains("<?xml version"))
            {
                try
                {
                    xmlDocument.LoadXml(userAnswerXML);
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException("读取答卷信息失败。原因是:" + e.Message);
                }
            }

            XmlNode examAnswersNode = null;
            if (xmlDocument.DocumentElement != null)
            {
                examAnswersNode = xmlDocument.DocumentElement.SelectSingleNode("exam_answers");
            }

            var needUpdateXml = false;
            if (examAnswersNode != null)
            {
                needUpdateXml = true;
            }

            var defaultVale = -1m;
            var dvExamAnswerList = await _iExamAnswerRep.GetAll().Where(a => a.examGradeUid == examGrade.Id).ToListAsync();
            var dvScoreList = await _iExamPaperNodeQuestionRep.GetAll().Where(a => a.paperUid == examGrade.paperUid).ToListAsync();

            if (questionUidArr != null)
            {
                for (var i = 0; i < questionUidArr.Length; i++)
                {
                    var questionId = questionUidArr[i].TryParseGuid();
                    var question = dvScoreList.FirstOrDefault(a => a.questionUid == questionId);
                    var score = question != null ? question.paperQuestionScore : 0;
                    var radEvaluate = request.Form.GetValues("radEvaluate" + questionId);
                    var judgeScore = ConvertUtil.ToDecimal(request["txtJudgeScore" + questionId], 0);
                    var judgeRemarks = ConvertUtil.ToString(request["txtJudgeDesc" + questionId]);
                    var judgeResultCode = string.Empty;
                    if (radEvaluate != null)
                    {
                        if (radEvaluate.Length >= 1)
                        {
                            if (ConvertUtil.ToDecimal(radEvaluate[0], defaultVale) == score)
                            {
                                judgeResultCode = EnumJudgeResultCode.Right;
                            }
                            else if (ConvertUtil.ToDecimal(radEvaluate[0], defaultVale) > 0)
                            {
                                judgeResultCode = EnumJudgeResultCode.Middle;
                            }
                            else
                            {
                                judgeResultCode = EnumJudgeResultCode.Error;
                            }
                        }
                    }
                    // 当没选择对错时,如果有分数则反过来获取状态
                    if (judgeScore > 0)
                    {
                        if (judgeScore >= score)
                        {
                            judgeResultCode = EnumJudgeResultCode.Right;
                        }
                        else
                        {
                            judgeResultCode = EnumJudgeResultCode.Middle;
                        }
                    }

                    var controlValue = string.Empty;
                    var controlId = "Answers" + questionId;
                    if (request[controlId] != null)
                    {
                        controlValue = request[controlId].Trim().Replace(",", "|");
                    }
                    else
                    {
                        controlValue = null;
                    }

                    var judgeScoreCode = string.Empty;
                    if (controlValue != null && controlValue != "")
                    {
                        judgeScoreCode += "|" + controlValue;
                    }

                    //更新XML
                    var answerText = string.Empty;
                    if (needUpdateXml)
                    {
                        var examAnswerNode = examAnswersNode.SelectSingleNode("exam_answer[question_uid=\"" + questionId + "\"]");
                        if (examAnswerNode != null)
                        {
                            answerText = examAnswerNode.SelectSingleNode("answer_text").InnerText;
                            examAnswerNode.SelectSingleNode("judge_score").InnerText = judgeScore.ToString();
                            examAnswerNode.SelectSingleNode("judge_result_code").InnerText = judgeResultCode;
                            //备注有可能没有
                            if (examAnswerNode.SelectSingleNode("judge_remarks") != null)
                            {
                                examAnswerNode.SelectSingleNode("judge_remarks").InnerText = judgeRemarks;
                            }

                            if (examAnswerNode.SelectSingleNode("judge_score_text") != null)
                            {
                                examAnswerNode.SelectSingleNode("judge_score_text").InnerText = judgeScoreCode;
                            }
                            else
                            {
                                XmlNode tempXml = null;
                                tempXml = xmlDocument.CreateElement("judge_score_text");
                                tempXml.InnerText = judgeScoreCode;
                                examAnswerNode.AppendChild(tempXml);
                            }
                        }
                        else if (judgeResultCode != "")
                        {
                            //如果是操作题等就可能原来没有答案
                            //创建节点
                            XmlNode tempNode = null;
                            examAnswerNode = xmlDocument.CreateElement("exam_answer");

                            tempNode = xmlDocument.CreateElement("exam_grade_uid");
                            tempNode.InnerText = examGradeId.ToString();
                            examAnswerNode.AppendChild(tempNode);

                            tempNode = xmlDocument.CreateElement("question_uid");
                            tempNode.InnerText = questionId.ToString();
                            examAnswerNode.AppendChild(tempNode);

                            tempNode = xmlDocument.CreateElement("answer_text");
                            tempNode.InnerText = "";
                            examAnswerNode.AppendChild(tempNode);

                            tempNode = xmlDocument.CreateElement("judge_score_text");
                            tempNode.InnerText = "";
                            examAnswerNode.AppendChild(tempNode);

                            tempNode = xmlDocument.CreateElement("answer_time");
                            tempNode.InnerText = "0";
                            examAnswerNode.AppendChild(tempNode);

                            tempNode = xmlDocument.CreateElement("judge_result_code");
                            tempNode.InnerText = judgeResultCode;
                            examAnswerNode.AppendChild(tempNode);

                            tempNode = xmlDocument.CreateElement("judge_score");
                            tempNode.InnerText = judgeScore.ToString("0.##");
                            examAnswerNode.AppendChild(tempNode);

                            tempNode = xmlDocument.CreateElement("judge_remarks");
                            tempNode.InnerText = judgeRemarks;
                            examAnswerNode.AppendChild(tempNode);

                            //把节点增加进去
                            examAnswersNode.AppendChild(examAnswerNode);
                        }
                    }

                    //更新数据库
                    if (examGrade.hasSaveAnswerToDb == "Y" || examGrade.hasSaveAnswerToDb == "")
                    {
                        var examAnswer = dvExamAnswerList.FirstOrDefault(a => a.questionUid == questionId);
                        if (examAnswer != null)
                        {
                            examAnswer.judgeResultCode = judgeResultCode;
                            examAnswer.judgeScore = judgeScore;
                            examAnswer.judgeRemarks = judgeRemarks;
                            examAnswer.judgeScoreText = judgeScoreCode;
                            await _iExamAnswerRep.UpdateAsync(examAnswer);
                        }
                        else if (judgeResultCode != "")
                        {
                            examAnswer = new ExamAnswer
                            {
                                Id = Guid.NewGuid(),
                                examGradeUid = examGradeId,
                                questionUid = questionId,
                                answerText = answerText,
                                judgeResultCode = judgeResultCode,
                                judgeScore = judgeScore,
                                judgeRemarks = judgeRemarks,
                                judgeScoreText = judgeScoreCode
                            };

                            await _iExamAnswerRep.InsertAsync(examAnswer);
                        }
                    }

                    //添加评卷记录
                    await _iExamJudgeInfoRep.InsertAsync(new ExamJudgeInfo
                    {
                        Id = Guid.NewGuid(),
                        ExamGradeUid = examGrade.Id,
                        QuestionUid = questionId,
                        JudgeUserUid = cookie.Id,
                        JudgeResultCode = judgeResultCode,
                        JudgeScore = judgeScore,
                        JudgeRemarks = judgeRemarks,
                        JudgeScoreText = judgeScoreCode,
                        CreateTime = DateTime.Now
                    });
                }
            }

            var totalScore = 0m;
            var examExamRow = await _iExamExamRep.FirstOrDefaultAsync(examGrade.examUid);
            if (needUpdateXml)
            {
                //求总分
                var xmlNodeList = examAnswersNode.SelectNodes("exam_answer");
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    totalScore = totalScore + ConvertUtil.ToDecimal(xmlNode.SelectSingleNode("judge_score").InnerText);
                }

                var userAnswer = xmlDocument.OuterXml;
                if (userAnswerEntity == null)
                {
                    userAnswerEntity = new ExamUserAnswer
                    {
                        Id = Guid.NewGuid(),
                        userAnswer = userAnswer
                    };
                    examGrade.userAnswerUid = userAnswerEntity.Id;
                    await _iExamUserAnswerRep.InsertAsync(userAnswerEntity);
                }
                else
                {
                    userAnswerEntity.userAnswer = userAnswer;
                    await _iExamUserAnswerRep.UpdateAsync(userAnswerEntity);
                }
            }
            else
            {
                for (var i = 0; i < dvExamAnswerList.Count; i++)
                {
                    totalScore = totalScore + ConvertUtil.ToDecimal(dvExamAnswerList[i].judgeScore);
                }
            }

            await UpdateExamGradeTotalScore(examGrade, examExamRow, totalScore);

            if (examExamRow.gradeReleaseType == "by_human")
            {
                examGrade.gradeStatusCode = EnumExamGradeStatusCode.Judged;
            }
            else
            {
                examGrade.gradeStatusCode = EnumExamGradeStatusCode.Release;
            }

            examGrade.judgeUserUid = cookie.Id;
            examGrade.judgeUserName = cookie.UserName;
            examGrade.judgeBeginTime = DateTime.Now;
            examGrade.judgeEndTime = DateTime.Now;

            await _iExamGradeRep.UpdateAsync(examGrade);

            return new JudgeResultOutputDto
            {
                GradeRate = examGrade.gradeRate ?? 0,
                GradeScore = examGrade.gradeScore ?? 0
            };
        }

        /* 暂时不做
        /// <summary>
        /// 自动评分
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AutoJudge(IdInputDto input) {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录。");
            }

            var examGrade = await _iExamGradeRep.FirstOrDefaultAsync(input.Id);
            if (examGrade == null)
            {
                throw new UserFriendlyException("无效的作答记录。");
            }

            var examExam = await _iExamExamRep.FirstOrDefaultAsync(examGrade.examUid);

            if (examExam == null)
            {
                throw new UserFriendlyException("无效的考试记录。");
            }

            var examPaper = await _iExamPaperRep.FirstOrDefaultAsync(examExam.paperUid);
            if (examPaper == null)
            {
                throw new UserFriendlyException("无效的试卷记录");
            }

            var userAnswer = await _iExamUserAnswerRep.FirstOrDefaultAsync(examGrade.userAnswerUid);
            if (userAnswer == null || string.IsNullOrEmpty(userAnswer.userAnswer))
            {
                throw new UserFriendlyException("无效的作答记录。");
            }
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.LoadXml(userAnswer.userAnswer);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("读取答卷信息失败。原因是:" + e.Message);
            }

            XmlNode examAnswersNode = null;
            if (xmlDocument.DocumentElement != null)
            {
                examAnswersNode = xmlDocument.DocumentElement.SelectSingleNode("exam_answers");
            }

            if (examAnswersNode == null)
            {
                throw new UserFriendlyException("无效的作答记录。");
            }

            //试题列表
            var dvQuestion = (from q in _iExamQuestionRep.GetAll()
                              join epq in _iExamPaperNodeQuestionRep.GetAll() on q.Id equals epq.questionUid
                              join epn in _iExamPaperNodeRep.GetAll() on epq.paperNodeUid equals epn.Id
                              where epn.paperUid == examGrade.paperUid
                              select new
                              {
                                  q,
                                  epq.paperQuestionScore,
                                  epq.listOrder
                              }).OrderBy(a => a.listOrder).ToList();
            //评卷策略
            var dvJudgePolicy = await _iExamJudgePolicyRep.GetAllListAsync(a => a.examUid == examExam.Id);
            //是否做错题扣分
            bool isDeductScore = false;
            if (examExam.isDeductScoreWhenError == "Y" && examExam.examDoModeCode == EnumExamDoModeCode.Question)
            {
                isDeductScore = true;
            }

            //计算总分及更新数据
            var totalScore = 0m;
            var xmlNodeList = examAnswersNode.SelectNodes("exam_answer");
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                totalScore += ConvertUtil.ToDecimal(xmlNodeList[i].SelectSingleNode("judge_score").InnerText);
            }
        }
        */

        private async Task UpdateExamGradeTotalScore(ExamGrade examGrade, ExamExam examExamRow, decimal totalScore) 
        {
            var paperTotalScore = examGrade.paperTotalScore;
            //判断最高得分
            var markPaperMaxScore = examExamRow.markPaperMaxScore;
            if (markPaperMaxScore > 0)
            {
                if (totalScore > markPaperMaxScore)
                {
                    if (markPaperMaxScore != null)
                    {
                        totalScore = (decimal)markPaperMaxScore;
                    }
                    paperTotalScore = markPaperMaxScore;
                }
            }
            examGrade.gradeScore = Math.Round(totalScore, 2, MidpointRounding.AwayFromZero);
            if (examGrade.paperTotalScore > 0)
            {
                examGrade.gradeRate = examGrade.gradeScore * 100 / paperTotalScore;
            }
            else
            {
                examGrade.gradeRate = 0;
            }

            examGrade.gradeRate = Math.Round((decimal)examGrade.gradeRate, 2, MidpointRounding.AwayFromZero);
            if (examExamRow.passGradeRate > 0)
            {
                examGrade.isPass = (examGrade.gradeRate >= examExamRow.passGradeRate) ? "Y" : "N";
            }
            else
            {
                examGrade.isPass = (examGrade.gradeScore >= examExamRow.passGradeScore) ? "Y" : "N";
            }
            await _iExamGradeRep.UpdateAsync(examGrade);
        }
    }
}
