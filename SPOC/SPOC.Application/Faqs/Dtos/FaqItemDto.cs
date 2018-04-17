using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using SPOC.SystemSet;

namespace SPOC.Faqs.Dtos
{
    [AutoMapFrom(typeof(Faq))]
    public class FaqItemDto : EntityDto<Guid>
    {
        public Guid Id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public Guid folderId { get; set; }
        public string folderName { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime updateTime { get; set; }
        public int  seq { get; set; }
        public bool IsActive { get; set; }

    }
  
}
