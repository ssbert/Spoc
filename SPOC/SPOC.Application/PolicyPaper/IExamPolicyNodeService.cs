using Abp.Application.Services;
using SPOC.Common.Pagination;
using SPOC.PolicyPaper.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.PolicyPaper
{
    /// <summary>
    /// 随机试卷大题服务
    /// </summary>
    public interface IExamPolicyNodeService:IApplicationService
    {
        /// <summary>
        /// 新增一个大题
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExamPolicyNodeOutputDto> Create(ExamPolicyNodeInputDto input);
        /// <summary>
        /// 更新一个大题
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(ExamPolicyNodeInputDto input);
        /// <summary>
        /// 根据id串删除对应的大题
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        Task Delete(string ids);
        /// <summary>
        /// 获取一个大题数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<ExamPolicyNodeOutputDto> Get(Guid id);
        /// <summary>
        /// 获取大题分页表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExamPolicyNodeOutputDto>> GetPagination(ExamPolicyNodePaginationInputDto input);

        /// <summary>
        /// 获取所有大题数据
        /// </summary>
        /// <param name="policyId"></param>
        /// <returns></returns>
        [HttpGet]
        Task<List<ExamPolicyNodeOutputDto>> GetList(Guid policyId);
    }
}