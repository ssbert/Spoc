using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Castle.Core.Logging;
using SPOC.Common;
using SPOC.Common.Cookie;
using SPOC.Common.EasyUI;
using SPOC.Common.Helper;
using SPOC.Common.Pagination;
using SPOC.Core.Dto;
using SPOC.User.Dto.Notification;

namespace SPOC.User
{
    /// <summary>
    /// 通知服务实现类
    /// </summary>
    public class NotificationService : ApplicationService, INotificationService
    {
        private readonly IRepository<Notification, Guid> _iNotificationRep;
        private readonly IRepository<NotificationClass, Guid> _iNotificationClassRep;
        private readonly IRepository<NotificationType, Guid> _iNotificationTypeRep;
        private readonly IRepository<RecordOfReadNotification, Guid> _iRecordRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        public ILogger _iLogger { get; set; }
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public NotificationService(IRepository<Notification, Guid> iNotificationRep, IRepository<NotificationClass, Guid> iNotificationClassRep,
            IRepository<TeacherInfo, Guid> iTeacherInfoRep, IRepository<RecordOfReadNotification, Guid> iRecordRep,
            IRepository<Class, Guid> iClassRep, IRepository<ClassStudent, Guid> iClassStudentRep, IRepository<UserBase, Guid> iUserBaseRep,
            IRepository<NotificationType, Guid> iNotificationTypeRep, ILogger iLogger)
        {
            _iNotificationRep = iNotificationRep;
            _iNotificationClassRep = iNotificationClassRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iRecordRep = iRecordRep;
            _iClassRep = iClassRep;
            _iClassStudentRep = iClassStudentRep;
            _iUserBaseRep = iUserBaseRep;
            _iNotificationTypeRep = iNotificationTypeRep;
            _iLogger = iLogger;
        }

        #endregion

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Send(NotificationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            #region 验证

            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            var type = await _iNotificationTypeRep.FirstOrDefaultAsync(a => a.Code == input.TypeCode);
            if (type == null)
            {
                throw new UserFriendlyException("无效的通知类型");
            }

            var classIds = await _iClassRep.GetAll()
                .Where(a => input.ClassIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();
            if (classIds.Count != input.ClassIds.Count)
            {
                throw new UserFriendlyException("有无效的班级");
            }

            #endregion

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                TypeId = type.Id,
                Content = input.Content,
                CreateTime = DateTime.Now,
                SendUserId = cookie.Id
            };
            await _iNotificationRep.InsertAsync(notification);

            foreach (var classId in input.ClassIds)
            {
                var notificationClass = new NotificationClass
                {
                    Id = notification.Id,
                    ClassId = classId
                };
                await _iNotificationClassRep.InsertAsync(notificationClass);
            }
        }

        /// <summary>
        /// 按分页获取通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<NotificationItem>> GetPagination(PaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            var query = from s in _iClassStudentRep.GetAll()
                        join ns in _iNotificationClassRep.GetAll() on s.ClassId equals ns.ClassId
                        join n in _iNotificationRep.GetAll() on ns.Id equals n.Id
                        join t in _iNotificationTypeRep.GetAll() on n.TypeId equals t.Id
                        join u in _iUserBaseRep.GetAll() on n.SendUserId equals u.Id
                        join r in _iRecordRep.GetAll() on new { n.Id, UserId = s.UserId } equals new { r.Id, r.UserId } into rTempTable
                        from rTemp in rTempTable.DefaultIfEmpty()
                        where s.UserId == cookie.Id
                        orderby n.CreateTime descending 
                        select new NotificationItem
                        {
                            Id = n.Id,
                            TypeCode = t.Code,
                            TypeName = t.Name,
                            Content = n.Content,
                            CreateTime = n.CreateTime,
                            SenderId = n.SendUserId,
                            SenderName = string.IsNullOrEmpty(u.userFullName) ? u.userLoginName : u.userFullName,
                            Read = rTemp != null
                        };

