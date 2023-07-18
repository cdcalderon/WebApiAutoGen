using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.UnitTests;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareReleaseControllerTests
{
    public class SoftwareReleaseControllerUpdateCountryListByDeviceTypeTests : SoftwareReleaseControllerTestBase
    {
        private List<Guid> _passedInDeviceTypeIds;

        private Guid _testCountryId;
        private string _testCountryName;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            _testCountryId = Guid.NewGuid();
            _testCountryName = $"Country {Guid.NewGuid()}";

            Repository
                .Setup(r => r.GetCountryDictionaryByDeviceType(
                    It.IsAny<List<Guid>>()))
                .ReturnsAsync(new Dictionary<string, string>
                {
                    { _testCountryId.ToString(), _testCountryName }
                })
                .Callback((List<Guid> passedInDeviceTypeIds) =>
                {
                    _passedInDeviceTypeIds = passedInDeviceTypeIds;
                });
        }

        [TestMethod]
        public async Task WhenCalled_WillReturnDictionaryValues()
        {
            var deviceTypeIdToPassIn = Guid.NewGuid();

            var deviceTypeIds = new List<Guid>
            {
                deviceTypeIdToPassIn
            };

            var result = await Controller.UpdateCountryListByDeviceType(deviceTypeIds);

            JsonResult jsonResult = result as JsonResult;
            dynamic json = jsonResult.Data;

            Assert.IsNotNull(json);
            Assert.AreEqual(_testCountryId, json[_testCountryName.ToString()]);

            Assert.AreEqual(deviceTypeIds.Count, _passedInDeviceTypeIds.Count);

            foreach (var deviceId in deviceTypeIds)
            {
                Assert.IsTrue(_passedInDeviceTypeIds.Contains(deviceId));
            }
        }
    }
}