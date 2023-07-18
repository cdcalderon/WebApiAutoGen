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
    public class SoftwareReleaseControllerDeactivateSoftwareReleaseTests : SoftwareReleaseControllerTestBase
    {
        private List<Guid> _deviceIds;
        private List<Device> _deviceList;
        private Device _deviceModel;
        private SoftwareReleaseDto _Dto;
        private SoftwareRelease _model;
        private SoftwareVersion _svModel;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            _Dto = new SoftwareReleaseDto();
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
            _deviceIds = new List<Guid>();
            _deviceIds.Add(deviceId);
            Repository.Setup(x => x.CreateSoftwareRelease(_Dto));
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
        public void WhenCalled_WillReturnViewResultType()
        {
            var result = Controller.DeactivateSoftwareRelease(new Guid());

            YAssert.IsType<JsonResult>(result);
        }
    }
}