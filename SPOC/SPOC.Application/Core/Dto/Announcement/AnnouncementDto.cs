using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SPOC.Core.Dto
{
    public class AnnouncementDto
    {
        public Guid id { get; set; }

        public string content { get; set; }

        public string title { get; set; }

        public int enable { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime updateTime { get; set; }
        public Guid userId { get; set; }

        public string userName { get; set; }

        /// <summary>
        /// 班级IDs
        /// </summary>
        public string classIds { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string classNames { get; set; }
        /// <summary>
        /// 附件id
        /// </summary>
        public Guid fileId { get; set; }
        /// <summary>
        /// 附件名
        /// </summary>
        public string fileName { get; set; }
    }
    public class MyAnnouncementDto
    {
        public Guid id { get; set; }

        public string content { get; set; }
        public string createUser { get; set; }
        public string userHeadImg { get; set; }
        public string title { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime createTime { get; set; }
    }
    /// <summary>
    /// 我的公告列表
    /// </summary>
    public class MyAnnouncementViewDto
    {
        /// <summary>
        /// 公告列表
        /// </summary>
        public List<MyAnnouncementDto> MyAnnouncementList { get; set; } = new List<MyAnnouncementDto>();

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; } = 0;
        /// <summary>
        /// 总页数
        /// </summary>
        public int SumPage { get; set; } = 0;
        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; } = 0;
    }
    /// <summary>
    /// 公告前台详细页
    /// </summary>
    public class AnnouncementFrontViewDto
    {
        public Guid id { get; set; }

        public string content { get; set; }

        public string title { get; set; }

        public int enable { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime updateTime { get; set; }
        public Guid userId { get; set; }

        public string userName { get; set; }

        public string fileName { get; set; }
        public Guid fileId { get; set; }
    }
}
