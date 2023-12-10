using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public class BookReservedEventHandler : BaseNotificationEventHandler<BookReservedEvent>
{
    public BookReservedEventHandler(IGenericRepository<Notification> notificationRepository,
        ILogger<BookReservedEventHandler> logger)
        : base(notificationRepository, logger)
    {
    }

    protected override Notification CreateNotification(BookReservedEvent notificationEvent)
    {
        return new Notification(notificationEvent.CustomerId, notificationEvent.BookId, nameof(BookReservedEvent))
        {
            Data = JsonConvert.SerializeObject(notificationEvent)
        };
    }
}