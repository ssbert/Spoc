using System;

namespace SPOC.Exam.Dto
{
    /// <summary>
    /// 考试程序题编译得分
    /// </summary>
    public class ExamCompileScore
    {
        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 得分比率（0.0 至 1.0）
        /// </summary>
        public float GradeRate { get; set; }
        /// <summary>
        /// 执行结果，多结果用“|”分隔
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 是否通过（判断依据来源结果是否正确）1通过 0不通过
        /// </summary>
        public int IsPass { get; set; }
    }
}