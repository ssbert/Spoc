using System;
using System.ComponentModel.DataAnnotations;

namespace SPOC.Category.Dto
{
    public class NvFolderTypeDto
    {
        public Guid Id { get; set; }
        [Required,StringLength(64)]
        public string folderTypeName { get; set; }
        [Required, StringLength(16)]
        public string folderTypeCode { get; set; }
        [StringLength(36)]
        public string folderCode { get; set; }
        [Required]
        public bool isCustomCode { get; set; }
        [Required, StringLength(64)]
        public string folderName { get; set; }
        public string remarks { get; set; }
        public int listOrder { get; set; }
    }
}