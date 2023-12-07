using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using Newtonsoft.Json;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public class BookReturnedEventHandler : BaseNotificationEventHandler<BookReturnedEvent>
{
    public BookReturnedEventHandler(IGenericRepository<Notification> notificationRepository)
        : base(notificationRepository)
    {
    }

    protected override Notification CreateNotification(BookReturnedEvent notificationEvent)
    {
        return new Notification(notificationEvent.CustomerId, notificationEvent.BookId, nameof(BookReturnedEvent))
        {
            Data = JsonConvert.SerializeObject(notificationEvent)
        };
    }
}