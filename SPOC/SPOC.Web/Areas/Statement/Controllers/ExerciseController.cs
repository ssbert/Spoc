using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NPOI.XSSF.UserModel;
using SPOC.Statement;
using SPOC.Statement.Dto.Exercise;

namespace SPOC.Web.Areas.Statement.Controllers
{
    public class ExerciseController : StatementBaseController
    {
        private readonly IExerciseStatementService _iExerciseStatementService;
        public ExerciseController(IExerciseStatementService iExerciseStatementService)
        {
            _iExerciseStatementService = iExerciseStatementService;
        }
        // GET: Statement/Exercise
        public ActionResult Index()
        {
            return View();
        }

        //学生练习情况
        public ActionResult Student()
        {
            return View();
        }

        //学生练习记录
        public ActionResult ExerciseRecord()
        {
            return View();
        }

        //效率排行榜报表
        public ActionResult EfficiencyRanking()
        {
            return View();
        }

        //积极性排行榜
        public ActionResult EnthusiasmRanking()
        {
            return View();
        }

        //班级排行榜
        public ActionResult ClassRanking()
        {
            return View();
        }

        //学生练习作答数据
        public async Task<ActionResult> ExerciseAnswer(Guid id)
        {
            var model = await _iExerciseStatementService.GetExerciseAnswer(id);
            return View(model);
        }

        #region 创建导出文件

        public async Task<JsonResult> ExportExercise(ExerciseStatementPaginationInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iExerciseStatementService.GetPagination(input);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);

            var titles = new[] { "练习名称", "通过率", "已通过人数", "未通过人数", "参考率", "已参加人数", "未参加人数", "学生总数", "创建时间", "创建者姓名", "创建者用户名"};
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
            var style1 = workbook.CreateCellStyle();
            style1.DataFormat = dataFormat.GetFormat("0.00%");
            foreach (var item in result.rows)
            {
                var row = sheet.CreateRow(rowIndex);
                //练习名称 
                row.CreateCell(0).SetCellValue(item.Title);
                //通过率
                var cell1 = row.CreateCell(1);
                cell1.CellStyle = style1;
                cell1.SetCellValue(Convert.ToDouble(item.PassRate / 100));
                //已通过人数
                row.CreateCell(2).SetCellValue(item.PassNum);
                //未通过人数
                row.CreateCell(3).SetCellValue(item.FailNum);
                //参考率
                var cell4 = row.CreateCell(4);
                cell4.CellStyle = style1;
                cell4.SetCellValue(Convert.ToDouble(item.JoinRate / 100));
                //已参加人数 
                row.CreateCell(5).SetCellValue(item.JoinNum);
                //未参加人数
                row.CreateCell(6).SetCellValue(item.WithoutNum);
                //学生总数
                row.CreateCell(7).SetCellValue(item.StudentNum);
                //创建时间
                row.CreateCell(8).SetCellValue(item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                //创建者姓名 
                row.CreateCell(9).SetCellValue(item.UserFullName);
                //创建者用户名
                row.CreateCell(10).SetCellValue(item.UserLoginName);
                rowIndex++;
            }
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);
            return Json(new { id = id.ToString() });
        }

