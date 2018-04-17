using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.Extensions;
using SPOC.SysSetting.MenuDTO;
using SPOC.SysSetting.RoleManageDTO;
using SPOC.SystemSet;
using SPOC.User;
using SPOC.User.Dto.UserInfo;

namespace SPOC.SysSetting
{
    public class MenuService : ApplicationService, IMenuService
    {
        private readonly IRepository<Menu, Guid> _iMenuRepository;
        private readonly IRepository<RolePermission, Guid> _iRolePermissionRepository;
        private readonly IRepository<UserRole, Guid> _iUserRoleRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public MenuService(IRepository<Menu, Guid> iMenuRepository, IUnitOfWorkManager unitOfWorkManager, IRepository<RolePermission, Guid> iRolePermissionRepository, IRepository<UserRole, Guid> iUserRoleRepository)
        {
            _iMenuRepository = iMenuRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _iRolePermissionRepository = iRolePermissionRepository;
            _iUserRoleRepository = iUserRoleRepository;
        }
        public EasyUiListResultDto<MenuDto> GetAllMenu(EasyuiDto input)
        {
            EasyUiListResultDto<MenuDto> result = new EasyUiListResultDto<MenuDto>();
            try
            {
        
                List<MenuDto> dtoList = new List<MenuDto>();
                var data = _iMenuRepository.GetAll();
                if (data == null)
                {
                    return result;
                }

                var dataList = data.ToList();
                foreach (var item in dataList)
                {
                    MenuDto dto = new MenuDto();
                    dto.id = item.Id.ToString();
                    dto.menuName = item.menuName;
                    dto.menuCode = item.menuCode;
                    dto.isActive = item.isActive == 0 ? "未启用" : "启用";
                    dto.listOrder = item.listOrder;
                    dto.menuUrl = item.menuUrl;
                    Menu menu = dataList.FirstOrDefault(d => d.menuCode == item.parentMenuCode);
                    if (menu == null || string.IsNullOrWhiteSpace(menu.menuName))
                    {
                        dto.pidName = string.Empty;
                    }
                    else
                    {
                        dto.pidName = menu.menuName;
                    }
                    dtoList.Add(dto);
                }
                result.total = dtoList.Count;
                result.rows = dtoList.OrderBy(input.OrderExpression).Skip(input.Skip).Take(input.PageSize).ToList();
                return result;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return result;
        }

        public async Task InsertMenu(MenuInputDto input)
        {
            if (_iMenuRepository.GetAll().Any(a => a.menuCode == input.menuCode))
            {
                throw new UserFriendlyException("已有相同的菜单编码");
            }
            Menu menu = new Menu();
            menu.Id = Guid.NewGuid();
            menu.isActive = input.isActive;
            menu.listOrder = input.listOrder;
            menu.menuIcon = "icon-page";
            if (!string.IsNullOrWhiteSpace(input.pid))
            {
                var data = _iMenuRepository.FirstOrDefault(d => d.Id.ToString() == input.pid);
                menu.parentMenuCode = data.menuCode;
                menu.parentMenuName = data.menuName;
            }
            menu.menuName = input.menuName;
            menu.menuUrl = string.IsNullOrWhiteSpace(input.menuUrl) ? string.Empty : input.menuUrl;
            menu.menuCode = input.menuCode;
            await _iMenuRepository.InsertAsync(menu);
        }

        public void ModifyMenu(MenuInputDto input)
        {
            Guid gid = Guid.Empty;
            bool isGuid = Guid.TryParse(input.id, out gid);
            if (!isGuid)
            {
                throw new UserFriendlyException("无效的Id");
            }

            if (_iMenuRepository.GetAll().Any(a => a.menuCode == input.menuCode && a.Id != gid))
            {
                throw new UserFriendlyException("已有相同的菜单编码");
            }
            var data = _iMenuRepository.FirstOrDefault(d => d.Id == gid);
            data.menuName = input.menuName;

            if (data.menuCode != input.menuCode)
            {
                var list = _iMenuRepository.GetAll().Where(a => a.parentMenuCode == data.menuCode).ToList();
                list.ForEach(m =>
                {
                    m.parentMenuCode = input.menuCode;
                    m.parentMenuName = input.menuName;
                    _iMenuRepository.UpdateAsync(m);
                });
                data.menuCode = input.menuCode;
            }
            if (!string.IsNullOrWhiteSpace(input.pid))
            {
                var menu = _iMenuRepository.FirstOrDefault(d => d.Id.ToString() == input.pid);
                data.parentMenuName = menu.menuName;
                data.parentMenuCode = menu.menuCode;
            }

            data.menuUrl = string.IsNullOrWhiteSpace(input.menuUrl) ? string.Empty: input.menuUrl;

            data.isActive = input.isActive;
            data.listOrder = input.listOrder;
        }

        public async Task DeleteMenu(BatchRequestInput input)
        {
            try
            {
                string[] ids = input.Id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ids.Length; i++)
                {
                    Guid gid = Guid.Empty;
                    bool isGuid = Guid.TryParse(ids[i], out gid);
                    if (!isGuid)
                    {
                        continue;
                    }
                    await _iMenuRepository.DeleteAsync(gid);
                }
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

  

      
        private void GetMenuTreeList(List<MenuDto> listData, ref MenuDto dto)
        {
            try
            {
                List<MenuDto> newlist = new List<MenuDto>();
                foreach (MenuDto item in listData)
                {
                    if (item.parentMenuCode == dto.menuCode)
                    {
                        item.pid = dto.id;
                        newlist.Add(item);
                        dto.children = newlist;
                    }
                }
                foreach (MenuDto item in newlist)
                {
                    MenuDto dt = item;
                    GetMenuTreeList(listData, ref dt);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        } 

        public MenuDto GetMenuById(string Id)
        {
            MenuDto dto = new MenuDto();
            try
            {
                Guid gid = Guid.Empty;
                bool isGuid = Guid.TryParse(Id, out gid);
                if (!isGuid)
                {
                    return dto;
                }
                Menu menu = _iMenuRepository.FirstOrDefault(d => d.Id == gid);
                if (menu == null || string.IsNullOrWhiteSpace(menu.Id.ToString()))
                {
                    return dto;
                }
                dto.id = menu.Id.ToString();
                dto.menuName = menu.menuName;
                dto.isActive = menu.isActive.ToString();
                dto.listOrder = menu.listOrder;
                dto.menuCode = menu.menuCode;
                dto.menuUrl = menu.menuUrl;
                dto.parentMenuName = string.IsNullOrWhiteSpace(menu.parentMenuName) == true ? "" : menu.parentMenuName;
                Menu data = _iMenuRepository.FirstOrDefault(d => d.menuCode == menu.parentMenuCode && d.menuName == menu.parentMenuName);
                if (data == null)
                {
                    dto.pid = "";
                }
                else
                {
                    dto.pid = data.Id.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return dto;
        }

        public List<MenuList> GetMenuList()
        {
            List<MenuList> modelList = new List<MenuList>();
            try
            {
                List<Menu> rootMenuList = _iMenuRepository.GetAllList(d => (d.parentMenuCode == string.Empty || d.parentMenuCode == null) && d.isActive > 0).OrderBy(d => d.listOrder).ToList();
                List<Menu> allChildList = _iMenuRepository.GetAllList(d => d.isActive > 0).OrderBy(d => d.listOrder).ToList();

                foreach (var item in rootMenuList)
                {
                    MenuList model = new MenuList();
                    model.MenuName = item.menuName;
                    model.Url = item.menuUrl;
                    List<Menu> childList = allChildList.Where(d => d.parentMenuCode == item.Id.ToString()).ToList();
                    List<MenuList> list = new List<MenuList>();
                    foreach (var child in childList)
                    {
                        MenuList mvm = new MenuList();
                        mvm.MenuName = child.menuName;
                        mvm.Url = child.menuUrl;
                        list.Add(mvm);
                    }
                    model.Children = list;

                    modelList.Add(model);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return modelList;
        }

        public List<MenuDto> TreeMenu()
        {
            List<MenuDto> dtoList = new List<MenuDto>();
            try
            {
                var data = _iMenuRepository.GetAllList();
                if (data == null)
                {
                    return dtoList;
                }

                List<MenuDto> allDataList = new List<MenuDto>();
                foreach (var item in data)
                {
                    MenuDto dto = new MenuDto();
                    dto.menuCode = item.menuCode;
                    dto.id = item.Id.ToString();
                    dto.parentMenuCode = item.parentMenuCode;
                    dto.text = item.menuName;
                    dto.menuName = item.menuName;
                    allDataList.Add(dto);
                }

                List<Menu> rootData = _iMenuRepository.GetAllList(d => (d.parentMenuCode == string.Empty || d.parentMenuCode == null) && d.isActive > 0);
                if (rootData == null)
                {
                    return dtoList;
                }

                foreach (Menu item in rootData)
                {
                    MenuDto dto = new MenuDto();
                    dto.menuCode = item.menuCode;
                    dto.id = item.Id.ToString();
                    dto.menuName = item.menuName;
                    dto.text = item.menuName;
                    dto.parentMenuCode = item.parentMenuCode;
                    GetMenuTreeList(allDataList, ref dto);
                    dtoList.Add(dto);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return dtoList;
        }

        /// <summary>
        /// 获取授权菜单树
        /// </summary>
        /// <returns></returns>
        public Task<List<RoleMenuModel>> GetPermissionTree()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null)
            {
                throw new UserFriendlyException("没有登录");
            }
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("没有登录");
            }
            List<Menu> menulist;
            if (cookie.IsSuperAdmin)  //超级管理员查所有菜单
            {
                menulist = _iMenuRepository.GetAll().AsNoTracking().Where(a => a.isActive == 1).ToList();
            }
            //查询权限范围内的菜单
            else
            {
                menulist = _iMenuRepository.GetAll().AsNoTracking().Where(a => a.isActive == 1)
                    .Join(_iRolePermissionRepository.GetAll().AsNoTracking(), m => m.Id, p => p.menuId, (m, p) => new { m,p})
                    .Join(_iUserRoleRepository.GetAll().Where(a=>a.userId.Equals(cookie.Id))
                    .AsNoTracking(),m=>m.p.roleId,r=>r.roleId,(m,p)=>m.m)
                    .Distinct()
                    .ToList();
            }
            var parentMenu = menulist.Where(a => a.parentMenuCode == "").OrderBy(a => a.listOrder).Select(a => new RoleMenuModel() { id = a.Id.ToString(),icon=a.menuIcon, code = a.menuCode, text = a.menuName, url = a.menuUrl, state = (a.menuCode == "userManger" ? "" : "closed")}).ToList();
            foreach (var item in parentMenu)
            {
                item.children = GetChildMenu(item.code, menulist);
            }
            return Task.FromResult(parentMenu);
        }
        /// <summary>
        /// 多级菜单获取子级
        /// </summary>
        /// <param name="parentMenuCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<RoleMenuModel> GetChildMenu(string parentMenuCode, List<Menu> data)
        {
            var parentMenu = data.Where(a => a.parentMenuCode == parentMenuCode).OrderBy(a => a.listOrder).Select(a => new RoleMenuModel() { id = a.Id.ToString(), icon = a.menuIcon, code = a.menuCode, text = a.menuName, url = a.menuUrl }).ToList();
            foreach (var item in parentMenu)
            {
                item.children = GetChildMenu(item.code, data); ;

            }
            return parentMenu;
        }
        public  List<MenuSetDto> GetAllMenu()
        {
            try
            {
                var parentMenu = new List<MenuSetDto>();
                UserCookie user = CookieHelper.GetLoginInUserInfo();
                if (user != null)
                {
                    if (user.IsAdmin || user.Identity == 2)
                    {
                        var data = _iMenuRepository.GetAll().ToList();
                        var allIst = data.ToList().ExMapToList<MenuDto>().Select(a => a.GetMenuSetDto()).ToList();
                        parentMenu = allIst.Where(a => string.IsNullOrEmpty(a.ParentMenuCode)).OrderBy(a => a.ListOrder).ToList();
                        foreach (var item in parentMenu)
                        {

                            item.ChildMenu = GetChildMenu(item.MenuCode, allIst);// allIst.Where(a => a.ParentMenuCode == item.MenuCode).OrderBy(a => a.ListOrder).ToList();
                        }
                        return parentMenu;
                    }
                    return parentMenu;
                }

                return parentMenu;
            }
            catch (Exception ex)
            {
                var parentMenu = new List<MenuSetDto>();
                return parentMenu;
            }

        }
        /// <summary>
        /// 多级菜单获取子级
        /// </summary>
        /// <param name="parentMenuCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<MenuSetDto> GetChildMenu(string parentMenuCode, List<MenuSetDto> data)
        {
            var parentMenu = data.Where(a => a.ParentMenuCode == parentMenuCode).OrderBy(a => a.ListOrder).ToList();
            foreach (var item in parentMenu)
            {
                item.ChildMenu = GetChildMenu(item.MenuCode, data); ;
               
            }
            return parentMenu;
        }

      
    }
}
