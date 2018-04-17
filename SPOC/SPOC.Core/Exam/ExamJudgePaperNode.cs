using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 逐大题评卷设置
    /// </summary>
    public class ExamJudgePaperNode : Entity<Guid>
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
        /// 试卷大题ID
        /// </summary>
        public Guid paperNodeUid { get; set; }
        [ForeignKey("paperNodeUid")]
        public ExamPaperNode PaperNode { get; set; }
        /// <summary>
        /// 评卷人ID
        /// </summary>
        public Guid judgeUserUid { get; set; }
        [ForeignKey("judgeUserUid")]
        public User.UserBase User { get; set; }
        /// <summary>
        /// 分配评卷数
        /// </summary>
        public int? assign_count { get; set; }
    }
}
