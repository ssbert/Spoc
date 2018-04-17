using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using SPOC.Common.EasyUI;
using SPOC.SysSetting.SystemLogDTO;

namespace SPOC.SysSetting
{
    public class SystemLogService : ApplicationService,ISystemLogService
    {
        private readonly IRepository<SystemSet.SystemLog, Guid> _iSystemLogRepository;
        public SystemLogService(IRepository<SystemSet.SystemLog, Guid> iSystemLogRepository)
        {
            _iSystemLogRepository = iSystemLogRepository;
        }
        
        public EasyUiListResultDto<SystemLogDto> GetAllServiceSet(SystemLogInputDto input)
        {
            EasyUiListResultDto<SystemLogDto> result = new EasyUiListResultDto<SystemLogDto>();
            try
            {
                var data = _iSystemLogRepository.GetAll();
                if (data == null)
                {
                    return result;
                }
                List<SystemSet.SystemLog> dataList = data.ToList();

                if (string.IsNullOrWhiteSpace(input.level) && string.IsNullOrWhiteSpace(input.startTime) && string.IsNullOrWhiteSpace(input.endTime) && string.IsNullOrWhiteSpace(input.module) && string.IsNullOrWhiteSpace(input.userName))
                {
                    result.total = dataList.Count;
                    result.rows =  dataList.Skip(input.Skip).Take(input.PageSize).ToList().ToDTOList();
                }

                if (!string.IsNullOrWhiteSpace(input.level))
                {
                    string levelName = GetLevelName(input.level);
                    if (!string.IsNullOrWhiteSpace(levelName))
                    {
                        dataList = dataList.Where(d => d.level.Contains(levelName)).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(input.startTime))
                {
                    dataList = dataList.Where(d=>d.createTime>DateTime.Parse(input.startTime)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(input.endTime))
                {
                    dataList = dataList.Where(d => d.createTime < DateTime.Parse(input.endTime)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(input.module))
                {
                    dataList = dataList.Where(d => d.module.Contains(input.module)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(input.operateName))
                {
                    dataList = dataList.Where(d => d.operateName.Contains(input.operateName)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(input.userName))
                {
                    dataList = dataList.Where(d => d.creator.Contains(input.userName)).ToList();
                }

                result.rows = dataList.Skip(input.Skip).Take(input.PageSize).ToList().ToDTOList();
                result.total = dataList.Count;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

            return result;                
        }

        private string GetLevelName(string level)
        {
            switch (level.Trim())
            {
                case "0":
                    return "";
                case "1":
                    return "提示";
                case "2":
                    return "警告";
                case "3":
                    return "错误";                      
                default:
                    return "";
            } 
        }
    }
}
