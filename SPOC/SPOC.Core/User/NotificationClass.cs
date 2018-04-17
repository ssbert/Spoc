using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.User
{
    /// <summary>
    /// 通知与班级关系
    /// </summary>
    [Table("notification_class")]
    public class NotificationClass : Entity<Guid>
    {
        /// <summary>
        /// 通知Id
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