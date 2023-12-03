using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Books;

/// <summary>
/// Provides functionality for retrieving all books in the library.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for fetching all books.
/// </remarks>
public static class GetAll
{
  /// <summary>
  /// Represents a request for retrieving all books.
  /// </summary>
  /// <remarks>
  /// This record is a marker type used to indicate a request for fetching all books in the library. It does not contain any properties.
  /// </remarks>
  public record Request(int PageNumber, int PageSize) : IRequest<Response>;

  /// <summary>
  /// Represents the response containing all books.
  /// </summary>
  /// <param name="Books">A read-only collection of book details.</param>
  /// <remarks>
  /// This record encapsulates the response data for a request to fetch all books, providing the book details as a read-only collection.
  /// </remarks>
  public record Response(IReadOnlyCollection<BookDetails> Books);

  /// <summary>
  /// Handles the retrieval of all books.
  /// </summary>
  /// <remarks>
  /// This class is responsible for fetching all book records from the repository and mapping them to the response format.
  /// </remarks>
  /// <param name="mapper">An instance of AutoMapper to map between different object types.</param>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for either the book repository or the mapper.</exception>
  public class Handler(IMapper mapper, IGenericRepository<Book> bookRepository) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    /// <summary>
    /// Handles the incoming request to retrieve all books.
    /// </summary>
    /// <param name="request">The request to retrieve all books.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing all book details.</returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var books = await _bookRepository.TableNoTracking
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToListAsync(cancellationToken);

      return _mapper.Map<Response>(books);
    }
  }
}