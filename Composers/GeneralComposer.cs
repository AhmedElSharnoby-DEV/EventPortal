using EventPortal.IService;
using EventPortal.PropertyEditors;
using EventPortal.Services;
using EventPortal.Validators;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace EventPortal.Composers
{
    public class GeneralComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register any additional services
            builder.Services.AddTransient<PasswordEncryptionService>();
            builder.Services.AddScoped<IPaginationService, PaginationService>();
            builder.AddNotificationHandler<ContentSavingNotification, EventValidator>();
            builder.Services.AddSingleton<ISearchService, SearchService>();
        }
    }
}
