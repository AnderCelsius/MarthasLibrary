using MarthasLibrary.API.Features.Books;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BooksController : ControllerBase
  {
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpGet(Name = "GetAllBooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GetAll.Response>> GetAllBooks(
      CancellationToken cancellationToken)
    {
      return await _mediator.Send(new GetAll.Request(), cancellationToken);
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
        var response = await _mediator.Send(request, cancellationToken);
        return Created(new Uri($"/books/{response.Id}", UriKind.Relative), response);
      }
      catch (BookAlreadyExistsException e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}
