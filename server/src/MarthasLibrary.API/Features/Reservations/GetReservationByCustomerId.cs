using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

public static class GetReservationByCustomerId
{
  public record Request(Guid CustomerId) : IRequest<Response>;

  public record Response(IReadOnlyCollection<ReservationDetails> Reservations);

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

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var reservations = await _reservationRepository.TableNoTracking
        .Where(r => r.Id == request.CustomerId)
        .ToListAsync(cancellationToken);

      var bookIds = reservations.Select(r => r.BookId).Distinct().ToList();

      var books = await _bookRepository.TableNoTracking
        .Where(b => bookIds.Contains(b.Id))
        .ToListAsync(cancellationToken);

      var bookDictionary = books.ToDictionary(b => b.Id, b => b.Title);

      var reservationDetails = reservations.Select(reservation =>
      {
        var options = new Action<IMappingOperationOptions<Reservation, ReservationDetails>>(opts =>
        {
          opts.Items["Title"] = bookDictionary.TryGetValue(reservation.BookId, out var title) ? title : string.Empty;
        });

        return _mapper.Map(reservation, options);
      }).ToList();

      return new Response(reservationDetails);
    }
  }
}