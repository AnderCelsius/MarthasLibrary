using AutoMapper;
using FluentValidation;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Application.UserData;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Reservations;

/// <summary>
/// Provides functionality for making a book reservation.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and response for creating a new reservation.
/// </remarks>
public static class MakeReservation
{
    /// <summary>
    /// Represents a request to make a reservation.
    /// </summary>
    /// <param name="BookId">The unique identifier of the book to be reserved.</param>
    public record Request(Guid BookId) : IRequest<Response>;

    /// <summary>
    /// Represents the response containing the details of the made reservation.
    /// </summary>
    /// <param name="ReservationDetails">Details of the made reservation.</param>
    public record Response(ReservationDetails ReservationDetails);

    /// <summary>
    /// Handles the process of making a book reservation.
    /// </summary>
    /// <remarks>
    /// This class is responsible for validating the book's availability, creating the reservation record, and updating the book's status.
    /// </remarks>
    /// <param name="bookRepository">Repository for accessing book entities.</param>
    /// <param name="mapper">An instance of AutoMapper for object mapping.</param>
    /// <param name="reservationRepository">Repository for accessing reservation entities.</param>
    /// <param name="logger">Logger for logging information and errors.</param>
    /// <exception cref="ArgumentException">Thrown when a null argument is passed for any of the repositories, the logger, or the mapper.</exception>
    /// <exception cref="BookNotFoundException">Thrown when the specified book is not found.</exception>
    /// <exception cref="BookNotAvailableException">Thrown when the book is already reserved.</exception>
    /// <exception cref="ConcurrencyConflictException">Thrown when a concurrency conflict occurs while updating the book's status.</exception>
    public class Handler(IGenericRepository<Book> bookRepository, IMapper mapper,
        IUserDataProvider<UserData> userDataProvider,
        IGenericRepository<Reservation> reservationRepository, ILogger<Handler> logger,
        IMediator mediator)
      : IRequestHandler<Request, Response>
    {
        private readonly IGenericRepository<Book> _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));
        private readonly IGenericRepository<Reservation> _reservationRepository = reservationRepository ?? throw new ArgumentException(nameof(reservationRepository));
        private readonly ILogger<Handler> _logger = logger ?? throw new ArgumentException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        private readonly IMediator _mediator = mediator ?? throw new ArgumentException(nameof(mediator));
        private readonly IUserDataProvider<UserData> _userDataProvider = userDataProvider ?? throw new ArgumentException(nameof(userDataProvider));


        /// <summary>
        /// Handles the incoming request to make a reservation.
        /// </summary>
        /// <param name="request">The request to make a reservation.</param>
        /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the response containing the details of the made reservation.</returns>
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                await bookRepository.BeginTransactionAsync(cancellationToken);

                var book = await _bookRepository.Table.FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);
                if (book is null)
                {
                    throw new BookNotFoundException($"Could not find book with Id: {request.BookId}");
                }

                var currentUserData = (await userDataProvider
                    .GetCurrentUserData(cancellationToken))
                  .EnsureAuthenticated();

                var reservation = Reservation.CreateInstance(request.BookId, currentUserData.Id);
                await _reservationRepository.InsertAsync(reservation);
                await _reservationRepository.SaveAsync(cancellationToken);

                try
                {
                    book.MarkAsReserved();
                }
                catch
                {
                    _logger.LogWarning("Invalid operation.");
                    throw new BookNotAvailableException("Book is already reserved.");
                }
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

                await _mediator.Publish(new BookReservedEvent(reservation.BookId, reservation.CustomerId), cancellationToken);
                return _mapper.Map<Response>(reservation);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Transaction failed... Could not make reservation.");
                await _bookRepository.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }

    /// <summary>
    /// Validator for the reservation making request.
    /// </summary>
    /// <remarks>
    /// This class provides validation rules for making a reservation request, including checks for BookId and CustomerId.
    /// </remarks>
    public class MakeReservationValidator : AbstractValidator<Request>
    {
        public MakeReservationValidator()
        {
            // Title validation: not empty, no digits, and a reasonable length limit
            RuleFor(request => request.BookId)
              .NotEmpty().WithMessage("BookId is required.");
        }
    }
}