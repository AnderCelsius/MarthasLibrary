using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using IdentityModel;
using MarthasLibrary.IdentityServer.Data;
using MarthasLibrary.IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace MarthasLibrary.IdentityServer
{
    public class SeedData
    {
        public static void EnsureSeedData(WebApplication app)
        {
            using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.Migrate();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configContext = scope.ServiceProvider
                .GetService<ConfigurationDbContext>();
            EnsureSeedData(configContext);

            SeedUsers(userMgr);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userMgr)
        {
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    Id = "f0611528-36f2-4a0e-80ce-96dea6ebd13f",
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
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
                    new(JwtClaimTypes.Role, "Customer"),
                    new(JwtClaimTypes.WebSite, "http://alice.com"),
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
                obai = new ApplicationUser
                {
                    Id = "83ec2132-9104-4615-814b-11cba2374e41",
                    UserName = "obai",
                    Email = "oasiegbulam@gmail.com",
                    EmailConfirmed = true
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
                    new(JwtClaimTypes.Role, "Admin"),
                    new(JwtClaimTypes.WebSite, "http://obai.com"),
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

            var josh = userMgr.FindByNameAsync("josh").Result;
            if (josh == null)
            {
                josh = new ApplicationUser
                {
                    Id = "13229d33-99e0-41b3-b18d-4f72127e3971",
                    UserName = "josh",
                    Email = "chris.josh@gmail.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(josh, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(josh, new Claim[]
                {
                    new(JwtClaimTypes.Name, "Chris Josh"),
                    new(JwtClaimTypes.GivenName, "Chris"),
                    new(JwtClaimTypes.FamilyName, "Josh"),
                    new(JwtClaimTypes.Role, "LibraryStaff"),
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("josh created");
            }
            else
            {
                Log.Debug("josh already exists");
            }
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
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
}