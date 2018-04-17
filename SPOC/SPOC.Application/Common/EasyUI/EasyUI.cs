using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SPOC.Common.EasyUI
{
    public class EasyUIQueryGrid<T>
    {
        public long total { get; set; }
        public IQueryable<T> rows { get; set; }
    }

    public class EasyUIListGrid<T>
    {
        public long total { get; set; }
        public List<T> rows { get; set; }
    }

    public class EasyUIDataTableGrid
    {
        public long total { get; set; }
        public DataTable rows { get; set; }
    }

    public class EasyUIToolBar
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string IconCls { get; set; }

        public string Handler { get; set; }
    }

    public class EasyUICombo
    {
        public string id { get; set; }
        public string text { get; set; }
        public string iconCls { get; set; }
        public string state { get; set; }
        public bool @checked { get; set; }
        public List<EasyUICombo> children = new List<EasyUICombo>();
    }

    public class EasyUIPager
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SortCloumnName { get; set; }
        public string SortOrder { get; set; }
        public string Filter { get; set; }
        /// <summary>
        /// 排序表达式
        /// </summary>
        public string OrderExpression { get { return SortCloumnName + " " + SortOrder; } }
    }
}
