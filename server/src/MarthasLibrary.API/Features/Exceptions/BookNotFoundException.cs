﻿namespace MarthasLibrary.API.Features.Exceptions
{
    [Serializable]
    public class BookNotFoundException : Exception
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
    }
}