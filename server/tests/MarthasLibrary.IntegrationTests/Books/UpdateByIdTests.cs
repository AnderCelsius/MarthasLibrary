using MarthasLibrary.APIClient;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Fixtures;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace MarthasLibrary.IntegrationTests.Books;

[Collection("test collection")]
public sealed class UpdateByIdTests : IDisposable
{
  private readonly TestFixture _fixture;
  private readonly LibraryDbContext _context;
  public UpdateByIdTests()
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
  public async Task UpdateBookById_Returns404_WhenBookDoesNotExist()
  {
    var response = await _fixture.Client.PutAsJsonAsync<Books_UpdateById_Request_UpdatedDetails>(
      $"/api/books/{Guid.NewGuid()}",
      new()
      {
        Title = "Title",
        Author = "Author",
        Isbn = "9780446310789",
        PublishedDate = DateTimeOffset.Now
      });

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task UpdateBookById_Returns400_WhenUpdatingToDuplicateIsbn()
  {
    // arrange: set up 2 teams
    var book1 = Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8));
    var book2 = Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10));
    await _context.Books.AddRangeAsync(book1, book2);
    await _context.SaveChangesAsync();

    // act: try to update one team's email to the other email already in use
    var response = await _fixture.Client.PutAsJsonAsync<Books_UpdateById_Request_UpdatedDetails>(
      $"/api/books/{book1.Id}",
      new()
      {
        Title = book1.Title,
        Author = book1.Author,
        Isbn = book2.Isbn,
        PublishedDate = book1.PublishedDate,
      });

    // assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    Assert.Contains("already exists", await response.Content.ReadAsStringAsync());
  }

  [Fact]
  public async Task UpdateBookById_SuccessfullyUpdatesBook_WithValidRequest()
  {
    // arrange
    var book = Book.CreateInstance("1984", "George", "9780451524935", new DateTime(1949, 6, 8));

    await _context.AddAsync(book);
    await _context.SaveChangesAsync();

    var update = new Books_UpdateById_Request_UpdatedDetails()
    {
      Title = book.Title,
      Author = "George Orwell",
      Isbn = book.Isbn,
      PublishedDate = book.PublishedDate,
    };

    // act
    var response = await _fixture.Client.PutAsJsonAsync<Books_UpdateById_Request_UpdatedDetails>(
      $"/api/books/{book.Id}",
      update);

    // assert
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    var updatedBook = await _fixture.Client.GetFromJsonFixedAsync<Books_GetById_Response>(
      $"/api/books/{book.Id}");
    Assert.Equal(update.Title, updatedBook?.Book.Title);
    Assert.Equal(update.Author, updatedBook?.Book.Author);
    Assert.Equal(update.Isbn, updatedBook?.Book.Isbn);
  }
}