using IdentityModel;
using MarthasLibrary.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Security.Claims;

namespace MarthasLibrary.Infrastructure.Data;

public static class SeedData
{
    public static void EnsureSeedUserData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        var context = scope.ServiceProvider.GetService<LibraryDbContext>();
        context.Database.Migrate();

        var userMgr = scope.ServiceProvider
            .GetRequiredService<UserManager<Customer>>();

        SeedCustomers(userMgr);
        SeedAddresses(context, userMgr);
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

    private static void SeedAddresses(LibraryDbContext context, UserManager<Customer> userMgr)
    {
        if (!context.Addresses.Any())
        {
            var alice = userMgr.FindByNameAsync("alice").Result;
            var bob = userMgr.FindByNameAsync("bob").Result;

            if (alice != null && bob != null)
            {
                context.Addresses.AddRange(
                    Address.CreateInstance(alice.Id, "123 Main St", "Metropolis", "Metro", "USA", "12345"),
                    Address.CreateInstance(bob.Id, "456 Elm St", "Smallville", "Kansas", "USA", "67890")
                // Add more addresses as needed
                );
                context.SaveChanges();
            }
        }
    }

    private static void SeedCustomers(UserManager<Customer> userMgr)
    {
        var alice = userMgr.FindByNameAsync("alice").Result;
        if (alice == null)
        {
            alice = new Customer("Alice", "Smith")
            {
                Email = "alice.smith@email.com",
                UserName = nameof(alice),
                EmailConfirmed = true,
                IsActive = true
            };
            var result = userMgr.CreateAsync(alice, "Pass123$").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(alice, new Claim[]
            {
                new(JwtClaimTypes.Name, "Alice Smith"),
                new(JwtClaimTypes.GivenName, "Alice"),
                new(JwtClaimTypes.FamilyName, "Smith"),
                new(JwtClaimTypes.WebSite, "http://alice.com")
            }).Result;

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            Log.Debug("alice created");
        }
        else
        {
            Log.Debug("alice already exists");
        }

        var obai = userMgr.FindByNameAsync("obai").Result;
        if (obai == null)
        {
            obai = new Customer("Obinna", "Asiegbulam")
            {
                Email = "oasiegbulam@gmail.com",
                UserName = nameof(obai),
                EmailConfirmed = true,
                IsActive = true
            };
            var result = userMgr.CreateAsync(obai, "Pass123$").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(obai, new Claim[]
            {
                new(JwtClaimTypes.Name, "Obinna Asiegbulam"),
                new(JwtClaimTypes.GivenName, "Obinna"),
                new(JwtClaimTypes.FamilyName, "Asiegbulam"),
                new(JwtClaimTypes.WebSite, "http://asiegbulam.com")
            }).Result;

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            Log.Debug("obai created");
        }
        else
        {
            Log.Debug("obai already exists");
        }
    }
}