using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SiteRepositoryTests
{
    [TestClass]
    public class GetLanguagesForSiteTests : SiteRepositoryTestBase
    {
        [TestMethod]
        public async Task GetLanguagesForSite_AllTest()
        {
            var configId = Guid.NewGuid();

            var result = await repository.GetLanguagesForSite(
                BaseSiteId,
                configId);

            Assert.AreEqual(Languages.Count, result.Count());
            
            foreach(var language in result)
            {
                Assert.IsTrue(Languages.Any(l => l.Id == language.Id));
            }

            VerifyGetAllLanguagesCall(configId);
        }

        [TestMethod]
        public async Task GetLanguagesForSite_SingleSiteTest()
        {
            var configId = Guid.NewGuid();

            var result = await repository.GetLanguagesForSite(
                JapaneseSiteId,
                configId);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(Japanese.Id, result.First().Id);

            VerifyGetAllLanguagesCall(configId);
        }

        private void VerifyGetAllLanguagesCall(
            Guid configId)
        {
            MockLanguageService
                .Verify(
                    s => s.GetAll(It.Is<Guid?>(cid => cid == configId)),
                    Times.Once);
        }
    }
}
