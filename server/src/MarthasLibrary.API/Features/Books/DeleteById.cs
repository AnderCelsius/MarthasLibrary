using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Books;

/// <summary>
/// Provides functionality for deleting a book by its identifier.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and logic for deleting a book based on its unique identifier.
/// </remarks>
public static class DeleteById
{
  /// <summary>
  /// Represents a request to delete a book.
  /// </summary>
  /// <param name="BookId">The unique identifier of the book to be deleted.</param>
  public record Request(Guid BookId) : IRequest;

  /// <summary>
  /// Handles the process of deleting a book.
  /// </summary>
  /// <remarks>
  /// This class is responsible for locating the book by its identifier and removing it from the repository.
  /// </remarks>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for the book repository.</exception>
  /// <exception cref="BookNotFoundException">Thrown when the specified book is not found in the repository.</exception>
  public class Handler(IGenericRepository<Book> bookRepository) : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));

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