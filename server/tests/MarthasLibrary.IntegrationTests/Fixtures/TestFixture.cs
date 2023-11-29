using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using WireMock.Server;

namespace MarthasLibrary.IntegrationTests.Fixtures;

public class TestFixture : IDisposable
{
  private readonly WebApplicationFactory<Program> _applicationFactory;

  public TestFixture()
  {
    MockServer = WireMockServer.Start();
    MockServer.MockAuthentication(true);

    var config = new ConfigurationBuilder().Build();

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
        })
        .UseConfiguration(config);
    });

    Server = _applicationFactory.Server;
    Client = _applicationFactory.CreateClient();
    Client.DefaultRequestHeaders.Authorization =
      new AuthenticationHeaderValue("Bearer", WireMockAuthenticationExtensions.BearerToken);

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
}