using Abp.AutoMapper;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SPOC.Exam.Dto
{
    [AutoMapTo(typeof(ExamExam))]
    public class ExamExamInputDto: IShouldNormalize, ICustomValidate
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        /// <summary>
        /// 考试名称
        /// </summary>
        [Required, StringLength(256)]
        public string ExamName { get; set; }
        /// <summary>
        /// 考试编号
        /// </summary>
        [StringLength(64)]
        public string ExamCode { get; set; }
        /// <summary>
        /// 是否是自定义编号
        /// </summary>
        [Required]
        public bool isCustomCode { get; set; }
        /// <summary>
        /// 考试类型（exam考试, task作业）
        /// </summary>
        [Required, StringLength(16)]
        public string examClassCode { get; set; }
        /// <summary>
        /// 考试类型（exam_normal: 正考, exam_retest: 补考, exam_train:培训计划中的考试, task_normal:作业, task_train: 培训计划中的作业）
        /// </summary>
        [Required, StringLength(16)]
        public string examTypeCode { get; set; }
        /// <summary>
        /// 考试模式（Paper整卷、question分题、node大题）
        /// </summary>
        [Required, StringLength(16)]
        public string examDoModeCode { get; set; }
        /// <summary>
        /// 是否限时（适用分题模式）
        /// </summary>
        [StringLength(1)]
        public string isNeedLimitedTime { get; set; }
        /// <summary>
        /// 试卷类型（fix固定试卷、radom随机试卷）
        /// </summary>
        [Required, StringLength(16)]
        public string paperTypeCode { get; set; }
        /// <summary>
        /// 试卷ID（固定和随机度卷都有可能）
        /// </summary>
        public Guid paperUid { get; set; }

        /// <summary>
        /// 考试时间（以秒为单位）
        /// </summary>
        public int examTime { get; set; }
        /// <summary>
        /// 考试时间策略（join_exam 进入考试时间、end_exam允许参考时间）
        /// </summary>
        [StringLength(36), Required]
        public string examTimeModule { get; set; }
        /// <summary>
        /// 允许试题反馈
        /// </summary>
        [StringLength(1)]
        public string allowFeedbook { get; set; }
        /// <summary>
        /// 通过考试判断类型（passGradeRate | passGradeScore）
        /// </summary>
        [StringLength(16), Required]
        public string passGradeType { get; set; }
        /// <summary>
        /// 通过的得分率（通过分数和通过得分率只能有一个不为0）
        /// </summary>
        public decimal? passGradeRate { get; set; }
        /// <summary>
        /// 通过的分数
        /// </summary>
        public decimal? passGradeScore { get; set; }
        /// <summary>
        /// 是否开卷考试
        /// </summary>
        [StringLength(1)]
        public string isOpenBook { get; set; }
        /// <summary>
        /// 是否需要评分
        /// </summary>
        [StringLength(1)]
        public string isNeedJudge { get; set; }
        /// <summary>
        /// 自动保存秒数
        /// </summary>
        public int autoSaveSecond { get; set; }
        /// <summary>
        /// 自动保存答案到服务器（只有在启用自动保存答案时生效）
        /// </summary>
        [StringLength(1)]
        public string autoSaveToServer { get; set; }
        /// <summary>
        /// 最多允许参加次数
        /// </summary>
        public int? maxExamNum { get; set; }
        /// <summary>
        /// 最少需要参加次数
        /// </summary>
        public int? minExamNum { get; set; }
        /// <summary>
        /// 是否允许查看成绩
        /// </summary>
        [StringLength(1)]
        public string isAllowSeeGrade { get; set; }
        /// <summary>
        /// 允许查看成绩天数(0代表永久)
        /// </summary>
        public int? allowSeeGradeDays { get; set; }
        /// <summary>
        /// 公布成绩日期
        /// </summary>
        [StringLength(10)]
        public string publishGradeDate { get; set; }
        /// <summary>
        /// 是否允许查看答卷
        /// </summary>
        [StringLength(1)]
        public string isAllowSeeAnswer { get; set; }
        /// <summary>
        /// 是否允许查看报告(只有测评试卷才有报告)
        /// </summary>
        [StringLength(1)]
        public string isAllowSeeReport { get; set; }
        /// <summary>
        /// 是否对所有人公布成绩
        /// </summary>
        [StringLength(1)]
        public string isPublishGrade { get; set; }
        /// <summary>
        /// 是否实时监控
        /// </summary>
        [StringLength(1)]
        public string isRealTimeControll { get; set; }
        /// <summary>
        /// 是否保存每题答案到库中
        /// </summary>
        [StringLength(1)]
        public string isRealtimeSaveAnswerToDb { get; set; }
        /// <summary>
        /// 答错和不回答试题扣分
        /// 说明：做错题扣分和因不会做答（不是没有答到）引起的空而不答扣分功能，并且最终得分需要减去做错题的分数和不会答的分数。选手最终得分=做对题的分数-做错题的分数-不会答的题的分数。
        /// </summary>
        [StringLength(1)]
        public string isDeductScoreWhenError { get; set; }
        /// <summary>
        /// 考试说明(在考试时会显示在试卷上方的一段说明性质的文字)
        /// </summary>
        public string examDescription { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(256)]
        public string remarks { get; set; }
        /// <summary>
        /// 考试记录状态(normal正常)
        /// </summary>
        [StringLength(16)]
        public string examStatusCode { get; set; }
        /// <summary>
        /// 是否显示结论
        /// </summary>
        [StringLength(1)]
        public string isDisplayResult { get; set; }
        /// <summary>
        /// 是否打乱顺序
        /// </summary>
        [StringLength(1)]
        public string isMixOrder { get; set; }
        /// <summary>
        /// 缓存试卷份数
        /// </summary>
        public int? bufferPaperNum { get; set; }
        /// <summary>
        /// 缓存过期秒数
        /// </summary>
        public int? bufferExpireSecond { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid creatorUid { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime modifyTime { get; set; }
        /// <summary>
        /// 禁止开始时间
        /// </summary>
        public int? forbidTime { get; set; }
        /// <summary>
        /// 成绩发布类型
        /// </summary>
        [StringLength(16)]
        public string gradeReleaseType { get; set; }
        /// <summary>
        /// 考试后多少天发布
        /// </summary>
        public int? publishGradeDays { get; set; }
        /// <summary>
        /// 是否允许查看答卷
        /// </summary>
        [StringLength(1)]
        public string isAllowSeePaper { get; set; }
        /// <summary>
        /// 考试信息提前发布时间(秒)
        /// </summary>
        public int? forwardPublishExamTime { get; set; }
        /// <summary>
        /// 是否有IP限制
        /// </summary>
        [StringLength(1)]
        public string isLimitByIp { get; set; }
        /// <summary>
        /// 禁止提前提交时间
        /// </summary>
        public int? forbitSubmitBeforeTime { get; set; }
        /// <summary>
        /// 是否允许修改答案
        /// </summary>
        [StringLength(1)]
        public string isAllowModifyUserAnswer { get; set; }
        /// <summary>
        /// 是否允许修改客观题
        /// </summary>
        [StringLength(1)]
        public string isAllowModifyObjectAnswer { get; set; }
        /// <summary>
        /// 是否允许考生评试
        /// </summary>
        [StringLength(1)]
        public string isAllowUserJudgePaper { get; set; }
        /// <summary>
        /// 是否参与积分
        /// </summary>
        [StringLength(1)]
        public string isNeedIntegral { get; set; }
        /// <summary>
        /// 考试积分
        /// </summary>
        public decimal examIntegral { get; set; }
        /// <summary>
        /// 总关数
        /// </summary>
        public int? gateNum { get; set; }
        /// <summary>
        /// 每关出题方式
        /// </summary>
        [StringLength(36)]
        public string gateQuestionMode { get; set; }
        /// <summary>
        /// 每关得分率
        /// </summary>
        public decimal? gatePassGratdeRate { get; set; }
        /// <summary>
        /// 补考安排次数
        /// </summary>
        public int examinationCount { get; set; }
        /// <summary>
        /// 问卷调查的编号和名称
        /// </summary>
        [StringLength(256)]
        public string questionireUidName { get; set; }
        /// <summary>
        /// 通过考试提示信息
        /// </summary>
        public string passExamMessage { get; set; }
        /// <summary>
        /// 未通过考试提示信息
        /// </summary>
        public string noPassExamMessage { get; set; }
        /// <summary>
        /// 是否允许考生报名考试
        /// </summary>
        [StringLength(1)]
        public string isAllowUserRegExam { get; set; }
        /// <summary>
        /// 报名考试是否需要审批
        /// </summary>
        [StringLength(1)]
        public string isUserRegExamApprove { get; set; }
        /// <summary>
        /// 评卷最高得分
        /// </summary>
        public decimal? markPaperMaxScore { get; set; }
        /// <summary>
        /// 允许客观题手工评卷
        /// </summary>
        [StringLength(1)]
        public string isAllowObjectJudge { get; set; }
        /// <summary>
        /// 是否线下考试
        /// </summary>
        [StringLength(1)]
        public string isOfflineExam { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        
        public void Normalize()
        {
            isNeedLimitedTime = isNeedLimitedTime ?? "N";
            allowFeedbook = allowFeedbook ?? "N";
            isOpenBook = isOpenBook ?? "N";
            isNeedJudge = isNeedJudge ?? "N";
            autoSaveToServer = autoSaveToServer ?? "Y";
            isAllowSeeGrade = isAllowSeeGrade ?? "N";
            publishGradeDate = publishGradeDate ?? "";
            isAllowSeeAnswer = isAllowSeeAnswer ?? "N";
            isAllowSeeReport = isAllowSeeReport ?? "Y";
            isPublishGrade = isPublishGrade ?? "N";
            isRealTimeControll = isRealTimeControll ?? "N";
            isRealtimeSaveAnswerToDb = isRealtimeSaveAnswerToDb ?? "N";
            isDeductScoreWhenError = isDeductScoreWhenError ?? "N";
            examDescription = examDescription ?? "";
            remarks = remarks ?? "";
            isDisplayResult = isDisplayResult ?? "Y";
            isMixOrder = isMixOrder ?? "Y";
            isAllowSeePaper = isAllowSeePaper ?? "N";
            isLimitByIp = isLimitByIp ?? "Y";
            isAllowModifyUserAnswer = isAllowModifyObjectAnswer ?? "N";
            isAllowModifyObjectAnswer = isAllowModifyObjectAnswer ?? "N";
            isAllowUserJudgePaper = isAllowUserJudgePaper ?? "N";
            isNeedIntegral = isNeedIntegral ?? "N";
            gateQuestionMode = gateQuestionMode ?? "";
            questionireUidName = questionireUidName ?? "";
            passExamMessage = passExamMessage ?? "";
            noPassExamMessage = noPassExamMessage ?? "";
            isAllowUserRegExam = isAllowUserRegExam ?? "N";
            isUserRegExamApprove = isUserRegExamApprove ?? "N";
            isAllowObjectJudge = isAllowObjectJudge ?? "Y";
            isOfflineExam = isOfflineExam ?? "N";
        }

        public void AddValidationErrors(CustomValidationContext context)
        {
            var dic = new Dictionary<string, string>
            {
                {"isNeedLimitedTime",isNeedLimitedTime},
                {"allowFeedbook",allowFeedbook},
                {"isOpenBook",isOpenBook},
                {"isNeedJudge",isNeedJudge},
                {"autoSaveToServer",autoSaveToServer},
                //{"isAllowSeeGrade",isAllowSeeGrade},
                //{"isAllowSeeAnswer",isAllowSeeAnswer},
                {"isAllowSeeReport",isAllowSeeReport},
                {"isRealTimeControll",isRealTimeControll},
                {"isRealtimeSaveAnswerToDb",isRealtimeSaveAnswerToDb},
                {"isDeductScoreWhenError",isDeductScoreWhenError},
                //{"isDisplayResult",isDisplayResult},
                {"isMixOrder",isMixOrder},
                //{"isAllowSeePaper",isAllowSeePaper},
                {"isLimitByIp",isLimitByIp},
                //{"isAllowModifyUserAnswer",isAllowModifyUserAnswer},
                //{"isAllowModifyObjectAnswer",isAllowModifyObjectAnswer},
                //{"isAllowUserJudgePaper",isAllowUserJudgePaper},
                {"isNeedIntegral",isNeedIntegral},
                {"isAllowUserRegExam",isAllowUserRegExam},
                {"isUserRegExamApprove",isUserRegExamApprove},
                {"isAllowObjectJudge",isAllowObjectJudge},
                {"isOfflineExam",isOfflineExam}
            };
            var values = new[] { "Y", "N" };
            foreach (var obj in dic)
            {
                if (obj.Value != null && !values.Contains(obj.Value))
                {
                    context.Results.Add(new ValidationResult(obj.Key + "的值只能为 N or Y"));
                }
            }
        }
    }
}
