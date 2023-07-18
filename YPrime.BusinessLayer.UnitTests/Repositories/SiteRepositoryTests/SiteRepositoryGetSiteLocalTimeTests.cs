using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.BusinessLayer.UnitTests.TestExtensions;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SiteRepositoryTests
{
    [TestClass]
    public class SiteRepositoryGetSiteLocalTimeTests : SiteRepositoryTestBase
    {
        [TestMethod]
        public void SiteRepositoryGetSiteLocalTimeTest()
        {
            var testSite = BaseSites.First();

            var result = repository.GetSiteLocalTime(testSite.Id);

            var expectedDateTime = DateTimeOffset.UtcNow;

            Assert.That.AreCloseInSeconds(expectedDateTime, result.UtcDateTime, 5);
        }
    }
}