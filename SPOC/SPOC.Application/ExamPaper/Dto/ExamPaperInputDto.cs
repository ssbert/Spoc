using Abp.AutoMapper;
using Abp.Runtime.Validation;
using SPOC.Exam;
using System;
using System.ComponentModel.DataAnnotations;

namespace SPOC.ExamPaper.Dto
{
    [AutoMap(typeof(Exam.ExamPaper), typeof(ExamPolicy))]
    public class ExamPaperInputDto: IShouldNormalize, ICustomValidate
    {
       public Guid Id { get; set; }

        [MaxLength(64)]
        public string paperCode { get; set; }

        [Required]
        public bool isCustomCode { get; set; }

        [Required, MaxLength(256)]
        public string paperName { get; set; }
        
        /// <summary>
        /// 单选变不定项
        /// </summary>
        [Required, MaxLength(1)]
        public string isSingleAsMulti { get; set; }

        [Required, MaxLength(16)]
        public string paperTypeCode { get; set; }

        public string policyUid { get; set; }

        [MaxLength(256)]
        public string remarks { get; set; }
        /// <summary>
        /// 难度系数
        /// </summary>
        [MaxLength(36)]
        public string paperHardGrade { get; set; }
        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime? outdatedDate { get; set; }

        public Guid folderUid { get; set; }

        /// <summary>
        /// 初始化默认值
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(paperHardGrade))
            {
                paperHardGrade = "";
            }
            if (string.IsNullOrEmpty(remarks))
            {
                remarks = "";
            }
           
        }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!string.IsNullOrEmpty(isSingleAsMulti) && isSingleAsMulti != "N" && isSingleAsMulti != "Y")
            {
                context.Results.Add(new ValidationResult("isSingleAsMulti 的值必须是 'N' or 'Y'"));
            }

            if (paperTypeCode != "fix" && paperTypeCode != "random")
            {
                context.Results.Add(new ValidationResult("paperTypeCode 的值必须是 'fix' or 'random'"));
            }
        }
    }
}