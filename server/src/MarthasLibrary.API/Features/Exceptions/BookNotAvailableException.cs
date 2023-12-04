namespace MarthasLibrary.API.Features.Exceptions;

[Serializable]
public class BookNotAvailableException : Exception
{
    public BookNotAvailableException()
    {
    }

    public BookNotAvailableException(string? message) : base(message)
    {
    }

    public BookNotAvailableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}