﻿using AutoMapper;
using FluentValidation;
using MarthasLibrary.API.Features.Exceptions;
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
    private readonly IGenericRepository<Book> _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));
    private readonly IGenericRepository<Reservation> _reservationRepository = reservationRepository ?? throw new ArgumentException(nameof(reservationRepository));
    private readonly ILogger<Handler> _logger = logger ?? throw new ArgumentException(nameof(logger));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      try
      {
        await bookRepository.BeginTransactionAsync(cancellationToken);

        var book = await _bookRepository.Table.FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);
        if (book is null)
        {
          throw new BookNotAvailableException("Book is not available.");
        }

        var reservation = Reservation.CreateInstance(request.BookId, request.CustomerId);
        await _reservationRepository.InsertAsync(reservation);
        await _reservationRepository.SaveAsync(cancellationToken);

        book.MarkAsReserved();
        try
        {
          await _bookRepository.SaveAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
          _logger.LogWarning("Concurrency conflict occurred when trying to reserve the book.");
          throw new ConcurrencyConflictException("The book has been modified by another transaction.");
        }

        await _bookRepository.CommitTransactionAsync(cancellationToken);

        return _mapper.Map<Response>(book);
      }
      catch
      {
        _logger.LogError("Transaction failed... Could not make reservation.");
        await _bookRepository.RollbackTransactionAsync(cancellationToken);
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