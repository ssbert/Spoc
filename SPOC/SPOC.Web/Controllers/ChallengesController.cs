using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.Domain.Repositories;
using SPOC.Common.Helper;
using SPOC.Core;
using SPOC.Core.Dto.Challenge;
using SPOC.Web.Filters;

namespace SPOC.Web.Controllers
{
    [UserAuthorization]
    public class ChallengesController : SPOCControllerBase
    {
        private readonly IChallengeQuestionService _iChallengeQuestionService;
        private readonly IRepository<ChallengeQuestion, Guid> _iChallengeQuestionRep;
        public ChallengesController(IChallengeQuestionService iChallengeQuestionService, IRepository<ChallengeQuestion, Guid> iChallengeQuestionRep)
        {
            _iChallengeQuestionService = iChallengeQuestionService;
            _iChallengeQuestionRep = iChallengeQuestionRep;
        }

        // GET: Challenge
        public async  Task<ActionResult> Index(int currentPage = 1, string categoryId = "", string hard = "", string isPass = "",string label="")
        {
            var folder = _iChallengeQuestionService.GetChallengeFolder();
            var pointsAndRank = _iChallengeQuestionService.GetPointsAndRank();
            var model =await  _iChallengeQuestionService.GetChallengeList(new ChallengeInputDto
            {
                pageSize = 10,
                currentPage = currentPage,
                folderId = categoryId,
                hard = hard,
                isPass = isPass,
                skip = 10* (currentPage-1),
                label = label
            });
            model.folderList = await folder;
            model.PointsRank = await pointsAndRank;
            ViewBag.categoryId = categoryId;
            ViewBag.hard = hard;
            ViewBag.isPass = isPass;
            ViewBag.label = label;
            return View(model);
        }

        public async Task<ActionResult> Problem(string id="")
        {
            var problem =await _iChallengeQuestionService.GetProblem(id.TryParseGuid());
            ViewBag.allowPasteCode = BaseSiteSetDto.allowPasteCode;
            return View(problem);
        }
        public async Task<ActionResult> Rank(string id = "", int currentPage = 1)
        {
            ViewBag.id = id;
            var rank = await _iChallengeQuestionService.GetRankPagination(new RankInputDto{id= id ,page = currentPage,skip = (currentPage-1)*5,pageSize = 5});
            var challengeQuestion = _iChallengeQuestionRep.FirstOrDefault(id.TryParseGuid());
            ViewBag.title = challengeQuestion==null?"":challengeQuestion.title;
            return View(rank);
        }

        public async Task<ActionResult> Submissions(string id = "",int currentPage = 1)
        {
            var challengeQuestion = _iChallengeQuestionRep.FirstOrDefault(id.TryParseGuid());
            ViewBag.title = challengeQuestion == null ? "" : challengeQuestion.title;
            ViewBag.id = id;
            var submission = await _iChallengeQuestionService.GetSubmissionPagination(new RankInputDto { id = id, page = currentPage, skip = (currentPage - 1) * 10, pageSize = 10 });
            return View(submission);
        }
    }
}