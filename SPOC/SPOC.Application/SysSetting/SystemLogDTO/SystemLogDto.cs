using System.Collections.Generic;

namespace SPOC.SysSetting.SystemLogDTO
{
    public class SystemLogDto
    {
        public string id { get; set; }

        public string operateName { get; set; }

        public string userName { get; set; }

        public string message { get; set; }

        public string level { get; set; }

        public string timeIp { get; set; }

    }

    public static class SystemLogConvertToDTO
    {
        public static SystemLogDto ToDTO(this SystemSet.SystemLog obj)
        {
            SystemLogDto dto = new SystemLogDto();
            if (obj == null)
            {
                return dto;
            }

            dto.id = obj.Id.ToString();
            dto.level = obj.level;
            dto.message = obj.message;
            dto.operateName = obj.operateName;
            dto.timeIp = obj.createTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + obj.ip;
            dto.userName = obj.creator;
            return dto;
        }

        public static List<SystemLogDto> ToDTOList(this List<SystemSet.SystemLog> objList)
        {
            List<SystemLogDto> dtoList = new List<SystemLogDto>();
            if (objList == null)
            {
                return dtoList;
            }
            foreach (SystemSet.SystemLog item in objList)
            {
                dtoList.Add(item.ToDTO());
            }
            return dtoList;
        }
    }
}
