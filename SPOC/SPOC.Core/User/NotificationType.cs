using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.User
{
    /// <summary>
    /// 通知类型
    /// </summary>
    [Table("notification_type")]
    public class NotificationType : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 类型编码
        /// </summary>
        [Column("code"), StringLength(16), Required]
        public string Code { get; set; }
        /// <summary>
        /// 类型名
        /// </summary>
        [Column("name"), StringLength(32), Required]
        public string Name { get; set; }
    }
}