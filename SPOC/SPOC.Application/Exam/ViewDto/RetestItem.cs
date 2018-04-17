using System;

namespace SPOC.Exam.ViewDto
{
    /// <summary>
    /// 补考列表项
    /// </summary>
    public class RetestItem
    {
        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid ExamId { get; set; }
        /// <summary>
        /// 补考名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 补考创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}