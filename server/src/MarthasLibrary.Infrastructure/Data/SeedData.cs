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
                Book.CreateInstance("To Kill a Mockingbird", "Harper Lee", "9780446310789", new DateTime(1960, 7, 11)),
                Book.CreateInstance("1984", "George Orwell", "9780451524935", new DateTime(1949, 6, 8)),
                Book.CreateInstance("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new DateTime(1925, 4, 10))
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
            alice = new("Alice", "Smith", "alice.smith@email.com", "f0611528-36f2-4a0e-80ce-96dea6ebd13f");
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
            obai = new("Obinna", "Asiegbulam", "oasiegbulam@gmail.com", "83ec2132-9104-4615-814b-11cba2374e41");
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
