using System;
using Abp.Application.Services.Dto;

namespace SPOC.User.Dto.Admin
{
    public class UpdateAdminInfoInputDto : EntityDto<Guid>
    {
        public Guid userId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string userLoginName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string userPassWord { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string userFullName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string userMobile { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        public string userEmail { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string userGender { get; set; }
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public string isAdmin { get; set; }
        /// <summary>
        /// 用户权限
        /// </summary>
       // public List<AdminPermissionInfo> adminPermissionInfoList { get; set; }

     
        /// <summary>
        /// 小头像
        /// </summary>
        public string smallAvatar { get; set; }

        /// <summary>
        /// 中头像
        /// </summary>
        public string mediumAvatar { get; set; }

        /// <summary>
        /// 大头像
        /// </summary>
        public string largeAvatar { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string about { get; set; }

        public string signature { get; set; }
        /// <summary>
        /// 未读私信条数
        /// </summary>
        public int? newMessageNum { get; set; }


        /// <summary>
        /// 未读消息数目
        /// </summary>
        public int? newNotificationNum { get; set; }

        /// <summary>
        /// 实名认证状态
        /// </summary>
        public string approvalStatus { get; set; }
      

        public UserBase GetUser()
        {

            return new UserBase()
            {
                Id = this.userId,
                userLoginName = this.userLoginName,
                userMobile = this.userMobile,
                userEmail = this.userEmail,
                userFullName = this.userFullName,
                userGender = this.userGender,
                userPassWord = this.userPassWord,
                identity = 3
                 ,
                about = this.about,
                largeAvatar = this.largeAvatar,
                mediumAvatar = this.mediumAvatar,
                smallAvatar = this.smallAvatar,
                 approvalStatus="",
                signature = this.signature
            };
        }


    }
}
