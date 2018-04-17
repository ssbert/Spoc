using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto.Lib
{
    /// <summary>
    /// 班级知识点掌握情况
    /// </summary>
    public class ClassLabelGettingInputDto : PaginationInputDto
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public List<Guid> ClassIdList { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 标签Id
        /// </summary>
        public List<Guid> LabelId { get; set; }
        /// <summary>
        /// 分类Id 多选 
        /// </summary>
        public List<Guid> FolderId { get; set; }
    }
    /// <summary>
    /// 班级下用户某个标签掌握情况查询输入
    /// </summary>
    public class UserLabelGettingInputDto : PaginationInputDto
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public List<Guid> ClassIdList { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 标签ID
        /// </summary>
        public Guid LabelId{ get; set; }
        /// <summary>
        /// 掌握状态
        /// </summary>
        public int Status { get; set; }
    }
}
