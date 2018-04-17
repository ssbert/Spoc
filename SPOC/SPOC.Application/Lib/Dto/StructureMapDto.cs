using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace SPOC.Lib.Dto
{
    /// <summary>
    /// 知识图谱
    /// </summary>
    [AutoMap(typeof(StructureMap))]
    public class StructureMapDto
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(128), Required]
        public string Title { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 是否为主知识图谱
        /// </summary>
        public bool IsMain { get; set; }
        /// <summary>
        /// 知识图谱拓扑数据
        /// </summary>
        public string MapData { get; set; }
    }
}