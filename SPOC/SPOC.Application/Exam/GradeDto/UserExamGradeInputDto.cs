using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SPOC.Exam.GradeDto
{
    /// <summary>
    /// 用户成绩更新
    /// </summary>
    public class UserExamGradeInputDto : ICustomValidate
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 成绩
        /// </summary>
        public decimal? GradeScore { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        [StringLength(1)]
        public string IsPass { get; set; }

        /// <summary>
        /// 成绩状态（examing考试中、submitted已提交、judging评卷中、judged已评分）
        /// </summary>
        [StringLength(16)]
        public string GradeStatusCode { get; set; }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="context"></param>
        public void AddValidationErrors(CustomValidationContext context)
        {
            var values = new[] { "Y", "N" };
            if (!values.Contains(IsPass))
            {
                context.Results.Add(new ValidationResult("isPass的值只能为 N or Y"));
            }

            var values2 = new[] { "examing", "submitted", "judging", "judged", "release" };
            if (!values2.Contains(GradeStatusCode))
            {
                context.Results.Add(new ValidationResult("gradeStatusCode赋值只能为examing、submitted、judging、judged、release"));
            }
        }
    }
}
