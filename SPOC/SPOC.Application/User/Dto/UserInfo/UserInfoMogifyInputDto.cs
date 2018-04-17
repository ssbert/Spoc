using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Abp.Domain.Entities;

namespace SPOC.User.Dto.UserInfo
{
    public class UserInfoMogifyInputDto:Entity<Guid>
    {
        [StringLength(20,MinimumLength=4, ErrorMessage="用户名格式不正确，4~20位")]
        [Required(ErrorMessage = "请输入用户名")]
        [UserNameFormatCheck]
        [Remote("UserNameExistCheckByModify", " ../../Account", HttpMethod = "Post", ErrorMessage = "该用户名已存在")]
      //  [RegularExpression(@"[A-Za-z0-9]{4,16}", ErrorMessage = "用户名格式不正确，4~16位")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        
    }

    [Serializable]
    public class EditDto : Entity<Guid>
    {

        public string idcard { get; set; }
        //无手机号通过用户名修改
        public string oldusername { get; set; }
        public string username { get; set; }
        public string truename { get; set; }
        public string oldpw { get; set; }
        public string newpw { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public int gender { get; set; } //性别
        /// <summary>
        /// 是否忽略旧密码
        /// </summary>
        public int ignoreoldpw { get; set; }
        /// <summary>
        /// 安全提问索引
        /// </summary>
        public int questionid { get; set; }
        /// <summary>
        /// 安全提问答案
        /// </summary>
        public string answer { get; set; }


    }
    public class ModifyMobile
    {
        /// <summary>
        /// 新增参数 无手机号码的通过旧用户名修改
        /// </summary>
        public string username { get; set; }
        public string oldmobile { get; set; }
        public string newmobile { get; set; }

    }

    public class ModifyPassWord {

        public string userMobile { get; set; }
        public string userName { get; set; }
        public Guid userId { get; set; }
        public string oldPassWord { get; set; }
        public string newPassWord { get; set; }

        public string confirmPwd { get; set; }
    }

    public class UserEmailModifyView {

        private string _userMobile = "";
        private string _userName = "";
        private string _passWord = "";
        private string _currentEmail = "";
        private string _newEmail = "";
        private Guid _userId = Guid.Empty;

        public string userMobile { get { return _userMobile; } set { _userMobile = value; } }
        public string userName { get { return _userName; } set { _userName = value; } }
        public Guid userId { get { return _userId; } set { _userId = value; } }

        public string passWord { get { return _passWord; } set { _passWord = value; } }

        public string currentEmail { get { return _currentEmail; } set { _currentEmail = value; } }
        public string newEmail { get { return _newEmail; } set { _newEmail = value; } }

        public void SetTrim() {
            this.userMobile = this.userMobile.Trim();
            this.userName = this.userName.Trim();
            this.passWord = this.passWord.Trim();
            this.currentEmail = (this.currentEmail??"").Trim();
            this.newEmail = this.newEmail.Trim();
        }

    }

    public class MobileModifyView
    {
       
        public string oldMobile { get; set; }

        [StringLength(11, ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机")]
        [Required(ErrorMessage = "请输入手机号码")]
        [RegularExpression(@"^[1]+[3,5,8,7]+\d{9}", ErrorMessage = "手机号码格式不正确")]
        [Remote("MobileExistCheckByRegister", "Account", HttpMethod = "Post", ErrorMessage = "该手机号码已存在")]
        public string newMobile { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
       [Required(ErrorMessage = "请输入验证码")]
       [Remote("VcodeExistCheckByForgetPwd", "Account", HttpMethod = "Post", ErrorMessage = "验证码不正确")]
        public string code { get; set; }

        /// <summary>
        /// /短信验证码
        /// </summary>
        [Required(ErrorMessage = "请输入短信验证码")]
        public string smsCode { get; set; }
        
        /// <summary>
        /// 登录密码
        /// </summary>
        [Required(ErrorMessage = "请输入登录密码")]
        [Remote("CheckPwd", "Setting", HttpMethod = "Post", ErrorMessage = "登录密码错误")]
        public string password { get; set; }


    }
}
