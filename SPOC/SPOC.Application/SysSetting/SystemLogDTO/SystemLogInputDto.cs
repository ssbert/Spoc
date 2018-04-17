using SPOC.Common.Dto;

namespace SPOC.SysSetting.SystemLogDTO
{
    public class SystemLogInputDto : EasyuiDto
    {
        public string level { get; set; }

        public string operateName { get; set; }

        public string startTime { get; set; }

        public string endTime { get; set; }

        public string module { get; set; }

        public string message { get; set; }

        public string userName { get; set; }
    }
}
