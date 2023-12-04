using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using WireMock.Server;

namespace MarthasLibrary.IntegrationTests.Fixtures;

public class TestFixture : IDisposable
{
    private readonly WebApplicationFactory<Program> _applicationFactory;

    public TestFixture()
    {
        MockServer = WireMockServer.Start();
        MockServer.MockAuthentication(true);

        var config = new ConfigurationBuilder()
          .AddInMemoryCollection(new Dictionary<string, string>
          {
              ["DuendeISP:Authority"] = MockServer.Urls[0],
              ["DuendeISP:Audience"] = "marthaslibraryapi",
              // Add other necessary configurations
          }!)
          .Build();

        _applicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing")
          .ConfigureServices(services =>
          {
              // Remove the existing DbContext configuration
                var dbContextDescriptor = services.SingleOrDefault(
              d => d.ServiceType ==
                   typeof(DbContextOptions<LibraryDbContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContextFactory<LibraryDbContext>(options => { options.UseInMemoryDatabase("TestDatabase"); });

                services.AddAuthentication("TestScheme")
              .AddJwtBearer("TestScheme", opt =>
              {
                  // Configure to use the mock server and test credentials
                    opt.RequireHttpsMetadata = false;
                    opt.Authority = MockServer.Urls[0]; // Mock IdentityServer URL
                    opt.Audience = "marthaslibraryapi";
                  // ... other options as necessary
                });
              // Mock the HttpClient to include the bearer token from WireMock
                services.AddHttpClient("TestClient",
              client =>
              {
                    client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", WireMockAuthenticationExtensions.BearerToken);
                });
            })
          .UseConfiguration(config);
        });

        Server = _applicationFactory.Server;
        Client = _applicationFactory.CreateClient();
        Client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", GetMockJwtToken());

        InitializeDatabase();
    }

    public TestServer Server { get; }

    public HttpClient Client { get; }

    public WireMockServer MockServer { get; }

    public void Dispose()
    {
        MockServer.Dispose();
        var dbContextFactory = Server.Services
          .GetRequiredService<IDbContextFactory<LibraryDbContext>>();
        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.EnsureDeleted();

        _applicationFactory.Dispose();
    }

    private void InitializeDatabase()
    {
        var dbContextFactory = Server.Services.GetRequiredService<IDbContextFactory<LibraryDbContext>>();
        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.EnsureDeleted();
    }


    public LibraryDbContext GetDbContext()
    {
        var dbContextFactory = Server.Services
          .GetRequiredService<IDbContextFactory<LibraryDbContext>>();
        return dbContextFactory.CreateDbContext();
    }

    private string GetMockJwtToken()
    {
        // Symmetric security key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your-Secret-Key-Here"));

        // Signing credentials
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims for the token
        var claims = new[]
        {
      new Claim(JwtRegisteredClaimNames.Sub, "test-user"),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new Claim("role", "User"),
      // Add other claims as needed for your testing purposes
    };

        // Token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
            SigningCredentials = credentials,
            Issuer = "https://mocked-issuer.com",
            Audience = "marthaslibraryapi",
        };

        // Token handler
        var tokenHandler = new JwtSecurityTokenHandler();

        // Create token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return serialized token
        return tokenHandler.WriteToken(token);
    }

}