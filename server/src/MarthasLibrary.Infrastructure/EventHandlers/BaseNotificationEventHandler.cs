using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;

namespace MarthasLibrary.Infrastructure.EventHandlers;

public abstract class BaseNotificationEventHandler<TEvent>
    (IGenericRepository<Notification> notificationRepository) : INotificationHandler<TEvent>
    where TEvent : INotification
{
    protected readonly IGenericRepository<Notification> NotificationRepository = notificationRepository;

    public async Task Handle(TEvent notificationEvent, CancellationToken cancellationToken)
    {
        var notification = CreateNotification(notificationEvent);
        await NotificationRepository.InsertAsync(notification);
        await NotificationRepository.SaveAsync(cancellationToken);
    }

    protected abstract Notification CreateNotification(TEvent notificationEvent);
}