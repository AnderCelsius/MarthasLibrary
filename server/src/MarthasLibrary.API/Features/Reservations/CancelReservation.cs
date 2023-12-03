using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace MarthasLibrary.API.Features.Reservations;

/// <summary>
/// Provides functionality for canceling a book reservation.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and logic for canceling a specific book reservation.
/// </remarks>
public static class CancelReservation
{
  /// <summary>
  /// Represents a request to cancel a reservation.
  /// </summary>
  /// <param name="ReservationId">The unique identifier of the reservation to be canceled.</param>
  public record Request(Guid ReservationId) : IRequest;

  /// <summary>
  /// Handles the process of canceling a book reservation.
  /// </summary>
  /// <remarks>
  /// This class is responsible for validating the reservation's existence, updating the book's availability status, and removing the reservation.
  /// </remarks>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <param name="reservationRepository">Repository for accessing reservation entities.</param>
  /// <param name="logger">Logger for logging information and errors.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for any of the repositories or the logger.</exception>
  /// <exception cref="ReservationNotFoundException">Thrown when the specified reservation is not found.</exception>
  public class Handler(IGenericRepository<Book> bookRepository,
      IGenericRepository<Reservation> reservationRepository, ILogger<Handler> logger)
    : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IGenericRepository<Reservation> _reservationRepository =
      reservationRepository ?? throw new ArgumentException(nameof(reservationRepository));

    private readonly ILogger<Handler> _logger = logger ?? throw new ArgumentException(nameof(logger));

    /// <summary>
    /// Handles the incoming request to cancel a reservation.
    /// </summary>
    /// <param name="request">The request to cancel a reservation.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
      try
      {
        await _bookRepository.BeginTransactionAsync(cancellationToken);

        var reservation =
          await _reservationRepository.Table.FirstOrDefaultAsync(reservation => reservation.Id == request.ReservationId,
            cancellationToken);
        if (reservation is null)
        {
          throw new ReservationNotFoundException($"Could not find a reservation with ID {request.ReservationId}.");
        }

        var book = await _bookRepository.Table.FirstOrDefaultAsync(book => book.Id == reservation.BookId,
          cancellationToken);

        book?.MarkAsAvailable();
        await _bookRepository.SaveAsync(cancellationToken);

        _reservationRepository.Delete(reservation);
        await _reservationRepository.SaveAsync(cancellationToken);

        await _bookRepository.CommitTransactionAsync(cancellationToken);
      }
      catch (TransactionException)
      {
        _logger.LogError("Transaction failed... Could not cancel reservation.");
        await _bookRepository.RollbackTransactionAsync(cancellationToken);
        throw;
      }
    }
  }
}