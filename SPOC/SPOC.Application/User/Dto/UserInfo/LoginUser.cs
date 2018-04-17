using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SPOC.User.Dto.UserInfo
{
    public class LoginUser
    {

        public LoginUser()
        {
            UserName = string.Empty;
            Password = string.Empty;
        }

        //    [StringLength(255)]
        [Required]
        //[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9._]+\.[A-Za-z]{2,4}", ErrorMessage = "邮箱地址格式不正确")]
        [Display(Name = "用户名")]
        // [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        //  [StringLength(255)]
        [Display(Name = "登录密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public void Trim()
        {
            if (!string.IsNullOrEmpty(UserName)) UserName = UserName.Trim();

            if (!string.IsNullOrEmpty(Password)) Password = Password.Trim();

        }

        public string loginIpAddress { get; set; }

    }

    public class UserLoginViewModel
    {
        public UserLoginViewModel()
        {
            UserName = string.Empty;
            PassWord = string.Empty;
            loginStatu = string.Empty;
            AutoLogin = false;
            ParentUrl = string.Empty;

        }

        // [StringLength(255)]
        //  [Required]
        [Required(ErrorMessage = "请输入用户名")]
        // [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "用戶名不正確")]
        [Display(Name = "用户名")]
        // [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        // [StringLength(255)]
        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }

        public string loginStatu { get; set; }

        public string loginMsg { get; set; }


        public string IsRememberMe { get; set; }

        /// <summary>
        /// 父级Url
        /// </summary>
        public string ParentUrl { get; set; }

        public bool AutoLogin { get; set; }

        private string userRegisterDisplay = "true";
        public string UserRegisterDisplay
        {
            get { return string.IsNullOrEmpty(userRegisterDisplay) ? "true" : userRegisterDisplay; }
            set { userRegisterDisplay = value ?? "true"; }
        }
        public void Trim()
        {
            if (!string.IsNullOrEmpty(UserName)) UserName = UserName.Trim();

            if (!string.IsNullOrEmpty(PassWord)) PassWord = PassWord.Trim();

        }

    }

    public class UserRegisterModel
    {

        [StringLength(16, ErrorMessage = "用戶名不超过16个字符")]
        [Required(ErrorMessage = "请输入用户名")]
        //    [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9_]\w{2,16}$", ErrorMessage = "用户名不合法（字母开头，允许4-16字节，允许字母数字下划线）")]
        //[RegularExpression(@"^(?!\d{4,16}$)(?:[a-z\d_]{4,16}|[\u4E00-\u9FA5]{2,8})$", ErrorMessage = "用户名不合法（中、英文均可，最长16个英文或8个汉字）")]
        [Display(Name = "用户名")]
        [Remote("UserNameExistCheck", "Account", HttpMethod = "Post", ErrorMessage = "该用户已存在")]
        public string UserName { get; set; }
        [Display(Name = "真实姓名")]
        public string UserFullName { get; set; }

        //[StringLength(20, ErrorMessage = "密码不超过20个字符")]
        [Display(Name = "密码")]
        //[RegularExpression(@"^[0-9a-zA-Z]\w{4,20}$", ErrorMessage = "密码格式不正确，(5-20位英文、数字、符号)！")]
        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }


        /// <summary>
        /// 手机
        /// </summary>
        [StringLength(11, ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机")]
        //[Required(ErrorMessage = "请输入手机号码")]
        [RegularExpression(@"^[1]+[3,5,8,7]+\d{9}", ErrorMessage = "手机号码格式不正确")]

        [Remote("MobileExistCheck", "Account", HttpMethod = "Post", ErrorMessage = "该手机号码已存在")]
        public string RegisterMobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [DataType(DataType.EmailAddress, ErrorMessage = "Email格式不正确")]
        //[Required(ErrorMessage = "请输入邮箱")]
        [RegularExpression("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$", ErrorMessage = "邮箱格式不正确")]
        [Remote("EmailExistCheck", "Account", HttpMethod = "Post", ErrorMessage = "该邮箱已存在")]
        public string RegisterEmail { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviteCode { get; set; }

        /// <summary>
        /// 短信验证码
        /// </summary>
        public string SmsCode { get; set; }

        public string RegisterIpAddress { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public string ClassId { get; set; }

        public string msg { get; set; }

        public string registerStatu { get; set; }

        public string returnUrl { get; set; }


        private string registerType = "student";
        public string RegisterType { get { return registerType; } set { registerType = value; } }
        public int gender { get; set; } //性别
        public void Trim()
        {
            if (!string.IsNullOrEmpty(UserName)) UserName = UserName.Trim();

            if (!string.IsNullOrEmpty(PassWord)) PassWord = PassWord.Trim();

            // if (!string.IsNullOrEmpty(RegisterMobileOrEmail)) RegisterMobileOrEmail = RegisterMobileOrEmail.Trim();
            if (!string.IsNullOrEmpty(RegisterMobile)) RegisterMobile = RegisterMobile.Trim();

            if (!string.IsNullOrEmpty(RegisterEmail)) RegisterEmail = RegisterEmail.Trim();

        }
    }

    public class ForgetPwdViewModel
    {

        private string msg = "";
        public string Msg { get { return msg; } set { msg = value; } }

        private string statu = "";
        public string Statu { get { return statu; } set { statu = value; } }

        private string userName = "";
        [Required(ErrorMessage = "请输入用户名")]
        [StringLength(15, ErrorMessage = "用戶名不超过15个字符")]
        [Display(Name = "用户名")]
        [Remote("UserNameExistCheckByForgetPwd", "Account", HttpMethod = "Post", ErrorMessage = "该用户名不存在")]
        public string UserName { get { return userName; } set { userName = value; } }

        /// <summary>
        /// 验证的邮箱
        /// </summary>
        private string email = "";
        [Required(ErrorMessage = "请输入邮箱")]
        [RegularExpression("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$", ErrorMessage = "邮箱格式不正确")]
        [Display(Name = "邮箱")]
        public string Email { get { return email; } set { email = value; } }

        /// <summary>
        /// 验证码
        /// </summary>
        private string comfrimCode = "";

        [Required(ErrorMessage = "请输入验证码")]
        [Remote("VcodeExistCheckByForgetPwd", "Account", HttpMethod = "Post", ErrorMessage = "验证码不正确")]
        public string ComfrimCode { get { return comfrimCode; } set { comfrimCode = value; } }

        public void modelTrim()
        {
            this.UserName = this.UserName.Trim();
            this.Email = this.Email.Trim();
            this.ComfrimCode = this.ComfrimCode.Trim();
        }

        public void reSet()
        {
            this.UserName = "";
            this.Email = "";
            this.ComfrimCode = "";
        }
    }

    public class PwdUpdateViewModel
    {

        public string Email { get; set; }

        public string Statu { get; set; }

        public string Msg { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "请输入用户名")]
        [StringLength(15, ErrorMessage = "用戶名不超过15个字符")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "请输入新的密码")]
        [StringLength(30, ErrorMessage = "新密码不超过30个字符")]
        [Display(Name = "新密码")]
        public string NewPwd { get; set; }

        /// <summary>
        /// 确认新密码
        /// </summary>
        [Required(ErrorMessage = "请确认新的密码")]
        [System.ComponentModel.DataAnnotations.Compare("NewPwd", ErrorMessage = "两次输入的密码不一致")]
        [Display(Name = "新密码")]
        public string ConfirmPwd { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "请输入验证码")]
        [Remote("VcodeExistCheckByForgetPwd", "Home", HttpMethod = "Post", ErrorMessage = "验证码不正确")]
        public string ComfrimCode { get; set; }

        public void reSet()
        {

            this.NewPwd = "";
            this.ComfrimCode = "";
        }
    }

    public class SmsLoginViewModel
    {
        public SmsLoginViewModel()
        {
            mobile = string.Empty;
            smsCode = string.Empty;
            parentUrl = string.Empty;
            loginMsg = string.Empty;
            isLoginSuccess = false;
        }
        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(11, ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机")]
        [Required(ErrorMessage = "请输入手机号码")]
        [RegularExpression(@"^[1]+[3,5,8,7]+\d{9}", ErrorMessage = "手机号码格式不正确")]
        public string mobile { get; set; }

        /// <summary>
        /// 短信验证码
        /// </summary>
        [Display(Name = "验证码")]
        [Required(ErrorMessage = "请输入短信验证码")]
        public string smsCode { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "请输入验证码")]
        [Remote("VcodeExistCheckByForgetPwd", "Home", HttpMethod = "Post", ErrorMessage = "验证码不正确")]
        public string code { get; set; }

        /// <summary>
        /// 路劲
        /// </summary>
        public string parentUrl { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string loginMsg { get; set; }

        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool isLoginSuccess { get; set; }


    }


}
