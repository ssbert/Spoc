using System;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;

namespace SPOC.Core.Dto
{
    public class AnnouncementInputDto : PaginationInputDto
    {
        public string id { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public string classId { get;set;}
        /// <summary>
        /// 是否启用
        /// </summary>
        public int enable { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 附件Id
        /// </summary>
        public Guid fileId { get; set; }
        /// <summary>
        /// 临时文件Id
        /// </summary>
        public Guid tempFileId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }
    }
}
