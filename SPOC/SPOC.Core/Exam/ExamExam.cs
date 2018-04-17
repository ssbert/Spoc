using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试信息
    /// </summary>
    public class ExamExam : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        public ExamExam()
        {
            #region 初始化数据
            isNeedLimitedTime = "N";
            allowFeedbook = "N";
            isNeedJudge = "N";
            autoSaveToServer = "N";
            isAllowSeeGrade = "N";
            publishGradeDate = "";
            isAllowSeeAnswer = "N";
            isAllowSeeReport = "Y";
            isPublishGrade = "N";
            isRealTimeControll = "N";
            isRealtimeSaveAnswerToDb = "N";
            isDeductScoreWhenError = "N";
            examDescription = "";
            remarks = "";
            isDisplayResult = "N";
            isMixOrder = "N";
            isAllowSeePaper = "N";
            isLimitByIp = "Y";
            isAllowModifyUserAnswer = "N";
            isAllowModifyObjectAnswer = "N";
            isAllowUserJudgePaper = "N";
            isNeedIntegral = "N";
            gateQuestionMode = "";
            questionireUidName = "";
            passExamMessage = "";
            noPassExamMessage = "";
            isAllowUserRegExam = "N";
            isUserRegExamApprove = "N";
            isAllowObjectJudge = "Y";
            isOfflineExam = "N";
            mobileFlag = "";
            passGradeType = "passGradeRate";
            IsExamination = "N";

            #endregion
        }
        /// <summary>
        /// 考试任务Id
        /// </summary>
        [Column("taskId")]
        public Guid TaskId { get; set; }
        /// <summary>
        /// 考试名称
        /// </summary>
        [Column("examName"), Required, StringLength(256)]
        public string ExamName { get; set; }
        /// <summary>
        /// 考试编号
        /// </summary>
        [Column("examCode"), Required, StringLength(64)]
        public string ExamCode { get; set; }
        /// <summary>
        /// 是否是自定义编号
        /// </summary>
        public bool isCustomCode { get; set; }
        /// <summary>
        /// 考试类型（exam考试, task作业）
        /// </summary>
        [StringLength(16)]
        [Required]
        public string examClassCode { get; set; }
        /// <summary>
        /// 考试类型（exam_normal: 正考, exam_retest: 补考, exam_train:培训计划中的考试, task_normal:作业, task_train: 培训计划中的作业）
        /// </summary>
        [StringLength(16)]
        [Required]
        public string examTypeCode { get; set; }
        /// <summary>
        /// 考试模式（Paper整卷、question分题、node大题）
        /// </summary>
        [StringLength(16), Required]
        public string examDoModeCode { get; set; }
        /// <summary>
        /// 是否限时（适用分题模式）
        /// </summary>
        [StringLength(1), Required, DefaultValue("N")]
        public string isNeedLimitedTime { get; set; }
        /// <summary>
        /// 试卷类型（fix固定试卷、radom随机试卷）
        /// </summary>
        [StringLength(16), Required]
        public string paperTypeCode { get; set; }
        /// <summary>
        /// 试卷ID（固定和随机度卷都有可能）
        /// </summary>
        public Guid paperUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]

        [ForeignKey("paperUid")] 
        public ExamPaper Paper { get; set; }
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
        [StringLength(1), Required, DefaultValue("N")]
        public string allowFeedbook { get; set; }
        /// <summary>
        /// 通过考试判断类型（passGradeRate | passGradeScore）
        /// </summary>
        [StringLength(16), Required, DefaultValue("passGradeRate")]
        public string passGradeType { get; set; }
        /// <summary>
        /// 通过的得分率（通过分数和通过得分率只能有一个不为0）
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? passGradeRate { get; set; }
        /// <summary>
        /// 通过的分数
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? passGradeScore { get; set; }
        /// <summary>
        /// 是否开卷考试
        /// </summary>
        [StringLength(1), Required, DefaultValue("N")]
        public string isOpenBook { get; set; }
        /// <summary>
        /// 是否需要评分
        /// </summary>
        [StringLength(1), Required, DefaultValue("N")]
        public string isNeedJudge { get; set; }
        /// <summary>
        /// 自动保存秒数
        /// </summary>
        public int autoSaveSecond { get; set; }
        /// <summary>
        /// 自动保存答案到服务器（只有在启用自动保存答案时生效）
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
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
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowSeeGrade { get; set; }
        /// <summary>
        /// 允许查看成绩天数(0代表永久)
        /// </summary>
        public int? allowSeeGradeDays { get; set; }
        /// <summary>
        /// 公布成绩日期
        /// </summary>
        [StringLength(10), DefaultValue("")]
        public string publishGradeDate { get; set; }
        /// <summary>
        /// 是否允许查看答卷
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isAllowSeeAnswer { get; set; }
        /// <summary>
        /// 是否允许查看报告(只有测评试卷才有报告)
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isAllowSeeReport { get; set; }
        /// <summary>
        /// 是否对所有人公布成绩
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isPublishGrade { get; set; }
        /// <summary>
        /// 是否实时监控
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isRealTimeControll { get; set; }
        /// <summary>
        /// 是否保存每题答案到库中
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isRealtimeSaveAnswerToDb { get; set; }
        /// <summary>
        /// 答错和不回答试题扣分
        /// 说明：做错题扣分和因不会做答（不是没有答到）引起的空而不答扣分功能，并且最终得分需要减去做错题的分数和不会答的分数。选手最终得分=做对题的分数-做错题的分数-不会答的题的分数。
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isDeductScoreWhenError { get; set; }
        /// <summary>
        /// 考试说明(在考试时会显示在试卷上方的一段说明性质的文字)
        /// </summary>
        [DefaultValue("")]
        public string examDescription { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(256), DefaultValue("")]
        public string remarks { get; set; }
        /// <summary>
        /// 考试记录状态(normal正常)
        /// </summary>
        [StringLength(16), Required]
        public string examStatusCode { get; set; }
        /// <summary>
        /// 是否显示结论
        /// </summary>
        [StringLength(1), DefaultValue("N"), Required]
        public string isDisplayResult { get; set; }
        /// <summary>
        /// 是否打乱顺序
        /// </summary>
        [StringLength(1), DefaultValue("N"), Required]
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
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isAllowSeePaper { get; set; }
        /// <summary>
        /// 考试信息提前发布时间(秒)
        /// </summary>
        public int? forwardPublishExamTime { get; set; }
        /// <summary>
        /// 是否有IP限制
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isLimitByIp { get; set; }
        /// <summary>
        /// 禁止提前提交时间
        /// </summary>
        public int? forbitSubmitBeforeTime { get; set; }
        /// <summary>
        /// 是否允许修改答案
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowModifyUserAnswer { get; set; }
        /// <summary>
        /// 是否允许修改客观题
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowModifyObjectAnswer { get; set; }
        /// <summary>
        /// 是否允许考生评试
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowUserJudgePaper { get; set; }
        /// <summary>
        /// 是否参与积分
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isNeedIntegral { get; set; }
        /// <summary>
        /// 考试积分
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal examIntegral { get; set; }
        /// <summary>
        /// 总关数
        /// </summary>
        public int? gateNum { get; set; }
        /// <summary>
        /// 每关出题方式
        /// </summary>
        [StringLength(36), DefaultValue("")]
        public string gateQuestionMode { get; set; }
        /// <summary>
        /// 每关得分率
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? gatePassGratdeRate { get; set; }
        /// <summary>
        /// 补考安排次数
        /// </summary>
        public int examinationCount { get; set; }
        /// <summary>
        /// 问卷调查的编号和名称
        /// </summary>
        [StringLength(256), DefaultValue("")]
        public string questionireUidName { get; set; }
        /// <summary>
        /// 通过考试提示信息
        /// </summary>
        [DefaultValue("")]
        public string passExamMessage { get; set; }
        /// <summary>
        /// 未通过考试提示信息
        /// </summary>
        [DefaultValue("")]
        public string noPassExamMessage { get; set; }
        /// <summary>
        /// 是否允许考生报名考试
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowUserRegExam { get; set; }
        /// <summary>
        /// 报名考试是否需要审批
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isUserRegExamApprove { get; set; }
        /// <summary>
        /// 评卷最高得分
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? markPaperMaxScore { get; set; }
        /// <summary>
        /// 允许客观题手工评卷
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isAllowObjectJudge { get; set; }
        /// <summary>
        /// 是否线下考试
        /// </summary>
        [StringLength(1), Required, DefaultValue("N")]
        public string isOfflineExam { get; set; }
        /// <summary>
        /// 移动端标识
        /// </summary>
        [StringLength(10), DefaultValue("")]
        public string mobileFlag { get; set; }

        /// <summary>
        /// 考试开始时间
        /// </summary>
        [Column("beginTime")]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 考试结束时间
        /// </summary>
        [Column("endTime")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否补考
        /// </summary>
        [Column("isExamination"), DefaultValue("N")]
        public string IsExamination { get; set; }
    }
}
