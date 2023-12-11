using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Books;

/// <summary>
/// Provides functionality for searching books in the library based on a query.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for searching books based on a given query string.
/// </remarks>
public static class Search
{
  /// <summary>
  /// Represents a request for searching books.
  /// </summary>
  /// <param name="Query">The query string used for searching books.</param>
  public record Request(string Query, int PageNumber, int PageSize) : IRequest<Response>;

  /// <summary>
  /// Represents the response containing the search results.
  /// </summary>
  /// <param name="Books">A read-only collection of book details that match the search query.</param>
  public record Response(IReadOnlyCollection<BookDetails> Books, int Total);

  /// <summary>
  /// Handles the process of searching for books.
  /// </summary>
  /// <remarks>
  /// This class is responsible for executing the search against the book repository based on the query string and mapping the results to the response format.
  /// </remarks>
  /// <param name="mapper">An instance of AutoMapper for object mapping.</param>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for the book repository or the mapper.</exception>
  public class Handler(IMapper mapper, IGenericRepository<Book> bookRepository) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    /// <summary>
    /// Handles the incoming request to search for books.
    /// </summary>
    /// <param name="request">The request containing the search query.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing the search results.</returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      // TODO: Implementing a debounce mechanism in the frontend to limit the frequency of requests
      var booksQuery = _bookRepository.TableNoTracking;
      var count = await booksQuery.CountAsync(cancellationToken);

      var books = await _bookRepository.TableNoTracking
        .Where(b => b.Title.Contains(request.Query) || b.Author.Contains(request.Query))
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToListAsync(cancellationToken);

      var searchResult = _mapper.Map<IReadOnlyCollection<BookDetails>>(books);

      return new Response(searchResult, count);
    }
  }
}