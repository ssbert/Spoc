using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Core
{
    /// <summary>
    /// 上传的文件
    /// </summary>
    [Table("upload_file")]
    public class UploadFile : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        [Column("size")]
        public long Size { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [Column("fileName"), StringLength(512)]
        public string FileName { get; set; }

        /// <summary>
        /// 上传来源
        /// announcement：公告
        /// </summary>
        [Column("source"), StringLength(64)]
        public string Source { get; set; }
        
    }
}