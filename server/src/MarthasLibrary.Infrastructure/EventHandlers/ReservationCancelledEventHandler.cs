using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public class ReservationCancelledEventHandler : BaseNotificationEventHandler<ReservationCancelledEvent>
{
    public ReservationCancelledEventHandler(IGenericRepository<Notification> notificationRepository,
        ILogger<ReservationCancelledEventHandler> logger)
        : base(notificationRepository, logger)
    {
    }

    protected override Notification CreateNotification(ReservationCancelledEvent notificationEvent)
    {
        return new Notification(notificationEvent.CustomerId, notificationEvent.BookId,
            nameof(ReservationCancelledEvent))
        {
            Data = JsonConvert.SerializeObject(notificationEvent)
        };
    }
}