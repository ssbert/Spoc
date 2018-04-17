using Abp.AutoMapper;
using Abp.Runtime.Validation;
using SPOC.Exam;
using System;
using System.ComponentModel.DataAnnotations;

namespace SPOC.ExamPaper.Dto
{
    [AutoMapTo(typeof(ExamPaperNodeQuestion))]
    public class ExamPaperNodeQuestionInputDto: ICustomValidate
    {
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// 试卷大题ID
        /// </summary>
        [Required]
        public Guid paperNodeUid { get; set; }
        /// <summary>
        /// 试题ID
        /// </summary>
        [Required]
        public Guid questionUid { get; set; }
        /// <summary>
        /// 试卷ID（为了方便查询的冗余字段）
        /// </summary>
        [Required]
        public Guid paperUid { get; set; }
        /// <summary>
        /// 试题在试卷中的分数
        /// </summary>
        public decimal paperQuestionScore { get; set; }

        /// <summary>
        /// 答题时限（以秒为单位）
        /// </summary>
        public int paperQuestionExamTime { get; set; }

        /// <summary>
        /// 试题顺序
        /// </summary>
        public int listOrder { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {

            if (paperUid == Guid.Empty)
            {
                context.Results.Add(new ValidationResult("paperUid为必填字段"));
            }

            if (paperNodeUid == Guid.Empty)
            {
                context.Results.Add(new ValidationResult("paperNodeUid为必填字段"));
            }

            if (questionUid == Guid.Empty)
            {
                context.Results.Add(new ValidationResult("questionUid为必填字段"));
            }
        }
    }
}