namespace MarthasLibrary.API.Shared;

public record NotificationDetails(
    Guid Id,
    Guid CustomerId,
    Guid BookId,
    string Type,
    bool IsRead);