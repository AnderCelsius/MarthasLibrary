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
  }
}
