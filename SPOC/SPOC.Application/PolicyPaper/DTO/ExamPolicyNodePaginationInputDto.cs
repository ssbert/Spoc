using SPOC.Common.Pagination;
using System;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 随机试卷大题分页查询
    /// </summary>
    public class ExamPolicyNodePaginationInputDto:PaginationInputDto 
    {
        /// <summary>
        /// 随机试卷id
        /// </summary>
        public Guid policyUid { get; set; }
    }
}