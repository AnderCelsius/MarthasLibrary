using AutoMapper;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Enums;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace MarthasLibrary.API.Features.Borrow;

/// <summary>
/// Provides functionality for borrowing a book based on a reservation.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request, response, and processing logic for borrowing a book.
/// </remarks>
public static class BorrowBook
{
    /// <summary>
    /// Represents a request to borrow a book.
    /// </summary>
    /// <param name="ReservationId">The unique identifier of the reservation for the book to be borrowed.</param>
    public record Request(Guid ReservationId) : IRequest<Response>;

    /// <summary>
    /// Represents the response containing the details of the borrowed book.
    /// </summary>
    /// <param name="BorrowDetails">Details of the borrowed book.</param>
    public record Response(BorrowDetails BorrowDetails);

    /// <summary>
    /// Handles the process of borrowing a book based on a reservation.
    /// </summary>
    /// <remarks>
    /// This class is responsible for validating the reservation, updating the book's status, and creating a borrow record.
    /// </remarks>
    /// <param name="bookRepository">Repository for accessing book entities.</param>
    /// <param name="mapper">An instance of AutoMapper for object mapping.</param>
    /// <param name="reservationRepository">Repository for accessing reservation entities.</param>
    /// <param name="borrowRepository">Repository for accessing borrow entities.</param>
    /// <param name="logger">Logger for logging information and errors.</param>
    /// <exception cref="ArgumentException">Thrown when a null argument is passed for any repository, the logger, or the mapper.</exception>
    public class Handler(IGenericRepository<Book> bookRepository,
            IGenericRepository<Reservation> reservationRepository,
            IGenericRepository<Core.Entities.Borrow> borrowRepository,
            IMapper mapper,
            IMediator mediator,
            ILogger<Handler> logger)
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

        private readonly IMediator _mediator = mediator ?? throw new ArgumentException(nameof(mediator));


        /// <summary>
        /// Handles the incoming request to borrow a book.
        /// </summary>
        /// <remarks>
        /// This method processes the request by validating the reservation, updating the book's status to borrowed, and creating a borrow record.
        /// It also handles transaction management and logs any transaction-related errors.
        /// </remarks>
        /// <param name="request">The request to borrow a book.</param>
        /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the response containing the borrow details.</returns>
        /// <exception cref="ReservationNotFoundException">Thrown when the reservation is not found.</exception>
        /// <exception cref="BookNotAvailableException">Thrown when the book is not available for borrowing.</exception>
        /// <exception cref="TransactionException">Thrown when there is an issue with the transaction.</exception>
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
                    Core.Entities.Borrow.CreateInstance(reservation.BookId, reservation.CustomerId,
                        DateTimeOffset.UtcNow.AddDays(14));

                await _borrowRepository.InsertAsync(borrow);
                await _bookRepository.SaveAsync(cancellationToken);

                book.MarkAsBorrowed();
                _bookRepository.Update(book);
                await _bookRepository.SaveAsync(cancellationToken);

                _reservationRepository.Delete(reservation);
                await _reservationRepository.SaveAsync(cancellationToken);

                await _bookRepository.CommitTransactionAsync(cancellationToken);

                await _mediator.Publish(new BookBorrowedEvent(borrow.BookId, borrow.CustomerId), cancellationToken);

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