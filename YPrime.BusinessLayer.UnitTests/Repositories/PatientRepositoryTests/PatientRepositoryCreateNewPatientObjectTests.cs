using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryCreateNewPatientObjectTests : PatientRepositoryTestBase
    {
        [TestMethod]
        public async Task CreateNewPatientObjectTest()
        {
            var testLatestVersionId = Guid.NewGuid();

            var latestVersionPassedInSiteIds = new List<Guid>();
            var latestVersionPassedInCountryIds = new List<Guid>();

            LettersOnlyConfigDetail.Countries = new List<CountryBaseModel> { UnitedStatesCountry };
            DateFormatConfigDetail.Countries = new List<CountryBaseModel> { UnitedStatesCountry };
            NumberAttributeConfigDetail.Countries = new List<CountryBaseModel> { new CountryBaseModel { Id = Guid.NewGuid() } };

            MockSoftwareReleaseRepository
                .Setup(r => r.FindLatestConfigurationVersion(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<Guid>>()))
                .ReturnsAsync(testLatestVersionId)
                .Callback((List<Guid> passedInSiteIds, List<Guid> passedInCountryIds) =>
                {
                    latestVersionPassedInSiteIds.AddRange(passedInSiteIds);
                    latestVersionPassedInCountryIds.AddRange(passedInCountryIds);
                });

            var result = await Repository.CreateNewPatientObject(BaseSite.Id);

            Assert.AreEqual(BaseSite.Id, result.SiteId);

            Assert.AreEqual(2, result.SubjectInformations.Count);
            Assert.IsTrue(result.SubjectInformations.Contains(LettersOnlyConfigDetail));
            Assert.IsTrue(result.SubjectInformations.Contains(DateFormatConfigDetail));
            Assert.IsFalse(result.SubjectInformations.Contains(NumberAttributeConfigDetail));

            Assert.AreEqual(2, result.PatientAttributes.Count);
            Assert.IsTrue(result.PatientAttributes.Any(pa => pa.PatientAttributeConfigurationDetailId == LettersOnlyConfigDetail.Id));
            Assert.IsTrue(result.PatientAttributes.Any(pa => pa.PatientAttributeConfigurationDetailId == DateFormatConfigDetail.Id));
            Assert.IsFalse(result.PatientAttributes.Any(pa => pa.PatientAttributeConfigurationDetailId == NumberAttributeConfigDetail.Id));

            Assert.AreEqual(1, latestVersionPassedInSiteIds.Count);
            Assert.AreEqual(BaseSite.Id, latestVersionPassedInSiteIds.First());
            Assert.AreEqual(1, latestVersionPassedInCountryIds.Count);
            Assert.AreEqual(BaseSite.CountryId, latestVersionPassedInCountryIds.First());
        }
    }
}
