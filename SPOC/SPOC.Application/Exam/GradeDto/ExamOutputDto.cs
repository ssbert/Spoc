using System;
using System.Collections.Generic;

namespace SPOC.Exam.GradeDto
{
    /// <summary>
    /// 课程考试管理列表数据
    /// </summary>
    public class ExamOutputDto
    {
        /// <summary>
        /// examUid
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 考试名称
        /// </summary>
        public string examName { get; set; }
        /// <summary>
        /// 考试编号
        /// </summary>
        public string examCode { get; set; }
        /// <summary>
        /// 试卷类型
        /// </summary>
        public string paperTypeCode { get; set; }
        /// <summary>
        /// 考试时间安排
        /// </summary>
        public List<DateTime?[]> examArranges { get; set; }
    }
}
