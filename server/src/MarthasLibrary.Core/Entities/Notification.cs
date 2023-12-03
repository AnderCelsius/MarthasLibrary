using MarthasLibrary.Core.Enums;

namespace MarthasLibrary.Core.Entities;

public class Notification(Guid customerId, Guid bookId)
{
  public Guid Id { get; private set; }
  public Guid CustomerId { get; private set; } = customerId;
  public Guid BookId { get; private set; } = bookId;
  public NotificationStatus Status { get; private set; } = NotificationStatus.Unsent;
  public DateTimeOffset NotificationDate { get; set; }
}