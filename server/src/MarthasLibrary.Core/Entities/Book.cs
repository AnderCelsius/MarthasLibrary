using MarthasLibrary.Core.Enums;

namespace MarthasLibrary.Core.Entities;

public class Book : IAuditableBase
{
    private Book(string title, string author, string isbn, DateTimeOffset publishedDate, string? description = null)
    {
        Title = title;
        Author = author;
        Isbn = isbn;
        PublishedDate = publishedDate;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        Status = BookStatus.Available;
        Description = description;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Isbn { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset PublishedDate { get; private set; }
    public BookStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }


    public static Book CreateInstance(string title, string author, string isbn, DateTimeOffset publishedDate, string? description = null)
    {
        return new Book(title, author, isbn, publishedDate, description);
    }

    public void UpdateDetails(BookUpdate update)
    {
        Title = update.Title;
        Author = update.Author;
        Isbn = update.Isbn;
        PublishedDate = update.PublishedDate;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    // Example of a domain behavior
    public void MarkAsReserved()
    {
        if (Status != BookStatus.Available)
            throw new InvalidOperationException("Only available books can be marked as reserved.");

        Status = BookStatus.Reserved;
    }

    public void MarkAsBorrowed()
    {
        if (Status != BookStatus.Reserved)
            throw new InvalidOperationException("Only reserved books can be marked as borrowed.");

        Status = BookStatus.Borrowed;
    }

    public void MarkAsAvailable()
    {
        if (Status != BookStatus.Reserved && Status != BookStatus.Borrowed)
            throw new InvalidOperationException("Only reserved books or borrowed can be marked as available.");

        Status = BookStatus.Available;
    }

    public record BookUpdate(string Title, string Author, string Isbn, DateTimeOffset PublishedDate);
}