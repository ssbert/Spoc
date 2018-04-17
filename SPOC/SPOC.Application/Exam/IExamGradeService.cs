using Abp.Application.Services;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using SPOC.Exam.GradeDto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.Exam
{
    public interface IExamGradeService:IApplicationService
    {
        /// <summary>
        /// 获取考试成绩分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExamGradeItem>> GetPagination(ExamGradePaginationInputDto input);

        /// <summary>
        /// 更新成绩信息
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        Task Update(List<UserExamGradeInputDto> inputList);

        /// <summary>
        /// 删除考试成绩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Delete(IdListInputDto input);

        /// <summary>
        /// 根据条件导出学生考试成绩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MemoryStream> ExportExamGrade(ExportGradePaginationInputDto input);

        /// <summary>
        /// 用户成绩个数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int UserExamGradeCount(Guid userId);

        /// <summary>
        /// 获取用户考试记录列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<UserExamRecordOutputDto>> GetUserExamRecordList(UserExamRecordInputDto input);
    }
}
