using Abp.AutoMapper;
using SPOC.Exam;
using System;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 随机试卷大题DTO
    /// </summary>
    [AutoMapFrom(typeof(ExamPolicyNode))]
    public class ExamPolicyNodeOutputDto
    {
        /// <summary>
        /// id
        /// </summary>
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
        /// 题型名称
        /// </summary>
        public string questionTypeName { get; set; }
        /// <summary>
        /// 随机试卷大题标题
        /// </summary>
        public string policyNodeName { get; set; }
        /// <summary>
        /// 试题数
        /// </summary>
        public int questionNum { get; set; }
        /// <summary>
        /// 大题总分
        /// </summary>
        public decimal totalScore { get; set; }
        /// <summary>
        /// 大题顺序
        /// </summary>
        public int listOrder { get; set; }
        /// <summary>
        /// 大题说明
        /// </summary>
        public string policyNodeDesc { get; set; }
    }
}