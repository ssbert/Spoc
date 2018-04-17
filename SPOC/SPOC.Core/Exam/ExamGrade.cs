using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// 考生成绩
    /// </summary>
    public class ExamGrade : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 考生ID
        /// </summary>
        public Guid userUid { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid examUid { get; set; }
        /// <summary>
        /// 试卷ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid paperUid { get; set; }
        /// <summary>
        /// 试卷总分
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? paperTotalScore { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime beginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime lastUpdateTime { get; set; }
        /// <summary>
        /// 允许考试时长（以秒为单位）
        /// </summary>
        public int? allowExamTime { get; set; }
        /// <summary>
        /// 答题时间
        /// </summary>
        public int? examTime { get; set; }
        /// <summary>
        /// 成绩
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? gradeScore { get; set; }
        /// <summary>
        /// 得分率
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? gradeRate { get; set; }
        /// <summary>
        /// 客观题分数
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? externalScore { get; set; }
        /// <summary>
        /// 主观题分数
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? subjectiveScore { get; set; }
        /// <summary>
        /// 评卷人ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid judgeUserUid { get; set; }
        /// <summary>
        /// 评卷人姓名
        /// </summary>
        [StringLength(64)]
        public string judgeUserName { get; set; }
        /// <summary>
        /// 评卷开始时间
        /// </summary>
        public DateTime? judgeBeginTime { get; set; }
        /// <summary>
        /// 评卷结束时间
        /// </summary>
        public DateTime? judgeEndTime { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        [StringLength(1)]
        public string isPass { get; set; }
        /// <summary>
        /// 成绩排名
        /// </summary>
        public int? gradeOrder { get; set; }
        /// <summary>
        /// 成绩状态（release已发布、submitted已提交、judged已评卷、pause暂停中、examing考试中、judging评卷中）
        /// </summary>
        [StringLength(16)]
        public string gradeStatusCode { get; set; }
        /// <summary>
        /// 试卷打乱顺序后的ID串（用在逐题考试时记下打乱后的顺序）
        /// </summary>
        public string paperQuestionUids { get; set; }
        /// <summary>
        /// 当前考试的试题顺序号（用在逐题考试时记下当前试题顺序号）
        /// </summary>
        public int? currentQuestionIndex { get; set; }
        /// <summary>
        /// 是否已经生成测评报告（只对测评系统时起作用）
        /// </summary>
        [StringLength(1)]
        [DefaultValue("N")]
        public string hasCreateReport { get; set; }
        /// <summary>
        /// 是否保存了答案到库中
        /// </summary>
        [StringLength(1)]
        [DefaultValue("Y")]
        public string hasSaveAnswerToDb { get; set; }
        /// <summary>
        /// 考生的答案ID (ExamUserAnswer)
        /// </summary>
         [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid userAnswerUid { get; set; }
       
        /// <summary>
        /// 已通过关数
        /// </summary>
        public int? passGateNum { get; set; }
        /// <summary>
        /// 考试结果统计类型(computer 机试考, paper 纸质考试, failure 失效试卷)
        /// </summary>
        [StringLength(36)]
        [DefaultValue("computer")]
        public string examResultType { get; set; }
        /// <summary>
        /// 参加考试IP
        /// </summary>
        [StringLength(36)]
        public string lastExamIp { get; set; }
        /// <summary>
        /// 是否为补考成绩
        /// </summary>
        [StringLength(1)]
        [DefaultValue("N")]
        public string isExamination { get; set; }
        /// <summary>
        /// 未答试题数
        /// </summary>
        public int? noAnswerQuestionNum { get; set; }
        /// <summary>
        /// 来源(此字段应用在分布式考试功能中，表示当前成绩是从哪个分支上传过来的，其值为分支站点的编号)
        /// </summary>
        [StringLength(64)]
        public string source { get; set; }

        /// <summary>
        /// 是否生成答卷
        /// </summary>
        [StringLength(1)]
        public string hasCreateAnswerPaper { get; set; }

        /// <summary>
        /// 是否编译完成
        /// </summary>
        [Column("isCompiled"), DefaultValue(true)]
        public bool IsCompiled { get; set; }
    }
}
