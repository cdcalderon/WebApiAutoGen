using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using YPrime.BusinessLayer.Extensions;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class DiaryEntryExtensionsTests
    {
        [TestMethod]
        public void GetDiaryStatusTest()
        {
            var testDiaryStatus = DiaryStatus.Source;

            var testDiaryEntry = new DiaryEntry
            {
                DiaryStatusId = testDiaryStatus.Id
            };

            var result = testDiaryEntry.GetDiaryStatus();

            Assert.AreEqual(testDiaryStatus, result);
        }

        [TestMethod]
        public void GetDataSourceTest()
        {
            var testDataSource = DataSource.eCOAApp;

            var testDiaryEntry = new DiaryEntry
            {
                DataSourceId = testDataSource.Id
            };

            var result = testDiaryEntry.GetDataSource();

            Assert.AreEqual(testDataSource, result);
        }

        [TestMethod]
        public void ToDtoTest()
        {
            var testDiaryStatus = DiaryStatus.Source;
            var testDataSource = DataSource.eCOAApp;

            var testDiaryEntry = new DiaryEntry
            {
                Id = Guid.NewGuid(),
                DiaryStatusId = testDiaryStatus.Id,
                DiaryDate = DateTime.Now.AddDays(-120),
                DataSourceId= testDataSource.Id,
                PatientId = Guid.NewGuid(),
                QuestionnaireId = Guid.NewGuid(),
                VisitId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
                StartedTime = DateTime.Now.AddDays(-120).AddMinutes(5),
                CompletedTime = DateTime.Now.AddDays(-120).AddMinutes(10),
                TransmittedTime = DateTime.Now.AddDays(-120).AddMinutes(15),
                ReviewedByUserid = Guid.NewGuid(),
                ReviewedDate = DateTime.Now.AddDays(-110),
            };

            var result = testDiaryEntry.ToDto();

            Assert.AreEqual(testDiaryEntry.Id, result.Id);
            Assert.AreEqual(testDiaryEntry.DiaryStatusId, result.DiaryStatusId);
            Assert.AreEqual(testDiaryStatus.Name, result.DiaryStatusName);
            Assert.AreEqual(testDiaryEntry.DiaryDate, result.DiaryDate);
            Assert.AreEqual(testDiaryEntry.DataSourceId, result.DataSourceId);
            Assert.AreEqual(testDataSource.Name, result.DataSourceName);
            Assert.AreEqual(testDiaryEntry.PatientId, result.PatientId);
            Assert.AreEqual(testDiaryEntry.QuestionnaireId, result.QuestionnaireId);
            Assert.AreEqual(testDiaryEntry.VisitId, result.VisitId);
            Assert.AreEqual(testDiaryEntry.DeviceId, result.DeviceId);
            Assert.AreEqual(testDiaryEntry.StartedTime, result.StartedTime);
            Assert.AreEqual(testDiaryEntry.CompletedTime, result.CompletedTime);
            Assert.AreEqual(testDiaryEntry.TransmittedTime, result.TransmittedTime);
            Assert.AreEqual(testDiaryEntry.ReviewedByUserid, result.ReviewedByUserid);
            Assert.AreEqual(testDiaryEntry.ReviewedDate, result.ReviewedDate);

            Assert.IsTrue(string.IsNullOrEmpty(result.QuestionnaireName));
            Assert.IsTrue(string.IsNullOrEmpty(result.QuestionnaireDisplayName));
            Assert.IsTrue(string.IsNullOrEmpty(result.ReviewedByUserName));
            Assert.IsTrue(string.IsNullOrEmpty(result.AssetTag));
            Assert.IsTrue(string.IsNullOrEmpty(result.PatientNumber));
            Assert.IsTrue(string.IsNullOrEmpty(result.VisitName));
            Assert.AreEqual(Guid.Empty, result.SiteId);
        }

        [TestMethod]
        public void ToDtoFullyHydratedTest()
        {
            var testDiaryStatus = DiaryStatus.Modified;
            var testDataSource = DataSource.Paper;

            var testPatient = new Patient
            {
                Id = Guid.NewGuid(),
                SiteId = Guid.NewGuid(),
                PatientNumber = "S-110-654378"
            };

            var testUser = new StudyUser
            {
                Id = Guid.NewGuid(),
                UserName = "Study User ABC"
            };

            var testVisit = new VisitModel
            {
                Id = Guid.NewGuid(),
                Name = "Visit 45"
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = Guid.NewGuid(),
                InternalName = "Internal Questionnaire Name",
                DisplayName = "Questionnaire Display Name"
            };

            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                AssetTag = "XYZ-129475"
            };

            var testDiaryEntry = new DiaryEntry
            {
                Id = Guid.NewGuid(),
                DiaryStatusId = testDiaryStatus.Id,
                DiaryDate = DateTime.Now.AddDays(-120),
                DataSourceId = testDataSource.Id,
                QuestionnaireId = testQuestionnaire.Id,
                VisitId = testVisit.Id,
                DeviceId = testDevice.Id,
                Device = testDevice,
                StartedTime = DateTime.Now.AddDays(-120).AddMinutes(5),
                CompletedTime = DateTime.Now.AddDays(-120).AddMinutes(10),
                TransmittedTime = DateTime.Now.AddDays(-120).AddMinutes(15),
                ReviewedDate = DateTime.Now.AddDays(-110),
                PatientId = testPatient.Id,
                Patient = testPatient,
                ReviewedByUserid = testUser.Id,
                ReviewedByUser = testUser,
            };

            var result = testDiaryEntry.ToDto(
                testQuestionnaire,
                testVisit);

            Assert.AreEqual(testDiaryEntry.Id, result.Id);
            Assert.AreEqual(testDiaryEntry.DiaryStatusId, result.DiaryStatusId);
            Assert.AreEqual(testDiaryStatus.Name, result.DiaryStatusName);
            Assert.AreEqual(testDiaryEntry.DiaryDate, result.DiaryDate);
            Assert.AreEqual(testDiaryEntry.DataSourceId, result.DataSourceId);
            Assert.AreEqual(testDataSource.Name, result.DataSourceName);
            Assert.AreEqual(testDiaryEntry.PatientId, result.PatientId);
            Assert.AreEqual(testDiaryEntry.QuestionnaireId, result.QuestionnaireId);
            Assert.AreEqual(testDiaryEntry.VisitId, result.VisitId);
            Assert.AreEqual(testDiaryEntry.DeviceId, result.DeviceId);
            Assert.AreEqual(testDiaryEntry.StartedTime, result.StartedTime);
            Assert.AreEqual(testDiaryEntry.CompletedTime, result.CompletedTime);
            Assert.AreEqual(testDiaryEntry.TransmittedTime, result.TransmittedTime);
            Assert.AreEqual(testDiaryEntry.ReviewedByUserid, result.ReviewedByUserid);
            Assert.AreEqual(testDiaryEntry.ReviewedDate, result.ReviewedDate);

            Assert.AreEqual(testQuestionnaire.InternalName, result.QuestionnaireName);
            Assert.AreEqual(testQuestionnaire.DisplayName, result.QuestionnaireDisplayName);
            Assert.AreEqual(testUser.UserName, result.ReviewedByUserName);
            Assert.AreEqual(testDevice.AssetTag, result.AssetTag);
            Assert.AreEqual(testPatient.PatientNumber, result.PatientNumber);
            Assert.AreEqual(testVisit.Name, result.VisitName);
            Assert.AreEqual(testPatient.SiteId, result.SiteId);
        }

        [TestMethod]
        public void ToDtoEmptyDisplayNameTest()
        {
            var testDiaryStatus = DiaryStatus.Modified;
            var testDataSource = DataSource.Paper;

            var testPatient = new Patient
            {
                Id = Guid.NewGuid(),
                SiteId = Guid.NewGuid(),
                PatientNumber = "S-110-654378"
            };

            var testUser = new StudyUser
            {
                Id = Guid.NewGuid(),
                UserName = "Study User ABC"
            };

            var testVisit = new VisitModel
            {
                Id = Guid.NewGuid(),
                Name = "Visit 45"
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = Guid.NewGuid(),
                InternalName = "Internal Questionnaire Name",
                DisplayName = ""
            };

            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                AssetTag = "XYZ-129475"
            };

            var testDiaryEntry = new DiaryEntry
            {
                Id = Guid.NewGuid(),
                DiaryStatusId = testDiaryStatus.Id,
                DiaryDate = DateTime.Now.AddDays(-120),
                DataSourceId = testDataSource.Id,
                QuestionnaireId = testQuestionnaire.Id,
                VisitId = testVisit.Id,
                DeviceId = testDevice.Id,
                Device = testDevice,
                StartedTime = DateTime.Now.AddDays(-120).AddMinutes(5),
                CompletedTime = DateTime.Now.AddDays(-120).AddMinutes(10),
                TransmittedTime = DateTime.Now.AddDays(-120).AddMinutes(15),
                ReviewedDate = DateTime.Now.AddDays(-110),
                PatientId = testPatient.Id,
                Patient = testPatient,
                ReviewedByUserid = testUser.Id,
                ReviewedByUser = testUser,
            };

            var result = testDiaryEntry.ToDto(
                testQuestionnaire,
                testVisit);

            Assert.AreEqual(testDiaryEntry.Id, result.Id);
            Assert.AreEqual(testDiaryEntry.DiaryStatusId, result.DiaryStatusId);
            Assert.AreEqual(testDiaryStatus.Name, result.DiaryStatusName);
            Assert.AreEqual(testDiaryEntry.DiaryDate, result.DiaryDate);
            Assert.AreEqual(testDiaryEntry.DataSourceId, result.DataSourceId);
            Assert.AreEqual(testDataSource.Name, result.DataSourceName);
            Assert.AreEqual(testDiaryEntry.PatientId, result.PatientId);
            Assert.AreEqual(testDiaryEntry.QuestionnaireId, result.QuestionnaireId);
            Assert.AreEqual(testDiaryEntry.VisitId, result.VisitId);
            Assert.AreEqual(testDiaryEntry.DeviceId, result.DeviceId);
            Assert.AreEqual(testDiaryEntry.StartedTime, result.StartedTime);
            Assert.AreEqual(testDiaryEntry.CompletedTime, result.CompletedTime);
            Assert.AreEqual(testDiaryEntry.TransmittedTime, result.TransmittedTime);
            Assert.AreEqual(testDiaryEntry.ReviewedByUserid, result.ReviewedByUserid);
            Assert.AreEqual(testDiaryEntry.ReviewedDate, result.ReviewedDate);

            Assert.AreEqual(testQuestionnaire.InternalName, result.QuestionnaireName);
            Assert.AreEqual(testQuestionnaire.InternalName, result.QuestionnaireDisplayName);
            Assert.AreEqual(testUser.UserName, result.ReviewedByUserName);
            Assert.AreEqual(testDevice.AssetTag, result.AssetTag);
            Assert.AreEqual(testPatient.PatientNumber, result.PatientNumber);
            Assert.AreEqual(testVisit.Name, result.VisitName);
            Assert.AreEqual(testPatient.SiteId, result.SiteId);
        }
    }
}
