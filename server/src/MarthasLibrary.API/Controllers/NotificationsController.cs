using MarthasLibrary.API.Features.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers;

[Route("api/notifications")]
[ApiController]
public class NotificationsController(IMediator mediator) : ControllerBase
{
  [HttpGet(Name = "GetNotificationsForCurrentUser")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<GetNotificationsForCurrentUser.Response>> GetNotificationsForCurrentUser(
    CancellationToken cancellationToken)
  {
    return await mediator.Send(new GetNotificationsForCurrentUser.Request(), cancellationToken);
  }
}