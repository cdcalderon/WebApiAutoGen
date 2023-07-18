using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryGetAssignedConfigurationTests : PatientRepositoryTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            BasePatients.Clear();
            BaseSoftwareReleases.Clear();
            BaseSoftwareReleaseCountries.Clear();
            BaseDevices.Clear();
        }

        [TestMethod]
        public async Task GetAssignedConfiguration_WithDeviceTest()
        {
            var oldestRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "1.0",
                StudyWide = true,
                IsActive = true
            };

            BaseSoftwareReleases.Add(oldestRelease);

            var middleRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "2.0",
                StudyWide = false,
                IsActive = true,
                Sites = new List<Site>
                {
                    BaseSite
                }
            };

            BaseSoftwareReleases.Add(middleRelease);

            var newestRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "3.0",
                StudyWide = false,
                IsActive = true
            };

            BaseSoftwareReleases.Add(newestRelease);

            var softwareReleaseCountry = new SoftwareReleaseCountry
            {
                CountryId = UnitedStatesCountry.Id,
                SoftwareReleaseId = newestRelease.Id,
                SoftwareRelease = newestRelease
            };

            BaseSoftwareReleaseCountries.Add(softwareReleaseCountry);

            var testPatient = new Patient
            {
                Id = BasePatientId,
                SiteId = BaseSite.Id,
                Site = BaseSite
            };

            BasePatients.Add(testPatient);

            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                SoftwareReleaseId = middleRelease.Id,
                SoftwareRelease = middleRelease
            };

            BaseDevices.Add(testDevice);

            MockSoftwareReleaseRepository.Setup(s => s.FindLatestConfigurationVersion(It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()))
                .ReturnsAsync(oldestRelease.ConfigurationId);

            var result = await Repository.GetAssignedConfiguration(testPatient.Id);

            Assert.AreEqual(middleRelease.ConfigurationId, result);
        }

        [TestMethod]
        public async Task GetAssignedConfiguration_NoDeviceReturnsExistingReleaseForPatientTest()
        {
            var testPatient = new Patient
            {
                Id = BasePatientId,
                SiteId = BaseSite.Id,
                Site = BaseSite
            };

            BasePatients.Add(testPatient);

            var newestRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "3.0",
                StudyWide = true,
                IsActive = true
            };
            
            BaseSoftwareReleases.Add(newestRelease);


            MockSoftwareReleaseRepository.Setup(s => s.FindLatestConfigurationVersion(It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()))
                .ReturnsAsync(newestRelease.ConfigurationId);

            var result = await Repository.GetAssignedConfiguration(testPatient.Id);

            Assert.AreEqual(newestRelease.ConfigurationId, result);
        }

        [TestMethod]
        public async Task GetAssignedConfiguration_NoConfigAvailableTest()
        {
            var testPatient = new Patient
            {
                Id = BasePatientId,
                SiteId = BaseSite.Id,
                Site = BaseSite
            };

            BasePatients.Add(testPatient);

            var onlyRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "3.0",
                StudyWide = false,
                IsActive = false
            };

            BaseSoftwareReleases.Add(onlyRelease);

            var softwareReleaseCountry = new SoftwareReleaseCountry
            {
                CountryId = UnitedStatesCountry.Id,
                SoftwareReleaseId = onlyRelease.Id,
                SoftwareRelease = onlyRelease
            };

            BaseSoftwareReleaseCountries.Add(softwareReleaseCountry);

            MockSoftwareReleaseRepository.Setup(s => s.FindLatestConfigurationVersion(It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()))
                .ReturnsAsync(Guid.Empty);

            var result = await Repository.GetAssignedConfiguration(testPatient.Id);

            Assert.AreEqual(Guid.Empty, result);
        }

        [TestMethod]
        public async Task GetAssignedConfiguration_PatientNotFoundTest()
        {
            var testPatient = new Patient
            {
                Id = BasePatientId,
                SiteId = BaseSite.Id,
                Site = BaseSite
            };

            BasePatients.Add(testPatient);

            var onlyRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "3.0",
                StudyWide = false,
                IsActive = false
            };

            BaseSoftwareReleases.Add(onlyRelease);

            var softwareReleaseCountry = new SoftwareReleaseCountry
            {
                CountryId = UnitedStatesCountry.Id,
                SoftwareReleaseId = onlyRelease.Id,
                SoftwareRelease = onlyRelease
            };

            BaseSoftwareReleaseCountries.Add(softwareReleaseCountry);

            MockSoftwareReleaseRepository.Setup(s => s.FindLatestConfigurationVersion(It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()))
                .ReturnsAsync(Guid.Empty);

            var result = await Repository.GetAssignedConfiguration(Guid.NewGuid());

            Assert.AreEqual(Guid.Empty, result);
        }
    }
}
