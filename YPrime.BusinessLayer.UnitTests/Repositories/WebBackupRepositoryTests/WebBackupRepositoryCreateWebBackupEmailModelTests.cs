using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Config.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.BusinessLayer.UnitTests.Repositories.WebBackupRepositoryTests
{
    [TestClass]
    public class WebBackupRepositoryCreateWebBackupEmailModelTests : WebBackupRepositoryTestBase
    {
        [TestMethod]
        public async Task CreateWebBackupEmailModelTest()
        {
            var testPatientId = Guid.NewGuid();
            var testSiteId = Guid.NewGuid();
            Guid testVisitId = Guid.NewGuid();
            var testDaysOpen = 2;
            var testSubject = "Email subject";
            var testBody = "Email body";

            var testSite = new SiteDto
            {
                Id = testSiteId,
                TimeZone = TestUtcTimeZone,
                WebBackupExpireDate = DateTime.Now.AddDays(7),
                Name = TestSiteName
            };

            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                SiteId = testSiteId,
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
                .Setup(r => r.GetWebBackupHandheldValue())
                .ReturnsAsync(testDaysOpen);

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(sid => sid == testSite.Id)))
                .ReturnsAsync(testSite);

            MockConfirmationRepository
                .Setup(r => r.GenerateEmailContent(
                    It.Is<Guid>(id => id == EmailTypes.SubjectHandheldWebBackup),
                    It.IsAny<Dictionary<string, string>>()))
                .Returns(new Dictionary<string, string>
                {
                    {"SUBJECT", testSubject},
                    {"BODY", testBody}
                });

            var testSubjectReplaced = $"Replaced {testSubject}";

            MockTranslationService
                .Setup(t => t.GetByKey(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(testSubjectReplaced);

            var language = new LanguageModel()
            {
                CultureName = "en-US",
                Id = new Guid()
            };

            MockLanguageService
                .Setup(r => r.Get(It.Is<Guid>(sid => sid == language.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(language);

            var repository = GetRepository();

            var result = await repository.CreateWebBackupEmailModel(
                testPatientId,
                testSiteId,
                WebBackupType.TabletPatient,
                testVisitId);

            Assert.AreEqual(testSubjectReplaced, result.Subject);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.EmailContent));
            Assert.AreEqual(EmailTypes.SubjectHandheldWebBackup, result.EmailContentId);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupHandheldDevice(It.IsAny<Guid>()), Times.Never);

            MockDeviceRepository
                .Verify(r => r.GetWebBackupTabletDevice(It.IsAny<Guid>()), Times.Once);

            MockJwtRepository
                .Verify(r => r.Encrypt(It.IsAny<WebBackupJwtModel>()), Times.Never);
        }
    }
}