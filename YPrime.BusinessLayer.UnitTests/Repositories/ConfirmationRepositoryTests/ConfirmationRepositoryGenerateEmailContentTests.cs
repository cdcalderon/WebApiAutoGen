using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.ConfirmationRepositoryTests
{
    [TestClass]
    public class ConfirmationRepositoryGenerateEmailContentTests : ConfirmationRepositoryTestBase
    {
        private const string Environment = "Test Env";
        private const string InventoryEnvironment = "Test Env-Product";
        private const string BaseUrl = "Test Base Url";
        private FakeDbSet<EmailContent> _dataSet;

        private Guid _emailTypeID;
        private EmailContent _testEmailContent;

        [TestInitialize]
        public void TestInitialize()
        {
            Config.Setup(config => config.YPrimeEnvironment)
                .Returns(Environment);
            Config.Setup(config => config.YPrimeInventoryEnvironment)
                .Returns(InventoryEnvironment);
            Config.Setup(config => config.StudyApiBaseUrl)
                .Returns(BaseUrl);
            _emailTypeID = Guid.Parse("0BDED019-6F77-4F06-8617-3D396C2906DA");
            _testEmailContent = new EmailContent
            {
                Id = Guid.Parse("0BDED019-6F77-4F06-8617-3D396C2906DA"),
                Name = "Web backup subject e-mail",
                TranslationKey = null, IsBlinded = false,
                Notes = null, LastUpdate = null,
                BodyTemplate =
                    "To access Web Backup, click the hyperlink below.  This link will expire on <=ExpireDate=>.%0A %0A <=EncryptedURL=>",
                SubjectLineTemplate = "<=StudyName=> Web Backup URL",
                IsEmailSentToPerformingUser = false,
                EmailContentTypeId = Guid.Parse("8F9502CA-CADA-4713-881C-B327DAAADB5F"),
                DisplayOnScreen = false, IsSiteSpecific = false, PatientStatusTypeId = null
            };


            SetupContext(new[] {_testEmailContent});
        }

        private void SetupContext(IEnumerable<EmailContent> items)
        {
            _dataSet = new FakeDbSet<EmailContent>(items);
            Context.Setup(ctx => ctx.EmailContents)
                .Returns(_dataSet.Object);
        }

        [TestMethod]
        public void WhenCalledWithCorrectTestData()
        {
            string formattedExpireDate = DateTime.Now.AddDays(3).ToString("dd-MMM-yyyy");
            string WebBackupURL = "URL";

            var testData = new Dictionary<string, string>
            {
                {"ExpireDate", formattedExpireDate},
                {"EncryptedURL", WebBackupURL},
                {"StudyName", "Example Study"}
            };
            Dictionary<string, string> testResult = Repository.GenerateEmailContent(_emailTypeID, testData);
            Equals(testResult["SUBJECT"], $"{testData["StudyName"]} Web Backup URL");
            Equals(testResult["BODY"],
                $"To access Web Backup, click the hyperlink below.  This link will expire on {testData["ExpireDate"]} .%0A %0A {testData["EncryptedURL"]}");
        }

        [TestMethod]
        public void WhenCalledWithMissingTemplateTestData()
        {
            string formattedExpireDate = DateTime.Now.AddDays(3).ToString("dd-MMM-yyyy");
            string WebBackupURL = "URL";

            var testData = new Dictionary<string, string>
            {
                {"ExpireDate", formattedExpireDate},
                {"EncryptedURL", WebBackupURL}
            };
            Dictionary<string, string> testResult = Repository.GenerateEmailContent(_emailTypeID, testData);
            Equals(testResult["SUBJECT"], "<=StudyName=> Web Backup URL");
            Equals(testResult["BODY"],
                $"To access Web Backup, click the hyperlink below.  This link will expire on {testData["ExpireDate"]} .%0A %0A {testData["EncryptedURL"]}");
        }

        [TestMethod]
        public void WhenCalledWithMissingBodyTestData()
        {
            var testData = new Dictionary<string, string>
            {
                {"StudyName", "Example Study"}
            };
            Dictionary<string, string> testResult = Repository.GenerateEmailContent(_emailTypeID, testData);
            Equals(testResult["SUBJECT"], $"{testData["StudyName"]} Web Backup URL");
            Equals(testResult["BODY"],
                "To access Web Backup, click the hyperlink below.  This link will expire on <=ExpireDate=>.%0A %0A <=EncryptedURL=>");
        }

        [TestMethod]
        public void WhenCalledWithEmptyDictionary()
        {
            var testData = new Dictionary<string, string>();
            Dictionary<string, string> testResult = Repository.GenerateEmailContent(_emailTypeID, testData);
            Equals(testResult["SUBJECT"], "<=StudyName=> Web Backup URL");
            Equals(testResult["BODY"],
                "To access Web Backup, click the hyperlink below.  This link will expire on <=ExpireDate=>.%0A %0A <=EncryptedURL=>");
        }

        [TestMethod]
        public void WhenCalledWithNoGUID()
        {
            string formattedExpireDate = DateTime.Now.AddDays(3).ToString("dd-MMM-yyyy");
            string WebBackupURL = "URL";

            var testData = new Dictionary<string, string>
            {
                {"ExpireDate", formattedExpireDate},
                {"EncryptedURL", WebBackupURL},
                {"StudyName", "Example Study"}
            };
            Assert.ThrowsException<InvalidOperationException>(() =>
                Repository.GenerateEmailContent(Guid.Empty, testData));
        }
    }
}