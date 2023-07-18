using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryGetDuplicatePatientsByIdTests : PatientRepositoryTestBase
    {
        private const string DefaultDateFormat = "dd-MMM-yyyy";

        [TestInitialize]
        public void TestInitialize()
        {
            SetupBaseData();
        }

        [TestMethod]
        public async Task GetDuplicatePatientsByIdTest()
        {
            var patientAId = Guid.NewGuid();
            var patientBId = Guid.NewGuid();

            var statusTypes = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Enrolled",
                    IsActive = true
                },
                new PatientStatusModel
                {
                    Id = 3,
                    Name = "Screened"
                }
            };

            var patientAStatusType = statusTypes.First(s => s.Id == 2);
            var patientBStatusType = statusTypes.First(s => s.Id == 3);

            var patientA = new Patient
            {
                Id = patientAId,
                PatientNumber = "S-100001-9987",
                PatientStatusTypeId = patientAStatusType.Id,
                PatientVisits = new List<PatientVisit>
                {
                    new PatientVisit
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientAId,
                        VisitId = BaseVisits.First().Id,
                        VisitDate = DateTime.Now.AddDays(-5)
                    }
                },
                PatientAttributes = new List<PatientAttribute>
                {
                    new PatientAttribute
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientAId,
                        PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                        AttributeValue = "ZYXW",
                    },
                    new PatientAttribute
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientAId,
                        PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                        AttributeValue = "19/September/2019",
                    },
                    new PatientAttribute
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientAId,
                        PatientAttributeConfigurationDetailId = NumberAttributeConfigDetail.Id,
                        AttributeValue = "15",
                    }
                },
                ConfigurationId = Guid.NewGuid()
            };

            BasePatients.Add(patientA);

            var patientB = new Patient
            {
                Id = patientBId,
                PatientNumber = "S-100001-9987",
                PatientStatusTypeId = patientBStatusType.Id,
                PatientAttributes = new List<PatientAttribute>
                {
                    new PatientAttribute
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientBId,
                        PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                        AttributeValue = "ABCD",
                    },
                    new PatientAttribute
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientBId,
                        PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                        AttributeValue = "30/September/2019",
                    },
                    new PatientAttribute
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patientBId,
                        PatientAttributeConfigurationDetailId = NumberAttributeConfigDetail.Id,
                        AttributeValue = "13",
                    }
                },
                ConfigurationId = Guid.NewGuid()
            };

            BasePatients.Add(patientB);

            MockPatientStatusService
                .Setup(pvr => pvr.GetAll(null))
                .ReturnsAsync(statusTypes);

            var results = await Repository.GetDuplicatePatientsById(
                patientAId,
                DefaultCultureCode,
                DefaultDateFormat);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            var patientAResult = results.First(r => r.PatientId == patientAId);
            var patientBResult = results.First(r => r.PatientId == patientBId);

            Assert.AreEqual(patientA.Id, patientAResult.PatientId);
            Assert.AreEqual(patientA.PatientNumber, patientAResult.PatientNumber);
            Assert.AreEqual(patientA.PatientStatusTypeId, patientAResult.PatientStatusTypeId);
            Assert.AreEqual(patientAStatusType.Name, patientAResult.PatientStatus);
            Assert.AreEqual(0, patientAResult.DiaryEntries.Count);
            Assert.AreEqual(1, patientAResult.PatientVisits.Count);
            Assert.IsTrue(patientAResult.PatientVisits.All(pv => !string.IsNullOrEmpty(pv.VisitName)));
            Assert.AreEqual(patientA.PatientAttributes.Count, patientAResult.PatientAttributes.Count);

            Assert.AreEqual(patientB.Id, patientBResult.PatientId);
            Assert.AreEqual(patientB.PatientNumber, patientBResult.PatientNumber);
            Assert.AreEqual(patientB.PatientStatusTypeId, patientBResult.PatientStatusTypeId);
            Assert.AreEqual(patientBStatusType.Name, patientBResult.PatientStatus);
            Assert.AreEqual(0, patientBResult.DiaryEntries.Count);
            Assert.AreEqual(0, patientBResult.PatientVisits.Count);
            Assert.AreEqual(patientB.PatientAttributes.Count, patientBResult.PatientAttributes.Count);

            Assert.AreNotEqual(patientAResult.Position, patientBResult.Position);

            MockQuestionnaireService
                .Verify(s => s.GetAllInflatedQuestionnaires(It.IsAny<Guid?>(), It.IsAny<Guid?>()), Times.Once);

            MockSubjectInformationService
                .Verify(s => s.GetAll(It.Is<Guid?>(cid => cid == patientA.ConfigurationId)), Times.Once);

            MockSubjectInformationService
                .Verify(s => s.GetAll(It.Is<Guid?>(cid => cid == patientB.ConfigurationId)), Times.Once);
        }
    }
}
