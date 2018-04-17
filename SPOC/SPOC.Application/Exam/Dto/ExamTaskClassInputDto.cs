using System;
using SPOC.Common.Dto;

namespace SPOC.Exam.Dto
{
    /// <summary>
    /// 考试任务班级 InputDto
    /// </summary>
    public class ExamTaskClassInputDto:IdListInputDto
    {
        /// <summary>
        /// 考试任务Id
        /// </summary>
        public Guid TaskId { get; set; }
    }
}