using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using SPOC.Common.Pagination;
using SPOC.Core.Dto.CodeCompile;
using SPOC.Exercises.Dto;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习视图服务接口
    /// </summary>
    public interface IExerciseViewService:IApplicationService
    {
        /// <summary>
        /// 获取练习基础信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ExerciseBaseViewOutputDto> GetBase(Guid id);
            /// <summary>
        /// 练习列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExerciseViewItem>> GetPagination(PaginationInputDto input);

        /// <summary>
        /// 运行代码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExerciseRunCodeOutputDto> RunCode(ExerciseCodeInputDto input);

        /// <summary>
        /// 检测用户使用权限
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> CheckUserAuthorization(Guid exerciseId, Guid userId);

        /// <summary>
        /// 开始练习
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task StartExercise(Guid exerciseId, Guid userId);

        /// <summary>
        /// 获取某人练习记录
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ExerciseRecordItem>> GetExerciseRecordList(Guid exerciseId, Guid userId);

        /// <summary>
        /// 获取某人练习的提交答案
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        Task<string> GetUserExerciseAnswer(Guid recordId);

        /// <summary>
        /// 获取练习参考答案
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> GetExerciseAnswer(Guid id);
    }
}