using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Application.Services;
using SPOC.Common.Pagination;
using SPOC.Exercises.Dto;
using SPOC.User.Dto.Department;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习管理服务接口
    /// </summary>
    public interface IExerciseManageService:IApplicationService
    {
        /// <summary>
        /// 获取一个练习
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<ExerciseOutputDto> Get(Guid id);

        /// <summary>
        /// 获取练习分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExerciseItem>> GetPagination(ExercisePaginationInputDto input);

        /// <summary>
        /// 创建一个练习
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExerciseOutputDto> Create(ExerciseInputDto input);

        /// <summary>
        /// 更新一个练习
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(ExerciseInputDto input);

        /// <summary>
        /// 删除一个练习
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task Delete(Guid id);

        /// <summary>
        /// 发布练习
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Publish(ExerciseClassInputDto input);

        /// <summary>
        /// 取消发布考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Unpublish(ExerciseClassInputDto input);

        /// <summary>
        /// 获取练习发布候选班级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<List<ClassOutDto>> GetCandidateClasses(Guid id);

        /// <summary>
        /// 获取练习已发布的班级Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<List<Guid>> GetClassIds(Guid id);
    }
}