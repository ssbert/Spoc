using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 练习报表分页查询
    /// </summary>
    public class ExerciseStatementPaginationInputDto:PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseStatementPaginationInputDto()
        {
            ClassIdList = new List<Guid>();
        }
        /// <summary>
        /// 班级ID列表
        /// </summary>
        public List<Guid> ClassIdList { get; set; }
        /// <summary>
        /// 练习标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 创建者用户名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}