            return new PaginationOutputDto<NotificationItem>
            {
                rows = await query.Skip(input.skip).Take(input.pageSize).ToListAsync(),
                total = await query.CountAsync()
            };
        }

        /// <summary>
        /// 设置所有为已读
        /// </summary>
        /// <returns></returns>
        public async Task SetAllRead()
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            var notificationIdList = await (from s in _iClassStudentRep.GetAll()
                join ns in _iNotificationClassRep.GetAll() on s.ClassId equals ns.ClassId
                join n in _iNotificationRep.GetAll() on ns.Id equals n.Id
                join t in _iNotificationTypeRep.GetAll() on n.TypeId equals t.Id
                join u in _iUserBaseRep.GetAll() on n.SendUserId equals u.Id
                join r in _iRecordRep.GetAll() on new {n.Id, UserId = s.UserId} equals new {r.Id, r.UserId} into rTempTable
                from rTemp in rTempTable.DefaultIfEmpty()
                where s.UserId == cookie.Id && rTemp == null
                select n.Id).ToListAsync();

            foreach (var guid in notificationIdList)
            {
                await _iRecordRep.InsertAsync(new RecordOfReadNotification {Id = guid, UserId = cookie.Id});
            }
        }

        public async Task<NotificationView> GetNotificationView(PaginationInputDto input)
        {

            if (!LoginValidation.IsLogin())
            {
               
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            try
            {
                var cookie = CookieHelper.GetLoginInUserInfo();
                var uid = cookie.UserUid.TryParseGuid();
                var data = (from n in _iNotificationRep.GetAll()
                            join nc in _iNotificationClassRep.GetAll() on n.Id equals nc.Id
                            join t in _iNotificationTypeRep.GetAll() on n.TypeId equals t.Id
                            join stu in _iClassStudentRep.GetAll() on nc.ClassId equals stu.ClassId
                            join u in _iUserBaseRep.GetAll() on n.SendUserId equals u.Id
                            join r in _iRecordRep.GetAll() on new { n.Id, UserId = stu.UserId } equals new { r.Id, r.UserId } into rTempTable
                            from rTemp in rTempTable.DefaultIfEmpty()
                            where stu.UserId == uid
                            orderby n.CreateTime descending
                            select new NotificationViewItem
                            {
                                Id = n.Id,
                                TypeCode = t.Code,
                                TypeName = t.Name,
                                Content = n.Content,
                                CreateTime = n.CreateTime.ToString(),
                                SenderId = n.SendUserId,
                                SenderName = string.IsNullOrEmpty(u.userFullName) ? u.userLoginName : u.userFullName,
                                Read = rTemp != null
                            });
                if (string.IsNullOrWhiteSpace(input.OrderExpression))
                {
                    input.sort = "createTime";
                    input.order = "desc";
                }
                var list = await data.Where(d=>!d.Read).OrderBy(input.OrderExpression).Skip(input.skip).Take(input.pageSize).ToListAsync();
                list.ForEach(a =>
                {
                    a.CreateTime = DateTimeUtil.GetTimtSetDay(a.CreateTime);
                });
                return new NotificationView { NotificationList = list, SumPage = (data.Count() / input.pageSize) + 1, CurrentPage = input.page + 1 };
            }
            catch (Exception e)
            {
                _iLogger.Error(e.ToString);
                throw new UserFriendlyException(e.ToString());

            }

        }

        public async Task SetNotificationIsRead(BatchRequestInput input)
        {
            if (!LoginValidation.IsLogin())
            {

                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            var cookie = CookieHelper.GetLoginInUserInfo();
            for (int i = 0; i < input.Id.Split(',').Length; i++)
            {
                if (string.IsNullOrEmpty((input.Id.Split(',')[i])))
                    continue;
                var uid = new Guid(input.Id.Split(',')[i]);
               if(!_iRecordRep.GetAll().Any(a=> a.Id.Equals(uid) && a.UserId.Equals(cookie.Id)))
                await _iRecordRep.InsertAsync(new RecordOfReadNotification {Id = uid, UserId = cookie.Id});
            }
        }
    }
}