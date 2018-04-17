using System;

namespace SPOC.Common.Dto
{
    /// <summary>
    /// This DTO can be used as EasyUI query.
    /// </summary>
    [Serializable]
    public class EasyuiDto 
    {

      
        public string LearningPlatformId { get; set; }
        /// <summary>当前页</summary>
        public int CurrentPage { get; set; }
        /// <summary>如果PageSize > 0，则分页</summary>
        public int PageSize { get; set; }
        /// <summary>总记录数分页用</summary>
        public int RecordCount { get; set; }
        /// <summary>排序字段</summary>
        public string SortCloumnName { get; set; }
        /// <summary>Asc Or Desc</summary>
        public string SortOrder { get; set; }
        public int Skip { get { return PageSize * CurrentPage; } }
        /// <summary>
        /// 排序表达式
        /// </summary>
        public string OrderExpression { get { return SortCloumnName + " " + SortOrder; } }
    }
}
