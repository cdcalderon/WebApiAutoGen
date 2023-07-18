using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.BusinessLayer.UnitTests;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareVersionControllerTests
{
    [TestClass]
    public class SoftwareVersionControllerGetIndexTests : SoftwareVersionControllerTestBase
    {
        private SoftwareVersion _expectedSoftwareVersion;
        private List<SoftwareVersionDto> _model;

        [TestInitialize]
        public void TestInitialize()
        {
            base.TestInitialize();

            _expectedSoftwareVersion = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = "1.2.3.4",
                PackagePath = RandomString()
            };
            Repository.Setup(repo => repo.GetAllSoftwareVersions())
                .Returns(new List<SoftwareVersion> {_expectedSoftwareVersion});
        }

        [TestMethod]
        public void WhenCalled_WillReturnViewResult()
        {
            var result = Controller.Index();
            YAssert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void WhenCalled_WillHaveViewNameBeIndex()
        {
            var result = Controller.Index();
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void WhenCalled_WillSetModel()
        {
            var result = Controller.Index();
            Assert.AreEqual(1, (result.Model as IEnumerable<SoftwareVersionDto>).Count());

            var resultSV = (result.Model as IEnumerable<SoftwareVersionDto>).First();

            Assert.AreEqual(_expectedSoftwareVersion.Id, resultSV.Id);
            Assert.AreEqual(_expectedSoftwareVersion.VersionNumber, resultSV.VersionNumber);
            Assert.AreEqual(_expectedSoftwareVersion.PackagePath, resultSV.PackagePath);
        }
    }
}