using Abp.AutoMapper;
using System;
using Newtonsoft.Json;

namespace SPOC.Exam.Dto
{

    [AutoMapFrom(typeof(ExamExam))]
    public class ExamExamOutputDto
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        /// <summary>
        /// 考试名称
        /// </summary>
        public string ExamName { get; set; }
        /// <summary>
        /// 考试编号
        /// </summary>
        public string ExamCode { get; set; }
        /// <summary>
        /// 是否是手动编号
        /// </summary>
        public bool isCustomCode { get; set; }
        /// <summary>
        /// 考试类型（exam考试, task作业）
        /// </summary>
        public string examClassCode { get; set; }
        /// <summary>
        /// 考试类型（exam_normal:考试, exam_retest:补考, exam_train:培训计划中的考试, task_normal:作业, task_train: 培训计划中的作业）
        /// </summary>
        public string examTypeCode { get; set; }
        /// <summary>
        /// 考试模式（Paper整卷、question分题、node大题）
        /// </summary>
        public string examDoModeCode { get; set; }
        /// <summary>
        /// 是否限时（适用分题模式）
        /// </summary>
        public string isNeedLimitedTime { get; set; }
        /// <summary>
        /// 试卷类型（fix固定试卷、radom随机试卷）
        /// </summary>
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
        public string examTimeModule { get; set; }
        /// <summary>
        /// 允许试题反馈
        /// </summary>
        public string allowFeedbook { get; set; }
        /// <summary>
        /// 通过考试判断类型（passGradeRate | passGradeScore）
        /// </summary>
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
        public string isOpenBook { get; set; }
        /// <summary>
        /// 是否需要评分
        /// </summary>
        public string isNeedJudge { get; set; }
        /// <summary>
        /// 自动保存秒数
        /// </summary>
        public int autoSaveSecond { get; set; }
        /// <summary>
        /// 自动保存答案到服务器（只有在启用自动保存答案时生效）
        /// </summary>
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
        public string isAllowSeeGrade { get; set; }
        /// <summary>
        /// 允许查看成绩天数(0代表永久)
        /// </summary>
        public int? allowSeeGradeDays { get; set; }
        /// <summary>
        /// 公布成绩日期
        /// </summary>
        public string publishGradeDate { get; set; }
        /// <summary>
        /// 是否允许查看答卷
        /// </summary>
        public string isAllowSeeAnswer { get; set; }
        /// <summary>
        /// 是否允许查看报告(只有测评试卷才有报告)
        /// </summary>
        public string isAllowSeeReport { get; set; }
        /// <summary>
        /// 是否对所有人公布成绩
        /// </summary>
        public string isPublishGrade { get; set; }
        /// <summary>
        /// 是否实时监控
        /// </summary>
        public string isRealTimeControll { get; set; }
        /// <summary>
        /// 是否保存每题答案到库中
        /// </summary>
        public string isRealtimeSaveAnswerToDb { get; set; }
        /// <summary>
        /// 答错和不回答试题扣分
        /// 说明：做错题扣分和因不会做答（不是没有答到）引起的空而不答扣分功能，并且最终得分需要减去做错题的分数和不会答的分数。选手最终得分=做对题的分数-做错题的分数-不会答的题的分数。
        /// </summary>
        public string isDeductScoreWhenError { get; set; }
        /// <summary>
        /// 考试说明(在考试时会显示在试卷上方的一段说明性质的文字)
        /// </summary>
        public string examDescription { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }
        /// <summary>
        /// 考试记录状态(normal正常)
        /// </summary>
        public string examStatusCode { get; set; }
        /// <summary>
        /// 是否显示结论
        /// </summary>
        public string isDisplayResult { get; set; }
        /// <summary>
        /// 是否打乱顺序
        /// </summary>
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
        [JsonConverter(typeof(DateFormat))]
        public DateTime createTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime modifyTime { get; set; }
        /// <summary>
        /// 禁止开始时间
        /// </summary>
        public int? forbidTime { get; set; }
        /// <summary>
        /// 成绩发布类型
        /// </summary>
        public string gradeReleaseType { get; set; }
        /// <summary>
        /// 考试后多少天发布
        /// </summary>
        public int? publishGradeDays { get; set; }
        /// <summary>
        /// 是否允许查看答卷
        /// </summary>
        public string isAllowSeePaper { get; set; }
        /// <summary>
        /// 考试信息提前发布时间(秒)
        /// </summary>
        public int? forwardPublishExamTime { get; set; }
        /// <summary>
        /// 是否有IP限制
        /// </summary>
        public string isLimitByIp { get; set; }
        /// <summary>
        /// 禁止提前提交时间
        /// </summary>
        public int? forbitSubmitBeforeTime { get; set; }
        /// <summary>
        /// 是否允许修改答案
        /// </summary>
        public string isAllowModifyUserAnswer { get; set; }
        /// <summary>
        /// 是否允许修改客观题
        /// </summary>
        public string isAllowModifyObjectAnswer { get; set; }
        /// <summary>
        /// 是否允许考生评试
        /// </summary>
        public string isAllowUserJudgePaper { get; set; }
        /// <summary>
        /// 是否参与积分
        /// </summary>
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
        public string isAllowUserRegExam { get; set; }
        /// <summary>
        /// 报名考试是否需要审批
        /// </summary>
        public string isUserRegExamApprove { get; set; }
        /// <summary>
        /// 评卷最高得分
        /// </summary>
        public decimal? markPaperMaxScore { get; set; }
        /// <summary>
        /// 允许客观题手工评卷
        /// </summary>
        public string isAllowObjectJudge { get; set; }
        /// <summary>
        /// 是否线下考试
        /// </summary>
        public string isOfflineExam { get; set; }

        public string paperName { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public string IsExamGradeRep { get; set; }
    }
    /// <summary>
    /// 检查用户考试
    /// </summary>
    public class CheckUserExtendExam
    {
        public string ExamGradeUid { get; set; }
        public string ReturnCode { get; set; }
    }
}
