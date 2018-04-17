using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using SPOC.Common.EasyUI;
using SPOC.Common.Pagination;
using SPOC.Faqs.Dtos;
using SPOC.SystemSet;

namespace SPOC.Faqs
{
    /// <summary>
    /// Faq应用层服务的接口方法
    /// </summary>
    public interface IFaqAppService : IApplicationService
    {
        /// <summary>
        /// 获取Faq的分页列表信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<EasyUiListResultDto<FaqListDto>> GetPagedFaqs(FaqInputDto input);

        /// <summary>
        /// 通过指定id获取FaqListDto信息
        /// </summary>
        Task<FaqItemDto> GetFaqByIdAsync(EntityDto<Guid> input);

     
        /// <summary>
        /// MPA版本才会用到的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GetFaqForEditOutput> GetFaqForEdit(NullableIdDto<Guid> input);

        //todo:缺少Dto的生成GetFaqForEditOutput
        /// <summary>
        /// 添加或者修改Faq的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateOrUpdateFaq(FaqEditDto input);

        /// <summary>
        /// 批量删除Faq
        /// </summary>
        Task BatchDeleteFaqsAsync(BatchRequestInput input);

        #region 首页前端接口

        /// <summary>
        /// 获取FAQ导航分类
        /// </summary>
        /// <returns></returns>
        Task<List<FaqFolderDto>> GetFaqFolder();
        /// <summary>
        /// 前端FAQ分页显示
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        Task<PaginationOutputDto<FaqItemDto>> GetPagination(FaqInputDto input);

        #endregion
    }
}
