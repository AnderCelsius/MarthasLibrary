using MarthasLibrary.Application.UserData;
using MarthasLibrary.Common.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MarthasLibrary.API.Extensions;

public static class WebApplicationAuthenticationExtenstion
{
    public static void AddExternalServiceAuthentication(
        this WebApplicationBuilder builder)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        builder.Services.AddAuthentication("token")
            .AddJwtBearer("token", opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.Authority = builder.Configuration["DuendeISP:Authority"];
                opt.Audience = builder.Configuration["DuendeISP:Audience"];
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "given_name",
                    RoleClaimType = "role",
                    ValidTypes = new[] { "at+jwt" },
                };
                opt.ForwardDefaultSelector = ForwardReferenceToken();

                opt.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (context.SecurityToken is JwtSecurityToken jwtSecurityToken)
                        {
                            if (jwtSecurityToken.Payload.TryGetValue("email", out var email) &&
                                email is string emailStr)
                            {
                                claimsIdentity?.AddClaim(new Claim(ClaimTypes.Email, emailStr));
                                context.HttpContext.SetEmailInHttpContext(emailStr);
                            }

                            if (jwtSecurityToken.Payload.TryGetValue("scp", out var scope) &&
                                scope is string scopeStr)
                            {
                                context.HttpContext.SetScopeInHttpContext(scopeStr);
                            }

                            if (jwtSecurityToken.Payload.TryGetValue("sub", out var sub) &&
                                sub is string subStr)
                            {
                                context.HttpContext.SetScopeInHttpContext(subStr);
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            })
            .AddOAuth2Introspection(options =>
            {
                options.Authority = builder.Configuration["DuendeISP:Authority"];
                options.ClientId = builder.Configuration["DuendeISP:ClientId"];
                options.ClientSecret = builder.Configuration["DuendeISP:ClientSecret"];
                options.NameClaimType = "given_name";
                options.RoleClaimType = "role";
            });

        builder.Services.AddAuthorization(authorizationOptions =>
        {
            authorizationOptions.AddPolicy(
                Policies.LibraryStaff, AuthorizationPolicies.CanAddBook());
            authorizationOptions.AddPolicy(
                Policies.ClientApplicationCanWrite,
                policyBuilder => { policyBuilder.RequireClaim("scope", "marthaslibraryapi.write"); });
            authorizationOptions.AddPolicy(
                Policies.CanApproveBorrowRequest, policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser()
                        .RequireRole(Policies.IsAdmin);
                });
        });
    }

    /// <summary>
    /// Provides a forwarding func for JWT vs reference tokens (based on existence of dot in token)
    /// </summary>
    /// <param name="introspectionScheme">Scheme name of the introspection handler</param>
    /// <returns></returns>
    public static Func<HttpContext, string?> ForwardReferenceToken(string? introspectionScheme = "introspection")
    {
        string? Select(HttpContext context)
        {
            var (scheme, credential) = GetSchemeAndCredential(context);
            if (scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) &&
                !credential.Contains("."))
            {
                return introspectionScheme;
            }

            return null;
        }

        return Select;
    }

    /// <summary>
    /// Extracts scheme and credential from Authorization header (if present)
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static (string, string) GetSchemeAndCredential(HttpContext context)
    {
        var header = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(header))
        {
            return ("", "");
        }

        var parts = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return ("", "");
        }

        return (parts[0], parts[1]);
    }
}