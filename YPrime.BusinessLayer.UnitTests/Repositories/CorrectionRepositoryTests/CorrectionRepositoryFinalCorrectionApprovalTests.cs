using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using YPrime.BusinessLayer.UnitTests.TestExtensions;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositoryFinalCorrectionApprovalTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task CorrectionRepositoryFinalCorrectionApprovalPaperDcfTest()
        {
            var correctionId = Guid.NewGuid();
            var questionnaireId = Guid.NewGuid();
            var diaryDate = DateTimeOffset.Now.AddDays(-5);
            var visitId = Guid.NewGuid();

            var patientId = Guid.NewGuid();

            var passedInDiaryEntries = new List<DiaryEntry>();

            var diaryEntriesDbSet = new FakeDbSet<DiaryEntry>(new List<DiaryEntry>());

            diaryEntriesDbSet
                .Setup(e => e.Add(It.IsAny<DiaryEntry>()))
                .Callback((DiaryEntry passedInDiaryEntry) => { passedInDiaryEntries.Add(passedInDiaryEntry); });

            MockContext
                .SetupSet(db => db.CorrectionId = It.IsAny<Guid?>())
                .Verifiable();

            MockContext
                .Setup(db => db.DiaryEntries)
                .Returns(diaryEntriesDbSet.Object);

            var freeTextAnswerApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.FreeTextAnswer),
                NewDataPoint = "Test free text answer"
            };

            var choiceAnswerApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = Guid.NewGuid().ToString()
            };

            var visitIdApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(DiaryEntry),
                ColumnName = nameof(DiaryEntry.VisitId),
                NewDataPoint = visitId.ToString()
            };

            var questionnaireIdApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(DiaryEntry),
                ColumnName = nameof(DiaryEntry.QuestionnaireId),
                NewDataPoint = questionnaireId.ToString()
            };

            var diaryDateApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(DiaryEntry),
                ColumnName = nameof(DiaryEntry.DiaryDate),
                NewDataPoint = diaryDate.ToString()
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = patientId,
                StartedByUserId = Guid.NewGuid(),
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    freeTextAnswerApprovalData,
                    choiceAnswerApprovalData,
                    visitIdApprovalData,
                    questionnaireIdApprovalData,
                    diaryDateApprovalData
                }
            };

            var correctionTypes = new List<CorrectionTypeModel>
            {
                new CorrectionTypeModel
                {
                    Id = CorrectionType.PaperDiaryEntry.Id,
                    Name = "Add Paper Questionnaire"
                }
            };

            MockCorrectionTypeService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(correctionTypes);

            var testPatient = new Patient
            {
                Id = patientId
            };

            var patientDbSet = new FakeDbSet<Patient>(new List<Patient> { testPatient });

            MockContext
            .Setup(db => db.Patients)
            .Returns(patientDbSet.Object);

            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));

            await Repository.FinalCorrectionApproval(correction);

            MockContext.Verify(db => db.SaveChanges(It.IsAny<string>()), Times.Once);

            MockContext.VerifySet(db => db.CorrectionId = It.Is<Guid?>(id => id == correction.Id));

            Assert.AreEqual(1, passedInDiaryEntries.Count);

            var savedDiaryEntry = passedInDiaryEntries.First();

            Assert.AreEqual(correction.PatientId, savedDiaryEntry.PatientId);
            Assert.AreEqual(visitId, savedDiaryEntry.VisitId);
            Assert.AreEqual(questionnaireId, savedDiaryEntry.QuestionnaireId);
            Assert.AreEqual(correction.StartedByUserId, savedDiaryEntry.UserId);
            Assert.AreEqual(diaryDate.Date, savedDiaryEntry.DiaryDate.Date);
            Assert.AreEqual(DiaryStatus.Modified.Id, savedDiaryEntry.DiaryStatusId);
            Assert.AreEqual(DataSource.Paper.Id, savedDiaryEntry.DataSourceId);
            Assert.AreEqual(DateTimeOffset.Now.Date, savedDiaryEntry.StartedTime);
            Assert.AreEqual(DateTimeOffset.Now.Date, savedDiaryEntry.CompletedTime);
            Assert.That.AreCloseInSeconds(DateTimeOffset.Now, savedDiaryEntry.TransmittedTime, 5);
            Assert.That.AreCloseInSeconds(DateTimeOffset.Now, correction.CompletedDate, 5);

            Assert.AreEqual(
                correction.CorrectionApprovalDatas.Count(cad => cad.TableName == nameof(Answer)),
                savedDiaryEntry.Answers.Count);

            Assert.IsTrue(savedDiaryEntry.Answers.All(a => a.DiaryEntryId == savedDiaryEntry.Id));

            var freeTextAnswer = savedDiaryEntry.Answers.First(a => a.QuestionId == freeTextAnswerApprovalData.RowId);
            Assert.AreEqual(freeTextAnswerApprovalData.NewDataPoint, freeTextAnswer.FreeTextAnswer);
            Assert.IsNull(freeTextAnswer.ChoiceId);

            var choiceAnswer = savedDiaryEntry.Answers.First(a => a.QuestionId == choiceAnswerApprovalData.RowId);
            Assert.AreEqual(choiceAnswerApprovalData.NewDataPoint, choiceAnswer.ChoiceId.ToString());
            Assert.IsNull(choiceAnswer.FreeTextAnswer);
        }

        [TestMethod]
        public async Task FinalCorrectionApprovalSubjectInfoTest()
        {
            var correctionId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var patientAttributeConfigDetailIdFirst = Guid.NewGuid();
            var patientAttributeConfigDetailIdSecond = Guid.NewGuid();
            var patientAttributeIdSecond = Guid.NewGuid();
            var patientAttributeIdThrid = Guid.NewGuid();
            var existingSyncVersion = 9;
            var expectedSyncVersion = 10;

            var passedInPatientAttributes = new List<PatientAttribute>();

            var patientAttributeApprovalDataFirst = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(PatientAttribute),
                ColumnName = nameof(PatientAttribute.AttributeValue),
                NewDataPoint = "Test attribute",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnValue = patientAttributeConfigDetailIdFirst.ToString()
                    }
                }
            };

            var patientAttributeApprovalDataSecond = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = patientAttributeIdSecond,
                TableName = nameof(PatientAttribute),
                ColumnName = nameof(PatientAttribute.AttributeValue),
                NewDataPoint = "Test attribute 2",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnValue = patientAttributeConfigDetailIdSecond.ToString()
                    }
                }
            };

            var patientAttributeApprovalDataThird = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = patientAttributeIdThrid,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.PatientStatusTypeId),
                NewDataPoint = "3"
            };

            var existingMatchingAttribute = new PatientAttribute
            {
                Id = patientAttributeIdSecond,
                SyncVersion = existingSyncVersion,
                PatientId = patientId
            };

            var patientAttributesDbSet = new FakeDbSet<PatientAttribute>(new List<PatientAttribute> { existingMatchingAttribute } );

            patientAttributesDbSet
                .Setup(e => e.AddOrUpdate(It.IsAny<PatientAttribute[]>()))
                .Callback((PatientAttribute[] passedInAttributeParams) =>
                {
                    passedInPatientAttributes.AddRange(passedInAttributeParams);
                });

            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));

            MockContext
                .SetupSet(db => db.CorrectionId = It.IsAny<Guid?>())
                .Verifiable();

            MockContext
                .Setup(db => db.PatientAttributes)
                .Returns(patientAttributesDbSet.Object);

            var testPatient = new Patient
            {
                Id = patientId
            };

            var patientDbSet = new FakeDbSet<Patient>(new List<Patient> { testPatient });

            patientDbSet
                .Setup(p => p.AddOrUpdate(It.IsAny<Patient>()));

            MockContext
                .Setup(db => db.Patients)
                .Returns(patientDbSet.Object);

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = patientId,
                ConfigurationId = configId,
                StartedByUserId = Guid.NewGuid(),
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    patientAttributeApprovalDataFirst,
                    patientAttributeApprovalDataSecond,
                    patientAttributeApprovalDataThird
                }
            };

            var correctionTypes = new List<CorrectionTypeModel>
            {
                new CorrectionTypeModel
                {
                    Id = CorrectionType.ChangeSubjectInfo.Id,
                    Name = "Change Subject Info"
                }
            };

            MockCorrectionTypeService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(correctionTypes);

            await Repository.FinalCorrectionApproval(correction);

            Assert.AreEqual(2, passedInPatientAttributes.Count);

            Assert.AreEqual(
                correction.CorrectionApprovalDatas.Count(cad => cad.TableName == nameof(PatientAttribute)),
                passedInPatientAttributes.Count);

            var firstAttribute = passedInPatientAttributes.First();

            Assert.AreEqual(correction.PatientId, firstAttribute.PatientId);
            Assert.AreEqual(patientAttributeConfigDetailIdFirst, firstAttribute.PatientAttributeConfigurationDetailId);

            Assert.AreEqual(patientAttributeApprovalDataFirst.NewDataPoint, firstAttribute.AttributeValue);
            Assert.AreEqual(1, firstAttribute.SyncVersion);
            Assert.IsTrue(firstAttribute.Id != Guid.Empty);

            var secondAttribute = passedInPatientAttributes.Last();

            Assert.AreEqual(correction.PatientId, secondAttribute.PatientId);
            Assert.AreEqual(patientAttributeConfigDetailIdSecond, secondAttribute.PatientAttributeConfigurationDetailId);

            Assert.AreEqual(patientAttributeApprovalDataSecond.NewDataPoint, secondAttribute.AttributeValue);
            Assert.AreEqual(expectedSyncVersion, secondAttribute.SyncVersion);
            Assert.IsTrue(secondAttribute.Id == patientAttributeIdSecond);

            MockContext.VerifySet(db => db.CorrectionId = It.Is<Guid?>(id => id == correction.Id));

            patientAttributesDbSet
                .Verify(
                    d => d.AddOrUpdate(It.IsAny<PatientAttribute[]>()),
                    Times.Exactly(2));

            patientDbSet
                .Verify(
                    p => p.AddOrUpdate(It.IsAny<Patient[]>()),
                    Times.Once);

            MockContext
                .Verify(
                    c => c.SaveChanges(It.IsAny<string>()),
                    Times.Once);

            MockPatientRepository.Verify( n => n.UpdateNotificationScheduleForPatient(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()));
        }
    }
}