using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositoryGetAvailableCorrectionPatientsTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task CorrectionRepositoryGetAvailableCorrectionPatientsAllSitesTest()
        {
            var firstTestSiteDto = new SiteDto
            {
                Id = Guid.NewGuid()
            };

            var secondTestSiteDto = new SiteDto
            {
                Id = Guid.NewGuid()
            };

            var testPatient = new PatientDto
            {
                Id = Guid.NewGuid()
            };

            var testPatientList = new List<PatientDto>
            {
                testPatient
            };

            var testPatientListQueryable = testPatientList
                .OrderBy(p => p.Id);

            var methodPassedInSiteIds = new List<Guid>();
            var methodPassedInIsActive = (bool?) null;

            MockPatientRepository
                .Setup(r => r.GetAllPatients(It.IsAny<List<Guid>>(), It.IsAny<bool?>()))
                .ReturnsAsync(testPatientListQueryable)
                .Callback<IEnumerable<Guid>, bool?>((passedInSiteIds, passedInIsActive) =>
                {
                    methodPassedInSiteIds = passedInSiteIds.ToList();
                    methodPassedInIsActive = passedInIsActive;
                });

            var testUser = new StudyUserDto()
            {
                Sites = new List<SiteDto>
                {
                    firstTestSiteDto,
                    secondTestSiteDto
                }
            };

            var result = await Repository.GetAvailableCorrectionPatients(Guid.Empty, testUser);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(testPatient, result.First());
            Assert.IsNull(methodPassedInIsActive);
            Assert.AreEqual(2, methodPassedInSiteIds.Count);
            Assert.IsTrue(methodPassedInSiteIds.Contains(firstTestSiteDto.Id));
            Assert.IsTrue(methodPassedInSiteIds.Contains(secondTestSiteDto.Id));
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetAvailableCorrectionPatientsSingleSiteTest()
        {
            var firstTestSiteDto = new SiteDto
            {
                Id = Guid.NewGuid()
            };

            var secondTestSiteDto = new SiteDto
            {
                Id = Guid.NewGuid()
            };

            var testPatient = new PatientDto
            {
                Id = Guid.NewGuid()
            };

            var testPatientList = new List<PatientDto>
            {
                testPatient
            };

            var testPatientListQueryable = testPatientList
                .OrderBy(p => p.Id);

            var methodPassedInSiteIds = new List<Guid>();
            var methodPassedInIsActive = (bool?) null;

            MockPatientRepository
                .Setup(r => r.GetAllPatients(It.IsAny<List<Guid>>(), It.IsAny<bool?>()))
                .ReturnsAsync(testPatientListQueryable)
                .Callback<IEnumerable<Guid>, bool?>((passedInSiteIds, passedInIsActive) =>
                {
                    methodPassedInSiteIds = passedInSiteIds.ToList();
                    methodPassedInIsActive = passedInIsActive;
                });

            var testUser = new StudyUserDto()
            {
                Sites = new List<SiteDto>
                {
                    firstTestSiteDto,
                    secondTestSiteDto
                }
            };

            var result = await Repository.GetAvailableCorrectionPatients(firstTestSiteDto.Id, testUser);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(testPatient, result.First());
            Assert.IsNull(methodPassedInIsActive);
            Assert.AreEqual(1, methodPassedInSiteIds.Count);
            Assert.IsTrue(methodPassedInSiteIds.Contains(firstTestSiteDto.Id));
            Assert.IsFalse(methodPassedInSiteIds.Contains(secondTestSiteDto.Id));
        }
    }
}