using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Newtonsoft.Json;

namespace SPOC.SystemSet
{
    [Table("faq")]
    public class Faq : Entity<Guid>, IPassivable
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(50)]
        public string title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 分类ID
        /// </summary>
        public Guid folderId { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime updateTime { get; set; }
        /// <summary>
        /// 加入序号
        /// </summary>
        public int seq { get; set; }
        /// <summary>
        /// 标记有用人次
        /// </summary>
        public int userFul { get; set; }
        /// <summary>
        /// 标记无用人次
        /// </summary>
        public int userLess { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DefaultValue(true)]
        public virtual bool IsActive { get; set; }
        /// <summary>
        /// 是否来自云
        /// </summary>
        [DefaultValue(false)]
        public virtual bool FromCloud { get; set; }
    }
}
