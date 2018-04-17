using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Core
{
    [Table("announcement")]
    public class Announcement : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 公告标题
        /// </summary>
        [StringLength(50)]
        public string title { get; set; }

        /// <summary>
        /// 课程的内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid createUserId { get; set; }
        /// <summary>
        /// 启用状态 0 不启用 1启用
        /// </summary>
        [DefaultValue(0)]
        [Column("enable")]
        public byte enable { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime updateTime { get; set; }

    }
}
