using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using YPrime.BusinessLayer.Constants;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DataSyncRepositoryTests
{
    [TestClass]
    public class DataSyncRepositoryCheckForUpdatesTests : DataSyncRepositoryTestBase
    {
        private const string softwareVersionNumber = "1.0.0.0";
        private const string configurationVersion = "2.0.0.0";
        private const string SRDVersion = "9.0.0.0";
        private const string packagePath = "test_path";
        private const string CDNUrl = "test_cdn_url";

        private Guid deviceId1 = Guid.NewGuid();
        private Guid deviceId2 = Guid.NewGuid();
        private Guid softwareVersionId = Guid.NewGuid();
        private Guid softwareReleaseId = Guid.NewGuid();
        private Guid softwareReleaseIdNotActive = Guid.NewGuid();
        private Guid softwareReleaseConfigurationId = Guid.NewGuid();
        private SoftwareRelease softwareRelease;
        private SoftwareRelease softwareReleaseNotActive;

        [TestInitialize]
        public void TestInitialize()
        {
            System.Configuration.ConfigurationManager.AppSettings["SharePathBase"] = "testSharePath";
            var softwareVersion = new SoftwareVersion
            {
                Id = softwareVersionId,
                VersionNumber = softwareVersionNumber,
                PackagePath = packagePath
            };

            softwareRelease = new SoftwareRelease
            {
                Id = softwareReleaseId,
                Name = "test",
                SoftwareVersionId = softwareVersionId,
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = softwareReleaseConfigurationId,
                ConfigurationVersion = configurationVersion,
                SRDVersion = SRDVersion,
                StudyWide = true,
                SoftwareVersion = softwareVersion
            };

            softwareReleaseNotActive = new SoftwareRelease
            {
                Id = softwareReleaseIdNotActive,
                Name = "test_not_active",
                SoftwareVersionId = softwareVersionId,
                DateCreated = new DateTimeOffset(),
                IsActive = false,
                Required = true,
                ConfigurationId = softwareReleaseConfigurationId,
                ConfigurationVersion = configurationVersion,
                StudyWide = true,
                SoftwareVersion = softwareVersion
            };

            var device1 = new Device
            {
                Id = deviceId1,
                SoftwareReleaseId = softwareReleaseId,
                SoftwareRelease = softwareRelease,

            };

            var device2 = new Device
            {
                Id = deviceId2,
                SoftwareReleaseId = softwareReleaseIdNotActive,
                SoftwareRelease = softwareReleaseNotActive,

            };
            var _deviceDataSet = new FakeDbSet<Device>(new[] { device1, device2 });
            _deviceDataSet.Setup(d => d.Find(It.Is<Guid>(id => id == deviceId1))).Returns(device1);
            _deviceDataSet.Setup(d => d.Find(It.Is<Guid>(id => id == deviceId2))).Returns(device2);
            Context.Setup(ctx => ctx.Devices)
                .Returns(_deviceDataSet.Object);

            Context.Setup(c => c.Devices).Returns(_deviceDataSet.Object);

            var _softwareVesionDataSet = new FakeDbSet<SoftwareVersion>(new[] { softwareVersion });
            Context.Setup(ctx => ctx.SoftwareVersions)
                .Returns(_softwareVesionDataSet.Object);

            Context.Setup(c => c.Devices).Returns(_deviceDataSet.Object);
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesReturnsNullPackagePathWhenApkVersionIsNull()
        {
            var result = Repository.CheckForUpdates(deviceId1, null, null);

            Assert.AreEqual(null, result.PackagePath);
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesReturnsAssignedConfigurationIdWhenCurrentVersionIsLower()
        {
            System.Configuration.ConfigurationManager.AppSettings["YPrimeConfigBlobUrl"] = CDNUrl;
            var result = Repository.CheckForUpdates(deviceId1, softwareVersionNumber, "1.0.0.0");

            var urlPath = result.ConfigCDNUrl.Split('/');

            Assert.AreEqual(3, urlPath.Length);
            Assert.AreEqual(CDNUrl, urlPath[0]);
            Assert.AreEqual(DataSyncConstants.ConfigCDNUrlPath, urlPath[1]);
            Assert.AreEqual(urlPath[2], $"{softwareReleaseConfigurationId}.json");
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesReturnsAssignedConfigurationTrailingSlash()
        {
            System.Configuration.ConfigurationManager.AppSettings["YPrimeConfigBlobUrl"] = $"{CDNUrl}/";
            var result = Repository.CheckForUpdates(deviceId1, softwareVersionNumber, "1.0.0.0");

            var urlPath = result.ConfigCDNUrl.Split('/');

            Assert.AreEqual(3, urlPath.Length);
            Assert.AreEqual(CDNUrl, urlPath[0]);
            Assert.AreEqual(DataSyncConstants.ConfigCDNUrlPath, urlPath[1]);
            Assert.AreEqual(urlPath[2], $"{softwareReleaseConfigurationId}.json");
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesReturnsAssignedConfigurationTranslationUrl()
        {
            System.Configuration.ConfigurationManager.AppSettings["YPrimeConfigBlobUrl"] = $"{CDNUrl}/";
            var result = Repository.CheckForUpdates(deviceId1, softwareVersionNumber, "1.0.0.0");

            var urlPath = result.ConfigCDNUrl.Split('/');
            var translationUrlPath = result.TranslationCDNUrl.Split('/');

            // Check the standard config url for comparison
            Assert.AreEqual(3, urlPath.Length);
            Assert.AreEqual(CDNUrl, urlPath[0]);
            Assert.AreEqual(DataSyncConstants.ConfigCDNUrlPath, urlPath[1]);
            Assert.AreEqual($"{softwareReleaseConfigurationId}.json", urlPath[2]);

            // Check the translation Url
            Assert.AreEqual(3, translationUrlPath.Length);
            Assert.AreEqual(CDNUrl, translationUrlPath[0]);
            Assert.AreEqual(DataSyncConstants.ConfigCDNUrlPath, translationUrlPath[1]);
            Assert.AreEqual(DataSyncConstants.TranslationCDNUrlPath, translationUrlPath[2]);

        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesReturnsNullConfigurationIdWhenCurrentVersionIsHigher()
        {
            System.Configuration.ConfigurationManager.AppSettings["YPrimeConfigBlobUrl"] = $"{CDNUrl}/";
            var result = Repository.CheckForUpdates(deviceId1, softwareVersionNumber, "3.0.0.0");

            Assert.IsNull(result.ConfigCDNUrl);
            Assert.IsNotNull(result.TranslationCDNUrl);

            var translationUrlPath = result.TranslationCDNUrl.Split('/');

            Assert.AreEqual(3, translationUrlPath.Length);
            Assert.AreEqual(CDNUrl, translationUrlPath[0]);
            Assert.AreEqual(DataSyncConstants.ConfigCDNUrlPath, translationUrlPath[1]);
            Assert.AreEqual(DataSyncConstants.TranslationCDNUrlPath, translationUrlPath[2]);
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesPackagePathIsAssignedWhenCurrentApkVersionLowerThanAssignedVersion()
        {
            var result = Repository.CheckForUpdates(deviceId1, "0.0.0.1", null);

            Assert.AreEqual(result.PackagePath, packagePath);
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesPackagePathNotAssignedWhenCurrentApkVersionHigherThanAssignedVersion()
        {
            var result = Repository.CheckForUpdates(deviceId1, "2.0.0.0", null);

            Assert.AreEqual(null, result.PackagePath);
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesPackagePathAndConfigurationAssignedWhenBothVersionsLowerThanAssigned()
        {
            var result = Repository.CheckForUpdates(deviceId1, "0.0.0.1", "0.0.0.1");

            Assert.AreEqual(result.PackagePath, packagePath);
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesReturnsNullConfigurationIdWhenCurrentVersionIsLowerAndApkVersionIsAssigned()
        {
            var result = Repository.CheckForUpdates(deviceId1, "0.0.0.1", "3.0.0.0");

            Assert.AreEqual(result.PackagePath, packagePath);
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesPackagePathAndConfigurationNullWhenSoftwareReleaseIsNotActive()
        {
            System.Configuration.ConfigurationManager.AppSettings["YPrimeConfigBlobUrl"] = CDNUrl;
            var result = Repository.CheckForUpdates(deviceId2, "0.0.0.1", "0.0.0.1");

            Assert.IsNull(result.PackagePath);
            Assert.IsNull(result.ConfigCDNUrl);
            Assert.IsNotNull(result.TranslationCDNUrl);

            var translationUrlPath = result.TranslationCDNUrl.Split('/');

            Assert.AreEqual(3, translationUrlPath.Length);
            Assert.AreEqual(CDNUrl, translationUrlPath[0]);
            Assert.AreEqual(DataSyncConstants.ConfigCDNUrlPath, translationUrlPath[1]);
            Assert.AreEqual(DataSyncConstants.TranslationCDNUrlPath, translationUrlPath[2]);
        }

        [TestMethod]
        public void DataSyncRepositoryCheckForUpdatesPackagePathAndConfigurationNullWhenSoftwareReleaseIsNotActiveWithDefaultConfig()
        {
            System.Configuration.ConfigurationManager.AppSettings["YPrimeConfigBlobUrl"] = CDNUrl;

            var result = Repository.CheckForUpdates(
                deviceId2, 
                "0.0.0.1", 
                Config.Defaults.ConfigurationVersions.InitialVersion.ConfigurationVersionNumber);

            Assert.AreEqual(packagePath, result.PackagePath);

            var urlPath = result.ConfigCDNUrl.Split('/');
            var translationUrlPath = result.TranslationCDNUrl.Split('/');

            Assert.AreEqual(3, urlPath.Length);
            Assert.AreEqual(CDNUrl, urlPath[0]);
            Assert.AreEqual("versionblobs", urlPath[1]);
            Assert.AreEqual(urlPath[2], $"{softwareReleaseConfigurationId}.json");

            // Check the translation Url
            Assert.AreEqual(3, translationUrlPath.Length);
            Assert.AreEqual(CDNUrl, translationUrlPath[0]);
            Assert.AreEqual(DataSyncConstants.ConfigCDNUrlPath, translationUrlPath[1]);
            Assert.AreEqual(DataSyncConstants.TranslationCDNUrlPath, translationUrlPath[2]);
        }
    }
}
