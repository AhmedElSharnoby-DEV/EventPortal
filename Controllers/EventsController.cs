using EventPortal.FormModels;
using EventPortal.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace EventPortal.Controllers
{
    public class EventsController : RenderController
    {
        private readonly IPaginationService _paginationService;
        public EventsController(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IPaginationService paginationService) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _paginationService = paginationService;
        }

        [HttpGet]
        public override IActionResult Index()
        {
            IPublishedContent eventPage = CurrentPage!;
            int page = int.TryParse(Request.Query["page"], out var p) ? p : 1;
            int pageSize = 4; 

            List<Event> events = eventPage.Children
                .Where(x => x.IsVisible())
                .OfType<Event>()
                .OrderByDescending(x => x.CreateDate).ToList();

            string? baseUrl = eventPage.Url();
            PaginationModel<Event> paginationModel = _paginationService.Paginate(events, baseUrl, page, pageSize);
            return View("~/Views/Events/Index.cshtml", paginationModel);
        }
    }
}
