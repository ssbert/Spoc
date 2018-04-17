using System;
using System.Collections.Generic;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 随机试卷策略项组
    /// </summary>
    public class ExamPolicyItemGroupOutputDto
    {
        /// <summary>
        /// 随机试卷大题ID
        /// </summary>
        public Guid policyNodeUid { get; set; }

        /// <summary>
        /// 试题类型ID
        /// </summary>
        public Guid questionTypeUid { get; set; }

        /// <summary>
        /// 试题类型
        /// </summary>
        public string questionTypeName { get; set; }

        /// <summary>
        /// 抽题试题分类ID
        /// </summary>
        public Guid folderUid { get; set; }

        /// <summary>
        /// 每题分数
        /// </summary>
        public decimal questionScore { get; set; }

        /// <summary>
        /// 随机试卷策略项组
        /// </summary>
        public List<ExamPolicyItemOutputDto> items { get; set; }
    }
}