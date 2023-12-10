namespace MarthasLibrary.Core.Events;

public record BookBorrowedEvent(Guid BookId, Guid CustomerId) : BaseEvent(BookId, CustomerId);
