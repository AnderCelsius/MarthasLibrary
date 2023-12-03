using AutoMapper;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations
{
  public static class GetById
  {
    public record Request(Guid ReservationId) : IRequest<Response>;

    public record Response(ReservationDetails Reservation);

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
}