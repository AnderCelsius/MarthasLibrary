using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
    [Route("api/books/reserve")]
    [ApiController]
    public class ReservationManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "ReserveBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Create.Response>> ReserveBook(
            [FromBody] Create.Request request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(request, cancellationToken);
                return Created(new Uri($"/books/{response.Id}", UriKind.Relative), response);
            }
            catch (BookWithIsbnAlreadyExistsException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
