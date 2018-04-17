using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 练习分页
    /// </summary>
    public class ExercisePaginationInputDto:PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExercisePaginationInputDto()
        {
            ClassIds = new List<Guid>();
        }
        /// <summary>
        /// 练习标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建者用户名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 创建时间（开始）
        /// </summary>
        public DateTime? CreateBeginTime { get; set; }
        /// <summary>
        /// 创建时间（结束）
        /// </summary>
        public DateTime? CreateEndTime { get; set; }
        /// <summary>
        /// 练习开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 练习结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public List<Guid> ClassIds { get; set; }
    }
}