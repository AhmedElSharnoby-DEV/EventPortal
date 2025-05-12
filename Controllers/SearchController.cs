using EventPortal.FormModels;
using EventPortal.IService;
using EventPortal.SearchModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace EventPortal.Controllers
{
    public class SearchController : RenderController
    {
        private readonly ISearchService _searchService;
        private readonly IPublishedValueFallback _publishedValueFallback;
        public SearchController(ISearchService searchService, IPublishedValueFallback publishedValueFallback, ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _searchService  = searchService;
            _publishedValueFallback = publishedValueFallback;
        }

        public override IActionResult Index()
        {
            var query = Request.Query["query"].ToString();
            int pageNumber = int.TryParse(Request.Query["page"], out var page) ? page : 1;
            const int pageSize = 5;

            var model = new SearchViewModel(CurrentPage, _publishedValueFallback)
            {
                SearchResults = _searchService.Search(query, pageNumber, pageSize)
            };
            return View("~/Views/Search/Index.cshtml", model);
        }
    }
}
