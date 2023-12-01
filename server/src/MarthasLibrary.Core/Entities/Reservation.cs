namespace MarthasLibrary.Core.Entities;

public class Reservation : IAuditableBase
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }


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
        };
    }
}