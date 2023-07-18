using System.Collections.Generic;
using Castle.Core.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using System;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.UserRepositoryTests
{
    [TestClass]
    public class UserRepositoryTests : LegacySiteTestSetup
    {
        protected Mock<MockableDbSetWithExtensions<SystemActionStudyRole>> _systemActionStudyRoles;
        private readonly Guid _studyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
        private readonly Guid _studyRoleId2 = Guid.NewGuid();
        private readonly Guid _studyRoleId3 = Guid.NewGuid();

        private const string TestFirstName = "First";
        private const string TestLastName = "Last";

        [TestInitialize]
        public void InitializeUserTests()
        {
            MockPatientVisitRepository = new Mock<IPatientVisitRepository>();
            MockAuthenticationRepository = new Mock<IAuthenticationUserRepository>();
            MockSoftwareReleaseRepository = new Mock<ISoftwareReleaseRepository>();
            MockStudyRoleService = new Mock<IStudyRoleService>();

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

            _systemActionStudyRoles = CreateDbSetMock(new List<SystemActionStudyRole>());
            _dbContext.Setup(x => x.SystemActionStudyRoles).Returns(_systemActionStudyRoles.Object);
        }

        [TestMethod]
        public async Task GetRolesForUserTest()
        {
            var siteId = "1";
            var userRepository = new UserRepository(
                _dbContext.Object,
                MockAuthenticationRepository.Object,
                MockStudyRoleService.Object,
                MockSoftwareReleaseRepository.Object);
            var getRoles = await userRepository.GetRolesForUser(mainStudyUserGuid, siteId);

            Assert.AreEqual("AB", getRoles.Find(x => x == "AB"));
        }


        [TestMethod]
        public async Task GetUserTest()
        {
            var userId = mainStudyUserGuid;
            var userRepository = new UserRepository(
                _dbContext.Object,
                MockAuthenticationRepository.Object,
                MockStudyRoleService.Object,
                MockSoftwareReleaseRepository.Object);
            var getUser = await userRepository.GetUser(userId, TestFirstName, TestLastName);

            Assert.AreEqual(mainStudyUserGuid, getUser.Id);
            Assert.AreEqual(TestFirstName, getUser.FirstName);
            Assert.AreEqual(TestLastName, getUser.LastName);
        }

        [TestMethod]
        public async Task GetUserDuplicateSitesTest()
        {
            var duplicateStudyUserRole = new StudyUserRole
            {
                Id = Guid.NewGuid(),
                StudyUserId = mainStudyUserGuid,
                StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123"),
                SiteId = mainSiteGuid,
                Site = _mainSite,
                StudyUser = new StudyUser
                {
                    Id = mainStudyUserGuid,
                    UserName = "email@email.com"
                }
            };

            _studyUserRoles.Add(duplicateStudyUserRole);

            _mainStudyUser.StudyUserRoles = _studyUserRoles
                .Where(sur => sur.StudyUserId == mainStudyUserGuid)
                .ToList();

            var userRepository = new UserRepository(
                _dbContext.Object,
                MockAuthenticationRepository.Object,
                MockStudyRoleService.Object,
                MockSoftwareReleaseRepository.Object);

            var result = await userRepository.GetUser(mainStudyUserGuid, TestFirstName, TestLastName);

            Assert.AreEqual(mainStudyUserGuid, result.Id);
            Assert.AreEqual(TestFirstName, result.FirstName);
            Assert.AreEqual(TestLastName, result.LastName);
            Assert.AreEqual(1, result.Sites.Count);
            Assert.AreEqual(_mainSite.Id, result.Sites.First().Id);
        }


        [TestMethod]
        public async Task IsUserInRoleTest()
        {
            var userName = mainStudyUserGuid.ToString();
            var roleName = "AB";
            var siteId = mainSiteGuid.ToString();

            var userRepository = new UserRepository(
                _dbContext.Object,
                MockAuthenticationRepository.Object,
                MockStudyRoleService.Object,
                MockSoftwareReleaseRepository.Object);
            var isUserInRole = await userRepository.IsUserInRole(userName, roleName, siteId);

            Assert.IsTrue(isUserInRole);
        }


        [TestMethod]
        public async Task GetUsersInRoleTest()
        {
            var roleName = "AB";
            var siteId = mainSiteGuid.ToString();

            var userRepository = new UserRepository(
                _dbContext.Object,
                MockAuthenticationRepository.Object,
                MockStudyRoleService.Object,
                MockSoftwareReleaseRepository.Object);
            var getUsers = await userRepository.GetUsersInRole(roleName, siteId);

            Assert.AreEqual("email@email.com", getUsers.Find(x => x == "email@email.com"));
        }

        [TestMethod]
        public void GetStudyUserRolesTest()
        {
            var userRepository = new UserRepository(
                _dbContext.Object,
                MockAuthenticationRepository.Object,
                MockStudyRoleService.Object,
                MockSoftwareReleaseRepository.Object);
            var getStudyUserRoles = userRepository.GetStudyUserRoles(mainStudyUserGuid);

            var siteIsActive = getStudyUserRoles.First().SiteIsActive;
            Assert.IsFalse(siteIsActive);

            Assert.AreEqual(mainStudyUserGuid,
                getStudyUserRoles.Find(x => x.StudyUserId == mainStudyUserGuid).StudyUserId);
        }
    }
}