        public async Task<JsonResult> ExportStudentExerciseInfo(ExerciseStudentStatementPaginationInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iExerciseStatementService.GetStudentPagination(input);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] { "姓名", "用户名", "班级", "通过情况", "参加情况"};
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
                //姓名
                row.CreateCell(0).SetCellValue(item.UserFullName);
                //用户名
                row.CreateCell(1).SetCellValue(item.UserLoginName);
                //班级
                row.CreateCell(2).SetCellValue(item.ClassName);
                //通过情况
                row.CreateCell(3).SetCellValue(item.PassState == 1 ? "已通过": "未通过");
                //参加情况
                row.CreateCell(4).SetCellValue(item.JoinState == 1 ? "已参加": "未参加");
                rowIndex++;
            }
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);
            return Json(new { id = id.ToString() });
        }

        public async Task<JsonResult> ExportExerciseEfficiencyRanking(ExerciseRankingStatementPaginationInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iExerciseStatementService.GetEfficiencyRankingPagination(input);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);

            var titles = new[] { "姓名", "用户名", "班级", "练习次数","耗时", "是否通过", "开始时间", "结束时间", "班级排名", "总排名" };
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
                //姓名
                row.CreateCell(0).SetCellValue(item.UserFullName);
                //用户名
                row.CreateCell(1).SetCellValue(item.UserLoginName);
                //班级
                row.CreateCell(2).SetCellValue(item.ClassName);
                //练习次数
                row.CreateCell(3).SetCellValue(item.ExerciseCount == 0 ? "-": "" + item.ExerciseCount);
                //耗时
                if (item.UseTime == 0)
                {
                    row.CreateCell(4).SetCellValue("-");
                }
                else
                {
                    var useTimeStr = "";
                    var useTime = item.UseTime;
                    if (useTime >= 86400)
                    {
                        var d = (int)Math.Floor((double)useTime / 86400);
                        useTimeStr += d + "天";
                        useTime -= d * 86400;
                    }

                    if (useTime >= 3600)
                    {
                        var h = (int)Math.Floor((double)useTime / 3600);
                        useTimeStr += h + "时";
                        useTime -= h * 3600;
                    }

                    if (useTime >= 60)
                    {
                        var m = (int)Math.Floor((double)useTime / 60);
                        useTimeStr += m + "分";
                        useTime -= m * 60;
                    }
                    useTimeStr += useTime + "秒";
                    row.CreateCell(4).SetCellValue(useTimeStr);
                }
                
                //是否通过
                row.CreateCell(5).SetCellValue(item.IsPass?"是":"否");
                //开始时间
                row.CreateCell(6).SetCellValue(item.BeginTime?.ToString("yyyy-MM-dd hh:mm:ss") ?? "-");
                //结束时间
                row.CreateCell(7).SetCellValue(item.EndTime?.ToString("yyyy-MM-dd hh:mm:ss") ?? "-");
                //班级排名
                row.CreateCell(8).SetCellValue(item.ClassRanking);
                //总排名
                row.CreateCell(9).SetCellValue(item.Ranking);
                rowIndex++;
            }
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);
            return Json(new { id = id.ToString() });
        }

        public async Task<JsonResult> ExportExerciseEnthusiasmRanking(ExerciseRankingStatementPaginationInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iExerciseStatementService.GetEnthusiasmRankingPagination(input);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);

            var titles = new[] { "姓名", "用户名", "班级", "开始时间", "是否通过", "班级排名", "总排名" };
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
                //姓名
                row.CreateCell(0).SetCellValue(item.UserFullName);
                //用户名
                row.CreateCell(1).SetCellValue(item.UserLoginName);
                //班级
                row.CreateCell(2).SetCellValue(item.ClassName);
                //开始时间
                row.CreateCell(3).SetCellValue(item.BeginTime?.ToString("yyyy-MM-dd hh:mm:ss") ?? "-");
                //是否通过
                row.CreateCell(4).SetCellValue(item.IsPass ? "是" : "否");
                //班级排名
                row.CreateCell(5).SetCellValue(item.ClassRanking);
                //总排名
                row.CreateCell(6).SetCellValue(item.Ranking);
                rowIndex++;
            }
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);
            return Json(new { id = id.ToString() });
        }

        public async Task<JsonResult> ExportExerciseClassRanking(ExerciseClassRankingQueryInputDto input)
        {
            var id = Guid.NewGuid();
            var result = await _iExerciseStatementService.GetClassRankingList(input);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);

            var titles = new[] { "班级", "通过率", "参加率", "通过人数", "参加人数", "班级人数", "排名" };
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
            var style1 = workbook.CreateCellStyle();
            style1.DataFormat = dataFormat.GetFormat("0.00%");
            foreach (var item in result)
            {
                var row = sheet.CreateRow(rowIndex);
                //班级
                row.CreateCell(0).SetCellValue(item.ClassName);
                //通过率
                var cell1 = row.CreateCell(1);
                cell1.CellStyle = style1;
                cell1.SetCellValue(Convert.ToDouble(item.PassRate / 100));
                //参加率
                var cell2 = row.CreateCell(2);
                cell2.CellStyle = style1;
                cell2.SetCellValue(Convert.ToDouble(item.JoinRate / 100));
                //通过人数
                row.CreateCell(3).SetCellValue(item.PassNum);
                //参加人数
                row.CreateCell(4).SetCellValue(item.JoinNum);
                //班级人数
                row.CreateCell(5).SetCellValue(item.StudentNum);
                //排名
                if (item.Ranking != 0)
                {
                    row.CreateCell(6).SetCellValue(item.Ranking);
                }

                rowIndex++;
            }
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);
            return Json(new { id = id.ToString() });
        }
        #endregion

    }
}