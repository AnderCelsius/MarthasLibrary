using MarthasLibrary.API.Features.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers;

[Authorize]
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

  [HttpPost("subscribe/{bookId}", Name = "SubscribeToBookAvailabilityAlert")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<Unit>> SubscribeToBookAvailabilityAlert(
    [FromRoute] Guid bookId,
    CancellationToken cancellationToken)
  {
    await mediator.Send(new SubscribeToBookAvailabilityAlert.Request(bookId), cancellationToken);
    return Ok();
  }
}