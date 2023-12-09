using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace MarthasLibrary.Tests.Common.Helpers;

public class ConfigurationAndOptions
{
    public static WebApplicationFactoryClientOptions DefaultOptions = new()
    {
        AllowAutoRedirect = false
    };

    public static IConfigurationRoot GetConfiguration()
    {
        var defaultConfiguration = new Dictionary<string, string>
        {
            ["AppSettings:WebUrl"] = "https://localhost:7241/",
            ["AppSettings:AppName"] = "Marthas-Library-Test",
            ["AppSettings:AllowedHeaders"] = "Content-Type, Accept, Authorization",
            ["AppSettings:AllowedMethods"] = "GET, POST, PUT, PATCH, DELETE",
            ["AppSettings:AllowedOrigins"] = "http://localhost:3008, https://rapha-identity-server-web.azurewebsites.net",
            ["DuendeISP:EncryptionKey"] = "yu9CPEwAvmJ6z2yxvBHioXd55lPG5SOc",
            ["DuendeISP:Issuer"] = "https://localhost:7241",
            ["DuendeISP:RequiredClaims"] = "",
            ["DuendeISP:Audiences"] = "marthaslibraryapi",
            ["DuendeISP:RefreshTokenLifeSpan"] = "20",
            ["DuendeISP:AccessTokenLifeSpan"] = "5",
            ["ConnectionStrings:DefaultConnection"] = "Data Source=.;Initial Catalog=MarthasLibrary.DB;Integrated Security=True; TrustServerCertificate=True"
            // ... Any necessary configuration
        };

        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(defaultConfiguration);
        return configBuilder.Build();
    }
}