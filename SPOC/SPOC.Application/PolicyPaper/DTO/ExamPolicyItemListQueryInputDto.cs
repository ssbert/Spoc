using System;
using System.ComponentModel.DataAnnotations;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 策略项列表查询
    /// </summary>
    public class ExamPolicyItemListQueryInputDto
    {
        /// <summary>
        /// 试卷Id
        /// </summary>
        [Required]
        public Guid PolicyId { get; set; }
        /// <summary>
        /// 大题ID
        /// </summary>
        public Guid NodeId { get; set; }
        /// <summary>
        /// 试题类型
        /// </summary>
        public string QuestionBaseTypeCode { get; set; }
        /// <summary>
        /// 排序字段名称
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// 排序方式 asc | desc
        /// </summary>
        public string Order { get; set; }
        /// <summary>
        /// 排序表达式
        /// </summary>
        public string OrderExpression => Sort + " " + Order;
    }
}
