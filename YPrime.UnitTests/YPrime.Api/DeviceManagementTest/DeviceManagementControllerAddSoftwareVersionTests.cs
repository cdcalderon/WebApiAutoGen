using System;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Data.Study.Models;
using YPrime.UnitTests.YPrime.Api.DeviceManagementControllerTests;

namespace YPrime.UnitTests.YPrime.Api.DeviceManagementTest
{
    [TestClass]
    public class DeviceManagementControllerAddSoftwareVersionTests : DeviceManangementControllerTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            base.TestInitialize();
            Controller.Request = new HttpRequestMessage();
        }

        [TestMethod]
        public void WhenCalledWithValidPayload_WillRespondWithSuccessStatusCode()
        {
            var newSoftwareVersion = new SoftwareVersion
            {
                VersionNumber = "1.0.0.0",
                PackagePath = "testUrl"
            };

            SoftwareVersionRepository.Setup(x => x.CheckVersionNumberIsUsed(newSoftwareVersion.VersionNumber))
                .Returns(false);
            SoftwareVersionRepository.Setup(x => x.AddSoftwareVersion(newSoftwareVersion)).Returns(true);

            var result = Controller.AddSoftwareVersion(newSoftwareVersion);
            Assert.IsTrue(result.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void WhenCalledWithInValidVersion_WillRespondWithBadRequestStatusCode()
        {
            var newSoftwareVersion = new SoftwareVersion
            {
                VersionNumber = "1.0.0.",
                PackagePath = "testUrl"
            };

            SoftwareVersionRepository.Setup(x => x.CheckVersionNumberIsUsed(newSoftwareVersion.VersionNumber))
                .Returns(false);
            SoftwareVersionRepository.Setup(x => x.AddSoftwareVersion(newSoftwareVersion)).Returns(true);

            var result = Controller.AddSoftwareVersion(newSoftwareVersion);
            Assert.IsTrue(result.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void WhenCalledWithExistingVersion_WillRespondWithOkRequestStatusCode()
        {
            var newSoftwareVersion = new SoftwareVersion
            {
                VersionNumber = "1.0.0.0",
                PackagePath = "testUrl"
            };

            SoftwareVersionRepository.Setup(x => x.CheckVersionNumberIsUsed(newSoftwareVersion.VersionNumber))
                .Returns(true);
            SoftwareVersionRepository.Setup(x => x.AddSoftwareVersion(newSoftwareVersion)).Returns(true);

            var result = Controller.AddSoftwareVersion(newSoftwareVersion);
            Assert.IsTrue(result.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void WhenCalledWithEmptyPayload_WillRespondWithBadRequestStatusCode()
        {
            var newSoftwareVersion = new SoftwareVersion();

            SoftwareVersionRepository.Setup(x => x.CheckVersionNumberIsUsed(newSoftwareVersion.VersionNumber))
                .Returns(false);
            SoftwareVersionRepository.Setup(x => x.AddSoftwareVersion(newSoftwareVersion)).Returns(true);

            var result = Controller.AddSoftwareVersion(newSoftwareVersion);
            Assert.IsTrue(result.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void WhenCalledWithNullPayload_WillRespondWithBadRequestStatusCode()
        {
            SoftwareVersion newSoftwareVersion = null;

            var result = Controller.AddSoftwareVersion(newSoftwareVersion);
            Assert.IsTrue(result.StatusCode == HttpStatusCode.BadRequest);
        }
    }
}