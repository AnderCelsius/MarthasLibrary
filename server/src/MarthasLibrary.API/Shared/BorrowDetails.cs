namespace MarthasLibrary.API.Shared;

public record BorrowDetails(
  Guid BorrowId,
  Guid CustomerId,
  Guid BookId,
  string Title,
  DateTimeOffset BorrowDate,
  DateTimeOffset DueDate);