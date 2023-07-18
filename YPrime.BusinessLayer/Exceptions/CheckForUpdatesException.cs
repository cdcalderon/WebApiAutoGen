using System;

namespace YPrime.BusinessLayer.Exceptions
{
    public class CheckForUpdatesException : Exception
    {
        private const string ExceptionMessage = "Error occured when checking for update";

        public override string Message => ExceptionMessage;
    }
}
