using SPOC.QuestionBank.Dto;
using System;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 学生练习作答
    /// </summary>
    public class ExerciseAnswerOutputDto
    {
        /// <summary>
        /// 练习记录ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 试题信息
        /// </summary>
        public ExamQuestionDto Question { get; set; }
        /// <summary>
        /// 用户答案
        /// </summary>
        public string UserAnswer { get; set; }
        /// <summary>
        /// 编译结果
        /// </summary>
        public string CompiledResults { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
    }
}