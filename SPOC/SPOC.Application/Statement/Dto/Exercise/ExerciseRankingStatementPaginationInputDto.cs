using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 练习效率/积极性排行榜分页查询
    /// </summary>
    public class ExerciseRankingStatementPaginationInputDto:PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseRankingStatementPaginationInputDto()
        {
            ClassIdList = new List<Guid>();
        }
        /// <summary>
        /// 练习ID
        /// </summary>
        public Guid ExerciseId { get; set; }
        /// <summary>
        /// 班级ID列表
        /// </summary>
        public List<Guid> ClassIdList { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 学生登录名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 通过情况，0：全部，1：通过，2：未通过
        /// </summary>
        public int PassState { get; set; }
    }
}