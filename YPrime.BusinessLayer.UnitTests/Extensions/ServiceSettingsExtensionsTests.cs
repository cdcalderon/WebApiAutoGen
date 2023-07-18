using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class ServiceSettingsExtensionsTests
    {
        [TestMethod]
        public void IsProductionEnvironmentTrueTest()
        {
            var settings = new ServiceSettings
            {
                StudyPortalAppEnvironment = "production"
            };

            var result = settings.IsProductionEnvironment();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsProductionEnvironmentFalseTest()
        {
            var settings = new ServiceSettings
            {
                StudyPortalAppEnvironment = "DEV"
            };

            var result = settings.IsProductionEnvironment();

            Assert.IsFalse(result);
        }
    }
}
