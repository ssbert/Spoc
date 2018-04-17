using System;

namespace SPOC.Lib.Dto
{
    /// <summary>
    /// 用户作答记录分页查询数据项
    /// </summary>
    public class UserAnswerRecordsPaginationItem
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 试题Id
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 作答详情Id
        /// </summary>
        public Guid RecordId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 试题题干
        /// </summary>
        public string QuestionText { get; set; }
        /// <summary>
        /// 试题类型
        /// </summary>
        public string QuestionBaseTypeCode { get; set; }
        /// <summary>
        /// 试题来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 得扣分
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}