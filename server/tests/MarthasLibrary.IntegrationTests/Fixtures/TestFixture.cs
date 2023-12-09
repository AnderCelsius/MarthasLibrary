using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Utilities;
using MarthasLibrary.Tests.Common.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
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
                ["ConnectionString"] = "Data Source=.;Initial Catalog=MarthasLibrary.Tests;Integrated Security=True; TrustServerCertificate=True;"
                // Add other necessary configurations
            }!)
            .Build();

        _applicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            _ = builder.UseEnvironment("Testing")
                .ConfigureServices(services =>
                {
                    // by default the OIDC auth scheme is intercepted and an auto-failing handler is returned,
                    //  this ensures we don't accidentally call the discovery URL
                    // CreateLoggedInClient will replace this provider with a different interceptor, last one in wins
                    services.AddTransient<IAuthenticationSchemeProvider, AutoFailSchemeProvider>();
                    services.AddAuthentication(AutoFailSchemeProvider.AutoFailScheme)
                        .AddScheme<AutoFailOptions, AutoFail>(AutoFailSchemeProvider.AutoFailScheme, null);

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
        Client = CreateLoggedInClient<GeneralUser>(ConfigurationAndOptions.DefaultOptions);
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TokenGeneratorHelper.GetMockJwtToken());

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
        dbContext.Database.Migrate();
    }

    public HttpClient CreateClientWithRole(Claim[] claims)
    {
        var token = TokenGeneratorHelper.GetMockJwtToken(claims);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return Client;
    }

    /// <summary>
    /// This configures the "InterceptedScheme" to return a particular type of user, which we can also enrich with extra
    /// parameters/options for use in custom Claims
    /// </summary>
    /// <remarks>
    /// Adding a new user type:
    ///   2. Add a new helper method with appropriate args typed to the new class (example above)
    /// </remarks>
    public HttpClient CreateLoggedInClient<T>(WebApplicationFactoryClientOptions options)
        where T : GeneralUser
    {
        var client = _applicationFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                    {
                        var authServiceDescriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(IAuthenticationSchemeProvider)
                                 && d.ServiceType.BaseType == typeof(AutoFailSchemeProvider));

                        if (authServiceDescriptor is not null)
                            services.Remove(authServiceDescriptor);

                        // configure the intercepting provider
                        services
                            .AddTransient<IAuthenticationSchemeProvider, InterceptOidcAuthenticationSchemeProvider>();
                        services.AddTransient<IAuthenticationService, MockAuthenticationService>();

                        // Add a "Test" scheme in to process the auth instead, using the provided user type
                        services.AddAuthentication(InterceptOidcAuthenticationSchemeProvider.InterceptedScheme)
                            .AddScheme<ImpersonatedAuthenticationSchemeOptions, T>("InterceptedScheme",
                                options => { options.OriginalScheme = "oidc"; });
                    })
                    .UseConfiguration(ConfigurationAndOptions.GetConfiguration());
            })
            .CreateClient(options);

        return client;
    }

    public class GeneralUser : ImpersonatedUser
    {
        public GeneralUser(IOptionsMonitor<ImpersonatedAuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }
    }
}