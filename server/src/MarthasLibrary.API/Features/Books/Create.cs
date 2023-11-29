using AutoMapper;
using FluentValidation;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using System.Text.RegularExpressions;

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
        throw new BookWithIsbnAlreadyExistsException($"A different book with this Isbn: '{request.Isbn}' already exist in record");
      }

      var book = Book.CreateInstance(request.Title, request.Author, request.Isbn, request.PublishedDate);
      await _bookRepository.InsertAsync(book);
      await _bookRepository.SaveAsync(cancellationToken);

      return _mapper.Map<Response>(book);
    }
  }

  public class CreateBookValidator : AbstractValidator<Request>
  {
    public CreateBookValidator()
    {
      // Title validation: not empty, no digits, and a reasonable length limit
      RuleFor(request => request.Title)
        .NotEmpty().WithMessage("Title is required.")
        .Length(1, 255).WithMessage("Title must be between 1 and 255 characters long.");

      // Author validation: not empty and no digits
      RuleFor(request => request.Author)
        .NotEmpty().WithMessage("Author is required.")
        .Must(author => !Regex.IsMatch(author, @"\d")).WithMessage("Author must not contain digits.")
        .Length(2, 255).WithMessage("Author must be between 1 and 255 characters long.");

      // ISBN validation: exactly 13 digits (ISBN-13 format)
      RuleFor(request => request.Isbn)
        .NotEmpty().WithMessage("ISBN is required.")
        .Length(13).WithMessage("ISBN must be exactly 13 digits long.")
        .Matches(new Regex("^[0-9]+$")).WithMessage("ISBN must only contain digits.");

      // PublishedDate validation: not in the future
      RuleFor(request => request.PublishedDate)
        .NotEmpty().WithMessage("PublishedDate is required.")
        .LessThanOrEqualTo(DateTimeOffset.Now).WithMessage("Published date cannot be in the future.");
    }
  }
}