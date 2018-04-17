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
    /// 院系
    /// </summary>
    [Table("faculty")]
    public class Faculty:Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 院系编码
        /// </summary>
        [StringLength(20)]
        public string code { get; set; }
        /// <summary>
        /// 院系名称
        /// </summary>
        [StringLength(36)]
        public string name { get; set; }
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
