using System;
using Newtonsoft.Json;

namespace SPOC.Exam.GradeDto
{
    /// <summary>
    /// 用户考试记录
    /// </summary>
    public class UserExamRecordOutputDto
    {
        /// <summary>
        /// 成绩Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 开始答题时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束答题时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 成绩
        /// </summary>
        public decimal? GradeScore { get; set; }
    }
}