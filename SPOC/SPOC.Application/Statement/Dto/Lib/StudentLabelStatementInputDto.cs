using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto.Lib
{
    /// <summary>
    /// 学生标签掌握情况查询条件
    /// </summary>
    public class StudentLabelStatementInputDto:PaginationInputDto
    {
        /// <summary>
        /// 学生Id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 标签名
        /// </summary>
        public string LabelTitle { get; set; }
        /// <summary>
        /// 熟练度
        /// -1: 无反馈
        /// 0：全部
        /// 1：未掌握
        /// 2：不稳定
        /// 3：已掌握
        /// </summary>
        public int Proficiency { get; set; }
        /// <summary>
        /// 分类Id 多选 
        /// </summary>
        public List<Guid> FolderId { get; set; }
    }
}