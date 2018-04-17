using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Category
{
    /// <summary>
    /// 分类类型
    /// </summary>
    public class NvFolderType : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 分类类型名称(自定义一个英文串)
        /// </summary>
        [StringLength(16)]
        [Required]
        public string folderTypeCode { get; set; }

        /// <summary>
        /// 分类类型名称(显示的名称)
        /// </summary>
        [StringLength(64)]
        [Required]
        public string folderTypeName { get; set; }

        /// <summary>
        /// 排序号(在管理页面中显示的顺序)
        /// </summary>
        [Required]
        public int listOrder { get; set; }

        /// <summary>
        /// 分类类型的备注(显示的备注)
        /// </summary>
        [StringLength(64)]
        public string remarks { get; set; }

    }
}