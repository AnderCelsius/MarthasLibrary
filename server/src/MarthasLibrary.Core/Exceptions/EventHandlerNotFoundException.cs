namespace MarthasLibrary.Core.Exceptions;

public class EventHandlerNotFoundException : Exception
{
  public EventHandlerNotFoundException(string message) : base(message)
  {
  }
}
