using Abp.Application.Services;
using SPOC.PolicyPaper.Dto;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.PolicyPaper
{
    /// <summary>
    /// 随机试卷基本信息服务
    /// </summary>
    public interface IExamPolicyService:IApplicationService
    {
        /// <summary>
        /// 获取随机试卷基本信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<ExamPolicyOutputDto> Get(Guid id);
        /// <summary>
        /// 创建随机试卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExamPolicyOutputDto> Create(ExamPolicyInputDto input);
        /// <summary>
        /// 修改随机试卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(ExamPolicyInputDto input);
        /// <summary>
        /// 删除随机试卷
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        Task Delete(string ids);
    }
}