using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using SPOC.Common.Pagination;
using SPOC.Exercises.Dto;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习排行榜服务接口
    /// </summary>
    public interface IExerciseRankingViewService:IApplicationService
    {
        /// <summary>
        /// 获取效率排行榜分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<EfficiencyRankingViewItem>> GetEfficiencyRankingPagination(ExerciseRankingPaginationInputDto input);

        /// <summary>
        /// 获取积极性排行榜分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<EnthusiasmRankingViewItem>> GetEnthusiasmRankingPagination(ExerciseRankingPaginationInputDto input);

        /// <summary>
        /// 获取某人效率排行榜
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<EfficiencyRankingViewItem> GetEfficiencyRanking(Guid exerciseId, Guid userId);

        /// <summary>
        /// 获取某人积极性排行榜
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<EnthusiasmRankingViewItem> GetEnthusiasmRanking(Guid exerciseId, Guid userId);
    }
}