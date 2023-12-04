using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

/// <summary>
/// Provides functionality for retrieving all reservations in the library system.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for fetching all reservations.
/// </remarks>
public static class GetAll
{
  /// <summary>
  /// Represents a request for retrieving all reservations.
  /// </summary>
  /// <remarks>
  /// This record is a marker type used to indicate a request for fetching all reservations in the library system. It does not contain any properties.
  /// </remarks>
  public record Request() : IRequest<Response>;

  /// <summary>
  /// Represents the response containing all reservation details.
  /// </summary>
  /// <param name="Books">A read-only collection of reservation details.</param>
  public record Response(IReadOnlyCollection<ReservationDetails> Books);

  /// <summary>
  /// Handles the retrieval of all reservations.
  /// </summary>
  /// <remarks>
  /// This class, inheriting from BaseReservationHandler, is responsible for fetching all reservation records from the repository and mapping them to the response format using AutoMapper.
  /// </remarks>
  /// <param name="reservationRepository">Repository for accessing reservation entities.</param>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <param name="mapper">An instance of AutoMapper for object mapping.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for any of the repositories or the mapper.</exception>
  public class Handler(IGenericRepository<Reservation> reservationRepository,
      IGenericRepository<Book> bookRepository,
      IMapper mapper)
    : BaseReservationHandler(reservationRepository, bookRepository, mapper), IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Reservation> _reservationRepository = reservationRepository ?? throw new ArgumentException(nameof(reservationRepository));

    /// <summary>
    /// Handles the incoming request to retrieve all reservations.
    /// </summary>
    /// <param name="request">The request to retrieve all reservations.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing all reservation details.</returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var reservations = await _reservationRepository.TableNoTracking.ToListAsync(cancellationToken);

      var reservationDetails = await GetReservationDetails(reservations, cancellationToken);

      return new Response(reservationDetails);
    }
  }
}