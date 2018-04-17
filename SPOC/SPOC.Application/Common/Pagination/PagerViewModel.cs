namespace SPOC.Common.Pagination
{
    public class PagerViewModel
    {
        /// <summary>
        /// 当前页
        /// </summary>
        private int _CurrentPage = 0;
        public int CurrentPage
        {
            get { return _CurrentPage; }
            set { _CurrentPage = value; }
        }

        /// <summary>
        /// 总条数
        /// </summary>
        private int _Total = 0;
        public int Total
        {
            get { return _Total; }
            set { _Total = value; }
        }

        /// <summary>
        /// 每页的数目
        /// </summary>
        private int _PageSize = 0;
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }

        /// <summary>
        /// 总页数
        /// </summary>
        private int _SumPage = 0;
        public int SumPage
        {
            get 
            {
                _SumPage = Total % PageSize > 0 ? (Total / PageSize) + 1 : Total / PageSize;
                return _SumPage;
            }
        }
    }
}
