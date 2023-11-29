using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Books;

public static class UpdateById
{
  public record Request(Guid BookId, Request.UpdatedDetails Details) : IRequest
  {
    public record UpdatedDetails(string Title, string Author, string Isbn, DateTimeOffset PublishedDate)
    {
      public static explicit operator Book.BookUpdate(UpdatedDetails d) =>
        new(Title: d.Title, Author: d.Author, Isbn: d.Isbn, PublishedDate: d.PublishedDate);
    }
  }

  public class Handler(IGenericRepository<Book> bookRepository) : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository;

    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
      var book = await _bookRepository.Table
                   .SingleOrDefaultAsync(book => book.Id == request.BookId, cancellationToken) ??
                 throw new BookNotFoundException($"Could not find a user with ID {request.BookId}.");

      var potentialDuplicate = await _bookRepository.Table
        .SingleOrDefaultAsync(
          b => request.BookId != b.Id && request.Details.Isbn == b.Isbn,
          cancellationToken);

      if (potentialDuplicate is not null)
      {
        throw new BookWithIsbnAlreadyExistsException("A different book with this Isbn already exists.");
      }

      book.UpdateDetails((Book.BookUpdate)request.Details);
      await _bookRepository.SaveAsync(cancellationToken);
    }
  }
}