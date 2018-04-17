using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试任务关联表
    /// </summary>
    [Table("exam_task")]
    public class ExamTask:Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 考试任务名
        /// </summary>
        [Column("title"), StringLength(256), Required]
        public string Title { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [Column("code"), StringLength(64)]
        public string Code { get; set; }

        /// <summary>
        /// 是否是自定义编号
        /// </summary>
        [Column("isCustomCode")]
        public bool IsCustomCode { get; set; }

        /// <summary>
        /// 创建者Id
        /// </summary>
        [Column("creatorId")]
        public Guid CreatorId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createTime")]
        public DateTime CreateTime { get; set; }
    }
}