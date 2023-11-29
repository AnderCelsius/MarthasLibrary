using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Books;

public static class DeleteById
{
  public record Request(Guid BookId) : IRequest;

  public class Handler(IGenericRepository<Book> bookRepository) : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository;

    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
      var book = await _bookRepository.Table
                   .SingleOrDefaultAsync(book => book.Id == request.BookId, cancellationToken) ??
                 throw new BookNotFoundException($"Could not find a book with ID {request.BookId}.");

      _bookRepository.Delete(book);
      await _bookRepository.SaveAsync(cancellationToken);
    }
  }
}