using System;
using SPOC.Common.Pagination;
using SPOC.Exam;
using System.Threading.Tasks;
using System.Web.Mvc;
using SPOC.Common.Cookie;
using SPOC.Exam.ViewDto;
using SPOC.Web.Filters;
using SPOC.Web.Models.ExamTask;

namespace SPOC.Web.Controllers
{
    public class ExamTaskController : SPOCControllerBase
    {
        private readonly IExamTaskViewService _iExamTaskViewService;
        private readonly IExamRankingViewService _iExamRankingViewService;
        public ExamTaskController(IExamTaskViewService iExamTaskViewService, IExamRankingViewService iExamRankingViewService)
        {
            _iExamTaskViewService = iExamTaskViewService;
            _iExamRankingViewService = iExamRankingViewService;
        }
        // GET: Exam
        [UserAuthorization]
        public async Task<ActionResult> Index(int page = 1)
        {
            var pageSize = 10;
            page = page < 1 ? 1 : page;
            var pagination = await _iExamTaskViewService.GetPagination(new PaginationInputDto
            {
                skip = (page - 1) * pageSize,
                pageSize = pageSize
            });
            ViewBag.currentPage = page;
            return View(pagination);
        }

        [UserAuthorization]
        public async Task<ActionResult> ExamGrade(Guid id, Guid taskId)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var dto = await _iExamTaskViewService.GetExamGrade(cookie.Id, taskId, id);
            return View(dto);
        }

        [UserAuthorization]
        public async Task<ActionResult> Ranking(Guid id, Guid examId, string examTypeCode="exam_normal", int page = 1)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var pageSize = 10;
            page = page < 1 ? 1 : page;
            var model = new RankingViewModel
            {
                Base = await _iExamTaskViewService.GetBase(id),
                Pagination = await _iExamRankingViewService.GetRankingPagination(new RankingPaginationInputDto
                {
                    ExamId = examId,
                    skip = (page - 1) * pageSize,
                    pageSize = pageSize,
                    ExamTypeCode = examTypeCode
                }),
                SelfRanking = await _iExamRankingViewService.GetRanking(examId, cookie.Id, examTypeCode),
                ExamTypeCode = examTypeCode
            };
            model.Base.TargetExamId = examId;
            ViewBag.currentPage = page;
            return View(model);
        }

        /// <summary>
        /// 考试心跳
        /// </summary>
        public void Heartbeat() { }
    }
}