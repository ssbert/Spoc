using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.User
{
    public class UserBase : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        public UserBase()
        {

            if (this.Id == null)
            {
                this.Id = Guid.NewGuid();
            }
            this.approvalStatus = "approved";
        }


        /// <summary>
        /// 用户名
        /// </summary>
         [StringLength(50)]
        public string userLoginName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        [StringLength(255)]
        public string userPassWord { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
         [StringLength(50)]
        public string userFullName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(11)]
        public string userMobile { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(50)]
        public string userEmail { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
         [StringLength(20)]
        public string userGender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
           
        public DateTime? userBirthday { get; set; }



        /// <summary>
        /// 身份证号码
        /// </summary>
         [StringLength(20)]
        public string userIdcard { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [StringLength(10)]
        public string userNational { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        [StringLength(20)]
        public string userPolitical { get; set; }



        /// <summary>
        /// 身份类型（1:学生 2:教师 3:管理员）
        /// </summary>
        public int identity { get; set; }

        /// <summary>
        /// 小头像
        /// </summary>
         [StringLength(255)]
        public string smallAvatar { get; set; }

         /// <summary>
        /// 中头像
        /// </summary>
        [StringLength(255)]
        public string mediumAvatar { get; set; }

          /// <summary>
        /// 大头像
        /// </summary>
        [StringLength(255)]
        public string largeAvatar { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [StringLength(2000)]
        public string about { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [StringLength(500)]
        public string signature { get; set; }

       
        
        /// <summary>
        /// 未读私信条数
        /// </summary>
        public int? newMessageNum { get; set; }


        /// <summary>
        /// 未读消息数目
        /// </summary>
        public int?  newNotificationNum { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        [StringLength(50)]
        [DefaultValue("approved")]
        public string approvalStatus { get; set; }

        /// <summary>
        /// 登录ip
        /// </summary>
        [StringLength(50)]
        public string loginIp { get; set; }

        [StringLength(50)]
        public string sessionId { get; set; }

        /// <summary>
        /// 是否已完善资料
        /// </summary>
        public bool isCompleted { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? loginTime { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool userEnbleFlag { get; set; }


        /// <summary>
        /// QQ号码
        /// </summary>
        [StringLength(15)]
        public string qqNumber { get; set; }

        /// <summary>
        /// QQ是否公开
        /// </summary>
        [StringLength(5)]
        public string qqIsOpen { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        [StringLength(20)]
        public string weChat { get; set; }

        /// <summary>
        /// 微信是否公开
        /// </summary>
        [StringLength(5)]
        public string weChatIsOpen { get; set; }
        /// <summary>
        /// 微博
        /// </summary>
        [StringLength(50)]
        public string weibo { get; set; }

        /// <summary>
        /// 微博是否公开
        /// </summary>
        [StringLength(5)]
        public string weiboIsOpen { get; set; }
        /// <summary>
        /// 个人主页
        /// </summary>
        [StringLength(50)]
        public string personalPage { get; set; }

        /// <summary>
        /// 新课网userId
        /// </summary>
        //[StringLength(36)]
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid newMoocUserId { get; set; }
        
   
        
    }
}
