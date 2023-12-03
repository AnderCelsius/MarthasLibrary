using AutoMapper;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Books;

/// <summary>
/// Provides functionality for retrieving a specific book by its identifier.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for fetching a book by its unique identifier.
/// </remarks>
public static class GetById
{
  /// <summary>
  /// Represents a request to retrieve a book by its ID.
  /// </summary>
  /// <param name="BookId">The unique identifier of the book to be retrieved.</param>
  public record Request(Guid BookId) : IRequest<Response>;

  /// <summary>
  /// Represents the response containing the details of the requested book.
  /// </summary>
  /// <param name="Book">Details of the requested book.</param>
  public record Response(BookDetails Book);

  /// <summary>
  /// Handles the process of retrieving a book by its identifier.
  /// </summary>
  /// <remarks>
  /// This class is responsible for fetching the book record from the repository and mapping it to the response format.
  /// </remarks>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <param name="mapper">An instance of AutoMapper for object mapping.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for the book repository or the mapper.</exception>
  /// <exception cref="BookNotFoundException">Thrown when the specified book is not found in the repository.</exception>
  public class Handler(IGenericRepository<Book> bookRepository, IMapper mapper) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    /// <summary>
    /// Handles the incoming request to retrieve a book by its ID.
    /// </summary>
    /// <param name="request">The request to retrieve a book by its ID.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing the details of the requested book.</returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var book = await _bookRepository.TableNoTracking
        .SingleOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

      if (book is null)
      {
        throw new BookNotFoundException($"Could not find a book with ID {request.BookId}.");
      }

      return _mapper.Map<Response>(book);
    }
  }
}
