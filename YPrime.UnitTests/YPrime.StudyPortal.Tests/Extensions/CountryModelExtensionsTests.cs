using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Extensions
{
    [TestClass]
    public class CountryModelExtensionsTests
    {
        [TestMethod]
        public void CountryGetTimeFormatTest()
        {
            var use24HourCountry = new CountryModel()
            {
                Use12HourTime = false
            };

            var use12HourCountry = new CountryModel()
            {
                Use12HourTime = true
            };

            Assert.AreEqual(use24HourCountry.GetTimeFormat(), "HH:mm");
            Assert.AreEqual(use12HourCountry.GetTimeFormat(), "hh:mm A");
        }
    }
}
