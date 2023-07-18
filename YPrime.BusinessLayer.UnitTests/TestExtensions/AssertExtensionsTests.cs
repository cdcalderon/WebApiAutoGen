using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YPrime.BusinessLayer.UnitTests.TestExtensions
{
    [TestClass]
    public class AssertExtensionsTests
    {
        private const int ExpectedThreshold = 5;

        [TestMethod]
        public void AssertExtensionsAreCloseInSecondsPositiveTest()
        {
            var expected = DateTimeOffset.UtcNow;
            var actual = DateTimeOffset.UtcNow.AddSeconds(ExpectedThreshold - 1);

            Assert.That.AreCloseInSeconds(expected, actual, ExpectedThreshold);
        }

        [TestMethod]
        public void AssertExtensionsAreCloseInSecondsReversedPositiveTest()
        {
            var actual = DateTimeOffset.UtcNow;
            var expected = DateTimeOffset.UtcNow.AddSeconds(ExpectedThreshold - 1);

            Assert.That.AreCloseInSeconds(expected, actual, ExpectedThreshold);
        }

        [TestMethod]
        public void AssertExtensionsAreCloseInSecondsNullDatesTest()
        {
            Assert.That.AreCloseInSeconds(null, null, ExpectedThreshold);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException), "Actual date is null.")]
        public void AssertExtensionsAreCloseInSecondsNullActualTest()
        {
            var expected = DateTimeOffset.UtcNow;

            Assert.That.AreCloseInSeconds(expected, null, ExpectedThreshold);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException), "Expected date is null")]
        public void AssertExtensionsAreCloseInSecondsNullExpectedTest()
        {
            var actual = DateTimeOffset.UtcNow;

            Assert.That.AreCloseInSeconds(null, actual, ExpectedThreshold);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException), "The number of seconds exceeds the expected threshold.")]
        public void AssertExtensionsAreCloseInSecondsNegativeTest()
        {
            var expected = DateTimeOffset.UtcNow;
            var actual = DateTimeOffset.UtcNow.AddSeconds(ExpectedThreshold * 2);

            Assert.That.AreCloseInSeconds(expected, actual, ExpectedThreshold);
        }
    }
}