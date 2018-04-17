using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace SPOC.Category.Dto
{
    [AutoMapFrom(typeof(NvFolder))]
    public class NvFolderInputDto
    {
        public Guid Id { get; set; }
        public Guid parentUid { get; set; }
        [StringLength(36)]
        public string folderCode { get; set; }
        [Required]
        public bool isCustomCode { get; set; }
        [Required, StringLength(16)]
        public string folderTypeCode { get; set; }
        [Required, StringLength(64)]
        public string folderName { get; set; }
        public int folderLevel { get; set; }
        public int listOrder { get; set; }
        public string remarks { get; set; }
    }
}