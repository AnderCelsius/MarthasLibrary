using MarthasLibrary.APIClient;
using MarthasLibrary.Application.UserData;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Fixtures;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace MarthasLibrary.IntegrationTests.Reservations;

[Collection("test collection")]
public sealed class MakeReservationTests : IDisposable
{
  private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
  private readonly TestFixture _fixture;
  private readonly LibraryDbContext _context;
  private const string RegularUserEmail = WireMockAuthenticationExtensions.MockRegularEmail;
  private const string TestUserIdentityId = "f0611528-36f2-4a0e-80ce-96dea6ebd13f";
  private Mock<IHttpContextAccessor> MockHttpContextAccessor { get; set; }
  public Mock<IUserDataProvider<UserData>> MockUserDataProvider { get; private set; }

  public MakeReservationTests()
  {
    _fixture = new TestFixture();
    _fixture.MockServer.Reset();
    _fixture.MockServer.MockAuthentication(true);

    var serviceScope = _fixture.Server.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    _context = serviceScope.ServiceProvider.GetRequiredService<LibraryDbContext>();

    MockHttpContextAccessor = new Mock<IHttpContextAccessor>();
    MockUserDataProvider = new Mock<IUserDataProvider<UserData>>();
    SetupHttpContextAccessor();
  }

  public void Dispose()
  {
    _fixture.Dispose();
    _context.Dispose();
  }

  [Fact]
  public async Task MakeReservation_ReservationSuccessful_ReturnsReservationDetails()
  {
    // Arrange
    var books = new List<Book>
        {
            Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789", new DateTime(1960, 7, 11)),
            Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8)),
            Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10))
        };

    await Seeder.SeedBook(books, _context);

    var customer = Customer.CreateInstance("Alice", "Smith", "alice.smith@email.com", TestUserIdentityId);
    customer.SetAsActive();

    _context.Customers.Add(customer);
    await _context.SaveChangesAsync();

    var firstBookId = books[0].Id;

    // Act
    var response = await _fixture.Client.PostAsJsonAsync<Reservations_MakeReservation_Request>(
        "api/books/reserve", new()
        {
          BookId = firstBookId
        }, JsonSerializerOptions);

    var responseBody =
        await response.Content.ReadFromJsonFixedAsync<Reservations_MakeReservation_Request>();

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    // ...
  }

  private void SetupHttpContextAccessor()
  {
    var mockHttpContext = new Mock<HttpContext>();
    var identity = new ClaimsIdentity(new Claim[]
    {
            new Claim(ClaimTypes.NameIdentifier, TestUserIdentityId)
    }, "TestAuthentication");
    var claimsPrincipal = new ClaimsPrincipal(identity);
    mockHttpContext.Setup(ctx => ctx.User).Returns(claimsPrincipal);
    MockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);
  }
}