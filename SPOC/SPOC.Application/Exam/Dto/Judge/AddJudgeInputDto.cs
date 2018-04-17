using System;
using System.Collections.Generic;

namespace SPOC.Exam.Dto.Judge
{
    /// <summary>
    /// 添加评卷人input dto
    /// </summary>
    public class AddJudgeInputDto
    {
        /// <summary>
        /// 教师IdList
        /// </summary>
        public List<Guid> UserIdList { get; set; }
        /// <summary>
        /// 考试IdList
        /// </summary>
        public Guid ExamId { get; set; }
    }
}
