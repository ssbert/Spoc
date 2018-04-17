/********************************************************************************
** auth： bert
** date： 2016/5/23 15:48:22
** desc： 
*********************************************************************************/

using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using SPOC.SystemSet;

namespace SPOC.SysSetting.CloudDTO
{
    [AutoMapFrom(typeof(Cloud))]
    public class CloudDto : EntityDto<Guid>
    {
        /// <summary>
        /// 域名
        /// </summary>      
        public string Host { get; set; }
        /// <summary>
        /// 机构编码 建议用院系代码 可为空
        /// </summary>      
        public string PartnerCode { get; set; }
        /// <summary>
        /// 机构|伙伴名称
        /// </summary>      
        public string PartnerName { get; set; }
        /// <summary>
        /// 所在省份
        /// </summary> 
        public int Province { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>    
        public int City { get; set; }
     
        /// <summary>
        /// 访问key
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        /// 密钥
        /// </summary> 
        public string SecretKey { get; set; }

        /// <summary>
        /// 直播服务状态
        /// </summary>
        public byte LiveServiceStatus { get; set; }
        /// <summary>
        /// 直播服务结束日期
        /// </summary>
        public DateTime LiveServiceEndDate { get; set; }
    }
}
