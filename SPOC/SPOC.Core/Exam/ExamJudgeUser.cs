using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// ������������
    /// </summary>
    public class ExamJudgeUser : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// ���Լ�¼ID
        /// </summary>
        public Guid examGradeUid { get; set; }
        [ForeignKey("examGradeUid")]
        public ExamGrade Grade { get; set; }
        /// <summary>
        /// ������ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid judgeUserUid { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        public Guid examUid { get; set; }
        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid userUid { get; set; }
        /// <summary>
        /// ����״̬
        /// </summary>
        [StringLength(256)]
        public string statusCode { get; set; }
    }
}
