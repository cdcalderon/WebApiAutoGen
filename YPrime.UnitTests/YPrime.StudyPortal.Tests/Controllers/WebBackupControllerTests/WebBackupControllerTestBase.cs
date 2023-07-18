using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.Auth.Data.Models.JSON;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Controllers;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.WebBackupControllerTests
{
    public abstract class WebBackupControllerTestBase : BaseControllerTest
    {
        protected Mock<IConfirmationRepository> ConfirmationRepository;

        protected WebBackupController Controller;
        protected Mock<IDeviceRepository> DeviceRepository;
        protected Mock<ISiteRepository> SiteRepository;
        protected Mock<IWebBackupRepository> WebBackUpRepository;
        protected Mock<IPatientVisitRepository> PatientVisitRepository;
        protected Mock<ISessionService> SessionService;
        protected Mock<IServiceSettings> ServiceSettings;

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
            DeviceRepository = new Mock<IDeviceRepository>();
            WebBackUpRepository = new Mock<IWebBackupRepository>();
            SiteRepository = new Mock<ISiteRepository>();
            PatientVisitRepository = new Mock<IPatientVisitRepository>();
            SessionService = new Mock<ISessionService>();
            ServiceSettings = new Mock<IServiceSettings>();

            Controller = new WebBackupController(
                SiteRepository.Object,
                DeviceRepository.Object, 
                WebBackUpRepository.Object, 
                ConfirmationRepository.Object,
                new Mock<IStudySettingService>().Object,
                PatientVisitRepository.Object,
                SessionService.Object,
                ServiceSettings.Object)
            {
                ControllerContext = (new Mock<ControllerContext>()).Object
            };
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
                SiteId = new Guid(),
                SiteName = "1000",
                DeviceTypeId = new Guid(),
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

            ConfirmationRepository.Setup(repo => repo.SendApiEmail(email));
            ConfirmationRepository.Setup(repo => repo.SaveConfirmation(email.Id, email.Subject, email.Body,
                string.Empty, Guid.NewGuid(), userId, null, new List<EmailRecipient>()));
        }
    }
}