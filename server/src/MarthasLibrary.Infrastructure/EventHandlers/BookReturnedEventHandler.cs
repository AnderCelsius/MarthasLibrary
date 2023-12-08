using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using Newtonsoft.Json;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public class BookReturnedEventHandler
    (IGenericRepository<Notification> notificationRepository) : BaseNotificationEventHandler<BookReturnedEvent>(
        notificationRepository)
{
    protected override Notification CreateNotification(BookReturnedEvent notificationEvent)
    {
        return new Notification(notificationEvent.CustomerId, notificationEvent.BookId, nameof(BookReturnedEvent))
        {
            Data = JsonConvert.SerializeObject(notificationEvent)
        };
    }
}