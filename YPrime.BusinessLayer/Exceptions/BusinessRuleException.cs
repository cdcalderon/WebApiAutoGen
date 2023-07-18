namespace YPrime.BusinessLayer.Exceptions
{
    public class BusinessRuleException : BusinessException
    {
        private const string ExceptionMessage = "Business Rule Execution Failed";

        public override string Message => ExceptionMessage;
    }
}
