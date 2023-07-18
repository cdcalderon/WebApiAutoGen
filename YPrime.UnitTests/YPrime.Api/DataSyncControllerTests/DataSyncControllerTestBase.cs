using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.API.Controllers;
using YPrime.BusinessLayer.Interfaces;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests.YPrime.Api.DataSyncControllerTests
{
    [TestClass]
    public class DataSyncControllerTestBase : BaseControllerTest
    {
        protected Mock<IDataSyncRepository> MockDataSyncRepository;
        protected Mock<IDiaryEntryRepository> MockDiaryEntryRepository;
        protected Mock<IDeviceRepository> MockDeviceRepository;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            MockDataSyncRepository = new Mock<IDataSyncRepository>();
            MockDiaryEntryRepository = new Mock<IDiaryEntryRepository>();
            MockDeviceRepository = new Mock<IDeviceRepository>();
        }

        public DataSyncController GetController()
        {
            var controller = new DataSyncController(
                MockDataSyncRepository.Object,
                MockDiaryEntryRepository.Object,
                MockDeviceRepository.Object,
                MockSessionService.Object);

            return controller;
        }

    }
}
