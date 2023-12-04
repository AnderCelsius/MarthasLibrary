namespace MarthasLibrary.API.Shared;

public record ReservationDetails(
  Guid ReservationId,
  Guid BookId,
  Guid CustomerId,
  string Title,
  DateTimeOffset ReservedDate,
  DateTimeOffset ExpiryDate);