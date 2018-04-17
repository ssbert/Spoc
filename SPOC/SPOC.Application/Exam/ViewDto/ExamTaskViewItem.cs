using System;
using Newtonsoft.Json;

namespace SPOC.Exam.ViewDto
{
    /// <summary>
    /// 考试任务视图项
    /// </summary>
    public class ExamTaskViewItem
    {
        /// <summary>
        /// 考试任务ID
        /// </summary>
        public Guid Id { get; set; }
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
        /// 考试ID
        /// </summary>
        public Guid ExamId { get; set; }
        /// <summary>
        /// 考试开始时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 考试结束时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 考试名
        /// </summary>
        public string ExamName { get; set; }
        /// <summary>
        /// 考试时长（秒）
        /// </summary>
        public int ExamTime { get; set; }
        /// <summary>
        /// 是否主考
        /// </summary>
        public bool IsMainExam { get; set; }
        /// <summary>
        /// 通过得分
        /// </summary>
        public decimal PassGradeScore { get; set; }

        /// <summary>
        /// 试卷ID
        /// </summary>
        public Guid PaperId { get; set; }
        /// <summary>
        /// 试卷名
        /// </summary>
        public string PaperName { get; set; }
        /// <summary>
        /// 试题数
        /// </summary>
        public int QuestionNum { get; set; }
        /// <summary>
        /// 试卷总分
        /// </summary>
        public decimal TotalScore { get; set; }

        /// <summary>
        /// 考试成绩ID
        /// </summary>
        public Guid ExamGradeId { get; set; }
        /// <summary>
        /// 考试成绩
        /// </summary>
        public decimal GradeScore { get; set; }
        /// <summary>
        /// 已最大考试次数
        /// </summary>
        public bool IsMaxExamCount { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 编译完成
        /// </summary>
        public bool IsCompiled { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        public int GradeCount { get; set; }
    }
}