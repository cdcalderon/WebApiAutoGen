using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SiteRepositoryTests
{
    [TestClass]
    public class SiteRepositoryGetSitesTests : LegacySiteTestSetup
    {
       
        [TestMethod]
        public async Task GetAllSitesTest()
        {
            var siteRepository = new SiteRepository(_dbContext.Object, null, new Mock<ILanguageService>().Object,
                new Mock<IStudyRoleService>().Object, new Mock<IPatientStatusService>().Object, _countryService.Object,
                 MockStudySettingService.Object);
            var allSites = await siteRepository.GetAllSites();

            var a = allSites.Select(x => new {x.Id, x.Name, x.IsActive}).ToList();

            Assert.AreEqual(mainSiteGuid, a.Find(x => x.Id == mainSiteGuid).Id);
            Assert.AreNotSame(secondarySiteGuid, a.Find(x => x.Id == secondarySiteGuid).Id);
        }

        [TestMethod]
        public void CheckSiteNumberIsUsedTest()
        {
            var siteRepository = new SiteRepository(_dbContext.Object, null, new Mock<ILanguageService>().Object,
                new Mock<IStudyRoleService>().Object, new Mock<IPatientStatusService>().Object, new Mock<ICountryService>().Object,
                MockStudySettingService.Object);
            var checkSite = siteRepository.CheckSiteNumberIsUsed("1");
            var checkSite2 = siteRepository.CheckSiteNumberIsUsed("3");

            Assert.IsTrue(checkSite);
            Assert.IsFalse(checkSite2);
        }
    }
}