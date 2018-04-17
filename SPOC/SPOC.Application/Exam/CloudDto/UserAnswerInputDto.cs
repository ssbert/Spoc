using System;
using System.Collections.Generic;

namespace SPOC.Exam.CloudDto
{
    /// <summary>
    /// 程序题提交新课云
    /// </summary>
    public class UserAnswerInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserAnswerInputDto()
        {
            UserAnswers = new List<UserAnswerDto>();
        }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid ExamId { get; set; }
        /// <summary>
        /// 成绩ID
        /// </summary>
        public Guid GradeId { get; set; }
        /// <summary>
        /// 答题列表
        /// </summary>
        public List<UserAnswerDto> UserAnswers { get; set; }
        /// <summary>
        /// 云端分配的ID（记录数据来源）
        /// </summary>
        public int CloudId { get; set; }
    }
}