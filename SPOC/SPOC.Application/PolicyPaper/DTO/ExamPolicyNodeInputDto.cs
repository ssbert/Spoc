using Abp.AutoMapper;
using SPOC.Exam;
using System;
using System.ComponentModel.DataAnnotations;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 随机试卷大题DTO
    /// </summary>
    [AutoMapTo(typeof(ExamPolicyNode))]
    public class ExamPolicyNodeInputDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 随机试卷ID
        /// </summary>
        public Guid policyUid { get; set; }
        /// <summary>
        /// 题型（可以为空只是辅助作用）
        /// </summary>
        public Guid questionTypeUid { get; set; }
        /// <summary>
        /// 随机试卷大题标题
        /// </summary>
        [Required, MaxLength(64)]
        public string policyNodeName { get; set; }
        /// <summary>
        /// 大题说明
        /// </summary>
        public string policyNodeDesc { get; set; }
    }
}