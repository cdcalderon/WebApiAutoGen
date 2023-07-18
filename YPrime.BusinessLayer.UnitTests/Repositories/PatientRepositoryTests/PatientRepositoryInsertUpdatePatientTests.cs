using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryInsertUpdatePatientTests : PatientRepositoryTestBase
    {
        [TestMethod]
        public async Task PatientRepositoryInsertUpdatePatientInsertTest()
        {
            const string testPatientNumber = "901";

            var testPatient1 = new Patient
                                   {
                                       Id = Guid.NewGuid(),
                                       PatientNumber = "S-1001-123456",
                                       IsHandheldTrainingComplete = true,
                                       IsTabletTrainingComplete = true,
                                       IsTempPin = false,
                                       EnrolledDate = DateTime.Now.AddDays(-10).Date,
                                       PatientStatusTypeId = BasePatientStatuses.First().Id,
                                       SiteId = BaseSite.Id,
                                       Site = BaseSite,
                                       DiaryEntries = new List<DiaryEntry>(),
                                       PatientVisits = new List<PatientVisit>()
                                   };

            BasePatients = new List<Patient> { testPatient1 };

            PatientDataset = new FakeDbSet<Patient>(BasePatients);
            MockContext.Setup(c => c.Patients).Returns(PatientDataset.Object);

            var testPatient = new PatientDto
            {
                Id = BasePatients.First().Id,
                PatientNumber = testPatientNumber,
                SiteId = BaseSites.First().Id,
                PatientAttributes = new List<PatientAttributeDto>
                {
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.InsertUpdatePatient(testPatient, false, modelState);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(0, modelState.Count);

            MockContext.ResetCalls();
        }

        [TestMethod]
        public async Task PatientRepositoryInsertUpdatePatientInsertBadDateTest()
        {
            const string testPatientNumber = "901";

            var dateAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                AttributeValue = "02/30/1990"
            };

            var expectedErrorMessage = $"{dateAttributeDto.Id}-InvalidDateFormat";

            var testPatient = new PatientDto
            {
                PatientNumber = testPatientNumber,
                SiteId = BaseSites.First().Id,
                PatientAttributes = new List<PatientAttributeDto>
                {
                    new PatientAttributeDto
                    {
                        Id = Guid.NewGuid(),
                        PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                        AttributeValue = "fdsaoiurelsdkajfcdslkaf"
                    },
                    dateAttributeDto
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.InsertUpdatePatient(testPatient, true, modelState);

            Assert.IsFalse(result.Success);

            MockContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Never);
            MockContext.ResetCalls();
        }
    }
}