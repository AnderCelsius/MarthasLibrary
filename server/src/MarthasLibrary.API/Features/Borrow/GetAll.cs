using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Borrow;

/// <summary>
/// Provides functionality for retrieving all borrow records from Martha's Library.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for fetching all borrow records.
/// </remarks>
public static class GetAll
{
  /// <summary>
  /// Represents a request for retrieving all borrow records.
  /// </summary>
  /// <remarks>
  /// This record is a marker type used to indicate a request for all borrow records in the library. It does not contain any properties.
  /// </remarks>
  public record Request() : IRequest<Response>;

  /// <summary>
  /// Represents the response containing all borrow records.
  /// </summary>
  /// <param name="Books">A read-only collection of borrow details.</param>
  /// <remarks>
  /// This record encapsulates the response data for a request to fetch all borrow records, providing the borrow details as a read-only collection.
  /// </remarks>
  public record Response(IReadOnlyCollection<BorrowDetails> Books);

  /// <summary>
  /// Handles the retrieval of all borrow records.
  /// </summary>
  /// <remarks>
  /// This class inherits from BaseBorrowHandler to leverage shared functionality and implements IRequestHandler to process requests of type <see cref="Request"/>.
  /// </remarks>
  /// <param name="mapper">An instance of AutoMapper to map between different object types.</param>
  /// <param name="borrowRepository">A generic repository for accessing borrow entities.</param>
  /// <param name="bookRepository">A generic repository for accessing book entities.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for either the borrow repository or the mapper.</exception>
  public class Handler
  (IMapper mapper, IGenericRepository<Core.Entities.Borrow> borrowRepository,
    IGenericRepository<Book> bookRepository) : BaseBorrowHandler(bookRepository, mapper),
    IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Core.Entities.Borrow> _borrowRepository =
      borrowRepository ?? throw new ArgumentException(nameof(borrowRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var borrows = await _borrowRepository.TableNoTracking.ToListAsync(cancellationToken);

      var borrowDetails = await GetBorrowDetails(borrows, cancellationToken);

      return _mapper.Map<Response>(borrowDetails);
    }
  }
}