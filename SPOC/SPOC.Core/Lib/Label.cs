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
    /// 知识图谱知识点
    /// </summary>
    [Table("label")]
    public class Label : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 知识点
        /// </summary>
        [StringLength(30)]
        public string title { get; set; }
        /// <summary>
        /// 知识点分类
        /// </summary>
        public Guid folderId { get; set; }

        /// <summary>
        /// 描述
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
        
    }
}
