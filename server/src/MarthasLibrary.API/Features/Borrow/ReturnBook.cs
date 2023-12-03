using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Borrow;

public static class ReturnBook
{
  public record Request(Guid CustomerId, Guid BookId) : IRequest;

  public class Handler(IGenericRepository<Book> bookRepository,
      IGenericRepository<Core.Entities.Borrow> borrowRepository, ILogger<Handler> logger)
    : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IGenericRepository<Core.Entities.Borrow> _borrowRepository =
      borrowRepository ?? throw new ArgumentException(nameof(borrowRepository));

    private readonly ILogger<Handler> _logger = logger ?? throw new ArgumentException(nameof(logger));

    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
      try
      {
        await _bookRepository.BeginTransactionAsync(cancellationToken);

        var borrowedBook = await _borrowRepository.Table
          .SingleOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

        if (borrowedBook is null)
        {
          throw new BorrowNotFoundException(
            $"Couldn't find any borrowing with for book-id: {request.BookId} and customer: {request.CustomerId}.");
        }

        _borrowRepository.Delete(borrowedBook);
        await _bookRepository.SaveAsync(cancellationToken);

        var book = await _bookRepository.Table.SingleOrDefaultAsync(b => b.Id == borrowedBook.BookId,
          cancellationToken);

        if (book is not null)
        {
          book.MarkAsAvailable();
          _bookRepository.Update(book);
          await _bookRepository.SaveAsync(cancellationToken);
        }

        await _bookRepository.CommitTransactionAsync(cancellationToken);
      }
      catch
      {
        _logger.LogError("Transaction failed... Could not return book.");
        await _bookRepository.RollbackTransactionAsync(cancellationToken);
        throw;
      }
    }
  }
}