using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Runtime.Validation;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;

namespace SPOC.SysSetting.RoleManageDTO
{
    public class RoleManageInputDto : EasyuiDto
    {
        public string id { get; set; }

        public string roleName { get; set; }

        public string roleCode { get; set; }
        public string roleGroup { get; set; }
        public string description { get; set; }
        /// <summary>
        /// 菜单授权代码集合 逗号分隔
        /// </summary>
        public string permissionId { get; set; }

    }
    /// <summary>
    /// 获取代理列表的查询条件
    /// </summary>
    public class GetRoleUserPaginationCondition : PaginationInputDto
    {
       
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string userLoginName { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string userFullName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string userMobile { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string userEmail { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string userGender { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid roleId { get; set; }
       
        public int identity { get; set; }
    }

    /// <summary>
    /// 角色用户DTO
    /// </summary>
    public class UserRoleInputDto : ICustomValidate
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public List<Guid> userIdList { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid roleId { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (userIdList == null || !userIdList.Any())
            {

                context.Results.Add(new ValidationResult("无效的用户列表"));
            }
        }
    }

}
