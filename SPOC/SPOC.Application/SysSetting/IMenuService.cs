using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.SysSetting.MenuDTO;
using SPOC.SysSetting.RoleManageDTO;

namespace SPOC.SysSetting
{
    public interface IMenuService:IApplicationService
    {
        EasyUiListResultDto<MenuDto> GetAllMenu(EasyuiDto input);

        Task InsertMenu(MenuInputDto input);

        void ModifyMenu(MenuInputDto input);

        Task DeleteMenu(BatchRequestInput input);

        MenuDto GetMenuById(string id);

        List<MenuList> GetMenuList();

        List<MenuSetDto> GetAllMenu();
        List<MenuDto> TreeMenu();

        #region  获取权限范围的菜单
        /// <summary>
        /// 获取授权菜单树
        /// </summary>
        /// <returns></returns>
        Task<List<RoleMenuModel>> GetPermissionTree();

        #endregion

    }
}
