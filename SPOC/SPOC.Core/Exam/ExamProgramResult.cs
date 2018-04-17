using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试编程题编译结果
    /// </summary>
    [Table("exam_program_result")]
    public class ExamProgramResult : Entity<Guid>
    {
        public ExamProgramResult()
        {
            Id = Guid.NewGuid();
            Result = "";
        }
        /// <summary>
        /// ID
        /// </summary>
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 成绩Id
        /// </summary>
        [Column("gradeId")]
        public Guid GradeId { get; set; }

        /// <summary>
        /// 编程题ID
        /// </summary>
        [Column("questionId")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// 编译运行结果
        /// </summary>
        [Column("result")]
        public string Result { get; set; }
    }
}