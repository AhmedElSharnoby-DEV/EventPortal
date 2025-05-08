using System.Reflection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace EventPortal.Validators
{
    public class EventValidator : INotificationHandler<ContentSavingNotification>
    {
        public EventValidator()
        {
        }

        public void Handle(ContentSavingNotification notification)
        {
            var events = notification.SavedEntities
                .Where(x => x.ContentType.Alias == Event.ModelTypeAlias)
                .Select(x => new
                {
                    Title = x.GetValue<string>("title"),
                    Description = x.GetValue<string>("description"),
                    Image = x.GetValue<string>("image"),
                })
                .ToList();
            foreach (var _event in events)
            {
                if (_event.Title!.Length < 50)
                    notification.CancelOperation(new EventMessage("Validation Error", "Event title must be more 50 character.", EventMessageType.Error));
                return;
            }
        }
    }
}
