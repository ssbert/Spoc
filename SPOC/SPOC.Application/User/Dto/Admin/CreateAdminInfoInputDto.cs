using System;
using Abp.Application.Services.Dto;
//using SmartUFO.SystemSet;


namespace SPOC.User.Dto.Admin
{
    public class CreateAdminInfoInputDto : EntityDto<Guid>
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
        /// 生日
        /// </summary>
        public DateTime? userBirthday { get; set; }



        /// <summary>
        /// 身份证号码
        /// </summary>
        public string userIdcard { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string userNational { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string userPolitical { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string adminInviteCode { get; set; }


        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime recentLoginTime { get; set; }

        /// <summary>
        /// 最近登录IP地址
        /// </summary>
        public string recentLoginIpAddress { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool isDel { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public string isAdmin { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool adminEnbleFlag { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string updateTime { get; set; }


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
        public AdminInfo GetAdminInfo()
        {
            return new AdminInfo(){
                Id=this.Id,
                userId=this.userId,
                adminInviteCode =this.adminInviteCode ,
                recentLoginTime =this.recentLoginTime ,
                recentLoginIpAddress =this.recentLoginIpAddress ,
                isDel =this.isDel ,
                adminEnbleFlag =this.adminEnbleFlag ,
                createTime = Convert.ToDateTime(this.createTime),
                updateTime = Convert.ToDateTime(this.updateTime),
                
            };
        }
      


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
                userNational = this.userNational,
                userPolitical = this.userPolitical,
                userBirthday = userBirthday,
                userIdcard = this.userIdcard,
                identity = 3
                 ,
                about = this.about,
                largeAvatar = this.largeAvatar,
                mediumAvatar = this.mediumAvatar,
                smallAvatar = this.smallAvatar,
                 approvalStatus="",
                  signature=this.signature
            };
        }
    }
}
