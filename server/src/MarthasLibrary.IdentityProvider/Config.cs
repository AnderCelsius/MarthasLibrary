using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace MarthasLibrary.IdentityProvider;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new ("roles",
                "Your role(s)",
                new[] { "role" }),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new("matharslibraryapi",
                "Marthas Library API",
                new[] { "role" })
            {
                Scopes =
                {
                    "matharslibraryapi.fullaccess",
                    "matharslibraryapi.read",
                    "matharslibraryapi.write"
                },
                ApiSecrets = { new Secret("apisecret".Sha256()) }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("matharslibraryapi.fullaccess"),
            new("matharslibraryapi.read"),
            new("matharslibraryapi.write"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new()
            {
                ClientName = "Marthas Library",
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
                    "matharslibraryapi.read",
                    "matharslibraryapi.write",
                },
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
            }
        };
}