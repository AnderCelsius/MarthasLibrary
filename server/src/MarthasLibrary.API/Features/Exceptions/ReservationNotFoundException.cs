﻿namespace MarthasLibrary.API.Features.Exceptions
{
    [Serializable]
    public class ReservationNotFoundException : Exception
    {
        public ReservationNotFoundException()
        {
        }

        public ReservationNotFoundException(string? message) : base(message)
        {
        }

        public ReservationNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}