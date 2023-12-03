using FluentValidation;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MarthasLibrary.API.Features.Books;

public static class UpdateById
{
  public record Request(Guid BookId, Request.UpdatedDetails Details) : IRequest
  {
    public record UpdatedDetails(string Title, string Author, string Isbn, DateTimeOffset PublishedDate)
    {
      public static explicit operator Book.BookUpdate(UpdatedDetails d) =>
        new(Title: d.Title, Author: d.Author, Isbn: d.Isbn, PublishedDate: d.PublishedDate);
    }
  }

  public class Handler(IGenericRepository<Book> bookRepository) : IRequestHandler<Request>
  {
    private readonly IGenericRepository<Book> _bookRepository = bookRepository ?? throw new ArgumentException(nameof(bookRepository));

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