using System;

namespace SPOC.Exam.GradeDto
{
    /// <summary>
    /// 用户考试记录列表查询
    /// </summary>
    public class UserExamRecordInputDto
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
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