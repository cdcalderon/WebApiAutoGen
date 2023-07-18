using Moq;
using System;
using System.Collections.Generic;
using YPrime.BusinessLayer.Helpers;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;

namespace YPrime.BusinessLayer.UnitTests.Repositories.ConfirmationRepositoryTests
{
    public abstract class ConfirmationRepositoryTestBase
    {
        protected readonly Mock<IConfigurationSettings> Config;
        protected readonly Mock<IStudyDbContext> Context;
        protected readonly ConfirmationRepository Repository;
        protected readonly Mock<IStudyRoleService> StudyRoleService;
        protected Mock<IStudySettingService> MockStudySettingService;
        protected Mock<IServiceSettings> MockServiceSettings;
        protected Mock<IAuthService> MockAuthService;
        protected Mock<ISystemSettingRepository> MockSystemSettingRepository;
        protected ConfirmationRepositoryTestBase()
        {
            Context = new Mock<IStudyDbContext>();
            Config = new Mock<IConfigurationSettings>();
            MockStudySettingService = new Mock<IStudySettingService>();
            MockSystemSettingRepository = new Mock<ISystemSettingRepository>();
            MockServiceSettings = new Mock<IServiceSettings>();
            MockAuthService = new Mock<IAuthService>();

            StudyRoleService = new Mock<IStudyRoleService>();
            StudyRoleService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<StudyRoleModel>
            {
                new StudyRoleModel
                {
                    Id = Guid.NewGuid()
                },
                new StudyRoleModel
                {
                    Id = Guid.NewGuid()
                }
            });

            Repository = new ConfirmationRepository(
                Context.Object,
                null,
                null,
                StudyRoleService.Object,
                MockSystemSettingRepository.Object,
                MockServiceSettings.Object,
                MockAuthService.Object);
        }
    }
}