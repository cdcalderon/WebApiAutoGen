using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.API.Controllers;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests.YPrime.Api.DeviceManagementControllerTests
{
    public abstract class DeviceManangementControllerTestBase : BaseControllerTest
    {
        protected DeviceManagementController Controller;
        protected Mock<IDeviceRepository> DeviceRepository;
        protected Mock<ISiteRepository> SiteRepository;
        protected Mock<ISoftwareVersionRepository> SoftwareVersionRepository;
        protected Mock<ISyncLogRepository> SynclogRepository;
        protected Mock<ISessionService> SessionService;

        [TestInitialize]
        public void TestInitialize()
        {
            base.Initialize();
            SoftwareVersionRepository = new Mock<ISoftwareVersionRepository>();
            SiteRepository = new Mock<ISiteRepository>();
            DeviceRepository = new Mock<IDeviceRepository>();
            SynclogRepository = new Mock<ISyncLogRepository>();
            SessionService = new Mock<ISessionService>();
            Controller = new DeviceManagementController(
                SiteRepository.Object, 
                DeviceRepository.Object,
                SoftwareVersionRepository.Object, 
                SynclogRepository.Object,
                SessionService.Object);

            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }
    }
}