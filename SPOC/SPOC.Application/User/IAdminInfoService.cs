using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using SPOC.Common.EasyUI;
using SPOC.SysSetting.RoleManageDTO;
using SPOC.User.Dto.Admin;
using SPOC.User.Dto.Common;

namespace SPOC.User
{

    public interface IAdminInfoService : IApplicationService
    {

        EasyUiListResultDto<AdminInfoDto> GetAdminInfoByGuid(AdminInfoInputDto input);
        void UpdateAdminInfo(CreateAdminInfoInputDto input);
        [DisableValidation]
        Task CreateAdminInfo(CreateAdminInfoInputDto input);
        Task DeleteAdminInfo(BatchRequestInput input);
        AdminInfoDto GetAdminInfoDtoByQueryInput(UserInfoQueryInputDto model);


        Task DeleteAdminInfos(List<BatchDeleteRequestInputByUser> inputList);

        void UpdateAdminInfoByUser(UpdateAdminInfoInputDto input);

        Task ActiveAdminInfo(List<BatchDeleteRequestInputByUser> inputList);
        List<JsonTree> GetJsonTree(List<RoleManageDto> perList);

        bool CheckNameExit(string name, string type, string oldname = "");
        [NonAction]
        Task AddDefaultAdmin();
         Task RecoverInsertAdmin(CreateAdminInfoInputDto input);
    }
}
