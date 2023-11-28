namespace MarthasLibrary.API.ErrorHandling;

public class ForbiddenException : Exception
{
  public ForbiddenException(string? message = null)
    : base(message)
  {
  }
}
