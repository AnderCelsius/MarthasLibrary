namespace MarthasLibrary.API.Shared;

public record ReservationDetails(
  Guid ReservationId,
  Guid BookId,
  string Title,
  DateTimeOffset ReservedDate,
  DateTimeOffset ExpiryDate);