using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryGetLatestSoftwareReleaseTests : SoftwareReleaseRepositoryTestBase
    {
        private Guid _countryAId = Guid.NewGuid();
        private Guid _countryBId = Guid.NewGuid();

        private Site _siteA;
        private Site _siteB;

        private SoftwareVersion _olderVersion;
        private SoftwareVersion _newerVersion;
        private SoftwareVersion _deviceTypeVersion;

        private SoftwareRelease _softwareReleaseAOlder;
        private SoftwareRelease _softwareReleaseANewer;
        private SoftwareRelease _softwareReleaseB;
        private SoftwareRelease _softwareReleaseDeviceType;

        private SoftwareReleaseCountry _softwareReleaseCountryAOlder;
        private SoftwareReleaseCountry _softwareReleaseCountryANewer;
        private SoftwareReleaseCountry _softwareReleaseCountryB;

        private SoftwareReleaseDeviceType _softwareReleaseDeviceTypePhone;
        private SoftwareReleaseDeviceType _softwareReleaseDeviceTypeTablet;

        [TestInitialize]
        public void TestInitialize()
        {
            _siteA = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = _countryAId
            };

            _siteB = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = _countryBId
            };

            _olderVersion = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = "1.0.0.0"
            };

            _deviceTypeVersion = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = "4.0.0.0"
            };

            _newerVersion = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = "1.0.0.1"
            };

            _softwareReleaseDeviceType = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                StudyWide = false,
                Name = "device type release",
                SoftwareVersionId = _deviceTypeVersion.Id,
                SoftwareVersion = _deviceTypeVersion,
                ConfigurationVersion = "4.0",
                ConfigurationId = Guid.NewGuid(),
                SoftwareReleaseDeviceTypes = new List<SoftwareReleaseDeviceType>() { _softwareReleaseDeviceTypeTablet },
            };

            _softwareReleaseAOlder = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                StudyWide = false,
                SoftwareVersionId = _olderVersion.Id,
                SoftwareVersion = _olderVersion,
                ConfigurationVersion = "1.0",
                ConfigurationId = Guid.NewGuid(),
                SoftwareReleaseDeviceTypes = new List<SoftwareReleaseDeviceType>(),
                Sites = new List<Site>
                {
                    _siteA
                }
            };

            _softwareReleaseANewer = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                StudyWide = false,
                SoftwareVersionId = _newerVersion.Id,
                SoftwareVersion = _newerVersion,
                ConfigurationVersion = "3.0",
                ConfigurationId = Guid.NewGuid(),
                SoftwareReleaseDeviceTypes = new List<SoftwareReleaseDeviceType>() { _softwareReleaseDeviceTypePhone },
                Sites = new List<Site>
                {
                    _siteA
                }
            };

            _softwareReleaseB = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                StudyWide = false,
                SoftwareVersionId = _olderVersion.Id,
                SoftwareVersion = _olderVersion,
                ConfigurationVersion = "2.0",
                ConfigurationId = Guid.NewGuid(),
                SoftwareReleaseDeviceTypes = new List<SoftwareReleaseDeviceType>(),
                Sites = new List<Site>
                {
                    _siteB
                }
            };

            _softwareReleaseCountryAOlder = new SoftwareReleaseCountry
            {
                SoftwareReleaseId = _softwareReleaseAOlder.Id,
                SoftwareRelease = _softwareReleaseAOlder,
                CountryId = _countryAId
            };

            _softwareReleaseCountryANewer = new SoftwareReleaseCountry
            {
                SoftwareReleaseId = _softwareReleaseANewer.Id,
                SoftwareRelease = _softwareReleaseANewer,
                CountryId = _countryAId
            };

            _softwareReleaseCountryB = new SoftwareReleaseCountry
            {
                SoftwareReleaseId = _softwareReleaseB.Id,
                SoftwareRelease = _softwareReleaseB,
                CountryId = _countryBId
            };

            _softwareReleaseDeviceTypePhone = new SoftwareReleaseDeviceType
            {
                SoftwareReleaseId = _softwareReleaseANewer.Id,
                SoftwareRelease = _softwareReleaseANewer,
                DeviceTypeId = DeviceType.Phone.Id
            };

            _softwareReleaseDeviceTypeTablet = new SoftwareReleaseDeviceType
            {
                SoftwareReleaseId = _softwareReleaseDeviceType.Id,
                SoftwareRelease = _softwareReleaseDeviceType,
                DeviceTypeId = DeviceType.Tablet.Id
            };

            var releaseDataSet = new FakeDbSet<SoftwareRelease>(new List<SoftwareRelease>
            {
                _softwareReleaseANewer,
                _softwareReleaseAOlder,
                _softwareReleaseB,
                _softwareReleaseDeviceType
            });

            Context
                .Setup(c => c.SoftwareReleases)
                .Returns(releaseDataSet.Object);

            var versionDataSet = new FakeDbSet<SoftwareVersion>(new List<SoftwareVersion>
            {
                _olderVersion,
                _newerVersion
            });

            Context
                .Setup(c => c.SoftwareVersions)
                .Returns(versionDataSet.Object);

            var releaseCountryDataSet = new FakeDbSet<SoftwareReleaseCountry>(new List<SoftwareReleaseCountry>
            {
                _softwareReleaseCountryANewer,
                _softwareReleaseCountryAOlder,
                _softwareReleaseCountryB
            });

            var deviceTypeDataSet = new FakeDbSet<SoftwareReleaseDeviceType>(new List<SoftwareReleaseDeviceType>
            {
                _softwareReleaseDeviceTypePhone,
                _softwareReleaseDeviceTypeTablet
            });

            Context
                .Setup(c => c.SoftwareReleaseCountry)
                .Returns(releaseCountryDataSet.Object);

            Context
                .Setup(c => c.SoftwareReleaseDeviceTypes)
                .Returns(deviceTypeDataSet.Object);

            var siteDataSet = new FakeDbSet<Site>(new List<Site>
            {
                _siteA,
                _siteB
            });

            Context
                .Setup(c => c.Sites)
                .Returns(siteDataSet.Object);
        }

        [TestMethod]
        public void GetLatestSoftwareReleaseTest()
        {
            var result = Repository.GetLatestSoftwareRelease(_siteA.Id, DeviceType.Phone.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseANewer.Id, result.Id);
        }

        [TestMethod]
        public void GetLatestSoftwareReleaseStudyWideInActiveReleaseTest()
        {
            _softwareReleaseANewer.StudyWide = true;
            _softwareReleaseANewer.IsActive = false;
            var result = Repository.GetLatestSoftwareRelease(_siteA.Id, DeviceType.Phone.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseAOlder.Id, result.Id);
        }

        [TestMethod]
        public void GetLatestSoftwareReleaseNullConfigVersionTest()
        {
            _softwareReleaseANewer.ConfigurationVersion = null;

            var result = Repository.GetLatestSoftwareRelease(_siteA.Id, DeviceType.Phone.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseANewer.Id, result.Id);
        }

        [TestMethod]
        public void GetLatestSoftwareReleaseDifferentConfigVersionsTest()
        {
            _softwareReleaseANewer.ConfigurationVersion = "2.0";
            _softwareReleaseANewer.SoftwareVersionId = _newerVersion.Id;
            _softwareReleaseANewer.SoftwareVersion = _newerVersion;

            _softwareReleaseB.SoftwareVersionId = _newerVersion.Id;
            _softwareReleaseB.SoftwareVersion = _newerVersion;
            _softwareReleaseB.ConfigurationVersion = "3.0";
            _softwareReleaseB.Sites = new List<Site> { _siteA };
            _softwareReleaseCountryB.CountryId = _countryAId;


            var result = Repository.GetLatestSoftwareRelease(_siteA.Id, DeviceType.Phone.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseB.Id, result.Id);
        }

        [TestMethod]
        public void GetLatestSoftwareReleaseAssignedByDeviceTypeTest()
        {
            _softwareReleaseANewer.ConfigurationVersion = "2.0";
            _softwareReleaseANewer.SoftwareVersionId = _newerVersion.Id;
            _softwareReleaseANewer.SoftwareVersion = _newerVersion;

            _softwareReleaseB.SoftwareVersionId = _newerVersion.Id;
            _softwareReleaseB.SoftwareVersion = _newerVersion;
            _softwareReleaseB.ConfigurationVersion = "3.0";
            _softwareReleaseB.Sites = new List<Site> { _siteA };

            var result = Repository.GetLatestSoftwareRelease(_siteA.Id, DeviceType.Tablet.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseDeviceType.Id, result.Id);
        }

        [TestMethod]
        public void GetLatestSoftwareReleaseWithOtherDeviceTypeTest()
        {
            _softwareReleaseANewer.ConfigurationVersion = "2.0";
            _softwareReleaseANewer.SoftwareVersionId = _newerVersion.Id;
            _softwareReleaseANewer.SoftwareVersion = _newerVersion;

            _softwareReleaseB.SoftwareVersionId = _newerVersion.Id;
            _softwareReleaseB.SoftwareVersion = _newerVersion;
            _softwareReleaseB.ConfigurationVersion = "3.0";
            _softwareReleaseB.Sites = new List<Site> { _siteA };

            var result = Repository.GetLatestSoftwareRelease(_siteA.Id, DeviceType.Phone.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseB.Id, result.Id);
        }

        [TestMethod]
        public void GetLatestSoftwareReleaseDifferentConfigWithNullVersionsTest()
        {
            _softwareReleaseANewer.ConfigurationVersion = "2.0";
            _softwareReleaseANewer.SoftwareVersionId = _newerVersion.Id;
            _softwareReleaseANewer.SoftwareVersion = _newerVersion;

            _softwareReleaseB.SoftwareVersionId = _newerVersion.Id;
            _softwareReleaseB.SoftwareVersion = _newerVersion;
            _softwareReleaseB.ConfigurationVersion = null;
            _softwareReleaseB.Sites = new List<Site> { _siteA };
            _softwareReleaseCountryB.CountryId = _countryAId;


            var result = Repository.GetLatestSoftwareRelease(_siteA.Id, DeviceType.Phone.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseANewer.Id, result.Id);
        }

        [TestMethod]
        public void GetLatestSoftwareReleaseDifferentCountryTest()
        {
            var olderReleaseStudyWide = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersionId = _olderVersion.Id,
                SoftwareVersion = _olderVersion,
                ConfigurationVersion = "1.0",
                IsActive = true,
                StudyWide = true
            };

            var newerReleaseAtSiteB = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersionId = _newerVersion.Id,
                SoftwareVersion = _newerVersion,
                ConfigurationVersion = "2.0",
                IsActive = true,
                StudyWide = false,
            };

            var newerReleaseAtSiteBCountry = new SoftwareReleaseCountry
            {
                CountryId = _countryBId,
                SoftwareReleaseId = newerReleaseAtSiteB.Id,
                SoftwareRelease = newerReleaseAtSiteB
            };

            var releaseDataSet = new FakeDbSet<SoftwareRelease>(new List<SoftwareRelease> { olderReleaseStudyWide, newerReleaseAtSiteB });
            var releaseCountryDataSet = new FakeDbSet<SoftwareReleaseCountry>(new List<SoftwareReleaseCountry>{ newerReleaseAtSiteBCountry });
            var deviceTypeDataSet = new FakeDbSet<SoftwareReleaseDeviceType>(new List<SoftwareReleaseDeviceType>());

            Context
                .Setup(c => c.SoftwareReleases)
                .Returns(releaseDataSet.Object);

            Context
                .Setup(c => c.SoftwareReleaseCountry)
                .Returns(releaseCountryDataSet.Object);

            Context
                .Setup(c => c.SoftwareReleaseDeviceTypes)
                .Returns(deviceTypeDataSet.Object);

            var result = Repository.GetLatestSoftwareRelease(_siteA.Id, DeviceType.BYOD.Id);

            Assert.AreEqual(olderReleaseStudyWide.Id, result.Id);
        }

        [TestMethod]
        public void GetLatestSoftwareReleaseBYODByDeviceType()
        {
            _softwareReleaseAOlder.StudyWide = true;
            var result = Repository.GetLatestSoftwareRelease(null, DeviceType.BYOD.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseANewer.Id, result.Id);
        }

        [TestMethod]
        public void GetLatestGlobalConfigurationStudyWide()
        {
            _softwareReleaseDeviceType.SoftwareReleaseDeviceTypes = new List<SoftwareReleaseDeviceType>(); 
            _softwareReleaseAOlder.StudyWide = true;
            _softwareReleaseANewer.StudyWide = true;

            var result = Repository.GetLatestGlobalConfigurationVersionId();

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseANewer.ConfigurationId, result);
        }

        [TestMethod]
        public void GetLatestGlobalConfigurationDeviceType()
        {
            var result = Repository.GetLatestGlobalConfigurationVersionId();

            Assert.IsNotNull(result);
            Assert.AreEqual(_softwareReleaseDeviceType.ConfigurationId, result);
        }
    }
}
