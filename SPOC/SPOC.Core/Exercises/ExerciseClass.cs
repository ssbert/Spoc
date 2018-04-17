using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习发布到的班级
    /// </summary>
    [Table("exercise_class")]
    public class ExerciseClass : Entity<Guid>
    {
        /// <summary>
        /// 练习ID
        /// </summary>
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        [Column("classId")]
        public Guid ClassId { get; set; }
    }
}