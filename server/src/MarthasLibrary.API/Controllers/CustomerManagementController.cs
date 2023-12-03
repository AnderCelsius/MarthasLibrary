using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarthasLibrary.API.Controllers
{
  [Route("api/customers")]
  [ApiController]
  public class CustomerManagementController(IMediator mediator) : ControllerBase
  {
  }
}
