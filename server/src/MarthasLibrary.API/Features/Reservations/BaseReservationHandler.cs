using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

/// <summary>
/// Serves as the base class for handling reservation-related operations in the library system.
/// </summary>
/// <remarks>
/// This abstract class provides core functionality common to reservation handlers, including accessing book repositories and mapping entities.
/// </remarks>
/// <param name="reservationRepository">A generic repository for accessing reservation entities.</param>
/// <param name="bookRepository">A generic repository for accessing book entities.</param>
/// <param name="mapper">An instance of AutoMapper to map between different object types.</param>
/// <exception cref="ArgumentException">Thrown when a null argument is passed for either the book repository or the mapper.</exception>
public abstract class BaseReservationHandler(IGenericRepository<Reservation> reservationRepository,
  IGenericRepository<Book> bookRepository,
  IMapper mapper)
{
  protected readonly IGenericRepository<Book> BookRepository =
    bookRepository ?? throw new ArgumentException(nameof(bookRepository));

  protected readonly IMapper Mapper = mapper ?? throw new ArgumentException(nameof(mapper));


  /// <summary>
  /// Retrieves a collection of reservation details for a given set of reservation records.
  /// </summary>
  /// <remarks>
  /// This method asynchronously fetches detailed information about each reservation, including the book title, by querying the book repository and using AutoMapper for mapping.
  /// </remarks>
  /// <param name="reservations">A collection of reservation records from which to retrieve details.</param>
  /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
  /// <returns>A task representing the asynchronous operation, with a result of a read-only collection of reservation details.</returns>
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