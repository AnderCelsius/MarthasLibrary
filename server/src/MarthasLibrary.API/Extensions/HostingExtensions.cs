﻿using MarthasLibrary.API.Configuration;
using MarthasLibrary.API.ErrorHandling;
using MarthasLibrary.API.Filters;
using MarthasLibrary.API.Middleware;
using MarthasLibrary.APIClient;
using MarthasLibrary.Application.InfrastructureImplementations;
using MarthasLibrary.Core.Repository;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.Infrastructure.EventHandlers;
using Serilog;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;

namespace MarthasLibrary.API.Extensions;

public static class HostingExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opts.JsonSerializerOptions.DefaultIgnoreCondition =
          JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGenWithExtraSetup(builder.Configuration, nameof(Features));

        builder.Services.AddHealthChecks()
          .AddDbContextCheck<LibraryDbContext>();

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.RegisterServicesFromAssembly(typeof(BookReservedEventHandler).Assembly);

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        builder.Services.AddHttpClient();

        builder.Services.AddDatabase<LibraryDbContext>(builder.Configuration
          .GetRequiredSection("Database").Get<DatabaseConfiguration>());

        builder.Services.AddScoped<IMarthasLibraryAPIClient, MarthasLibraryAPIClient>();

        builder.AddExternalServiceAuthentication();

        builder.Services.AddScoped<CustomerAuthorizationFilter>();

    }

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Web Application</param>
    public static void ConfigurePipeline(this WebApplication app, WebApplicationBuilder builder)
    {
        if (!app.Environment.IsProduction() && !app.Environment.IsTesting())
        {
            Log.Information("Seeding database...");
            SeedData.EnsureSeedUserData(app);
            Log.Information("Done seeding database. Exiting.");

            app.UseDeveloperExceptionPage();
            app.UseSwagger().UseSwaggerUI();
        }

        app.UseHealthChecks("/health");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseErrorHandling(
          !builder.Environment.IsProduction(),
          new Dictionary<Type, HttpStatusCode>());

        app.MapControllers();
    }
}