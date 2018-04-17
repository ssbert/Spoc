using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// �������������
    /// </summary>
    public class ExamJudgePaperNode : Entity<Guid>
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
        /// �Ծ����ID
        /// </summary>
        public Guid paperNodeUid { get; set; }
        [ForeignKey("paperNodeUid")]
        public ExamPaperNode PaperNode { get; set; }
        /// <summary>
        /// ������ID
        /// </summary>
        public Guid judgeUserUid { get; set; }
        [ForeignKey("judgeUserUid")]
        public User.UserBase User { get; set; }
        /// <summary>
        /// ����������
        /// </summary>
        public int? assign_count { get; set; }
    }
}
