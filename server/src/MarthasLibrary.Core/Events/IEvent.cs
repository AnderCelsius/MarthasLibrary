namespace MarthasLibrary.Core.Events;

public interface IEvent
{
  public Guid EventId { get; init; }
  public DateTime Timestamp { get; }
}
