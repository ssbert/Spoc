/********************************************************************************
** auth： bert
** date： 2016/4/27 19:44:43
** desc： 
*********************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace SPOC.User.Dto
{
    public class CreateDemoInputDto : EntityDto<Guid>, ICustomValidate
    {
       [Required]
        public string name { get; set; }
       [Required]
        public string code { get; set; }
        public string idCard { get; set; }
        public DateTime? LastModificationTime { get; set; }


        public void AddValidationErrors(CustomValidationContext context)
        {
            if (code == null && name == null)
            {
                context.Results.Add(new ValidationResult("name和code不能同时为空!", new[] { "name", "code" }));
            }
        }
    }
}


