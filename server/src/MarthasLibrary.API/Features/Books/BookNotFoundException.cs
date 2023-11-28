using System.Runtime.Serialization;

namespace MarthasLibrary.API.Features.Books
{
  [Serializable]
  internal class BookNotFoundException : Exception
  {
    public BookNotFoundException()
    {
    }

    public BookNotFoundException(string? message) : base(message)
    {
    }

    public BookNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BookNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}