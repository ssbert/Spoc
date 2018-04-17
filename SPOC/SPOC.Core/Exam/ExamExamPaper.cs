using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// �����õ�����Ծ������, 1���� �� n����Ծ�
    /// </summary>
    public class ExamExamPaper : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        public Guid examUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }
        /// <summary>
        /// �Ծ�ID
        /// </summary>
        public Guid paperUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("paperUid")]
        public ExamPaper Paper { get; set; }
        /// <summary>
        /// �Ƿ����
        /// </summary>
        [StringLength(1)]
        [DefaultValue("Y")]
        public string isActive { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime createTime { get; set; }
    }
}
