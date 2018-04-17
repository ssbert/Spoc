using System.Collections.Generic;

namespace SPOC.User.Dto.Common
{
    public class JsonTree
    {
        /// <summary> 
        /// ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string text { get; set; }

        /// 子类
        /// </summary>
        public List<JsonTree> children { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>IsFoucus
        public string SortNumber { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsFoucus { get; set; }


        private string isAllowChecked = "true";//是否允许被选中
        public string IsAllowChecked { get { return isAllowChecked; } set { isAllowChecked = value; } }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse { get; set; }

        public string LpId { get; set; }

        public string Information { get; set; }

        public void setChildren(List<JsonTree> children)
        {
            this.children = children;
        }
    }

    public class JsonNavigationTree {
        /// <summary> 
        /// ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string text { get; set; }

        /// 子类
        /// </summary>
        public string url { get; set; }
         
    }
}
