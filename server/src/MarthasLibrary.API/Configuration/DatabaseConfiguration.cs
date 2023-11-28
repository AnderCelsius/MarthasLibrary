namespace MarthasLibrary.API.Configuration;

public record DatabaseConfiguration
{
  public string? ConnectionString { get; init; }
}
