using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using SPOC.Statement.Dto;
using System.Web.Http;

namespace SPOC.Statement
{
    /// <summary>
    /// 考试报表服务
    /// </summary>
    public interface IExamTaskStatementService:IApplicationService
    {
        /// <summary>
        /// 获取考试报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExamTaskStatementItem>> GetPagination(ExamTaskStatementPaginationInputDto input);

        /// <summary>
        /// 获取考试成绩报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<GradeRankingItem>> GetGradePagination(GradeRankingPaginationInputDto input);

        /// <summary>
        /// 获取班级成绩报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<ClassRankingItem>> GetClassRankingList(ClassRankingQueryInputDto input);

        /// <summary>
        /// 获取补考成绩报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<GradeRankingItem>> GetRetestPagination(RetestRankPaginationInputDto input);

        /// <summary>
        /// 考试相关班级树
        /// </summary>
        /// <param name="id">考试任务ID</param>
        /// <returns></returns>
        [HttpGet]
        Task<List<SelectListItemDto>> GetExamTaskClassTree(Guid id);

        /// <summary>
        /// 获取学生考试相关信息报表分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExamTaskStudentStatementItem>> GetStudentPagination(
            ExamTaskStudentStatementPaginationInputDto input);
    }
}