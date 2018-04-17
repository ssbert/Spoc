using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.User;

namespace SPOC.Category
{
    /// <summary>
    /// 分类
    /// </summary>
    public class NvFolder : Entity<Guid>
    {
        public NvFolder()
        {
            parentUid = Guid.Empty;
            remarks = "";
            hasChild = "N";
        }
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid parentUid { get; set; }

        /// <summary>
        /// 包含父分类ID的全路径
        /// </summary>
        [Required]
        [StringLength(8000)]
        public string fullPath { get; set; }

        /// <summary>
        /// 分类编码
        /// </summary>
        [StringLength(36)]
        public string folderCode { get; set; }

        [Required, DefaultValue(false)]
        public bool isCustomCode { get; set; }

        /// <summary>
        /// 分类类型编码
        /// </summary>
        [StringLength(16)]
        [Required]
        public string folderTypeCode { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [StringLength(64)]
        [Required]
        public string folderName { get; set; }

        /// <summary>
        /// 所在的层数
        /// </summary>
        [Required]
        public int folderLevel { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [Required]
        public int listOrder { get; set; }

        /// <summary>
        /// 是否有子级
        /// </summary>
        [StringLength(1)]
        [DefaultValue("N")]
        [Required]
        public string hasChild { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [StringLength(2000)]
        public string remarks { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        [Required]
        public Guid creatorUid { get; set; }
        [ForeignKey("creatorUid")]
        public UserBase UserBase { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime createTime { get; set; }
    }
}