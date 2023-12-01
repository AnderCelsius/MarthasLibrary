using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using IdentityModel;
using MarthasLibrary.IdentityProvider.Data;
using MarthasLibrary.IdentityProvider.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace MarthasLibrary.IdentityProvider;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context.Database.Migrate();

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configurationContext = scope.ServiceProvider
            .GetService<ConfigurationDbContext>();

        SeedIdentityUsers(userMgr);
        SeedIdentityServerData(configurationContext);
    }

    private static void SeedIdentityUsers(UserManager<ApplicationUser> userMgr)
    {
        var alice = userMgr.FindByNameAsync("alice").Result;
        if (alice == null)
        {
            alice = new()
            {
                Id = "f0611528-36f2-4a0e-80ce-96dea6ebd13f",
                FirstName = "Alice",
                LastName = "Smith",
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
            obai = new()
            {
                Id = "83ec2132-9104-4615-814b-11cba2374e41",
                FirstName = "Obinna",
                LastName = "Asiegbulam",
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

    private static void SeedIdentityServerData(ConfigurationDbContext context)
    {
        if (!context.Clients.Any())
        {
            Log.Debug("Clients being populated");
            foreach (var client in Config.Clients.ToList())
            {
                context.Clients.Add(client.ToEntity());
            }
            context.SaveChanges();
        }
        else
        {
            Log.Debug("Clients already populated");
        }

        if (!context.IdentityResources.Any())
        {
            Log.Debug("IdentityResources being populated");
            foreach (var resource in Config.IdentityResources.ToList())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            context.SaveChanges();
        }
        else
        {
            Log.Debug("IdentityResources already populated");
        }

        if (!context.ApiScopes.Any())
        {
            Log.Debug("ApiScopes being populated");
            foreach (var resource in Config.ApiScopes.ToList())
            {
                context.ApiScopes.Add(resource.ToEntity());
            }
            context.SaveChanges();
        }
        else
        {
            Log.Debug("ApiScopes already populated");
        }

        if (!context.ApiResources.Any())
        {
            Log.Debug("ApiResources being populated");
            foreach (var resource in Config.ApiResources.ToList())
            {
                context.ApiResources.Add(resource.ToEntity());
            }
            context.SaveChanges();
        }
        else
        {
            Log.Debug("ApiResources already populated");
        }
    }
}