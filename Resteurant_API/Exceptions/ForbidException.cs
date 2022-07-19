using System;

namespace Resteurant_API.Exceptions
{
    public class ForbidException : Exception
    {
        public ForbidException()
        {
        }

        public ForbidException(string message) : base(message) {}
    }
}
