using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using SPOC.Common.Helper;
using SPOC.SysSetting.SiteSetDTO;
using SPOC.SystemSet;
using SPOC.User;

namespace SPOC.SysSetting
{
    public class SiteSetService : ApplicationService, ISiteSetService
    {

        private readonly IRepository<SiteSet, Guid> _iSiteSetRepository;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;
        private readonly IRepository<UserBase, Guid> _userRepository;
      
        public SiteSetService(IRepository<SiteSet, Guid> iSiteSetRepository, IUnitOfWorkManager iUnitOfWorkManager, IRepository<UserBase, Guid> userRepository)
        {
            _iSiteSetRepository = iSiteSetRepository;
            _iUnitOfWorkManager = iUnitOfWorkManager;
            _userRepository = userRepository;
           
        }

        public UserBase GetUserSessionId(Guid userId)
        {
            return _userRepository.Get(userId);
        }

        /// <summary>
        /// 获取所有的站点设置信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public SiteSetDto GetAllSiteSet()
        {
            SiteSetDto dto = new SiteSetDto();
            try
            {
                var siteSetList = _iSiteSetRepository.GetAllList();

                if (!siteSetList.Any())
                {
                    return dto;
                }
                dto = GetSiteDtoByTb(siteSetList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return dto;
        }

        /// <summary>
        /// 修改站点配置信息
        /// </summary>
        /// <param name="input"></param>
        public void ModifySiteSet(List<SiteSetInputDto> input)
        {
            try
            {
                var data = _iSiteSetRepository.GetAll();
                //foreach (var item in data)
                //{
                //    SiteSetInputDto inputDto = input.SingleOrDefault(d => d.setKey == item.settingKey);
                //    if (inputDto != null)
                //    {
                //        item.settingValue = inputDto.setValue;
                //    }
                //    else
                //    {
                //        InsertSiteSet(input);
                //    }
                //}

                foreach (var item in input)
                {
                    if (item.setKey == "invitation_code_support")
                    {
                        var a = "1";
                    }
                    var inputDto = data.SingleOrDefault(d => d.settingKey == item.setKey);
                    if (inputDto != null)
                    {

                        inputDto.settingValue = item.setValue;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.setKey))
                        {
                            InsertSiteSet(item);
                        }

                    }

                }
                _iUnitOfWorkManager.Current.SaveChanges();

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }



        private SiteSetDto GetSiteDtoByTb(List<SiteSet> siteSetList)
        {
            SiteSetDto dto = new SiteSetDto();
            try
            {
                if (siteSetList == null)
                {
                    return dto;
                }

                foreach (SiteSet item in siteSetList)
                {
                    switch (item.settingKey)
                    {
                        case "site_name":
                            dto.siteName = item.settingValue??"";
                            break;
                        case "site_slogan":
                            dto.siteSlogan = item.settingValue ?? "";
                            break;
                        case "site_url":
                            dto.siteUrl = item.settingValue ?? "";
                            break;
                        case "site_logo":
                            dto.siteLogo = item.settingValue ?? "";
                            break;
                        case "site_favicon":
                            dto.siteFavicon = item.settingValue ?? "";
                            break;
                        case "site_seo_keywords":
                            dto.siteSeoKeyWords = item.settingValue ?? "";
                            break;
                        case "site_seo_description":
                            dto.siteSeoKeyWordsDescription = item.settingValue ?? "";
                            break;
                        case "site_master_email":
                            dto.siteMasterEmail = item.settingValue ?? "";
                            break;
                        case "site_copyright":
                            dto.siteCopyright = item.settingValue ?? "";
                            break;
                        case "site_icp":
                            dto.siteIcp = item.settingValue ?? "";
                            break;
                        case "site_qq":
                            dto.siteQQ = item.settingValue ?? "";
                            break;
                        case "site_analytics":
                            dto.siteAnalytics = item.settingValue ?? "";
                            break;
                        case "user_register_dispaly":
                            dto.userRegisterDispaly = item.settingValue ?? "";
                            break;
                        case "register_email_activation_title":
                            dto.registerEmailActivationTitle = item.settingValue ?? "";
                            break;
                        case "register_email_activation_body":
                            dto.registerEmailActivationBody = item.settingValue ?? "";
                            break;
                        case "register_welcome_enabled":
                            dto.registerWelcomeEnabled = item.settingValue ?? "";
                            break;
                        case "register_welcome_sender":
                            dto.registerWelcomeSender = item.settingValue ?? "";
                            break;
                        case "register_welcome_title":
                            dto.registerWelcomeTitle = item.settingValue ?? "";
                            break;
                        case "register_welcome_body":
                            dto.registerWelcomeBody = item.settingValue ?? "";
                            break;
                        case "course_teacher_modify_price":
                            dto.courseTeacherModifyPrice = item.settingValue ?? "";
                            break;
                        case "course_buy_fill_userinfo":
                            dto.courseBuyFillUserInfo = item.settingValue ?? "";
                            break;
                        case "invitation_code_needed":
                            dto.invitationCodeNeeded = item.settingValue ?? "";
                            break;
                        case "invitation_code_allowed_show":
                            dto.invitationCodeAllowedShow = item.settingValue ?? "";
                            break;
                        case "invitation_code_support":
                            dto.invitationCodeSupport = item.settingValue ?? "";
                            break;
                        case "login_weibo_enabled":
                            dto.loginWeiboEnabled = item.settingValue ?? "";
                            break;
                        case "login_weibo_key":
                            dto.loginWeiboKey = item.settingValue ?? "";
                            break;
                        case "login_weibo_secret":
                            dto.loginWeiboSecret = item.settingValue ?? "";
                            break;
                        case "login_qq_enabled":
                            dto.loginQQEnabled = item.settingValue ?? "";
                            break;
                        case "login_qq_key":
                            dto.loginQQKey = item.settingValue ?? "";
                            break;
                        case "login_qq_secret":
                            dto.loginQQSecret = item.settingValue ?? "";
                            break;
                        case "login_renren_key":
                            dto.loginRenrenKey = item.settingValue ?? "";
                            break;
                        case "login_renren_secret":
                            dto.loginRenrenSecret = item.settingValue ?? "";
                            break;
                        case "login_verify_code":
                            dto.loginVerifyCode = item.settingValue ?? "";
                            break;
                        case "username_contain_EN":
                            dto.usernameContainEN = item.settingValue ?? "";
                            break;
                        case "mailer_enabled":
                            dto.mailerEnabled = item.settingValue ?? "";
                            break;
                        case "user_for_register_dispaly":
                            dto.userForRegisterDispaly = item.settingValue ?? "";
                            break;
                        case "user_register_isapprove":
                            dto.userRegisterIsapprove = item.settingValue ?? "";
                            break;
                        case "allow_edit_avatar_picture":
                            dto.allowEditAvatarPicture = item.settingValue ?? "";
                            break;
                        case "first_login_perfect_userdata":
                            dto.firstLoginPerfectUserData = item.settingValue ?? "";
                            break;
                        case "allow_nologin_access":
                            dto.allowNologinAccess = item.settingValue ?? "";
                            break;
                        case "login_limit":
                            dto.loginLimit = item.settingValue ?? "";
                            break;
                        case "login_enabled":
                            dto.loginEnabled = item.settingValue ?? "";
                            break;
                        case "register_userterms":
                            dto.registerUserTemrs = item.settingValue ?? "";
                            break;
                        case "is_send_email":
                            dto.isSendEmail = item.settingValue ?? "";
                            break;
                        case "mailer_host":
                            dto.mailer_host = item.settingValue ?? "";
                            break;
                        case "mailer_hostPort":
                            dto.mailer_hostPort = item.settingValue ?? "";
                            break;
                        case "mailer_username":
                            dto.mailer_username = item.settingValue ?? "";
                            break;
                        case "mailer_password":
                            dto.mailer_password = item.settingValue ?? "";
                            break;
                        case "mailer_from":
                            dto.mailer_from = item.settingValue ?? "";
                            break;
                        case "mailer_name":
                            dto.mailer_name = item.settingValue ?? "";
                            break;
                        case "site_Login":
                            dto.siteLogin = item.settingValue ?? "";
                            break;
                        case "allow_paste_code":
                            dto.allowPasteCode = item.settingValue ?? "";
                            break;
                        case "lable_point":
                            dto.labelPoint = item.settingValue ?? "";
                            break;
                        case "lable_deduct_point":
                            dto.labelDeductPoint = item.settingValue ?? "";
                            break;
                        case "max_point_rate":
                            dto.maxPointRate = item.settingValue ?? "50";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return dto;
        }


        public void InsertSiteSet(SiteSetInputDto input)
        {
            try
            {
                SiteSet inputDto = new SiteSet();
                inputDto.Id = Guid.NewGuid();
                inputDto.modifyTime = DateTime.Now;
                inputDto.settingGroup = "sys_site";
                inputDto.settingKey = input.setKey;
                inputDto.settingValue = input.setValue;

                switch (input.setKey)
                {
                    case "site_name":
                        inputDto.settingName = "网站名称";
                        inputDto.settingRemark = "网站名称";
                        break;
                    case "site_slogan":
                        inputDto.settingName = "网站副标题";
                        inputDto.settingRemark = "网站副标题";
                        break;
                    case "site_url":
                        inputDto.settingName = "网站的url";
                        inputDto.settingRemark = "网站的url";
                        break;
                    case "site_logo":
                        inputDto.settingName = "网站的logo";
                        inputDto.settingRemark = "网站的logo";
                        break;
                    case "site_favicon":
                        inputDto.settingName = "网站浏览器图标";
                        inputDto.settingRemark = "网站浏览器图标";
                        break;
                    case "site_seo_keywords":
                        inputDto.settingName = "网站seo关键字";
                        inputDto.settingRemark = "网站seo关键字";
                        break;
                    case "site_seo_description":
                        inputDto.settingName = "网站seo关键字描述";
                        inputDto.settingRemark = "网站seo关键字描述";
                        break;
                    case "site_master_email":
                        inputDto.settingName = "网站管理员邮箱";
                        inputDto.settingRemark = "网站管理员邮箱";
                        break;
                    case "site_copyright":
                        inputDto.settingName = "网站版权方";
                        inputDto.settingRemark = "网站版权方";
                        break;
                    case "site_icp":
                        inputDto.settingName = "网站的备案号";
                        inputDto.settingRemark = "网站的备案号";
                        break;
                    case "site_qq":
                        inputDto.settingName = "网站的QQ";
                        inputDto.settingRemark = "网站的QQ";
                        break;
                    case "site_analytics":
                        inputDto.settingName = "统计分析代码";
                        inputDto.settingRemark = "统计分析代码";
                        break;
                    case "user_register_dispaly":
                        inputDto.settingName = "是否显示学生注册";
                        inputDto.settingRemark = "是否显示学生注册";
                        if (input.setValue == "true")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "register_email_activation_title":
                        inputDto.settingName = "注册后发邮件显示的标题";
                        inputDto.settingRemark = "注册后发邮件显示的标题";
                        break;
                    case "register_email_activation_body":
                        inputDto.settingName = "注册后发邮件显示的文字";
                        inputDto.settingRemark = "注册后发邮件显示的文字";
                        break;
                    case "register_welcome_enabled":
                        inputDto.settingName = "是否发送欢迎信息";
                        inputDto.settingRemark = "是否发送欢迎信息";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "register_welcome_sender":
                        inputDto.settingName = "欢迎信息发送方";
                        inputDto.settingRemark = "欢迎信息发送方";
                        break;
                    case "register_welcome_title":
                        inputDto.settingName = "欢迎信息标题";
                        inputDto.settingRemark = "欢迎信息标题";
                        break;
                    case "register_welcome_body":
                        inputDto.settingName = "欢迎信息内容";
                        inputDto.settingRemark = "欢迎信息内容";
                        break;
                    case "course_teacher_modify_price":
                        inputDto.settingName = "允许教师设置课程价格";
                        inputDto.settingRemark = "允许教师设置课程价格";
                        break;
                    case "course_buy_fill_userinfo":
                        inputDto.settingName = "购买课程时填写个人资料";
                        inputDto.settingRemark = "购买课程时填写个人资料";
                        break;
                    case "invitation_code_needed":
                        inputDto.settingName = "邀请码是否必填";
                        inputDto.settingRemark = "邀请码是否必填";
                        if (input.setValue == "true")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "invitation_code_allowed_show":
                        inputDto.settingName = "允许学生显示邀请码";
                        inputDto.settingRemark = "允许学生显示邀请码";
                        if (input.setValue == "true")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "invitation_code_support":
                        inputDto.settingName = "支持邀请码";
                        inputDto.settingRemark = "支持邀请码";
                        if (input.setValue == "true")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "login_weibo_enabled":
                        inputDto.settingName = "是否开启微博登录";
                        inputDto.settingRemark = "是否开启微博登录";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "login_weibo_key":
                        inputDto.settingName = "微博app key";
                        inputDto.settingRemark = "微博app key";
                        break;
                    case "login_weibo_secret":
                        inputDto.settingName = "微博app secret";
                        inputDto.settingRemark = "微博app secret";
                        break;
                    case "login_qq_enabled":
                        inputDto.settingName = "是否开启QQ登录";
                        inputDto.settingRemark = "是否开启QQ登录";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "login_qq_key":
                        inputDto.settingName = "QQ app key";
                        inputDto.settingRemark = "QQ app key";
                        break;
                    case "login_qq_secret":
                        inputDto.settingName = "QQ app secret";
                        inputDto.settingRemark = "QQ app secret";
                        break;
                    case "login_renren_enabled":
                        inputDto.settingName = "是否开启人人登录";
                        inputDto.settingRemark = "是否开启人人登录";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "closed";
                        }
                        break;
                    case "login_renren_key":
                        inputDto.settingName = "人人 app key";
                        inputDto.settingRemark = "人人 app key";
                        break;
                    case "login_renren_secret":
                        inputDto.settingName = "人人  app secret";
                        inputDto.settingRemark = "人人  app secret";
                        break;
                    case "login_verify_code":
                        inputDto.settingName = "登录接口验证码";
                        inputDto.settingRemark = "登录接口验证码";
                        break;
                    case "username_contain_EN":
                        inputDto.settingName = "是否开启用户名一定要英文+数字";
                        inputDto.settingRemark = "是否开启用户名一定要英文+数字";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "mailer_enabled":
                        inputDto.settingName = "是否开启邮件发送";
                        inputDto.settingRemark = "是否开启邮件发送";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "user_for_register_dispaly":
                        inputDto.settingName = "是否开启注册报名";
                        inputDto.settingRemark = "是否开启注册报名";
                        if (input.setValue == "true")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "user_register_isapprove":
                        inputDto.settingName = "注册是否需要审批";
                        inputDto.settingRemark = "注册是否需要审批";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "allow_edit_avatar_picture":
                        inputDto.settingName = "允许编辑头像";
                        inputDto.settingRemark = "允许编辑头像";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "first_login_perfect_userdata":
                        inputDto.settingName = "首次登录完善用户信息";
                        inputDto.settingRemark = "首次登录完善用户信息";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "allow_nologin_access":
                        inputDto.settingName = "允许未登录用户访问";
                        inputDto.settingRemark = "允许未登录用户访问";
                        if (input.setValue == "true")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "login_limit":
                        inputDto.settingName = "用户登录限制";
                        inputDto.settingRemark = "用户登录限制";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "login_enabled":
                        inputDto.settingName = "第三方登录";
                        inputDto.settingRemark = "第三方登录";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;

                    case "register_userterms":
                        inputDto.settingName = "用户注册协议";
                        inputDto.settingRemark = "用户注册协议";
                        break;
                    case "isSendEmail":
                        inputDto.settingName = "是否允许发送邮件";
                        inputDto.settingRemark = "是否允许发送邮件";
                        inputDto.settingGroup = "sys_mailer";
                        if (input.setValue == "open")
                        {
                            inputDto.isVisible = "Y";
                        }
                        else
                        {
                            inputDto.isVisible = "F";
                        }
                        break;
                    case "mailer_host":
                        inputDto.settingName = "SMTP服务器地址";
                        inputDto.settingRemark = "SMTP服务器地址";
                        inputDto.settingGroup = "sys_mailer";
                        break;
                    case "mailer_hostPort":
                        inputDto.settingName = "SMTP端口号";
                        inputDto.settingRemark = "SMTP端口号";
                        inputDto.settingGroup = "sys_mailer";
                        break;
                    case "mailer_username":
                        inputDto.settingName = "SMTP用户名";
                        inputDto.settingRemark = "SMTP用户名";
                        inputDto.settingGroup = "sys_mailer";
                        break;
                    case "mailer_password":
                        inputDto.settingName = "SMTP密码";
                        inputDto.settingRemark = "SMTP密码";
                        inputDto.settingGroup = "sys_mailer";
                        break;
                    case "mailer_from":
                        inputDto.settingName = "发信人地址";
                        inputDto.settingRemark = "发信人地址";
                        inputDto.settingGroup = "sys_mailer";
                        break;
                    case "mailer_name":
                        inputDto.settingName = "发信人名称";
                        inputDto.settingRemark = "发信人名称";
                        inputDto.settingGroup = "sys_mailer";
                        break;
                    default:

                        break;
                }
                _iSiteSetRepository.Insert(inputDto);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

      
    }
}
