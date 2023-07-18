using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.BusinessLayer.UnitTests;
using YPrime.StudyPortal.Controllers;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareVersionControllerTests
{
    [TestClass]
    public class SoftwareVersionControllerConstructorTests : SoftwareVersionControllerTestBase
    {
        [TestMethod]
        public void WithAllParameters_WillNotThrowException()
        {
            YAssert.DoesNotThrow<Exception>(() => new SoftwareVersionController(Repository.Object, MockSessionService.Object));
        }
    }
}