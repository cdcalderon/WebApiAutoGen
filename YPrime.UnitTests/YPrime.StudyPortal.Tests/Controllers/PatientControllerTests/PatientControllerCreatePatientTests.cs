using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessRule.Entities;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.PatientControllerTests
{
    [TestClass]
    public class PatientControllerCreatePatientTests : PatientControllerTestBase
    {
        [TestMethod]
        public async Task WhenCalled_WillReturnRouteResultToIndex()
        {
            var patientId = Guid.NewGuid();
            SetUpInsertPatient(patientId);
            var _Dto = new PatientDto
            {
                Id = patientId,
                SiteId = InitialSiteId,
                PatientNumber = "1001",
                LanguageId = Guid.NewGuid(),
                PatientAttributes = new List<PatientAttributeDto>()
            };
            var result = await Controller.Create(_Dto);
            var routeResult = result as RedirectToRouteResult;

            Assert.IsNotNull(routeResult);
            Assert.IsTrue(routeResult.RouteValues.ContainsValue("Index"));
            Assert.IsTrue(routeResult.RouteValues.ContainsKey("action"));
        }

        [TestMethod]
        public async Task WhenBYODDeviceSelected_WillReturnRouteResultToIndex()
        {
            var patientId = Guid.NewGuid();
            SetUpInsertPatient(patientId);
            var _Dto = new PatientDto
            {
                Id = patientId,
                SiteId = InitialSiteId,
                PatientNumber = "1001",
                LanguageId = Guid.NewGuid(),
                PatientAttributes = new List<PatientAttributeDto>(), 
                SubjectUsePersonalDevice = true
            };
            var result = await Controller.Create(_Dto);
            var routeResult = result as RedirectToRouteResult;

            Assert.IsNotNull(routeResult);
            Assert.IsTrue(routeResult.RouteValues.ContainsValue("Index"));
            Assert.IsTrue(routeResult.RouteValues.ContainsKey("action"));
        }

        [TestMethod]
        public void WhenBussinessAttributesAreFalsed_WillReturnSameAttributes()
        {
            var execResult = new BusinessRuleResult
            {
                ExecutionResult = false
            };

            var patientId = Guid.NewGuid();
            SetUpInsertPatient(patientId);
            var _Dto = new PatientDto
            {
                Id = patientId,
                SiteId = InitialSiteId,
                PatientNumber = "1001",
                LanguageId = Guid.NewGuid(),
                PatientAttributes = new List<PatientAttributeDto>() { new PatientAttributeDto { SubjectInformation = new Core.BusinessLayer.Models.SubjectInformationModel { BusinessRuleId = Guid.NewGuid(), BusinessRuleTrueFalseIndicator = true } } },
                SubjectUsePersonalDevice = true
            };

            RuleService.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<bool?>(), It.IsAny<DateTime?>()))
              .Returns(execResult);

            var result = Controller.GetPatientAttributesToRemovedBasedOnRuleEngine(_Dto);
            Assert.AreEqual(1, result.Count);
        }


        [TestMethod]
        public void WhenBussinessAttributesAreFalsed_WillReturnSameNullAttributes()
        {
            var patientId = Guid.NewGuid();
            SetUpInsertPatient(patientId);
            var _Dto = new PatientDto
            {
                Id = patientId,
                SiteId = InitialSiteId,
                PatientNumber = "1001",
                LanguageId = Guid.NewGuid(),
                PatientAttributes = new List<PatientAttributeDto>() { new PatientAttributeDto { SubjectInformation = new Core.BusinessLayer.Models.SubjectInformationModel { BusinessRuleTrueFalseIndicator = null } } },
                SubjectUsePersonalDevice = true
            };

           var result= Controller.GetPatientAttributesToRemovedBasedOnRuleEngine(_Dto);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task WhenBYODDeviceNotSelected_WillReturnRouteResultToIndex()
        {
            var patientId = Guid.NewGuid();
            SetUpInsertPatient(patientId);
            var _Dto = new PatientDto
            {
                Id = patientId,
                SiteId = InitialSiteId,
                PatientNumber = "1001",
                LanguageId = Guid.NewGuid(),
                PatientAttributes = new List<PatientAttributeDto>(),
                SubjectUsePersonalDevice = false
            };
            var result = await Controller.Create(_Dto);
            var routeResult = result as RedirectToRouteResult;

            Assert.IsNotNull(routeResult);
            Assert.IsTrue(routeResult.RouteValues.ContainsValue("Index"));
            Assert.IsTrue(routeResult.RouteValues.ContainsKey("action"));
        }

        [TestMethod]
        public async Task WhenPatientNumberdIsNotValid_WillReturnViewResultToCreatePage()
        {
            var patientId = Guid.NewGuid();
            SetUpInsertPatient(patientId);
            var _Dto = new PatientDto
            {
                Id = patientId,
                SiteId = InitialSiteId,
                PatientNumber = "",
                LanguageId = Guid.NewGuid(),
                PatientAttributes = new List<PatientAttributeDto>()
            };
            var result = await Controller.Create(_Dto);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);

            var viewResult = result as ViewResult;
            Assert.IsTrue(viewResult.Model is PatientDto);
        }


        [TestMethod]
        public async Task WhenLanguageIdIsNotValid_WillReturnViewResultToCreatePage()
        {
            var patientId = Guid.NewGuid();
            SetUpInsertPatient(patientId);
            var _Dto = new PatientDto
            {
                Id = patientId,
                SiteId = InitialSiteId,
                PatientNumber = "",
                LanguageId = null,
                PatientAttributes = new List<PatientAttributeDto>()
            };
            var result = await Controller.Create(_Dto);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);

            var viewResult = result as ViewResult;
            Assert.IsTrue(viewResult.Model is PatientDto);
        }

        [TestMethod]
        public async Task WhenInsertPatientFails_WillReturnViewResultToCreatePage()
        {
            var patientId = Guid.NewGuid();
            SetUpInsertPatient(patientId, false);
            var _Dto = new PatientDto
            {
                Id = patientId,
                SiteId = InitialSiteId,
                PatientNumber = "",
                LanguageId = Guid.NewGuid(),
                PatientAttributes = new List<PatientAttributeDto>()
            };
            var result = await Controller.Create(_Dto);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);

            var viewResult = result as ViewResult;
            Assert.IsTrue(viewResult.Model is PatientDto);
        }
    }
}