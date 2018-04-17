using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SPOC.Exam.Dto
{
    /// <summary>
    /// 考试信息
    /// </summary>
    public class ExamTaskItem
    {
        /// <summary>
        /// Task Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 考试任务标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 创建者Id
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
        /// 发布班级
        /// </summary>
        public Dictionary<Guid, string> Classes { get; set; }
    }
}