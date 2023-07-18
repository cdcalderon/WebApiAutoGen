using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryGetDeviceTypesForStudyTests : SoftwareReleaseRepositoryTestBase
    {
        private List<Device> _devices;

        [TestInitialize]
        public void TestInitialize()
        {
            _devices = new List<Device>();

            var deviceDbSet = new FakeDbSet<Device>(_devices);

            Context
                .Setup(c => c.Devices)
                .Returns(deviceDbSet.Object);
        }

        [TestMethod]
        public async Task GetDeviceTypesForStudyPhoneTest()
        {
            var deviceType = DeviceType.Phone;

            _devices.Add(CreateDevice(deviceType));

            var result = await Repository.GetProvisionalDeviceTypesForStudy();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(deviceType, result[0]);
        }

        [TestMethod]
        public async Task GetDeviceTypesForStudyTabletTest()
        {
            var deviceType = DeviceType.Tablet;

            _devices.Add(CreateDevice(deviceType));

            var result = await Repository.GetProvisionalDeviceTypesForStudy();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(deviceType, result[0]);
        }

        [TestMethod]
        public async Task GetDeviceTypesForStudyNoneTest()
        {
            var result = await Repository.GetProvisionalDeviceTypesForStudy();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetProvisionalDeviceTypesForStudyAllTest()
        {
            var allDeviceTypes = DeviceType.GetAll<DeviceType>().Where(dt => dt.Id != DeviceType.BYOD.Id);

            foreach (var deviceType in allDeviceTypes)
            {
                _devices.Add(CreateDevice(deviceType));
            }

            var result = await Repository.GetProvisionalDeviceTypesForStudy();

            Assert.AreEqual(allDeviceTypes.Count(), result.Count);

            foreach (var deviceType in allDeviceTypes)
            {
                Assert.IsTrue(result.Contains(deviceType));
            }
        }

        private Device CreateDevice(DeviceType deviceType)
        {
            var device = new Device
            {
                Id = Guid.NewGuid(),
                DeviceTypeId = deviceType.Id
            };

            return device;
        }
    }
}
