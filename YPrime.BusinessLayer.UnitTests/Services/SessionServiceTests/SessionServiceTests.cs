using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.SessionServiceTests
{
    [TestClass]
    public class SessionServiceTests
    {
        [TestMethod]
        public void SessionServiceTest()
        {
            var expectedConfigId = Guid.NewGuid();

            var service = new SessionService();
            service.UserConfigurationId = expectedConfigId;

            Assert.AreEqual(expectedConfigId, service.UserConfigurationId);
        }
    }
}
