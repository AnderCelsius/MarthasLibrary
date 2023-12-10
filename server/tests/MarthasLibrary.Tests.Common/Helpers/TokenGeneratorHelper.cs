using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MarthasLibrary.Tests.Common.Helpers;

public static class TokenGeneratorHelper
{
    private const string TestUserIdentityId = "f0611528-36f2-4a0e-80ce-96dea6ebd13f";
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
    
    public static string GetMockJwtToken(IEnumerable<Claim>? additionalClaims)
    {
        ArgumentNullException.ThrowIfNull(additionalClaims);
        var key = Convert.ToBase64String("P+kBMFC5yqp9GUNTUnc8SWzasdr25MdtByfxhNWieIs="u8.ToArray());
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, TestUserIdentityId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        claims.AddRange(additionalClaims);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials,
            Issuer = "https://mocked-issuer.com",
            Audience = "marthaslibraryapi",
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public static string GetMockJwtToken()
    {
        // Base64 string that represents a 256-bit key
        var key = Convert.ToBase64String("P+kBMFC5yqp9GUNTUnc8SWzasdr25MdtByfxhNWieIs="u8.ToArray());

        // Symmetric security key
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(key));



        // Signing credentials
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims for the token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, TestUserIdentityId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", "User"),
            // Add other claims as needed for your testing purposes
        };

        // Token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
            SigningCredentials = credentials,
            Issuer = "https://mocked-issuer.com",
            Audience = "marthaslibraryapi",
        };

        // Token handler
        var tokenHandler = new JwtSecurityTokenHandler();

        // Create token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return serialized token
        return tokenHandler.WriteToken(token);
    }

}