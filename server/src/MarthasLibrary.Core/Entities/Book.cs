namespace MarthasLibrary.Core.Entities;

public class Book : IAuditableBase
{
  private Book(string title, string author, string isbn, DateTime publishedDate)
  {
    Title = title;
    Author = author;
    Isbn = isbn;
    PublishedDate = publishedDate;
    CreatedAt = DateTime.Now;
    UpdatedAt = DateTime.Now;
  }

  public static Book CreateInstance(string title, string author, string isbn, DateTime publishedDate)
  {
    return new Book(title, author, isbn, publishedDate);
  }

  public Guid Id { get; private set; }
  public string Title { get; private set; }
  public string Author { get; private set; }
  public string Isbn { get; private set; }
  public DateTimeOffset PublishedDate { get; private set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset UpdatedAt { get; set; }
}

