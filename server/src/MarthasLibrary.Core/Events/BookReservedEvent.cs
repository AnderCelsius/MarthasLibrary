namespace MarthasLibrary.Core.Events;

public record BookReservedEvent(Guid BookId, Guid CustomerId) : IEvent
{
  public Guid EventId { get; init; } = Guid.NewGuid();
  public DateTime Timestamp { get; } = DateTime.UtcNow;
}
