using Abp.Web.Models;
using SmartUFO.EntityFramework;
using SPOC.Common.Cookie;
using SPOC.Common.Helper;
using SPOC.Core;
using SPOC.Core.Dto;
using SPOC.EntityFramework;
using SPOC.SysSetting;
using SPOC.User;
using SPOC.User.Dto.UserInfo;
using SPOC.Web.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WebGrease.Css.Extensions;

namespace SPOC.Web.Controllers
{
    [DontWrapResult]
    public class HomeController : SPOCControllerBase
    {
        private SPOCDbContext db = ContextFactory.GetCurrentContext();
        public readonly ISiteVersionService _siteVersionService;
        public readonly IUserInfoService _userInfoService;
        public readonly IMenuService _menuService;
        public readonly IAnnouncementService _announcementService;
        public HomeController(
           ISiteVersionService siteVersionService, ISiteSetService siteSetService, IUserInfoService userInfoService, IMenuService menuService, IAnnouncementService announcementService)
        {
            _siteVersionService = siteVersionService;
            _userInfoService = userInfoService;
            _menuService = menuService;
            _announcementService = announcementService;
        }
        [UserAuthorization]
        public ActionResult Index()
        {
            var myAnnouncementList = _announcementService.GetMyAnnouncement(new AnnouncementInputDto{pageSize=2});
            ViewBag.labelDeductPoint = -Convert.ToInt32(BaseSiteSetDto.labelDeductPoint);
            var cookie = CookieHelper.GetLoginInUserInfo();
            ViewBag.userName = cookie.UserName;
            ViewBag.loginName = cookie.LoginName;
            return View(myAnnouncementList);
           
        }

