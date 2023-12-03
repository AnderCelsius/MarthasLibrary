using System.Runtime.Serialization;

namespace MarthasLibrary.API.Features.Exceptions
{
    [Serializable]
    public class ConcurrencyConflictException : Exception
    {
        public ConcurrencyConflictException()
        {
        }

        public ConcurrencyConflictException(string? message) : base(message)
        {
        }

        public ConcurrencyConflictException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ConcurrencyConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}