using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.API.Features.Reservations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
  [Route("api/books/reserve")]
  [ApiController]
  public class ReservationManagementController(IMediator mediator) : ControllerBase
  {
    [HttpGet("/reservations", Name = "GetAllReservations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GetAll.Response>> GetAllReservations(
      CancellationToken cancellationToken)
    {
      return await mediator.Send(new GetAll.Request(), cancellationToken);
    }

    [HttpGet("/reserve/{customerId}", Name = "GetReservationsForCustomer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetReservationsByCustomerId.Response>> GetReservationsForCustomer(
      [FromRoute] Guid customerId,
      CancellationToken cancellationToken)
    {
      return Ok(await mediator.Send(new GetReservationsByCustomerId.Request(customerId), cancellationToken));
    }

    [HttpGet("/reservation/{reservationId}", Name = "GetReservationById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetById.Response>> GetReservationById(
      [FromRoute] Guid reservationId,
      CancellationToken cancellationToken)
    {
      try
      {
        return Ok(await mediator.Send(new GetById.Request(reservationId), cancellationToken));
      }
      catch (ReservationNotFoundException e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("/reserve", Name = "ReserveBook")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MakeReservation.Response>> ReserveBook(
      [FromBody] MakeReservation.Request request,
      CancellationToken cancellationToken)
    {
      try
      {
        var response = await mediator.Send(request, cancellationToken);
        return Created(new Uri($"/books/reserve/{response.ReservationDetails.ReservationId}", UriKind.Relative),
          response);
      }
      catch (BookNotAvailableException e)
      {
        return BadRequest(e.Message);
      }
      catch (ConcurrencyConflictException)
      {
        return Conflict(
          "The operation could not be completed due to a conflicting update by another user. Please reload the data and try again.");
      }
    }

    [HttpDelete("/reserve/{reservationId}", Name = "CancelReservation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Unit>> CancelReservation(
      [FromRoute] Guid reservationId,
      CancellationToken cancellationToken)
    {
      try
      {
        await mediator.Send(new CancelReservation.Request(reservationId), cancellationToken);
        return NoContent();
      }
      catch (ReservationNotFoundException ex)
      {
        return NotFound(ex.Message);
      }
    }
  }
}