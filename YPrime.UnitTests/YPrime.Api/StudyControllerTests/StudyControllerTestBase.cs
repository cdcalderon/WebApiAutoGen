using Moq;
using YPrime.API.Controllers;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.UnitTests.YPrime.Api.StudyControllerTests
{
    public abstract class StudyControllerTestBase
    {
        protected Mock<ILanguageService> MockLanguageService = new Mock<ILanguageService>();
        protected Mock<ISystemSettingRepository> MockSystemSettingRepository = new Mock<ISystemSettingRepository>();
        protected Mock<IDataCopyRepository> MockDataCopyRepository = new Mock<IDataCopyRepository>();
        protected Mock<IRoleRepository> MockRoleRepository = new Mock<IRoleRepository>();

        protected void BaseInitialize()
        {
            MockLanguageService = new Mock<ILanguageService>();
            MockSystemSettingRepository = new Mock<ISystemSettingRepository>();
            MockDataCopyRepository = new Mock<IDataCopyRepository>();
            MockRoleRepository = new Mock<IRoleRepository>();
        }

        protected StudyController GetController()
        {
            var controller = new StudyController(
                MockLanguageService.Object,
                MockSystemSettingRepository.Object,
                MockDataCopyRepository.Object);

            return controller;
        }

    }
}
