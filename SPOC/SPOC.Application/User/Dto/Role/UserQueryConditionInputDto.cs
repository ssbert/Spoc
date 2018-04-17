using SPOC.Common.Pagination;

namespace SPOC.User.Dto.Role
{
    /// <summary>
    /// 用户角色设置的用户查询条件
    /// </summary>
    public class UserQueryConditionInputDto:PaginationInputDto
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
        /// 用户角色
        /// 1：学生
        /// 2：教师
        /// 3：管理员
        /// </summary>
        public int identity { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string userMobile { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string userEmail { get; set; }
        /// <summary>
        /// 用户性别
        /// </summary>
        public string userGender { get; set; }
    }
}