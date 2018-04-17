using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SPOC.Common.Cookie;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.Common.Tools;
using SPOC.Core;
using SPOC.User;
using SPOC.Web.Controllers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPOC.Web.Areas.Statement.Controllers
{

    public class ChallengeController : SPOCControllerBase
    {
        private readonly IDepartmentService _iDepartmentService;
        private readonly IChallengeQuestionService _iChallengeQuestionService;
        public ChallengeController( IDepartmentService iDepartmentService, IChallengeQuestionService iChallengeQuestionService)
        {
            _iDepartmentService = iDepartmentService;
            _iChallengeQuestionService = iChallengeQuestionService;
        }

        // GET: Statement/Challenge
        public async Task<ActionResult> Leaderboard()
        {
             var cookie = CookieHelper.GetLoginInUserInfo();
             //教师角色的过滤出自己所教班级
             ViewBag.classIds = cookie.Identity==2 ? (await _iDepartmentService.GetAllClass()).Select(a => a.id).ToArray() : new Guid[]{};
            return View();
        }
        public  ActionResult UserAnswer()
        {
            return View();
        }
        /// <summary>
        /// 设置导出参数 缓存60秒有效期
        /// </summary>
        /// <param name="input"></param>
        public void SetExportParms(SPOC.Core.Dto.Challenge.RankInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            CacheStrategy.Remove(cookie+ "ExportParms");
            CacheStrategy.Insert(cookie+ "ExportParms", input, 60);

        }
        public async Task<FileResult>  ExportData()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var classIds = cookie.Identity == 2
                ? (await _iDepartmentService.GetAllClass()).Select(a => a.id).ToArray()
                : new Guid[] { };
            var input= (SPOC.Core.Dto.Challenge.RankInputDto) CacheStrategy.Get(cookie + "ExportParms")??new SPOC.Core.Dto.Challenge.RankInputDto();
            input.pageSize = int.MaxValue;
            input.page = 1;
            var result = await _iChallengeQuestionService.ChallengeLeaderboard(input);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] { "排名","姓名", "用户名", "总分", "班级" };
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
            //默认分数样式
            var scoreStyle = workbook.CreateCellStyle();
            scoreStyle.DataFormat = dataFormat.GetFormat("0.00");
            //带颜色的分数样式
            var scoreColorStyle = workbook.CreateCellStyle();
            scoreColorStyle.DataFormat = dataFormat.GetFormat("0.00");
            scoreColorStyle.FillForegroundColor = HSSFColor.Pink.Index;
            scoreColorStyle.FillBackgroundColor = HSSFColor.Pink.Index;
            scoreColorStyle.FillPattern = FillPattern.SolidForeground;
            //scoreColorStyle.BorderBottom =BorderStyle.Thin;
            //scoreColorStyle.BorderLeft = BorderStyle.Thin;
            //scoreColorStyle.BorderRight = BorderStyle.Thin;
            //scoreColorStyle.BorderTop = BorderStyle.Thin;
            var colorStyle = workbook.CreateCellStyle();
            colorStyle.FillForegroundColor = HSSFColor.Pink.Index;
            colorStyle.FillBackgroundColor = HSSFColor.Pink.Index;
            colorStyle.FillPattern = FillPattern.SolidForeground;
            foreach (var item in result.RankList)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.rank);
                row.CreateCell(1).SetCellValue(item.userName);
                row.CreateCell(2).SetCellValue(item.loginName);
                var cell = row.CreateCell(3);
                cell.CellStyle = scoreStyle;
                cell.SetCellValue(Convert.ToDouble(item.score));
                row.CreateCell(4).SetCellValue(string.IsNullOrEmpty(item.className) ? "" : item.facultyName+"/"+item.majorName + "/" + item.className);
                //符合条件标记颜色
                if (classIds.Any(c=> item.classId.Equals(c)))
                {
                    row.Cells.ForEach(c => { c.CellStyle = !cell.Equals(c) ? colorStyle : scoreColorStyle; });
                  
                }
                rowIndex++;
            }

            var ms = new NpoiMemoryStream {AllowClose = false};
            workbook.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            return File(ms, "application/vnd.ms-excel", "挑战排行榜" + DateTimeUtil.NowData + ".xlsx");
        }
    }
}