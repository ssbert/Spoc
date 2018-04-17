using SPOC.Common.Cookie;
using SPOC.Common.Pagination;
using SPOC.Exercises;
using SPOC.Exercises.Dto;
using SPOC.Web.Filters;
using SPOC.Web.Models.Exercises;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SPOC.Common.Helper;
using SPOC.QuestionBank;

namespace SPOC.Web.Controllers
{
    public class ExerciseTaskController : SPOCControllerBase
    {
        private readonly IExerciseViewService _iExerciseViewService;
        private readonly IExerciseRankingViewService _iExerciseRankingViewService;
        private readonly IQuestionBankService _iQuestionBankService;
        public ExerciseTaskController(IExerciseViewService iExerciseViewService, IExerciseRankingViewService iExerciseRankingViewService, 
            IQuestionBankService iQuestionBankService)
        {
            _iExerciseViewService = iExerciseViewService;
            _iExerciseRankingViewService = iExerciseRankingViewService;
            _iQuestionBankService = iQuestionBankService;
        }

        //练习任务首页
        [UserAuthorization]
        public async Task<ActionResult> Index(int page = 1)
        {
            var pageSize = 10;
            page = page < 1 ? 1 : page;
            var pagination = await _iExerciseViewService.GetPagination(new PaginationInputDto
            {
                skip = (page - 1) * pageSize,
                pageSize = pageSize
            });
            ViewBag.currentPage = page;
            return View(pagination);
        }

        //效率排行
        [UserAuthorization]
        public async Task<ActionResult> EfficiencyRanking(Guid id, int page = 1)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var pageSize = 10;
            page = page < 1 ? 1 : page;
            var model = new RankingViewModel<EfficiencyRankingViewItem>
            {
                Base = await _iExerciseViewService.GetBase(id),
                Pagination = await _iExerciseRankingViewService.GetEfficiencyRankingPagination(
                    new ExerciseRankingPaginationInputDto
                    {
                        ExerciseId = id,
                        skip = (page - 1) * pageSize,
                        pageSize = pageSize
                    }),
                SelfRanking = await _iExerciseRankingViewService.GetEfficiencyRanking(id, cookie.Id)
            };
            ViewBag.currentPage = page;
            return View(model);
        }

        //积极性排行
        [UserAuthorization]
        public async Task<ActionResult> EnthusiasmRanking(Guid id, int page = 1)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var pageSize = 10;
            page = page < 1 ? 1 : page;
            var model = new RankingViewModel<EnthusiasmRankingViewItem>
            {
                Base = await _iExerciseViewService.GetBase(id),
                Pagination = await _iExerciseRankingViewService.GetEnthusiasmRankingPagination(
                    new ExerciseRankingPaginationInputDto
                    {
                        ExerciseId = id,
                        skip = (page - 1) * pageSize,
                        pageSize = pageSize
                    }),
                SelfRanking = await _iExerciseRankingViewService.GetEnthusiasmRanking(id, cookie.Id)
            };
            ViewBag.currentPage = page;
            return View(model);
        }

        //练习
        [UserAuthorization]
        public async Task<ActionResult> Exercise(Guid id)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            
            if (!await _iExerciseViewService.CheckUserAuthorization(id, cookie.Id))
            {
                return RedirectToAction("Index");
            }

            var model = new ExerciseViewModel
            {
                Base = await _iExerciseViewService.GetBase(id),
                AllowPasteCode = BaseSiteSetDto.allowPasteCode == "true"
            };

            model.Question = await _iQuestionBankService.Get(model.Base.QuestionId);

            await _iExerciseViewService.StartExercise(id, cookie.Id);

            return View(model);
        }

        //练习记录
        [UserAuthorization]
        public async Task<ActionResult> Record(Guid id, string recordId = "")
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            var model = new RecordViewModel
            {
                Base = await _iExerciseViewService.GetBase(id),
                RecordList = await _iExerciseViewService.GetExerciseRecordList(id, cookie.Id)
            };
            if (!string.IsNullOrWhiteSpace(recordId))
            {
                var recordGuid = recordId.TryParseGuid();
                model.Answer = await _iExerciseViewService.GetUserExerciseAnswer(recordGuid);
            }
            return View(model);
        }

        //练习答案
        [UserAuthorization]
        public async Task<ActionResult> Answer(Guid id)
        {
            var baseData = await _iExerciseViewService.GetBase(id);
            if (!baseData.ShowAnswer || (baseData.ShowAnswerType == 0 &&
                                         (!baseData.EndTime.HasValue || baseData.EndTime.Value > DateTime.Now)))
            {
                return RedirectToAction("Index");
            }
            var model = new AnswerViewModel
            {
                Base = baseData,
                Answer = await _iExerciseViewService.GetExerciseAnswer(id)
            };
            return View(model);
        }
    }
}