using System;

namespace SPOC.Exam.Dto
{
    /// <summary>
    /// 学生编程题成绩
    /// </summary>
    public class UserExamCompileScore
    {
        /// <summary>
        /// 学生ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 考试Id
        /// </summary>
        public Guid ExamId { get; set; }
        /// <summary>
        /// 成绩ID
        /// </summary>
        public Guid GradeId { get; set; }
        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 得分比率
        /// </summary>
        public float GradeRate { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 是否通过（判断依据来源结果是否正确）1通过 0不通过
        /// </summary>
        public int IsPass { get; set; }
    }
}