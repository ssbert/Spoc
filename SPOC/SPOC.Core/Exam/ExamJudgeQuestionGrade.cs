using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// ∞¥Ã‚∆¿æÌ∑÷≈‰±Ì
    /// </summary>
    public class ExamJudgeQuestionGrade : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// øº ‘ID
        /// </summary>
        public Guid examUid { get; set; }

        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }

        /// <summary>
        /// øº ‘º«¬ºID
        /// </summary>
        public Guid examGradeUid { get; set; }

        [ForeignKey("examGradeUid")]
        public ExamGrade grade { get; set; }

        /// <summary>
        ///  ‘Ã‚ID
        /// </summary>
        public Guid questionUid { get; set; }

        [ForeignKey("questionUid")]
        public ExamQuestion Question { get; set; }

        /// <summary>
        /// ∆¿æÌ»ÀID
        /// </summary>
        public Guid judgeUserUid { get; set; }
        [ForeignKey("judgeUserUid")]
        public UserBase User { get; set; }

        /// <summary>
        /// ∆¿æÌ◊¥Ã¨£®judging∆¿æÌ÷–°¢judged“—∆¿∑÷£©
        /// </summary>
        [StringLength(256)]
        public string statusCode { get; set; }
    }
}
