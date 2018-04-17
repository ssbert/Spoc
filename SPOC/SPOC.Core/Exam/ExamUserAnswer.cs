using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 考生的答案(所有试题答案串)
    /// id == ExamGrade.userAnswerUid
    /// </summary>
    public class ExamUserAnswer:Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 考生的答案(所有试题答案串)
        /// </summary>
        public string userAnswer { get; set; }
    }
}