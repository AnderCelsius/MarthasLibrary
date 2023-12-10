using IdentityModel;
using MarthasLibrary.APIClient;
using MarthasLibrary.Application.UserData;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Fixtures;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace MarthasLibrary.IntegrationTests.Reservations;

[Collection("test collection")]
public sealed class GetReservationsByCustomerIdTests : IDisposable
{
  private readonly TestFixture _fixture;
  private readonly LibraryDbContext _context;
  private readonly Mock<IGenericRepository<Customer>> _mockCustomerRepository;
  private const string TestUserIdentityId = "f0611528-36f2-4a0e-80ce-96dea6ebd13f";
  private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
  public Mock<IUserDataProvider<UserData>> MockUserDataProvider;
  public GetReservationsByCustomerIdTests()
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
    _context.Dispose();
    _fixture.Dispose();
  }

  [Fact]
  public async Task GetReservationsByCustomerId_ReturnsArray_WhenReservationExist()
  {
    // Arrange
    var book = Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789",
      new DateTime(1960, 7, 11));

    _context.Books.Add(book);

    var customer = Customer.CreateInstance("Alice", "Smith", "alice.smith@email.com", TestUserIdentityId);
    customer.SetAsActive();
    _context.Customers.Add(customer);

    var reservation = Reservation.CreateInstance(book.Id, customer.Id);
    _context.Reservations.Add(reservation);

    await _context.SaveChangesAsync();

    var claims = SetupAuthenticatedUserClaims();

    var client = _fixture.CreateClientWithRole(claims);

    // Act
    var response = await client.GetFromJsonFixedAsync<Reservations_GetReservationsByCustomerId_Response>(
      $"api/books/reserve/{customer.Id}");


    // Assert
    Assert.NotNull(response);
    Assert.Single(response.Reservations);
  }

  [Fact]
  public async Task GetReservationsByCustomerId_ReturnsEmptyArray_WhenNoReservationsHasBeenMade()
  {
    // Arrange
    var customer = Customer.CreateInstance("Alice", "Smith", "alice.smith@email.com", TestUserIdentityId);
    customer.SetAsActive();
    _context.Customers.Add(customer);

    await _context.SaveChangesAsync();

    var claims = SetupAuthenticatedUserClaims();

    var client = _fixture.CreateClientWithRole(claims);

    // Act
    var response = await client.GetFromJsonFixedAsync<Reservations_GetReservationsByCustomerId_Response>(
      $"api/books/reserve/{customer.Id}");


    // Assert
    Assert.NotNull(response);
    Assert.Empty(response.Reservations);
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