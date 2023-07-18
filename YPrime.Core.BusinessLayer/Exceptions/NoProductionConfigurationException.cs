using System;

namespace YPrime.Core.BusinessLayer.Exceptions
{
    public class NoProductionConfigurationException : Exception
    {
        private const string ExceptionMessage = "No configurations have been approved for production.";

        public NoProductionConfigurationException()
            : base(ExceptionMessage)
        { }
    }
}
