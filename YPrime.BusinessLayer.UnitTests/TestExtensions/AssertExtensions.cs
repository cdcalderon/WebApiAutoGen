using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YPrime.BusinessLayer.UnitTests.TestExtensions
{
    public static class AssertExtensions
    {
        private const string NullActualDateMessage = "Actual date is null.";
        private const string NullExpectedDateMessage = "Expected date is null";

        private const string NumberOfSecondsOverThresholdMessage =
            "The number of seconds exceeds the expected threshold.";

        public static void AreCloseInSeconds(
            this Assert assert,
            DateTimeOffset? expectedDate,
            DateTimeOffset? actualDate,
            int numberOfSecondsThreshold)
        {
            if (!expectedDate.HasValue && !actualDate.HasValue)
            {
                return;
            }

            if (!expectedDate.HasValue)
            {
                throw new AssertFailedException(NullExpectedDateMessage);
            }

            if (!actualDate.HasValue)
            {
                throw new AssertFailedException(NullActualDateMessage);
            }

            var timeDifference = expectedDate.Value - actualDate.Value;
            var totalSeconds = timeDifference.TotalSeconds;

            if (totalSeconds < 0)
            {
                totalSeconds *= -1;
            }

            if (totalSeconds > numberOfSecondsThreshold)
            {
                throw new AssertFailedException(NumberOfSecondsOverThresholdMessage);
            }
        }
    }
}