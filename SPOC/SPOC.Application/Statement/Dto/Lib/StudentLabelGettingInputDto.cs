using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto.Lib
{
    /// <summary>
    /// 学生知识点统计
    /// </summary>
    public class StudentLabelGettingInputDto:PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public StudentLabelGettingInputDto()
        {
            ClassIds = new List<Guid>();
        }

        /// <summary>
        /// 用户登陆名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 班级Id列表
        /// </summary>
        public List<Guid> ClassIds { get; set; }
    }
}