using Abp.AutoMapper;
using System;
using AutoMapper;

namespace SPOC.Category.Dto
{
    [AutoMapFrom(typeof(NvFolder))]
    public class NvFolderItemOutputDto
    {
     
        public Guid Id { get; set; }
        public string folderName { get; set; }
        public string folderCode { get; set; }
        public bool isCustomCode { get; set; }
        public Guid parentUid { get; set; }
        public int listOrder { get; set; }
        public int folderLevel { get; set; }
        public string remarks { get; set; }

    }
}