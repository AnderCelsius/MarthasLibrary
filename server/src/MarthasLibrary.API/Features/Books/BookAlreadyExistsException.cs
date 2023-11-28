namespace MarthasLibrary.API.Features.Books;

public class BookAlreadyExistsException : Exception
{
  public BookAlreadyExistsException(string message) : base(message)
  {
  }
}