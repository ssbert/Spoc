using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using SPOC.Common.Dto;
using SPOC.Lib.Dto;
using System.Web.Http;
using Abp.Web.Models;

namespace SPOC.Lib
{
    /// <summary>
    /// 知识图谱服务类接口
    /// </summary>
    public interface IStructureMapService:IApplicationService
    {
        /// <summary>
        /// 获取单个知识图谱数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task<StructureMapDto> Get(Guid id);

        /// <summary>
        /// 获取所有知识图谱列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        Task<List<StructureMapItem>> GetList();

        /// <summary>
        /// 创建知识图谱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<StructureMapDto> Create(StructureMapDto input);

        /// <summary>
        /// 更新知识图谱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(StructureMapDto input);

        /// <summary>
        /// 更新知识图谱地图数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateData(StructureMapDataInputDto input);

        /// <summary>
        /// 删除知识图谱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Delete(IdListInputDto input);

        /// <summary>
        /// 设置为主图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        Task SetIsMain(Guid id);

        /// <summary>
        /// 更新设置是否显示
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateIsShow(KeyValuePair<Guid, bool> input);

        /// <summary>
        /// 获取主知识图谱数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, DontWrapResult]
        Task<string> GetMainMapData();
    }
}