using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace MarthasLibrary.Tests.Common.Helpers;

public class MockAuthenticationService : IAuthenticationService
{
    // Simulates the authentication process by creating a ClaimsIdentity and AuthenticationTicket.
    //public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
    //{
    //    var identity = new ClaimsIdentity(scheme);
    //    var principal = new ClaimsPrincipal(identity);
    //    var ticket = new AuthenticationTicket(principal, scheme);
    //    return Task.FromResult(AuthenticateResult.Success(ticket));
    //}

    public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
    {
        try
        {
            string? authorizationHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                // Token is missing or not in the correct format
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Task.FromResult(AuthenticateResult.Fail("Bearer token is missing or invalid."));
            }

            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // Decode and parse the JWT token to extract claims
            var claims = ParseClaimsFromToken(token);

            // Create a ClaimsIdentity and AuthenticationTicket with extracted claims
            var identity = new ClaimsIdentity(claims, scheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.FromResult(AuthenticateResult.Fail("UnAuthorized"));
        }
    }

    /// <summary>
    /// Simulates a challenge response during authentication.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
    {
        return DefaultAuthResponse(scheme);
    }

    /// <summary>
    ///  Simulates a forbidden response during authentication.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
    {
        return DefaultAuthResponse(scheme);
    }

    /// <summary>
    /// Simulates signing in a user by returning a successful authentication result.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="principal"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties)
    {
        return DefaultAuthResponse(scheme);
    }

    /// <summary>
    /// Simulates signing out a user by returning a successful authentication result.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
    {
        return DefaultAuthResponse(scheme);
    }

    /// <summary>
    /// Helper method to create a default authentication response.
    /// </summary>
    /// <param name="scheme"></param>
    /// <returns></returns>
    private static Task DefaultAuthResponse(string? scheme)
    {
        var identity = new ClaimsIdentity(scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, scheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    // Helper method to parse claims from the JWT token
    private IEnumerable<Claim> ParseClaimsFromToken(string token)
    {
        // Add your logic to decode and extract claims from the token (use a JWT library or custom logic)
        // Example: Parse claims from a JWT token
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken != null)
        {
            return jsonToken.Claims;
        }

        throw new InvalidOperationException("Invalid Token");
    }
}
