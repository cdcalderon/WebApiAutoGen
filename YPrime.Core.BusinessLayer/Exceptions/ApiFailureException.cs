using System;
namespace YPrime.Core.BusinessLayer.Exceptions
{
    public class ApiFailureException : Exception
    {
        public ApiFailureException(string endpoint, string statusCode, string message)
            : base($"{endpoint} - Status code : {statusCode} - {message}")
        { }
    }
}
