using System;
using Abp.AutoMapper;
using SPOC.User;

namespace SPOC.SysSetting.RoleManageDTO
{
    [AutoMapFrom(typeof(RoleManage))]
    public class RoleManageDto
    {
        public string id { get; set; }
        public string roleName { get; set; }

        public string roleCode { get; set; }
        public string menuNames { get; set; }
        public string menusCodes { get; set; }
        public string description { get; set; }
        public string roleGroup { get; set; }
        public bool isDefault { get; set; }
    }

    public class UserSelectOutputDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid userId { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string userLoginName { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string userFullName { get; set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public int identity { get; set; }
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
    }

}
