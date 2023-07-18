using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Config.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using YPrime.Config.Dtos;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.BusinessLayer.UnitTests.Repositories.WebBackupRepositoryTests
{
    [TestClass]
    public class WebBackupRepositoryGetWebBackupUrlTests : WebBackupRepositoryTestBase
    {
        [TestMethod]
        public void GetWebBackupUrlHandheldTest()
        {
            var testWebBackupType = WebBackupType.HandheldPatient;
            var testPatientId = Guid.NewGuid();
            var testCaregiverId = Guid.NewGuid();
            var testVisitId = Guid.NewGuid();
            var testConfigId = Guid.NewGuid();

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
                DeviceTypeId = DeviceType.Phone.Id,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupHandheldPublicKeyName)))
                .Returns(TestKeyValue);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName)))
                .Returns(TestKeyValue);

            var repository = GetRepository();

            var result = repository.GetWebBackupUrl(
                testWebBackupType,
                testDevice.Id,
                testDevice.DeviceTypeId,
                testSite.Id,
                testDevice.AssetTag,
                testPatientId,
                TestTimeZone,
                testConfigId,
                testVisitId,
                testCaregiverId);

            Assert.IsNotNull(result);

            var paramsIndex = result.IndexOf("params=");
            var paramsEscaped = result.Substring(paramsIndex + 7);
            var paramsUnescaped = HttpUtility.UrlDecode(paramsEscaped);

            var paramResult = JsonConvert.DeserializeObject<WebBackupParams>(paramsUnescaped);

            Assert.AreEqual(testSite.Id, paramResult.SiteId);
            Assert.AreEqual(testDevice.Id, paramResult.DeviceId);
            Assert.AreEqual(testPatientId, paramResult.PatientId);
            Assert.AreEqual(TestAssetTag, paramResult.AssetTag);
            Assert.IsFalse(paramResult.BringYourOwnDevice);
            Assert.IsFalse(paramResult.SiteBased);
            Assert.AreEqual(TestTimeZone, paramResult.IanaTimeZone);
            Assert.AreEqual(testVisitId, paramResult.VisitId);
            Assert.AreEqual(testCaregiverId, paramResult.CaregiverId);
            Assert.AreEqual(testWebBackupType, paramResult.WebBackupType);
        }

        [TestMethod]
        public void GetWebBackupUrlClinicianTabletTest()
        {
            var testWebBackupType = WebBackupType.TabletClinician;
            var testPatientId = Guid.NewGuid();
            var testVisitId = Guid.NewGuid();
            var testConfigId = Guid.NewGuid();

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
                DeviceTypeId = DeviceType.Phone.Id,
                Site = new Site
                {
                    Name = TestSiteName
                }
            };

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupHandheldPublicKeyName)))
                .Returns(TestKeyValue);

            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName)))
                .Returns(TestKeyValue);

            var repository = GetRepository();

            var result = repository.GetWebBackupUrl(
                testWebBackupType,
                testDevice.Id,
                testDevice.DeviceTypeId,
                testSite.Id,
                testDevice.AssetTag,
                testPatientId,
                TestTimeZone,
                testConfigId,
                testVisitId,
                null);

            Assert.IsNotNull(result);

            var paramsIndex = result.IndexOf("params=");
            var paramsEscaped = result.Substring(paramsIndex + 7);
            var paramsUnescaped = HttpUtility.UrlDecode(paramsEscaped);

            var paramResult = JsonConvert.DeserializeObject<WebBackupParams>(paramsUnescaped);

            Assert.AreEqual(testSite.Id, paramResult.SiteId);
            Assert.AreEqual(testDevice.Id, paramResult.DeviceId);
            Assert.AreEqual(testPatientId, paramResult.PatientId);
            Assert.AreEqual(TestAssetTag, paramResult.AssetTag);
            Assert.IsFalse(paramResult.BringYourOwnDevice);
            Assert.IsTrue(paramResult.SiteBased);
            Assert.AreEqual(TestTimeZone, paramResult.IanaTimeZone);
            Assert.AreEqual(testVisitId, paramResult.VisitId);
            Assert.IsNull(paramResult.CaregiverId);
            Assert.AreEqual(testWebBackupType, paramResult.WebBackupType);
        }
    }
}
