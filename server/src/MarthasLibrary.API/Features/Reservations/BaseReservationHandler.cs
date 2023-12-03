using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

public abstract class BaseReservationHandler(IGenericRepository<Reservation> reservationRepository,
  IGenericRepository<Book> bookRepository,
  IMapper mapper)
{
  protected readonly IGenericRepository<Book> BookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));
  protected readonly IMapper Mapper = mapper ?? throw new ArgumentException(nameof(mapper));

  protected async Task<IReadOnlyCollection<ReservationDetails>> GetReservationDetails(
    IEnumerable<Reservation> reservations, CancellationToken cancellationToken)
  {
    var enumerable = reservations.ToList();
    var bookIds = enumerable.Select(r => r.BookId).Distinct().ToList();
    var books = await BookRepository.TableNoTracking
      .Where(b => bookIds.Contains(b.Id))
      .ToListAsync(cancellationToken);

    var bookDictionary = books.ToDictionary(b => b.Id, b => b.Title);

    return enumerable.Select(reservation =>
    {
      var options = new Action<IMappingOperationOptions<Reservation, ReservationDetails>>(opts =>
      {
        opts.Items["Title"] = bookDictionary.TryGetValue(reservation.BookId, out var title) ? title : string.Empty;
      });

      return Mapper.Map(reservation, options);
    }).ToList();
  }
}
