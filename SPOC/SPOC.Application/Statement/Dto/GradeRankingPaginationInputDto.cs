using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto
{
    /// <summary>
    /// 成绩排名分页查询
    /// </summary>
    public class GradeRankingPaginationInputDto:PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GradeRankingPaginationInputDto()
        {
            ClassIdList = new List<Guid>();
        }
        /// <summary>
        /// 考试任务ID
        /// </summary>
        public Guid ExamTaskId { get; set; }
        /// <summary>
        /// 班级ID列表
        /// </summary>
        public List<Guid> ClassIdList { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string UserLoginName { get; set; }
    }
}