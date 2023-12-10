namespace MarthasLibrary.Core.Entities;

public class Notification(Guid customerId, Guid bookId, string type)
{
  public Guid Id { get; private set; } = Guid.NewGuid();
  public Guid CustomerId { get; private set; } = customerId;
  public Guid BookId { get; private set; } = bookId;
  public string Type { get; private set; } = type;
  public string Data { get; set; } = "{}";
  public bool IsRead { get; private set; }
  public DateTimeOffset NotificationDate { get; set; }
}