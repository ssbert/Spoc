using System;
using Newtonsoft.Json;

namespace SPOC.Exam.Dto
{
    /// <summary>
    /// 考试基本信息
    /// </summary>
    public class ExamItem
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 考试编码
        /// </summary>
        public string ExamCode { get; set; }

        /// <summary>
        /// 考试标题
        /// </summary>
        public string ExamName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 考试时长
        /// </summary>
        public int ExamTime { get; set; }

        /// <summary>
        /// 通过考试判断类型（passGradeRate | passGradeScore）
        /// </summary>
        public string PassGradeType { get; set; }

        /// <summary>
        /// 通过的分数
        /// </summary>
        public decimal? PassGradeScore { get; set; }

        /// <summary>
        /// 通过率
        /// </summary>
        public decimal? PassGradeRate { get; set; }

        /// <summary>
        /// 最多允许参加次数
        /// </summary>
        public int? MaxExamNum { get; set; }

        /// <summary>
        /// 是否是补考
        /// </summary>
        public string ExamTypeCode { get; set; }
    }
}