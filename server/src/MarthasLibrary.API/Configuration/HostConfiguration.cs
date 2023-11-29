namespace MarthasLibrary.API.Configuration;

public record HostConfiguration
{
  public string Host { get; init; } = default!;
}
