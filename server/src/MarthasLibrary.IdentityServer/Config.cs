using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace MarthasLibrary.IdentityServer;

public static class Config
{
  public static IEnumerable<IdentityResource> IdentityResources =>
    new IdentityResource[]
    {
      new IdentityResources.OpenId(),
      new IdentityResources.Profile(),
      new("roles",
        "Your role(s)",
        new[] { "role" })
    };

  public static IEnumerable<ApiResource> ApiResources =>
    new ApiResource[]
    {
      new("marthaslibraryapi",
        "Marthas Library API",
        new[] { "role" })
      {
        Scopes =
        {
          "marthaslibraryapi.fullaccess",
          "marthaslibraryapi.read",
          "marthaslibraryapi.write"
        },
        ApiSecrets = { new Secret("apisecret".Sha256()) }
      }
    };


  public static IEnumerable<ApiScope> ApiScopes =>
    new ApiScope[]
    {
      new("marthaslibraryapi.fullaccess"),
      new("marthaslibraryapi.read"),
      new("marthaslibraryapi.write")
    };

  public static IEnumerable<Client> Clients =>
    new Client[]
    {
      new()
      {
        ClientName = "Marthas Library Web App",
        ClientId = "marthaslibraryclient",
        AllowedGrantTypes = GrantTypes.Code,
        AccessTokenType = AccessTokenType.Reference,
        AllowOfflineAccess = true,
        UpdateAccessTokenClaimsOnRefresh = true,
        AccessTokenLifetime = 120,
        RedirectUris =
        {
          "https://localhost:7184/signin-oidc"
        },
        PostLogoutRedirectUris =
        {
          "https://localhost:7184/signout-callback-oidc"
        },
        AllowedScopes =
        {
          IdentityServerConstants.StandardScopes.OpenId,
          IdentityServerConstants.StandardScopes.Profile,
          "roles",
          "marthaslibraryapi.read",
          "marthaslibraryapi.write"
        },
        ClientSecrets =
        {
          new Secret("secret".Sha256())
        },
      }
    };
}