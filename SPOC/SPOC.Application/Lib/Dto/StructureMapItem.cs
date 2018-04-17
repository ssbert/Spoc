using System;
using Newtonsoft.Json;

namespace SPOC.Lib.Dto
{
    /// <summary>
    /// 知识图谱项
    /// </summary>
    public class StructureMapItem
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid CreatorId { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 知识图谱标题
        /// </summary>
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
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime CreateTime { get; set; }
    }
}