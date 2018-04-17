using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto
{
    /// <summary>
    /// 考试报表分页查询
    /// </summary>
    public class ExamTaskStatementPaginationInputDto : PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamTaskStatementPaginationInputDto()
        {
            ClassIdList = new List<Guid>();
        }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 任务标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建者登录名
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
        /// 考试开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public List<Guid> ClassIdList { get; set; }
    }
}