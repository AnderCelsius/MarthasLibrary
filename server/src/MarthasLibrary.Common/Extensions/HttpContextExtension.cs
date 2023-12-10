using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace MarthasLibrary.Common.Extensions;

public static class HttpContextExtension
{
  public const string UserEmailHeader = "User-Email";

  public const string UserScopeHeader = "User-Scope";

  public const string IdentityUserId = "User-IdentityId";

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
    return httpContext?.Items["UserName"] as string ?? string.Empty;
  }

  public static void SetEmailInHttpContext(this HttpContext httpContext, string email) =>
      httpContext.Request.Headers[UserEmailHeader] = email;

  public static void SetScopeInHttpContext(this HttpContext httpContext, string scope) =>
      httpContext.Request.Headers[UserScopeHeader] = scope.ToLower();

  public static void SetIdentityIdInHttpContext(this HttpContext httpContext, string sub) =>
      httpContext.Request.Headers[IdentityUserId] = sub.ToLower();

  public static string? GetScopeFromHttpContext(this HttpContext httpContext)
  {
    string userScope = httpContext.Request.Headers[UserScopeHeader];
    return string.IsNullOrEmpty(userScope) ? null : userScope;
  }
}