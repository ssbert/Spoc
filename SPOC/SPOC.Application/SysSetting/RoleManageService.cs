using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.EasyUI;
using SPOC.Common.Extensions;
using SPOC.Common.Pagination;
using SPOC.SysSetting.RoleManageDTO;
using SPOC.SystemSet;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SPOC.SysSetting
{
    public class RoleManageService:ApplicationService,IRoleManageService
    {
        private readonly IRepository<RoleManage, Guid> _iRoleManageRepository;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<RolePermission,Guid> _iRolePermissionRepository;
        private readonly IRepository<Menu, Guid> _iMenuRepository;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<AdminInfo, Guid> _iAdminInfoRep;
        private readonly IRepository<UserRole, Guid> _iUserRoleRep;
        /// <summary>
        /// 构造函数 注入
        /// </summary>
        /// <param name="iRoleManageRepository"></param>
        /// <param name="iRolePermissionRepository"></param>
        /// <param name="iMenuRepository"></param>
        /// <param name="iUserBaseRep"></param>
        /// <param name="iTeacherInfoRep"></param>
        /// <param name="iAdminInfoRep"></param>
        public RoleManageService(IRepository<RoleManage, Guid> iRoleManageRepository, IRepository<RolePermission, Guid> iRolePermissionRepository, IRepository<Menu, Guid> iMenuRepository, IRepository<UserBase, Guid> iUserBaseRep , IRepository<TeacherInfo, Guid> iTeacherInfoRep, IRepository<AdminInfo, Guid> iAdminInfoRep, IRepository<UserRole, Guid> iUserRoleRep)
        {
            _iRoleManageRepository = iRoleManageRepository;
            _iRolePermissionRepository = iRolePermissionRepository;
            _iMenuRepository = iMenuRepository;
            _iUserBaseRep = iUserBaseRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iAdminInfoRep = iAdminInfoRep;
            _iUserRoleRep = iUserRoleRep;
        }

        public EasyUiListResultDto<RoleManageDto> GetAllRoleManageUiList(RoleManageInputDto input)
        {
            var result = new EasyUiListResultDto<RoleManageDto>();
            try
            {
               
                var roleManageList = _iRoleManageRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.roleName),a=>a.roleName.Contains(input.roleName))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.roleGroup), a => a.roleGroup.Equals(input.roleGroup));
                var dtoList = new List<RoleManageDto>();
                var permissionData = _iRolePermissionRepository.GetAll().AsNoTracking().Join(_iMenuRepository.GetAll().AsNoTracking(), r => r.menuId, m => m.Id, (r, m) => new { r, m }).Select(
                    a=>new
                    {
                        a.r.roleId,
                        a.m.menuName,
                        a.m.menuCode,
                        a.m.Id
                    }).ToList();
                foreach (var roleManage in roleManageList)
                {
                   
                    var dto = roleManage.MapTo<RoleManageDto>(); 
                    //授权菜单信息
                    var permission = permissionData.Where(d => d.roleId == roleManage.Id).Select(a => a.menuName).ToList();
                    dto.menuNames = permission.Any()? permission.Aggregate((right, left) => right + "," + left):"" ;
                    permission =permissionData.Where(d => d.roleId == roleManage.Id).Select(a => a.Id.ToString()).ToList();
                    dto.menusCodes = permission.Any() ? permission.Aggregate((right, left) => right + "," + left) : "";   
                    dtoList.Add(dto);
                }
                result.total = dtoList.Count;
                result.rows = dtoList.Skip(input.Skip).Take(input.PageSize).ToList();
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return result;
        }

      

        public async Task InsertRoleManage(RoleManageInputDto input)
        {
            try
            {
                RoleManage rm = new RoleManage();
                rm.Id = Guid.NewGuid();
                rm.description = input.description;
                rm.rolecode = input.roleCode;
                rm.roleName = input.roleName;
                rm.roleGroup = input.roleGroup;
                await _iRoleManageRepository.InsertAsync(rm);
                var menuList = input.permissionId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in menuList)
                {
                    var rolePermission = new RolePermission();
                    rolePermission.Id = Guid.NewGuid();
                    rolePermission.menuId = new Guid(item);
                    rolePermission.roleId = rm.Id;
                    _iRolePermissionRepository.Insert(rolePermission);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new UserFriendlyException("角色信息添加失败");
            }
        }

        public async Task DeleteRoleManage(BatchRequestInput input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.Id))
                {
                    return ;
                }
                var ids=input.Id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    Guid gid ;
                    Guid.TryParse(ids[i], out gid);
                    await _iRoleManageRepository.DeleteAsync(gid);
                    await _iRolePermissionRepository.DeleteAsync(a => a.roleId.Equals(gid));
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public void ModifyRoleManage(RoleManageInputDto input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.id))
                {
                    return;
                }

                Guid gid;
                Guid.TryParse(input.id,out gid);
                var rm = _iRoleManageRepository.FirstOrDefault(d=>d.Id == gid);
                if (rm == null)
                {
                    return;
                }
                rm.description = input.description;
                rm.roleGroup = input.roleGroup;
                if (!string.IsNullOrWhiteSpace(input.roleCode))
                {
                    rm.rolecode = input.roleCode;
                }
                if (!string.IsNullOrWhiteSpace(input.roleName))
                {
                    rm.roleName = input.roleName;
                }
                var menuList = input.permissionId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                _iRolePermissionRepository.Delete(a => a.roleId == rm.Id);
                foreach (var item in menuList)
                {
                    var rolePermission = new RolePermission();
                    rolePermission.Id = Guid.NewGuid();
                    rolePermission.menuId = new Guid(item);
                    rolePermission.roleId = rm.Id;
                    _iRolePermissionRepository.Insert(rolePermission);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new UserFriendlyException("角色信息更新失败");
            }
        }

        public RoleManageDto GetRoleManageById(string id)
        {
            var dto = new RoleManageDto();
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return dto;
                }

                Guid gid;
                bool isGuid = Guid.TryParse(id,out gid);
                if (isGuid)
                {
                    dto = _iRoleManageRepository.FirstOrDefault(d => d.Id == gid).ExMapTo<RoleManageDto>(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return dto;
        }

        public List<RoleMenuModel> GetPermissionTree(string roleId)
        {
        
            List<Menu> menulist ;
            if (!string.IsNullOrWhiteSpace(roleId)) //根据角色ID查询过滤
            {
                menulist =
                    _iMenuRepository.GetAll().AsNoTracking().Where(a => a.isActive == 1)
                        .Join(_iRolePermissionRepository.GetAll().AsNoTracking().Where(a => a.roleId.Equals(new Guid(roleId))),
                            m => m.Id, p => p.menuId,
                            (m, p) => m).ToList();
            }
            else
            {
                menulist = _iMenuRepository.GetAll().AsNoTracking().Where(a => a.isActive == 1).ToList();
            }


            var parentMenu = menulist.Where(a => a.parentMenuCode == "").OrderBy(a => a.listOrder).Select(a => new RoleMenuModel() { id = a.Id.ToString(), code = a.menuCode, text = a.menuName, url = a.menuUrl}).ToList();
            foreach (var item in parentMenu)
            {
                item.children = GetChildMenu(item.code, menulist);
            }
            var  permissionTree = parentMenu;
            //if (string.IsNullOrWhiteSpace(roleId)) //预览菜单的时候不需要加载根目录
            //{
            //    permissionTree = new List<RoleMenuModel>()
            //    {
            //        new RoleMenuModel {id = "", code = "", text = "菜单权限", children = parentMenu}
            //    };
            //}
            return permissionTree;
        }

      
        /// <summary>
        /// 多级菜单获取子级
        /// </summary>
        /// <param name="parentMenuCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<RoleMenuModel> GetChildMenu(string parentMenuCode, List<Menu> data)
        {
            var parentMenu = data.Where(a => a.parentMenuCode == parentMenuCode).OrderBy(a => a.listOrder).Select(a => new RoleMenuModel() { id = a.Id.ToString(), code = a.menuCode, text = a.menuName, url = a.menuUrl }).ToList();
            foreach (var item in parentMenu)
            {
                item.children = GetChildMenu(item.code, data); ;

            }
            return parentMenu;
        }
        #region 角色用户信息
        public async Task<PaginationOutputDto<UserSelectOutputDto>> GetUserPagination(GetRoleUserPaginationCondition input)
        {
            #region 验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("没有登录");
            }
            if (!cookie.IsAdmin)
            {
                throw new UserFriendlyException("没有权限");
            }
            #endregion

            IQueryable<Guid> userFilters = _iTeacherInfoRep.GetAll().AsNoTracking().Select(a => a.userId);
            //判断角色
            if (input.identity == 2) //教师
            {
                userFilters = _iTeacherInfoRep.GetAll().AsNoTracking().Select(a => a.userId);
            }
            else if (input.identity == 3) //管理员
            {
                userFilters = _iAdminInfoRep.GetAll().AsNoTracking().Select(a => a.userId);
            }
          
            var query = _iUserBaseRep.GetAll().AsNoTracking()
                .WhereIf(!string.IsNullOrEmpty(input.userLoginName), t => (!string.IsNullOrWhiteSpace(t.userLoginName) && t.userLoginName.Contains(input.userLoginName)))
                .WhereIf(!string.IsNullOrEmpty(input.userFullName), t => (!string.IsNullOrWhiteSpace(t.userFullName) && t.userFullName.Contains(input.userFullName)))
                .WhereIf(!string.IsNullOrEmpty(input.userMobile), t => t.userMobile.Contains(input.userMobile))
                .WhereIf(!string.IsNullOrEmpty(input.userEmail), t => (!string.IsNullOrWhiteSpace(t.userEmail) && t.userEmail.Contains(input.userEmail)))
                .WhereIf(!string.IsNullOrEmpty(input.userGender), t => t.userGender == input.userGender)
                .Join(userFilters, user => user.Id, filter => filter, (user, filter) => new { user  })
                //过滤已经添加的用户
                .GroupJoin(_iUserRoleRep.GetAll().AsNoTracking().Where(a => a.roleId.Equals(input.roleId)), user => user.user.Id, filter => filter.userId, (user, filter) => new { user, hasAgent = filter.Any() })
                .Where(a => !a.hasAgent)
                .Select(a => new UserSelectOutputDto
                {
                    identity = input.identity,
                    userFullName = a.user.user.userFullName,
                    userLoginName = a.user.user.userLoginName,
                    userId = a.user.user.Id,
                    userMobile = a.user.user.userMobile,
                    userEmail = a.user.user.userEmail,
                    userGender = a.user.user.userGender
                });
            var pagination = new PaginationOutputDto<UserSelectOutputDto>
            {
                total = query.Count(),
                rows = query.Skip(input.skip).Take(input.pageSize).ToList()
            };
            return await Task.FromResult(pagination);
        }
        public async Task<PaginationOutputDto<UserSelectOutputDto>> GetRoleUserPagination(GetRoleUserPaginationCondition input)
        {
            #region 验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("没有登录");
            }
            if (!cookie.IsAdmin)
            {
                throw new UserFriendlyException("没有权限");
            }
            #endregion
            var query = _iUserBaseRep.GetAll().AsNoTracking()
                 .WhereIf(!string.IsNullOrEmpty(input.userLoginName), t => (!string.IsNullOrWhiteSpace(t.userLoginName) && t.userLoginName.Contains(input.userLoginName)))
                .WhereIf(!string.IsNullOrEmpty(input.userFullName), t => (!string.IsNullOrWhiteSpace(t.userFullName) && t.userFullName.Contains(input.userFullName)))
                .WhereIf(!string.IsNullOrEmpty(input.userMobile), t => t.userMobile.Contains(input.userMobile))
                .WhereIf(!string.IsNullOrEmpty(input.userEmail), t => (!string.IsNullOrWhiteSpace(t.userEmail) && t.userEmail.Contains(input.userEmail)))
                .WhereIf(!string.IsNullOrEmpty(input.userGender), t => t.userGender == input.userGender)
                .Join(_iUserRoleRep.GetAll().AsNoTracking().Where(a => a.roleId.Equals(input.roleId)), user => user.Id, role => role.userId, (user, role) => new { user })
                .Select(a => new UserSelectOutputDto
                {
                    identity = input.identity,
                    userFullName = a.user.userFullName,
                    userLoginName = a.user.userLoginName,
                    userId = a.user.Id,
                    userMobile = a.user.userMobile,
                    userEmail = a.user.userEmail,
                    userGender = a.user.userGender
                });
            var pagination = new PaginationOutputDto<UserSelectOutputDto>
            {
                total = query.Count(),
                rows = query.Skip(input.skip).Take(input.pageSize).ToList()
            };
            return await Task.FromResult(pagination);
        }

        public async Task AddUserRole(UserRoleInputDto input)
        {
            var roleUser = _iUserRoleRep.GetAll().Where(a => a.roleId.Equals(input.roleId)).Select(a=>a.userId).ToList();
            input.userIdList.ForEach(async userId =>
            {
                if (!roleUser.Contains(userId))
                {
                    await _iUserRoleRep.InsertAsync(new UserRole { roleId = input.roleId, userId = userId });
                }
            });
        }
        public async Task DeleteUserRole(UserRoleInputDto input)
        {
            input.userIdList.ForEach(async userId =>
            {
                    await _iUserRoleRep.DeleteAsync(a=>a.roleId.Equals(input.roleId) && a.userId.Equals(userId));
            });
        }

        #endregion
    }
}
