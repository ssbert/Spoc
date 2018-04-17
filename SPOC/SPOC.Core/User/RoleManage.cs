using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.User
{
    [Serializable]
    public class RoleManage : Entity<Guid>
    {

        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [StringLength(50)]
        public string roleName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(500)]
        public string description { get; set; }

        /// <summary>
        /// 权限依据编码
        /// </summary>
        [StringLength(255)]
        public string rolecode { get; set; }
        /// <summary>
        /// 分组
        /// </summary>
        [StringLength(10)]
        public string roleGroup { get; set; }
        /// <summary>
        /// 是否初始化默认列  默认数据不能删除(2017 01-06 需求默认添加管理员、代理商、教师权限数据)
        /// </summary>
        public bool isDefault { get; set; }

    }
    /// <summary>
    /// 角色菜单授权表
    /// </summary>
    [Serializable]
    public class RolePermission : Entity<Guid>
    {

        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid roleId{get;set;}
        /// <summary>
        /// 菜单ID
        /// </summary>
        public Guid menuId { get; set; }

      

    }
    /// <summary>
    /// 角色用户关联表
    /// </summary>
    [Serializable]
    public class UserRole : Entity<Guid>
    {

        [Column("id")]
        public sealed override Guid Id { get; set; }
        public UserRole()
        {
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
            }
        }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid userId { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid roleId { get; set; }

    }
}
