namespace MarthasLibrary.API.Shared;

public record BookDetails(
  Guid Id,
  string Title,
  string Author,
  string Isbn,
  string Status,
  string Description,
  DateTimeOffset PublishedDate);
