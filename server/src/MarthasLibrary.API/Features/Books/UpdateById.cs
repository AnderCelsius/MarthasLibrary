using FluentValidation;
using MarthasLibrary.API.Features.Exceptions;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MarthasLibrary.API.Features.Books;

/// <summary>
/// Provides functionality for updating an existing book's details by its identifier.
/// </summary>
/// <remarks>
/// This static class contains nested types to handle the request and logic for updating a book based on its unique identifier.
/// </remarks>
public static class UpdateById
{
  /// <summary>
  /// Represents a request to update a book.
  /// </summary>
  /// <param name="BookId">The unique identifier of the book to be updated.</param>
  /// <param name="Details">The updated details of the book.</param>
  public record Request(Guid BookId, Request.UpdatedDetails Details) : IRequest
  {
    /// <summary>
    /// Represents the updated details of the book.
    /// </summary>
    /// <param name="Title">The updated title of the book.</param>
    /// <param name="Author">The updated author of the book.</param>
    /// <param name="Isbn">The updated ISBN of the book.</param>
    /// <param name="PublishedDate">The updated publication date of the book.</param>
    public record UpdatedDetails(string Title, string Author, string Isbn, DateTimeOffset PublishedDate)
    {
      public static explicit operator Book.BookUpdate(UpdatedDetails d) =>
        new(Title: d.Title, Author: d.Author, Isbn: d.Isbn, PublishedDate: d.PublishedDate);
    }
  }

  /// <summary>
  /// Handles the process of updating a book's details.
  /// </summary>
  /// <remarks>
  /// This class is responsible for locating the book by its identifier, ensuring no ISBN conflicts, and applying the updated details.
  /// </remarks>
  /// <param name="bookRepository">Repository for accessing book entities.</param>
  /// <exception cref="ArgumentException">Thrown when a null argument is passed for the book repository.</exception>
  /// <exception cref="BookNotFoundException">Thrown when the specified book is not found in the repository.</exception>
  /// <exception cref="BookWithIsbnAlreadyExistsException">Thrown when another book with the same ISBN already exists.</exception>
  public class Handler(IGenericRepository<Book> bookRepository) : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));

    /// <summary>
    /// Handles the incoming request to update a book's details.
    /// </summary>
    /// <param name="request">The request containing the book ID and updated details.</param>
    /// <param name="cancellationToken">A token for cancelling the operation if necessary.</param>
    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
      var book = await _bookRepository.Table
                   .SingleOrDefaultAsync(book => book.Id == request.BookId, cancellationToken) ??
                 throw new BookNotFoundException($"Could not find a user with ID {request.BookId}.");

      var potentialDuplicate = await _bookRepository.Table
        .SingleOrDefaultAsync(
          b => request.BookId != b.Id && request.Details.Isbn == b.Isbn,
          cancellationToken);

      if (potentialDuplicate is not null)
      {
        throw new BookWithIsbnAlreadyExistsException("A different book with this Isbn already exists.");
      }

      book.UpdateDetails((Book.BookUpdate)request.Details);
      await _bookRepository.SaveAsync(cancellationToken);
    }
  }

  public class UpdatedDetailsValidator : AbstractValidator<Request.UpdatedDetails>
  {
    public UpdatedDetailsValidator()
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

  public class RequestValidator : AbstractValidator<Request>
  {
    public RequestValidator()
    {
      RuleFor(request => request.BookId)
        .NotEmpty().WithMessage("Book ID is required.");

      // Include the UpdatedDetailsValidator for the nested Details property
      RuleFor(request => request.Details).SetValidator(new UpdatedDetailsValidator());
    }
  }
}