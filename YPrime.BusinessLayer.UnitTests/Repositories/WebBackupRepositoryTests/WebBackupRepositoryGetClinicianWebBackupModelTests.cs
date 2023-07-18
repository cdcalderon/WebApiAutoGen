using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.WebBackupRepositoryTests
{
    [TestClass]
    public class WebBackupRepositoryGetClinicianWebBackupModelTests : WebBackupRepositoryTestBase
    {
        [TestMethod]
        public async Task GetModelTimeZoneLookupTest()
        {
            var testConfigId = Guid.NewGuid();

            var testSite = new SiteDto
            {
                Id = Guid.NewGuid(),
                TimeZone = string.Empty,
                WebBackupExpireDate = DateTime.Now.AddDays(7),
                Name = TestSiteName
            };

            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                SiteId = testSite.Id,
                AssetTag = TestAssetTag,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            MockSoftwareReleaseRepository
                .Setup(r => r.FindLatestConfigurationVersion(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<Guid>>()))
                .ReturnsAsync(testConfigId);

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == testSite.Id)))
                .ReturnsAsync(testSite);

            MockTimeZoneRepository
                .Setup(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()))
                .ReturnsAsync(TestTimeZone);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName)))
                .Returns(TestKeyValue);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupTabletDevice(It.Is<Guid>(id => id == testSite.Id)))
                .Returns(testDevice);

            var repository = GetRepository();

            var result = await repository.GetClinicianWebBackupModel(
                testSite.Id,
                TestHostAddress);

            Assert.IsTrue(result.CanDoWebBackup);
            Assert.IsTrue(result.Title.Contains(TestSiteName));
            Assert.AreEqual(ExpectedTabletHeight, result.IFrameHeight);
            Assert.AreEqual(ExpectedTabletWidth, result.IFrameWidth);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.WebBackupInstruction));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.WebBackupError));
            Assert.AreEqual(TestTimeZone, result.TimeZone);

            MockTimeZoneRepository
                .Verify(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetModelTimeZoneViaSiteTest()
        {
            var testSite = new SiteDto
            {
                Id = Guid.NewGuid(),
                TimeZone = TestTimeZone,
                WebBackupExpireDate = DateTime.Now.AddDays(7),
                Name = TestSiteName
            };

            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                SiteId = testSite.Id,
                AssetTag = TestAssetTag,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == testSite.Id)))
                .ReturnsAsync(testSite);

            MockTimeZoneRepository
                .Setup(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()))
                .ReturnsAsync(TestTimeZone);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName)))
                .Returns(TestKeyValue);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupTabletDevice(It.Is<Guid>(id => id == testSite.Id)))
                .Returns(testDevice);

            var repository = GetRepository();

            var result = await repository.GetClinicianWebBackupModel(
                testSite.Id,
                TestHostAddress);

            Assert.IsTrue(result.CanDoWebBackup);
            Assert.IsTrue(result.Title.Contains(TestSiteName));
            Assert.AreEqual(ExpectedTabletHeight, result.IFrameHeight);
            Assert.AreEqual(ExpectedTabletWidth, result.IFrameWidth);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.WebBackupInstruction));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.WebBackupError));
            Assert.AreEqual(TestTimeZone, result.TimeZone);

            MockTimeZoneRepository
                .Verify(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task GetModelNullDeviceTest()
        {
            var testSite = new SiteDto
            {
                Id = Guid.NewGuid(),
                TimeZone = TestTimeZone,
                WebBackupExpireDate = DateTime.Now.AddDays(7),
                Name = TestSiteName
            };

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == testSite.Id)))
                .ReturnsAsync(testSite);

            MockTimeZoneRepository
                .Setup(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()))
                .ReturnsAsync(TestTimeZone);

            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName), It.IsAny<Guid?>()))
                .ReturnsAsync(TestKeyValue);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupTabletDevice(It.Is<Guid>(id => id == testSite.Id)))
                .Returns((Device) null);

            var repository = GetRepository();

            var result = await repository.GetClinicianWebBackupModel(
                testSite.Id,
                TestHostAddress);

            Assert.IsFalse(result.CanDoWebBackup);
            Assert.IsFalse(result.Title.Contains(TestSiteName));
            Assert.AreEqual(ExpectedTabletHeight, result.IFrameHeight);
            Assert.AreEqual(ExpectedTabletWidth, result.IFrameWidth);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Title));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.WebBackupInstruction));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.WebBackupError));
            Assert.AreEqual(TestTimeZone, result.TimeZone);
        }

        [TestMethod]
        public async Task GetModelNotEligibleTest()
        {
            var testSite = new SiteDto
            {
                Id = Guid.NewGuid(),
                TimeZone = TestTimeZone,
                WebBackupExpireDate = DateTime.Now.AddDays(-7),
                Name = TestSiteName
            };

            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                SiteId = testSite.Id,
                AssetTag = TestAssetTag,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == testSite.Id)))
                .ReturnsAsync(testSite);

            MockTimeZoneRepository
                .Setup(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()))
                .ReturnsAsync(TestTimeZone);

            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName), It.IsAny<Guid?>()))
                .ReturnsAsync(TestKeyValue);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupTabletDevice(It.Is<Guid>(id => id == testSite.Id)))
                .Returns(testDevice);

            var repository = GetRepository();

            var result = await repository.GetClinicianWebBackupModel(
                testSite.Id,
                TestHostAddress);

            Assert.IsFalse(result.CanDoWebBackup);
            Assert.IsFalse(result.Title.Contains(TestSiteName));
            Assert.AreEqual(ExpectedTabletHeight, result.IFrameHeight);
            Assert.AreEqual(ExpectedTabletWidth, result.IFrameWidth);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Title));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.WebBackupInstruction));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.WebBackupError));
            Assert.AreEqual(TestTimeZone, result.TimeZone);
        }
    }
}