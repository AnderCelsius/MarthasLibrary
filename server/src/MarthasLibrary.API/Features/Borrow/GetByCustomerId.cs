using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Borrow;

/// <summary>
/// Provides functionality for retrieving borrow records by a specific customer ID.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for fetching borrow records for a specific customer.
/// </remarks>
public static class GetByCustomerId
{
  /// <summary>
  /// Represents a request for retrieving borrow records by customer ID.
  /// </summary>
  /// <param name="CustomerId">The unique identifier of the customer.</param>
  /// <remarks>
  /// This record is used to indicate a request for borrow records associated with a specific customer.
  /// </remarks>
  public record Request(Guid CustomerId) : IRequest<Response>;

  /// <summary>
  /// Represents the response containing borrow records for a specific customer.
  /// </summary>
  /// <param name="Borrowings">A read-only collection of borrow details for the specified customer.</param>
  /// <remarks>
  /// This record encapsulates the response data for a request to fetch borrow records for a specific customer, providing the details as a read-only collection.
  /// </remarks>
  public record Response(IReadOnlyCollection<BorrowDetails> Borrowings);

  /// <summary>
  /// Handles the retrieval of borrow records for a specific customer.
  /// </summary>
  /// <remarks>
  /// This class inherits from BaseBorrowHandler to leverage shared functionality and implements IRequestHandler to process requests of type <see cref="Request"/>.
  /// </remarks>
  /// <param name="borrowRepository">A generic repository for accessing borrow entities.</param>
  /// <param name="bookRepository">A generic repository for accessing book entities.</param>
  /// <param name="mapper">An instance of AutoMapper to map between different object types.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for any of the repositories or the mapper.</exception>
  public class Handler
  (IGenericRepository<Core.Entities.Borrow> borrowRepository,
    IGenericRepository<Book> bookRepository,
    IMapper mapper) : BaseBorrowHandler(bookRepository, mapper), IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Core.Entities.Borrow> _borrowRepository =
      borrowRepository ?? throw new ArgumentException(nameof(borrowRepository));

    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    /// <summary>
    /// Handles the incoming request to retrieve borrow records for a specific customer.
    /// </summary>
    /// <remarks>
    /// This method asynchronously processes the request to fetch borrow records associated with a specific customer, returning the relevant details.
    /// </remarks>
    /// <param name="request">The request to retrieve borrow records for a specific customer.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing the borrow details for the specified customer.</returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var borrows = await _borrowRepository.TableNoTracking
        .Where(r => r.Id == request.CustomerId)
        .ToListAsync(cancellationToken);

      var borrowDetails = await GetBorrowDetails(borrows, cancellationToken);

      return new Response(borrowDetails);
    }
  }
}