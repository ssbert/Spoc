using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Abp.Domain.Entities;

namespace SPOC.SystemSet
{
    [Serializable]
    public class Menu : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 菜单编码
        /// </summary>
        [StringLength(100)]
        public string menuCode { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public int menuType { get; set; }

        /// <summary>
        /// 父编码
        /// </summary>
        [StringLength(100)]
        public string parentMenuCode { get; set; }


        public string parentMenuName { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        [StringLength(255)]
        public string menuUrl { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [StringLength(50)]
        public string menuName { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public int isActive { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int listOrder { get; set; }

        /// <summary>
        /// 是否有子菜单
        /// </summary>
        public int hasChild { get; set; }

        /// <summary>
        /// 是否为右侧菜单
        /// </summary>
        public int isRight { get; set; }

        /// <summary>
        /// 站点来源Id
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid relateuid { get; set; }

        public string menuIcon { get; set; }
    }

    /// <summary>
    /// 实体类menu 。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    [DataContract]
    public class menu
    {
        public menu()
        {

        }

        #region Model
        private string _menuurl;
        private string _menuname;
        private int _isactive;
        private int _listorder;
        private int _haschild;
        private int _isright;
        private int _menuType;
        private string _menuIcon;

        /// <summary>
        /// menuType
        /// </summary>
        [DataMember]
        public int MenuType
        {
            set { _menuType = value; }
            get { return _menuType; }
        }
        /// <summary>
        /// 菜单CODE
        /// </summary>
        [Key]
        [StringLength(128)]
        public string MenuCode { get; set; }
        /// <summary>
        /// 父级CODE
        /// </summary>
        [StringLength(128)]
        public string ParentMenuCode { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        [StringLength(256)]
        public string MenuUrl { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [StringLength(128)]
        public string MenuName
        {
            set { _menuname = value; }
            get { return _menuname; }
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int IsActive
        {
            set { _isactive = value; }
            get { return _isactive; }
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ListOrder
        {
            set { _listorder = value; }
            get { return _listorder; }
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int HasChild
        {
            set { _haschild = value; }
            get { return _haschild; }
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int IsRight
        {
            set { _isright = value; }
            get { return _isright; }
        }


        /// <summary>
        /// MenuIcon
        /// </summary>
        [DataMember]
        public string MenuIcon
        {
            set { _menuIcon = value; }
            get { return _menuIcon; }
        }
        #endregion Model

    }
}
