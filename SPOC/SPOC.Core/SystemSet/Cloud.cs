/********************************************************************************
** auth： bert
** date： 2016/5/23 15:08:35
** desc： Cloud 高校云授权信息表
*********************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.SystemSet
{
    [Serializable]
    public class Cloud : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 机构编码 建议用院系代码 可为空
        /// </summary>
        [StringLength(32)]
        [Column("partnerCode")]
        public string PartnerCode { get; set; }
        /// <summary>
        /// 机构|伙伴名称
        /// </summary>
        [StringLength(128)]
        [Column("partnerName")]
        public string PartnerName { get; set; }
        /// <summary>
        /// 所在省份
        /// </summary>
        [Column("province")]
        public int Province { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>
        [Column("city")]
        public int City { get; set; }
        /// <summary>
        /// 云端编号 云端生成(机构ID）
        /// </summary>
        [Column("cloudId")]
        public int CloudId { get; set; }
        /// <summary>
        /// 访问key 云端生成
        /// </summary>
        [StringLength(36)]
        [Column("accessKey")]
        public string AccessKey { get; set; }
        /// <summary>
        /// 密钥 云端生成
        /// </summary>
         [StringLength(36)]
        [Column("secretKey")]
        public string SecretKey { get; set; }

         /// <summary>
         /// 直播服务状态
         /// </summary>
         [Column("liveServiceStatus")]
         public byte LiveServiceStatus { get; set; }
         /// <summary>
         /// 直播服务结束日期
         /// </summary>
         [Column("liveServiceEndDate")]
         public DateTime LiveServiceEndDate { get; set; }
    }
}
