using MarthasLibrary.Application.UserData;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace MarthasLibrary.IntegrationTests.Utilities;

public class AuthenticatedUserClaimsManager
{
  public Claim[] SetupAuthenticatedUserClaims(string testUserIdentityId,
    Mock<IGenericRepository<Customer>> mockCustomerRepository, Mock<IUserDataProvider<UserData>> mockUserDataProvider,
    Mock<IHttpContextAccessor> mockHttpContextAccessor)
  {
    var userData = SetupCustomerRepository(testUserIdentityId, mockCustomerRepository);

    SetupUserDataProvider(userData, mockUserDataProvider);

    var claims = CustomerClaims(testUserIdentityId);

    SetupHttpContextAccessor(claims, mockHttpContextAccessor);
    return claims;
  }

  private void SetupHttpContextAccessor(Claim[] claims, Mock<IHttpContextAccessor> mockHttpContextAccessor)
  {
    var mockHttpContext = new Mock<HttpContext>();
    var identity = new ClaimsIdentity(claims, "TestAuthentication");
    var claimsPrincipal = new ClaimsPrincipal(identity);
    mockHttpContext.Setup(ctx => ctx.User).Returns(claimsPrincipal);
    mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);
  }

  public Claim[] CustomerClaims(string testUserIdentityId)
  {
    return new[]
    {
      new Claim(ClaimTypes.NameIdentifier, testUserIdentityId),
      new(ClaimTypes.Name, "Alice Smith"),
      new(ClaimTypes.GivenName, "Alice"),
      new(ClaimTypes.Surname, "Smith"),
      new(ClaimTypes.Role, "Customer"),
      new(ClaimTypes.Webpage, "http://alice.com")
    };
  }

  public UserData SetupCustomerRepository(string testUserIdentityId,
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

  private void SetupUserDataProvider(UserData mockUserData, Mock<IUserDataProvider<UserData>> mockUserDataProvider)
  {
    mockUserDataProvider.Setup(provider => provider.GetCurrentUserData(It.IsAny<CancellationToken>()))
      .ReturnsAsync(() => mockUserData);
  }
}