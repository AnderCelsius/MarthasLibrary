using MarthasLibrary.APIClient;
using MarthasLibrary.Core.Entities;
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

    [Fact]
    public async Task CreateBook_ReturnsStatus400_WhenTheIsbnIsNotUnique()
    {
        const string isbn = "9780451524935";

        var books = new List<Book>
        {
            Book.CreateInstance("1984", "George Orwell", isbn, new DateTime(1949, 6, 8)),
            Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10))
        };


        await Seeder.SeedBook(books, _context);

        var response = await _fixture.Client.PostAsJsonAsync<Books_Create_Request>(
            "api/Books",
            new()
            {
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                Isbn = isbn,
                PublishedDate = DateTimeOffset.UtcNow,
            },
            JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains($"A different book with this Isbn: '{isbn}' already exist in record",
            await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("To Kill a Mockingbird", "Harper Lee", "string", null)]
    [InlineData("", "Some Author", "9780451524989", "2023-11-29T10:39:26.435Z")]
    [InlineData("To Kill a Mockingbird", "", "9780451654935", "2023-11-29T10:39:26.435Z")]
    [InlineData("To Kill a Mockingbird", "H", "9780451524935", "2023-11-29T10:39:26.435Z")]
    [InlineData("To Kill a Mockingbird", "Harper Lee", "9780451524935", null)]
    public async Task CreateBook_ReturnsBadRequest_OnInvalidInput(string title, string author, string isbn,
        DateTimeOffset publishedDate)
    {
        // Arrange & Act
        var response = await _fixture.Client.PostAsJsonAsync<Books_Create_Request>(
            "api/Books",
            new()
            {
                Title = title,
                Author = author,
                Isbn = isbn,
                PublishedDate = publishedDate,
            },
            JsonSerializerOptions);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("One or more validation errors occurred", responseContent);
    }
}