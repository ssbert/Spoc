using SPOC.Exam;
using SPOC.Exercises.Dto;
using SPOC.QuestionBank.Dto;

namespace SPOC.Web.Models.Exercises
{
    public class ExerciseViewModel
    {
        /// <summary>
        /// 练习基础信息
        /// </summary>
        public ExerciseBaseViewOutputDto Base { get; set; }
        
        /// <summary>
        /// 试题信息
        /// </summary>
        public ExamQuestionDto Question { get; set; }

        /// <summary>
        /// 允许黏贴代码
        /// </summary>
        public bool AllowPasteCode { get; set; }
    }
}