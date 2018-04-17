using System;
using System.Web;
using SPOC.User;
using SPOC.User.Dto.UserInfo;

namespace SPOC.Common.Cookie
{
    public class CookieHelper
    {
        private static bool IsLogin=true;
        /// <summary>
        /// 将User写入Cookie
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isAdminLogin">是否为后台登录</param>
        public static void SetLoginInUserCookie(object user, bool isAdminLogin = false,bool isRemberme=false)
        {
            if (user != null)
            {
                CookieOpreate.Remove("SPOC_UserInfo");
                CookieHelper.IsLogin = true;
                if (isAdminLogin)
                {
                    //CookieOpreate.Set("SPOC_UserInfo", user, DateTime.Now.AddMinutes(30));
                    CookieOpreate.Set("SPOC_UserInfo", user);//当管理员登录时，将管理员信息保存到内存中。
                }
                else
                {
                    if (isRemberme)
                    {
                        CookieOpreate.Set("SPOC_UserInfo", user, DateTime.Now.AddDays(7));//当管理员登录时，将用户信息保存到硬盘中。
                    }
                    else {
                        CookieOpreate.Set("SPOC_UserInfo", user, DateTime.Now.AddMinutes(30));//当管理员登录时，将用户信息保存到硬盘中。
                    }
                    
                }
            }
        }

        /// <summary>
        /// 获取登录的用户信息
        /// </summary>
        /// <returns></returns>
        public static UserCookie GetLoginInUserInfo()
        {

            return CookieOpreate.Get<UserCookie>("SPOC_UserInfo") ??new UserCookie();
        }

        /// <summary>
        /// 更新用户cookie信息
        /// </summary>
        /// <param name="user">user值</param>
        /// <param name="isAdminOpreate">是否为管理员登录</param>
        public static void UpdateUserCookie(object user,bool isAdminOpreate =false, bool isRemberMe=false) { 
        
           CookieOpreate.Remove("SPOC_UserInfo");
           CookieHelper.SetLoginInUserCookie(user, isAdminOpreate, isRemberMe);


            //  CookieOpreate.ModCookieByObj("SPOC_UserInfo", user);
        }

        /// <summary>
        /// 清除用户cookie
        /// </summary>
        public static void RemoveUserCookie() {
            CookieOpreate.Remove("SPOC_UserInfo");
        }

        /// <summary>
        /// 判断cookie是否过期
        /// </summary>
        /// <returns></returns>
        public static bool UserCookIsExist() 
        {
            var cookie = HttpContext.Current.Request.Cookies["SPOC_UserInfo"];
            if (cookie == null)
            {
                return false;
            }
            else {
                return true;
            }
        }

        /// <summary>
        /// cookie顺延
        /// </summary>
        public static void UserCookiePostpone(bool isAdmin=false) 
        { 
           var user=CookieHelper.GetLoginInUserInfo();
           if (user != null && !string.IsNullOrEmpty(user.UserUid)) {
               CookieHelper.UpdateUserCookie(user, isAdmin, user.IsRemberMe);
              // var users = CookieHelper.GetLoginInUserInfo();
           }
        }

        public static bool CheckIsLogin() { 
        var cookie = CookieHelper.GetLoginInUserInfo();
        if (cookie != null && (!string.IsNullOrEmpty(cookie.UserUid)))
           {
               return true;
           }
           else {
               return false;
           }
        }

        /// <summary>
        /// 检查登录
        /// </summary>
        public static void CheckLogin()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || string.IsNullOrEmpty(cookie.UserUid))
            {
                string url =  HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString());
                url = url.Replace("&", "%26");
                if (url.Contains("StudyPlatform")  && HttpContext.Current.Request.RequestContext.RouteData.Values["plat"] != null)
                {
                        HttpContext.Current.Response.Redirect("/" + HttpContext.Current.Request.RequestContext.RouteData.Values["plat"] + "/userSet/Account/Login?skipUrl=" + url);
                        HttpContext.Current.Response.End();
                }
                else {
                    HttpContext.Current.Response.Redirect("/Account/Login?skipUrl=" + url);
                    HttpContext.Current.Response.End();
                }
            }
        }
        public static void SetUserRememberCookie(string key) {

            CookieOpreate.Set("smartUFO_UserLoginRemeberKey", key, DateTime.Now.AddDays(7));
        }
        public static string GetRememberCookie()
        {
            return CookieOpreate.Get("smartUFO_UserLoginRemeberKey");
        }
        public static void RemoveRememberCookie()
        {
            if (CookieHelper.GetRememberCookie() != null) {
                CookieOpreate.Remove("smartUFO_UserLoginRemeberKey");
            }
        }
    }
}
