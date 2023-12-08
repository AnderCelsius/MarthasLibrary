using Duende.IdentityServer;
using MarthasLibrary.IdentityServer.Data;
using MarthasLibrary.IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

namespace MarthasLibrary.IdentityServer
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Identity")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var migrationsAssembly = typeof(Program).GetTypeInfo()
                .Assembly.GetName().Name;

            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.EmitStaticAudienceClaim = true;
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = optionsBuilder =>
                        optionsBuilder.UseSqlServer(
                            builder.Configuration
                                .GetConnectionString("IdentityServer"),
                            sqlOptions => sqlOptions
                                .MigrationsAssembly(migrationsAssembly));
                })
                .AddConfigurationStoreCache()
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = optionsBuilder =>
                        optionsBuilder.UseSqlServer(builder.Configuration
                                .GetConnectionString("IdentityServer"),
                            sqlOptions => sqlOptions
                                .MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<ApplicationUser>();

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "408144084342-edugfoip6oslup3lh7hln5fohr1jno7f.apps.googleusercontent.com";
                    options.ClientSecret = "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";
                });

            return builder.Build();
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