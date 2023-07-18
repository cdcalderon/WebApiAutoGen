namespace YPrime.BusinessLayer.Exceptions
{
    public class AlarmNotFoundException : BusinessException
    {
        private const string ExceptionMessage = "Alarm not found";

        public override string Message => ExceptionMessage;
    }
}