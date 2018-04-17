using SPOC.Lib;
using SPOC.Web.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Lib.Dto;
using SPOC.Web.Models.LibLabel;

namespace SPOC.Web.Controllers
{
    public class LibLabelViewController : Controller
    {
        private readonly ILibLabelViewService _iLibLabelViewService;

        public LibLabelViewController(ILibLabelViewService iLibLabelViewService)
        {
            _iLibLabelViewService = iLibLabelViewService;
        }

        [UserAuthorization]
        public async Task<ActionResult> Index(Guid id, int page = 1, string type = "", string source = "", string title = "", string text = "", int status = 0)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            var result = await _iLibLabelViewService.GetUserAnswerRecordsPagination(new UserAnswerRecordsPaginationInput
            {
                LabelId = id,
                UserId = cookie.Id,
                Title = title,
                QuestionText = text,
                QuestionBaseTypeCode = type,
                Source = source,
                Status = status,
                skip = (page - 1) * 10,
                pageSize = 10
            });

            var model = new LibLabelViewModel
            {
                Id = id,
                Page = page,
                Title = title,
                Type = type,
                Source = source,
                Text = text,
                Status = status,
                Total = result.total,
                Rows = result.rows
            };
            
            return View(model);
        }

        [UserAuthorization]
        public async Task<ActionResult> UserAnswerQuestion(Guid id)
        {
            var model = await _iLibLabelViewService.GetUserAnswerRecordsQuestion(id);
            return View(model);
        }
    }
}