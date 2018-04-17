using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPOC.Common.Pagination;

namespace SPOC.Lib.Dto
{
    /// <summary>
    /// 知识点分页查询对象
    /// </summary>
    public class LabelPaginationInputDto : PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LabelPaginationInputDto()
        {
            folderId = new List<Guid>();
            exceptList = new List<Guid>();
        }

        /// <summary>
        /// 标签标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string userFullName { get; set; }
        /// <summary>
        /// 创建者用户名
        /// </summary>
        public string userLoginName { get; set; }
        /// <summary>
        /// 分类Id 多选 逗号隔开 
        /// 构造函数
        /// </summary>
        public List<Guid> folderId { get; set; }
        /// <summary>
        /// 排除的标签id列表
        /// </summary>
        public List<Guid> exceptList { get; set; }
    }
}
