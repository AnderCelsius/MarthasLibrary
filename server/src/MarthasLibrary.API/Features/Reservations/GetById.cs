using AutoMapper;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

/// <summary>
/// Provides functionality for retrieving a specific reservation by its identifier.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for fetching a reservation by its unique identifier.
/// </remarks>
public static class GetById
{
  /// <summary>
  /// Represents a request to retrieve a reservation by its ID.
  /// </summary>
  /// <param name="ReservationId">The unique identifier of the reservation to be retrieved.</param>
  public record Request(Guid ReservationId) : IRequest<Response>;

  /// <summary>
  /// Represents the response containing the details of the requested reservation.
  /// </summary>
  /// <param name="Reservation">Details of the requested reservation.</param>
  public record Response(ReservationDetails Reservation);

  /// <summary>
  /// Handles the process of retrieving a reservation by its identifier.
  /// </summary>
  /// <remarks>
  /// This class is responsible for fetching the reservation record from the repository, retrieving associated book details, and mapping them to the response format using AutoMapper.
  /// </remarks>
  /// <param name="reservationRepository">Repository for accessing reservation entities.</param>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <param name="mapper">An instance of AutoMapper for object mapping.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for any of the repositories or the mapper.</exception>
  /// <exception cref="ReservationNotFoundException">Thrown when the specified reservation is not found in the repository.</exception>
  public class Handler
  (IGenericRepository<Reservation> reservationRepository,
    IGenericRepository<Book> bookRepository,
    IMapper mapper) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Reservation> _reservationRepository =
      reservationRepository ?? throw new ArgumentException(nameof(reservationRepository));

    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    /// <summary>
    /// Handles the incoming request to retrieve a reservation by its ID.
    /// </summary>
    /// <param name="request">The request to retrieve a reservation by its ID.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing the details of the requested reservation.</returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var reservation = await _reservationRepository.TableNoTracking
        .SingleOrDefaultAsync(r => r.Id == request.ReservationId, cancellationToken: cancellationToken);

      if (reservation is null)
      {
        throw new ReservationNotFoundException();
      }

      var book = await _bookRepository.TableNoTracking
        .Where(b => b.Id == reservation.BookId)
        .Select(b => new { b.Title })
        .SingleOrDefaultAsync(cancellationToken);

      // Check if book is null and set title accordingly
      var bookTitle = book?.Title ?? string.Empty;

      var reservationDetails = _mapper.Map<Reservation, ReservationDetails>(reservation, opt =>
      {
        opt.Items["Title"] = bookTitle;
      });

      return new Response(reservationDetails);
    }
  }
}