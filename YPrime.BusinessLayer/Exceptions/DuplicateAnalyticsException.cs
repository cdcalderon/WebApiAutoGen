namespace YPrime.BusinessLayer.Exceptions
{
    public class DuplicateAnalyticsException : BusinessException
    {
        private const string ExceptionMessage = "Duplicate analytics";

        public override string Message => ExceptionMessage;
    }
}