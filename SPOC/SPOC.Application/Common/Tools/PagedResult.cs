using System.Collections.Generic;

namespace SPOC.Common.Tools
{
    public class PagedResult<T,N>:Paged where T :class
    {
        public IList<T> result { get; set; }
        public IList<N> resultCondition { get; set; }
        public PagedResult()
        {
            result = new List<T>();
            resultCondition = new List<N>();
        }

    }

    public class PagedResult<T> : Paged where T : class
    {
        public IList<T> result { get; set; }
        public PagedResult()
        {
            result = new List<T>();
        }

    }
    /// <summary>
    /// 分页后
    /// </summary>
    public class Paged : Paging
    {
        public int SizeCount { get; set; }

    }
    /// <summary>
    /// 分页前
    /// </summary>
    public class Paging
    {

        public int page { get; set; }

        public int rows { get; set; }

        public string sort { get; set; }
        public string order { get; set; }

        public int pageIndex { get; set; }
        public int pageSize { get; set; }

        public string order_asc_desc { get; set; }
        public string sort_filed { get; set; }
    }
}
