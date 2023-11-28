using MarthasLibrary.Core.Enums;

namespace MarthasLibrary.Core.Entities;

public class Reservation : IAuditableBase
{
  public Guid Id { get; private set; }
  public Guid BookId { get; private set; }
  public Guid CustomerId { get; private set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset UpdatedAt { get; set; }
  public ReservationStatus Status { get; private set; }



  public static Reservation CreateInstance(Guid bookId, Guid customerId)
  {
    if (string.IsNullOrWhiteSpace(bookId.ToString())) throw new ArgumentException(nameof(bookId));
    if (string.IsNullOrWhiteSpace(customerId.ToString())) throw new ArgumentException(nameof(customerId));

    return new Reservation
    {
      Id = Guid.NewGuid(),
      BookId = bookId,
      CustomerId = customerId,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      Status = ReservationStatus.Reserved
    };
  }

  // Example of a domain behavior
  public void MarkAsBorrowed()
  {
    if (Status != ReservationStatus.Reserved)
      throw new InvalidOperationException("Only reserved books can be marked as borrowed.");

    Status = ReservationStatus.Borrowed;
  }

  public void MarkAsCancelled()
  {
    if (Status != ReservationStatus.Reserved)
      throw new InvalidOperationException("Only reserved books can be marked as cancelled.");

    Status = ReservationStatus.Cancelled;
  }

  public void MarkAsReturned()
  {
    if (Status != ReservationStatus.Borrowed)
      throw new InvalidOperationException("Only borrowed books can be marked as returned.");

    Status = ReservationStatus.Returned;
  }
}