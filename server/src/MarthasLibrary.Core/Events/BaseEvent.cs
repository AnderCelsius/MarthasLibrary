using MediatR;

namespace MarthasLibrary.Core.Events;

public abstract record BaseEvent(Guid BookId, Guid CustomerId) : INotification
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}
