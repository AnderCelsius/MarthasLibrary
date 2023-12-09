using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public abstract class BaseNotificationEventHandler<TEvent>
    (IGenericRepository<Notification> notificationRepository,
        ILogger<BaseNotificationEventHandler<TEvent>> logger) : INotificationHandler<TEvent>
    where TEvent : INotification
{
    public async Task Handle(TEvent notificationEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Publishing {notificationEvent} to DB");
        Console.WriteLine($"Publishing {notificationEvent} to DB");
        var notification = CreateNotification(notificationEvent);
        await notificationRepository.InsertAsync(notification);
        await notificationRepository.SaveAsync(cancellationToken);
    }

    protected abstract Notification CreateNotification(TEvent notificationEvent);
}