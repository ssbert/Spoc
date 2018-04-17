using Abp.Application.Services;
using SPOC.Category.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.Category
{
    /// <summary>
    /// 分类类型
    /// </summary>
    public interface INvFolderTypeService:IApplicationService
    {
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<NvFolderTypePaginationOutputDto> GetPagination(NvFolderTypePaginationInputDto input);
        /// <summary>
        /// 获取分类类型列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<NvFolderTypeDto>> GetAll(NvFolderQueryInputDto input);
        /// <summary>
        /// 创建一个分类类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Create(NvFolderTypeDto input);
        /// <summary>
        /// 更新一个分类类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(NvFolderTypeDto input);
        /// <summary>
        /// 删除一组分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task Delete(string ids);
    }
}