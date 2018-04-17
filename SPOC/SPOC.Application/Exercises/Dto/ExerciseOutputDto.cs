using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using Newtonsoft.Json;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 练习
    /// </summary>
    [AutoMapFrom(typeof(Exercise))]
    public class ExerciseOutputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseOutputDto()
        {
            Classes = new Dictionary<Guid, string>();
        }
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 考试任务标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid QuestionId { get; set; }

        /// <summary>
        /// 试题内容
        /// </summary>
        public string QuestionText { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否显示答案
        /// </summary>
        public bool ShowAnswer { get; set; }

        /// <summary>
        /// 显示答案的类型
        /// 0：考试时间结束后显示
        /// 1：任何时候都显示
        /// </summary>
        public byte ShowAnswerType { get; set; }

        /// <summary>
        /// 班级列表
        /// </summary>
        public Dictionary<Guid, string> Classes { get; set; }
    }
}