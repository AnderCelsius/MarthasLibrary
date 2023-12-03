using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

public static class CancelReservation
{
  public record Request(Guid ReservationId) : IRequest;

  public class Handler(IGenericRepository<Book> bookRepository,
      IGenericRepository<Reservation> reservationRepository, ILogger<Handler> logger)
    : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));
    private readonly IGenericRepository<Reservation> _reservationRepository = reservationRepository ?? throw new ArgumentException(nameof(reservationRepository));
    private readonly ILogger<Handler> _logger = logger ?? throw new ArgumentException(nameof(logger));

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
      catch (Exception e)
      {
        _logger.LogError("Transaction failed... Could not cancel reservation.");
        await _bookRepository.RollbackTransactionAsync(cancellationToken);
        throw;
      }
    }
  }
}