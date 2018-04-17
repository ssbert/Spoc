using SPOC.Exercises.Dto;

namespace SPOC.Web.Models.Exercises
{
    public class AnswerViewModel
    {
        /// <summary>
        /// 练习基础信息
        /// </summary>
        public ExerciseBaseViewOutputDto Base { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }
    }
}