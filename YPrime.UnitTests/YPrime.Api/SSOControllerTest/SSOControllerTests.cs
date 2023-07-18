using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using YPrime.API.Controllers;
using YPrime.Auth.Data.Models.JSON;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.UnitTests.YPrime.Api.SSOControllerTest
{
    [TestClass]
    public class SSOControllerTests : SiteTestSetup
    {
        protected Mock<IUserRepository> MockUserRepository;
        protected Mock<ILanguageService> MockLanguageService;
        protected Mock<IStudyRoleService> MockStudyRoleService;
        protected Mock<ICountryService> MockCountryService;
        protected Mock<IAuthenticationUserRepository> MockAuthenticationRepository;
        protected Mock<ISoftwareReleaseRepository> MockSoftwareReleaseRepository;
        private readonly Guid _studyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
        private readonly Guid _studyRoleId2 = Guid.NewGuid();
        private readonly Guid _studyRoleId3 = Guid.NewGuid();

        public SSOController InitializeSSO()
        {
            MockUserRepository = new Mock<IUserRepository>();
            MockLanguageService = new Mock<ILanguageService>();
            MockStudyRoleService = new Mock<IStudyRoleService>();
            MockAuthenticationRepository = new Mock<IAuthenticationUserRepository>();
            MockSoftwareReleaseRepository = new Mock<ISoftwareReleaseRepository>();

            MockStudyRoleService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<StudyRoleModel>
            {
                new StudyRoleModel
                {
                    Id = _studyRoleId,
                    IsBlinded = true,
                    ShortName ="AB"
                },
                new StudyRoleModel
                {
                    Id = _studyRoleId2,
                    IsBlinded = true,
                    ShortName ="XY"
                },
                new StudyRoleModel
                {
                    Id = _studyRoleId3,
                    IsBlinded = false,
                    ShortName ="ZZ"
                }
            });

            MockCountryService = new Mock<ICountryService>();
            MockCountryService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<CountryModel>
            {
                new CountryModel
                {
                    Id = countrySiteGuid,
                    Name = "Test Country"
                }
            });

            var siteRepo = new SiteRepository(_dbContext.Object, MockUserRepository.Object, 
                MockLanguageService.Object, MockStudyRoleService.Object, new Mock<IPatientStatusService>().Object, MockCountryService.Object, MockStudySettingSerice.Object);
            var userRepo = new UserRepository(_dbContext.Object, MockAuthenticationRepository.Object, MockStudyRoleService.Object, MockSoftwareReleaseRepository.Object);

            return new SSOController(siteRepo, MockStudyRoleService.Object, userRepo, MockSessionService.Object);
        }

        [TestMethod]
        public async Task GetSitesTest()
        {
            var ssoController = InitializeSSO();
            var listOfUserData = new List<UserData>
            {
                new UserData
                {
                    StudyUserRoles = new List<StudyUserRole>
                    {
                        new StudyUserRole
                        {
                            RoleName = "AB",
                            SiteNumber = "1",
                            SiteName = "Test Site",
                            StudyUserId = Guid.Parse("42345678-1234-1234-1234-123456789121")
                        },
                        new StudyUserRole
                        {
                            RoleName = "AB",
                            SiteName = "Test Site",
                            StudyUserId = Guid.Parse("42345678-1234-1234-1234-123456789121")
                        },
                        new StudyUserRole
                        {
                            RoleName = "AB",
                            SiteNumber = "1",
                            StudyUserId = Guid.Parse("42345678-1234-1234-1234-123456789121")
                        }
                    }
                }
            };

            var result = await ssoController.InflateStudyDataByName(listOfUserData);

            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void CheckSiteNumberApi()
        {
            var ssoController = InitializeSSO();
            var actionResult = ssoController.CheckIfStudyAcceptsSiteNumber();
            var actionResponse = actionResult as OkResult;
            Assert.IsNotNull(actionResponse);
        }
    }
}