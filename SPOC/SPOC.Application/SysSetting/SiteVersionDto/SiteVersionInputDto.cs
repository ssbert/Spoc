using System;
using SPOC.Common.Dto;

namespace SPOC.SysSetting.SiteVersionDto
{
    public class SiteVersionInputDto : EasyuiDto
    {

        public Guid id { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 更新说明
        /// </summary>
        public string updateInfo { get; set; }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool isForceUpdate { get; set; }

        /// <summary>
        /// 平台
        /// </summary>
        public string platform { get; set; }


    }

    public class APPSiteVersionDto
    { 
         /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 更新说明
        /// </summary>
        public string updateInfo { get; set; }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public string forceUpdate { get; set; }
    }
    
}
