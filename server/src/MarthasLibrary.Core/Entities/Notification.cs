using MarthasLibrary.Core.Enums;

namespace MarthasLibrary.Core.Entities;

public class Notification(Guid customerId, Guid bookId, string type)
{
  public Guid Id { get; private set; } = Guid.NewGuid();
  public Guid CustomerId { get; private set; } = customerId;
  public Guid BookId { get; private set; } = bookId;
  public string Type { get; private set; } = type;

  public string Data { get; set; } = "{}";
  public NotificationStatus Status { get; private set; } = NotificationStatus.Unsent;
  public DateTimeOffset NotificationDate { get; set; }
}