using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPOC.ExamPaper.Dto
{
    public class ExamPaperNodeQuestionCreateInputDto: ICustomValidate
    {
        [Required]
        public Guid paperNodeUid { get; set; }
        [Required]
        public Guid paperUid { get; set; }
        [Required]
        public List<Guid> questionUidList { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (questionUidList == null || questionUidList.Count == 0)
            {
                context.Results.Add(new ValidationResult("questionUidList不可为空"));
            }
        }
    }
}