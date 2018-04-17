using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace SPOC.User
{
    /// <summary>
    /// 专业
    /// </summary>
    [Table("major")]
    public class Major:Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 专业ID
        /// </summary>
        public Guid facultyId { get; set; }
        /// <summary>
        /// 专业名称
        /// </summary>
        [StringLength(36)]
        public string name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [StringLength(20)]
        public string code { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public Guid updateUserId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }
    }
}
