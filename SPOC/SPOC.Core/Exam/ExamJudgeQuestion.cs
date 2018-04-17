using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// ÷Ã‚∆¿æÌ…Ë÷√
    /// </summary>
    public class ExamJudgeQuestion : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// øº ‘ID
        /// </summary>
        public Guid examUid { get; set; }

        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }

        /// <summary>
        ///  ‘Ã‚ID
        /// </summary>
        public Guid questionUid { get; set; }

        [ForeignKey("questionUid")]
        public ExamQuestion Question { get; set; }

        /// <summary>
        /// ∆¿æÌ»ÀID
        /// </summary>
        public Guid judgeUserUid { get; set; }
        [ForeignKey("judgeUserUid")]
        public User.UserBase User { get; set; }
    }
}
