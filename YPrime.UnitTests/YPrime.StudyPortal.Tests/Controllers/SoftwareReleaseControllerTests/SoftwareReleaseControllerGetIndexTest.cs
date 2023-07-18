using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.UnitTests;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareReleaseControllerTests
{
    [TestClass]
    public class SoftwareReleaseControllerGetIndexTests : SoftwareReleaseControllerTestBase
    {
        private SoftwareReleaseDto _expectedSoftwareRelease;
        private ConfigurationVersion _configurationVersion;
        private ConfigurationVersion _prodApprovedConfigurationVersion;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            _expectedSoftwareRelease = new SoftwareReleaseDto
            {
                Countries = new List<CountryModel>(),
                Sites = new List<SiteDto>()
            };

            _configurationVersion = new ConfigurationVersion
            {
                Id = Guid.NewGuid(),
                SrdVersion = "03.00",
                ConfigurationVersionNumber = "1.2.3.4",
                ApprovedForProd = false
            };

            _prodApprovedConfigurationVersion = new ConfigurationVersion
            {
                Id = Guid.NewGuid(),
                SrdVersion = "02.00",
                ConfigurationVersionNumber = "1.2.3.3",
                ApprovedForProd = true
            };

            CountryService
                .Setup(repo => repo.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CountryModel>());

            ConfigurationVersionService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ConfigurationVersion>
                {
                    _prodApprovedConfigurationVersion,
                    _configurationVersion
                });

            var sites = new List<SiteDto>().AsEnumerable().OrderBy(s => s.Id);
            SiteRepository.Setup(repo => repo.GetAllSites(null))
                .ReturnsAsync(sites);

            SoftwareVersionRepository.Setup(repo => repo.GetAllSoftwareVersions())
                .Returns(new List<SoftwareVersion>());

            var devices = new List<DeviceDto>().AsQueryable().OrderBy(s => s.Id);
            DeviceRepository.Setup(repo => repo.GetAllDevices(new List<Guid>()))
                .Returns(Task.FromResult(devices));

            Repository
                .Setup(r => r.GetProvisionalDeviceTypesForStudy())
                .ReturnsAsync(new List<DeviceType>
                {
                    DeviceType.Phone,
                    DeviceType.Tablet
                });
        }

        [TestMethod]
        public async Task WhenCalled_WillReturnViewResult()
        {
            var result = await Controller.Index();
            YAssert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public async Task WhenCalled_WillHaveViewNameBeIndex()
        {
            var result = await Controller.Index();
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public async Task WhenCalled_WillSetModel()
        {
            ServiceSettings
                .SetupGet(ss => ss.StudyPortalAppEnvironment)
                .Returns("DEV");

            var result = await Controller.Index();

            var resultSR = result.Model as SoftwareReleaseDto;

            CollectionAssert.AreEqual(_expectedSoftwareRelease.Countries, resultSR.Countries);
            CollectionAssert.AreEqual(_expectedSoftwareRelease.Sites, resultSR.Sites);

            Assert.AreEqual(2, resultSR.ConfigVersionList.Count());

            Assert.IsTrue(resultSR.ConfigVersionList.Any(vl => vl.Value == _configurationVersion.Id.ToString()));
            Assert.IsTrue(resultSR.ConfigVersionList.Any(vl => vl.Text == _configurationVersion.DisplayVersion));

            Assert.IsTrue(resultSR.ConfigVersionList.Any(vl => vl.Value == _prodApprovedConfigurationVersion.Id.ToString()));
            Assert.IsTrue(resultSR.ConfigVersionList.Any(vl => vl.Text == _prodApprovedConfigurationVersion.DisplayVersion));
        }

        [TestMethod]
        public async Task WhenCalled_WillSetModelProdEnv()
        {
            ServiceSettings
                .SetupGet(ss => ss.StudyPortalAppEnvironment)
                .Returns("Production");

            var result = await Controller.Index();

            var resultSR = result.Model as SoftwareReleaseDto;

            CollectionAssert.AreEqual(_expectedSoftwareRelease.Countries, resultSR.Countries);
            CollectionAssert.AreEqual(_expectedSoftwareRelease.Sites, resultSR.Sites);

            Assert.AreEqual(1, resultSR.ConfigVersionList.Count());

            Assert.IsFalse(resultSR.ConfigVersionList.Any(vl => vl.Value == _configurationVersion.Id.ToString()));
            Assert.IsFalse(resultSR.ConfigVersionList.Any(vl => vl.Text == _configurationVersion.DisplayVersion));

            Assert.IsTrue(resultSR.ConfigVersionList.Any(vl => vl.Value == _prodApprovedConfigurationVersion.Id.ToString()));
            Assert.IsTrue(resultSR.ConfigVersionList.Any(vl => vl.Text == _prodApprovedConfigurationVersion.DisplayVersion));
        }
    }
}