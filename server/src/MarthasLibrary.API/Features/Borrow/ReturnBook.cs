using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Borrow;

/// <summary>
/// Provides functionality for returning a borrowed book.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and processing logic for returning a borrowed book.
/// </remarks>
public static class ReturnBook
{
  /// <summary>
  /// Represents a request to return a borrowed book.
  /// </summary>
  /// <param name="BorrowId">The unique identifier of the borrow record.</param>
  public record Request(Guid BorrowId) : IRequest;

  /// <summary>
  /// Handles the process of returning a borrowed book.
  /// </summary>
  /// <remarks>
  /// This class is responsible for validating the borrow record, updating the book's status to available, and deleting the borrow record.
  /// </remarks>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <param name="borrowRepository">Repository for accessing borrow entities.</param>
  /// <param name="logger">Logger for logging information and errors.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for any of the repositories or the logger.</exception>
  public class Handler(IGenericRepository<Book> bookRepository,
      IGenericRepository<Core.Entities.Borrow> borrowRepository, ILogger<Handler> logger)
    : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IGenericRepository<Core.Entities.Borrow> _borrowRepository =
      borrowRepository ?? throw new ArgumentException(nameof(borrowRepository));

    private readonly ILogger<Handler> _logger = logger ?? throw new ArgumentException(nameof(logger));

    /// <summary>
    /// Handles the incoming request to return a borrowed book.
    /// </summary>
    /// <remarks>
    /// This method processes the request by validating the borrow record, updating the book's status to available, and deleting the borrow record.
    /// It also manages transactions and logs any errors encountered during the process.
    /// </remarks>
    /// <param name="request">The request to return a borrowed book.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <exception cref="BorrowNotFoundException">Thrown when the borrow record is not found.</exception>
    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
      try
      {
        await _bookRepository.BeginTransactionAsync(cancellationToken);

        var borrowedBook = await _borrowRepository.Table
          .SingleOrDefaultAsync(borrow => borrow.Id == request.BorrowId, cancellationToken);

        if (borrowedBook is null)
        {
          throw new BorrowNotFoundException(
            $"Couldn't find any borrowing with id: {request.BorrowId}.");
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