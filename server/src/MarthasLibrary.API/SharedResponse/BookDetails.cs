namespace MarthasLibrary.API.SharedResponse;

public record BookDetails(
  Guid Id,
  string Title,
  string Author,
  string Isbn,
  DateTimeOffset PublishedDate);
