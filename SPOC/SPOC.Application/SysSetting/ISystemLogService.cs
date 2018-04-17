using Abp.Application.Services;
using SPOC.Common.EasyUI;
using SPOC.SysSetting.SystemLogDTO;

namespace SPOC.SysSetting
{
    public interface ISystemLogService : IApplicationService
    {
        EasyUiListResultDto<SystemLogDto> GetAllServiceSet(SystemLogInputDto input);
    }
}
