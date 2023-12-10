using MarthasLibrary.APIClient;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Infrastructure.Data;
using MarthasLibrary.IntegrationTests.Fixtures;
using MarthasLibrary.IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace MarthasLibrary.IntegrationTests.Books;

[Collection("test collection")]
public sealed class GetAllTests : IDisposable
{
  private readonly TestFixture _fixture;
  private readonly LibraryDbContext _context;
  private readonly IServiceScope _serviceScope;

  public GetAllTests()
  {
    _fixture = new TestFixture();
    _fixture.MockServer.Reset();
    _fixture.MockServer.MockAuthentication(true);

    _serviceScope = _fixture.Server.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    _context = _serviceScope.ServiceProvider.GetRequiredService<LibraryDbContext>();
  }

  public void Dispose()
  {
    _fixture.Dispose();
    _context.Dispose();
    _serviceScope.Dispose();
  }

  [Fact]
  public async Task GetAll_ReturnsArray_WhenOneBookExists()
  {
    const string isbn = "9780451524935";

    var books = new List<Book>
    {
      Book.CreateInstance("1984", "George Orwell", isbn, new DateTime(1949, 6, 8)),
    };


    await Seeder.SeedBook(books, _context);

    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_GetAll_Response>(
      "api/Books");

    Assert.NotNull(response);
    Assert.Collection(response!.Books,
      bookDetails => Assert.Equal(books[0].Isbn, bookDetails.Isbn));
  }

  [Fact]
  public async Task GetAll_ReturnsArray_WhenMultipleBooksExist()
  {
    await _context.Database.EnsureDeletedAsync();
    await _context.Database.EnsureCreatedAsync();

    // arrange: Set up 2 otpConfigurations
    var books = new List<Book>
    {
      Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8)),
      Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10))
    };


    await Seeder.SeedBook(books, _context);

    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_GetAll_Response>(
      "api/Books");

    Assert.NotNull(response);
    Assert.Collection(
      response!.Books,
      bk1 => Assert.Equal(books[0].Title, bk1.Title),
      bk2 => Assert.Equal(books[1].Title, bk2.Title));
  }

  [Fact]
  public async Task GetAll_ReturnsCorrectPage_WhenPaginatedQuerySent()
  {
    // arrange: Set up 2 otpConfigurations
    const int pageNumber = 1;
    const int pageSize = 1;
    var books = new List<Book>
    {
      Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8)),
      Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10))
    };


    await Seeder.SeedBook(books, _context);

    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_GetAll_Response>(
      $"api/Books?pageNumber={pageNumber}&pageSize={pageSize}");

    Assert.NotNull(response);
    Assert.NotNull(response.Books);

    Assert.Single(response.Books);

    Assert.Equal(books[0].Title, response.Books.ToList()[0].Title);
  }

  [Fact]
  public async Task GetAll_ReturnsEmptyArray_WhenNoBookExist()
  {
    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_GetAll_Response>(
      "api/Books");

    Assert.NotNull(response);
    Assert.Empty(response!.Books);
  }
}