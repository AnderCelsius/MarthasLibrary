using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MarthasLibrary.Tests.Common.Helpers;

public static class TokenGeneratorHelper
{
    public static string GenerateJwtToken(List<Claim> claims)
    {
        // Set the key material to sign the token
        var securityKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

        // Create signing credentials
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create the JWT token
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: signingCredentials
        );

        // Serialize the JWT token to a string
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}