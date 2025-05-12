using Umbraco.Cms.Web.Common.PublishedModels;

namespace EventPortal.SearchModel
{
    public class SearchResultsModel
    {
        public string SearchQuery { get; set; } = null!;
        public IEnumerable<EventViewModel> Results { get; set; } = null!;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
