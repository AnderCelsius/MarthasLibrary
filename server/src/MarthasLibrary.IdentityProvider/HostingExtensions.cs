using Duende.IdentityServer;
using MarthasLibrary.IdentityProvider.Data;
using MarthasLibrary.IdentityProvider.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog;
using System.Reflection;

namespace MarthasLibrary.IdentityProvider
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            var identityServerConString = builder.Configuration
              .GetConnectionString("IdentityServer");
            var identityConString = builder.Configuration
              .GetConnectionString("Identity");

            var migrationsAssembly = typeof(Program).GetTypeInfo()
              .Assembly.GetName().Name;

            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(identityConString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            builder.Services
              .AddIdentityServer(options =>
              {
                  options.Events.RaiseErrorEvents = true;
                  options.Events.RaiseInformationEvents = true;
                  options.Events.RaiseFailureEvents = true;
                  options.Events.RaiseSuccessEvents = true;

                  // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                  options.EmitStaticAudienceClaim = true;
              })
              .AddConfigurationStore(configurationStoreOptions =>
                configurationStoreOptions.ResolveDbContextOptions =
                  ResolveDbContextOptions(identityServerConString, migrationsAssembly))
              .AddConfigurationStoreCache()
              .AddOperationalStore(configurationStoreOptions =>
                configurationStoreOptions.ResolveDbContextOptions =
                  ResolveDbContextOptions(identityServerConString, migrationsAssembly))
              .AddAspNetIdentity<ApplicationUser>();

            builder.Services.AddAuthentication()
              .AddGoogle(options =>
              {
                  options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                  // register your IdentityServer with Google at https://console.developers.google.com
                  // enable the Google+ API
                  // set the redirect URI to https://localhost:5001/signin-google
                  options.ClientId = "copy client ID from Google here";
                  options.ClientSecret = "copy client secret from Google here";
              });

            return builder.Build();
        }

        private static Action<IServiceProvider, DbContextOptionsBuilder>
          ResolveDbContextOptions(string connectionString, string migrationAssembly)
        {
            return (_, dbContextOptionsBuilder) =>
            {
                dbContextOptionsBuilder.UseSqlServer(connectionString!,
            SqlServerOptionsAction(migrationAssembly));
            };
        }

        private static Action<SqlServerDbContextOptionsBuilder> SqlServerOptionsAction(string migrationAssembly)
        {
            return sqlServerDbContextOptionsBuilder =>
            {
                sqlServerDbContextOptionsBuilder.MigrationsAssembly(
            migrationAssembly);
            };
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapRazorPages()
              .RequireAuthorization();

            return app;
        }
    }
}