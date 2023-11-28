namespace MarthasLibrary.API.ErrorHandling;

public class NotFoundException : Exception
{
  public NotFoundException(string? message)
    : base(message)
  {
  }
}
