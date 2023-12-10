using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public class BookAvailableEventHandler : BaseNotificationEventHandler<BookAvailableEvent>
{
    public BookAvailableEventHandler(IGenericRepository<Notification> notificationRepository,
        ILogger<BookAvailableEventHandler> logger)
        : base(notificationRepository, logger)
    {
    }

    protected override Notification CreateNotification(BookAvailableEvent notificationEvent)
    {
        return new Notification(notificationEvent.CustomerId, notificationEvent.BookId, nameof(BookReservedEvent))
        {
            Data = JsonConvert.SerializeObject(notificationEvent)
        };
    }
}