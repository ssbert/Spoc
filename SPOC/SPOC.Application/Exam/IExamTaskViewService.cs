using Abp.Application.Services;
using SPOC.Common.Pagination;
using SPOC.Exam.ViewDto;
using System;
using System.Threading.Tasks;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试任务视图服务接口
    /// </summary>
    public interface IExamTaskViewService:IApplicationService
    {
        /// <summary>
        /// 获取视图分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExamTaskViewItem>> GetPagination(PaginationInputDto input);

        /// <summary>
        /// 获取用户成绩列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="examTaskId"></param>
        /// <param name="examId"></param>
        /// <returns></returns>
        Task<SingleExamGradeViewOutputDto> GetExamGrade(Guid userId, Guid examTaskId, Guid examId);

        /// <summary>
        /// 获取考试任务基础信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ExamTaskBaseViewOutputDto> GetBase(Guid id);
    }
}