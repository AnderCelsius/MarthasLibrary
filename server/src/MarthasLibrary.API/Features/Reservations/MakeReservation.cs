using AutoMapper;
using FluentValidation;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

public static class MakeReservation
{
  public record Request(Guid CustomerId, Guid BookId) : IRequest<Response>;

  public record Response(
    Guid ReservationId);

  public class Handler(IGenericRepository<Book> bookRepository, IMapper mapper,
      IGenericRepository<Reservation> reservationRepository, ILogger<Handler> logger)
    : IRequestHandler<Request, Response>
  {
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      try
      {
        await bookRepository.BeginTransactionAsync(cancellationToken);

        var book = await bookRepository.Table.FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);
        if (book is null)
        {
          throw new BookNotAvailableException("Book is not available.");
        }

        var reservation = Reservation.CreateInstance(request.BookId, request.CustomerId);
        await reservationRepository.InsertAsync(reservation);
        await reservationRepository.SaveAsync(cancellationToken);

        book.MarkAsReserved();
        await bookRepository.SaveAsync(cancellationToken);

        await bookRepository.CommitTransactionAsync(cancellationToken);

        return mapper.Map<Response>(book);
      }
      catch (Exception e)
      {
        logger.LogError("Transaction failed... Could not make reservation.");
        await bookRepository.RollbackTransactionAsync(cancellationToken);
        throw;
      }
    }
  }

  public class MakeReservationValidator : AbstractValidator<Request>
  {
    public MakeReservationValidator()
    {
      // Title validation: not empty, no digits, and a reasonable length limit
      RuleFor(request => request.BookId)
        .NotEmpty().WithMessage("BookId is required.");

      // Author validation: not empty and no digits
      RuleFor(request => request.CustomerId)
        .NotEmpty().WithMessage("CustomerId is required.");
    }
  }
}