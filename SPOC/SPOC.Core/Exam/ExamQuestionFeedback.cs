using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// ���ⷴ��
    /// </summary>
    public class ExamQuestionFeedback : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        public Guid questionUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("questionUid")]
        public ExamQuestion Question { get; set; }

        /// <summary>
        /// �û�ID
        /// </summary>
        public Guid userUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("userUid")]
        public UserBase User { get; set; }

        /// <summary>
        /// �û���
        /// </summary>
        [StringLength(64)]
        public string userName { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// �����Ծ�ID
        /// </summary>
        public Guid paperUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("paperUid")]
        public ExamPaper Paper { get; set; }

        /// <summary>
        /// ���Գɼ�ID
        /// </summary>
        public Guid examGradeUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("examGradeUid")]
        public ExamGrade Grade { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        public Guid examUid { get; set; }

        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }

        /// <summary>
        /// ״̬
        /// </summary>
        [StringLength(16)]
        public string statusCode { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// �������û�ID
        /// </summary>
        [StringLength(64)]
        public string processUser { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public string result { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime updateTime { get; set; }
    }
}
