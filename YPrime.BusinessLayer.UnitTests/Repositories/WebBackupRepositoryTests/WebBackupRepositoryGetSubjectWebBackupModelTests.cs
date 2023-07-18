using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Config.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Config.Defaults;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.BusinessLayer.UnitTests.Repositories.WebBackupRepositoryTests
{
    [TestClass]
    public class WebBackupRepositoryGetSubjectWebBackupModelTests : WebBackupRepositoryTestBase
    {
        [TestMethod]
        public async Task GetModelHandheldTest()
        {
            var testConfigId = Guid.NewGuid();

            var testPatientCultureCode = "en-US";
            var testTokenContent = "Test Token Content";
            var testToken = HttpUtility.HtmlEncode(testTokenContent);
            var testPatientId = Guid.NewGuid();
            var testCaregiverId = Guid.NewGuid();
            var testVisitId = Guid.NewGuid();

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
                PatientId = testPatientId,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            var testTokenModel = new WebBackupJwtModel
            {
                SiteId = testSite.Id,
                WebBackupType = WebBackupType.HandheldPatient,
                PatientId = testPatientId,
                ExpirationDate = testSite.WebBackupExpireDate.Value,
                VisitId = testVisitId,
                CaregiverId = testCaregiverId,
                CultureName = testPatientCultureCode
            };

            var testPatient = new Patient
            {
                Id = testPatientId,
                LanguageId = Languages.English.Id
            };

            var patientDataset = new FakeDbSet<Patient>(new List<Patient> { testPatient });
            MockContext.Setup(c => c.Patients).Returns(patientDataset.Object);

            MockSoftwareReleaseRepository
                .Setup(r => r.FindLatestConfigurationVersion(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<List<Guid>>()))
                .ReturnsAsync(testConfigId);

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == testTokenModel.SiteId)))
                .ReturnsAsync(testSite);

            MockJwtRepository
                .Setup(r => r.Decrypt<WebBackupJwtModel>(It.Is<string>(t => t == testTokenContent)))
                .Returns(testTokenModel);

            MockTimeZoneRepository
                .Setup(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()))
                .ReturnsAsync(TestTimeZone);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupHandheldDevice(It.Is<Guid>(pid => pid == testTokenModel.PatientId)))
                .Returns(testDevice);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupHandheldPublicKeyName)))
                .Returns(TestKeyValue);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName)))
                .Returns(TestKeyValue);

            var repository = GetRepository();

            var result = await repository.GetSubjectWebBackupModel(
                testToken,
                TestHostAddress);

            Assert.IsTrue(result.CanDoWebBackup);
            Assert.IsTrue(result.Title.Contains(TestSiteName));
            Assert.AreEqual(ExpectedHandheldHeight, result.IFrameHeight);
            Assert.AreEqual(ExpectedHandheldWidth, result.IFrameWidth);
            Assert.AreEqual(WebBackupHandheldInstructionValue, result.WebBackupInstruction);
            Assert.AreEqual(WebBackupHandheldErrorValue, result.WebBackupError);
            Assert.AreEqual(TestTimeZone, result.TimeZone);

            MockTimeZoneRepository
                .Verify(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()), Times.Once);

            MockJwtRepository
                .Verify(r => r.Decrypt<WebBackupJwtModel>(It.Is<string>(t => t == testTokenContent)), Times.Once);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupHandheldDevice(It.IsAny<Guid>()), Times.Once);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupTabletDevice(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public async Task GetModelTabletTest()
        {
            var testPatientCultureCode = "en-US";
            var testTokenContent = "Test Token Content";
            var testToken = HttpUtility.HtmlEncode(testTokenContent);
            var testPatientId = Guid.NewGuid();
            var testCaregiverId = Guid.NewGuid();
            var testVisitId = Guid.NewGuid();

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
                PatientId = testPatientId,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            var testTokenModel = new WebBackupJwtModel
            {
                SiteId = testSite.Id,
                WebBackupType = WebBackupType.TabletPatient,
                PatientId = testPatientId,
                ExpirationDate = testSite.WebBackupExpireDate.Value,
                VisitId = testVisitId,
                CaregiverId = testCaregiverId,
                CultureName = testPatientCultureCode
            };

            var testPatient = new Patient
            {
                Id = testPatientId,
                LanguageId = Languages.English.Id
            };

            var patientDataset = new FakeDbSet<Patient>(new List<Patient> { testPatient });
            MockContext.Setup(c => c.Patients).Returns(patientDataset.Object);

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == testTokenModel.SiteId)))
                .ReturnsAsync(testSite);

            MockJwtRepository
                .Setup(r => r.Decrypt<WebBackupJwtModel>(It.Is<string>(t => t == testTokenContent)))
                .Returns(testTokenModel);

            MockTimeZoneRepository
                .Setup(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()))
                .ReturnsAsync(TestTimeZone);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupTabletDevice(It.Is<Guid>(sid => sid == testTokenModel.SiteId)))
                .Returns(testDevice);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupHandheldPublicKeyName)))
                .Returns(TestKeyValue);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName)))
                .Returns(TestKeyValue);

            var repository = GetRepository();

            var result = await repository.GetSubjectWebBackupModel(
                testToken,
                TestHostAddress);

            Assert.IsTrue(result.CanDoWebBackup);
            Assert.IsTrue(result.Title.Contains(TestSiteName));
       
            Assert.AreEqual(ExpectedTabletHeight, result.IFrameHeight);
            Assert.AreEqual(ExpectedTabletWidth, result.IFrameWidth);
            Assert.AreEqual(WebBackupHandheldInstructionValue, result.WebBackupInstruction);
            Assert.AreEqual(WebBackupHandheldErrorValue, result.WebBackupError);
            Assert.AreEqual(TestTimeZone, result.TimeZone);

            MockTimeZoneRepository
                .Verify(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()), Times.Once);

            MockJwtRepository
                .Verify(r => r.Decrypt<WebBackupJwtModel>(It.Is<string>(t => t == testTokenContent)), Times.Once);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupHandheldDevice(It.IsAny<Guid>()), Times.Never);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupTabletDevice(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task GetModelNotEligibleTest()
        {
            var testPatientCultureCode = "en-US";
            var testTokenContent = "Test Token Content";
            var testToken = HttpUtility.HtmlEncode(testTokenContent);
            var testPatientId = Guid.NewGuid();
            var testCaregiverId = Guid.NewGuid();
            var testVisitId = Guid.NewGuid();

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
                PatientId = testPatientId,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            var testTokenModel = new WebBackupJwtModel
            {
                SiteId = testSite.Id,
                WebBackupType = WebBackupType.HandheldPatient,
                PatientId = testPatientId,
                ExpirationDate = testSite.WebBackupExpireDate.Value,
                VisitId = testVisitId,
                CaregiverId = testCaregiverId,
                CultureName = testPatientCultureCode
            };

            var testPatient = new Patient
            {
                Id = testPatientId,
                LanguageId = Languages.English.Id
            };

            var patientDataset = new FakeDbSet<Patient>(new List<Patient> { testPatient });
            MockContext.Setup(c => c.Patients).Returns(patientDataset.Object);

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == testTokenModel.SiteId)))
                .ReturnsAsync(testSite);

            MockJwtRepository
                .Setup(r => r.Decrypt<WebBackupJwtModel>(It.Is<string>(t => t == testTokenContent)))
                .Returns(testTokenModel);

            MockTimeZoneRepository
                .Setup(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()))
                .ReturnsAsync(TestTimeZone);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupHandheldDevice(It.Is<Guid>(pid => pid == testTokenModel.PatientId)))
                .Returns(testDevice);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupHandheldPublicKeyName)))
                .Returns(TestKeyValue);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName)))
                .Returns(TestKeyValue);

            var repository = GetRepository();

            var result = await repository.GetSubjectWebBackupModel(
                testToken,
                TestHostAddress);

            Assert.IsFalse(result.CanDoWebBackup);
            Assert.IsTrue(result.Title.Contains(TestSiteName));
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.Url));
            Assert.AreEqual(ExpectedHandheldHeight, result.IFrameHeight);
            Assert.AreEqual(ExpectedHandheldWidth, result.IFrameWidth);
            Assert.AreEqual(WebBackupHandheldInstructionValue, result.WebBackupInstruction);
            Assert.AreEqual(WebBackupHandheldErrorValue, result.WebBackupError);
            Assert.AreEqual(TestTimeZone, result.TimeZone);

            MockTimeZoneRepository
                .Verify(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()), Times.Once);

            MockJwtRepository
                .Verify(r => r.Decrypt<WebBackupJwtModel>(It.Is<string>(t => t == testTokenContent)), Times.Once);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupHandheldDevice(It.IsAny<Guid>()), Times.Once);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupTabletDevice(It.IsAny<Guid>()), Times.Never);

            MockSystemSettingRepository
                .Verify(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupHandheldPublicKeyName)), Times.Once);

            MockSystemSettingRepository
                .Verify(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName)), Times.Never);
        }

        [TestMethod]
        public void GetModelHandheldNoTimeZoneLookupTest()
        {
            var testPatientCultureCode = "en-US";
            var testTokenContent = "Test Token Content";
            var testToken = HttpUtility.HtmlEncode(testTokenContent);
            var testPatientId = Guid.NewGuid();
            var testCaregiverId = Guid.NewGuid();
            var testVisitId = Guid.NewGuid();

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
                PatientId = testPatientId,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            var testTokenModel = new WebBackupJwtModel
            {
                SiteId = testSite.Id,
                WebBackupType = WebBackupType.HandheldPatient,
                PatientId = testPatientId,
                ExpirationDate = testSite.WebBackupExpireDate.Value,
                VisitId = testVisitId,
                CultureName = testPatientCultureCode,
                CaregiverId = testCaregiverId
            };

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == testTokenModel.SiteId)))
                .ReturnsAsync(testSite);

            MockJwtRepository
                .Setup(r => r.Decrypt<WebBackupJwtModel>(It.Is<string>(t => t == testTokenContent)))
                .Returns(testTokenModel);

            MockTimeZoneRepository
                .Setup(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()))
                .ReturnsAsync(TestTimeZone);

            MockDeviceRepository
                .Setup(r => r.GetWebBackupHandheldDevice(It.Is<Guid>(pid => pid == testTokenModel.PatientId)))
                .Returns(testDevice);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupHandheldPublicKeyName)))
                .Returns(TestKeyValue);

            var repository = GetRepository();

            var result = repository.GetSubjectWebBackupModel(
                testToken,
                string.Empty);

            MockTimeZoneRepository
                .Verify(r => r.GetTimeZoneIdWithDefault(
                    It.Is<string>(ip => ip == TestHostAddress),
                    It.IsAny<string>()), Times.Never);
        }
    }
}