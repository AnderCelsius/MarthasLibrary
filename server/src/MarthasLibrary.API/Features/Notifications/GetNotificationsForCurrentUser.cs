using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Application.UserData;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Notifications;

public class GetNotificationsForCurrentUser
{
  /// <summary>
  /// Represents a request for retrieving notifications by customer ID.
  /// </summary>
  /// <remarks>
  /// This record is used to indicate a request for notifications for a specific customer.
  /// </remarks>
  public record Request() : IRequest<Response>;

  /// <summary>
  /// Represents the response containing borrow records for a specific customer.
  /// </summary>
  /// <param name="Notifications">A read-only collection of notifications for the specified customer.</param>
  /// <remarks>
  /// This record encapsulates the response data for a request to fetch notifications for a specific customer, providing the details as a read-only collection.
  /// </remarks>
  public record Response(IReadOnlyCollection<NotificationDetails> Notifications);

  public class Handler
  (IGenericRepository<Notification> notificationRepository,
    IUserDataProvider<UserData> userDataProvider,
    IMapper mapper) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Notification> _notificationRepository =
      notificationRepository ?? throw new ArgumentException(nameof(notificationRepository));

    private readonly IUserDataProvider<UserData> _userDataProvider =
      userDataProvider ?? throw new ArgumentException(nameof(userDataProvider));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    /// <summary>
    /// Handles the incoming request to retrieve notifications for a specific customer.
    /// </summary>
    /// <remarks>
    /// This method asynchronously processes the request to fetch notifications for a specific customer, returning the relevant details.
    /// </remarks>
    /// <param name="request">The request to retrieve notifications for a specific customer.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing the notifications for the specified customer.</returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var currentUserData = (await userDataProvider
          .GetCurrentUserData(cancellationToken))
        .EnsureAuthenticated();

      var notifications = await _notificationRepository.TableNoTracking
        .Where(r => r.Id == currentUserData.Id)
        .ToListAsync(cancellationToken);

      return _mapper.Map<Response>(notifications);
    }
  }
}