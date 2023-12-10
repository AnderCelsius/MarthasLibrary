using AutoMapper;
using MarthasLibrary.API.Features.Notifications;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<List<Notification>, GetNotificationsForCurrentUser.Response>()
            .ConstructUsing((src, context) => new GetNotificationsForCurrentUser.Response(context.Mapper.Map<IReadOnlyCollection<NotificationDetails>>(src)));

        CreateMap<Notification, NotificationDetails>()
            .ConstructUsing((notification, context) =>
            {
                // Retrieve the title from the context
                var title = context.Items.TryGetValue("Title", out var bookTitle) ? bookTitle.ToString() : string.Empty;

                // Construct the ReservationDetails object
                return new NotificationDetails(
                    notification.Id,
                    notification.CustomerId,
                    notification.BookId,
                    title,
                    notification.Type.ToString(),
                    false);
            });
    }
}