using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using MarthasLibrary.Common.Authorization;
using MarthasLibrary.IdentityServer.Entities;
using MarthasLibrary.IdentityServer.Factories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MarthasLibrary.IdentityServer.Services;

public class CustomProfileService : IProfileService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ApplicationUserClaimsPrincipalFactory _claimsFactory;
  private readonly ILogger<CustomProfileService> _logger;

  public CustomProfileService(UserManager<ApplicationUser> userManager, ApplicationUserClaimsPrincipalFactory claimsFactory, ILogger<CustomProfileService> logger)
  {
    _userManager = userManager;
    _claimsFactory = claimsFactory;
    _logger = logger;
  }

  public async Task GetProfileDataAsync(ProfileDataRequestContext context)
  {
    _logger.LogInformation("GetProfileDataAsync called for subject {subject}", context.Subject.GetSubjectId());
    var sub = context.Subject.GetSubjectId();
    var user = await _userManager.FindByIdAsync(sub);

    if (user != null)
    {
      var userClaims = await _userManager.GetClaimsAsync(user);

      _logger.LogInformation("Claims for user {UserId}: {Claims}", user.Id, userClaims.Select(c => $"{c.Type}: {c.Value}").ToList());

      var principal = await _claimsFactory.CreateAsync(user);
      var customClaims = principal.Claims.Where(c => c.Type == CustomClaimTypes.FirstName || c.Type == CustomClaimTypes.LastName);

      var enumerable = customClaims as Claim[] ?? customClaims.ToArray();
      var customClaimsText = string.Join(", ", enumerable.ToList());

      _logger.LogInformation("Custom Claims are retrieved: {customClaims}", customClaimsText);

      context.AddRequestedClaims(userClaims.Select(c => new Claim(c.Type, c.Value)).ToList());
      context.IssuedClaims.AddRange(userClaims.Where(c => context.RequestedClaimTypes.Contains(c.Type)));
      context.IssuedClaims.Add(new Claim("sub", sub));
    }
  }

  public async Task IsActiveAsync(IsActiveContext context)
  {
    _logger.LogInformation("IsActiveAsync called for subject {subject}", context.Subject.GetSubjectId());
    var sub = context.Subject.GetSubjectId();
    var user = await _userManager.FindByIdAsync(sub);
    context.IsActive = user.IsActive;
  }
}
