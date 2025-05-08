using EventPortal.FormModels;

namespace EventPortal.IService
{
    public interface IPaginationService
    {
        PaginationModel<T> Paginate<T>(IEnumerable<T> items, string url, int page, int pageSize);
    }
}
