using AutoMapper;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

public static class CancelReservation
{
  public record Request(Guid ReservationId) : IRequest;

  public class Handler(IGenericRepository<Book> bookRepository, IMapper mapper,
      IGenericRepository<Reservation> reservationRepository, ILogger<Handler> logger)
    : IRequestHandler<Request>
  {
    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
      try
      {
        await bookRepository.BeginTransactionAsync(cancellationToken);

        var reservation =
          await reservationRepository.Table.FirstOrDefaultAsync(reservation => reservation.Id == request.ReservationId,
            cancellationToken);
        if (reservation is null)
        {
          throw new ReservationNotFoundException($"Could not find a reservation with ID {request.ReservationId}.");
        }

        var book = await bookRepository.Table.FirstOrDefaultAsync(book => book.Id == reservation.BookId,
          cancellationToken);

        book?.MarkAsAvailable();
        await bookRepository.SaveAsync(cancellationToken);

        reservationRepository.Delete(reservation);
        await reservationRepository.SaveAsync(cancellationToken);

        await bookRepository.CommitTransactionAsync(cancellationToken);
      }
      catch (Exception e)
      {
        logger.LogError("Transaction failed... Could not cancel reservation.");
        await bookRepository.RollbackTransactionAsync(cancellationToken);
        throw;
      }
    }
  }
}