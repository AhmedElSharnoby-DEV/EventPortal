using Umbraco.Cms.Core.Models.PublishedContent;

namespace EventPortal.SearchModel
{
    public class SearchViewModel : PublishedContentWrapped
    {
        public SearchViewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
            : base(content, publishedValueFallback)
        {
        }

        public SearchResultsModel SearchResults { get; set; } = null!;
    }
}
