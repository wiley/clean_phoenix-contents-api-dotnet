using System.Collections.Generic;

namespace Contents.Domain.Pagination
{
    public class PageRequest
    {
        public int PageOffset { get; set; }
        public int PageSize { get; set; }
        public List<Filter> Filters { get; set; }
        public string SortField { get; set; }
        public EnumSortOrder SortOrder { get; set; }
    }
}