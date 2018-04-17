using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// øº ‘∆¿æÌ»À
    /// </summary>
    public class ExamJudge : Entity<Guid>
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
        /// ∆¿æÌ»ÀID
        /// </summary>
        public Guid ownerUid { get; set; }
        [ForeignKey("ownerUid")]
        public User.UserBase User { get; set; }

    }
}
