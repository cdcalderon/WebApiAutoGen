using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.ApproverGroupServiceTests
{
    [TestClass]
    public abstract class ApproverGroupServiceTestBase : ConfigServiceTestBase<ApproverGroupModel>
    {
        protected string ExpectedBaseEndpointAddress = $"{BaseTestHttpAddress}/ApproverGroup";

        protected const string GetAllResponse = "[{\"id\":\"784008ed-e66f-4ebb-9591-e59011765f53\",\"name\":\"Test Group\",\"actionPanel\":{\"canDelete\":false,\"canUpdate\":false,\"acknowledge\":true},\"roles\":[{\"id\":\"8e4df193-beb5-4498-a451-b1798cf8ec52\",\"description\":\"Clinical Research Associate\"},{\"id\":\"7430faf4-e0a4-44ab-b708-c0b85061f9c4\",\"description\":\"Investigator\"}]}]";

        protected ApproverGroupServiceTestBase()
            : base(GetAllResponse)
        { }

        protected IApproverGroupService GetService()
        {
            var service = new ApproverGroupService(
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
