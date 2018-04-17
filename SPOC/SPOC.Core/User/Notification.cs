using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOC.User
{
    /// <summary>
    /// 通知
    /// </summary>
    [Table("notification")]
    public class Notification:Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 通知类型
        /// </summary>
        [Column("typeId")]
        public Guid TypeId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Column("content"), Required]
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 发送者用户的ID
        /// </summary>
        [Column("sendUserId")]
        public Guid SendUserId { get; set; }

    }
}
