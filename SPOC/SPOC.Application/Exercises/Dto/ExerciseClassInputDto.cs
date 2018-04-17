using SPOC.Common.Dto;
using System;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 练习任务班级 InputDto
    /// </summary>
    public class ExerciseClassInputDto : IdListInputDto
    {
        /// <summary>
        /// 练习id
        /// </summary>
        public Guid TaskId { get; set; }
    }
}