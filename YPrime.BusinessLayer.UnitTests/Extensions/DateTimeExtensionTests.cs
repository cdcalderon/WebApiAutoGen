using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using YPrime.BusinessLayer.Extensions;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class DateTimeExtensionTests
    {
        [TestMethod]
        public void DateTimeExtensionConvertToTimeZoneDefaultLocalNotTimeZoneTest()
        {
            var date = DateTimeOffset.Now.ConvertToTimeZone(string.Empty);
            var expectedDate = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimeZoneInfo.Local);

            Assert.AreEqual(expectedDate.Date, date.Date);
        }

        [TestMethod]
        public void DateTimeExtensionConvertToTimeZoneDefaultLocalWithIncorrectTimeZoneTest()
        {
            var testTimeZone = "test time zone";
            var date = DateTimeOffset.Now.ConvertToTimeZone(testTimeZone);
            var expectedDate = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimeZoneInfo.Local);

            Assert.AreEqual(expectedDate.Date, date.Date);
        }

        [TestMethod]
        public void DateTimeExtensionConvertToTimeZoneWithTimeZoneTest()
        {
            var timeZone = "Tokyo Standard Time";
            var date = DateTimeOffset.UtcNow.ConvertToTimeZone(timeZone);
            var timeZoneOffset = 9;

            var expectedDate = DateTimeOffset.UtcNow.AddHours(timeZoneOffset);

            Assert.AreEqual(expectedDate.DateTime.ToString(), date.DateTime.ToString());
        }
    }
}
