using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;
using MarthasLibrary.Common.Authorization;

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
                ApiSecrets = { new Secret("apisecret".Sha256()) },
                UserClaims =
                {
                    JwtClaimTypes.GivenName, JwtClaimTypes.FamilyName, CustomClaimTypes.FirstName,
                    CustomClaimTypes.LastName
                }
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
            // interactive ASP.NET Core Web App
            new()
            {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect to after login
                RedirectUris = { "https://localhost:7209/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:7209/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles",
                    "marthaslibraryapi.read",
                    "marthaslibraryapi.write"
                },
                AlwaysIncludeUserClaimsInIdToken = true,
            },
            new()
            {
                ClientId = "blazor-client",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect to after login
                RedirectUris = { "https://localhost:7209/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:7209/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles",
                    "marthaslibraryapi.read",
                    "marthaslibraryapi.write"
                },
                AlwaysIncludeUserClaimsInIdToken = true,
            },
            new()
            {
                ClientName = "Marthas Library Web App",
                ClientId = "marthaslibraryclient",
                AllowedGrantTypes = GrantTypes.Code,
                AccessTokenType = AccessTokenType.Jwt,
                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                AccessTokenLifetime = 120,
                RedirectUris =
                {
                    "https://localhost:7155/signin-oidc",
                    "https://localhost:7155/swagger/oauth2-redirect.html",
                    "https://oauth.pstmn.io/v1/callback",
                },
                PostLogoutRedirectUris =
                {
                    "https://localhost:7155/signout-callback-oidc"
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
                    new Secret("apisecret".Sha256())
                },
                AllowAccessTokensViaBrowser = true,
            }
        };
}