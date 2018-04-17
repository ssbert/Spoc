using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using SPOC.Core;
using SPOC.SystemSet;

namespace SPOC.Faqs.Dtos
{
    [AutoMapFrom(typeof(Faq))]
    public class FaqListDto : EntityDto<Guid>
    {
        public   Guid Id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public Guid folderId { get; set; }
        public string folderName { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime updateTime { get; set; }
        public int seq { get; set; }
        public int userFul { get; set; }
        public int userLess { get; set; }
        public  bool IsActive { get; set; }
    }
}