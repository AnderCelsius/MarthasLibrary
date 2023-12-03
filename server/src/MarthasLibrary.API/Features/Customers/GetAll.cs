using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Customers;

public static class GetAll
{
  public record Request() : IRequest<Response>;

  public record Response(IReadOnlyCollection<BookDetails> Books);

  public class Handler(IMapper mapper, IGenericRepository<Book> bookRepository) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var books = await _bookRepository.TableNoTracking.ToListAsync(cancellationToken);

      return _mapper.Map<Response>(books);
    }
  }
}