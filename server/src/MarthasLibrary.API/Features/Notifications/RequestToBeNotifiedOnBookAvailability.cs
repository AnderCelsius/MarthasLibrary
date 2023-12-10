using MarthasLibrary.Application.UserData;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Enums;
using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Notifications;

public static class RequestToBeNotifiedOnBookAvailability
{
  public record Request(Guid BookId) : IRequest;

  public class Handler(IGenericRepository<Book> bookRepository,
      IGenericRepository<Notification> notificationRepository,
      IUserDataProvider<UserData> userDataProvider)
    : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IGenericRepository<Notification> _notificationRepository =
      notificationRepository ?? throw new ArgumentException(nameof(notificationRepository));

    private readonly IUserDataProvider<UserData> _userDataProvider =
      userDataProvider ?? throw new ArgumentException(nameof(userDataProvider));


    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
      var currentUserData = (await _userDataProvider
          .GetCurrentUserData(cancellationToken))
        .EnsureAuthenticated();

      var book = await _bookRepository.TableNoTracking
        .SingleOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

      if (book is null || book.Status != BookStatus.Reserved || book.Status != BookStatus.Borrowed)
      {
        throw new InvalidOperationException("Book is available for borrowing.");
      }

      var notification = new Notification(currentUserData.Id, book.Id, nameof(BookAvailableEvent));

      await _notificationRepository.InsertAsync(notification);
      await _notificationRepository.SaveAsync(cancellationToken);
    }
  }
}