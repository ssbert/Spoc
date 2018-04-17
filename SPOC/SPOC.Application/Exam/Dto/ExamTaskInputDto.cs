using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.Runtime.Validation;

namespace SPOC.Exam.Dto
{
    /// <summary>
    /// 考试任务 InputDto
    /// </summary>
    [AutoMapTo(typeof(ExamTask))]
    public class ExamTaskInputDto:ICustomValidate
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 考试任务名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 是否自定义编号
        /// </summary>
        public bool IsCustomCode { get; set; }

        /// <summary>
        /// 自定义验证
        /// </summary>
        /// <param name="context"></param>
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (IsCustomCode && string.IsNullOrEmpty(Code))
            {
                context.Results.Add(new ValidationResult("未填写Code"));
            }
        }
    }
}