using AutoMapper;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Books;

public static class GetAll
{
  public record Request() : IRequest<Response>;

  public record Response(IReadOnlyCollection<Response.Book> Books)
  {
    public record Book(
      Guid Id,
      string Title,
      string Author,
      string Isbn,
      DateTimeOffset PublishedDate);
  }

  public class Handler : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository;

    private readonly IMapper _mapper;

    public Handler(IMapper mapper, IGenericRepository<Book> bookRepository)
    {
      _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
      _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var books = await _bookRepository.TableNoTracking.ToListAsync(cancellationToken);

      return _mapper.Map<Response>(books);
    }
  }
}