using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.ConfirmationRepositoryTests
{
    [TestClass]
    public class ConfirmationRepositoryGetUsersSubscribedToConfirmationTests : LegacyTestBase
    {
        private readonly Guid _emailContentId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private readonly Guid _studyRoleId = Guid.NewGuid();
        private readonly Guid _studyRoleId2 = Guid.NewGuid();
        private readonly Guid _studyRoleId3 = Guid.NewGuid();
        private Mock<IStudyDbContext> _dbContext;
        private Mock<IStudyRoleService> _studyRoleService;
        private Mock<IServiceSettings> _serviceSettings;
        private Mock<IAuthService> _authService;
        private Mock<MockableDbSetWithExtensions<EmailContentStudyRole>> _emailContentStudyRoles;
        private Mock<MockableDbSetWithExtensions<StudyUserRole>> _studyUserRole;
        protected Mock<ISystemSettingRepository> MockSystemSettingRepository;

        [TestInitialize]
        public void Initialize()
        {
            _emailContentStudyRoles = CreateDbSetMock(new List<EmailContentStudyRole>
            {
                new EmailContentStudyRole
                {
                    StudyRoleId = _studyRoleId,
                    EmailContentId = _emailContentId,
                    EmailContent = new EmailContent
                    {
                        Id = _emailContentId,
                        IsBlinded = true,
                        IsSiteSpecific = false
                    }
                },
                new EmailContentStudyRole
                {
                    StudyRoleId = _studyRoleId3,
                    EmailContentId = _emailContentId,
                    EmailContent = new EmailContent
                    {
                        Id = _emailContentId,
                        IsBlinded = false,
                        IsSiteSpecific = false
                    }
                }
            });

            _dbContext = new Mock<IStudyDbContext>();
            _dbContext.Setup(x => x.EmailContentStudyRoles).Returns(_emailContentStudyRoles.Object);

            _studyRoleService = new Mock<IStudyRoleService>();
            _studyRoleService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<StudyRoleModel>
            {
                new StudyRoleModel
                {
                    Id = _studyRoleId, 
                    IsBlinded = true
                },
                new StudyRoleModel
                {
                    Id = _studyRoleId2,
                    IsBlinded = true
                },
                new StudyRoleModel
                {
                    Id = _studyRoleId3,
                    IsBlinded = false
                }
            });

            _authService = new Mock<IAuthService>();
            _serviceSettings = new Mock<IServiceSettings>();

            MockSystemSettingRepository = new Mock<ISystemSettingRepository>();
        }

        [TestMethod]
        public async Task GetUsersSubscribedToConfirmationTestBlindedRole()
        {
            _studyUserRole = CreateDbSetMock(new List<StudyUserRole>
            {
                new StudyUserRole
                {
                    StudyUserId = Guid.NewGuid(),
                    StudyRoleId = _studyRoleId,
                    StudyUser = new StudyUser
                    {
                        Id = Guid.NewGuid(),
                        Email = "unblinded1@yprime.com"
                    }
                },
                new StudyUserRole
                {
                    StudyUserId = Guid.NewGuid(),
                    StudyRoleId = _studyRoleId2,
                    StudyUser = new StudyUser
                    {
                        Id = Guid.NewGuid(),
                        Email = "unblinded2@yprime.com"
                    }
                }
            });
            _dbContext.Setup(x => x.StudyUserRoles).Returns(_studyUserRole.Object);

            var confirmationRepository = new ConfirmationRepository(
                _dbContext.Object, 
                null, 
                null, 
                _studyRoleService.Object,
                MockSystemSettingRepository.Object,
                _serviceSettings.Object,
                _authService.Object);

            var result = await confirmationRepository.GetUsersSubscribedToConfirmation(_emailContentId, null);
            Assert.IsTrue(result.Count == 1);
        }

        [TestMethod]
        public async Task GetUsersSubscribedToConfirmationTestUnblindedRole()
        {
            _studyUserRole = CreateDbSetMock(new List<StudyUserRole>
            {
                new StudyUserRole
                {
                    StudyUserId = Guid.NewGuid(),
                    StudyRoleId = _studyRoleId3,
                    StudyUser = new StudyUser
                    {
                        Id = Guid.NewGuid(),
                        Email = "unblinded1@yprime.com"
                    }
                }
            });

            _dbContext.Setup(x => x.StudyUserRoles).Returns(_studyUserRole.Object);

            var confirmationRepository = new ConfirmationRepository(
                _dbContext.Object,
                null,
                null,
                _studyRoleService.Object,
                MockSystemSettingRepository.Object,
                _serviceSettings.Object,
                _authService.Object);

            var result = await confirmationRepository.GetUsersSubscribedToConfirmation(_emailContentId, null);

            Assert.IsTrue(result.Count == 1);
        }
    }
}