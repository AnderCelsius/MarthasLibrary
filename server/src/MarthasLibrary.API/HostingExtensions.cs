using MarthasLibrary.API.Configuration;
using MarthasLibrary.API.Extensions;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

namespace MarthasLibrary.API;

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
    builder.Services.AddSwaggerGen();

    builder.Services.AddHealthChecks()
      .AddDbContextCheck<LibraryDbContext>();

    builder.Services.AddMediatR(c =>
      c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

    builder.Services.AddHttpClient();

    builder.Services.AddIdentity<Customer, IdentityRole>()
      .AddEntityFrameworkStores<LibraryDbContext>();

    builder.Services.AddDatabase<LibraryDbContext>(builder.Configuration
      .GetRequiredSection("Database").Get<DatabaseConfiguration>());
  }

  /// <summary>
  /// Configures the HTTP request pipeline.
  /// </summary>
  /// <param name="app">Web Application</param>
  public static void ConfigurePipeline(this WebApplication app)
  {
    if (!app.Environment.IsProduction())
    {
      Log.Information("Seeding database...");
      SeedData.EnsureSeedUserData(app);
      Log.Information("Done seeding database. Exiting.");
      app.UseSwagger().UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
  }
}