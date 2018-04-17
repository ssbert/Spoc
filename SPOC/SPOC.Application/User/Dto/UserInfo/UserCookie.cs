using System;

namespace SPOC.User.Dto.UserInfo
{
    [Serializable]
    public class UserCookie 
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserUid { get; set; }

        /// <summary>
        /// 登录的用户名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码（加密后）
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 是否为管理员
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserHeadImg { get; set; }

        public string PhoneNumber { get; set; }

        public string Birthday { get; set; }

        public string Gender { get; set; }
        /// <summary>
        ///角色类型
        /// </summary>
        public int Identity { get; set; }

        public bool UserEnbleFlag { get; set; }

        public string LoginIpAddress { get; set; }

        public string SessionId { get; set; }

        public bool IsCompleted { get; set; }
        public bool IsRemberMe  { get; set; }

        /// <summary>
        /// 是否为管理员登录
        /// </summary>
        public bool IsAdmminLogin { get; set; }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        public bool IsLogin { get; set; }
    }


    public class LoginSuccessData {

        public string userId { get; set; }
        public string loginName { get; set; }
        public string fullName { get; set; }
        public bool isCompleted { get; set; }

        public string gender { get; set; }

        public int identity { get; set; }

        public string email { get; set; }

        public string idCard { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserHeadImg { get; set; }

        public string phoneNumber { get; set; }

        public string birthday { get; set; }

    }

    public class PlatSession
    {

        /// <summary>
        /// 学习平台ID
        /// </summary>
        public string LearnPlatformId { get; set; }
        /// <summary>
        /// 学习平台默认页
        /// </summary>
        public string PlatformDefaultPage { get; set; }
    }
    public class RemeberMeUserDto {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}
