using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto
{
    /// <summary>
    /// 补考排名报表分页查询项
    /// </summary>
    public class RetestRankPaginationInputDto:PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RetestRankPaginationInputDto()
        {
            ClassIdList = new List<Guid>();
        }
        /// <summary>
        /// 补考ID
        /// </summary>
        public Guid ExamId { get; set; }
        /// <summary>
        /// 班级ID列表
        /// </summary>
        public List<Guid> ClassIdList { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserFullName { get; set; }
    }
}