using EventPortal.SearchModel;
using Examine.Search;
using Examine;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using EventPortal.IService;

namespace EventPortal.Services
{
    public class SearchService : ISearchService
    {
        private readonly IExamineManager _examineManager;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public SearchService(IExamineManager examineManager, IUmbracoContextFactory umbracoContextFactory)
        {
            _examineManager = examineManager;
            _umbracoContextFactory = umbracoContextFactory;
        }

        public SearchResultsModel Search(string query, int pageNumber, int pageSize)
        {
            var resultsModel = new SearchResultsModel
            {
                SearchQuery = query,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            if (!_examineManager.TryGetIndex("ExternalIndex", out var index))
            {
                resultsModel.Results = Enumerable.Empty<EventViewModel>();
                return resultsModel;
            }

            var searcher = index.Searcher;
            IQueryExecutor queryExecutor;

            if (string.IsNullOrWhiteSpace(query))
            {
                // When no query is provided, fetch all Event nodes
                queryExecutor = searcher.CreateQuery("content")
                    .NodeTypeAlias(Event.ModelTypeAlias);
            }
            else
            {
                // When a query is provided, search title and description
                queryExecutor = searcher.CreateQuery("content")
                    .NodeTypeAlias(Event.ModelTypeAlias)
                    .And()
                    .GroupedOr(new[] { "title", "description" }, query);
            }

            var searchResults = queryExecutor.Execute(new QueryOptions((pageNumber - 1) * pageSize, pageSize));
            resultsModel.TotalItems = searchResults.TotalItemCount;

            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var contentCache = context.UmbracoContext.Content;
                var eventResults = new List<EventViewModel>();

                foreach (var result in searchResults)
                {
                    var content = contentCache.GetById(int.Parse(result.Id));
                    if (content != null && content.ContentType.Alias == "event")
                    {
                        eventResults.Add(new EventViewModel
                        {
                            Title = content.Value<string>("title"),
                            Description = content.Value<string>("description"),
                            ImageUrl = content.Value<string>("image") ?? string.Empty,
                            Url = content.Url()
                        });
                    }
                }

                resultsModel.Results = eventResults;
            }

            return resultsModel;
        }
    }
}
