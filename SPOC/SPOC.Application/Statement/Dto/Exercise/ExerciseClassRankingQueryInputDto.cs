using SPOC.Common.Pagination;
using System;
using System.Collections.Generic;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 练习报表班级排行分页
    /// </summary>
    public class ExerciseClassRankingQueryInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseClassRankingQueryInputDto()
        {
            Sort = string.Empty;
            Order = string.Empty;
            ClassIdList = new List<Guid>();
        }

        /// <summary>
        /// 班级ID
        /// </summary>
        public List<Guid> ClassIdList { get; set; }

        /// <summary>
        /// 练习ID
        /// </summary>
        public Guid ExerciseId { get; set; }

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