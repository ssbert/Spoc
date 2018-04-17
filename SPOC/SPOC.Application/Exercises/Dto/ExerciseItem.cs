using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 练习分页列表项
    /// </summary>
    public class ExerciseItem
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid CreatorId { get; set; }
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 创建者用户名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 发布班级
        /// </summary>
        public Dictionary<Guid, string> Classes { get; set; }
    }
}