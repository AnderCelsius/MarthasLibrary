using AutoMapper;
using MarthasLibrary.API.Shared;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Features.Books;

public static class GetById
{
  public record Request(Guid BookId) : IRequest<Response>;

  public record Response(BookDetails Book);

  public class Handler(IGenericRepository<Book> bookRepository, IMapper mapper) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var book = await _bookRepository.Table
        .SingleOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

      if (book is null)
      {
        throw new BookNotFoundException($"Could not find a user with ID {request.BookId}.");
      }

      return _mapper.Map<Response>(book);
    }
  }
}
