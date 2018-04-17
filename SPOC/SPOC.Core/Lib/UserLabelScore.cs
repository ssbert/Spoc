using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Lib
{
    /// <summary>
    /// 用户标签积分表
    /// </summary>
    [Table("user_label_score")]
    public class UserLabelScore:Entity<Guid>
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Column("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 标签ID
        /// </summary>
        [Column("labelId")]
        public Guid LabelId { get; set; }

        /// <summary>
        /// 标签分值
        /// </summary>
        [Column("score")]
        public int Score { get; set; }
    }
}