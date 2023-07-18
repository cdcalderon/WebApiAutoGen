using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Constants;
using YPrime.Config.Defaults;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.BusinessLayer.UnitTests.Repositories.WebBackupRepositoryTests
{
    [TestClass]
    public class WebBackupRepositoryCreateWebBackupEmailBodyTests : WebBackupRepositoryTestBase
    {
        [TestMethod]
        public async Task WebBackupRepositoryCreateWebBackupEmailBodyTest()
        {
            var testPatientCultureCode = "en-US";
            var testTokenValue = "This is a test";
            var testSubject = "Email subject";
            var testBody = "Email body";

            var patientId = Guid.NewGuid();

            var testJwtModel = new WebBackupJwtModel
            {
                ExpirationDate = DateTime.Now.AddDays(3),
                CultureName = testPatientCultureCode,
                PatientId = patientId,
            };

            var testPatient = new Patient
            {
                Id = patientId,
                LanguageId = Languages.English.Id
            };

            var patientDataset = new FakeDbSet<Patient>(new List<Patient> { testPatient });
            MockContext.Setup(c => c.Patients).Returns(patientDataset.Object);

            MockJwtRepository
                .Setup(r => r.Encrypt(It.Is<WebBackupJwtModel>(m => m == testJwtModel)))
                .Returns(testTokenValue);

            MockConfirmationRepository
                .Setup(r => r.GenerateEmailContent(
                    It.Is<Guid>(id => id == EmailTypes.SubjectHandheldWebBackup),
                    It.IsAny<Dictionary<string, string>>()))
                .Returns(new Dictionary<string, string>
                {
                    {"SUBJECT", testSubject},
                    {"BODY", testBody}
                });

            var testBodyReplaced = $"Replaced {testBody}";

            MockTranslationService
                .Setup(t => t.GetByKey(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(testBodyReplaced);

            var mockRequest = new Mock<HttpRequestBase>();

            mockRequest.SetupGet(r => r.Url)
                .Returns(new Uri(TestUrl));

            var mockUrlHelper = new Mock<UrlHelper>();

            mockUrlHelper
                .Setup(h => h.Action(
                    It.Is<string>(an => an == ""),
                    It.Is<string>(cn => cn == ""),
                    It.IsAny<object>()))
                .Returns(TestUrl);

            var repository = GetRepository();

            var result = await repository.CreateWebBackupEmailBody(
                mockRequest.Object,
                mockUrlHelper.Object,
                testJwtModel);

            Assert.AreEqual(testBodyReplaced, result);

            MockJwtRepository
                .Verify(r => r.Encrypt(It.IsAny<WebBackupJwtModel>()), Times.Once);
        }
    }
}