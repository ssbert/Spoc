/********************************************************************************
** auth： bert
** date： 2016/5/23 16:37:29
** desc： 
*********************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.SystemSet
{
    [Table("city_area")]
    public class CityArea : Entity
    {
        /// <summary>
        /// 父节点ID
        /// </summary> 
        [Column("parentId")]
        public int ParentId { get; set; }

         /// <summary>
        /// 城市英文
        /// </summary> 
        [Column("eCity")]
        [StringLength(32)]
        public string ECity { get; set; }
        /// <summary>
        /// 城市
        /// </summary> 
        [Column("city")]
        [StringLength(32)]
        public string City { get; set; }

        [Column("orderId")]
        public int OrderId { get; set; }
        
    }
}
