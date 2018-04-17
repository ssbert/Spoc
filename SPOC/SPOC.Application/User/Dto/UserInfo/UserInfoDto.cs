using Abp.AutoMapper;
using Abp.Domain.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPOC.User;
using SPOC.User.Dto.UserInfo;

namespace SPOC.User.Dto.UserInfo
{

    [Serializable]
    [AutoMapFrom(typeof(UserBase))]
   public   class UserInfoDto:Entity<Guid>
    {
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
        [StringLength(11)]
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
        /// 身份类型（1:学生 2:教师 3:管理员）
        /// </summary>
        public int identity { get; set; }

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

        /// <summary>
        /// 个性签名
        /// </summary>
        public string signature { get; set; }
        

        /// <summary>
        /// 未读私信条数
        /// </summary>
        public string newMessageNum { get; set; }


        /// <summary>
        /// 未读消息数目
        /// </summary>
        public int? newNotificationNum { get; set; }

        /// <summary>
        /// 实名认证状态
        /// </summary>
        public string approvalStatus { get; set; }

        /// <summary>
        /// 登录ip
        /// </summary>
        public string loginIp { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? loginTime { get; set; }


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
    }

    
}

public static class UserInfoDtoExt {

    public static UserInfoInputDto GetUserInfoInputDto(this UserInfoDto model) {

        UserInfoInputDto obj = new UserInfoInputDto();

        var perArr = obj.GetType().GetProperties();
        foreach (var item in perArr)
        {
            var per = model.GetType().GetProperty(item.Name) ?? null;
            if (per != null) { 
                item.SetValue(obj, model.GetType().GetProperty(item.Name).GetValue(model));
            }
        }
        return obj;
    }

    public static UserRegisterModel GetUserRegister(this UserInfoDto model)
    {
        return new UserRegisterModel()
        {
            PassWord = model.userPassWord,
            UserName = model.userLoginName,
            RegisterEmail = model.userEmail,
            RegisterMobile = model.userMobile,
            RegisterIpAddress = model.loginIp,
            gender = int.Parse(string.IsNullOrEmpty(model.userGender) ? "0" : model.userGender)

        };
    }
    public static EditDto GetEditDto(this UserInfoDto model)
    {
        return new EditDto()
        {
            Id = model.Id,
            answer = "",
            email = model.userEmail,
            gender = int.Parse(string.IsNullOrEmpty(model.userGender) ? "0" : model.userGender),
            idcard = model.userIdcard,
            ignoreoldpw = 0,
            mobile = model.userMobile,
            newpw = string.Empty,
            oldpw = string.Empty,
            questionid = 0,
            truename = model.userFullName,
            username = model.userLoginName

        };
    }

    public static UserDetailShow GetUserDetailShowModel(this UserInfoDto model)
    {

        return new UserDetailShow()
        {
            Id = model.Id,
            UserLoginName = model.userLoginName,
            UserName = string.IsNullOrWhiteSpace(model.userFullName) ? model.userLoginName : model.userFullName,
            Signature = model.signature,
            About = model.about,
            Identity = model.identity,
            IsAdmin = model.identity == 3,
            IsTeacher = model.identity == 2,
            HederImg = model.smallAvatar,
            WeiboIsOpen = model.weiboIsOpen,
            Weibo = model.weibo,
            WeChat = model.weChat,
            WeChatIsOpen = model.weChatIsOpen,
            QQNumber = model.qqNumber,
            QQIsOpen = model.qqIsOpen,
            PersonalPage = model.personalPage
        };
    }

}

public class CouponUserInfoDto : Entity<Guid>
{
    public string userId { get; set; }
    public string userLoginName { get; set; }

    public string userFullName { get; set; }

    public string departId { get; set; }

    public string departStr { get; set; }

    public List<string> peartIdList { get; set; }
    public List<string> peartStrList { get; set; }

} 
public class ApiUserInfoDto {
  public   string id { get; set; }
  public string loginname { get; set; }
  public string mobile { get; set; }
  public string email { get; set; }
  public string truename { get; set; }
  public string gender { get; set; }
  public string idcard { get; set; }
}
public class CenterUser
{
    /// <summary>
    /// 用户名
    /// </summary>
    private string _UserName = string.Empty;
    public string UserName
    {
        get { return _UserName; }
        set { _UserName = value; }
    }

    /// <summary>
    /// 用户头像
    /// </summary>
    private string _UserImageUrl = string.Empty;
    public string UserImageUrl
    {
        get { return _UserImageUrl; }
        set { _UserImageUrl = value; }
    }

    /// <summary>
    /// 用户id
    /// </summary>
    private string _UserId = string.Empty;
    public string UserId
    {
        get { return _UserId; }
        set { _UserId = value; }
    }

}
