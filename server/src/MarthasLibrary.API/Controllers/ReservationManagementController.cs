﻿using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.API.Features.Reservations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
  [Route("api/books/reserve")]
  [ApiController]
  public class ReservationManagementController(IMediator mediator) : ControllerBase
  {
    [HttpGet("{customerId}", Name = "GetReservationsForCustomer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetReservationByCustomerId.Response>> GetReservationsForCustomer(
      [FromRoute] Guid customerId,
      CancellationToken cancellationToken)
    {
      return Ok(await mediator.Send(new GetReservationByCustomerId.Request(customerId), cancellationToken));
    }

    [HttpPost(Name = "ReserveBook")]
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
        return Created(new Uri($"/books/reserve/{response.ReservationDetails.ReservationId}", UriKind.Relative), response);
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

    [HttpDelete("{reservationId}", Name = "CancelReservation")]
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