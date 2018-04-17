using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Runtime.Validation;

namespace SPOC.User.Dto.Notification
{
    /// <summary>
    /// 发送通知Dto
    /// </summary>
    public class NotificationInputDto:ICustomValidate
    {
        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// 通知类型编码
        /// </summary>
        [Required]
        public string TypeCode { get; set; }

        /// <summary>
        /// 班级Ids
        /// </summary>
        [Required]
        public List<Guid> ClassIds { get; set; }

        /// <summary>
        /// 自定义验证规则
        /// </summary>
        /// <param name="context"></param>
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (ClassIds == null || ClassIds.Count == 0)
            {
                context.Results.Add(new ValidationResult("班级不可为空"));
            }
        }
    }
}
