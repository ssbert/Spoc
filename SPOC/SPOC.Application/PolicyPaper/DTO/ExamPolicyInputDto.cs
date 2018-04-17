using Abp.AutoMapper;
using Abp.Runtime.Validation;
using SPOC.Exam;
using System;
using System.ComponentModel.DataAnnotations;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 随机试卷策略DTO
    /// </summary>
    [AutoMapTo(typeof(ExamPolicy))]
    public class ExamPolicyInputDto: IShouldNormalize, ICustomValidate
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 随机试卷分类ID
        /// </summary>
        public Guid folderUid { get; set; }

        /// <summary>
        /// 随机试卷编号
        /// </summary>
        [MaxLength(64)]
        public string policyCode { get; set; }

        /// <summary>
        /// 是否自定义编号
        /// </summary>
        [Required]
        public bool isCustomCode { get; set; }

        /// <summary>
        /// 随机试卷名称
        /// </summary>
        [MaxLength(64)]
        [Required]
        public string policyName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [StringLength(36)]
        public string statusCode { get; set; }

        /// <summary>
        /// 试卷类型（exam 考试，task 作业, testing 测评）
        /// </summary>
        [MaxLength(16)]
        [Required]
        public string paperClassCode { get; set; }

        /// <summary>
        /// 单选变不定项
        /// </summary>
        [MaxLength(1)]
        public string isSingleAsMulti { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
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

        /// <summary>
        /// 学科
        /// </summary>
        public Guid subjectUid { get; set; }

        /// <summary>
        /// 组织架构
        /// </summary>
        public Guid departmentUid { get; set; }

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

            if (string.IsNullOrEmpty(statusCode))
            {
                statusCode = "approved";
            }
        }
    }
}