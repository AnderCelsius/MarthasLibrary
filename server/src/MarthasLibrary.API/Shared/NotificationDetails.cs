namespace MarthasLibrary.API.Shared;

public record NotificationDetails(
    Guid Id,
    Guid CustomerId,
    Guid BookId,
    string BookTitle,
    string Type,
    bool IsRead);