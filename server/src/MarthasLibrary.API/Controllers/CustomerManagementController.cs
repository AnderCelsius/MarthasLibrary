using MarthasLibrary.API.Features.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
  [Route("api/customers")]
  [ApiController]
  public class CustomerManagementController(IMediator mediator) : ControllerBase
  {
    [HttpGet(Name = "GetAllCustomers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GetAll.Response>> GetAllCustomers(
      CancellationToken cancellationToken)
    {
      return await mediator.Send(new GetAll.Request(), cancellationToken);
    }
  }
}
