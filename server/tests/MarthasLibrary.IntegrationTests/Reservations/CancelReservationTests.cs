using IdentityModel;
using MarthasLibrary.Application.UserData;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Fixtures;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Security.Claims;

namespace MarthasLibrary.IntegrationTests.Reservations;

[Collection("test collection")]
public sealed class CancelReservationTests : IDisposable
{
  private readonly TestFixture _fixture;
  private readonly LibraryDbContext _context;
  private readonly Mock<IGenericRepository<Customer>> _mockCustomerRepository;
  private const string TestUserIdentityId = "f0611528-36f2-4a0e-80ce-96dea6ebd13f";
  private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
  public Mock<IUserDataProvider<UserData>> MockUserDataProvider;

  public CancelReservationTests()
  {
    _fixture = new TestFixture();
    _fixture.MockServer.Reset();
    _fixture.MockServer.MockAuthentication(true);

    var serviceScope = _fixture.Server.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    _context = serviceScope.ServiceProvider.GetRequiredService<LibraryDbContext>();


    _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
    MockUserDataProvider = new Mock<IUserDataProvider<UserData>>();
    _mockCustomerRepository = new Mock<IGenericRepository<Customer>>();
  }

  public void Dispose()
  {
    _fixture.Dispose();
    _context.Dispose();
  }

  [Fact]
  public async Task CancelReservation_CancelsSuccessfully_WhenRequestIsValid()
  {
    // Arrange
    var customer = Customer.CreateInstance("Alice", "Smith", "alice.smith@email.com", TestUserIdentityId);
    customer.SetAsActive();
    _context.Customers.Add(customer);

    var book = Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789",
      new DateTime(1960, 7, 11));
    book.MarkAsReserved();
    _context.Books.Add(book);

    var reservation = Reservation.CreateInstance(book.Id, customer.Id);
    _context.Reservations.Add(reservation);

    await _context.SaveChangesAsync();

    var claims = SetupAuthenticatedUserClaims();

    var client = _fixture.CreateClientWithRole(claims);

    // Act
    var response = await client.DeleteAsync(
      $"api/books/reserve/{reservation.Id}", new CancellationToken());


    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task CancelReservation_Returns400BadRequestWithMessage_WhenBookStatusIsNotReserved()
  {
    // Arrange
    var customer = Customer.CreateInstance("Alice", "Smith", "alice.smith@email.com", TestUserIdentityId);
    customer.SetAsActive();
    _context.Customers.Add(customer);

    var book = Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789",
      new DateTime(1960, 7, 11));
    _context.Books.Add(book);

    var reservation = Reservation.CreateInstance(book.Id, customer.Id);
    _context.Reservations.Add(reservation);

    await _context.SaveChangesAsync();

    var claims = SetupAuthenticatedUserClaims();

    var client = _fixture.CreateClientWithRole(claims);

    // Act
    var response = await client.DeleteAsync(
      $"api/books/reserve/{reservation.Id}", new CancellationToken());


    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    Assert.Contains("Only reserved books or borrowed can be marked as available.",
      await response.Content.ReadAsStringAsync());
  }

  [Fact]
  public async Task CancelReservation_Returns404NotFoundWithMessage_WhenReservationIsNotFound()
  {
    // Arrange
    var claims = SetupAuthenticatedUserClaims();

    var client = _fixture.CreateClientWithRole(claims);

    var reservationId = Guid.NewGuid();

    // Act
    var response = await client.DeleteAsync(
      $"api/books/reserve/{reservationId}", new CancellationToken());


    // Assert
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    Assert.Contains($"Could not find a reservation with ID {reservationId}",
      await response.Content.ReadAsStringAsync());
  }

  #region Test Setup

  private Claim[] SetupAuthenticatedUserClaims()
  {
    var userData = SetupCustomerRepository();

    SetupUserDataProvider(userData);

    var claims = CustomerClaims();

    SetupHttpContextAccessor(claims);
    return claims;
  }

  private void SetupHttpContextAccessor(Claim[] claims)
  {
    var mockHttpContext = new Mock<HttpContext>();
    var identity = new ClaimsIdentity(claims, "TestAuthentication");
    var claimsPrincipal = new ClaimsPrincipal(identity);
    mockHttpContext.Setup(ctx => ctx.User).Returns(claimsPrincipal);
    _mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);
  }

  private Claim[] CustomerClaims()
  {
    return new[]
    {
      new Claim(ClaimTypes.NameIdentifier, TestUserIdentityId),
      new(JwtClaimTypes.Name, "Alice Smith"),
      new(JwtClaimTypes.GivenName, "Alice"),
      new(JwtClaimTypes.FamilyName, "Smith"),
      new(JwtClaimTypes.Role, "Customer"),
      new(JwtClaimTypes.WebSite, "http://alice.com")
    };
  }

  private UserData SetupCustomerRepository()
  {
    UserData mockUserData = WireMockAuthenticationExtensions.MockCustomerUser;
    var customers = new List<Customer>
    {
      Customer.CreateInstance(mockUserData.FirstName,
        mockUserData.LastName,
        mockUserData.Email, TestUserIdentityId),
    };

    var customersQueryable = customers.AsQueryable();

    _mockCustomerRepository.Setup(repo => repo.Table)
      .Returns(customersQueryable);

    return mockUserData;
  }

  private void SetupUserDataProvider(UserData mockUserData)
  {
    MockUserDataProvider.Setup(provider => provider.GetCurrentUserData(It.IsAny<CancellationToken>()))
      .ReturnsAsync(() => mockUserData);
  }

  #endregion
}