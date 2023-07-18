using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryFindLatestConfigVersionTests : SoftwareReleaseRepositoryTestBase
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
        public async Task FindLatestConfiguration_SortingTest()
        {
            var oldestRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "9.0",
                StudyWide = true,
                IsActive = true
            };

            BaseSoftwareReleases.Add(oldestRelease);

            var middleRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "30.0",
                StudyWide = true,
                IsActive = true,
            };

            BaseSoftwareReleases.Add(middleRelease);

            var newestRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "40.0",
                StudyWide = true,
                IsActive = true
            };

            BaseSoftwareReleases.Add(newestRelease);

            var testPatient = new Patient
            {
                Id = BasePatientId,
                SiteId = BaseSite.Id,
                Site = BaseSite
            };

            BasePatients.Add(testPatient);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid> { testPatient.SiteId }, new List<Guid> { testPatient.Site.CountryId });

            Assert.AreEqual(newestRelease.ConfigurationId, result);
        }

        [TestMethod]
        public async Task FindLatestConfiguration_WithDeviceTypeTabletTest()
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

            var ReleaseByDeviceType = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "4.0",
                StudyWide = false,
                IsActive = true
            };

            BaseSoftwareReleases.Add(ReleaseByDeviceType);

            var softwareReleaseCountry = new SoftwareReleaseCountry
            {
                CountryId = UnitedStatesCountry.Id,
                SoftwareReleaseId = newestRelease.Id,
                SoftwareRelease = newestRelease
            };

            BaseSoftwareReleaseCountries.Add(softwareReleaseCountry);

            var softwareReleaseDeviceType = new SoftwareReleaseDeviceType
            {
                DeviceTypeId = Config.Enums.DeviceType.Tablet.Id,
                SoftwareReleaseId = ReleaseByDeviceType.Id,
                SoftwareRelease = ReleaseByDeviceType
            };

            BaseSoftwareReleaseDeviceTypes.Add(softwareReleaseDeviceType);

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
                PatientId = null,
                SoftwareReleaseId = middleRelease.Id,
                SoftwareRelease = middleRelease,
                DeviceTypeId = Config.Enums.DeviceType.Tablet.Id,
            };

            BaseDevices.Add(testDevice);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid> { BaseSite.Id }, new List<Guid> { BaseSite.CountryId } );

            Assert.AreEqual(ReleaseByDeviceType.ConfigurationId, result);
        }

        [TestMethod]
        public async Task FindLatestConfiguration_StudyWideTest()
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
                ConfigurationVersion = "1.0",
                StudyWide = true,
                IsActive = true
            };

            BaseSoftwareReleases.Add(onlyRelease);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid> { BaseSite.Id }, new List<Guid> { BaseSite.CountryId });

            Assert.AreEqual(onlyRelease.ConfigurationId, result);
        }

        [TestMethod]
        public async Task FindLatestConfiguration_OnlySiteTest()
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
                ConfigurationVersion = "2.0",
                StudyWide = false,
                IsActive = true,
                Sites = new List<Site>
                {
                    BaseSite
                }
            };

            BaseSoftwareReleases.Add(onlyRelease);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid> { BaseSite.Id }, new List<Guid> { BaseSite.CountryId });

            Assert.AreEqual(onlyRelease.ConfigurationId, result);
        }

        [TestMethod]
        public async Task FindLatestConfiguration_OnlyCountryTest()
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
                IsActive = true
            };

            BaseSoftwareReleases.Add(onlyRelease);

            var softwareReleaseCountry = new SoftwareReleaseCountry
            {
                CountryId = UnitedStatesCountry.Id,
                SoftwareReleaseId = onlyRelease.Id,
                SoftwareRelease = onlyRelease
            };

            BaseSoftwareReleaseCountries.Add(softwareReleaseCountry);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid> { BaseSite.Id }, new List<Guid> { BaseSite.CountryId });

            Assert.AreEqual(onlyRelease.ConfigurationId, result);
        }

        [TestMethod]
        public async Task FindLatestConfiguration_DuplicateConfigTest()
        {
            var configId = Guid.NewGuid();
            const string configVersion = "2.0";

            var releaseOne = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = true,
                IsActive = true,
            };

            BaseSoftwareReleases.Add(releaseOne);

            var releaseTwo = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true,
                Sites = new List<Site>
                {
                    BaseSite
                }
            };

            BaseSoftwareReleases.Add(releaseTwo);

            var thirdRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true
            };

            BaseSoftwareReleases.Add(thirdRelease);

            var softwareReleaseCountry = new SoftwareReleaseCountry
            {
                CountryId = UnitedStatesCountry.Id,
                SoftwareReleaseId = thirdRelease.Id,
                SoftwareRelease = thirdRelease
            };

            BaseSoftwareReleaseCountries.Add(softwareReleaseCountry);

            var testPatient = new Patient
            {
                Id = BasePatientId,
                SiteId = BaseSite.Id,
                Site = BaseSite
            };

            BasePatients.Add(testPatient);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid> { BaseSite.Id }, new List<Guid> { BaseSite.CountryId });

            Assert.AreEqual(configId, result);
        }

        [TestMethod]
        public async Task FindLatestConfiguration_NoConfigFoundTest()
        {
            var configId = Guid.NewGuid();
            const string configVersion = "2.0";

            var releaseOne = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true,
            };

            BaseSoftwareReleases.Add(releaseOne);

            var otherSite = new Site
            {
                Id = Guid.NewGuid()
            };

            var releaseTwo = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true,
                Sites = new List<Site>
                {
                    otherSite
                }
            };

            BaseSoftwareReleases.Add(releaseTwo);

            var thirdRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true
            };

            BaseSoftwareReleases.Add(thirdRelease);

            var testPatient = new Patient
            {
                Id = BasePatientId,
                SiteId = BaseSite.Id,
                Site = BaseSite
            };

            BasePatients.Add(testPatient);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid> { BaseSite.Id } , new List<Guid> { BaseSite.CountryId });

            Assert.AreEqual(Config.Defaults.ConfigurationVersions.InitialVersion.Id, result);
        }

        [TestMethod]
        public async Task FindLatestConfiguration_EmptySiteCountryListsReturnsStudyWideTest()
        {
            var configId = Guid.NewGuid();
            const string configVersion = "2.0";

            var releaseOne = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = true,
                IsActive = true,
            };

            BaseSoftwareReleases.Add(releaseOne);

            var releaseTwo = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true,
                Sites = new List<Site>
                {
                    BaseSite
                }
            };

            BaseSoftwareReleases.Add(releaseTwo);

            var thirdRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true
            };

            BaseSoftwareReleases.Add(thirdRelease);

            var testPatient = new Patient
            {
                Id = BasePatientId,
                SiteId = BaseSite.Id,
                Site = BaseSite
            };

            BasePatients.Add(testPatient);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid>(), new List<Guid> ());

            Assert.AreEqual(releaseOne.ConfigurationId, result);
        }

        [TestMethod]
        public async Task FindLatestConfiguration_EmptySiteCountryListsReturnsEmptyGuidTest()
        {
            var configId = Guid.NewGuid();
            const string configVersion = "2.0";

            var releaseOne = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true,
            };

            BaseSoftwareReleases.Add(releaseOne);

            var releaseTwo = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true,
                Sites = new List<Site>
                {
                    BaseSite
                }
            };

            BaseSoftwareReleases.Add(releaseTwo);

            var thirdRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = configId,
                ConfigurationVersion = configVersion,
                StudyWide = false,
                IsActive = true
            };

            BaseSoftwareReleases.Add(thirdRelease);

            var testPatient = new Patient
            {
                Id = BasePatientId,
                SiteId = BaseSite.Id,
                Site = BaseSite
            };

            BasePatients.Add(testPatient);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid>(), new List<Guid>());

            Assert.AreEqual(Config.Defaults.ConfigurationVersions.InitialVersion.Id, result);
        }

        [TestMethod]
        public async Task FindLatestConfiguration_OnlyInitialReleaseExists()
        {
            var initialRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                ConfigurationId = Config.Defaults.ConfigurationVersions.InitialVersion.Id,
                ConfigurationVersion = Config.Defaults.ConfigurationVersions.InitialVersion.ConfigurationVersionNumber,
                StudyWide = true,
                IsActive = true,
            };

            BaseSoftwareReleases.Add(initialRelease);

            var result = await Repository.FindLatestConfigurationVersion(new List<Guid>(), new List<Guid>());

            Assert.AreEqual(Config.Defaults.ConfigurationVersions.InitialVersion.Id, result);
        }
    }
}
