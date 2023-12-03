using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Borrow;

public static class GetAll
{
  public record Request() : IRequest<Response>;

  public record Response(IReadOnlyCollection<BorrowDetails> Books);

  public class Handler
    (IMapper mapper, IGenericRepository<Core.Entities.Borrow> borrowRepository) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Core.Entities.Borrow> _borrowRepository =
      borrowRepository ?? throw new ArgumentException(nameof(borrowRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var books = await _borrowRepository.TableNoTracking.ToListAsync(cancellationToken);

      return _mapper.Map<Response>(books);
    }
  }
}