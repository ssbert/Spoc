using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Runtime.Validation;
using SPOC.Common.Pagination;

namespace SPOC.User
{
    /// <summary>
    /// 用户角色服务接口
    /// </summary>
    public interface IRoleService:IApplicationService
    {
        /// <summary>
        /// 用户分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        Task<PaginationOutputDto<SPOC.User.Dto.Role.UserListItem>> UserPagination(SPOC.User.Dto.Role.UserQueryConditionInputDto input);

        /// <summary>
        /// 改变角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task ChangeRole(SPOC.User.Dto.Role.UserRolesInputDto input);
    }
}