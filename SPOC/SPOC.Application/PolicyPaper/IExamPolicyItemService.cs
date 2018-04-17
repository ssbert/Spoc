using Abp.Application.Services;
using SPOC.PolicyPaper.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.PolicyPaper
{
    /// <summary>
    /// 随机试卷策略项服务
    /// </summary>
    public interface IExamPolicyItemService:IApplicationService
    {
        /// <summary>
        /// 根据大题ID获取策略项组
        /// </summary>
        /// <param name="policyNodeUid"></param>
        /// <returns></returns>
        [HttpGet]
        Task<List<ExamPolicyItemOutputDto>> GetList(Guid policyNodeUid);
        /// <summary>
        /// 创建策略项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExamPolicyItemOutputDto> Create(ExamPolicyItemInputDto input);
        /// <summary>
        /// 更新策略项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(ExamPolicyItemInputDto input);
        /// <summary>
        /// 删除策略项
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        Task Delete(string ids);
        /// <summary>
        /// 根据查询项获取试卷所有策略项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<ExamPolicyItemItem>> GetAllList(ExamPolicyItemListQueryInputDto input);
    }
}