using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Core
{
    [Table("announcement_file")]
    public class AnnouncementFile : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 公告ID
        /// </summary>
        [Column("announcementId")]
        public Guid AnnouncementId { get; set; }
        /// <summary>
        /// 上传文件ID
        /// </summary>
        [Column("uploadFileId")]
        public Guid UploadFileId { get; set; }
    }
}