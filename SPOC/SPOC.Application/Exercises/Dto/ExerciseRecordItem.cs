using System;
using Newtonsoft.Json;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 练习记录项
    /// </summary>
    public class ExerciseRecordItem
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime EndTime { get; set; }
    }
}