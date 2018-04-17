using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试考生
    /// </summary>
    public class ExamUser : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        [Required]
        public Guid examUid { get; set; }

        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }

        /// <summary>
        /// 考生ID
        /// </summary>
        [Required]
        public Guid ownerUid { get; set; }
        [ForeignKey("ownerUid")]
        public UserBase User { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        [StringLength(256), DefaultValue("")]
        public string payment { get; set; }
    }
}
