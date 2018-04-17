using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Lib
{
    /// <summary>
    /// 知识图谱
    /// </summary>
    [Table("structure_map")]
    public class StructureMap : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        [Column("creatorId")]
        public Guid CreatorId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Column("title"), StringLength(128)]
        public string Title { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        [Column("isShow")]
        public bool IsShow { get; set; }
        /// <summary>
        /// 是否为主知识图谱
        /// </summary>
        public bool IsMain { get; set; }
        /// <summary>
        /// 知识图谱拓扑数据
        /// </summary>
        [Column("mapData")]
        public string MapData { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}