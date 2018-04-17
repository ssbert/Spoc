using Abp.AutoMapper;
using SPOC.Exam;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 随机试卷策略DTO
    /// </summary>
    [AutoMapFrom(typeof(ExamPolicy))]
    public class ExamPolicyOutputDto
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
        public string policyCode { get; set; }

        /// <summary>
        /// 是否自定义编号
        /// </summary>
        public bool isCustomCode { get; set; }

        /// <summary>
        /// 随机试卷名称
        /// </summary>
        public string policyName { get; set; }


        /// <summary>
        /// 试卷类型（exam 考试，task 作业, testing 测评）
        /// </summary>
        public string paperClassCode { get; set; }

        /// <summary>
        /// 单选变不定项
        /// </summary>
        public string isSingleAsMulti { get; set; }

        /// <summary>
        /// 试卷总分
        /// </summary>
        public decimal totalScore { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 难度系数
        /// </summary>
        [StringLength(36)]
        public string paperHardGrade { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? outdatedDate { get; set; }

    }
}