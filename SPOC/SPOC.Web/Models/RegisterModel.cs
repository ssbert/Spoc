using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Abp.AutoMapper;
using SPOC.User.Dto.UserInfo;

namespace SPOC.Web.Models
{
     //[AutoMapFrom(typeof(UserRegisterModel))]
    public class RegisterModel
    {
        //[StringLength(16, MinimumLength = 4, ErrorMessage = "用戶名不超过16个字符,最短不能低于4个字符")]
        //[Required(ErrorMessage = "请输入用户名")]
        //[RegularExpression(@"^(?!\d{4,16}$)(?:[a-z\d_]{4,16}|[\u4E00-\u9FA5]{2,8})$", ErrorMessage = "用户名不合法（中、英文均可，最长16个英文或8个汉字）")]
        [UserNameFormatCheck]
        [Display(Name = "用户名")]
        [Remote("UserNameExistCheck", "Account", HttpMethod = "Post", ErrorMessage = "该用户名已被注册，可用该用户名直接登录")]
        public string UserName { get; set; }
        [Display(Name = "真实姓名")]
        public string UserFullName { get; set; }
       // [StringLength(20, MinimumLength = 5, ErrorMessage = "密码不超过20个字符,最短不能低于5个字符")]
        [Display(Name = "密码")]
        //[RegularExpression(@"^[0-9a-zA-Z]\w{4,20}$", ErrorMessage = "密码格式不正确，(5-20位英文、数字、符号)！")]
       // [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
        public string EncucriptPassWord { get; set; }

        public string EncucriptUserName { get; set; }

