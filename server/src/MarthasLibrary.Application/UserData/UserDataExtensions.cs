using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

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

    return services;
  }


  public static UserBasicData EnsureAuthenticated(this UserBasicData? userData)
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
