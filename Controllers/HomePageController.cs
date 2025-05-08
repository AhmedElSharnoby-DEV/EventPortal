using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace EventPortal.Controllers
{
    public class HomePageController : RenderController
    {
        public HomePageController(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor
            ) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
        }
        public override IActionResult Index()
        {
            return CurrentTemplate(CurrentPage as Home);
        }
    }
}
