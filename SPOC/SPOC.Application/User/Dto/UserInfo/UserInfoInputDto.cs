using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SPOC.Common;
using SPOC.Common.Dto;
using SPOC.SysSetting;

namespace SPOC.User.Dto.UserInfo
{
    public class UserInfoInputDto : Entity<Guid>
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
        [StringLength(32, ErrorMessage = "姓名不超过32位")]
        //[Required]
        //   [RegularExpression(@"^([\u4e00-\u9fa5]+|([a-zA-Z]+\s?)+)$", ErrorMessage = "姓名不正确")]
        [Display(Name = "姓名")]
        // [DataType(DataType.EmailAddress)]
        public string userFullName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(11)]
        public string userMobile { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [RegularExpression("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$", ErrorMessage = "邮箱格式不正确")]
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
        [StringLength(32)]
        //[Required]


        [RegularExpression(@"/(^\d{15}$)|(^\d{18}$)|(^\d{17}[X]|[x]$)/", ErrorMessage = "身份证号码格式不正确")]
        // [Remote("UserIdcardExistCheckByModify", "~/StudyPlatform/User/", HttpMethod = "Post", ErrorMessage = "该身份证号码已存在")]
        [Display(Name = "身份证号码")]
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
        /// 简介
        /// </summary>
        public string about { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [StringLength(250, ErrorMessage = "不超过250个字符")]
        public string signature { get; set; }

        /// <summary>
        /// QQ号码
        /// </summary>
        [StringLength(15, ErrorMessage = "最大不超过15个字符")]
        public string qqNumber { get; set; }

        /// <summary>
        /// QQ是否公开
        /// </summary>
        [StringLength(5)]
        public string qqIsOpen { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        [StringLength(20, ErrorMessage = "最大不超过20个字符")]
        public string weChat { get; set; }

        /// <summary>
        /// 微信是否公开
        /// </summary>
        [StringLength(5)]
        public string weChatIsOpen { get; set; }
        /// <summary>
        /// 微博
        /// </summary>
        [StringLength(50, ErrorMessage = "最大不超过50个字符")]
        public string weibo { get; set; }

        /// <summary>
        /// 微博是否公开
        /// </summary>
        [StringLength(5)]
        public string weiboIsOpen { get; set; }

        /// <summary>
        /// 个人主页
        /// </summary>
        [StringLength(50, ErrorMessage = "最大不超过50个字符")]
        public string personalPage { get; set; }
        /// <summary>
        /// 个人简历
        /// </summary>
        public string teacherPersonalResume { get; set; }

        /// <summary>
        /// 教师信息表id
        /// </summary>
        public Guid teacherId { get; set; }

    }


}
public class UserInfoQueryInputDto : EasyuiDto
{

    public Guid id { get; set; }
    public Guid userId { get; set; }
    public string userName { get; set; }

    public string identity { get; set; }



}


public enum userLoginType
{
    userName = 1,
    userMobel = 2,
    userEmail = 3,
    userIdCard = 4
}

public class UserNameFormatCheckAttribute : ValidationAttribute, IClientValidatable
{
    private readonly ISiteSetService _iSiteSetService;
    public UserNameFormatCheckAttribute(ISiteSetService iSiteSetService)
    {
        _iSiteSetService = iSiteSetService;
    }
    public UserNameFormatCheckAttribute()
    {

    }
    public string Pattern
    {
        get
        {
            return @"/^(?=.*[a-zA-Z])(?=.*\d).*$/";
        }
    }
    public override string FormatErrorMessage(string name)
    {
        return "用户名格式错误，必须包含英文和数字";
    }
    public override bool IsValid(object value)
    {
        /*  if (_iSiteSetService != null) {
              if (_iSiteSetService.GetAllSiteSet().usernameContainEN.Trim() == "open") {
                  var regex = new Regex(this.Pattern);
                  return regex.IsMatch(value.ToString());
              }
          }*/
        return true;
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        var validationRule = new ModelClientValidationRule
        {
            ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            //  ValidationType = "url",
            //ValidationType = "usernameformatcheck",
            ValidationType = "usernameformatcheck",

        };

        yield return validationRule;
    }
}