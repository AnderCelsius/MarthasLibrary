namespace MarthasLibrary.Core.Events;

public record BookAvailableEvent(Guid BookId, Guid CustomerId) : BaseEvent(BookId, CustomerId);
