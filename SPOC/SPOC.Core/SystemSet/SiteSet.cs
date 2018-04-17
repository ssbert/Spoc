using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.SystemSet
{
    /// <summary>
    /// 站点设置表
    /// </summary>
    [Serializable]
    [Table("site_set")]
    public class SiteSet : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        [StringLength(255)]
        public string settingGroup { get; set; }

        /// <summary>
        /// key
        /// </summary>
        [StringLength(255)]
        public string settingKey { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(255)]
        public string settingName { get; set; }

        /// <summary>
        /// 设置值
        /// </summary>
        public string settingValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string settingRemark { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime modifyTime { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [StringLength(10)]
        public string isVisible { get; set; }
    }
}
