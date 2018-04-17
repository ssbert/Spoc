using System;
using SPOC.Common.Pagination;

namespace SPOC.Lib.Dto
{
    /// <summary>
    /// 用户作答记录分页查询
    /// </summary>
    public class UserAnswerRecordsPaginationInput:PaginationInputDto
    {
        /// <summary>
        /// 标签ID
        /// </summary>
        public Guid LabelId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 题干
        /// </summary>
        public string QuestionText { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 试题类型
        /// </summary>
        public string QuestionBaseTypeCode { get; set; }
        /// <summary>
        /// 试题答题状态
        /// 0：所有
        /// 1：只错误（不通过）
        /// 2：只正确（通过）
        /// </summary>
        public int Status { get; set; }
    }
}