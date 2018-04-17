using System;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Application.Services;
using SPOC.Common.EasyUI;
using SPOC.Common.Pagination;
using SPOC.User.Dto.Notification;

namespace SPOC.User
{
    /// <summary>
    /// 通知服务接口
    /// </summary>
    public interface INotificationService:IApplicationService
    {
        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Send(NotificationInputDto input);

        /// <summary>
        /// 按分页获取通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<NotificationItem>> GetPagination(PaginationInputDto input);
        /// <summary>
        /// 设置所有已读
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        Task SetAllRead();
        /// <summary>
        /// 首页获取通知列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<NotificationView> GetNotificationView(PaginationInputDto input);
        /// <summary>
        /// 设置通知已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task SetNotificationIsRead(BatchRequestInput input);
    }
}