using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Application.Services;
using SPOC.Common.Pagination;
using SPOC.Exam.ViewDto;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试排行服务接口
    /// </summary>
    public interface IExamRankingViewService:IApplicationService
    {
        /// <summary>
        /// 考试排行分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExamRankingViewItem>> GetRankingPagination(RankingPaginationInputDto input);

        /// <summary>
        /// 获取某人考试排行榜
        /// </summary>
        /// <param name="examId"></param>
        /// <param name="userId"></param>
        /// <param name="examTypeCode"></param>
        /// <returns></returns>
        Task<ExamRankingViewItem> GetRanking(Guid examId, Guid userId, string examTypeCode);
        /// <summary>
        /// 获取用户最近的考试列表(正考 不含补考)
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<System.Web.Mvc.SelectListItem>> RecentExamList();
        /// <summary>
        /// 获取当前用户所在班级指定考试的排名统计报表信息
        /// </summary>
        /// <param name="examId">考试ID</param>
        /// <returns></returns>
        [HttpGet]
        Task<LeaderboardViewDto> LeaderboardView(string examId);
    }
}