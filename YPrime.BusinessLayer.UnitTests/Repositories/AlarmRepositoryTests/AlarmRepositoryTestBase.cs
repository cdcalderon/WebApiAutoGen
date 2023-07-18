using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessRule.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Repositories;
using System;

namespace YPrime.BusinessLayer.UnitTests.Repositories.AlarmRepositoryTests
{
    [TestClass]
    public abstract class AlarmRepositoryTestBase
    {
        protected Mock<IRuleService> MockRuleService;
        protected Mock<IAlarmService> MockAlarmService;
        protected Mock<IDeviceRepository> MockDeviceRepository;
        protected Mock<IPatientRepository> MockPatientRepository;
        protected Mock<ISiteRepository> MockSiteRepository;

        protected DateTimeOffset TestSiteLocalTime;
        protected Guid TestSiteId;
        protected Guid TestPatientId;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            MockDeviceRepository = new Mock<IDeviceRepository>();
            MockPatientRepository = new Mock<IPatientRepository>();
            MockRuleService = new Mock<IRuleService>();
            MockAlarmService = new Mock<IAlarmService>();
            MockSiteRepository = new Mock<ISiteRepository>();

            TestSiteId = Guid.NewGuid();
            TestPatientId = Guid.NewGuid();

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            TestSiteLocalTime = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, timeZone);

            MockSiteRepository
                .Setup(r => r.GetSiteLocalTime(It.Is<Guid>(sid => sid == TestSiteId)))
                .Returns(TestSiteLocalTime);
        }

        protected IAlarmRepository GetService()
        {
            return new AlarmRepository(MockRuleService.Object,
                MockAlarmService.Object,
                MockDeviceRepository.Object,
                MockPatientRepository.Object,
                MockSiteRepository.Object);
        }
    }
}
