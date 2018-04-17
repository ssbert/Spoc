using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using SPOC.Exam.Dto;
using System.Collections.Generic;
using System.Web.Http;
using SPOC.User.Dto.Department;
using SPOC.Exam.Dto.Judge;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试任务服务接口
    /// </summary>
    public interface IExamTaskService: IApplicationService
    {
        /// <summary>
        /// 获取一个考试任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<ExamTaskOutputDto> Get(Guid id);

        /// <summary>
        /// 按照分页获取考试列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExamTaskItem>> GetPagination(ExamTaskPaginationInputDto input);

        /// <summary>
        /// 创建考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExamTaskOutputDto> Create(ExamTaskInputDto input);

        /// <summary>
        /// 更新考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(ExamTaskInputDto input);

        /// <summary>
        /// 删除考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Delete(IdListInputDto input);

        /// <summary>
        /// 发布考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Publish(ExamTaskClassInputDto input);

        /// <summary>
        /// 取消发布考试任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Unpublish(ExamTaskClassInputDto input);

        /// <summary>
        /// 获取考试任务发布候选班级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<List<ClassOutDto>> GetCandidateClasses(Guid id);

        /// <summary>
        /// 获取考试任务已发布的班级Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<List<Guid>> GetClassIds(Guid id);

        /// <summary>
        /// 获取考试任务下所有的补考
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<List<RetestComboboxItem>> GetRetestList(Guid id);

        #region 评卷人设置

        /// <summary>
        /// 添加评卷人
        /// </summary>
        /// <param name="input"></param>
        Task AddJudgeUsers(AddJudgeInputDto input);

        /// <summary>
        /// 删除评卷人
        /// </summary>
        /// <param name="input"></param>
        Task DeleteJudgeUsers(IdListInputDto input);

        #endregion
    }
}