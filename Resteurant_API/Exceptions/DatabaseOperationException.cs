using System;

namespace Resteurant_API.Exceptions
{
    public class DatabaseOperationException : Exception
    {
        public DatabaseOperationException(string message) : base(message) {}
    }
}
