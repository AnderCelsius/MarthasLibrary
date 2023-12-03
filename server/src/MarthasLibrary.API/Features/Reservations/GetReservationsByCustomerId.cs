using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

/// <summary>
/// Provides functionality for retrieving reservations made by a specific customer.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for fetching reservations associated with a particular customer ID.
/// </remarks>
public static class GetReservationsByCustomerId
{
  /// <summary>
  /// Represents a request for retrieving reservations made by a specific customer.
  /// </summary>
  /// <param name="CustomerId">The unique identifier of the customer whose reservations are to be retrieved.</param>
  public record Request(Guid CustomerId) : IRequest<Response>;

  /// <summary>
  /// Represents the response containing the reservations made by the specified customer.
  /// </summary>
  /// <param name="Reservations">A read-only collection of reservation details for the specified customer.</param>
  public record Response(IReadOnlyCollection<ReservationDetails> Reservations);

  /// <summary>
  /// Handles the process of retrieving reservations made by a specific customer.
  /// </summary>
  /// <remarks>
  /// This class, inheriting from BaseReservationHandler, is responsible for fetching reservation records for a specific customer from the repository and mapping them to the response format using AutoMapper.
  /// </remarks>
  /// <param name="reservationRepository">Repository for accessing reservation entities.</param>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <param name="mapper">An instance of AutoMapper for object mapping.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for any of the repositories or the mapper.</exception>
  public class Handler
  (IGenericRepository<Reservation> reservationRepository,
    IGenericRepository<Book> bookRepository,
    IMapper mapper) : BaseReservationHandler(reservationRepository, bookRepository, mapper), IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Reservation> _reservationRepository =
      reservationRepository ?? throw new ArgumentException(nameof(reservationRepository));

    /// <summary>
    /// Handles the incoming request to retrieve reservations made by a specific customer.
    /// </summary>
    /// <param name="request">The request containing the customer ID whose reservations are to be retrieved.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing the reservation details for the specified customer.</returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var reservations = await _reservationRepository.TableNoTracking
        .Where(r => r.CustomerId == request.CustomerId)
        .ToListAsync(cancellationToken);

      var reservationDetails = await GetReservationDetails(reservations, cancellationToken);
      return new Response(reservationDetails);
    }
  }
}