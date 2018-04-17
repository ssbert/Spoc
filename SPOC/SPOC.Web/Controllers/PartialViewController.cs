using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SPOC.Common.Cookie;
using SPOC.Common.Pagination;
using SPOC.User;
using SPOC.User.Dto.UserInfo;
using SPOC.Web.Filters;
using SPOC.Web.Models;

namespace SPOC.Web.Controllers
{
    [UserAuthorization]
    public class PartialViewController : SPOCControllerBase
    {
        public readonly INotificationService NotificationService;

        public PartialViewController(INotificationService notificationService)
        {
            NotificationService = notificationService;
        }

        public async Task<ActionResult> Top()
        {
            int index = 0;
            index = Request.RawUrl.IndexOf("/", StringComparison.Ordinal)+1;
            int indexofB = Request.RawUrl.IndexOf("/",1, StringComparison.Ordinal);
            var menu = indexofB > 0 ? Request.RawUrl.Substring(index, indexofB - index) : Request.RawUrl.Substring(index);
            ViewBag.ActiveMenu = menu.ToLower();
            //公告页临时开放菜单栏
            if (Request.RawUrl.ToLower().Contains("/announcement"))
            { ViewBag.ActiveMenu = "announcement"; }
            var topView = new TopViewModel
            {
                NotificationList=await NotificationService.GetNotificationView(new PaginationInputDto{ pageSize = 5}),
                UserCookie = CookieHelper.GetLoginInUserInfo()
           };
            return View(topView);
        }
    }
}