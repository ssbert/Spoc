using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Core
{
    [Table("class_announcement")]
    public class ClassAnnouncement : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid classId { get; set; }

        /// <summary>
        /// 公告ID
        /// </summary>
        public Guid announcementId { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime createTime { get; set; }


    }
}
