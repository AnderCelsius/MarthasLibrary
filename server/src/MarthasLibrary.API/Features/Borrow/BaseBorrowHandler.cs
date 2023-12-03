using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Borrow;

/// <summary>
/// Represents the base handler for borrow-related operations in Martha's Library API.
/// </summary>
/// <param name="bookRepository">An instance of a generic repository for accessing book entities</param>
/// <param name="mapper">An instance of AutoMapper to map between different object types.</param>
/// <exception cref="ArgumentException">Thrown when a null argument is passed for either the book repository or the mapper.</exception>
/// <remarks>
/// This abstract class provides core functionality to handle borrow operations,
/// leveraging AutoMapper for object mapping and a generic repository pattern for data access.
/// </remarks>
public abstract class BaseBorrowHandler(
  IGenericRepository<Book> bookRepository,
  IMapper mapper)
{
  protected readonly IGenericRepository<Book> BookRepository =
    bookRepository ?? throw new ArgumentException(nameof(bookRepository));

  protected readonly IMapper Mapper = mapper ?? throw new ArgumentException(nameof(mapper));


  /// <summary>
  /// Retrieves a collection of borrow details for a given set of borrow records.
  /// </summary>
  /// <remarks>
  /// This method asynchronously fetches detailed information about each borrow record,
  /// including the book title, by querying the book repository and using AutoMapper for mapping.
  /// </remarks>
  /// <param name="borrows">A collection of borrow records from which to retrieve details.</param>
  /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
  /// <returns>A task representing the asynchronous operation, with a result of a read-only collection of borrow details.</returns>
  /// <example>
  /// To use this method, provide a collection of <c>Borrow</c> instances and a <c>CancellationToken</c>. 
  /// The method returns detailed information about each borrow, including book titles.
  /// </example>
  protected async Task<IReadOnlyCollection<BorrowDetails>> GetBorrowDetails(
    IEnumerable<Core.Entities.Borrow> borrows, CancellationToken cancellationToken)
  {
    var enumerable = borrows.ToList();
    var bookIds = enumerable.Select(r => r.BookId).Distinct().ToList();
    var books = await BookRepository.TableNoTracking
      .Where(b => bookIds.Contains(b.Id))
      .ToListAsync(cancellationToken);

    var bookDictionary = books.ToDictionary(b => b.Id, b => b.Title);

    return enumerable.Select(borrow =>
    {
      var options = new Action<IMappingOperationOptions<Core.Entities.Borrow, BorrowDetails>>(opts =>
      {
        opts.Items["Title"] = bookDictionary.TryGetValue(borrow.BookId, out var title) ? title : string.Empty;
      });

      return Mapper.Map(borrow, options);
    }).ToList();
  }
}