using MarthasLibrary.APIClient;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Fixtures;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace MarthasLibrary.IntegrationTests.Books;

[Collection("test collection")]
public sealed class SearchTests : IDisposable
{
  private readonly TestFixture _fixture;
  private readonly LibraryDbContext _context;

  public SearchTests()
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
  public async Task Search_ReturnsBooks_WhenQueryMatchesTitleOrAuthor()
  {
    // Arrange
    var books = new List<Book>
    {
      Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8)),
      Book.CreateInstance("Animal Farm", "George Orwell", "9780451526342", new DateTime(1945, 8, 17)),
      Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10))
    };

    await Seeder.SeedData(books, _context);

    // Act
    var searchTerm = "Orwell";
    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_Search_Response>(
      $"api/Books/search?query={searchTerm}");

    // Assert
    Assert.NotNull(response);
    Assert.NotEmpty(response!.Books);
    Assert.All(response.Books, book =>
      Assert.True(book.Title.Contains(searchTerm) || book.Author.Contains(searchTerm))
    );
  }

  [Fact]
  public async Task Search_ReturnsEmptyCollection_WhenQueryDoesNotMatch()
  {
    // Arrange
    var books = new List<Book>
    {
      Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8)),
      Book.CreateInstance("Animal Farm", "George Orwell", "9780451526342", new DateTime(1945, 8, 17)),
      Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10))
    };

    await Seeder.SeedData(books, _context);

    // Act
    var searchTerm = "Hemingway";
    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_Search_Response>(
      $"api/Books/search?query={searchTerm}");

    // Assert
    Assert.NotNull(response);
    Assert.Empty(response!.Books);
  }

  [Fact]
  public async Task Search_ReturnsBadRequest_WhenQueryIsNullOrEmpty()
  {
    // Act
    var searchTerm = string.Empty; // Also test for null if your API should handle it.
    var response = await _fixture.Client.GetAsync($"api/Books/search?query={searchTerm}");

    // Assert
    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
  }
}