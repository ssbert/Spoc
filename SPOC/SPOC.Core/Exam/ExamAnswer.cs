using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// ������
    /// </summary>
    public class ExamAnswer : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// �����ɼ�ID
        /// </summary>
        public Guid examGradeUid { get; set; }
         [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("examGradeUid")]
        public ExamGrade Grade { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        public Guid questionUid { get; set; }
         [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("questionUid")]
        public ExamQuestion Question { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [StringLength(7600)]
        public string answerText { get; set; }
        /// <summary>
        /// ����ʱ��(����Ϊ��λ)
        /// </summary>
        public int answerTime { get; set; }
        /// <summary>
        /// �÷�
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal judgeScore { get; set; }
        /// <summary>
        /// ������̬���
        /// </summary>
        [StringLength(16)]
        public string judgeResultCode { get; set; }
        /// <summary>
        /// ������ע
        /// </summary>
        [StringLength(256)]
        public string judgeRemarks { get; set; }
        /// <summary>
        /// �÷ֵ�
        /// </summary>
        [StringLength(256)]
        public string judgeScoreText { get; set; }
        /// <summary>
        /// ������ID
        /// </summary>
        public Guid judgeUserUid { get; set; }
      
        /// <summary>
        /// ����������
        /// </summary>
        [StringLength(256)]
        public string judgeUserName { get; set; }
    }
}
