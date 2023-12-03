using Microsoft.Extensions.Hosting;

namespace MarthasLibrary.Common.Extensions;

public static class HostEnvironmentExtensions
{
    public static bool IsTesting(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment("Testing");
    }

    public static bool IsSwaggerGen(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment("SwaggerGen");
    }
}