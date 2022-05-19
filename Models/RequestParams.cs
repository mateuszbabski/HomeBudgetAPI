namespace HomeBudget.Models
{
    public class RequestParams
    {
        public string SearchPhrase { get; set; }

        const int maxPageSize = 25;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }

        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
