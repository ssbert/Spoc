using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Runtime.Validation;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Extensions;
using SPOC.Common.Helper;
using SPOC.Common.Pagination;
using SPOC.User.Dto.Role;
using Abp.AutoMapper;
namespace SPOC.User
{
    /// <summary>
    /// 用户角色服务接口
    /// </summary>
    public class RoleService:ApplicationService, IRoleService
    {
        private readonly IRepository<UserBase, Guid> _iUsersRep;
        private readonly IRepository<AdminInfo, Guid> _iAdminInfoRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;

        /// <summary>
        /// 构造函数
        /// </summary>
        public RoleService(IRepository<UserBase, Guid> iUsersRep, IRepository<AdminInfo, Guid> iAdminInfoRep, 
            IRepository<TeacherInfo, Guid> iTeacherInfoRep)
        {
            _iUsersRep = iUsersRep;
            _iAdminInfoRep = iAdminInfoRep;
            _iTeacherInfoRep = iTeacherInfoRep;
        }
        [DisableValidation]
        public async Task<PaginationOutputDto<UserListItem>> UserPagination(UserQueryConditionInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }

            if (!cookie.IsAdmin)
            {
                throw new UserFriendlyException("权限不足");
            }

            var users = from u in _iUsersRep.GetAll()
                        where (string.IsNullOrEmpty(input.userLoginName) || u.userLoginName.Contains(input.userLoginName))
                        && (string.IsNullOrEmpty(input.userFullName) || u.userFullName.Contains(input.userFullName))
                        && (input.identity == 0 || u.identity == input.identity)
                        && (string.IsNullOrEmpty(input.userMobile) || u.userMobile.Contains(input.userMobile))
                        && (string.IsNullOrEmpty(input.userEmail) || u.userEmail.Contains(input.userEmail))
                        && (string.IsNullOrEmpty(input.userGender) || u.userGender == input.userGender)
                        select u;

            if (string.IsNullOrEmpty(input.sort))
            {
                users = users.OrderBy(u => u.userLoginName);
            }
            else
            {
                users = users.OrderBy(input.sort + " " + input.order);
            }
            var total = await users.CountAsync();
            var rows = await users.Skip(input.skip).Take(input.pageSize).ToListAsync();
            var list= new PaginationOutputDto<UserListItem>
            {
                rows = rows.MapTo<List<UserListItem>>(),
                total = total
            };
            return list;
        }

        public async Task ChangeRole(UserRolesInputDto input)
        {
            var userRoleDic = new Dictionary<Guid, int>();
            foreach (var item in input.roleDic)
            {
                var identity = input.roleDic[item.Key];
                var userId = item.Key;
                if (identity < 1 || identity > 3)
                {
                    throw new UserFriendlyException("无效的角色");
                }

                var cookie = CookieHelper.GetLoginInUserInfo();
                if (!cookie.IsLogin)
                {
                    throw new UserFriendlyException("未登录或登录已过期");
                }

                if (!cookie.IsAdmin)
                {
                    throw new UserFriendlyException("权限不足");
                }

                var user = await _iUsersRep.GetAll().Where(a => a.Id == userId)
                    .Select(a=>new {a.Id, a.identity})
                    .FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new UserFriendlyException("无效的用户");
                }

                userRoleDic.Add(user.Id, user.identity);
            }


            foreach (var item in userRoleDic)
            {
                var identity = userRoleDic[item.Key];
                var newIdentity = input.roleDic[item.Key];
                var userId = item.Key;
                if (identity == newIdentity)
                {
                    return;
                }

                if (identity == 2)
                {
                    await _iTeacherInfoRep.DeleteAsync(a => a.userId == userId);
                }
                else if (identity == 3)
                {
                    await _iAdminInfoRep.DeleteAsync(a => a.userId == userId);
                }

                if (newIdentity == 2)
                {
                    await _iTeacherInfoRep.InsertAsync(new TeacherInfo
                    {
                        Id = Guid.NewGuid(),
                        userId = userId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        teacherInviteCode = await InviteCodeHelper.NewTeacherInviteCode(_iTeacherInfoRep)
                    });
                }
                else if (newIdentity == 3)
                {
                    await _iAdminInfoRep.InsertAsync(new AdminInfo
                    {
                        Id = Guid.NewGuid(),
                        userId = userId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                    });
                }

                await _iUsersRep.UpdateAsync(userId, u =>
                {
                    u.identity = newIdentity;
                    return Task.FromResult(u);
                });
            }
            
        }
    }
}