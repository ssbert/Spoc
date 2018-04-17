using Abp.Application.Services;
using Abp.Web.Models;
using SPOC.Common.EasyUI;
using SPOC.Common.Pagination;
using SPOC.Core.Dto;
using System;
using System.Threading.Tasks;

namespace SPOC.Core
{
    /// <summary>
    /// 公告服务接口
    /// </summary>
    public interface IAnnouncementService : IApplicationService
    {
        /// <summary>
        /// 公告管理列表
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [DontWrapResult]
        EasyUiListResultDto<AnnouncementDto> GetAnnouncementList(AnnouncementInputDto inputDto);
        /// <summary>
        /// 新增公告
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task InsertAnnouncement(AnnouncementInputDto input);
        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task ModifyAnnouncement(AnnouncementInputDto input);
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteAnnouncement(BatchRequestInput input);
        /// <summary>
        /// 获取公告详细信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AnnouncementDto GetAnnouncement(AnnouncementInputDto input);
        #region 前台
        /// <summary>
        /// 获取我的公告
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        MyAnnouncementViewDto GetMyAnnouncement(PaginationInputDto input);
        /// <summary>
        /// 公告详细信息(前台展示)
        /// </summary>
        /// <param name="id">公告ID</param>
        /// <returns></returns>
        AnnouncementFrontViewDto AnnouncementDetail(Guid id);
        #endregion
    }
}
