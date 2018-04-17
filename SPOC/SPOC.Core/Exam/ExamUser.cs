using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// ���Կ���
    /// </summary>
    public class ExamUser : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        [Required]
        public Guid examUid { get; set; }

        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        [Required]
        public Guid ownerUid { get; set; }
        [ForeignKey("ownerUid")]
        public UserBase User { get; set; }

        /// <summary>
        /// ���ʽ
        /// </summary>
        [StringLength(256), DefaultValue("")]
        public string payment { get; set; }
    }
}
