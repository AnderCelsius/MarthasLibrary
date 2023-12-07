using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using Newtonsoft.Json;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public class BookBorrowedEventHandler : BaseNotificationEventHandler<BookBorrowedEvent>
{
    public BookBorrowedEventHandler(IGenericRepository<Notification> notificationRepository)
        : base(notificationRepository)
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