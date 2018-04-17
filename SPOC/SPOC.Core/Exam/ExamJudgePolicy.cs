using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// ������Ա�
    /// </summary>
    public class ExamJudgePolicy : Entity<Guid>
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
        public Guid questionTypeUid { get; set; }
        [ForeignKey("questionTypeUid")]
        public ExamQuestionType QuestinType { get; set; }
        /// <summary>
        /// ������Դ���
        /// </summary>
        [StringLength(64)]
        public string judgePolicyCode { get; set; }
        /// <summary>
        /// ���Բ���
        /// </summary>
        [StringLength(2000)]
        public string parameter { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        public string remarks { get; set; }
    }
}
