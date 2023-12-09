using FluentAssertions;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.UnitTests.EntitiesTests;

public sealed class ReservationTests
{
  [Fact]
  public void Create_Should_ReturnReservation_WhenInputIsValid()
  {
    // Arrange
    var reservation = Reservation.CreateInstance(Guid.NewGuid(), Guid.NewGuid());

    // Assert
    reservation.Should().NotBeNull();
  }

  [Theory]
  [InlineData("f0611528-36f2-4a0e-80ce-96dea6ebd13f", null, "customerId")]
  [InlineData(null, "f0611528-36f2-4a0e-80ce-96dea6ebd13f", "bookId")]
  public void ShouldThrow_ArgumentNullException_WhenInputIsInvalid(string bookIdString, string customerIdString, string paramName)
  {
    // Convert strings to Guids, handling nulls
    Guid bookId = string.IsNullOrEmpty(bookIdString) ? Guid.Empty : Guid.Parse(bookIdString);
    Guid customerId = string.IsNullOrEmpty(customerIdString) ? Guid.Empty : Guid.Parse(customerIdString);

    // Arrange
    Reservation Action() => Reservation.CreateInstance(bookId, customerId);

    // Assert
    FluentActions.Invoking(Action).Should().Throw<ArgumentException>()
      .WithMessage($"Value cannot be null. (Parameter '{paramName}')");
  }
}