using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习记录
    /// </summary>
    [Table("exercise_record")]
    public class ExerciseRecord : Entity<Guid>
    {
        public ExerciseRecord()
        {
            Id = Guid.NewGuid();
            CompiledResults = "";
        }

        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 练习ID
        /// </summary>
        [Column("exerciseId")]
        public Guid ExerciseId { get; set; }

        /// <summary>
        /// 学生ID
        /// </summary>
        [Column("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        [Column("isPass")]
        public bool IsPass { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Column("beginTime")]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Column("endTime")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 编译结果
        /// </summary>
        [Column("compiledResults")]
        public string CompiledResults { get; set; }
    }
}