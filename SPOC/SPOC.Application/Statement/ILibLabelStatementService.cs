using Abp.Application.Services;
using SPOC.Common.Pagination;
using SPOC.Statement.Dto.Lib;
using System.Threading.Tasks;

namespace SPOC.Statement
{
    /// <summary>
    /// 知识点统计报表
    /// </summary>
    public interface ILibLabelStatementService : IApplicationService
    {
        /// <summary>
        /// 班级标签掌握结果报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ClassLabelGettingItem>> ClassLabelGettingList(ClassLabelGettingInputDto input);

        /// <summary>
        /// 班级标签掌握结果对比表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ClassContrastChartDto> ClassContrast(ClassLabelGettingInputDto input);


        /// <summary>
        /// 某个标签班级下所有学员掌握情况
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<UserLabelGettingItem>> UserLabelGettingList(UserLabelGettingInputDto input);

        

        /// <summary>
        /// 学生标签统计报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<StudentLabelGettingItem>> StudentLabelGettingList(StudentLabelGettingInputDto input);

        /// <summary>
        /// 学生标签掌握熟练度查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<StudentLabelStatementItem>> StudentLabelStatementPagination(
            StudentLabelStatementInputDto input);
    }
}
