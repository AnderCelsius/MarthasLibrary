﻿using MarthasLibrary.APIClient;
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

  public GetAllTests()
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
  public async Task GetAll_ReturnsArray_WhenOneBookExists()
  {
    const string isbn = "9780451524935";

    var books = new List<Book>
    {
      Book.CreateInstance("1984", "George Orwell", isbn, new DateTime(1949, 6, 8)),
    };


    await SeedData(books);

    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_GetAll_Response>(
      "api/Books");

    Assert.NotNull(response);
    Assert.Collection(response!.Books,
      bookDetails => Assert.Equal(books[0].Isbn, bookDetails.Isbn));
  }

  [Fact]
  public async Task GetAll_ReturnsArray_WhenMultipleBooksExist()
  {
    // arrange: Set up 2 otpConfigurations
    var books = new List<Book>
    {
      Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8)),
      Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10))
    };


    await SeedData(books);

    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_GetAll_Response>(
      "api/Books");

    Assert.NotNull(response);
    Assert.Collection(
      response!.Books,
      bk1 => Assert.Equal(books[0].Title, bk1.Title),
      bk2 => Assert.Equal(books[1].Title, bk2.Title));
  }

  [Fact]
  public async Task GetAll_ReturnsEmptyArray_WhenNoOtpConfigurationExist()
  {
    var response = await _fixture.Client.GetFromJsonFixedAsync<Books_GetAll_Response>(
      "api/Books");

    Assert.NotNull(response);
    Assert.Empty(response!.Books);
  }

  private async Task SeedData(List<Book> books)
  {
    await _context.Books.AddRangeAsync(books);
    await _context.SaveChangesAsync();
  }
}