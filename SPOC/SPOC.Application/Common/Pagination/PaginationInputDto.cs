namespace SPOC.Common.Pagination
{
    /// <summary>
    /// 用于被继承，提供分页数据
    /// </summary>
    public class PaginationInputDto
    {
        /// <summary>
        /// 排序字段名称
        /// </summary>
        public string sort { get; set; }
        /// <summary>
        /// 排序方式 asc | desc
        /// </summary>
        public string order { get; set; }
        public int skip { get; set; }
        public int pageSize { get; set; }
        //rows<=>pageSize
        public int rows { get; set; }
        //页数
        public int page { get; set; }

        /// <summary>
        /// 排序表达式
        /// </summary>
        public string OrderExpression => sort + " " + order;
    }
}