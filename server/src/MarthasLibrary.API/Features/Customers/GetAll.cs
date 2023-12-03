using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Customers;

/// <summary>
/// Provides functionality for retrieving all customers in the library system.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for fetching all active customers.
/// </remarks>
public static class GetAll
{
  /// <summary>
  /// Represents a request for retrieving all active customers.
  /// </summary>
  /// <remarks>
  /// This record is a marker type used to indicate a request for fetching all active customers in the library system. It does not contain any properties.
  /// </remarks>
  public record Request() : IRequest<Response>;

  /// <summary>
  /// Represents the response containing all active customer details.
  /// </summary>
  /// <param name="Customers">A read-only collection of active customer details.</param>
  public record Response(IReadOnlyCollection<CustomerDetails> Customers);


  /// <summary>
  /// Handles the retrieval of all active customers.
  /// </summary>
  /// <remarks>
  /// This class is responsible for fetching all active customer records from the repository and mapping them to the response format using AutoMapper.
  /// </remarks>
  /// <param name="mapper">An instance of AutoMapper for object mapping.</param>
  /// <param name="customerRepository">Repository for accessing customer entities.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for either the customer repository or the mapper.</exception>
  public class Handler
    (IMapper mapper, IGenericRepository<Customer> customerRepository) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Customer> _customerRepository =
      customerRepository ?? throw new ArgumentException(nameof(customerRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    /// <summary>
    /// Handles the incoming request to retrieve all active customers.
    /// </summary>
    /// <param name="request">The request to retrieve all active customers.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing all active customer details.</returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var customerDetails = await _customerRepository.TableNoTracking
        .Where(c => c.IsActive)
        .ProjectTo<CustomerDetails>(_mapper.ConfigurationProvider)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

      return new Response(customerDetails);
    }
  }
}