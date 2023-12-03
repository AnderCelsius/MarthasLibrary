using AutoMapper;
using FluentValidation;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using System.Text.RegularExpressions;

namespace MarthasLibrary.API.Features.Books;

/// <summary>
/// Provides functionality for creating a new book entry in the library.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request, response, and logic for creating a new book.
/// </remarks>
public static class Create
{
  /// <summary>
  /// Represents a request to create a new book.
  /// </summary>
  /// <param name="Title">The title of the book.</param>
  /// <param name="Author">The author of the book.</param>
  /// <param name="Isbn">The ISBN of the book.</param>
  /// <param name="PublishedDate">The publication date of the book.</param>
  public record Request(string Title, string Author, string Isbn, DateTimeOffset PublishedDate) : IRequest<Response>;

  /// <summary>
  /// Represents the response after creating a new book.
  /// </summary>
  /// <param name="Id">The unique identifier of the created book.</param>
  /// <param name="Title">The title of the created book.</param>
  public record Response(
    Guid Id,
    string Title);

  /// <summary>
  /// Handles the process of creating a new book.
  /// </summary>
  /// <remarks>
  /// This class is responsible for validating the book information, checking for duplicates, and adding the new book to the repository.
  /// </remarks>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <param name="mapper">An instance of AutoMapper for object mapping.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for the book repository or the mapper.</exception>
  /// <exception cref="BookWithIsbnAlreadyExistsException">Thrown when a book with the same ISBN already exists.</exception>
  public class Handler(IGenericRepository<Book> bookRepository, IMapper mapper) : IRequestHandler<Request, Response>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

    /// <summary>
    /// Handles the incoming request to create a new book.
    /// </summary>
    /// <param name="request">The request to create a new book.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the response containing the details of the created book.</returns>
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

  /// <summary>
  /// Validator for the book creation request.
  /// </summary>
  /// <remarks>
  /// This class provides validation rules for book creation requests, including title, author, ISBN, and publication date.
  /// </remarks>
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