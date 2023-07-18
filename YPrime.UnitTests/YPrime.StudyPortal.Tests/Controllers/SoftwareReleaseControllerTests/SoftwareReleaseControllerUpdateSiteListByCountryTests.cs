using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.UnitTests;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareReleaseControllerTests
{
    [TestClass]
    public class SoftwareReleaseControllerUpdateSiteListByCountryTests : SoftwareReleaseControllerTestBase
    {
        private List<Guid> _passedInDeviceTypeIds;
        private List<Guid> _passedInCountryIds;

        private Guid _testSiteId;
        private string _testSiteName;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            _testSiteId = Guid.NewGuid();
            _testSiteName = $"Site {Guid.NewGuid()}";

            Repository
                .Setup(r => r.GetSiteDictionaryByCountry(It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()))
                .ReturnsAsync(new Dictionary<string, string>
                {
                    { _testSiteId.ToString(), _testSiteName }
                })
                .Callback((List<Guid> passedInDeviceTypeIds, List<Guid> passedInCountryIds) =>
                {
                    _passedInDeviceTypeIds = passedInDeviceTypeIds;
                    _passedInCountryIds = passedInCountryIds;
                });

        }

        [TestMethod]
        public async Task WhenCalled_WillReturnJsonResult()
        {
            var deviceTypeIds = new List<Guid>();
            var countryIds = new List<Guid>();
            var result = await Controller.UpdateSiteListByCountry(deviceTypeIds, countryIds);

            YAssert.IsType<JsonResult>(result);
        }

        [TestMethod]
        public async Task WhenCalled_WillReturnDictionaryValues()
        {
            var deviceTypeIdToPassIn = Guid.NewGuid();
            var countryIdToPassIn = Guid.NewGuid();

            var deviceTypeIds = new List<Guid>
            {
                deviceTypeIdToPassIn
            };

            var countryIds = new List<Guid>
            {
                countryIdToPassIn
            };

            var result = await Controller.UpdateSiteListByCountry(deviceTypeIds, countryIds);

            JsonResult jsonResult = result as JsonResult;
            dynamic json = jsonResult.Data;

            Assert.IsNotNull(json);
            Assert.AreEqual(_testSiteName, json[_testSiteId.ToString()]);

            Assert.AreEqual(deviceTypeIds.Count, _passedInDeviceTypeIds.Count);

            foreach (var deviceId in deviceTypeIds)
            {
                Assert.IsTrue(_passedInDeviceTypeIds.Contains(deviceId));
            }

            Assert.AreEqual(countryIds.Count, _passedInCountryIds.Count);

            foreach (var countryId in countryIds)
            {
                Assert.IsTrue(_passedInCountryIds.Contains(countryId));
            }
        }
    }
}