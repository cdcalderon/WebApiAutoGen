using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.ConfigurationVersionServiceTests
{
    [TestClass]
    public abstract class ConfigurationVersionServiceTestBase : ConfigServiceTestBase<ConfigurationVersion>
    {
        protected string ExpectedBaseEndpointAddress = $"{BaseTestHttpAddress}/Versions/true";
        protected readonly Guid VersionOneId = Guid.Parse("b20b304c-14f1-4df4-9195-20de7653c047");

        protected const string GetAllResponse = "[{\"id\":\"b20b304c-14f1-4df4-9195-20de7653c047\",\"studyId\":\"265326f4-e285-4117-a8ba-915d5373bd1f\",\"srdVersion\":\"01.00\",\"configurationVersionNumber\":\"1.0\",\"dateCreated\":\"2020-09-15T12:48:49.325Z\",\"createdBy\":\"ypadmin@yprime.com\",\"approvedForProd\":false,\"description\":\"Initial file version\",\"actionPanel\":{\"canDelete\":true,\"canUpdate\":true,\"acknowledge\":false}},{\"id\":\"975ba44f-8980-4829-bfc4-f7d31cc6c1f0\",\"studyId\":\"265326f4-e285-4117-a8ba-915d5373bd1f\",\"srdVersion\":\"02.00\",\"configurationVersionNumber\":\"2.0\",\"dateCreated\":\"2020-09-15T12:48:58.394Z\",\"createdBy\":\"ypadmin@yprime.com\",\"approvedForProd\":false,\"description\":\"Second file version\",\"actionPanel\":{\"canDelete\":true,\"canUpdate\":true,\"acknowledge\":false}}]";

        protected Mock<IServiceSettings> MockServiceSettings;

        protected ConfigurationVersionServiceTestBase()
            : base(GetAllResponse)
        { }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            MockServiceSettings = new Mock<IServiceSettings>();
        }

        protected IConfigurationVersionService GetService()
        {
            var service = new ConfigurationVersionService(
                MockHttpFactory.Object,
                MemoryCache,
                MockSessionService.Object,
                MockServiceSettings.Object,
                _authSettings,
                MockAuthService.Object);

            return service;
        }
    }
}
