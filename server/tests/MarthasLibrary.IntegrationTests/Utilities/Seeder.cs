using MarthasLibrary.Core.Entities;
using MarthasLibrary.Infrastructure.Data;

namespace MarthasLibrary.IntegrationTests.Utilities;

public static class Seeder
{
  public static async Task SeedData(List<Book> books, LibraryDbContext context)
  {
    await context.Books.AddRangeAsync(books);
    await context.SaveChangesAsync();
  }
}