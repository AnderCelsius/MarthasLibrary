using MarthasLibrary.APIClient;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Fixtures;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MarthasLibrary.IntegrationTests.Books;

[Collection("test collection")]
public sealed class CreateTests : IDisposable
{
  private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
  private const string UrlPath = "/books";
  private readonly TestFixture _fixture;
  private readonly LibraryDbContext _context;

  public CreateTests()
  {
    _fixture = new TestFixture();
    _fixture.MockServer.Reset();
    _fixture.MockServer.MockAuthentication(true);

    var serviceScope = _fixture.Server.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    _context = serviceScope.ServiceProvider.GetRequiredService<LibraryDbContext>();
  }

  public void Dispose()
  {
    _fixture.Dispose();
    _context.Dispose();
  }

  [Fact]
  public async Task CreateBook_SuccessfullyCreatesBook_WithValidRequest()
  {
    // Arrange
    _fixture.MockServer.Reset();
    _fixture.MockServer.MockAuthentication(true);
    const string title = "To Kill a Mockingbird";

    // Act
    var response = await _fixture.Client.PostAsJsonAsync<Books_Create_Request>(
      "api/Books",
      new()
      {
        Title = title,
        Author = "Harper Lee",
        Isbn = "9780446310789",
        PublishedDate = DateTimeOffset.UtcNow,
      },
      JsonSerializerOptions);

    var responseBody =
      await response.Content.ReadFromJsonFixedAsync<Books_Create_Response>();

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.Equal(title, responseBody?.Title);
    Assert.NotNull(responseBody?.Id);
  }
}