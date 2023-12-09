using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public class BookReturnedEventHandler
    (IGenericRepository<Notification> notificationRepository, ILogger<BookReturnedEventHandler> logger) :
        BaseNotificationEventHandler<BookReturnedEvent>(
            notificationRepository, logger)
{
    protected override Notification CreateNotification(BookReturnedEvent notificationEvent)
    {
        return new Notification(notificationEvent.CustomerId, notificationEvent.BookId, nameof(BookReturnedEvent))
        {
            Data = JsonConvert.SerializeObject(notificationEvent)
        };
    }
}