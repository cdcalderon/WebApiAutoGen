using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryGetDevicesForSoftwareReleaseTests : SoftwareReleaseRepositoryTestBase
    {
        private FakeDbSet<SoftwareRelease> _dataSet;
        private FakeDbSet<Device> _deviceDataSet;
        private Guid _phoneDeviceId;
        private Guid _tabletDeviceId;
        private Guid _byodDeviceId;
        private List<Device> _deviceList;
        private Device _phoneDevice;
        private Device _tabletDevice;
        private Device _byodDevice;
        private SoftwareReleaseDto _Dto;
        private SoftwareRelease _model;
        private string _packagePath;
        private Guid _platformTypeId;
        private Guid _softwareVersionId;
        private FakeDbSet<SoftwareVersion> _svDataSet;
        private SoftwareVersion _svModel;
        private string _versionNumber;
        private Site _site;
        private Site _site2;

        [TestInitialize]
        public void TestInitialize()
        {
            _softwareVersionId = Guid.NewGuid();
            _versionNumber = "1.2.3.4";
            _packagePath = "http://testPath/YPrime_DevelopServices/Packages/YPrime.eCOA.Droid_1.0.0.0.apk";
            _platformTypeId = Guid.NewGuid();
            _phoneDeviceId = Guid.NewGuid();
            _tabletDeviceId = Guid.NewGuid();
            _byodDeviceId = Guid.NewGuid();
            _Dto = new SoftwareReleaseDto
            {
                ReleaseDate = "dd-MM-yy",
                Name = "test",
                VersionNumber = "0.0.0.1",
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true,
                CountryNameList = "USA, Canada",
                SiteNameList = "10001, 20001",
                AssetTagList = "YP-12345, YP-67890",
                AssignedReportedVersionCount = "2",
            };

            _model = new SoftwareRelease
            {
                Id = new Guid(),
                Name = "test",
                SoftwareVersionId = _softwareVersionId,
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true
            };

            _svModel = new SoftwareVersion
            {
                Id = _softwareVersionId,
                VersionNumber = _versionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };

            _site = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = Guid.NewGuid()
            };

            _site2 = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = Guid.NewGuid()
            };

            _phoneDevice = new Device
            {
                Id = _phoneDeviceId,
                SoftwareReleaseId = new Guid(),
                AssetTag = "YP-Test",
                IMEI1 = null,
                IMEI2 = null,
                MACAddress = null,
                SerialNumber = null,
                SiteId = _site.Id,
                Site = _site,
                DeviceTypeId = DeviceType.Phone.Id,
                LastSyncDate = null,
                SoftwareRelease = _model
            };

            _tabletDevice = new Device
            {
                Id = _tabletDeviceId,
                SoftwareReleaseId = new Guid(),
                AssetTag = "YP-Test",
                IMEI1 = null,
                IMEI2 = null,
                MACAddress = null,
                SerialNumber = null,
                SiteId = _site2.Id,
                Site = _site2,
                DeviceTypeId = DeviceType.Tablet.Id,
                LastSyncDate = null,
                SoftwareRelease = _model
            };

            _byodDevice = new Device
            {
                Id = _byodDeviceId,
                SoftwareReleaseId = new Guid(),
                AssetTag = "YP-Test",
                IMEI1 = null,
                IMEI2 = null,
                MACAddress = null,
                SerialNumber = null,
                SiteId = _site.Id,
                Site = _site,
                DeviceTypeId = DeviceType.BYOD.Id,
                LastSyncDate = null,
                SoftwareRelease = _model
            };

            _deviceList = new List<Device>();
            _deviceList.Add(_phoneDevice);
            _deviceList.Add(_tabletDevice);
            _deviceList.Add(_byodDevice);

            SetupSoftwareVersionContext(new[] { _svModel });
            SetupDeviceContext(new[] { _phoneDevice, _tabletDevice, _byodDevice });
            SetupContext(new[] { _model });
        }

        [TestMethod]
        public void WithExistingRecordsInDataBase_WillReturnDeviceList()
        {
            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void WithNoDeviceRecordsInDataBase_WillReturnEmptyList()
        {
            SetupDeviceContext(Enumerable.Empty<Device>());
            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void WithNoSelectedSiteIdNull_WillReturnDeviceList()
        {
            _Dto.StudyWide = false;
            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void WhenCalledWillReturnCollectionType_List()
        {
            var result = Repository.GetDevicesForSoftwareRelease(_Dto);
            Assert.IsTrue(result.GetType() == typeof(List<Device>));
        }

        [TestMethod]
        public void WithNoFilters()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = null;
            _Dto.CountryIds = null;
            _Dto.SiteIds = null;

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void WithAllFilters()
        {
            _Dto.StudyWide = false;
            _Dto.SiteIds = new List<Guid> { _phoneDevice.SiteId.Value, _tabletDevice.SiteId.Value };
            _Dto.CountryIds = new List<Guid> { _site.CountryId, _site2.CountryId };
            _Dto.DeviceTypeIds = new List<Guid> { DeviceType.Phone.Id, DeviceType.Tablet.Id };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Contains(_byodDevice));
            Assert.IsTrue(result.Contains(_phoneDevice));
            Assert.IsTrue(result.Contains(_tabletDevice));
        }

        [TestMethod]
        public void WithCountryFilters()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = null;
            _Dto.CountryIds = new List<Guid> { _site.CountryId };
            _Dto.SiteIds = null;

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void WithWrongCountryFilters()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = null;
            _Dto.CountryIds = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() };
            _Dto.SiteIds = null;

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void WithSiteFilters()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = null;
            _Dto.CountryIds = null;
            _Dto.SiteIds = new List<Guid> { _site.Id };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void WithWrongSiteFilters()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = null;
            _Dto.CountryIds = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() };
            _Dto.SiteIds = null;

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.AreEqual(0, result.Count);
        }


        [TestMethod]
        public void WithPhoneDeviceTypeFilters()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds.Add(DeviceType.Phone.Id);

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(_byodDevice));
            Assert.IsTrue(result.Contains(_phoneDevice));
        }

        [TestMethod]
        public void WithTabletDeviceTypeFilters()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds.Add(DeviceType.Tablet.Id);

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(result[0], _tabletDevice);
        }

        [TestMethod]
        public void WithWrongDeviceTypeFilters()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds.Add(Guid.NewGuid());

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void WithPhoneDeviceTypeAndCountryFilter()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = new List<Guid> { DeviceType.Phone.Id };
            _Dto.CountryIds = new List<Guid> { _site.CountryId };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(_byodDevice));
            Assert.IsTrue(result.Contains(_phoneDevice));
        }

        [TestMethod]
        public void WithPhoneDeviceTypeAndWrongCountryFilter()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = new List<Guid> { DeviceType.Phone.Id };
            _Dto.CountryIds = new List<Guid> { _site2.CountryId };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void WithTabletDeviceTypeAndCountryFilter()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = new List<Guid> { DeviceType.Tablet.Id };
            _Dto.CountryIds = new List<Guid> { _site2.CountryId };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_tabletDevice.Id, result[0].Id);
        }

        [TestMethod]
        public void WithTabletDeviceTypeAndWrongCountryFilter()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = new List<Guid> { DeviceType.Tablet.Id };
            _Dto.CountryIds = new List<Guid> { _site.CountryId };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void WithPhoneDeviceTypeAndCountryAndSiteFilter()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = new List<Guid> { DeviceType.Phone.Id };
            _Dto.SiteIds = new List<Guid> { _phoneDevice.SiteId.Value };
            _Dto.CountryIds = new List<Guid> { _site.CountryId };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(_byodDevice));
            Assert.IsTrue(result.Contains(_phoneDevice));
        }

        [TestMethod]
        public void WithPhoneDeviceTypeAndCountryAndWrongSiteFilter()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = new List<Guid> { DeviceType.Phone.Id };
            _Dto.SiteIds = new List<Guid> { _tabletDevice.SiteId.Value };
            _Dto.CountryIds = new List<Guid> { _site.CountryId };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void WithTabletDeviceTypeAndCountryAndSiteFilter()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = new List<Guid> { DeviceType.Tablet.Id };
            _Dto.SiteIds = new List<Guid> { _tabletDevice.SiteId.Value };
            _Dto.CountryIds = new List<Guid> { _site2.CountryId };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Contains(_tabletDevice));
        }

        [TestMethod]
        public void WithTabletDeviceTypeAndCountryAndWrongSiteFilter()
        {
            _Dto.StudyWide = false;
            _Dto.DeviceTypeIds = new List<Guid> { DeviceType.Tablet.Id };
            _Dto.SiteIds = new List<Guid> { _phoneDevice.SiteId.Value };
            _Dto.CountryIds = new List<Guid> { _site2.CountryId };

            var result = Repository.GetDevicesForSoftwareRelease(_Dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        private void SetupContext(IEnumerable<SoftwareRelease> items)
        {
            _dataSet = new FakeDbSet<SoftwareRelease>(items);
            Context.Setup(ctx => ctx.SoftwareReleases)
                .Returns(_dataSet.Object);
        }


        private void SetupSoftwareVersionContext(IEnumerable<SoftwareVersion> items)
        {
            _svDataSet = new FakeDbSet<SoftwareVersion>(items);
            Context.Setup(ctx => ctx.SoftwareVersions)
                .Returns(_svDataSet.Object);
        }

        private void SetupDeviceContext(IEnumerable<Device> items)
        {
            _deviceDataSet = new FakeDbSet<Device>(items);
            Context.Setup(ctx => ctx.Devices)
                .Returns(_deviceDataSet.Object);
        }
    }
}