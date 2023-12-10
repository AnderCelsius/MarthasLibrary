using System.Security.Cryptography;
using System.Text;

namespace MarthasLibrary.Common.Authentication;

public class PkceHelper
{
  public static string CreateCodeVerifier()
  {
    var bytes = new byte[32];
    RandomNumberGenerator.Fill(bytes);
    return Convert.ToBase64String(bytes)
      .TrimEnd('=')
      .Replace('+', '-')
      .Replace('/', '_');
  }

  public static string CreateCodeChallenge(string codeVerifier)
  {
    using var sha256 = SHA256.Create();
    var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
    return Convert.ToBase64String(challengeBytes)
      .TrimEnd('=')
      .Replace('+', '-')
      .Replace('/', '_');
  }

  public static string GenerateSecureSecret()
  {
    var randomNumber = new byte[32]; // 256 bits
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
  }
}

