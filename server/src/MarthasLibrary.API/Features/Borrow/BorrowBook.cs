using AutoMapper;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Enums;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace MarthasLibrary.API.Features.Borrow;

public static class BorrowBook
{
  public record Request(Guid ReservationId) : IRequest<Response>;

  public record Response(BorrowDetails BorrowDetails);

  public class Handler(IGenericRepository<Book> bookRepository, IMapper mapper,
      IGenericRepository<Reservation> reservationRepository,
      IGenericRepository<Core.Entities.Borrow> borrowRepository, ILogger<Handler> logger)
    : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IGenericRepository<Reservation> _reservationRepository =
      reservationRepository ?? throw new ArgumentException(nameof(reservationRepository));

    private readonly IGenericRepository<Core.Entities.Borrow> _borrowRepository =
      borrowRepository ?? throw new ArgumentException(nameof(borrowRepository));

    private readonly ILogger<Handler> _logger = logger ?? throw new ArgumentException(nameof(logger));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      try
      {
        await _bookRepository.BeginTransactionAsync(cancellationToken);

        var reservation =
          await _reservationRepository.Table.SingleOrDefaultAsync(r => r.Id == request.ReservationId,
            cancellationToken);

        if (reservation is null)
        {
          throw new ReservationNotFoundException();
        }

        var book = await _bookRepository.Table
          .SingleOrDefaultAsync(book => book.Id == reservation.BookId, cancellationToken);

        if (book is null || book.Status != BookStatus.Reserved)
        {
          throw new BookNotAvailableException("Book is not available for borrowing.");
        }

        var borrow =
          Core.Entities.Borrow.CreateInstance(reservation.BookId, reservation.CustomerId, DateTimeOffset.UtcNow.AddDays(14));

        await _borrowRepository.InsertAsync(borrow);
        await _bookRepository.SaveAsync(cancellationToken);

        book.MarkAsBorrowed();
        _bookRepository.Update(book);
        await _bookRepository.SaveAsync(cancellationToken);

        _reservationRepository.Delete(reservation);
        await _reservationRepository.SaveAsync(cancellationToken);

        await _bookRepository.CommitTransactionAsync(cancellationToken);

        return _mapper.Map<Response>(borrow);
      }
      catch (TransactionException)
      {
        _logger.LogError("Transaction failed... Could not make reservation.");
        await _bookRepository.RollbackTransactionAsync(cancellationToken);
        throw;
      }
    }
  }
}