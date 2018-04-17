using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace SPOC.Lib
{
    /// <summary>
    /// 知识图谱标签
    /// </summary>
    [Table("label_rule")]
    public class LabelRule : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 标签Id
        /// </summary>
        public Guid labelId { get; set; }
        /// <summary>
        /// 匹配关键字
        /// </summary>
        [StringLength(30)]
        public string matchText { get; set; }
        /// <summary>
        /// 逻辑 1与 0或  默认0(或)关系
        /// </summary>
        [DefaultValue(0)]
        public byte logic { get; set; }
        /// <summary>
        /// 说明描述
        /// </summary>
        [StringLength(128)]
        public string describe { get; set; }
        /// <summary>
        /// 正则表达式
        /// </summary>
        [StringLength(128)]
        public string regExpressions { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        public Guid creatorId { get; set; }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int seq { get; set; }
    }
}
