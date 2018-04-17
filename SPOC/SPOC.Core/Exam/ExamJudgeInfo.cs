using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试评卷信息表
    /// </summary>
    [Table("exam_judge_info")]
    public class ExamJudgeInfo : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 考生成绩ID
        /// </summary>
        [Column("examGradeUid")]
        public Guid ExamGradeUid { get; set; }

        /// <summary>
        /// 试题ID
        /// </summary>
        [Column("questionUid")]
        public Guid QuestionUid { get; set; }

        /// <summary>
        /// 评卷人ID
        /// </summary>
        [Column("judgeUserUid")]
        public Guid JudgeUserUid { get; set; }

        /// <summary>
        /// 评卷结果状态编号（right为对,error为错,middle为半对）
        /// </summary>
        [Column("judgeResultCode"), StringLength(16)]
        public string JudgeResultCode { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        [Column("judgeScore"), DecimalPrecision(18, 2)]
        public decimal? JudgeScore { get; set; }

        /// <summary>
        /// 评卷批注
        /// </summary>
        [Column("judgeRemarks"), StringLength(256)]
        public string JudgeRemarks { get; set; }

        /// <summary>
        /// 得分点
        /// </summary>
        [Column("judgeScoreText"), StringLength(256)]
        public string JudgeScoreText { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createTime")]
        public DateTime CreateTime { get; set; }
    }
}
