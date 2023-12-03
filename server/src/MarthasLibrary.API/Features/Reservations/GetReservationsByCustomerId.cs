using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

public static class GetReservationsByCustomerId
{
  public record Request(Guid CustomerId) : IRequest<Response>;

  public record Response(IReadOnlyCollection<ReservationDetails> Reservations);

  public class Handler
  (IGenericRepository<Reservation> reservationRepository,
    IGenericRepository<Book> bookRepository,
    IMapper mapper) : BaseReservationHandler(reservationRepository, bookRepository, mapper), IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Reservation> _reservationRepository =
      reservationRepository ?? throw new ArgumentException(nameof(reservationRepository));

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