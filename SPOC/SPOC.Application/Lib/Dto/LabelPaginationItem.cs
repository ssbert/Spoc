using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Newtonsoft.Json;
using SPOC.Common.Pagination;

namespace SPOC.Lib.Dto
{
    /// <summary>
    /// 知识点分页数据项
    /// </summary>
    [AutoMapFrom(typeof(Exam.ExamPaper))]
    public class LabelPaginationItem 
    {
        public Guid id { get; set; }
        /// <summary>
        /// 分类Id 
        /// </summary>
        public Guid folderId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string folderName{ get; set; }
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
        /// 说明
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 正则表达式
        /// </summary>
        public string regExpressions { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime createTime { get; set; }
    }


}
