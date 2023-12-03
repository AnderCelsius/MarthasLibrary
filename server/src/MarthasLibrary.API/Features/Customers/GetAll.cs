using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Customers;

public static class GetAll
{
  public record Request() : IRequest<Response>;

  public record Response(IReadOnlyCollection<CustomerDetails> Customers);

  public class Handler
    (IMapper mapper, IGenericRepository<Customer> customerRepository) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Customer> _customerRepository =
      customerRepository ?? throw new ArgumentException(nameof(customerRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var customerDetails = await _customerRepository.TableNoTracking
        .Where(c => c.IsActive)
        .ProjectTo<CustomerDetails>(_mapper.ConfigurationProvider)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

      return new Response(customerDetails.AsReadOnly());
    }
  }
}