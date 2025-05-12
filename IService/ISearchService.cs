using EventPortal.SearchModel;

namespace EventPortal.IService
{
    public interface ISearchService
    {
        SearchResultsModel Search(string query, int pageNumber, int pageSize);
    }
}
