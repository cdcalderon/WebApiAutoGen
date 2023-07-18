using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.Auth.Data.Models.JSON;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Controllers;
using YPrime.StudyPortal.Models;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.WebBackupControllerTests
{
    public abstract class VisitManagementControllerTestBase : BaseControllerTest
    {
        protected Mock<IConfirmationRepository> ConfirmationRepository;

        protected PatientVisitController Controller;
        protected Mock<ISessionService> SessionService;
        protected Mock<ICareGiverTypeService> CareGiverTypeService;
        protected Mock<IPatientVisitRepository> PatientVisitRepository;
        protected Mock<IPatientRepository> PatientRepository;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _yprimeSession.CurrentUser = new StudyUserDto
            {
                Id = Guid.NewGuid(),
                Roles = new List<Core.BusinessLayer.Models.StudyRoleModel>
                {
                    new Core.BusinessLayer.Models.StudyRoleModel
                    {
                        ShortName = "YP"
                    }
                }
            };

            base.Initialize();
            ConfirmationRepository = new Mock<IConfirmationRepository>();            
            SessionService = new Mock<ISessionService>();
            CareGiverTypeService = new Mock<ICareGiverTypeService>();
            PatientVisitRepository = new Mock<IPatientVisitRepository>();
            PatientRepository = new Mock<IPatientRepository>();

            Controller = new PatientVisitController(
                
                PatientVisitRepository.Object,             
                SessionService.Object,
                CareGiverTypeService.Object)
            {
                ControllerContext = (new Mock<ControllerContext>()).Object
            };
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {

            var siteDto = new SiteDto
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                SiteNumber = "10001",
                Name = "10001",
                Address1 = "test",
                Address2 = "test",
                Address3 = "test",
                TimeZone = "test",
                Notes = "test",
                PatientDOBFormatId = 0,
                City = "test",
                State = "test",
                Zip = "test",
                CountryId = Guid.Empty,
                CountryName = "test",
                PrimaryContact = "test",
                PhoneNumber = "test",
                FaxNumber = "test",
                IsActive = true,
                LastUpdate = new DateTime()
            };

            var siteList = new List<SiteDto>();
            siteList.Add(siteDto);

            var Patient = new Patient() { 
                Id = Guid.NewGuid(),
                PatientNumber = "1"
            };

            var currentPatientVisitGuid = Guid.NewGuid();

            Guid VisitId = Guid.NewGuid();

            PatientVisitRepository.Setup(repo => repo.ActivatePatientVisit(VisitId, Patient.Id));

        }
    }
}