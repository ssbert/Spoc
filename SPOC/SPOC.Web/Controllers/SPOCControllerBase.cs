using System;
using System.Web.Mvc;
using System.Web.Routing;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Web.Mvc.Controllers;
using SPOC.Common.Cookie;
using SPOC.Common.Tools;
using SPOC.SysSetting;
using SPOC.SysSetting.SiteSetDTO;
using SPOC.User;
using SPOC.User.Dto.UserInfo;

namespace SPOC.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class SPOCControllerBase : AbpController
    {
        protected SPOCControllerBase()
        {
            LocalizationSourceName = SPOCConsts.LocalizationSourceName;
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {


            //系统设置用户设置
            if (Request.Url != null )
            {

                var site =BaseSiteSetDto;
                var user = CookieHelper.GetLoginInUserInfo();
                if (string.IsNullOrEmpty(user?.UserUid) && (!Request.Url.AbsoluteUri.ToLower().Contains("account") && !Request.Url.AbsoluteUri.ToLower().Contains("home")))//当没有登录时且不允许游客访问时，跳转到登录页。
                {
                    Response.Write("<script>parent.location.href='/Account/Login';</script>");
                    //filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                }
                if (site.loginLimit == "open")//当用户登录限制时(只能在一处登录,不能在两处同时登录，可以在一个浏览器中同时登录)
                {
                    if (!string.IsNullOrEmpty(user?.UserUid))
                    {
                        //从容器获取实现业务类
                        var lpset = IocManager.Instance.Resolve<ISiteSetService>();
                        var checkUser = lpset.GetUserSessionId(user.Id) ?? new UserBase();
                        if (!string.IsNullOrEmpty(checkUser.sessionId) && checkUser.sessionId != Session.SessionID && checkUser.loginTime.HasValue)
                        {
                            if (DateTime.Now > checkUser.loginTime.Value.AddMinutes(30))//如果当前账户在其他地方登陆且过了有效期，则不需要重新登录。
                            {
                                checkUser.sessionId = Session.SessionID;
                                checkUser.loginTime = DateTime.Now;
                                checkUser.loginIp = Request.UserHostAddress;
                            }
                            else
                            {
                                //CookieHelper.RemoveUserCookie();
                                Response.Write("<script>alert('您的账号已在其他地方登陆，请重新登陆！');location.href='/Account/Login?otherLogin=true';</script>");

                            }
                        }
                    }
                }
                CookieHelper.UserCookiePostpone();//检查cookie是否从存在，若存在，顺延cookie
            }

            base.OnActionExecuting(filterContext);
        }
   
        public SiteSetDto BaseSiteSetDto
        {
            get
            {
                if (CacheStrategy.Get("SiteSet") != null)
                {
                    return (SiteSetDto)CacheStrategy.Get("SiteSet") ?? BaseSiteSetDto;
                }
                //从容器获取实现业务类
                var lpset = IocManager.Instance.Resolve<ISiteSetService>();
                var site = lpset.GetAllSiteSet();
                CacheStrategy.Remove("SiteSet");
                CacheStrategy.Insert("SiteSet", site, 3600);
                return site;
            }
        }
    }
}