using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace MarthasLibrary.Application.UserData;

/// <summary>
/// Static class to contain extension method that registers user data providers.
/// </summary>
public static class UserDataExtensions
{
  public const string UserEmailHeader = "User-Email";

  public const string UserScopeHeader = "User-Scope";

  /// <summary>
  /// Register the User Data providers.
  /// </summary>
  /// <param name="services">Service Collection to add the token providers to.</param>
  /// <returns>Service Collection.</returns>
  public static IServiceCollection AddUserDataProvider(this IServiceCollection services)
  {
    services.AddHttpContextAccessor();

    services.AddScoped<IUserDataProvider<UserBasicData>, UserBasicDataProvider>();
    services.AddScoped<IUserDataProvider<UserData>, UserDataProvider>();

    return services;
  }

  public static UserData EnsureAuthenticated(this UserData? userData)
  {
    if (userData is null)
    {
      throw new UserUnauthenticatedException();
    }

    return userData;
  }

  public static string? GetEmailFromHttpContext(this HttpContext httpContext)
  {
    var userEmailHeader = httpContext.Request.Headers[UserEmailHeader];

    if (StringValues.IsNullOrEmpty(userEmailHeader))
    {
      return null;
    }

    return userEmailHeader;
  }

  public static string? GetIdentityUserIdFromHttpContext(this HttpContext? httpContext)
  {
    var identityUserId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    identityUserId ??= httpContext?.User.FindFirst(JwtClaimTypes.Subject)?.Value;
    return identityUserId;
  }

  public static void SetEmailInHttpContext(this HttpContext httpContext, string email) =>
    httpContext.Request.Headers[UserEmailHeader] = email;

  public static void SetScopeInHttpContext(this HttpContext httpContext, string scope) =>
    httpContext.Request.Headers[UserScopeHeader] = scope.ToLower();

  public static string? GetScopeFromHttpContext(this HttpContext httpContext)
  {
    string userScope = httpContext.Request.Headers[UserScopeHeader];
    return string.IsNullOrEmpty(userScope) ? null : userScope;
  }
}
