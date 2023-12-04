using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace MarthasLibrary.Tests.Common.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                       d => d.ServiceType ==
                            typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            // by default the OIDC auth scheme is intercepted and an auto-failing handler is returned,
            //  this ensures we don't accidentally call the discovery URL
            // CreateLoggedInClient will replace this provider with a different interceptor, last one in wins
            services.AddTransient<IAuthenticationSchemeProvider, AutoFailSchemeProvider>();
            services.AddAuthentication(AutoFailSchemeProvider.AutoFailScheme)
                .AddScheme<AutoFailOptions, AutoFail>(AutoFailSchemeProvider.AutoFailScheme, null);
        }).UseConfiguration(ConfigurationAndOptions.GetConfiguration());
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
        var client = WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var authServiceDescriptor = services.SingleOrDefault(
                   d => d.ServiceType == typeof(IAuthenticationSchemeProvider)
                   && d.ServiceType.BaseType == typeof(AutoFailSchemeProvider));

                if (authServiceDescriptor is not null)
                    services.Remove(authServiceDescriptor);

                // configure the intercepting provider
                services.AddTransient<IAuthenticationSchemeProvider, InterceptOidcAuthenticationSchemeProvider>();
                services.AddTransient<IAuthenticationService, MockAuthenticationService>();

                // Add a "Test" scheme in to process the auth instead, using the provided user type
                services.AddAuthentication(InterceptOidcAuthenticationSchemeProvider.InterceptedScheme)
                    .AddScheme<ImpersonatedAuthenticationSchemeOptions, T>("InterceptedScheme", options =>
                    {
                        options.OriginalScheme = "oidc";
                    });
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
        { }
    }
}

