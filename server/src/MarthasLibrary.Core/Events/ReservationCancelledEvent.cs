namespace MarthasLibrary.Core.Events;

public record ReservationCancelledEvent(Guid BookId, Guid CustomerId) : BaseEvent(BookId, CustomerId);