using MarthasLibrary.API.Features.Borrow;
using MarthasLibrary.API.Features.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
  [Route("api/books")]
  [ApiController]
  public class BorrowManagementController(IMediator mediator) : ControllerBase
  {
    [HttpPost("/borrow", Name = "BorrowBook")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BorrowBook.Response>> BorrowBook(
      [FromBody] BorrowBook.Request request,
      CancellationToken cancellationToken)
    {
      try
      {
        return Ok(await mediator.Send(request, cancellationToken));
      }
      catch (BookNotAvailableException e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("/return/{borrowId}", Name = "ReturnBook")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Unit>> ReturnBook(
      [FromRoute] Guid borrowId,
      CancellationToken cancellationToken)
    {
      try
      {
        await mediator.Send(new ReturnBook.Request(borrowId), cancellationToken);
        return NoContent();
      }
      catch (BorrowNotFoundException ex)
      {
        return NotFound(ex.Message);
      }
    }

  }
}
