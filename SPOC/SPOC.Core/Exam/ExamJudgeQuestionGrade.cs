using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// ������������
    /// </summary>
    public class ExamJudgeQuestionGrade : Entity<Guid>
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
        /// ���Լ�¼ID
        /// </summary>
        public Guid examGradeUid { get; set; }

        [ForeignKey("examGradeUid")]
        public ExamGrade grade { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        public Guid questionUid { get; set; }

        [ForeignKey("questionUid")]
        public ExamQuestion Question { get; set; }

        /// <summary>
        /// ������ID
        /// </summary>
        public Guid judgeUserUid { get; set; }
        [ForeignKey("judgeUserUid")]
        public UserBase User { get; set; }

        /// <summary>
        /// ����״̬��judging�����С�judged�����֣�
        /// </summary>
        [StringLength(256)]
        public string statusCode { get; set; }
    }
}
