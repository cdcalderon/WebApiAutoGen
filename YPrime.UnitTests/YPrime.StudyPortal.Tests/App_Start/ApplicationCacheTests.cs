using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.StudyPortal;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.App_Start
{
    [TestClass]
    public class ApplicationCacheTests : BaseControllerTest
    {
        private const string TestKey = "test";
        private const string TestValue = "test";
        private Mock<IStudySettingService> MockStudySettingService;

        [TestMethod]
        public void GetExistsInCacheTest()
        {
            MockStudySettingService = new Mock<IStudySettingService>();

            var applicationCache = new ApplicationCache(MockStudySettingService.Object);

            //setup cache
            applicationCache.Set(TestKey, TestValue);

            Assert.AreEqual(applicationCache.Get(TestKey), TestValue);
        }

        [TestMethod]
        public void GetNotExistsInCacheTest()
        {
            MockStudySettingService = new Mock<IStudySettingService>();

            MockStudySettingService
                .Setup(f => f.GetStringValue(It.IsAny<string>(), null))
                .ReturnsAsync(TestValue);

            var applicationCache = new ApplicationCache(MockStudySettingService.Object);

            Assert.AreEqual(applicationCache.Get(TestKey), TestValue);
        }
    }
}
