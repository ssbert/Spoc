using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SPOC.Exam;
using SPOC.Exam.GradeDto;
using SPOC.Statement;
using SPOC.Statement.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPOC.Web.Areas.Statement.Controllers
{
    public class ExamController : StatementBaseController
    {
        private readonly IExamTaskStatementService _iExamTaskStatementService;
        private readonly IExamGradeService _iExamGradeService;
        public ExamController(IExamTaskStatementService iExamTaskStatementService, IExamGradeService iExamGradeService)
        {
            _iExamTaskStatementService = iExamTaskStatementService;
            _iExamGradeService = iExamGradeService;
        }
        // GET: Statement/Exam
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Student()
        {
            return View();
        }

        public ActionResult StudentGrade()
        {
            return View();
        }

        public ActionResult Retest()
        {
            return View();
        }

        public ActionResult Class()
        {
            return View();
        }

        public ActionResult UserExamInfo()
        {
            return View();
        }

        #region 创建导出文件
        //导出考试报表
        public async Task<JsonResult> ExportExam(ExamTaskStatementPaginationInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iExamTaskStatementService.GetPagination(input);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            
            var titles = new[]
            {
                "考试名称", "通过率", "已通过人数", "未通过人数",
                "参考率", "已参加人数", "未参加人数",
                "已提交人数", "已参加未提交人数",
                "已出成绩人数", "等待评分人数", "学生总数",
                "平均分", "最高分", "最低分",
                "创建时间", "创建者姓名", "创建者用户名"
            };
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
            var style2 = workbook.CreateCellStyle();
            style2.DataFormat = dataFormat.GetFormat("0.00");
            var style3 = workbook.CreateCellStyle();
            style3.Alignment = HorizontalAlignment.Right;

            foreach (var item in result.rows)
            {
                //考试名称
                var row = sheet.CreateRow(rowIndex);
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
                //已提交人数
                row.CreateCell(7).SetCellValue(item.SubmitNum);
                //未提交人数
                row.CreateCell(8).SetCellValue(item.UnSubmitNum);
                //已出成绩人数
                row.CreateCell(9).SetCellValue(item.CompiledNum);
                //等待评分人数
                row.CreateCell(10).SetCellValue(item.UnCompiledNum);
                //学生总数
                row.CreateCell(11).SetCellValue(item.StudentNum);
                //平均分
                var cell12 = row.CreateCell(12);
                if (item.AverageScore.HasValue)
                {
                    cell12.CellStyle = style2;
                    cell12.SetCellValue(Convert.ToDouble(item.AverageScore.Value));
                }
                else
                {
                    cell12.CellStyle = style3;
                    cell12.SetCellValue("-");
                }
                //最高分
                var cell13 = row.CreateCell(13);
                if (item.MaxScore.HasValue)
                {
                    cell13.CellStyle = style2;
                    cell13.SetCellValue(Convert.ToDouble(item.MaxScore.Value));
                }
                else
                {
                    cell13.CellStyle = style3;
                    cell13.SetCellValue("-");
                }
                //最低分
                var cell14 = row.CreateCell(14);
                if (item.MinScore.HasValue)
                {
                    cell14.CellStyle = style2;
                    cell14.SetCellValue(Convert.ToDouble(item.MinScore.Value));
                }
                else
                {
                    cell14.CellStyle = style3;
                    cell14.SetCellValue("-");
                }

                //创建时间
                row.CreateCell(15).SetCellValue(item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                //创建者姓名
                row.CreateCell(16).SetCellValue(item.UserFullName);
                //创建者用户名
                row.CreateCell(17).SetCellValue(item.UserLoginName);
                rowIndex++;
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);
            return Json(new {id = id.ToString()});
        }
        //导出按学生考试报表
        public async Task<JsonResult> ExportGrade(GradeRankingPaginationInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iExamTaskStatementService.GetGradePagination(input);
            ExportExamGrade(id, result.rows);
            return Json(new { id = id.ToString() });
        }
        //导出补考考试报表
        public async Task<JsonResult> ExportRetest(RetestRankPaginationInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iExamTaskStatementService.GetRetestPagination(input);
            ExportExamGrade(id, result.rows);
            return Json(new {id = id.ToString()});
        }
        //导出按班级考试报表
        public async Task<JsonResult> ExportClass(ClassRankingQueryInputDto input)
        {
            var id = Guid.NewGuid();
            var result = await _iExamTaskStatementService.GetClassRankingList(input);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] { "班级", "平均分", "最高分", "最低分", "通过率", "参考率", "通过人数", "参考人数", "班级人数", "0-49", "50-59", "60-69", "70-79", "80-89", "90-100", "排名" };
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
            var style2 = workbook.CreateCellStyle();
            style2.DataFormat = dataFormat.GetFormat("0.00");
            var style3 = workbook.CreateCellStyle();
            style3.Alignment = HorizontalAlignment.Right;
            foreach (var item in result)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.ClassName);

                var cell1 = row.CreateCell(1);
                if (item.AverageScore.HasValue)
                {
                    cell1.CellStyle = style2;
                    cell1.SetCellValue(Convert.ToDouble(item.AverageScore.Value));
                }
                else
                {
                    cell1.CellStyle = style3;
                    cell1.SetCellValue("-");
                }

                var cell2 = row.CreateCell(2);
                if (item.MaxScore.HasValue)
                {
                    cell2.CellStyle = style2;
                    cell2.SetCellValue(Convert.ToDouble(item.MaxScore.Value));
                }
                else
                {
                    cell2.CellStyle = style3;
                    cell2.SetCellValue("-");
                }

                var cell3 = row.CreateCell(3);
                if (item.MinScore.HasValue)
                {
                    cell3.CellStyle = style2;
                    cell3.SetCellValue(Convert.ToDouble(item.MinScore.Value));
                }
                else
                {
                    cell3.CellStyle = style3;
                    cell3.SetCellValue("-");
                }

                var cell4 = row.CreateCell(4);
                cell4.CellStyle = style1;
                cell4.SetCellValue((double)item.PassRate / 100);

                cell4 = row.CreateCell(5);
                cell4.CellStyle = style1;
                cell4.SetCellValue((double)item.JoinRate / 100);

                row.CreateCell(6).SetCellValue(item.PassNum);
                row.CreateCell(7).SetCellValue(item.JoinNum);
                row.CreateCell(8).SetCellValue(item.StudentNum);
                row.CreateCell(9).SetCellValue(item.ScoreSectionNum1);
                row.CreateCell(10).SetCellValue(item.ScoreSectionNum2);
                row.CreateCell(11).SetCellValue(item.ScoreSectionNum3);
                row.CreateCell(12).SetCellValue(item.ScoreSectionNum4);
                row.CreateCell(13).SetCellValue(item.ScoreSectionNum5);
                row.CreateCell(14).SetCellValue(item.ScoreSectionNum6);
                if (item.Ranking.HasValue)
                {
                    row.CreateCell(15).SetCellValue(item.Ranking.Value);
                }
                rowIndex++;
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);

            return Json(new {id = id.ToString()});
        }
        //导出用户考试记录
        public async Task<JsonResult> ExportUserExamRecord(UserExamRecordInputDto input)
        {
            var id = Guid.NewGuid();
            var result = await _iExamGradeService.GetUserExamRecordList(input);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] { "开始时间", "结束时间", "分数", "是否通过"};
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
            style1.DataFormat = dataFormat.GetFormat("0.00");
            var style3 = workbook.CreateCellStyle();
            style3.Alignment = HorizontalAlignment.Right;
            foreach (var item in result)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"));
                row.CreateCell(1).SetCellValue(item.EndTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-");
                var cell2 = row.CreateCell(2);
                if (item.GradeScore.HasValue)
                {
                    cell2.CellStyle = style1;
                    cell2.SetCellValue(Convert.ToDouble(item.GradeScore.Value));
                }
                else
                {
                    cell2.CellStyle = style3;
                    cell2.SetCellValue("-");
                }
                row.CreateCell(3).SetCellValue(item.IsPass ? "是" : "否");
                rowIndex++;
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);

            return Json(new { id = id.ToString() });
        }
        //导出学生考试情况
        public async Task<JsonResult> ExportStudentExamInfo(ExamTaskStudentStatementPaginationInputDto input)
        {
            var id = Guid.NewGuid();
            input.skip = 0;
            input.pageSize = int.MaxValue;
            var result = await _iExamTaskStatementService.GetStudentPagination(input);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] {"姓名", "用户名", "分数", "班级", "通过情况", "参加情况", "提交情况", "评分情况"};
            var titleRow = sheet.CreateRow(0);
            var titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            var titleStyle = workbook.CreateCellStyle();
            titleStyle.SetFont(titleFont);
            var dataFormat = workbook.CreateDataFormat();
            var style1 = workbook.CreateCellStyle();
            style1.DataFormat = dataFormat.GetFormat("0.00");
            var style3 = workbook.CreateCellStyle();
            style3.Alignment = HorizontalAlignment.Right;
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
                //分数
                var cell2 = row.CreateCell(2);
                if (item.Score.HasValue)
                {
                    cell2.CellStyle = style1;
                    cell2.SetCellValue(Convert.ToDouble(item.Score.Value));
                }
                else
                {
                    cell2.CellStyle = style3;
                    cell2.SetCellValue("-");
                }
                //班级
                row.CreateCell(3).SetCellValue(item.ClassName);
                //通过情况
                row.CreateCell(4).SetCellValue(item.PassState == 1 ? "已通过": "未通过");
                //参加情况
                row.CreateCell(5).SetCellValue(item.JoinState == 1 ? "已参加": "未参加");
                //提交情况
                if (item.SubmitState != 0)
                {
                    row.CreateCell(6).SetCellValue(item.SubmitState == 1 ? "已提交" : "未提交");
                }
                //评分情况
                if (item.CompileState != 0)
                {
                    row.CreateCell(7).SetCellValue(item.CompileState == 1 ? "已出成绩" : "未出成绩");
                }
                rowIndex++;
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);
            return Json(new { id = id.ToString() });
        }
        private void ExportExamGrade(Guid id, IEnumerable<GradeRankingItem> rows)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            sheet.CreateFreezePane(0, 1);
            var titles = new[] { "姓名", "用户名", "分数", "班级", "班级排名", "总排名" };
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
            style1.DataFormat = dataFormat.GetFormat("0.00");
            var style2 = workbook.CreateCellStyle();
            style2.Alignment = HorizontalAlignment.Right;
            foreach (var item in rows)
            {
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(item.UserFullName);
                row.CreateCell(1).SetCellValue(item.UserLoginName);
                var cell = row.CreateCell(2);
                if (item.Score.HasValue)
                {
                    cell.CellStyle = style1;
                    cell.SetCellValue(Convert.ToDouble(item.Score.Value));
                }
                else
                {
                    cell.CellStyle = style2;
                    cell.SetCellValue("-");
                }
                row.CreateCell(3).SetCellValue(item.ClassFullName);
                row.CreateCell(4).SetCellValue(item.RankingInClass);
                row.CreateCell(5).SetCellValue(item.Ranking);
                rowIndex++;
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "temp", "statement");
            CreateStatementTempFile(path, id, workbook);
        }
        #endregion

       
    }
}