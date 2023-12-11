using MarthasLibrary.API.Features.Borrow;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
  [Authorize]
  [Route("api/books")]
  [ApiController]
  [ServiceFilter(typeof(CustomerAuthorizationFilter))]
  public class BorrowManagementController(IMediator mediator) : ControllerBase
  {
    [AllowAnonymous]
    [HttpGet("/borrowed", Name = "GetAllBorrowedBooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GetAll.Response>> GetAllBorrowedBooks(
      CancellationToken cancellationToken)
    {
      return await mediator.Send(new GetAll.Request(), cancellationToken);
    }

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

    [HttpGet("/borrow/history/{customerId}", Name = "GetBorrowingsByCustomerId")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetByCustomerId.Response>> GetBorrowingsByCustomerId(
      [FromRoute] Guid customerId,
      CancellationToken cancellationToken)
    {
      return Ok(await mediator.Send(new GetByCustomerId.Request(customerId), cancellationToken));
    }

    [HttpGet("/borrow/history/_me", Name = "GetBorrowingsForCurrentCustomer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetBorrowingForCurrentUser.Response>> GetBorrowingsForCurrentCustomer(
      CancellationToken cancellationToken)
    {
      return Ok(await mediator.Send(new GetBorrowingForCurrentUser.Request(), cancellationToken));
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