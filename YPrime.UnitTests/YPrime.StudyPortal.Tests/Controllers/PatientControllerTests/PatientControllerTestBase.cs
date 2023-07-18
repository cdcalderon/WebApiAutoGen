using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using YPrime.Auth.Data.Models.JSON;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Responses;
using YPrime.BusinessRule.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Controllers;
using YPrime.StudyPortal.Models;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.PatientControllerTests
{
    public abstract class PatientControllerTestBase : BaseControllerTest
    {
        protected Mock<IConfirmationRepository> ConfirmationRepository;

        protected PatientController Controller;
        protected Mock<IDeviceRepository> DeviceRepository;
        protected Mock<ISiteRepository> SiteRepository;
        protected Mock<IPatientRepository> PatientRepository;
        protected Mock<IDiaryEntryRepository> DiaryEntryRepository;
        protected Mock<IReportRepository> ReportRepository;
        protected Mock<ISessionService> SessionService;
        protected Mock<ITranslationService> TranslationService;
        protected Mock<ILanguageService> LanguageService;
        protected Mock<ISubjectInformationService> SubjectInformationService;
        protected Mock<IPatientStatusService> PatientStatusService;
        protected Mock<IStudySettingService> StudySettingService;
        protected Mock<IWebBackupRepository> WebBackUpRepository;
        protected Mock<IPatientForEditAdapter> PatientForEditAdapter;
        protected Mock<IRuleService> RuleService;
        protected Mock<IServiceSettings> MockServiceSettings;
        protected Mock<IAuthService> AuthService;
        protected Guid InitialSiteId = Guid.NewGuid();
        protected Guid PatientId = Guid.NewGuid();
        protected Guid BYODPatientId = Guid.NewGuid();

        protected Guid CurrentUserId = Guid.NewGuid();

        [TestInitialize]
        public void TestInitialize()
        {
            var controllerContext = new Mock<ControllerContext>();
            var request = new Mock<HttpRequestBase>();
            var requestContext = new Mock<RequestContext>();

            var routeData = new Mock<RouteData>();
            routeData.Object.Values.Add("controller", "index");

            requestContext.Setup(r => r.RouteData).Returns(routeData.Object);
            request.Setup(r => r.RequestContext).Returns(requestContext.Object);

            controllerContext.SetupGet(p => p.HttpContext.Session["CurrentSiteId"]).Returns(InitialSiteId);
            controllerContext.SetupGet(p => p.HttpContext.Request).Returns(request.Object);

            base.Initialize();
            ConfirmationRepository = new Mock<IConfirmationRepository>();
            DeviceRepository = new Mock<IDeviceRepository>();
            PatientRepository = new Mock<IPatientRepository>();
            DiaryEntryRepository = new Mock<IDiaryEntryRepository>();
            ReportRepository = new Mock<IReportRepository>();
            SiteRepository = new Mock<ISiteRepository>();
            SessionService = new Mock<ISessionService>();
            WebBackUpRepository = new Mock<IWebBackupRepository>();
            TranslationService = new Mock<ITranslationService>();
            LanguageService = new Mock<ILanguageService>();
            SubjectInformationService = new Mock<ISubjectInformationService>();
            PatientStatusService = new Mock<IPatientStatusService>();
            StudySettingService = new Mock<IStudySettingService>();
            PatientForEditAdapter = new Mock<IPatientForEditAdapter>();
            RuleService = new Mock<IRuleService>();
            AuthService = new Mock<IAuthService>();

            MockServiceSettings = new Mock<IServiceSettings>();
            MockServiceSettings.Setup(s => s.InventoryAppEnvironment).Returns(string.Empty);

            Controller = new PatientController(
                        PatientRepository.Object,
                        DiaryEntryRepository.Object,
                        SiteRepository.Object,
                        ReportRepository.Object,
                        ConfirmationRepository.Object,
                        PatientForEditAdapter.Object,
                        DeviceRepository.Object,
                        WebBackUpRepository.Object,
                        SubjectInformationService.Object,
                        LanguageService.Object,
                        TranslationService.Object,
                        PatientStatusService.Object,
                        StudySettingService.Object,
                        SessionService.Object,
                        RuleService.Object,
                        MockServiceSettings.Object,
                        AuthService.Object
                        )
                    {
                        ControllerContext = controllerContext.Object
                    };

            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(m => m.Content(It.IsAny<string>())).Returns("test path");
            Controller.Url = mockUrlHelper.Object;


            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            var deviceDto = new DeviceDto
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                IMEI1 = null,
                IMEI2 = null,
                MACAddress = null,
                SerialNumber = "111111111",
                SiteId = InitialSiteId,
                SiteName = "1000",
                DeviceTypeId = Config.Enums.DeviceType.BYOD.Id,
                DeviceTypeName = "Mason",
                AssignedSoftwareVersionNumber = "1.0.0.0",
                LastReportedSoftwareVersionNumber = "1.0.0.0",
                LastDataSyncDate = new DateTimeOffset(),
                AssetTag = "YP-12345"
            };

            var deviceList = new List<DeviceDto>();
            deviceList.Add(deviceDto);

            DeviceRepository.Setup(repo => repo.GetAllDevices(new List<Guid>()))
                .Returns(Task.FromResult(deviceList.AsQueryable().OrderBy(s => s.Id)));

            var siteDto = new SiteDto
            {
                Id = InitialSiteId,
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

            SiteRepository.Setup(repo => repo.GetAllSites(null))
                .ReturnsAsync(siteList.AsEnumerable().OrderBy(s => s.Id));

            var emailId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var studyId = Guid.NewGuid();
            var email = new SendingEmailModel
            {
                Id = emailId,
                To = new List<string>
                {
                    "test@yprime.com"
                },
                Cc = new List<string>(),
                Bcc = new List<string>(),
                ToUsers = new List<Guid>(),
                CcUsers = new List<Guid>(),
                BccUsers = new List<Guid>(),
                From = userId.ToString(),
                Subject = "test subject",
                Body = "test body",
                CreatedDate = DateTime.Now,
                Attachments = new Dictionary<string, byte[]>(),
                StudyId = studyId
            };

            var patientDto = new PatientDto
            {
                Id = PatientId,
                SiteId = InitialSiteId
            };

            var patientBYODDto = new PatientDto
            {
                Id = BYODPatientId,
                SiteId = InitialSiteId
            };

            var deviceEntity = new Device
            {
                Id = deviceDto.Id,
                PatientId = PatientId,
                AssetTag = deviceDto.AssetTag,
                SiteId = deviceDto.SiteId,
                DeviceTypeId = deviceDto.DeviceTypeId
            };

            var byodDeviceEntity = new Device
            {
                Id = Guid.NewGuid(),
                PatientId = BYODPatientId,
                AssetTag = "byodAssetTag",
                SiteId = deviceDto.SiteId,
                DeviceTypeId = Config.Enums.DeviceType.BYOD.Id
            };

            _yprimeSession.CurrentUser = new StudyUserDto
            {
                Id = CurrentUserId,
                Roles = new List<Core.BusinessLayer.Models.StudyRoleModel>
                {
                    new Core.BusinessLayer.Models.StudyRoleModel
                    {
                        ShortName = "YP"
                    }
                },
                Sites = new List<SiteDto>
                {
                    siteDto
                }
            };

            ConfirmationRepository.Setup(repo => repo.SendApiEmail(email));
            ConfirmationRepository.Setup(repo => repo.SaveConfirmation(email.Id, email.Subject, email.Body,
                string.Empty, Guid.NewGuid(), userId, null, new List<EmailRecipient>()));

            StudySettingService.Setup(s => s.GetStringValue(It.Is<string>(v => v == "PatientNumberLength"), null)).ReturnsAsync("4");
            SiteRepository.Setup(s => s.GetSite(It.IsAny<Guid>())).ReturnsAsync(siteDto);
            TranslationService.Setup(t => t.GetByKey(It.IsAny<string>(), null, null)).ReturnsAsync("translation text");

            StudySettingService.Setup(s => s.GetIntValue(It.IsAny<string>(), 0, null)).ReturnsAsync(0);
            PatientRepository.Setup(p => p.GeneratePatientNumber(It.IsAny<Guid>(), It.IsAny<string>(), null)).ReturnsAsync("1234");
            SubjectInformationService.Setup(s => s.GetForCountry(It.IsAny<Guid>(), null)).ReturnsAsync(new List<SubjectInformationModel>());
            SiteRepository.Setup(s => s.GetSite(It.IsAny<Guid>())).ReturnsAsync(siteDto);
            SiteRepository.Setup(s => s.GetSitesForUser(It.IsAny<Guid>())).ReturnsAsync(new List<SiteDto>());
            PatientRepository.Setup(p => p.AddBringYourOwnDeviceAssetTag(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<int>())).ReturnsAsync(byodDeviceEntity);

            PatientRepository.Setup(p => p.GetPatient(It.Is<Guid>(pt => pt == PatientId), It.IsAny<string>()))
                .ReturnsAsync(patientDto);

            PatientRepository.Setup(p => p.GetPatient(It.Is<Guid>(pt => pt == BYODPatientId), It.IsAny<string>()))
                .ReturnsAsync(patientBYODDto);

            DeviceRepository.Setup(d => d.GetPatientBYODDevice(It.IsAny<Guid>()))
                .Returns(deviceEntity);
        }

        protected void SetUpInsertPatient(Guid patientId, bool success = true)
        {
            var patientResponse = new PatientResponse
            {
                Success = success,
                PatientId = patientId
            };

            PatientRepository.Setup(p => p.InsertUpdatePatient(It.IsAny<PatientDto>(), true, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(patientResponse);
        }
    }
}