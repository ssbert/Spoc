using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// ExamTask 与 Class 关联关系
    /// </summary>
    [Table("exam_task_class")]
    public class ExamTaskClass:Entity<Guid>
    {
        /// <summary>
        /// ExamTask Id
        /// </summary>
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// Class Id
        /// </summary>
        [Column("classId")]
        public Guid ClassId { get; set; }
    }
}