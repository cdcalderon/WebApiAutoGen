using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareVersionControllerTests
{
    [TestClass]
    public class SoftwareVersionControllerAddBlankTests : SoftwareVersionControllerTestBase
    {
        private SoftwareVersionDto _model;

        [TestInitialize]
        public void TestInitialize()
        {
            base.TestInitialize();
            _model = new SoftwareVersionDto();
        }

        [TestMethod]
        public void WhenCalled_WillSetModel()
        {
            var result = Controller.Add();
            Assert.IsNotNull((result.Model as SoftwareVersionDto));
            Assert.AreEqual(Guid.Empty, (result.Model as SoftwareVersionDto).Id);
            Assert.IsNull((result.Model as SoftwareVersionDto).VersionNumber);
            Assert.IsNull((result.Model as SoftwareVersionDto).PackagePath);
        }
    }
}