using MarthasLibrary.Common.Authorization;
using MarthasLibrary.Common.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MarthasLibrary.Common.Authentication;

public static class WebApplicationAuthenticationBuilder
{
  public static void AddOpenIdConnectAuthentication(
  this WebApplicationBuilder builder)
  {
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    builder.Services.AddAuthentication(opt =>
    {
      opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
      .AddOpenIdConnect(configureOptions: ConfigureOpenIdConnect)
      .AddCookie(o =>
      {
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        //o.Cookie.SameSite = SameSiteMode.Strict;
        o.Cookie.HttpOnly = true;
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
    return;

    void ConfigureOpenIdConnect(OpenIdConnectOptions config)
    {
      config.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      config.Authority = builder.Configuration["DuendeISP:Authority"];
      config.ClientId = builder.Configuration["DuendeISP:ClientId"];
      config.ClientSecret = builder.Configuration["DuendeISP:Secret"];
      config.ResponseType = OpenIdConnectResponseType.Code;
      config.GetClaimsFromUserInfoEndpoint = true;

      // Configure claims
      config.ClaimActions.DeleteClaim("sid");
      config.ClaimActions.DeleteClaim("idp");
      config.ClaimActions.DeleteClaim("auth_time");
      config.ClaimActions.DeleteClaim("s_hash");

      // Configure the scope
      config.Scope.Clear();
      config.Scope.Add(OpenIdConnectScope.OpenId);
      config.Scope.Add("profile");
      config.Scope.Add("offline_access");
      config.Scope.Add("marthaslibraryapi.write");
      config.Scope.Add("marthaslibraryapi.read");

      // Validate Token
      config.TokenValidationParameters = new TokenValidationParameters
      {
        NameClaimType = "name",
        RequireExpirationTime = true,
        RequireSignedTokens = true,
        ValidateAudience = true,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        //ValidIssuers = builder.Configuration.GetSection("DuendeISP:Issuers").Get<List<string>>(),
      };

      // This saves the tokens in the session cookie
      config.SaveTokens = true;
      config.Events.OnTokenValidated += context =>
      {
        var logger = context.Request.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>().CreateLogger("TokenValidation");
        logger.LogInformation("Upstream token: {Token}",
                context.SecurityToken.RawData);
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
        }

        return Task.CompletedTask;
      };
      config.Events.OnRedirectToIdentityProvider += context =>
      {
        // Generate code_verifier and code_challenge
        var codeVerifier = PkceHelper.CreateCodeVerifier();
        var codeChallenge = PkceHelper.CreateCodeChallenge(codeVerifier);

        // Store the code_verifier in a temporary storage for later use
        context.Properties.Items["code_verifier"] = codeVerifier;

        // Include the code_challenge and code_challenge_method in the request if they don't exist
        if (!context.ProtocolMessage.Parameters.ContainsKey("code_challenge"))
        {
          context.ProtocolMessage.Parameters.Add("code_challenge",
                  codeChallenge);
        }

        if (!context.ProtocolMessage.Parameters.ContainsKey(
                    "code_challenge_method"))
        {
          context.ProtocolMessage.Parameters.Add("code_challenge_method",
                  "S256");
        }

        // Add the audience parameter if it doesn't exist
        if (!context.ProtocolMessage.Parameters.ContainsKey("audience"))
        {
          context.ProtocolMessage.SetParameter("audience",
                  builder.Configuration["DuendeISP:Audience"]);
        }

        return Task.CompletedTask;
      };

      config.Events.OnAuthorizationCodeReceived += context =>
      {
        // Retrieve the code_verifier from temporary storage
        if (context.Properties!.Items.TryGetValue("code_verifier",
                    out var codeVerifier))
        {
          context.TokenEndpointRequest!.Parameters.Add("code_verifier",
                  codeVerifier);
        }

        return Task.CompletedTask;
      };
    }
  }
}