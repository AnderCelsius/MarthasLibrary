using MarthasLibrary.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MarthasLibrary.Infrastructure.Data;

public static class SeedData
{
  public static void EnsureSeedUserData(WebApplication app)
  {
    using var scope = app.Services.GetRequiredService<IServiceScopeFactory>()
        .CreateScope();
    var context = scope.ServiceProvider.GetService<LibraryDbContext>();
    context!.Database.Migrate();

    SeedCustomers(context);
    SeedAddresses(context);
    SeedBooks(context);
  }

  private static void SeedBooks(LibraryDbContext context)
  {
    if (!context.Books.Any())
    {
      context.Books.AddRange(
        Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789", new DateTime(1960, 7, 11), "A novel that explores the issues of race and class in the Deep South of the 1930s."),
        Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8), "A dystopian novel that delves into the dangers of totalitarianism."),
        Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10), "A tale of love, wealth, and ambition, set in the roaring 1920s."),
        Book.CreateInstance("Pride and Prejudice", "Jane Austen", "9781936594291", new DateTime(1813, 1, 28), "A classic novel of manners, love, and marriage in early 19th-century England."),
        Book.CreateInstance("The Catcher in the Rye", "J.D. Salinger", "9780316769488", new DateTime(1951, 7, 16), "A story about teenage alienation and the loss of innocence."),
        Book.CreateInstance("The Hobbit", "J.R.R. Tolkien", "9780547928227", new DateTime(1937, 9, 21), "A fantasy novel about the journey of Bilbo Baggins, a hobbit who sets out on a grand adventure."),
        Book.CreateInstance("Harry Potter and the Sorcerer's Stone", "J.K. Rowling", "9780590353427", new DateTime(1997, 6, 26), "The first book in the Harry Potter series, introducing the young wizard and his adventures at Hogwarts.")
      );
      context.SaveChanges();
    }
  }

  private static void SeedAddresses(LibraryDbContext context)
  {
    if (!context.Addresses.Any())
    {
      var alice = context.Customers.FirstOrDefaultAsync(c => c.Email == "alice.smith@email.com").Result;
      var obai = context.Customers.FirstOrDefaultAsync(c => c.Email == "oasiegbulam@gmail.com").Result;

      if (alice != null && obai != null)
      {
        context.Addresses.AddRange(
            Address.CreateInstance(alice.Id, "123 Main St", "Metropolis", "Metro", "USA", "12345"),
            Address.CreateInstance(obai.Id, "456 Elm St", "Smallville", "Kansas", "USA", "67890")
        );
        context.SaveChanges();
      }
    }
  }

  private static void SeedCustomers(LibraryDbContext context)
  {
    var alice = context.Customers.FirstOrDefaultAsync(c => c.Email == "alice.smith@email.com").Result;

    if (alice == null)
    {
      alice = Customer.CreateInstance("Alice", "Smith", "alice.smith@email.com", "f0611528-36f2-4a0e-80ce-96dea6ebd13f");
      alice.SetAsActive();

      context.Customers.Add(alice);
      context.SaveChanges();

      Log.Debug("alice created");
    }
    else
    {
      Log.Debug("alice already exists");
    }

    var obai = context.Customers.FirstOrDefaultAsync(c => c.Email == "oasiegbulam@gmail.com").Result;

    if (obai == null)
    {
      obai = Customer.CreateInstance("Obinna", "Asiegbulam", "oasiegbulam@gmail.com", "83ec2132-9104-4615-814b-11cba2374e41");
      obai.SetAsActive();

      context.Customers.Add(obai);
      context.SaveChanges();

      Log.Debug("obai created");
    }
    else
    {
      Log.Debug("obai already exists");
    }
  }
}
