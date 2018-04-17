using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试成绩发布者
    /// </summary>
    public class ExamPublish : Entity<Guid>
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
        /// 发布者ID
        /// </summary>
        public Guid ownerUid { get; set; }
        [ForeignKey("ownerUid")]
        public UserBase User { get; set; }

    }
}
