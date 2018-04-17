using System;

namespace SPOC.Exam.ViewDto
{
    /// <summary>
    /// 考试成绩视图列表
    /// </summary>
    public class ExamGradeViewItem
    {
        /// <summary>
        /// 成绩Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 开始答题时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束答题时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 是否编译完毕
        /// </summary>
        public bool IsCompiled { get; set; }
        /// <summary>
        /// 成绩
        /// </summary>
        public decimal? GradeScore { get; set; }
    }
}