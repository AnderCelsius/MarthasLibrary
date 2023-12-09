using FluentAssertions;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Enums;

namespace MarthasLibrary.UnitTests.EntitiesTests;

public sealed class BookTests
{
  [Fact]
  public void Create_Should_ReturnBook_WhenInputIsValid()
  {
    // Arrange
    var book = Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789",
      new DateTime(1960, 7, 11));

    // Assert
    book.Should().NotBeNull();
    book.Status.Should().Be(BookStatus.Available);
  }

  [Fact]
  public void MarkAsReserved_ShouldThrow_ArgumentNullException_WhenBookStatusIsNotAvailable()
  {
    // Arrange
    var book = Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789",
      new DateTime(1960, 7, 11));
    book.MarkAsReserved();
    book.MarkAsBorrowed();

    // Act & Assert
    FluentActions.Invoking(book.MarkAsReserved).Should().Throw<InvalidOperationException>()
      .WithMessage("Only available books can be marked as reserved.");
  }

  [Fact]
  public void MarkAsBorrowed_ShouldThrow_ArgumentNullException_WhenBookStatusIsNotReserved()
  {
    // Arrange
    var book = Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789",
      new DateTime(1960, 7, 11));

    // Act & Assert
    FluentActions.Invoking(book.MarkAsBorrowed).Should().Throw<InvalidOperationException>()
      .WithMessage("Only reserved books can be marked as borrowed.");
  }

  [Fact]
  public void MarkAsAvailable_ShouldThrow_ArgumentNullException_WhenBookStatusIsAvailable()
  {
    // Arrange
    var book = Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789",
      new DateTime(1960, 7, 11));

    // Act & Assert
    FluentActions.Invoking(book.MarkAsAvailable).Should().Throw<InvalidOperationException>()
      .WithMessage("Only reserved books or borrowed can be marked as available.");
  }
}