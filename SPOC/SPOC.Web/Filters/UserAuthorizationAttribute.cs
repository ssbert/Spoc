using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SPOC.Common.Cookie;

namespace SPOC.Web.Filters
{
    public class UserAuthorizationAttribute : ActionFilterAttribute
    {

         
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
 
            var isImgUpload = filterContext.HttpContext.Request.Params["spoc_Temp"] != null && filterContext.HttpContext.Request.Params["spoc_Temp"] == "imgUpload";
            var res = CookieHelper.UserCookIsExist();

            if (!res && !isImgUpload)
            {
                //跳转到登录界面
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    //filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                    filterContext.HttpContext.Response.Write("<script>parent.location.href = '/Account/Login';</script>");
                    filterContext.HttpContext.Response.End();
                     //return;
                    //evtBus.dispatchEvt("show_login")
                }
                else
                {
                    //filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                    //  filterContext.HttpContext.Response.Write("<script>evtBus.dispatchEvt('show_login');</script>");
                    filterContext.HttpContext.Response.Write("<script>parent.location.href = '/Account/Login';</script>");
                    filterContext.HttpContext.Response.End();
                   // return;
                }
               

            }
            else
            {
                
                var cookie = CookieHelper.GetLoginInUserInfo();
                if (cookie == null && !isImgUpload)
                {
                    //filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                    filterContext.HttpContext.Response.Write("<script>parent.location.href = '/Account/Login';</script>");
                    filterContext.HttpContext.Response.End();
                   // return;
                    //filterContext.Result = new RedirectResult(ManageLoginUrl);
                }
               
                
            }

            base.OnActionExecuting(filterContext);
        }
    }


    public class ClientUserAuthorizationAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CookieHelper.CheckLogin();
            base.OnActionExecuting(filterContext);
        }
    }
     public class AllowNologinAccessAttribute : ActionFilterAttribute
    {
       
         
         public override void OnActionExecuting(ActionExecutingContext filterContext) {

             var _allowNologinAccess = "false";
              string url = HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.ToString());
                url = url.Replace("&", "%26");
                if (!url.Contains("Account") && !url.ToLower().Contains("home/error"))
                {
                    if (_allowNologinAccess == "false") { 
                    filterContext.HttpContext.Response.Redirect("/Account/Login");
                    }
                }
             base.OnActionExecuting(filterContext);
        } 
    }  
    public class SiteSetAttribute : ActionFilterAttribute {
        public string _UserForRegisterDispaly { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_UserForRegisterDispaly == null) { _UserForRegisterDispaly = ""; };
            var actionName = filterContext.HttpContext.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
            switch (actionName) {

                case "register": {
                    if (_UserForRegisterDispaly.ToLower() == "false")
                    {
                        filterContext.HttpContext.Response.Write("暂不开放注册");
                        filterContext.HttpContext.Response.End();
                    } 
                } break;
      
            }
            base.OnActionExecuting(filterContext);
        }
    }
}