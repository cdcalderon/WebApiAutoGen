using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Repositories;

namespace YPrime.BusinessLayer.UnitTests.Repositories.TimeZoneRepositoryTests
{
    [TestClass]
    public class TimeZoneRepositoryTests
    {
        private TimeZoneRepository repository;

        [TestInitialize]
        public void Initialize()
        {
            repository = new TimeZoneRepository(null);
        }

        [TestMethod]
        [Ignore]
        public async Task SpecificIPAddress()
        {
            // Test that the IP Address: 50.235.211.118 returns America/Denver
            string expectedTimeZone = "America/New_York";
            string IPAddress = "50.235.211.118";
            string TimeZoneFound = await repository.GetTimeZoneId(IPAddress);
            Assert.AreEqual(expectedTimeZone, TimeZoneFound);
        }

        [TestMethod]
        public async Task BadIPAddressWithDefault()
        {
            // Test that the IP Address: 257.235.211.118 returns the default time zone
            string defaultTimeZone = "America/New York";
            string IPAddress = "257.235.211.118";
            string TimeZoneFound = await repository.GetTimeZoneIdWithDefault(IPAddress, defaultTimeZone);
            Assert.AreEqual(defaultTimeZone, TimeZoneFound);
        }

        [TestMethod]
        public async Task BadIPAddress()
        {
            string IPAddress = "::1";
            string TimeZoneFound = await repository.GetTimeZoneId(IPAddress);
            Assert.AreEqual(TimeZoneFound, string.Empty);
        }
    }
}