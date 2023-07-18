using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Constants;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DeviceRepositoryTests
{
    [TestClass]
    public class DeviceRepositoryGetDevicesAtSiteCountByDeviceTypeTests : DeviceRepositoryTestBase
    {
        private const int SiteATabletCount = 5;
        private const int SiteAHandheldCount = 8;
        private const int SiteBTabletCount = 11;
        private const int SiteBHandheldCount = 29;
        private const int SiteCTabletCount = 2;
        private const int SiteCHandheldCount = 6;

        private readonly Guid SiteAId = Guid.NewGuid();
        private readonly Guid SiteBId = Guid.NewGuid();
        private readonly Guid SiteCId = Guid.NewGuid();

        [TestInitialize]
        public void TestInitialize()
        {
            Context.Reset();

            var devices = new List<Device>();

            for (var i = 0; i < SiteATabletCount; i++)
            {
                devices.Add(CreateTestDevice(SiteAId, DeviceType.Tablet.Id));
            }

            for (var i = 0; i < SiteAHandheldCount; i++)
            {
                devices.Add(CreateTestDevice(SiteAId, DeviceType.Phone.Id));
            }

            for (var i = 0; i < SiteBTabletCount; i++)
            {
                devices.Add(CreateTestDevice(SiteBId, DeviceType.Tablet.Id));
            }

            for (var i = 0; i < SiteBHandheldCount; i++)
            {
                devices.Add(CreateTestDevice(SiteBId, DeviceType.Phone.Id));
            }

            for (var i = 0; i < SiteCTabletCount; i++)
            {
                devices.Add(CreateTestDevice(SiteCId, DeviceType.Tablet.Id));
            }

            for (var i = 0; i < SiteCHandheldCount; i++)
            {
                devices.Add(CreateTestDevice(SiteCId, DeviceType.Phone.Id));
            }

            SetupContext(devices);
        }

        [TestMethod]
        public void DeviceRepositoryGetDevicesAtSiteCountByDeviceTypeAllSiteTest()
        {
            var expectedResult = SiteAHandheldCount + SiteATabletCount + SiteBHandheldCount + SiteBTabletCount +
                                 SiteCHandheldCount + SiteCTabletCount;

            var result = Repository.GetDevicesAtSiteCountByDeviceType(null, null, null);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void DeviceRepositoryGetDevicesAtSiteCountByDeviceTypeSingleSiteTest()
        {
            var expectedResult = SiteAHandheldCount + SiteATabletCount;

            var result = Repository.GetDevicesAtSiteCountByDeviceType(SiteAId, null, null);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void DeviceRepositoryGetDevicesAtSiteCountByDeviceTypeMultipleSiteTest()
        {
            var expectedResult = SiteBHandheldCount + SiteBTabletCount + SiteCHandheldCount + SiteCTabletCount;

            var sites = new List<SiteDto>
            {
                new SiteDto
                {
                    Id = SiteBId
                },
                new SiteDto
                {
                    Id = SiteCId
                }
            };

            var result = Repository.GetDevicesAtSiteCountByDeviceType(null, sites, null);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void DeviceRepositoryGetDevicesAtSiteCountByDeviceTypeMultipleSiteWithSingleTest()
        {
            var expectedResult = SiteBHandheldCount + SiteBTabletCount + SiteCHandheldCount + SiteCTabletCount;

            var sites = new List<SiteDto>
            {
                new SiteDto
                {
                    Id = SiteBId
                }
            };

            var result = Repository.GetDevicesAtSiteCountByDeviceType(SiteCId, sites, null);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void DeviceRepositoryGetDevicesAtSiteCountByDeviceTypeMultipleSitesWithTypeTest()
        {
            var expectedResult = SiteAHandheldCount + SiteBHandheldCount;

            var sites = new List<SiteDto>
            {
                new SiteDto
                {
                    Id = SiteAId
                },
                new SiteDto
                {
                    Id = SiteBId
                }
            };

            var result = Repository.GetDevicesAtSiteCountByDeviceType(null, sites, HandheldDeviceTypeName);

            Assert.AreEqual(expectedResult, result);
        }
    }
}