using IdentityModel;
using MarthasLibrary.IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MarthasLibrary.IdentityServer.Factories;

public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
  private readonly ILogger<ApplicationUserClaimsPrincipalFactory> _logger;

  public ApplicationUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<ApplicationUserClaimsPrincipalFactory> logger) : base(userManager, optionsAccessor)
  {
    _logger = logger;
  }

  protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
  {
    var claimsIdentity = await base.GenerateClaimsAsync(user);

    // Add standard JWT claims for name and role
    var claimsForUser = await UserManager.GetClaimsAsync(user);
    var givenNameClaim = claimsForUser.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName);
    var familyNameClaim = claimsForUser.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName);

    _logger.LogInformation("Generating claims for user: {UserId}", user.Id);

    if (givenNameClaim is not null)
    {
      _logger.LogInformation("Adding GivenName claim: {GivenName}", user.FirstName);
      claimsIdentity.AddClaim(new Claim("FirstName", givenNameClaim.Value));
    }

    if (familyNameClaim is not null)
    {
      _logger.LogInformation("Adding FamilyName claim: {FamilyName}", user.FirstName);
      claimsIdentity.AddClaim(new Claim("LastName", familyNameClaim.Value));
    }

    return claimsIdentity;
  }
}
