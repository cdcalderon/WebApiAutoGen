using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.BusinessLayer.UnitTests;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareVersionControllerTests
{
    [TestClass]
    public class SoftwareVersionControllerDeleteSoftwareVersionByIdTest : SoftwareVersionControllerTestBase
    {
        private SoftwareVersion _expectedSoftwareVersion;
        private Guid _Id;
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

            _Id = Guid.NewGuid();
            Repository.Setup(repo => repo.DeleteSoftwareVersionById(_Id))
                .Throws(new InvalidOperationException());
        }

        [TestMethod]
        public void WhenCalledWithExistingVersionId_WillReturnJsonResult()
        {
            var result = Controller.DeleteSoftwareVersionById(_expectedSoftwareVersion.Id);
            YAssert.IsType<JsonResult>(result);
        }

        [TestMethod]
        public void WhenCalledWithExistingVersionId_WillReturnObjectWillNotBeNull()
        {
            var result = Controller.DeleteSoftwareVersionById(_expectedSoftwareVersion.Id);
            JsonResult jsonResult = result as JsonResult;
            dynamic jsonData = jsonResult.Data;
            Assert.IsNotNull(jsonData);
        }

        [TestMethod]
        public void WhenCalledWithExistingVersionId_WillReturnObjectWithSuccessProperty()
        {
            var result = Controller.DeleteSoftwareVersionById(_expectedSoftwareVersion.Id);
            JsonResult jsonResult = result as JsonResult;
            dynamic jsonData = jsonResult.Data;
            foreach (var json in new RouteValueDictionary(jsonData))
            {
                Assert.IsTrue(json.Key == "success");
                Assert.IsTrue((bool) json.Value);
            }
        }

        [TestMethod]
        public void WithNoExistingVersion_WillThrowException()
        {
            try
            {
                Controller.DeleteSoftwareVersionById(_Id);
            }
            catch (Exception)
            {
                return; // Test Passed
            }

            Assert.Fail("Expected exception Exception, was not thrown");
        }
    }
}