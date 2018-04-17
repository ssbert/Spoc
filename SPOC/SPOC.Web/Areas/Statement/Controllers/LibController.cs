using NPOI.XSSF.UserModel;
using SPOC.Statement;
using SPOC.Statement.Dto.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Domain.Repositories;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using SPOC.Common.Cookie;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.Common.Tools;
using SPOC.Lib;
using SPOC.Lib.Dto;

namespace SPOC.Web.Areas.Statement.Controllers
{
    public class LibController : StatementBaseController
    {
        private readonly ILibLabelStatementService _iLibLabelStatementService;
        private readonly ILibLabelViewService _iLibLabelViewService;
        private readonly IRepository<Label,Guid> _iLabelRep;
        public LibController(ILibLabelStatementService iLibLabelStatementService, ILibLabelViewService iLibLabelViewService, IRepository<Label, Guid> iLabelRep)
        {
            _iLibLabelStatementService = iLibLabelStatementService;
            _iLibLabelViewService = iLibLabelViewService;
            _iLabelRep = iLabelRep;
        }

        // GET: Statement/Lib/Class
        public ActionResult Class()
        {
            return View();
        }
        public ActionResult ClassContrast()
        {
            return View();
        }
        
        public ActionResult ClassLabelGettingDetail()
        {
            return View();
        }

        /// <summary>
        /// 知识点按学生统计
        /// </summary>
        /// <returns></returns>
        public ActionResult Student()
        {
            return View();
        }
        /// <summary>
        /// 学生的知识点图谱
        /// </summary>
        /// <returns></returns>
        public ActionResult StructureMap()
        {
            ViewBag.labelDeductPoint = -Convert.ToInt32(BaseSiteSetDto.labelDeductPoint);
            return View();
        }
        /// <summary>
        /// 用户标签掌握情况
        /// </summary>
        /// <returns></returns>
        public ActionResult UserLabelStatement()
        {
            ViewBag.labelDeductPoint = -Convert.ToInt32(BaseSiteSetDto.labelDeductPoint);
            return View();
        }
        /// <summary>
        /// 用户标签作答记录
        /// </summary>
        /// <returns></returns>
        public ActionResult UserAnswerRecords()
        {
            return View();
        }
        /// <summary>
        /// 作答详细记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewType">查看类型 默认lib表示通过知识点查看 challenge:挑战报表查看</param>
        /// <returns></returns>
        public async Task<ActionResult> UserAnswerQuestion(Guid id,string viewType="lib")
        {
            UserAnswerRecordsQuestion model;
            if (viewType.Equals("challenge"))
                model = await _iLibLabelViewService.GetRecordsQuestionByChallenge(id);
            else
                model = await _iLibLabelViewService.GetUserAnswerRecordsQuestion(id);
            return View(model);
        }

        #region 创建导出文件

        public async Task<JsonResult> ExportStudent(StudentLabelGettingInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iLibLabelStatementService.StudentLabelGettingList(input);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] { "姓名", "用户名", "班级", "掌握", "未掌握", "不稳定", "无反馈", "知识点总数", "掌握比率"};
            var titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            var titleStyle = workbook.CreateCellStyle();
            titleStyle.SetFont(titleFont);
            var titleRow = sheet.CreateRow(0);
            for (var i = 0; i < titles.Length; i++)
            {
                var cell = titleRow.CreateCell(i);
                cell.SetCellValue(titles[i]);
                cell.CellStyle = titleStyle;
            }

