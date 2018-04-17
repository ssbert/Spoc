using System;

namespace SPOC.Exam.CloudDto
{
    /// <summary>
    /// 用户答案
    /// </summary>
    public class UserAnswerDto
    {
        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 答案（代码）
        /// </summary>
        public string Answer { get; set; }
    }
}