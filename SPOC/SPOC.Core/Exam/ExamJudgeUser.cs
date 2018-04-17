using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// ÷»À∆¿æÌ…Ë÷√
    /// </summary>
    public class ExamJudgeUser : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// øº ‘º«¬ºID
        /// </summary>
        public Guid examGradeUid { get; set; }
        [ForeignKey("examGradeUid")]
        public ExamGrade Grade { get; set; }
        /// <summary>
        /// ∆¿æÌ»ÀID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid judgeUserUid { get; set; }
        /// <summary>
        /// øº ‘ID
        /// </summary>
        public Guid examUid { get; set; }
        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }
        /// <summary>
        /// øº…˙ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid userUid { get; set; }
        /// <summary>
        /// ∆¿æÌ◊¥Ã¨
        /// </summary>
        [StringLength(256)]
        public string statusCode { get; set; }
    }
}