            var rowIndex = 1;
            var dataFormat = workbook.CreateDataFormat();
            var style = workbook.CreateCellStyle();
            style.DataFormat = dataFormat.GetFormat("0.00%");
            foreach (var item in result.rows)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.UserFullName);
                row.CreateCell(1).SetCellValue(item.UserLoginName);
                row.CreateCell(2).SetCellValue(item.ClassName);
                row.CreateCell(3).SetCellValue(item.MasterNum);
                row.CreateCell(4).SetCellValue(item.FailNum);
                row.CreateCell(5).SetCellValue(item.UnskilledNum);
                row.CreateCell(6).SetCellValue(item.EmptyNum);
                row.CreateCell(7).SetCellValue(item.LabelNum);
                var cell = row.CreateCell(8);
                cell.CellStyle = style;
                cell.SetCellValue(item.MasterRate / 100);
                rowIndex++;
            }
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);

            return Json(new { id = id.ToString() });
        }

        public async Task<JsonResult> ExportUserLabelStatement(StudentLabelStatementInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iLibLabelStatementService.StudentLabelStatementPagination(input);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] { "知识点", "掌握情况" };
            var titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            var titleStyle = workbook.CreateCellStyle();
            titleStyle.SetFont(titleFont);
            var titleRow = sheet.CreateRow(0);
            for (var i = 0; i < titles.Length; i++)
            {
                var cell = titleRow.CreateCell(i);
                cell.SetCellValue(titles[i]);
                cell.CellStyle = titleStyle;
            }

            var rowIndex = 1;
            foreach (var item in result.rows)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.LabelTitle);
                var proficiency = "";
                if (item.Score == null)
                {
                    proficiency = "无反馈";
                } else if (item.Score.Value > 0)
                {
                    proficiency = "已掌握";
                } else if (item.Score.Value == 0)
                {
                    proficiency = "不稳定";
                } else if (item.Score.Value < 0)
                {
                    proficiency = "未掌握";
                }
                row.CreateCell(1).SetCellValue(proficiency);
                rowIndex++;
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);

            return Json(new { id = id.ToString() });
        }
        #endregion
        #region 班级标签报表导出
        /// <summary>
        /// 设置导出参数 缓存60秒有效期
        /// </summary>
        /// <param name="input"></param>
        public void SetClassExportParms(ClassLabelGettingInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            CacheStrategy.Remove(cookie + "ExportParms");
            CacheStrategy.Insert(cookie + "ExportParms", input, 60);

        }
        public void SetStudentsExportParms(UserLabelGettingInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            CacheStrategy.Remove(cookie + "ExportParms");
            CacheStrategy.Insert(cookie + "ExportParms", input, 60);

        }
        /// <summary>
        /// 导出班级标签掌握统计表
        /// </summary>
        /// <returns></returns>
        public async Task<FileResult> ExportClassData()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
           
            var input = (ClassLabelGettingInputDto)CacheStrategy.Get(cookie + "ExportParms") ?? new ClassLabelGettingInputDto();
            input.pageSize = int.MaxValue;
            input.page = 1;
            if(input.ClassIdList==null)
                input.ClassIdList=new List<Guid>();
            var result = await _iLibLabelStatementService.ClassLabelGettingList(input);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] { "知识点", "掌握学生数", "未掌握学生数", "无反馈学生数", "不稳定学生数", "学生总数", "掌握比率" };
            var titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            var titleStyle = workbook.CreateCellStyle();
            titleStyle.SetFont(titleFont);
            var titleRow = sheet.CreateRow(0);
            for (var i = 0; i < titles.Length; i++)
            {
                var cell = titleRow.CreateCell(i);
                cell.SetCellValue(titles[i]);
                cell.CellStyle = titleStyle;
            }
            var rowIndex = 1;
            var dataFormat = workbook.CreateDataFormat();
            var style = workbook.CreateCellStyle();
            style.DataFormat = dataFormat.GetFormat("0.00%");
            foreach (var item in result.rows)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.Title);
                row.CreateCell(1).SetCellValue(item.PassNumber);
                row.CreateCell(2).SetCellValue(item.FailNumber);
                row.CreateCell(3).SetCellValue(item.NotJoinNumber);
                row.CreateCell(4).SetCellValue(item.UnstableNumber);
                row.CreateCell(5).SetCellValue(item.StudentNumber);
                var cell = row.CreateCell(6);
                cell.CellStyle = style;
                cell.SetCellValue((double)item.PassRate / 100);
                rowIndex++;
            }
            var ms = new NpoiMemoryStream { AllowClose = false };
            workbook.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            return File(ms, "application/vnd.ms-excel", "班级知识点掌握情况统计报表" + DateTimeUtil.NowData + ".xlsx");
        }
        /// <summary>
        /// 导出班级下学生某个标签统计表
        /// </summary>
        /// <returns></returns>
        public async Task<FileResult> ExportStudentsData()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
          
            var input = (UserLabelGettingInputDto)CacheStrategy.Get(cookie + "ExportParms") ?? new UserLabelGettingInputDto();
            var lable = _iLabelRep.FirstOrDefault(a => a.Id.Equals(input.LabelId));
            if (input.ClassIdList == null)
                input.ClassIdList = new List<Guid>();
            input.pageSize = int.MaxValue;
            input.page = 1;
            var result = await _iLibLabelStatementService.UserLabelGettingList(input);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] { "姓名", "用户名", "班级", "掌握情况" };
            var titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            var titleStyle = workbook.CreateCellStyle();
            titleStyle.SetFont(titleFont);
            var titleRow = sheet.CreateRow(0);
            for (var i = 0; i < titles.Length; i++)
            {
                var cell = titleRow.CreateCell(i);
                cell.SetCellValue(titles[i]);
                cell.CellStyle = titleStyle;
            }
            var rowIndex = 1;
          
            foreach (var item in result.rows)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.UserFullName);
                row.CreateCell(1).SetCellValue(item.UserLoginName);
                row.CreateCell(2).SetCellValue(item.ClassName);
                var status = "";
                if (item.Status == -1)
                {
                    status = "无反馈";
                }
                else if (item.Status==3)
                {
                    status = "已掌握";
                }
                else if (item.Status == 2)
                {
                    status = "不稳定";
                }
                else if (item.Status == 1)
                {
                    status = "未掌握";
                }
                row.CreateCell(3).SetCellValue(status);
                
               
                rowIndex++;
            }
            var ms = new NpoiMemoryStream { AllowClose = false };
            workbook.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            return File(ms, "application/vnd.ms-excel", "["+lable.title+"]学员掌握情况报表" + DateTimeUtil.NowData + ".xlsx");
        }
        #endregion
    }
}