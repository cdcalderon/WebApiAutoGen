using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Controllers;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareReleaseControllerTests
{
    public abstract class SoftwareReleaseControllerTestBase : BaseControllerTest
    {
        protected SoftwareReleaseController Controller;
        protected Mock<ICountryService> CountryService;
        protected Mock<IDeviceRepository> DeviceRepository;
        protected Mock<ISoftwareReleaseRepository> Repository;
        protected Mock<ISiteRepository> SiteRepository;
        protected Mock<ISoftwareVersionRepository> SoftwareVersionRepository;
        protected Mock<IConfigurationVersionService> ConfigurationVersionService;
        protected Mock<IServiceSettings> ServiceSettings;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            _yprimeSession.CurrentUser = new StudyUserDto
            {
                Roles = new List<Core.BusinessLayer.Models.StudyRoleModel>
                {
                    new Core.BusinessLayer.Models.StudyRoleModel
                    {
                        ShortName = "YP"
                    }
                }
            };

            base.Initialize();
            Repository = new Mock<ISoftwareReleaseRepository>();
            SoftwareVersionRepository = new Mock<ISoftwareVersionRepository>();
            DeviceRepository = new Mock<IDeviceRepository>();
            CountryService = new Mock<ICountryService>();
            SiteRepository = new Mock<ISiteRepository>();
            ConfigurationVersionService = new Mock<IConfigurationVersionService>();
            ServiceSettings = new Mock<IServiceSettings>();

            Controller = new SoftwareReleaseController(
                CountryService.Object,
                ConfigurationVersionService.Object,
                Repository.Object, 
                SoftwareVersionRepository.Object,
                DeviceRepository.Object, 
                SiteRepository.Object,
                ServiceSettings.Object,
                MockSessionService.Object);

            Controller.ControllerContext = (new Mock<ControllerContext>()).Object;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            var deviceDto = new DeviceDto
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                IMEI1 = null,
                IMEI2 = null,
                MACAddress = null,
                SerialNumber = "111111111",
                SiteId = new Guid(),
                SiteName = "1000",
                DeviceTypeId = new Guid(),
                DeviceTypeName = "Mason",
                AssignedSoftwareVersionNumber = "1.0.0.0",
                LastReportedSoftwareVersionNumber = "1.0.0.0",
                LastDataSyncDate = new DateTimeOffset(),
                AssetTag = "YP-12345"
            };

            var deviceList = new List<DeviceDto>();
            deviceList.Add(deviceDto);

            DeviceRepository.Setup(repo => repo.GetAllDevices(new List<Guid>()))
                .Returns(Task.FromResult(deviceList.AsQueryable().OrderBy(s => s.Id)));

            var siteDto = new SiteDto
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                SiteNumber = "10001",
                Name = "10001",
                Address1 = "test",
                Address2 = "test",
                Address3 = "test",
                TimeZone = "test",
                Notes = "test",
                PatientDOBFormatId = 0,
                City = "test",
                State = "test",
                Zip = "test",
                CountryId = Guid.Empty,
                CountryName = "test",
                PrimaryContact = "test",
                PhoneNumber = "test",
                FaxNumber = "test",
                IsActive = true,
                LastUpdate = new DateTime()
            };

            var siteList = new List<SiteDto>();
            siteList.Add(siteDto);

            SiteRepository.Setup(repo => repo.GetAllSites(null))
                .ReturnsAsync(siteList.AsEnumerable().OrderBy(s => s.Id));
        }
    }
}