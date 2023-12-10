namespace MarthasLibrary.Core.Entities;

public class Reservation
{
  public Guid Id { get; private set; }
  public Guid BookId { get; private set; }
  public Guid CustomerId { get; private set; }
  public DateTimeOffset ReservedDate { get; set; }
  public DateTimeOffset? ExpiryDate { get; set; }


  public static Reservation CreateInstance(Guid bookId, Guid customerId)
  {
    if (bookId == Guid.Empty) throw new ArgumentException("Value cannot be null.", nameof(bookId));
    if (customerId == Guid.Empty) throw new ArgumentException("Value cannot be null.", nameof(customerId));

    return new Reservation
    {
      Id = Guid.NewGuid(),
      BookId = bookId,
      CustomerId = customerId,
      ReservedDate = DateTime.UtcNow,
      ExpiryDate = DateTime.UtcNow.AddDays(1),
    };
  }
}