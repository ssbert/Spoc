using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Abp.UI;
using Abp.Web.Models;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.File;
using SPOC.SysSetting;
using SPOC.User;
using SPOC.User.Dto.Common;
using SPOC.User.Dto.StudentInfo;
using SPOC.User.Dto.Teacher;
using SPOC.Web.Controllers;

namespace SPOC.Web.Areas.User.Controllers
{
    [Filters.UserAuthorization]
    [DontWrapResult]
    public class TeacherController : SPOCControllerBase
    {
        public static List<TeacherInfoDto> teacherList = new List<TeacherInfoDto>();
        private readonly ITeacherInfoService _teacherGuidService;
        private readonly IUserInfoService _userInfoService;
        private readonly IUserInfoApiService _iUserInfoApiService;
        // GET: User/Teacher

        public TeacherController(ITeacherInfoService teacherGuidService, IUserInfoService userInfoService,IUserInfoApiService iUserInfoApiService)
        {
            _userInfoService = userInfoService;
            _teacherGuidService = teacherGuidService;
            _iUserInfoApiService = iUserInfoApiService;
        }
        public ActionResult Index()
        {
            ViewBag.usernameContainEN =BaseSiteSetDto.usernameContainEN;
            return View();
        }
        public ActionResult Selector()
        {
            return View();
        }
        public JsonResult Get()
        {

          

            try
            {
                var provider = new EasyUIProvider();
                var pager = provider.GetPager(Request);
                var filter = new TeacherInfoInputDto();
                if (!string.IsNullOrEmpty(pager.Filter))
                {
                    filter = provider.DeserializeObject<TeacherInfoInputDto>(pager.Filter);
                }
                filter.PageSize = pager.PageSize;
                filter.CurrentPage = pager.CurrentPage - 1;
                filter.SortOrder = pager.SortOrder;
                filter.SortCloumnName = pager.SortCloumnName;
                var grid = _teacherGuidService.GetTeacherInfoByGuid(filter);
                teacherList = grid.rows.ToList();
                return Json(grid);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public JsonResult TeacherNameCheck(string type, string checkname, string oldName = "")
        {
            return Json(_teacherGuidService.CheckNameExit(checkname, type, oldName), JsonRequestBehavior.AllowGet);

        }


        public ActionResult TeacherEdit(string id)
        {
            ViewBag.isAdmin =  CookieHelper.GetLoginInUserInfo().IsAdmin;
            ViewBag.id = string.IsNullOrEmpty(id) ? "" : id;
            ViewBag.usernameContainEN = BaseSiteSetDto.usernameContainEN;
            return View();
        }


        public JsonResult GetTeacher(string id)
        {
            var gid = string.IsNullOrEmpty(id) ? Guid.Empty : new Guid(id);
            TeacherInfoDto res = new TeacherInfoDto();
            //if (teacherList == null || teacherList.Count == 0)
            //{
            res = _teacherGuidService.GetTeacherInfoDtoBuUserId(new UserInfoQueryInputDto() { id = gid });
            //}
            //else {
            //   res = teacherList.Where(a => a.id == gid).FirstOrDefault();
            //  }

           
            return Json(res);
        }

        public JsonResult TeacherResumeImgUpload()
        {
            HttpPostedFileBase postFile = Request.Files["imgFile"];
            string srcVirtualpath = GetFilePath(postFile, "Teacher\\TeacherResume");
            string srcPath = Server.MapPath(srcVirtualpath);
            postFile.SaveAs(srcPath);
            Hashtable hash = new Hashtable();
            hash["error"] = 0;
            hash["url"] = srcVirtualpath;
            return Json(hash, "text/html;charset=UTF-8"); ;
        }
        public string GetFilePath(HttpPostedFileBase file, string dirName)
        {
            string path = string.Empty;
            try
            {
                string rootPath = System.Web.HttpContext.Current.Server.MapPath("~/");
                string tempDir = Path.Combine(rootPath, "files", dirName);
                if (!System.IO.Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                if (file != null)
                {
                    string name = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(tempDir, name);

                    string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
                    string fileUrl = filePath.Replace(tmpRootDir, ""); //转换成相对路径
                    fileUrl = fileUrl.Replace(@"\", @"/");
                    path = "/" + fileUrl;
                }
            }
            catch (Exception ex)
            {
                // _iLogger.Error(ex.ToString());
            }
            return path;
        }

        public FileResult UserInfoExportTemplate(string id)
        {
            return File(_teacherGuidService.GetTeacherInfoExportTemplate(id), "application/vnd.ms-excel", Server.UrlDecode("教师信息导入模板") + ".xls");
            //return File(_studentGuidService.GetStudentInfoExportTemplate("登录名,密码"), "application/vnd.ms-excel", Server.UrlDecode("学生信息导入模板") + ".xls");
        }

        /// <summary>
        /// 从Excel汇入学生列表
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public ActionResult CreateTeacherFromFile(HttpPostedFileBase file)
        {
            try
            {
                var index = file.FileName.LastIndexOf('.');
                var extName = file.FileName.Substring(index + 1);
                ImportResultOutputDto result = new ImportResultOutputDto();
                if (extName == "xls" || extName == "xlsx")
                {
                    result = _teacherGuidService.CreateTeacherInfoFromFile(file.InputStream);
                }
                return Json(new { Success = true, Result = result }, JsonRequestBehavior.AllowGet);
            }
            catch (UserFriendlyException e)
            {
                return Json(new { Success = false, Error = new { e.Message } }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                file.InputStream.Close();
                file.InputStream.Dispose();
            }
        }

        public void GetExportDataParms()
        {
            Session.Remove("ExportDataFilter");
            Session.Remove("ExportDataIdList");
            var filter = Request.Params["filter"];
            Session["ExportDataFilter"] = filter;
            var idList = Request.Params["idList"];
            Session["ExportDataIdList"] = idList;

        }

        [System.Web.Mvc.ActionName("exportuserinfo")]
        public FileResult ExportData(UserInfoSeacheExport userInfoModel)
        {
            //var filter = Request.Params["filter"];
            var filterSession = Session["ExportDataFilter"];
            var filter = filterSession == null ? "" : filterSession.ToString();
            var input = new TeacherInfoInputDto();

            JavaScriptSerializer json = new JavaScriptSerializer();
            if (filter != null)
            {
                input = json.Deserialize<TeacherInfoInputDto>(filter);
            }
            List<BatchDeleteRequestInputByUser> inputList = new List<BatchDeleteRequestInputByUser>();
            //var idList = Request.Params["idList"];
            var idListSession = Session["ExportDataIdList"];
            var idList = idListSession == null ? "" : idListSession.ToString();
            if (!string.IsNullOrEmpty(idList))
            {
                inputList = json.Deserialize<List<BatchDeleteRequestInputByUser>>(idList);
            }

            List<string> keyUserCode = new List<string>();
            List<string> keyUserName = new List<string>();


            var listConstraint = _teacherGuidService.GetTeacherInfoMap();
            foreach (var userConstraintList in listConstraint)
            {
                keyUserCode.Add(userConstraintList.FieldCode);
                keyUserName.Add(userConstraintList.FieldName);
            }
            ExcelImportExport excel = new ExcelImportExport();
            var book = excel.GenerateTeacherInfoData("教师", "教师", keyUserCode.ToArray(), keyUserName.ToArray(), userInfoModel, inputList, input, _userInfoService, _teacherGuidService);
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "教师.xls");
        }


        /// <summary>
        /// 恢复
        /// </summary>
        /// <returns></returns>
        public ActionResult Recovery()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            try
            {
                string userName = Request.Params["user_login_name"].ToString().Trim();
                string mobile = Request.Params["user_mobile"].ToString().Trim();
                string email = Request.Params["user_email"].ToString().Trim();
                var result = _iUserInfoApiService.UserSearch(null, userName, email, mobile);
                if (result.Result.IsSuccess)
                {
                    UserBase model = new UserBase();
                    model.userLoginName = result.Result.Data.InKey.loginname;
                    model.userFullName = result.Result.Data.InKey.truename;
                    model.userMobile = result.Result.Data.InKey.mobile;
                    model.userEmail = result.Result.Data.InKey.email;
                    model.newMoocUserId = string.IsNullOrEmpty(result.Result.Data.InKey.id) ? Guid.Empty : Guid.Parse(result.Result.Data.InKey.id);
                    model.userGender = result.Result.Data.InKey.gender;
                    model.userPassWord = "6ED5833CF35286EBF8662B7B5949F0D742BBEC3F";
                    model.Id = Guid.NewGuid();
                    model.identity = 2;
                    model.userEnbleFlag = false;

                    CreateTeacherInfoInputDto techerDto = new CreateTeacherInfoInputDto()
                    {
                        user_id = model.Id
                    };
                    _teacherGuidService.CreateRecovryTeacherInfo(techerDto);
                    _userInfoService.RecoverInsertUsers(model);
                    list.Add("msg", "ok");
                }
                else
                {
                    list.Add("msg", "notExist");
                    list.Add("ErrMsg", result.Result.ErrMsg);
                    //msg = result.Result.ErrMsg;
                }

            }
            catch (Exception ex)
            {
                list.Add("msg", "fiale");
                //iLogger.Error(ex.ToString());
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}