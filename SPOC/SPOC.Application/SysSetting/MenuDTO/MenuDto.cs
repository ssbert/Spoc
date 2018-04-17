using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using SPOC.SystemSet;

namespace SPOC.SysSetting.MenuDTO
{
    [AutoMapFrom(typeof(Menu))]
    public class MenuDto
    {
        /// <summary>
        /// 菜单编码
        /// </summary>
        [StringLength(100)]
        public string menuCode { get; set; }
        public string id { get; set; }

        public string menuName { get; set; }

        public string isActive { get; set; }

        public int listOrder { get; set; }

        public string pidName { get; set; }
        
        public string pid { get; set; }

        public string text { get; set; }

        public string menuUrl { get; set; }

        public List<MenuDto> children { get; set; }

        public string menuIcon { get; set; }

        /// <summary>
        /// 是否有子菜单
        /// </summary>
        public int hasChild { get; set; }
        /// <summary>
        /// 是否为右侧菜单
        /// </summary>
        public int isRight { get; set; }

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


    }

    public class MenuTreeDto
    {
       
        public string code { get; set; }
        public string parentCode { get; set; }
        public string text { get; set; }
        public string url { get; set; }
      




    }
    public static class MenuDtoExt {

        public static MenuSetDto GetMenuSetDto(this MenuDto model) {

            return new MenuSetDto() {
           //  ChildMenu=model.children,
             HasChild=model.hasChild,
             IsActive = string.IsNullOrEmpty(model.isActive) ? 0 : int.Parse(model.isActive),
               IsRight=model.isRight,
                ListOrder=model.listOrder,
                 MenuCode=model.menuCode,
                  MenuIcon=model.menuIcon,
                   MenuName=model.menuName,
                    MenuType=model.menuType,
                     MenuUrl=model.menuUrl,
                      ParentMenuCode= model.parentMenuCode,
                       ParentMenuName =model.parentMenuName
              
            };
        }
    }

    public class MenuSetDto {

        public MenuSetDto()
        {
            ChildMenu = new List<MenuSetDto>();
        
        } 

        /// <summary>
        /// menuType
        /// </summary>
        public int MenuType
        {
            get;
            set;
        }
        /// <summary>
        /// 菜单CODE
        /// </summary>
        public string MenuCode { get; set; }
        /// <summary>
        /// 父级CODE
        /// </summary>
        public string ParentMenuCode { get; set; }

        /// <summary>
        /// 父级CODE
        /// </summary>
        public string ParentMenuName { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        public string MenuUrl { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int IsActive
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int ListOrder
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int HasChild
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int IsRight
        {
            get;
            set;
        }


        /// <summary>
        /// MenuIcon
        /// </summary>
        public string MenuIcon
        {
            get;
            set;
        }


        public List<MenuSetDto> ChildMenu { get; set; }
       
    }

    //public static class MenuConvertToDTO
    //{
    //    public static MenuDto ToDTO(this Menu obj)
    //    {
    //        MenuDto dto = new MenuDto();
    //        if (obj == null)
    //        {
    //            return dto;
    //        }

    //        dto.id = obj.id.ToString();
    //        dto.isActive = obj.IsActive==0?"未启用":"启用";
    //        dto.listOrder = obj.ListOrder;
    //        dto.pidName = 
    //    }

    //    public static List<MenuDto> ToDTOList(this List<Menu> objList)
    //    {
 
    //    }
    //}
}
