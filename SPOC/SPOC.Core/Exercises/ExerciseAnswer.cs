using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习答案
    /// </summary>
    [Table("exercise_answer")]
    public class ExerciseAnswer : Entity<Guid>
    {
        /// <summary>
        /// 练习记录ID
        /// </summary>
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        [Column("answer")]
        public string Answer { get; set; }
    }
}