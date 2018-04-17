using System;
using System.Collections.Generic;

namespace SPOC.Statement.Dto
{
    /// <summary>
    /// 班级排行查询
    /// </summary>
    public class ClassRankingQueryInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ClassRankingQueryInputDto()
        {
            Sort = string.Empty;
            Order = string.Empty;
            ClassIdList = new List<Guid>();
        }
        /// <summary>
        /// 考试任务ID
        /// </summary>
        public Guid ExamTaskId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public List<Guid> ClassIdList { get; set; }

        #region 排序相关

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

        #endregion

    }
}