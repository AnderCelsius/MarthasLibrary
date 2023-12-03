using MarthasLibrary.IdentityServer.Entities;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace MarthasLibrary.IntegrationTests.Utilities;

public static class WireMockAuthenticationExtensions
{
    public const string MockAdminEmail = "admin@marthaslib.com";

    public const string MockFirstName = "John";

    public const string MockLastName = "Doe";

    public const string MockRegularEmail = "regular.user@example.com";

    public const string BearerToken = "some-random-string";

    private static readonly ApplicationUser MockAdminUser = new()
    {
        FirstName = MockFirstName,
        LastName = MockLastName,
        Email = MockAdminEmail,
        PhoneNumber = "+2348023415243"
    };

    private static readonly ApplicationUser MockRegularUser = new()
    {
        FirstName = "Regular",
        LastName = "Person",
        Email = MockRegularEmail,
        PhoneNumber = "+2348023546789"
    };

    public static void MockAuthentication(this WireMockServer wireMockServer, bool allowAdmin = false, bool isUppercaseEmail = false)
    {
        var userData = allowAdmin ? MockAdminUser : MockRegularUser;
        wireMockServer
            .Given(Request.Create().WithPath("/your-endpoint").UsingPost()
                .WithHeader("Authorization", $"Bearer {BearerToken}"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBodyAsJson(JsonSerializer.Serialize(userData)));
    }
}