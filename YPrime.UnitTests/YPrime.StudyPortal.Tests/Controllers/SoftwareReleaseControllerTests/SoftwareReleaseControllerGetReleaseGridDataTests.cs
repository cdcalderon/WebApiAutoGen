using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.UnitTests;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareReleaseControllerTests
{
    [TestClass]
    public class SoftwareReleaseControllerGetReleaseGridDataTests : SoftwareReleaseControllerTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            var gridData = new SoftwareReleaseDto
            {
                ReleaseDate = "dd-MM-yy",
                Name = "test",
                VersionNumber = "0.0.0.1",
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true,
                CountryNameList = "USA, Canada",
                SiteNameList = "10001, 20001",
                AssetTagList = "YP-12345, YP-67890",
                AssignedReportedVersionCount = "2"
            };
            var dataList = new List<SoftwareReleaseDto>();
            dataList.Add(gridData);
            Repository.Setup(repo => repo.GetSoftwareReleaseGridData())
                .ReturnsAsync(dataList);
        }

        [TestMethod]
        public async Task WhenCalled_WillReturnJsonResult()
        {
            var result = await Controller.GetReleaseGridData();
            YAssert.IsType<JsonResult>(result);
            JsonResult jsonResult = result as JsonResult;
            dynamic jsonData = jsonResult.Data;

            foreach (dynamic json in jsonData)
            {
                Assert.IsNotNull(json.ReleaseDate,
                    "JSON record does not contain \"ReleaseDate\" property.");
                Assert.IsNotNull(json.Name,
                    "JSON record does not contain \"Name\" property.");
                Assert.IsNotNull(json.VersionNumber,
                    "JSON record does not contain \"VesionNumber\" property.");
                Assert.IsNotNull(json.IsActive,
                    "JSON record does not contain \"IsActive\" property.");
                Assert.IsNotNull(json.Required,
                    "JSON record does not contain \"Required\" property.");
                Assert.IsNotNull(json.ConfigurationId,
                    "JSON record does not contain \"ConfigurationId\" property.");
                Assert.IsNotNull(json.StudyWide,
                    "JSON record does not contain \"StudyWide\" property.");
                Assert.IsNotNull(json.CountryNameList,
                    "JSON record does not contain \"CountryNameList\" property.");
                Assert.IsNotNull(json.SiteNameList,
                    "JSON record does not contain \"SiteNameList\" property.");
                Assert.IsNotNull(json.AssetTagList,
                    "JSON record does not contain \"AssetTagList\" property.");
                Assert.IsNotNull(json.AssignedReportedVersionCount,
                    "JSON record does not contain \"AssignedReportedVersionCount\" property.");
            }
        }
    }
}