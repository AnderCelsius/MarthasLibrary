namespace MarthasLibrary.API.ErrorHandling;

public class UnauthorizedException : Exception
{
  public UnauthorizedException(string? message)
    : base(message)
  {
  }
}
