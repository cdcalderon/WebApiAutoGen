using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Controllers;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareVersionControllerTests
{
    [TestClass]
    public class SoftwareVersionControllerAddTests : SoftwareVersionControllerTestBase
    {
        private SoftwareVersionDto _model;

        [TestInitialize]
        public void TestInitialize()
        {
            base.TestInitialize();
            _model = new SoftwareVersionDto();
        }

        [TestMethod]
        public void WhenCalled_WillInsertNewSoftwareVersion()
        {
            _model = new SoftwareVersionDto
            {
                Id = new Guid(),
                VersionNumber = "1.2.3.4",
                PackagePath = RandomString(),
                ApkFile = new Mock<HttpPostedFileBase>().Object
            };
            var newSoftwareVersion = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = _model.VersionNumber,
                PlatformTypeId = PlatformType.Android.Id
            };

            var Controller1 = new Mock<SoftwareVersionController>();

            Repository.Setup(x => x.CheckVersionNumberIsUsed(_model.VersionNumber)).Returns(false);
            Repository.Setup(x => x.AddSoftwareVersion(newSoftwareVersion)).Returns(true);
            Repository.Setup(x => x.SaveToCDNFolder(_model, new Uri("http://www.yprime.com"))).Returns(RandomString());

            var result = Controller.Add(_model);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void WhenCalled_WillFailToInsertSoftwareVersion()
        {
            _model = new SoftwareVersionDto
            {
                Id = new Guid(),
                VersionNumber = "1.2.3.4",
                PackagePath = RandomString()
            };
            var newSoftwareVersion = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = _model.VersionNumber,
                PlatformTypeId = PlatformType.Android.Id
            };

            Repository.Setup(x => x.CheckVersionNumberIsUsed(_model.VersionNumber)).Returns(false);
            Repository.Setup(x => x.AddSoftwareVersion(newSoftwareVersion)).Returns(true);
            Repository.Setup(x => x.SaveToCDNFolder(_model, new Uri("http://www.yprime.com"))).Returns(RandomString());

            var result = Controller.Add(_model);
            ViewResult rs = (ViewResult) result;

            Assert.IsTrue(rs.ViewData.ModelState["APK File"].Errors.Count > 0);
        }
    }
}