        [UserAuthorization]
        #region 公告 通知
        public ActionResult Notice()
        {

            return View();

        }
        [UserAuthorization]
        public ActionResult Announcement()
        {
           
            return View();

        }
        [UserAuthorization]
        public ActionResult AnnouncementDetail(string id)
        {
            var model = _announcementService.AnnouncementDetail(id.TryParseGuid());
            return View(model);

        }
        #endregion
        #region 菜单
        public JsonResult GetMenu()
        {
            var entity = _menuService.GetAllMenu();
            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTreeMenu()
        {
            var entity = _menuService.GetPermissionTree().Result;
            return Json(entity, JsonRequestBehavior.AllowGet);
        }
#endregion
        public ActionResult ConvertToAdminSystem()
        {
         
            return Redirect("/Home/Index");
        }
    
        #region 登录登出
        /// <summary>
        /// 后台登出
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOut()
        {
          
            var userSession = CookieHelper.GetLoginInUserInfo();
            _userInfoService.RemoveLoginSessionId(userSession == null ? Guid.Empty : userSession.Id, Session.SessionID);
            CookieHelper.RemoveUserCookie();

            return Redirect("/account/login");

        }

        [HttpPost]
        public string Login(LoginUser userLogin)
        {
            try
            {

                userLogin.Trim();
           
                string msg = "";
                UserCookie loginUserCookie = _userInfoService.GetUserSession(userLogin.UserName, userLogin.Password,
                    Request.UserHostAddress, ref msg);

                if (loginUserCookie != null && loginUserCookie.IsAdmin && loginUserCookie.UserEnbleFlag == false)
                {
                    Session[Session.SessionID] = loginUserCookie;
                    Session.Timeout = 20;
                    return "{\"error\":\"\"}";
                }
                else
                {

                    return "{\"error\":\"" + (string.IsNullOrWhiteSpace(msg) ? "登录密码错误或用户不存在或用户被禁用" : msg) + "\"}";
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("登录异常");

            }
            ModelState.AddModelError("_error", "登录密码错误或用户不存在或用户被禁用。");
            return "{\"error\":\"用户名或密码错误。\"}";

        }
          #endregion
        public ActionResult Error()
        {
            return View();
        }


        #region 初始化站点数据库
        public ActionResult InitDataBase()
        {
            return View();
        }

    
        public JsonResult CheckDatabase()
        {
            var success = true;
            try
            {
                var entity = (from e in db.Sites select e.Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Info("检查数据库：" + ex);
            }
            List<FileInfo> list = new List<FileInfo>();
            foreach (var item in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Data"))
            {
                list.Add(new FileInfo(item));
            }
            var item0 =
                (from e in list where e.Name == "procedure.sql" select new {e.FullName, e.Name}).FirstOrDefault();
            try
            {
                db.Database.CommandTimeout = 180;
                db.Database.ExecuteSqlCommand(System.IO.File.ReadAllText(item0.FullName, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                success = false;
                Logger.Error("脚本" + item0.Name + "：" + ex);
            }

        

            var items = from e in list
                where e.Name != "procedure.sql" && e.Name != "nv_folder.sql" && e.Name != "index.sql" &&
                      e.Extension == ".sql"
                select new {e.FullName, e.Name};

            //System.Threading.Tasks.Parallel.ForEach(items, item =>
            //{
            //    try
            //    {
            //        db.Database.ExecuteSqlCommand(System.IO.File.ReadAllText(item.FullName, Encoding.UTF8));
            //    }
            //    catch (Exception ex)
            //    {
            //        success = false;
            //        Logger.Error("脚本" + item.Name + "：" + ex);
            //    }
            //});
            foreach (var item in items)
            {
                try
                {
                    if (item.Name.Equals("city_area.sql"))
                    {
                        var sqlList = System.IO.File.ReadAllText(item.FullName, Encoding.UTF8).Trim().Split(';');
                        sqlList.ForEach(a =>
                        {
                            if (!string.IsNullOrWhiteSpace(a.Trim()))
                            {
                                db.Database.CommandTimeout = 180;
                                db.Database.ExecuteSqlCommand(a.Trim());
                                db.SaveChanges();
                            }
                        });
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand(System.IO.File.ReadAllText(item.FullName, Encoding.UTF8));

                    }


                }
                catch (Exception ex)
                {
                    success = false;
                    Logger.Error("脚本" + item.Name + "：" + ex);
                }
            }
            var item1 =
                (from e in list where e.Name == "nv_folder.sql" select new {e.FullName, e.Name}).FirstOrDefault();
            try
            {
                db.Database.CommandTimeout = 180;
                db.Database.ExecuteSqlCommand(System.IO.File.ReadAllText(item1.FullName, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                success = false;
                Logger.Error("脚本" + item1.Name + "：" + ex);
            }
            var item2 = (from e in list where e.Name == "index.sql" select new {e.FullName, e.Name}).FirstOrDefault();
            try
            {
                System.IO.File.ReadAllLines(item2.FullName, Encoding.UTF8).ForEach(a =>
                {
                    if (!string.IsNullOrWhiteSpace(a.Trim()))
                    {
                        db.Database.CommandTimeout = 180;
                        db.Database.ExecuteSqlCommand(a.Trim());
                        db.SaveChanges();
                    }
                });

            }
            catch (Exception ex)
            {
                success = false;
                Logger.Error("脚本" + item2.Name + "：" + ex);
            }
            var xml = AppDomain.CurrentDomain.BaseDirectory + "Localization\\" + SPOCConsts.LocalizationSourceName +
                      "\\" + SPOCConsts.LocalizationSourceName + ".xml";
            XDocument doc = XDocument.Load(xml);
            foreach (var item in doc.Descendants("text"))
            {
                if (item.Attribute("name").Value == "version")
                {
                    item.SetAttributeValue("value", SPOCConsts.version);
                }
                if (item.Attribute("name").Value == "IsAdminInitialize")
                {
                    item.SetAttributeValue("value", "true");
                }
            }
            try
            {
                if (success)
                {
                    doc.Save(xml);
                    SystemSet.Site site = new SystemSet.Site();
                    site.version = SPOCConsts.version;
                    _siteVersionService.Save(site);
                    HttpRuntime.UnloadAppDomain();
                }
            }
            catch (Exception ex)
            {
                success = false;
                Logger.Error("初始化数据：" + ex);
            }


            return Json(new {success = success}, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult Version()
        {
            return View();
        }

    }
}