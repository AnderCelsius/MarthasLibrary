using MarthasLibrary.API.Features.Books;
using MarthasLibrary.API.Features.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
  [Route("api/books")]
  [ApiController]
  //[Authorize]
  public class BookManagementController(IMediator mediator) : ControllerBase
  {
    [AllowAnonymous]
    [HttpGet(Name = "GetAllBooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GetAll.Response>> GetAllBooks(
      CancellationToken cancellationToken)
    {
      return await mediator.Send(new GetAll.Request(), cancellationToken);
    }

    [HttpGet("search", Name = "Search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Search.Response>> Search([FromQuery] string query,
      CancellationToken cancellationToken)
    {
      return await mediator.Send(new Search.Request(query), cancellationToken);
    }

    [HttpPost(Name = "CreateBook")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Create.Response>> CreateBook(
      [FromBody] Create.Request request,
      CancellationToken cancellationToken)
    {
      try
      {
        var response = await mediator.Send(request, cancellationToken);
        return Created(new Uri($"/books/{response.Id}", UriKind.Relative), response);
      }
      catch (BookWithIsbnAlreadyExistsException e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("{bookId}", Name = "GetBookById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetById.Response>> GetBookById(
      [FromRoute] Guid bookId,
      CancellationToken cancellationToken)
    {
      try
      {
        return await mediator.Send(new GetById.Request(bookId), cancellationToken);
      }
      catch (BookNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpPut("{bookId}", Name = "UpdateBookById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateBookById(
      [FromRoute] Guid bookId,
      [FromBody] UpdateById.Request.UpdatedDetails updatedDetails,
      CancellationToken cancellationToken)
    {
      try
      {
        await mediator.Send(new UpdateById.Request(bookId, updatedDetails), cancellationToken);
        return NoContent();
      }
      catch (BookWithIsbnAlreadyExistsException e)
      {
        return BadRequest(e.Message);
      }
      catch (BookNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpDelete("{bookId}", Name = "DeleteBook")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Unit>> DeleteBook(
      [FromRoute] Guid bookId,
      CancellationToken cancellationToken)
    {
      try
      {
        await mediator.Send(new DeleteById.Request(bookId), cancellationToken);
        return NoContent();
      }
      catch (BookNotFoundException ex)
      {
        return NotFound(ex.Message);
      }
    }
  }
}