using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.UnitTests;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareReleaseControllerTests
{
    [TestClass]
    public class SoftwareReleaseControllerPostConfrimationTests : SoftwareReleaseControllerTestBase
    {
        private List<Device> _deviceList;
        private Device _deviceModel;
        private SoftwareReleaseDto _Dto;
        private SoftwareRelease _model;
        private SoftwareVersion _svModel;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            _Dto = new SoftwareReleaseDto
            {
                ReleaseDate = "dd-MM-yy",
                Name = "test",
                VersionNumber = "1.2.3.4",
                IsActive = true,
                Required = false,
                ConfigurationId = new Guid(),
                StudyWide = true,
                CountryNameList = "USA, Canada",
                SiteNameList = "10001, 20001",
                AssetTagList = "YP-12345, YP-67890",
                AssignedReportedVersionCount = "2"
            };
            _svModel = new SoftwareVersion
            {
                Id = new Guid(),
                VersionNumber = "1.2.3.4",
                PackagePath = "testPath",
                PlatformTypeId = new Guid()
            };
            _model = new SoftwareRelease
            {
                Id = new Guid(),
                Name = "test",
                SoftwareVersionId = new Guid(),
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true,
                SoftwareVersion = _svModel
            };

            var deviceId = new Guid();
            _deviceList = new List<Device>();
            _deviceModel = new Device
            {
                Id = deviceId,
                SoftwareReleaseId = new Guid(),
                AssetTag = "YP-Test",
                IMEI1 = null,
                IMEI2 = null,
                MACAddress = null,
                SerialNumber = null,
                SiteId = null,
                DeviceTypeId = new Guid(),
                LastSyncDate = null,
                SoftwareRelease = _model
            };
            _deviceList.Add(_deviceModel);

            Repository
                .Setup(x => x.CreateSoftwareRelease(_Dto))
                .Returns(async () => { });

            Repository.Setup(x => x.GetDevicesForSoftwareRelease(_Dto)).Returns(_deviceList);
            SoftwareVersionRepository.Setup(x => x.GetAllSoftwareVersions()).Returns(new List<SoftwareVersion>());

            var devices = new List<DeviceDto>().AsQueryable().OrderBy(s => s.Id);
            DeviceRepository.Setup(repo => repo.GetAllDevices(new List<Guid>()))
                .Returns(Task.FromResult(devices));

            CountryService
                .Setup(repo => repo.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CountryModel>());

            var sites = new List<SiteDto>().AsEnumerable().OrderBy(s => s.Id);
            SiteRepository.Setup(repo => repo.GetAllSites(null))
                .ReturnsAsync(sites);
        }

        [TestMethod]
        public async Task WhenCalled_WillInsertNewSoftwareReleaseAndReturnActionResult()
        {
            var result = await Controller.Create(_Dto) as RedirectToRouteResult;

            YAssert.IsType<RedirectToRouteResult>(result);

            Repository
                .Verify(x => x.CreateSoftwareRelease(_Dto), Times.Once);
        }

        [TestMethod]
        public async Task WhenCalled_WillFailToInsertSoftwareReleaseWhenModelStateIsFalse()
        {
            Controller.ModelState.AddModelError("test", "test");
            var result = await Controller.Create(_Dto) as RedirectToRouteResult;

            YAssert.IsType<RedirectToRouteResult>(result);
            Assert.IsFalse(Controller.ModelState.IsValid);
        }
    }
}