using System;
namespace YPrime.Core.BusinessLayer.Exceptions
{
    public class StudyAccessDeactivatedException : Exception
    {
        private const string ExceptionMessage = "Your access to the study has been deactivated, please contact your system administrator for details.";

        public StudyAccessDeactivatedException()
            : base(ExceptionMessage)
        { }
    }
}
