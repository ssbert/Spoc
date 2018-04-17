using System;

namespace SPOC.Exam.Dto
{
    /// <summary>
    /// 补考候选列表项
    /// </summary>
    public class RetestComboboxItem
    {
        /// <summary>
        /// 考试任务ID
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid ExamId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}