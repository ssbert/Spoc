using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPOC.Common.Helper;
using SPOC.Core.Dto;
using SPOC.Web.Filters;

namespace SPOC.Web.Areas.AdminHome.Controllers
{
    [UserAuthorization]
    public class AnnouncementController : Controller
    {
        // GET: AdminHome/Announcement
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddAnnouncement(string id)
        {
            var aId = id.TryParseGuid();
            var dto = new AnnouncementDto {id = aId };
            return View(dto);
        }
    }
}