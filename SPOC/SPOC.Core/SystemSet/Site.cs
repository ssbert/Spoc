using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.SystemSet
{
    /// <summary>
    /// 站点更新表
    /// </summary>
    public class Site : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        public Site()
        {
            createTime = DateTime.Now;
        }
    }
}
