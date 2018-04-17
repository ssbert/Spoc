

/**************************************
     ╭︿︿︿╮╭︿︿︿╮╭︿︿︿╮     
     {/ o o /} {/ . . /} {/ ︿︿ /}   
     ( (oo) )  ( (oo) )  ( (oo) )     
       ︶︶︶    ︶︶︶   ︶ ︶ ︶     
***************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Abp.UI;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.File;
using SPOC.SysSetting;
using SPOC.User;
using SPOC.User.Dto.Common;
using SPOC.User.Dto.StudentInfo;
using SPOC.Web.Controllers;
using SPOC.Web.Filters;

namespace SPOC.Web.Areas.User.Controllers
{
    [UserAuthorization]
    [DontWrapResult]
    public class StudentController : SPOCControllerBase
    {

        public static List<StudentInfoDto> stuList = new List<StudentInfoDto>();

        //  private readonly IUserInfoService _testGuidService;
        private readonly IStudentInfoService _studentGuidService;
        private readonly IUserInfoService _userInfoService;

        private readonly IUserInfoApiService _iUserInfoApiService;
        private string NewMoocApiUrl
        {
            get { return L("payUrl").TrimEnd('/') + "/"; }
        }
        public StudentController(IStudentInfoService studentGuidService, IUserInfoService userInfoService, IUserInfoApiService iUserInfoApiService)
        {

            _studentGuidService = studentGuidService;
            _userInfoService = userInfoService;
            LocalizationSourceName = SPOCConsts.LocalizationSourceName;
            _iUserInfoApiService = iUserInfoApiService;

        }
        // GET: User/Student

        public ActionResult Index()
        {
            ViewBag.apiUrl = NewMoocApiUrl;
            ViewBag.usernameContainEN = BaseSiteSetDto.usernameContainEN;
            return View();
        }
        public ActionResult CheckStudent()
        {

            return View();
        }





        //public ActionResult ModifyStudent(CreateUserInfoInputDto model)
        //{
        //    return View(model);
        //}

        #region 获取列表

        public JsonResult Get()
        {



            try
            {
                var provider = new EasyUIProvider();
                var pager = provider.GetPager(Request);
                var filter = new StudentInfoInputDto();
                if (!string.IsNullOrEmpty(pager.Filter))
                {
                    filter = provider.DeserializeObject<StudentInfoInputDto>(pager.Filter);
                }
                filter.PageSize = pager.PageSize;
                filter.CurrentPage = pager.CurrentPage - 1;
                filter.SortOrder = pager.SortOrder;
                filter.SortCloumnName = pager.SortCloumnName;
                var grid = _studentGuidService.GetStudentInfoByGuid(filter);
                stuList = grid.rows.ToList();
                return Json(grid);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public ActionResult StudentEdit(string id)
        {

            ViewBag.id = string.IsNullOrEmpty(id) ? "" : id;

            ViewBag.usernameContainEN = BaseSiteSetDto.usernameContainEN;

            return View();
        }
        #endregion

        public JsonResult GetStudent(string id)
        {
            var gid = string.IsNullOrEmpty(id) ? Guid.Empty : new Guid(id);
            StudentInfoDto res = new StudentInfoDto();
            //var res = _studentGuidService.GetStudentInfoByStudentId(string.IsNullOrEmpty(id)?Guid.Empty:new Guid(id));
            //if (stuList == null || stuList.Count == 0)
            //{
            //    Redirect("/User/User/Login");
            res = _studentGuidService.GetStudentInfo(new UserInfoQueryInputDto() { id = gid });
   
           
            return Json(res);
        }

        /// <summary>
        /// 从Excel汇入学生列表
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DontWrapResult]
        public ActionResult CreateStudentFromFile(HttpPostedFileBase file, string departmentUid)
        {
            try
            {
                var index = file.FileName.LastIndexOf('.');
                var extName = file.FileName.Substring(index + 1);
                ImportResultOutputDto result = new ImportResultOutputDto();
                if (extName == "xls" || extName == "xlsx")
                {
                    result = _studentGuidService.CreateUserInfoFromFile(file.InputStream, departmentUid);
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

        [System.Web.Mvc.ActionName("exportuserinfo")]
        public FileResult ExportData(UserInfoSeacheExport userInfoModel)
        {
            //var filter = Request.Params["filter"];
            var filterSession = Session["ExportDataFilter"];
            var filter = filterSession == null ? "" : filterSession.ToString();
            var input = new StudentInfoInputDto();
            JavaScriptSerializer json = new JavaScriptSerializer();
            if (filter != null)
            {
                input = json.Deserialize<StudentInfoInputDto>(filter);
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
          
            var listConstraint = _studentGuidService.GetStudentInfoMap();
            foreach (var userConstraintList in listConstraint)
            {
                keyUserCode.Add(userConstraintList.FieldCode);
                keyUserName.Add(userConstraintList.FieldName);
            }
            ExcelImportExport excel = new ExcelImportExport();
            var book = excel.GenerateData("学生", "学生", keyUserCode.ToArray(), keyUserName.ToArray(), userInfoModel, inputList, input, _userInfoService, _studentGuidService);
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "学生.xls");
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

        public FileResult UserInfoExportTemplate(string id)
        {
            return File(_studentGuidService.GetStudentInfoExportTemplate(id), "application/vnd.ms-excel", Server.UrlDecode("学生信息导入模板") + ".xls");
            //return File(_studentGuidService.GetStudentInfoExportTemplate("登录名,密码"), "application/vnd.ms-excel", Server.UrlDecode("学生信息导入模板") + ".xls");
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
                    model.identity = 1;
                    model.userEnbleFlag = false;

                    StudentInfo _stu = new StudentInfo()
                    {
                        Id = Guid.NewGuid(),
                        userId = model.Id,
                        userEnbleFlag = 0,
                        createTime = DateTime.Now,
                        isDel = false
                    };
                    _studentGuidService.CreateRecovryStudentInfo(_stu);
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