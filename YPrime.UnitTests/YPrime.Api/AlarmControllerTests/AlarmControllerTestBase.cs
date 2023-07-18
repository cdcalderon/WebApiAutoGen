using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Interfaces;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests.YPrime.Api.AlarmControllerTests
{
    public class AlarmControllerTestBase : BaseControllerTest
    {
        protected Mock<IAlarmRepository> MockAlarmRepository;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            MockAlarmRepository = new Mock<IAlarmRepository>();

        }
    }
}
