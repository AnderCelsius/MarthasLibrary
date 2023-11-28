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
        .SingleOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

      if (book is null)
      {
        throw new BookNotFoundException($"Could not find a user with ID {request.BookId}.");
      }

      book.UpdateDetails((Book.BookUpdate)request.Details);
      await _bookRepository.SaveAsync(cancellationToken);
    }
  }
}