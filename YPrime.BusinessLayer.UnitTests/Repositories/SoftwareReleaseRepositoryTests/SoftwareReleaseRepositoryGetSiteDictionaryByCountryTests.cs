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
    public class SoftwareReleaseRepositoryGetSiteDictionaryByCountryTests : SoftwareReleaseRepositoryTestBase
    {
        private CountryModel _countryA;
        private CountryModel _countryB;

        private Guid _siteAId;
        private Site _siteA;

        private Guid _siteBId;
        private Site _siteB;

        private Device _tabletDeviceA;
        private Device _handheldDeviceB;

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
                Name = "Site A"
            };

            _siteBId = Guid.NewGuid();

            _siteB = new Site
            {
                Id = _siteBId,
                CountryId = _countryB.Id,
                Name = "Site B"
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

            var devices = new List<Device> { _tabletDeviceA, _handheldDeviceB };

            var deviceDataset = new FakeDbSet<Device>(devices);

            Context
                .Setup(c => c.Devices)
                .Returns(deviceDataset.Object);

            var sites = new List<Site> { _siteA, _siteB };

            var siteDataset = new FakeDbSet<Site>(sites);

            Context
                .Setup(c => c.Sites)
                .Returns(siteDataset.Object);
        }

        [TestMethod]
        public async Task GetSiteDictionaryByCountryTest()
        {
            var deviceTypeIds = new List<Guid> { DeviceType.Phone.Id };
            var countryIds = new List<Guid> { _countryB.Id };

            var results = await Repository.GetSiteDictionaryByCountry(
                deviceTypeIds,
                countryIds);

            Assert.AreEqual(1, results.Count);

            var firstResult = results.First();
            Assert.AreEqual(_siteBId.ToString(), firstResult.Key);
            Assert.AreEqual(_siteB.Name, firstResult.Value);
        }

        [TestMethod]
        public async Task GetSiteDictionaryByCountryNullDeviceTypeTest()
        {
            var countryIds = new List<Guid> { _countryA.Id };

            var results = await Repository.GetSiteDictionaryByCountry(
                null,
                countryIds);

            Assert.AreEqual(1, results.Count);

            var firstResult = results.First();
            Assert.AreEqual(_siteAId.ToString(), firstResult.Key);
            Assert.AreEqual(_siteA.Name, firstResult.Value);
        }

        [TestMethod]
        public async Task GetSiteDictionaryByCountryNullListsTest()
        {
            var results = await Repository.GetSiteDictionaryByCountry(
                null,
                null);

            Assert.AreEqual(2, results.Count);

            var firstResult = results.First(r => r.Key == _siteAId.ToString());
            Assert.AreEqual(_siteA.Name, firstResult.Value);

            var secondResult = results.First(r => r.Key == _siteBId.ToString());
            Assert.AreEqual(_siteB.Name, secondResult.Value);
        }
    }
}
