using AutoMapper;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;

namespace MarthasLibrary.API.Features.Books;
public class Create
{
  public record Request(string Title, string Author, string Isbn, DateTimeOffset PublishedDate) : IRequest<Response>;

  public record Response(
    Guid Id,
    string Title);

  public class Handler : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository;

    private readonly IMapper _mapper;

    public Handler(IGenericRepository<Book> bookRepository, IMapper mapper)
    {
      _bookRepository = bookRepository;
      _mapper = mapper;
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
      var potentialDuplicate = _bookRepository.Table.SingleOrDefault(user => user.Isbn == request.Isbn);
      if (potentialDuplicate != null)
      {
        throw new BookAlreadyExistsException($"A book with Isbn: {request.Isbn} already exist in record");
      }

      var book = Book.CreateInstance(request.Title, request.Author, request.Isbn, request.PublishedDate);
      await _bookRepository.InsertAsync(book);
      await _bookRepository.SaveAsync(cancellationToken);

      return _mapper.Map<Response>(book);
    }
  }
}