using AutoMapper;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Enums;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Borrow;

public static class BorrowBook
{
  public record Request(Guid CustomerId, Guid BookId) : IRequest<Response>;

  public record Response(
    Guid BorrowId);

  public class Handler(IGenericRepository<Book> bookRepository, IMapper mapper,
      IGenericRepository<Core.Entities.Borrow> borrowRepository, ILogger<Handler> logger)
    : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IGenericRepository<Core.Entities.Borrow> _borrowRepository =
      borrowRepository ?? throw new ArgumentException(nameof(borrowRepository));

    private readonly ILogger<Handler> _logger = logger ?? throw new ArgumentException(nameof(logger));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      try
      {
        await _bookRepository.BeginTransactionAsync(cancellationToken);

        var book = await _bookRepository.Table
          .SingleOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

        if (book is null || book.Status != BookStatus.Available)
        {
          throw new BookNotAvailableException("Book is not available for borrowing.");
        }

        var borrow =
          Core.Entities.Borrow.CreateInstance(request.BookId, request.CustomerId, DateTimeOffset.UtcNow.AddDays(14));

        await _borrowRepository.InsertAsync(borrow);
        await _bookRepository.SaveAsync(cancellationToken);

        book.MarkAsBorrowed();
        _bookRepository.Update(book);
        await _bookRepository.SaveAsync(cancellationToken);

        await _bookRepository.CommitTransactionAsync(cancellationToken);

        return _mapper.Map<Response>(borrow);
      }
      catch
      {
        _logger.LogError("Transaction failed... Could not make reservation.");
        await _bookRepository.RollbackTransactionAsync(cancellationToken);
        throw;
      }
    }
  }
}