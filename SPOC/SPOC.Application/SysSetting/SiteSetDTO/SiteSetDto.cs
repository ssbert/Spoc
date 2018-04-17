namespace SPOC.SysSetting.SiteSetDTO
{
    public class SiteSetDto
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public string siteName { get; set; }

        /// <summary>
        /// 网站副标题
        /// </summary>
        public string siteSlogan { get; set; }

        /// <summary>
        /// 网站的url
        /// </summary>
        public string siteUrl { get; set; }

        /// <summary>
        /// 网站的logo
        /// </summary>
        public string siteLogo { get; set; }

        /// <summary>
        /// 网站浏览器图标
        /// </summary>
        public string siteFavicon { get; set; }

        /// <summary>
        /// 网站seo关键字
        /// </summary>
        public string siteSeoKeyWords { get; set; }

        /// <summary>
        /// 网站seo关键字描述
        /// </summary>
        public string siteSeoKeyWordsDescription { get; set; }

        /// <summary>
        /// 网站管理员邮箱
        /// </summary>
        public string siteMasterEmail { get; set; }

        /// <summary>
        /// 网站版权方
        /// </summary>
        public string siteCopyright { get; set; }

        /// <summary>
        /// 网站的备案号
        /// </summary>
        public string siteIcp { get; set; }

        /// <summary>
        /// 网站的QQ
        /// </summary>
        public string siteQQ { get; set; }

        /// <summary>
        /// 统计分析代码
        /// </summary>
        public string siteAnalytics { get; set; }

        /// <summary>
        /// 是否显示学生注册
        /// </summary>
        public string userRegisterDispaly { get; set; }

        /// <summary>
        /// 注册后发邮件显示的标题
        /// </summary>
        public string registerEmailActivationTitle { get; set; }

        /// <summary>
        /// 注册后发邮件显示的文字
        /// </summary>
        public string registerEmailActivationBody { get; set; }

        /// <summary>
        /// 是否发送欢迎信息
        /// </summary>
        public string registerWelcomeEnabled { get; set; }

        /// <summary>
        /// 欢迎信息发送方
        /// </summary>
        public string registerWelcomeSender { get; set; }

        /// <summary>
        /// 欢迎信息标题
        /// </summary>
        public string registerWelcomeTitle { get; set; }

        /// <summary>
        /// 欢迎信息内容
        /// </summary>
        public string registerWelcomeBody { get; set; }
        
        /// <summary>
        /// 允许教师设置课程价格
        /// </summary>
        public string courseTeacherModifyPrice { get; set; }

        /// <summary>
        /// 购买课程时填写个人资料
        /// </summary>
        public string courseBuyFillUserInfo { get; set; }

        /// <summary>
        /// 邀请码是否必填
        /// </summary>
        public string invitationCodeNeeded { get; set; }

        /// <summary>
        /// 允许学生显示邀请码
        /// </summary>
        public string invitationCodeAllowedShow { get; set; }

        /// <summary>
        /// 支持邀请码
        /// </summary>
        public string invitationCodeSupport { get; set; }

        /// <summary>
        /// 是否开启微博登录
        /// </summary>
        public string loginWeiboEnabled { get; set; }

        /// <summary>
        /// 微博app key
        /// </summary>
        public string loginWeiboKey { get; set; }

        /// <summary>
        /// 微博app secret
        /// </summary>
        public string loginWeiboSecret { get; set; }

        /// <summary>
        /// 是否开启微博登录
        /// </summary>
        public string loginQQEnabled { get; set; }

        /// <summary>
        /// QQ app key
        /// </summary>
        public string loginQQKey { get; set; }

        /// <summary>
        /// QQ app secret
        /// </summary>
        public string loginQQSecret { get; set; }

        /// <summary>
        /// 是否开启人人登录
        /// </summary>
        public string loginRenrenEnabled { get; set; }

        /// <summary>
        /// 人人  app secret
        /// </summary>
        public string loginRenrenSecret { get; set; }

        /// <summary>
        /// 人人 app key
        /// </summary>
        public string loginRenrenKey { get; set; }

        /// <summary>
        /// 登录接口验证码
        /// </summary>
        public string loginVerifyCode { get; set; }

        /// <summary>
        /// 是否开启用户名一定要英文+数字
        /// </summary>
        public string usernameContainEN { get; set; }

        /// <summary>
        /// 是否开启邮件发送
        /// </summary>
        public string mailerEnabled { get; set; }

        /// <summary>
        /// 是否开启注册报名
        /// </summary>
        public string userForRegisterDispaly { get; set; }

        /// <summary>
        /// 注册是否需要审批
        /// </summary>
        public string userRegisterIsapprove { get; set; }

        /// <summary>
        /// 允许编辑头像
        /// </summary>
        public string allowEditAvatarPicture { get; set; }

        /// <summary>
        /// 首次登录完善用户信息
        /// </summary>
        public string firstLoginPerfectUserData { get; set; }

        /// <summary>
        /// 允许未登录用户访问
        /// </summary>
        public string allowNologinAccess { get; set; }

        /// <summary>
        /// 用户登录限制
        /// </summary>
        public string loginLimit { get; set; }

        /// <summary>
        /// 第三方登录
        /// </summary>
        public string loginEnabled { get; set; }

        /// <summary>
        /// 用户注册协议
        /// </summary>
        public string registerUserTemrs { get; set; }

        /// <summary>
        /// 是否允许发送邮件
        /// </summary>
        public string isSendEmail { get; set; }
        

         /// <summary>
        /// SMTP服务器地址
        /// </summary>
        public string mailer_host { get; set; }

        /// <summary>
        /// SMTP端口号
        /// </summary>
        public string mailer_hostPort { get; set; }

        /// <summary>
        /// SMTP用户名
        /// </summary>
        public string mailer_username { get; set; }

        /// <summary>
        /// SMTP密码
        /// </summary>
        public string mailer_password { get; set; }

        /// <summary>
        ///发信人地址
        /// </summary>
        public string mailer_from { get; set; }

        /// <summary>
        ///发信人名称
        /// </summary>
        public string mailer_name { get; set; }

        /// <summary>
        /// 本站登录
        /// </summary>
        public string siteLogin { get; set; }

        /// <summary>
        /// 允许拷贝粘贴代码
        /// </summary>
        public string allowPasteCode { get; set; }
        /// <summary>
        /// 标签得分点
        /// </summary>

        public string labelPoint { get; set; }
        /// <summary>
        /// 标签扣分点
        /// </summary>
        public string labelDeductPoint { get; set; }
        /// <summary>
        /// 得分点比率上限
        /// </summary>
        public string maxPointRate { get; set; }


    }
}
