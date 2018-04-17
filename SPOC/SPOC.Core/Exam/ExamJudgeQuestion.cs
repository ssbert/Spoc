using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// ������������
    /// </summary>
    public class ExamJudgeQuestion : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        public Guid examUid { get; set; }

        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        public Guid questionUid { get; set; }

        [ForeignKey("questionUid")]
        public ExamQuestion Question { get; set; }

        /// <summary>
        /// ������ID
        /// </summary>
        public Guid judgeUserUid { get; set; }
        [ForeignKey("judgeUserUid")]
        public User.UserBase User { get; set; }
    }
}
