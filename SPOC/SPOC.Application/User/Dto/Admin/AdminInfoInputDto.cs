using SPOC.Common.Dto;

namespace SPOC.User.Dto.Admin
{
    public class AdminInfoInputDto : EasyuiDto
    {

        public AdminInfoInputDto()
        {

           
        }

        /// <summary>
        /// 角色（组织架构）
        /// </summary>
        public string roleStr { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string userLoginName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string userMobile { get; set; }

         

        /// <summary>
        /// 姓名
        /// </summary>
        public string userFullName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string userEmail { get; set; }



        
    }
}
