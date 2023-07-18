using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryGetCountryDictionaryByDeviceTypeTests : SoftwareReleaseRepositoryTestBase
    {
        private CountryModel _countryA;
        private CountryModel _countryB;

        private Guid _siteAId;
        private Site _siteA;

        private Guid _siteBId;
        private Site _siteB;

        private Device _tabletDeviceA;
        private Device _handheldDeviceB;
        private Device _byodDeviceC;

        [TestInitialize]
        public void TestInitialize()
        {
            _countryA = new CountryModel
            {
                Id = Guid.NewGuid(),
                Name = "Country A"
            };

            _countryB = new CountryModel
            {
                Id = Guid.NewGuid(),
                Name = "Country B"
            };

            CountryService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CountryModel>
                {
                    _countryA,
                    _countryB
                });

            _siteAId = Guid.NewGuid();

            _siteA = new Site
            {
                Id = _siteAId,
                CountryId = _countryA.Id,
            };

            _siteBId = Guid.NewGuid();

            _siteB = new Site
            {
                Id = _siteBId,
                CountryId = _countryB.Id,
            };

            _tabletDeviceA = new Device
            {
                Id = Guid.NewGuid(),
                AssetTag = Guid.NewGuid().ToString(),
                SiteId = _siteA.Id,
                Site = _siteA,
                DeviceTypeId = DeviceType.Tablet.Id
            };

            _handheldDeviceB = new Device
            {
                Id = Guid.NewGuid(),
                AssetTag = Guid.NewGuid().ToString(),
                SiteId = _siteB.Id,
                Site = _siteB,
                DeviceTypeId = DeviceType.Phone.Id
            };

            _byodDeviceC = new Device
            {
                Id = Guid.NewGuid(),
                AssetTag = Guid.NewGuid().ToString(),
                SiteId = _siteA.Id,
                Site = _siteA,
                DeviceTypeId = DeviceType.BYOD.Id
            };

            var devices = new List<Device> { _tabletDeviceA, _handheldDeviceB, _byodDeviceC };

            var deviceDataset = new FakeDbSet<Device>(devices);

            Context
                .Setup(c => c.Devices)
                .Returns(deviceDataset.Object);
        }

        [TestMethod]
        public async Task GetCountryDictionaryByDeviceTypeTest()
        {
            var results = await Repository.GetCountryDictionaryByDeviceType(new List<Guid>
            {
                DeviceType.Tablet.Id
            });

            Assert.AreEqual(1, results.Count);

            var firstResult = results.First(r => r.Key == _countryA.Id.ToString());
            Assert.AreEqual(_countryA.Name.ToString(), firstResult.Value);
        }

        [TestMethod]
        public async Task GetCountryDictionaryByDeviceTypeByodTest()
        {
            var results = await Repository.GetCountryDictionaryByDeviceType(new List<Guid>
            {
                DeviceType.Phone.Id
            });

            Assert.AreEqual(2, results.Count);

            var firstResult = results.First(r => r.Key == _countryA.Id.ToString());
            Assert.AreEqual(_countryA.Name.ToString(), firstResult.Value);

            var secondResult = results.First(r => r.Key == _countryB.Id.ToString());
            Assert.AreEqual(_countryB.Name.ToString(), secondResult.Value);
        }

        [TestMethod]
        public async Task GetCountryDictionaryByDeviceTypeNullListTest()
        {
            var results = await Repository.GetCountryDictionaryByDeviceType(null);

            Assert.AreEqual(2, results.Count);

            var firstResult = results.First(r => r.Key == _countryA.Id.ToString());
            Assert.AreEqual(_countryA.Name.ToString(), firstResult.Value);

            var secondResult = results.First(r => r.Key == _countryB.Id.ToString());
            Assert.AreEqual(_countryB.Name.ToString(), secondResult.Value);
        }
    }
}