using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using SPOC.Statement.Dto.Exercise;
using System.Web.Http;
using SPOC.Exercises.Dto;

namespace SPOC.Statement
{
    /// <summary>
    /// 练习报表服务接口
    /// </summary>
    public interface IExerciseStatementService : IApplicationService
    {
        /// <summary>
        /// 获取练习报表分页数据
        /// </summary>
        /// <returns></returns>
        Task<PaginationOutputDto<ExerciseStatementItem>> GetPagination(ExerciseStatementPaginationInputDto input);

        /// <summary>
        /// 获取学生练习相关信息报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExerciseStudentStatementItem>> GetStudentPagination(
            ExerciseStudentStatementPaginationInputDto input);

        /// <summary>
        /// 获取某学生练习记录报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExerciseRecordItem>> GetExerciseRecordPagination(
            ExerciseRecordStatementPaginationInputDto input);

        /// <summary>
        /// 获取某学生练习的某一次作答数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ExerciseAnswerOutputDto> GetExerciseAnswer(Guid id);

        /// <summary>
        /// 获取某练习的效率排行榜
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExerciseEfficiencyRankingItem>> GetEfficiencyRankingPagination(
            ExerciseRankingStatementPaginationInputDto input);

        /// <summary>
        /// 获取某练习的积极性排行榜
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExerciseEnthusiasmRankingItem>> GetEnthusiasmRankingPagination(
            ExerciseRankingStatementPaginationInputDto input);

        /// <summary>
        /// 获取某联系的班级排行榜
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<ExerciseClassRankingItem>> GetClassRankingList(ExerciseClassRankingQueryInputDto input);

        /// <summary>
        /// 考试相关班级树
        /// </summary>
        /// <param name="id">考试任务ID</param>
        /// <returns></returns>
        [HttpGet]
        Task<List<SelectListItemDto>> GetExerciseClassTree(Guid id);
    }
}