using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YPrime.BusinessLayer.UnitTests
{
    public static class YAssert
    {
        public static void DoesNotThrow<TException>(Action method)
            where TException : Exception
        {
            try
            {
                method();
            }
            catch (TException ex)
            {
                Assert.IsNull(ex);
            }
        }

        public static void DoesThrow<TException>(Action method)
            where TException : Exception
        {
            try
            {
                method();
            }
            catch (TException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(TException));
                return;
            }

            Assert.Fail($"No {typeof(TException)} was thrown.");
        }

        public static void ThrowsSameException<TException>(TException exception, Action method)
            where TException : Exception
        {
            try
            {
                method();
            }
            catch (TException ex)
            {
                Assert.AreEqual(exception, ex);
                return;
            }

            Assert.Fail($"No {exception.GetType()} was thrown.");
        }

        public static void IsType<T>(object value)
        {
            Assert.IsInstanceOfType(value, typeof(T));
        }
    }
}