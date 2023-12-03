namespace MarthasLibrary.API.Shared;

public record BorrowDetails(
  Guid BorrowId,
  Guid CustomerId,
  Guid BookId,
  DateTimeOffset BorrowDate,
  DateTimeOffset DueDate);