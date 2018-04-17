using Abp.Application.Services;
using SPOC.Category.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.Category
{
    public interface INvFolderService : IApplicationService
    {
        /// <summary>
        /// 根据code获取分类数据
        /// </summary>
        /// <param name="folderTypeCode"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task<List<NvFolderItemOutputDto>> Get(string folderTypeCode);

        /// <summary>
        /// 创建新的分类节点
        /// </summary>
        /// <param name="input"></param>
        Task Create(NvFolderInputDto input);
        /// <summary>
        /// 更新一个节点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(NvFolderInputDto input);
        /// <summary>
        /// 根据ID删除节点
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task Delete(string ids);
        /// <summary>
        /// 根据分页获取分类数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<NvFolderPaginationOutputDto> GetPagination(NvFolderPaginationInputDto input);

        /// <summary>
        /// 获取子节点的直系父节点
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        Task<List<NvFolder>> GetAllParent(Guid childId);
    }
}