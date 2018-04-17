using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPOC.User.Dto.Notification;
using SPOC.User.Dto.UserInfo;

namespace SPOC.Web.Models
{
    public class TopViewModel
    {
        /// <summary>
        /// 我的通知列表
        /// </summary>
        public NotificationView NotificationList { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserCookie UserCookie { get; set; }
    }
}