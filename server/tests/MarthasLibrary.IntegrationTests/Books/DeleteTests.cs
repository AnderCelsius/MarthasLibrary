using MarthasLibrary.Core.Entities;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Fixtures;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace MarthasLibrary.IntegrationTests.Books;

[Collection("test collection")]
public sealed class DeleteTests : IDisposable
{
  private readonly TestFixture _fixture;
  private readonly LibraryDbContext _context;

  public DeleteTests()
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
  public async Task DeleteBook_SuccessfullyDeletesBook_WhenBookExists()
  {
    // Arrange
    var book = Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8));
    _context.Books.Add(book);
    await _context.SaveChangesAsync();

    // Act
    var response = await _fixture.Client.DeleteAsync($"api/Books/{book.Id}");

    // Assert
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    var fetch = await _fixture.Client.GetAsync(
      $"/api/Books/{book.Id}");
    Assert.Equal(HttpStatusCode.NotFound, fetch.StatusCode);
  }

  [Fact]
  public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
  {
    // Act
    var response = await _fixture.Client.DeleteAsync($"api/Books/{Guid.NewGuid()}");

    // Assert
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}