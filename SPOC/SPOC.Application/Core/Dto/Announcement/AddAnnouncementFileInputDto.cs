using System;

namespace SPOC.Core.Dto.Announcement
{
    /// <summary>
    /// 添加公告附件
    /// </summary>
    public class AddAnnouncementFileInputDto
    {
        /// <summary>
        /// 公告ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 临时文件ID
        /// </summary>
        public string TempFileId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
    }
}