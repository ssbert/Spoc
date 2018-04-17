using System;
using SPOC.Common.Dto;

namespace SPOC.SysSetting.SiteDto
{
    public class SiteInputDto : EasyuiDto
    {
        public string version { get; set; }
        public DateTime createTime { get; set; }
    }
}
