using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.SystemSet
{
    [Table("site_version")]
    public class SiteVersion : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        [StringLength(255)]
        [DefaultValue("")]
        public string version { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        [StringLength(255)]
        [DefaultValue("")]
        public string url { get; set; }


        /// <summary>
        /// 更新说明
        /// </summary>
        [StringLength(255)]
        [DefaultValue("")]
        public string updateInfo { get; set; }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        [DefaultValue(false)]
        public bool isForceUpdate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createtime { get; set; }

        /// <summary>
        /// 平台 1：android 2：ios
        /// </summary>
        public int platform { get; set; }


    }
}
