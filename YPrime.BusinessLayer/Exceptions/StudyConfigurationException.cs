using System;

namespace YPrime.BusinessLayer.Exceptions
{
    public class StudyConfigurationException : BusinessException
    {
        public StudyConfigurationException()
        {
        }

        public StudyConfigurationException(string message) : base(message)
        {
        }
    }
}