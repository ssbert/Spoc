using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 评卷策略表
    /// </summary>
    public class ExamJudgePolicy : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid examUid { get; set; }
        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }
        /// <summary>
        /// 题型ID
        /// </summary>
        public Guid questionTypeUid { get; set; }
        [ForeignKey("questionTypeUid")]
        public ExamQuestionType QuestinType { get; set; }
        /// <summary>
        /// 评卷策略代码
        /// </summary>
        [StringLength(64)]
        public string judgePolicyCode { get; set; }
        /// <summary>
        /// 策略参数
        /// </summary>
        [StringLength(2000)]
        public string parameter { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }
    }
}
