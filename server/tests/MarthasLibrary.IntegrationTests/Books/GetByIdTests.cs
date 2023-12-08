using MarthasLibrary.APIClient;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Fixtures;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace MarthasLibrary.IntegrationTests.Books;

[Collection("test collection")]
public sealed class GetByIdTests : IDisposable
{
  private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
  private readonly TestFixture _fixture;
  private readonly LibraryDbContext _context;

  public GetByIdTests()
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
  public async Task GetById_ReturnsABook_WhenBookExists()
  {
    const string isbn = "9780451524935";

    var books = new List<Book>
    {
      Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8)),
    };


    await Seeder.SeedBook(books, _context);

    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_GetById_Response>(
      $"/api/Books/{books[0].Id}");

    Assert.NotNull(response);
    Assert.Equal(isbn, response.Book.Isbn);
  }

  [Fact]
  public async Task GetById_Returns404_WhenBookDoesNotExist()
  {

    var response = await _fixture.Client.GetAsync(
      $"/api/Books/{Guid.NewGuid()}");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}