        public string EncucriptEmail { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [StringLength(11, ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机")]
        //[Required(ErrorMessage = "请输入手机号码")]
        [RegularExpression(@"^[1]+[3,5,8,7]+\d{9}", ErrorMessage = "手机号码格式不正确")]
        //[Remote("MobileExistCheckByRegister", "Account", HttpMethod = "Post", ErrorMessage = "该手机号码已存在")]
        [Remote("MobileExistCheckByRegister", "Account", HttpMethod = "Post", ErrorMessage = "该手机号码已被注册，可用该手机号码直接登录")]
        public string RegisterMobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [DataType(DataType.EmailAddress, ErrorMessage = "Email格式不正确")]
        [RegularExpression("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$", ErrorMessage = "邮箱格式不正确")]
        [Remote("EmailExistCheck", "Account", HttpMethod = "Post", ErrorMessage = "该邮箱已被注册")]
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
        /// 班级ID
        /// </summary>
        public string ClassId { get; set; }

        public string msg { get; set; }

        public string registerStatu { get; set; }

        public string returnUrl { get; set; }

        private string registerType = "student";
        public string RegisterType { get { return registerType; } set { registerType = value; } }

        public void Trim()
        {
            if (!string.IsNullOrEmpty(UserName)) UserName = UserName.Trim();

            if (!string.IsNullOrEmpty(PassWord)) PassWord = PassWord.Trim();

            // if (!string.IsNullOrEmpty(RegisterMobileOrEmail)) RegisterMobileOrEmail = RegisterMobileOrEmail.Trim();
            if (!string.IsNullOrEmpty(RegisterMobile)) RegisterMobile = RegisterMobile.Trim();

            if (!string.IsNullOrEmpty(RegisterEmail)) RegisterEmail = RegisterEmail.Trim();

        }

        public UserRegisterModel GetUserRegisterModel() {

            return new UserRegisterModel() { 
             InviteCode=this.InviteCode,
             RegisterMobile = this.RegisterMobile,
               msg=this.msg,
                ClassId= this.ClassId,
                 PassWord=this.PassWord,
                   RegisterEmail=this.RegisterEmail,
                    RegisterIpAddress=this.RegisterIpAddress,
                     registerStatu=this.registerStatu,
                      returnUrl=this.returnUrl,
                       SmsCode=this.SmsCode,
                        UserName=this.UserName,
             RegisterType = this.RegisterType
            };
        }
    }

    public class AdminRegisterModel
    {
        [StringLength(16, MinimumLength = 4, ErrorMessage = "用戶名不超过16个字符,最短不能低于4个字符")]
        [Required(ErrorMessage = "请输入用户名")]
        //[RegularExpression(@"^(?!\d{4,16}$)(?:[a-z\d_]{4,16}|[\u4E00-\u9FA5]{2,8})$", ErrorMessage = "用户名不合法（中、英文均可，最长16个英文或8个汉字）")]
        [UserNameFormatCheck]
        [Display(Name = "用户名")]
        [Remote("UserNameExistCheck", "Account", HttpMethod = "Post", ErrorMessage = "该用户名已被注册，可用该用户名直接登录")]
        public string UserName { get; set; }
  
        [StringLength(20, MinimumLength = 5, ErrorMessage = "密码不超过20个字符,最短不能低于5个字符")]
        [Display(Name = "密码")]
       
        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
        public string EncucriptPassWord { get; set; }

        public string EncucriptUserName { get; set; }

        public string EncucriptEmail { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [StringLength(11, ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机")]
        //[Required(ErrorMessage = "请输入手机号码")]
        [RegularExpression(@"^[1]+[3,5,8,7]+\d{9}", ErrorMessage = "手机号码格式不正确")]
        [Remote("MobileExistCheckByRegister", "Account", HttpMethod = "Post", ErrorMessage = "该手机号码已被注册，可用该手机号码直接登录")]
        public string RegisterMobile { get; set; }


       

        /// <summary>
        /// 短信验证码
        /// </summary>
        public string SmsCode { get; set; }

        public string RegisterIpAddress { get; set; }


        public string msg { get; set; }

        public string registerStatu { get; set; }

        public string returnUrl { get; set; }

        private string registerType = "student";
        public string RegisterType { get { return registerType; } set { registerType = value; } }

        public void Trim()
        {
            if (!string.IsNullOrEmpty(UserName)) UserName = UserName.Trim();

            if (!string.IsNullOrEmpty(PassWord)) PassWord = PassWord.Trim();

            // if (!string.IsNullOrEmpty(RegisterMobileOrEmail)) RegisterMobileOrEmail = RegisterMobileOrEmail.Trim();
            if (!string.IsNullOrEmpty(RegisterMobile)) RegisterMobile = RegisterMobile.Trim();

           

        }

        public UserRegisterModel GetUserRegisterModel()
        {

            return new UserRegisterModel()
            {

                RegisterMobile = this.RegisterMobile,
                msg = this.msg,
                PassWord = this.PassWord,
                RegisterIpAddress = this.RegisterIpAddress,
                registerStatu = this.registerStatu,
                returnUrl = this.returnUrl,
                SmsCode = this.SmsCode,
                UserName = this.UserName,
                RegisterType = this.RegisterType
            };
        }
    }


    public class ForgetPwdViewModel {

         private string msg = "";
         public string Msg { get{return msg;}set{msg=value;}}

         private string statu = "";
         public string Statu{ get{return statu;}set{statu = value;}}

         private string userName = "";
         [Required(ErrorMessage = "请输入用户名")]
         [StringLength(15, ErrorMessage = "用戶名不超过15个字符")]
         [Display(Name = "用户名")]
         [Remote("UserNameExistCheckByForgetPwd", "Account", HttpMethod = "Post", ErrorMessage = "该用户名不存在")]
         public string UserName{get{return userName;}set{ userName = value;}}

         /// <summary>
         /// 验证的邮箱
         /// </summary>
         private string email = "";
         [Required(ErrorMessage = "请输入邮箱")]
         [RegularExpression("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$", ErrorMessage = "邮箱格式不正确")]
         [Display(Name = "邮箱")]
         public string Email{get{return email;}set{ email = value;} }

         /// <summary>
         /// 验证码
         /// </summary>
         private string comfrimCode = "";

          [Required(ErrorMessage = "请输入验证码")]
          [Remote("VcodeExistCheckByForgetPwd", "Account", HttpMethod = "Post", ErrorMessage = "验证码不正确")]
         public string ComfrimCode{get{return comfrimCode;}set{comfrimCode = value;}}

          public void modelTrim() {
              this.UserName = this.UserName.Trim();
              this.Email = this.Email.Trim();
              this.ComfrimCode = this.ComfrimCode.Trim();
          }

          public void reSet() {
              this.UserName = "";
              this.Email = "";
              this.ComfrimCode = "";
          }
     }

     public class ForgetPwdViewModel2
     {

         private string msg = "";
         public string Msg { get { return msg; } set { msg = value; } }

         private string statu = "";
         public string Statu { get { return statu; } set { statu = value; } }

         private string userName = "";
         [Required(ErrorMessage = "请输入账号")]
         [StringLength(50, ErrorMessage = "账户不超过15个字符")]
         [Display(Name = "账户")]
         public string UserName { get { return userName; } set { userName = value; } }

         

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
             this.ComfrimCode = this.ComfrimCode.Trim();
         }

         public void reSet()
         {
             this.UserName = "";
             this.ComfrimCode = "";
         }
     }
     public class PwdUpdateViewModel {

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
          [Remote("VcodeExistCheckByForgetPwd", "Account", HttpMethod = "Post", ErrorMessage = "验证码不正确")]
         public string ComfrimCode { get; set; }

          public void reSet()
          {
             
              this.NewPwd = "";
              this.ComfrimCode = "";
          }
     }

     public class SmsRegisterModel
     { 

        /// <summary>
        /// 手机
        /// </summary>
        [StringLength(11, ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机")]
        [Required(ErrorMessage = "请输入手机号码")]
        [RegularExpression(@"^[1]+[3,5,8,7]+\d{9}", ErrorMessage = "手机号码格式不正确")]
        [Remote("MobileExistCheckByRegister", "Account", HttpMethod = "Post", ErrorMessage = "该手机号码已被注册，可用该手机号码直接登录")]
         public string RegisterMobile { get; set; }

        /// <summary>
        /// 短信验证码
        /// </summary>

        [StringLength(11, ErrorMessage = "动态密码格式不正确")]
        [Display(Name = "动态密码")]
        [Required(ErrorMessage = "请输入动态密码")]
        public string smsCode { get; set; }
        public string passWord { get; set; }

        public string RegisterIpAddress { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassId { get; set; }

        public string msg { get; set; }

        public string registerStatu { get; set; }

        public string returnUrl { get; set; }


        private string registerType = "student";
        public string RegisterType { get { return registerType; } set { registerType = value; } }

        public UserRegisterModel GetUserRegisterModel()
        {

            return new UserRegisterModel()
            {
                RegisterMobile = this.RegisterMobile,
                msg = this.msg,
                ClassId = this.ClassId,
                PassWord=this.passWord,
                RegisterIpAddress = this.RegisterIpAddress,
                registerStatu = this.registerStatu,
                returnUrl = this.returnUrl,
                SmsCode = this.smsCode,
                RegisterType = this.RegisterType
            };
        }

     }

     public class SmsForgetPwdViewModel
     {
         private string msg = "";
         public string Msg { get { return msg; } set { msg = value; } }

         private string statu = "";
         public string Statu { get { return statu; } set { statu = value; } }

         [StringLength(11, ErrorMessage = "手机号码格式不正确")]
         [Display(Name = "手机")]
         [Required(ErrorMessage = "请输入手机号码")]
         [RegularExpression(@"^[1]+[3,5,8,7]+\d{9}", ErrorMessage = "手机号码格式不正确")]
         [Remote("SmsForgetCheckMobile", "Account", HttpMethod = "Post", ErrorMessage = "手机号不存在，请重新输入")]
         public string Mobile { get; set; }


         /// <summary>
         /// 验证码
         /// </summary>
         private string comfrimCode = "";

         [Required(ErrorMessage = "请输入验证码")]
         [Remote("VcodeExistCheckByForgetPwd", "Account", HttpMethod = "Post", ErrorMessage = "验证码不正确")]
         public string ComfrimCode { get { return comfrimCode; } set { comfrimCode = value; } }

         /// <summary>
         /// 短信验证码
         /// </summary>
         public string SmsCode { get; set; }
     }

     public class SmsUpdatePwdModel
     {
         public string Statu { get; set; }

         public string Msg { get; set; }


         /// <summary>
         /// 新密码
         /// </summary>
        [Required(ErrorMessage = "请输入新的密码")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "密码不超过20个字符,最短不能低于5个字符")]
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
         [Remote("VcodeExistCheckByForgetPwd", "Account", HttpMethod = "Post", ErrorMessage = "验证码不正确")]
         public string ComfrimCode { get; set; }

         public string mobile { get; set; }
     }

    public class UserInfoUpdateModel
    {
        [UserNameFormatCheck]
        [Display(Name = "用户名")]
        [Remote("UserNameUpdateExistCheck", "Account", HttpMethod = "Post", ErrorMessage = "该用户名已被注册")]
        public string UserName { get; set; }
        [Display(Name = "真实姓名")]
        public string UserFullName { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [DataType(DataType.EmailAddress, ErrorMessage = "Email格式不正确")]
        [RegularExpression("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$", ErrorMessage = "邮箱格式不正确")]
        [Remote("EmailExistCheck", "Account", HttpMethod = "Post", ErrorMessage = "该邮箱已被注册")]
        public string Email { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [StringLength(20, MinimumLength = 5, ErrorMessage = "密码不超过20个字符,最短不能低于5个字符")]
        [Display(Name = "新密码")]
        [DataType(DataType.Password)]
        public string NewPwd { get; set; }

        /// <summary>
        /// 确认新密码
        /// </summary>
        [System.ComponentModel.DataAnnotations.Compare("NewPwd", ErrorMessage = "两次输入的密码不一致")]
        [Display(Name = "新密码")]
        [DataType(DataType.Password)]
        public string ConfirmPwd { get; set; }
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string UserGender { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string FacultyName { get; set; }
        /// <summary>
        /// 院系
        /// </summary>
        public string MajorName { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 允许编辑邮箱
        /// </summary>
        public bool AllowEditEmail { get; set; }
        public bool AllowSetAvtar { get; set; }
        public string CurrentEmail { get; set; }
        public float x1 { get; set; }
        public float x2 { get; set; }
        public float y1 { get; set; }
        public float y2 { get; set; }


        public float selectionW { get; set; }
        public float selectionH { get; set; }

        public float defaultImgLen { get; set; }


        /// <summary>
        /// 图片路径
        /// </summary>
        public string AcatarImg { get; set; }
        public Guid Id { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string operation { get; set; }
    }
}