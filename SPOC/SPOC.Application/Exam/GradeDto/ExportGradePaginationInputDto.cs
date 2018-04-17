using Abp.Runtime.Validation;
using SPOC.Common.Pagination;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SPOC.Exam.GradeDto
{
    public class ExportGradePaginationInputDto:PaginationInputDto,ICustomValidate,IShouldNormalize
    {
        public Guid examUid { get; set; }
        public string userLoginName { get; set; }
        public string userFullName { get; set; }
        /// <summary>
        /// 成绩状态（examing考试中、submitted已提交、judging评卷中、judged已评分）
        /// </summary>
        public string gradeStatusCode { get; set; }
        public decimal? minGradeScore { get; set; }
        public decimal? maxGradeScore { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(userLoginName))
            {
                userLoginName = "";
            }

            if (string.IsNullOrEmpty(userFullName))
            {
                userFullName = "";
            }
        }

        public void AddValidationErrors(CustomValidationContext context)
        {
            var values = new[] { "examing", "submitted", "judging", "judged", "release" };
            if (!string.IsNullOrEmpty(gradeStatusCode) && !values.Contains(gradeStatusCode))
            {
                context.Results.Add(new ValidationResult("gradeStatusCode赋值只能为examing、submitted、judging、judged、release"));
            }
        }
    }
}
