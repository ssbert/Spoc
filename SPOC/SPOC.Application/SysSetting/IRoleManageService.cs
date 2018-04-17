using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.Pagination;
using SPOC.SysSetting.RoleManageDTO;

namespace SPOC.SysSetting
{
    public interface IRoleManageService : IApplicationService
    {

        EasyUiListResultDto<RoleManageDto> GetAllRoleManageUiList(RoleManageInputDto input);

        Task InsertRoleManage(RoleManageInputDto input);

        Task DeleteRoleManage(BatchRequestInput input);

        void ModifyRoleManage(RoleManageInputDto input);
        [HttpGet]
        RoleManageDto GetRoleManageById(string id);
        /// <summary>
        /// 获取授权树形结构
        /// </summary>
        /// <param name="roleId">权限ID</param>
        /// <returns></returns>
        List<RoleMenuModel> GetPermissionTree(string roleId);

        /// <summary>
        /// 角色选取用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<UserSelectOutputDto>> GetUserPagination(GetRoleUserPaginationCondition input);
        /// <summary>
        /// 获取已经选择的角色用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<UserSelectOutputDto>> GetRoleUserPagination(GetRoleUserPaginationCondition input);

        /// <summary>
        /// 新增角色用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task AddUserRole(UserRoleInputDto input);
        /// <summary>
        /// 删除角色用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteUserRole(UserRoleInputDto input);
    }
}
