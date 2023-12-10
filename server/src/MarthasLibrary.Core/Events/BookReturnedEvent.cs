namespace MarthasLibrary.Core.Events;

public record BookReturnedEvent(Guid BookId, Guid CustomerId) : BaseEvent(BookId, CustomerId);
