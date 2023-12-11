using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MarthasLibrary.BlazorApp.Services;

public class AuthenticationService(IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration)
{

    public async Task Logout()
    {
        var client = httpClientFactory.CreateClient("IDPClient");

        var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();
        if (discoveryDocumentResponse.IsError)
        {
            throw new Exception(discoveryDocumentResponse.Error);
        }

        if (httpContextAccessor.HttpContext != null)
        {
            var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var refreshToken = await httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            if (discoveryDocumentResponse.RevocationEndpoint != null)
            {
                if (accessToken != null)
                    await RevokeToken(discoveryDocumentResponse.RevocationEndpoint, accessToken, configuration["DuendeISP:ClientId"],
                        configuration["DuendeISP:Secret"]);
                if (refreshToken != null)
                    await RevokeToken(discoveryDocumentResponse.RevocationEndpoint, refreshToken, configuration["DuendeISP:ClientId"],
                        configuration["DuendeISP:Secret"]);
            }
        }

        if (httpContextAccessor.HttpContext != null)
        {
            await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await httpContextAccessor.HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }

    private async Task RevokeToken(string revocationEndpoint, string token, string? clientId, string? clientSecret)
    {
        var client = httpClientFactory.CreateClient("IDPClient");
        if (clientId != null)
        {
            var response = await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = revocationEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Token = token
            });

            if (response.IsError)
            {
                throw new Exception(response.Error);
            }
        }
    }
}