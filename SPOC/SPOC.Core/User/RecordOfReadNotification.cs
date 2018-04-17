using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.User
{
    /// <summary>
    /// 已读通知记录
    /// </summary>
    [Table("record_of_read_notification")]
    public class RecordOfReadNotification:Entity<Guid>
    {
        /// <summary>
        /// 通知Id
        /// </summary>
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [Column("userId")]
        public Guid UserId { get; set; }
    }
}