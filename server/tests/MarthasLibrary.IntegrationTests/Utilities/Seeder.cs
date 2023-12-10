using MarthasLibrary.Core.Entities;
using MarthasLibrary.Infrastructure.Data;

namespace MarthasLibrary.IntegrationTests.Utilities;

public static class Seeder
{
  public static async Task SeedBook(List<Book> books, LibraryDbContext context)
  {
    await context.Books.AddRangeAsync(books);
    await context.SaveChangesAsync();
  }
  
  public static async Task SeedCustomers(List<Customer> customers, LibraryDbContext context)
  {
    foreach (var customer in customers)
    {
      customer.SetAsActive();
    }
    await context.Customers.AddRangeAsync(customers);
    await context.SaveChangesAsync();
  }
}