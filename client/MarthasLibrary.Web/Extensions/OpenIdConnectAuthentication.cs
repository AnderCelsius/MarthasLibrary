using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MarthasLibrary.Web.Extensions;

public static class OpenIdConnectAuthentication
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
      config.Scope.Add("marthaslibraryapi.read");
      config.Scope.Add("marthaslibraryapi.write");

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
    }
  }
}
