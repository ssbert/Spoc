using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 随机试卷策略项知识点
    /// </summary>
    [Table("exam_policy_item_label")]
    public class ExamPolicyItemLabel : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 策略项ID
        /// </summary>
        [Column("itemId")]
        public Guid ItemId { get; set; }
        /// <summary>
        /// 知识点ID
        /// </summary>
        [Column("labelId")]
        public Guid LabelId { get; set; }
    }
}