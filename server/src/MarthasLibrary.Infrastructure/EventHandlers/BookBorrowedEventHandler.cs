using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public class BookBorrowedEventHandler : BaseNotificationEventHandler<BookBorrowedEvent>
{
    public BookBorrowedEventHandler(IGenericRepository<Notification> notificationRepository,
        ILogger<BookBorrowedEventHandler> logger)
        : base(notificationRepository, logger)
    {
    }

    protected override Notification CreateNotification(BookBorrowedEvent notificationEvent)
    {
        return new Notification(notificationEvent.CustomerId, notificationEvent.BookId, nameof(BookBorrowedEvent))
        {
            Data = JsonConvert.SerializeObject(notificationEvent)
        };
    }
}