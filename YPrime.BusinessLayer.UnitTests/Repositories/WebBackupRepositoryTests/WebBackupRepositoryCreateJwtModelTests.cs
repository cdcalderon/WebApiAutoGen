using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Config.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
namespace YPrime.BusinessLayer.UnitTests.Repositories.WebBackupRepositoryTests
{
    [TestClass]
    public class WebBackupRepositoryCreateJwtModelTests : WebBackupRepositoryTestBase
    {
        [TestMethod]
        public async Task GetModelHandheldTest()
        {
            var testPatientId = Guid.NewGuid();
            var testWebBackupType = WebBackupType.HandheldPatient;
            Guid testVisitId = Guid.NewGuid();
            var testDaysOpen = 2;
            var testWebBackUpHandheldEnabled = 1;

            var testSite = new SiteDto
            {
                Id = Guid.NewGuid(),
                TimeZone = TestUtcTimeZone,
                WebBackupExpireDate = DateTime.Now.AddDays(7),
                Name = TestSiteName
            };

            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                SiteId = testSite.Id,
                AssetTag = TestAssetTag,
                PatientId = testPatientId,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            var testPatient = new Patient
            {
                Id = testPatientId,
            };

            var patientDataset = new FakeDbSet<Patient>(new List<Patient> {testPatient});
            patientDataset.Setup(d => d.Find(It.Is<Guid>(id => id == testPatientId))).Returns(testPatient);
            MockContext.Setup(c => c.Patients).Returns(patientDataset.Object);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupHandheldDevice(It.Is<Guid>(pid => pid == testPatientId)))
                .Returns(testDevice);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupTabletDevice(It.Is<Guid>(sid => sid == testSite.Id)))
                .Returns(testDevice);

            MockWebBackUpRepository
                .Setup(r => r.GetWebBackupHandheldValue())
                .ReturnsAsync(testDaysOpen);

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(sid => sid == testSite.Id)))
                .ReturnsAsync(testSite);

            var language = new LanguageModel()
            {
                CultureName = "en-US",
                Id = new Guid()
            };

            MockLanguageService
                .Setup(r => r.Get(It.Is<Guid>(sid => sid == language.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(language);
            
            var repository = GetRepository();       

            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(t => t == "WebBackupHandheldEnabled"), It.IsAny<Guid?>()))
                .ReturnsAsync(testWebBackUpHandheldEnabled.ToString());

            var result = await repository.CreateJwtModel(
                testPatientId,
                testSite.Id,
                testWebBackupType,
                testVisitId);

            var dates = DateTime.UtcNow.AddDays(testDaysOpen);

            Assert.AreEqual(testWebBackupType, result.WebBackupType);
            Assert.AreEqual(testVisitId, result.VisitId);
            Assert.AreEqual(testSite.Id, result.SiteId);
            Assert.AreEqual(testDevice.Id, result.DeviceId);
            Assert.AreEqual(testPatientId, result.PatientId);
            Assert.IsTrue(result.ExpirationDate > DateTime.UtcNow);
            Assert.IsTrue(result.ExpirationDate <= dates);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupHandheldDevice(It.IsAny<Guid>()), Times.Once);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupTabletDevice(It.IsAny<Guid>()), Times.Never);

            MockWebBackUpRepository
                .Verify(r => r.GetWebBackupHandheldValue(), Times.Never);

            MockWebBackUpRepository
                .Verify(r => r.GetWebBackupTabletValue(), Times.Never);
        }

        [TestMethod]
        public async Task GetModelTabletTest()
        {
            var testPatientId = Guid.NewGuid();
            var testWebBackupType = WebBackupType.TabletPatient;
            Guid? testVisitId = null;
            var testDaysOpen = 4;

            var testSite = new SiteDto
            {
                Id = Guid.NewGuid(),
                TimeZone = TestUtcTimeZone,
                WebBackupExpireDate = DateTime.Now.AddDays(7),
                Name = TestSiteName
            };

            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                SiteId = testSite.Id,
                AssetTag = TestAssetTag,
                PatientId = testPatientId,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            var testPatient = new Patient
            {
                Id = testPatientId
            };

            var patientDataset = new FakeDbSet<Patient>(new List<Patient> {testPatient});
            patientDataset.Setup(d => d.Find(It.Is<Guid>(id => id == testPatientId))).Returns(testPatient);
            MockContext.Setup(c => c.Patients).Returns(patientDataset.Object);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupHandheldDevice(It.Is<Guid>(pid => pid == testPatientId)))
                .Returns(testDevice);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupTabletDevice(It.Is<Guid>(sid => sid == testSite.Id)))
                .Returns(testDevice);

            MockWebBackUpRepository
                .Setup(r => r.GetWebBackupTabletValue())
                .ReturnsAsync(testDaysOpen);

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(sid => sid == testSite.Id)))
                .ReturnsAsync(testSite);

            var language = new LanguageModel()
            {
                CultureName = "en-US",
                Id = new Guid()
            };

            MockLanguageService
                .Setup(r => r.Get(It.Is<Guid>(sid => sid == language.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(language);

            var repository = GetRepository();

            var result = await repository.CreateJwtModel(
                testPatientId,
                testSite.Id,
                testWebBackupType,
                testVisitId);

            Assert.AreEqual(testWebBackupType, result.WebBackupType);
            Assert.AreEqual(testVisitId, result.VisitId);
            Assert.AreEqual(testSite.Id, result.SiteId);
            Assert.AreEqual(testDevice.Id, result.DeviceId);
            Assert.AreEqual(testPatientId, result.PatientId);
            Assert.AreEqual(testSite.WebBackupExpireDate, result.ExpirationDate);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupHandheldDevice(It.IsAny<Guid>()), Times.Never);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupTabletDevice(It.IsAny<Guid>()), Times.Once);  
        }
    }
}