using System;
using System.Collections.Generic;
using SPOC.Core.Dto;

namespace SPOC.User.Dto.Notification
{
    /// <summary>
    /// 通知记录项
    /// </summary>
    public class NotificationItem
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 类型编码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 类型名
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发送者名
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// 发送者Id
        /// </summary>
        public Guid SenderId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 已读
        /// </summary>
        public bool Read { get; set; }
    }
    /// <summary>
    /// 通知显示模型
    /// </summary>
    public class NotificationView
    {
        /// <summary>
        /// 通知列表
        /// </summary>
        public List<NotificationViewItem> NotificationList { get; set; } = new List<NotificationViewItem>();
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; } = 0;

        /// <summary>
        /// 总页数
        /// </summary>
        public int SumPage { get; set; } = 0;
    }
    public class NotificationViewItem
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 类型编码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 类型名
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发送者名
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// 发送者Id
        /// </summary>
        public Guid SenderId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 已读
        /// </summary>
        public bool Read { get; set; }
    }
}