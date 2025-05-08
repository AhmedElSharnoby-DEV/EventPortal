using EventPortal.FormModels;
using EventPortal.IService;
using static Umbraco.Cms.Core.Constants.Conventions;

namespace EventPortal.Services
{
    public class PaginationService : IPaginationService
    {
        public PaginationModel<T> Paginate<T>(IEnumerable<T> items, string url, int page, int pageSize)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

            if (page <= 0)
                page = 1;

            var totalItems = items.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page > totalPages)
                page = totalPages > 0 ? totalPages : 1;

            var paginatedItems = items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginationModel<T>
            {
                Items = paginatedItems,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                Url = url
            };
        }
    }
}
