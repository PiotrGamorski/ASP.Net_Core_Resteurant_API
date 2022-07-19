using System;

namespace Resteurant_API.Exceptions
{
    public class NotUniqueItemException : Exception
    {
        public NotUniqueItemException(string message) : base(message) {}
    }
}
