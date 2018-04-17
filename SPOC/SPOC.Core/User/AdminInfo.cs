using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.User
{
    public class AdminInfo : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        public Guid userId { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        [StringLength(255)]
        public string adminInviteCode { get; set; }
        

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime recentLoginTime { get; set; }

        /// <summary>
        /// 最近登录IP地址
        /// </summary>
         [StringLength(50)]
        public string recentLoginIpAddress { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool isDel { get; set; }


        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool adminEnbleFlag { get; set; }
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool isAdmin { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }
        
    }
}
