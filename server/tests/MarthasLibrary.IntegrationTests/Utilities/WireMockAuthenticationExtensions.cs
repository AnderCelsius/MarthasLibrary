using IdentityModel.Client;
using MarthasLibrary.IdentityServer.Entities;
using System.Text.Json;
using MarthasLibrary.Application.UserData;
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

  public const string IdentityServerAuthority = "https://your-identityserver-url.com";
  public const string ClientId = "your-client-id";
  public const string ClientSecret = "your-client-secret";
  public const string Scope = "your-api-scope";

  private static string AccessToken { get; set; }

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

  public static async Task InitializeAuthentication()
  {
    var client = new HttpClient();

    // Discover the IdentityServer configuration
    var discoveryDocument = await client.GetDiscoveryDocumentAsync(IdentityServerAuthority);

    if (discoveryDocument.IsError)
    {
      throw new Exception($"Error discovering IdentityServer: {discoveryDocument.Error}");
    }

    // Request a token
    var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
      Address = discoveryDocument.TokenEndpoint,
      ClientId = ClientId,
      ClientSecret = ClientSecret,
      Scope = Scope
    });

    if (tokenResponse.IsError)
    {
      throw new Exception($"Error requesting token: {tokenResponse.Error}");
    }

    AccessToken = tokenResponse.AccessToken;
  }

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

  public static void MockAuthentication(this WireMockServer wireMockServer)
  {
    var mockResponse = new
    {
      access_token = "mocked_access_token", // This should be a valid token
      expires_in = 3600,
      token_type = "Bearer"
    };

    wireMockServer
      .Given(Request.Create().WithPath("/connect/token").UsingPost())
      .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithBodyAsJson(JsonSerializer.Serialize(mockResponse)));
  }
}