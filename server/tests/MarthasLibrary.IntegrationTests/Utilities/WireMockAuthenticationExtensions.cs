using IdentityModel;
using MarthasLibrary.Application.UserData;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace MarthasLibrary.IntegrationTests.Utilities;

public static class WireMockAuthenticationExtensions
{
  public const string MockAdminEmail = "admin@marthaslib.com";

  public const string MockAdminFirstName = "John";

  public const string MockAdminLastName = "Doe";

  public const string MockCustomerFirstName = "Alice";

  public const string MockCustomerLastName = "Smith";

  public const string MockCustomerEmail = "alice.smith@email.com";

  public const string BearerToken = "some-random-string";

  private static readonly UserData MockAdminUser = new(
    Id: Guid.NewGuid(),
    Type: "",
    FirstName: MockAdminFirstName,
    LastName: MockAdminLastName,
    Email: MockCustomerEmail
  );

  public static readonly UserData MockCustomerUser = new(
    Id: Guid.NewGuid(),
    Type: "",
    FirstName: MockCustomerFirstName,
    LastName: MockCustomerLastName,
    Email: MockCustomerEmail
  );


  public static void MockAuthentication(this WireMockServer wireMockServer, bool allowAdmin = false)
  {
    var userData = allowAdmin ? MockAdminUser : MockCustomerUser;
    wireMockServer
      .Given(Request.Create().WithPath("https://localhost:5001").UsingPost()
        .WithHeader("Authorization", $"Bearer {BearerToken}"))
      .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithBodyAsJson(JsonSerializer.Serialize(userData)));
  }

  public static Claim[] SetupAuthenticatedUserClaims(string testUserIdentityId,
    Mock<IGenericRepository<Customer>> mockCustomerRepository, Mock<IUserDataProvider<UserData>> mockUserDataProvider,
    Mock<IHttpContextAccessor> mockHttpContextAccessor)
  {
    var userData = SetupCustomerRepository(testUserIdentityId, mockCustomerRepository);

    SetupUserDataProvider(userData, mockUserDataProvider);

    var claims = CustomerClaims(testUserIdentityId);

    SetupHttpContextAccessor(claims, mockHttpContextAccessor);
    return claims;
  }

  private static void SetupHttpContextAccessor(Claim[] claims, Mock<IHttpContextAccessor> mockHttpContextAccessor)
  {
    var mockHttpContext = new Mock<HttpContext>();
    var identity = new ClaimsIdentity(claims, "TestAuthentication");
    var claimsPrincipal = new ClaimsPrincipal(identity);
    mockHttpContext.Setup(ctx => ctx.User).Returns(claimsPrincipal);
    mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);
  }

  public static Claim[] CustomerClaims(string testUserIdentityId)
  {
    return new[]
    {
      new Claim(ClaimTypes.NameIdentifier, testUserIdentityId),
      new(JwtClaimTypes.Name, "Alice Smith"),
      new(JwtClaimTypes.GivenName, "Alice"),
      new(JwtClaimTypes.FamilyName, "Smith"),
      new(JwtClaimTypes.Role, "Customer"),
      new(JwtClaimTypes.WebSite, "http://alice.com")
    };
  }

  public static UserData SetupCustomerRepository(string testUserIdentityId,
    Mock<IGenericRepository<Customer>> mockCustomerRepository)
  {
    UserData mockUserData = WireMockAuthenticationExtensions.MockCustomerUser;
    var customers = new List<Customer>
    {
      Customer.CreateInstance(mockUserData.FirstName,
        mockUserData.LastName,
        mockUserData.Email, testUserIdentityId),
    };

    var customersQueryable = customers.AsQueryable();

    mockCustomerRepository.Setup(repo => repo.Table)
      .Returns(customersQueryable);

    return mockUserData;
  }

  private static void SetupUserDataProvider(UserData mockUserData, Mock<IUserDataProvider<UserData>> mockUserDataProvider)
  {
    mockUserDataProvider.Setup(provider => provider.GetCurrentUserData(It.IsAny<CancellationToken>()))
      .ReturnsAsync(() => mockUserData);
  }
}