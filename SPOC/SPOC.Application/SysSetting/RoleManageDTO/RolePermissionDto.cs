using System.Collections.Generic;

namespace SPOC.SysSetting.RoleManageDTO
{
    public class RolePermissionDto
    {
        public string id { get; set; }

        public string roleId { get; set; }

        public string menuId { get; set; }

    }
    /// <summary>
    /// 权限菜单
    /// </summary>
    public class RoleMenuModel
    {

        public string id { get; set; }
        public string code { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string state { get; set; }
        public string url { get; set; }

        public List<RoleMenuModel> children = new List<RoleMenuModel>();
    }
   
}
