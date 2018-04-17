using System;
using System.Collections.Generic;
using Abp.Application.Services;
using SPOC.Common.Pagination;
using SPOC.Lib.Dto;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Runtime.Validation;
using SPOC.Common.Dto;

namespace SPOC.Lib
{
    /// <summary>
    /// 知识库标签接口
    /// </summary>
    public  interface ILibLabelService : IApplicationService
    {
        /// <summary>
        /// 根据分页获取数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<LabelPaginationItem>> GetPagination(LabelPaginationInputDto input);

        /// <summary>
        /// 获取知识库标签Combobox数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        Task<List<ComboboxItem>> GetComboboxList();

        /// <summary>
        /// 根据ID串删除试卷
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task Delete(string ids);
        /// <summary>
        /// 获取标签信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        Task<LabelDto> Get(string id);
        /// <summary>
        /// 新增编辑标签
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        Task<Guid> CreateOrUpdate(LabelDto input);
        /// <summary>
        /// 选择控件加载标签值 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        Task<dynamic> LoadLabelForChoose();

        /// <summary>
        /// 搜索文字 根据规则匹配标签
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <summary>
        /// 创建用户作答记录并更新标签积分
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="questionId">试题id</param>
        /// <param name="recordId">作答详情id</param>
        /// <param name="source">来源(exam, exercise, challenge)</param>
        /// <param name="pass">是否通过</param>
        /// <returns></returns>
        Task CreateUserAnswerRecords(Guid userId, Guid questionId, Guid recordId, string source, bool pass);

        [DisableValidation]
        Task<dynamic> SmartSeachLabel(MatchTextInputDto input);

    }
}
