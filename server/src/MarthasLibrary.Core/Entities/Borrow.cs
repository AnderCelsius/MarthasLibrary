namespace MarthasLibrary.Core.Entities;

public class Borrow
{
  public Borrow(Guid bookId, Guid customerId, DateTimeOffset dueDate)
  {
    BookId = bookId;
    CustomerId = customerId;
    BorrowDate = DateTimeOffset.UtcNow;
    DueDate = dueDate;
  }
  public Guid Id { get; private set; }
  public Guid BookId { get; private set; }
  public Guid CustomerId { get; private set; }
  public DateTimeOffset BorrowDate { get; set; }
  public DateTimeOffset DueDate { get; set; }
  public DateTimeOffset? ReturnDate { get; set; }

  public void ReturnBook()
  {
    ReturnDate = DateTimeOffset.UtcNow;
  }
}