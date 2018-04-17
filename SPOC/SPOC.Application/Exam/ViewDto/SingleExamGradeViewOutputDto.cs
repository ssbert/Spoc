using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SPOC.Exam.ViewDto
{
    /// <summary>
    /// 单一考试视图成绩展示
    /// </summary>
    public class SingleExamGradeViewOutputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SingleExamGradeViewOutputDto()
        {
            ExamGradeList = new List<ExamGradeViewItem>();
        }
        /// <summary>
        /// 考试任务标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 成绩列表
        /// </summary>
        public List<ExamGradeViewItem> ExamGradeList;
    }
}