using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.Web.Models;
using SPOC.Common.Cookie;
using SPOC.Core;
using SPOC.Exam;
using SPOC.Web.Filters;
using SPOC.Web.Models.Leaderboard;

namespace SPOC.Web.Controllers
{
    [UserAuthorization]
    public class LeaderboardController : SPOCControllerBase
    {
        private readonly IChallengeQuestionService _iChallengeQuestionService;
        private readonly IExamRankingViewService _iexamvRankingViewService;
        public LeaderboardController(IChallengeQuestionService iChallengeQuestionService, IExamRankingViewService iexamvRankingViewService)
        {
            _iChallengeQuestionService = iChallengeQuestionService;
            _iexamvRankingViewService = iexamvRankingViewService;
        }
        // GET: Leaderboard
        public async Task<ActionResult> Index(string classId="",string userName="", string userId = "", string className = "", int currentPage = 1)
        {
            var dto = await _iChallengeQuestionService.ChallengeLeaderboard(new Core.Dto.Challenge.RankInputDto{pageSize=10,  page = currentPage,classId = classId,userId = userId });
            ViewBag.currentPage = currentPage;
            ViewBag.classId = classId;
            ViewBag.userId = userId;
            ViewBag.className = className;
            ViewBag.userName = userName;
            ViewBag.currUserId =  CookieHelper.GetLoginInUserInfo().UserUid; ;
            return View(dto);
        }
        public async Task<ActionResult> ExamRank()
        {
            var recentExamList= await _iexamvRankingViewService.RecentExamList();
            var exam = recentExamList.FirstOrDefault();
            var model = new ExamRankViewModel
            {
                ExamListItems = recentExamList,
                Exam = exam?.Value
            };
            return View(model);
        }
        /// <summary>
        /// 获取挑战用户列表
        /// </summary>
        /// <returns></returns>
        [DontWrapResult]
        public async Task<JsonResult> GetChallengeUserArray(string q)
        {
            var result = await _iChallengeQuestionService.GetChallengeUserList(q);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取挑战用户所在班级列表
        /// </summary>
        /// <returns></returns>
        [DontWrapResult]
        public async Task<JsonResult> GetChallengeClassArray(string q)
        {
            var result = await _iChallengeQuestionService.GetChallengeClassList(q);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}