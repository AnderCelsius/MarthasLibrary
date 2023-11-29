namespace MarthasLibrary.API.Extensions;

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
