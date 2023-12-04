namespace MarthasLibrary.API.Features.Exceptions
{
    [Serializable]
    public class BookWithIsbnAlreadyExistsException : Exception
    {
        public BookWithIsbnAlreadyExistsException()
        {
        }

        public BookWithIsbnAlreadyExistsException(string? message) : base(message)
        {
        }

        public BookWithIsbnAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}