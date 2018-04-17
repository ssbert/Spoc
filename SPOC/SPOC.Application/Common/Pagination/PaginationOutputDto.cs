using System.Collections.Generic;

namespace SPOC.Common.Pagination
{
    public class PaginationOutputDto<T>
    {
        public PaginationOutputDto()
        {
            rows = new List<T>();
        }
        public List<T> rows { get; set; }
        public int total { get; set; }
    }
}