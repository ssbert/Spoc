using System.ComponentModel.DataAnnotations;
using SPOC.Common.Dto;

namespace SPOC.SysSetting.MenuDTO
{
    public class MenuInputDto : EasyuiDto
    {
        public string id { get; set; }
        [Required]
        public string menuCode { get; set; }
        [Required]
        public string menuName { get; set; }
        [Required]
        public int isActive { get; set; }
        [Required]
        public int listOrder { get; set; }

        public string pid { get; set; }

        public string menuUrl { get; set; }

    }
}
