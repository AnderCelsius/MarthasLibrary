namespace MarthasLibrary.API.Features.Exceptions;

public class BorrowNotFoundException : Exception
{
    public BorrowNotFoundException()
    {
    }

    public BorrowNotFoundException(string? message) : base(message)
    {
    }

    public BorrowNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}