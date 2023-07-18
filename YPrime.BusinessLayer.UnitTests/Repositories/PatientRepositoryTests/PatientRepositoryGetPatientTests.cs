using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryGetPatientTests : PatientRepositoryTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            SetupBaseData();
        }

        [TestMethod]
        public async Task GetPatientWithPatientTest()
        {
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
                    Name = "Removed"
                }
            };
            var enrolledStatusType = statusTypes.First();

            var testPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-123-4567",
                SiteId = BaseSite.Id,
                Site = BaseSite,
                PatientStatusTypeId = enrolledStatusType.Id
            };

            var testPatientAttributes = new List<PatientAttribute>
            {
                new PatientAttribute
                {
                    Id = Guid.NewGuid(),
                    PatientId = testPatient.Id,
                    PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                    AttributeValue = "abcd"
                },
                new PatientAttribute
                {
                    Id = Guid.NewGuid(),
                    PatientId = testPatient.Id,
                    PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                    AttributeValue = DateTime.Now.ToString("dd/MM/yyyy")
                },
                new PatientAttribute
                {
                    Id = Guid.NewGuid(),
                    PatientId = testPatient.Id,
                    PatientAttributeConfigurationDetailId = NumberAttributeConfigDetail.Id,
                    AttributeValue = "123"
                },
            };

            testPatient.PatientAttributes = testPatientAttributes;

            BasePatients.Add(testPatient);

            var testPatientVisits = new List<PatientVisitDto>();

            for (var i = 0; i < BaseVisits.Count; i++)
            {
                testPatientVisits.Add(new PatientVisitDto
                {
                    Id = Guid.NewGuid(),
                    VisitId = BaseVisits[i].Id,
                    PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
                    VisitDate = DateTime.Now.AddDays(i),
                    VisitName = $"Visit {i + 1}"
                });
            }

            MockPatientStatusService
                .Setup(pvr => pvr.GetAll(null))
                .ReturnsAsync(statusTypes);

            MockPatientVisitRepository
                .Setup(pvr => pvr.GetAllPatientVisit(It.Is<Guid>(id => id == testPatient.Id), It.Is<string>(cc => cc == DefaultCultureCode)))
                .ReturnsAsync(testPatientVisits);

            var result = await Repository.GetPatient(testPatient, DefaultCultureCode);

            Assert.AreEqual(testPatient.Id, result.Id);
            Assert.IsTrue(result.Compliance);
            Assert.AreEqual(BaseSite.Name, result.SiteNumber);
            Assert.AreEqual(enrolledStatusType, result.PatientStatusType);
            Assert.AreEqual(enrolledStatusType.Name, result.PatientStatus);
            Assert.IsTrue(result.IsActive);
            Assert.IsTrue(result.IsSiteActive);
            Assert.AreEqual(testPatientVisits.Count, result.PatientVisits.Count);
            Assert.AreEqual(testPatientVisits.Last().VisitName, result.LastVisit);
            Assert.AreEqual(testPatientVisits.Last().VisitDate, result.LastVisitDate);
            Assert.AreEqual(testPatientAttributes.Count, result.PatientAttributes.Count);

            foreach (var patientAttribute in testPatientAttributes)
            {
                var matchingDto = result.PatientAttributes
                    .FirstOrDefault(pa => pa.Id == patientAttribute.Id);

                Assert.IsNotNull(matchingDto);
                Assert.AreEqual(patientAttribute.AttributeValue, matchingDto.AttributeValue);
                matchingDto.CorrectionApprovalDatas.Should().BeEmpty();
            }

            result.QuestionnairesTaken.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetPatientWithIdTest()
        {
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
                    Name = "Removed"
                }
            };
            var enrolledStatusType = statusTypes.First();

            var testPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-123-4567",
                SiteId = BaseSite.Id,
                Site = BaseSite,
                PatientStatusTypeId = enrolledStatusType.Id
            };

            var testPatientAttributes = new List<PatientAttribute>
            {
                new PatientAttribute
                {
                    Id = Guid.NewGuid(),
                    PatientId = testPatient.Id,
                    PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                    AttributeValue = "abcd"
                },
                new PatientAttribute
                {
                    Id = Guid.NewGuid(),
                    PatientId = testPatient.Id,
                    PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                    AttributeValue = DateTime.Now.ToString("dd/MM/yyyy")
                },
                new PatientAttribute
                {
                    Id = Guid.NewGuid(),
                    PatientId = testPatient.Id,
                    PatientAttributeConfigurationDetailId = NumberAttributeConfigDetail.Id,
                    AttributeValue = "123"
                },
            };

            testPatient.PatientAttributes = testPatientAttributes;

            BasePatients.Add(testPatient);

            var testPatientVisits = new List<PatientVisitDto>();

            for (var i = 0; i < BaseVisits.Count; i++)
            {
                testPatientVisits.Add(new PatientVisitDto
                {
                    Id = Guid.NewGuid(),
                    VisitId = BaseVisits[i].Id,
                    PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
                    VisitDate = DateTime.Now.AddDays(i),
                    VisitName = $"Visit {i + 1}"
                });
            }

            MockPatientStatusService
                .Setup(pvr => pvr.GetAll(null))
                .ReturnsAsync(statusTypes);

            MockPatientVisitRepository
                .Setup(pvr => pvr.GetAllPatientVisit(It.Is<Guid>(id => id == testPatient.Id), It.Is<string>(cc => cc == DefaultCultureCode)))
                .ReturnsAsync(testPatientVisits);

            var result = await Repository.GetPatient(testPatient.Id, DefaultCultureCode);

            Assert.AreEqual(testPatient.Id, result.Id);
            Assert.IsTrue(result.Compliance);
            Assert.AreEqual(BaseSite.Name, result.SiteNumber);
            Assert.AreEqual(enrolledStatusType, result.PatientStatusType);
            Assert.AreEqual(enrolledStatusType.Name, result.PatientStatus);
            Assert.IsTrue(result.IsActive);
            Assert.IsTrue(result.IsSiteActive);
            Assert.AreEqual(testPatientVisits.Count, result.PatientVisits.Count);
            Assert.AreEqual(testPatientVisits.Last().VisitName, result.LastVisit);
            Assert.AreEqual(testPatientVisits.Last().VisitDate, result.LastVisitDate);
            Assert.AreEqual(testPatientAttributes.Count, result.PatientAttributes.Count);

            foreach (var patientAttribute in testPatientAttributes)
            {
                var matchingDto = result.PatientAttributes
                    .FirstOrDefault(pa => pa.Id == patientAttribute.Id);

                Assert.IsNotNull(matchingDto);
                Assert.AreEqual(patientAttribute.AttributeValue, matchingDto.AttributeValue);
                matchingDto.CorrectionApprovalDatas.Should().BeEmpty();
            }

            result.QuestionnairesTaken.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetPatientNullTest()
        {
            var result = await Repository.GetPatient(null, DefaultCultureCode);

            Assert.IsNotNull(result);
            Assert.AreEqual(Guid.Empty, result.Id);
            result.Should().BeEquivalentTo(new PatientDto());
        }
    }
}
