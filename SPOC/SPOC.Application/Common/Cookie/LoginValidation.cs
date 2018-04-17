using System;
using System.Linq;
using Abp.Domain.Repositories;
using SPOC.User;

namespace SPOC.Common.Cookie
{
    public class LoginValidation
    {
        public static bool IsLogin()
        {
            var cookie = CookieHelper.GetLoginInUserInfo(); ;
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                return false;
            }
            return true;
        }
        public static bool IsAdmin()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie != null && cookie.IsAdmin)
            {
                return true;
            }
            return false;
        }

        public static bool IsTeacher(IRepository<TeacherInfo, Guid> iTeacherInfoRep)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null)
            {
                return false;
            }
            return iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id);
        }
    }
}