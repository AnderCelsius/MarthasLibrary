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
          new Claim(JwtClaimTypes.Name, "Alice Smith"),
          new Claim(JwtClaimTypes.GivenName, "Alice"),
          new Claim(JwtClaimTypes.FamilyName, "Smith"),
          new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
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
          new Claim(JwtClaimTypes.Name, "Obinna Asiegbulam"),
          new Claim(JwtClaimTypes.GivenName, "Obinna"),
          new Claim(JwtClaimTypes.FamilyName, "Asiegbulam"),
          new Claim(JwtClaimTypes.WebSite, "http://obai.com"),
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
}