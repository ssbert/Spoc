using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.SystemSet
{
    /// <summary>
    /// 系统日志表
    /// </summary>
    [Serializable]
    [Table("system_log")]
    public class SystemLog : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 用户的ID
        /// </summary>
        public Guid userId { get; set; }

        [StringLength(50)]
        public string operateName { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [StringLength(255)]
        public string module { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        [StringLength(255)]
        public string level { get; set; }

        /// <summary>
        /// 用户ip
        /// </summary>
        [StringLength(255)]
        public string ip { get; set; }

        /// <summary>
        /// 记录的信息内容
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 具体内容
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 站点来源ID
        /// </summary>
        public Guid relateuid { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [StringLength(128)]
        public string creator { get; set; }
    }
}
