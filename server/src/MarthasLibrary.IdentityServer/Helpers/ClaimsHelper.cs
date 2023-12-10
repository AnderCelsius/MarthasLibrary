using MarthasLibrary.IdentityServer.Dtos;
using MarthasLibrary.IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Claims;

namespace MarthasLibrary.IdentityServer.Helpers
{
  public static class ClaimsHelper
  {
    public static void GetPermission(this List<ClaimDto> allPermissions,
        Type policy)
    {
      FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

      foreach (var item in fields)
      {
        // allPermissions.Add(new ClaimDto() { Value = item.GetValue(null)?.ToString(), Type = "Permissions" });
      }
    }

    public static async Task AddPermissionClaim(
        this RoleManager<IdentityRole> roleManager,
        IdentityRole role, string? permission)
    {
      var allClaims = await roleManager.GetClaimsAsync(role);

      if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
      {
        await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
      }
    }

    public static async Task AddPermissionClaim(
        this UserManager<ApplicationUser> userManager,
        ApplicationUser user, string? permission)
    {
      var allClaims = await userManager.GetClaimsAsync(user);

      if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
      {
        await userManager.AddClaimAsync(user, new Claim("Permission", permission));
      }
    }
  }
}
