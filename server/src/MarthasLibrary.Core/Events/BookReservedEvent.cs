namespace MarthasLibrary.Core.Events;

public record BookReservedEvent(Guid BookId, Guid CustomerId) : BaseEvent(BookId, CustomerId);
