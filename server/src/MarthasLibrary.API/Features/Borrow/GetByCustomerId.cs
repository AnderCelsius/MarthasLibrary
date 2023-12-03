using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Borrow;

public static class GetByCustomerId
{
  public record Request(Guid CustomerId) : IRequest<Response>;

  public record Response(IReadOnlyCollection<BorrowDetails> Reservations);

  public class Handler
  (IGenericRepository<Core.Entities.Borrow> borrowRepository,
    IGenericRepository<Book> bookRepository,
    IMapper mapper) : BaseBorrowHandler(bookRepository, mapper), IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Core.Entities.Borrow> _borrowRepository =
      borrowRepository ?? throw new ArgumentException(nameof(borrowRepository));

    private readonly IGenericRepository<Book> _bookRepository =
      bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var borrows = await _borrowRepository.TableNoTracking
        .Where(r => r.Id == request.CustomerId)
        .ToListAsync(cancellationToken);

      var borrowDetails = await GetBorrowDetails(borrows, cancellationToken);

      return new Response(borrowDetails);
    }
  }
}