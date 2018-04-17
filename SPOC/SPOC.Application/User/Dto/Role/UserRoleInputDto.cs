using System;
using System.Collections.Generic;

namespace SPOC.User.Dto.Role
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserRolesInputDto
    {
        /// <summary>
        /// key: 用户ID
        /// value: 用户角色
        /// 1：学生
        /// 2：教师
        /// 3：管理员
        /// </summary>
        public Dictionary<Guid, int> roleDic { get; set; }
    }
}