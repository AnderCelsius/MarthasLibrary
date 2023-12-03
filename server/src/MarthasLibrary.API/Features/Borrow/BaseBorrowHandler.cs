using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Borrow;

public abstract class BaseBorrowHandler(
  IGenericRepository<Book> bookRepository,
  IMapper mapper)
{
  protected readonly IGenericRepository<Book> BookRepository =
    bookRepository ?? throw new ArgumentException(nameof(bookRepository));

  protected readonly IMapper Mapper = mapper ?? throw new ArgumentException(nameof(mapper));

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