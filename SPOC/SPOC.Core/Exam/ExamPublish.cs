using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// ���Գɼ�������
    /// </summary>
    public class ExamPublish : Entity<Guid>
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
        /// ������ID
        /// </summary>
        public Guid ownerUid { get; set; }
        [ForeignKey("ownerUid")]
        public UserBase User { get; set; }

    }
}
