using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.BusinessLayer.UnitTests;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Tests.Controllers.SoftwareVersionControllerTests;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.SoftwareVersionControllerTests
{
    [TestClass]
    public class SoftwareVersionControllerGetSoftwareVersionsByReleaseTests : SoftwareVersionControllerTestBase
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
            Repository.Setup(repo => repo.GetSoftwareVersionIdsAssignedToReleases())
                .Returns(new List<Guid> {_expectedSoftwareVersion.Id});
        }

        [TestMethod]
        public void WhenCalled_WillReturnJsonResult()
        {
            var result = Controller.GetSoftwareVersionIdsAssignedToReleases();
            YAssert.IsType<JsonResult>(result);
        }

        [TestMethod]
        public void WhenCalled_WillReturnListOfTypeGuid()
        {
            var result = Controller.GetSoftwareVersionIdsAssignedToReleases();
            JsonResult jsonResult = result as JsonResult;
            dynamic jsonData = jsonResult.Data;
            foreach (dynamic json in jsonData)
            {
                Assert.IsTrue(Guid.TryParse(json.ToString(), out Guid newGuid));
            }
        }

        [TestMethod]
        public void WhenCalledWithNoResults_WillReturnEmptyJsonResult()
        {
            Repository.Setup(repo => repo.GetSoftwareVersionIdsAssignedToReleases())
                .Returns(new List<Guid>());

            var result = Controller.GetSoftwareVersionIdsAssignedToReleases();
            JsonResult jsonResult = result as JsonResult;
            dynamic jsonData = jsonResult.Data;
            Assert.IsTrue(jsonData.Count == 0);
        }
    }
}