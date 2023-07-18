using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.VisitServiceTests
{
    [TestClass]
    public abstract class VisitServiceTestBase : ConfigServiceTestBase<VisitModel>
    {
        protected string ExpectedBaseEndpointAddress = $"{BaseTestHttpAddress}/Visit";

        protected const string GetAllResponse = "[{\"daysExpected\":0,\"windowBefore\":0,\"windowAfter\":0,\"notes\":\"Test Visit 1\",\"isScheduled\":true,\"visitAnchor\":null,\"visitStop_HSN\":\"S\",\"alwaysAvailable\":false,\"closeoutFormAvailable\":true,\"visitAvailableBusinessRuleId\":null,\"visitAvailableBusinessRuleTrueFalseIndicator\":false,\"questionnaires\":[{\"questionnaireId\":\"d63f9b76-a70a-4529-bd64-0a654fc4d9fa\",\"sequence\":0},{\"questionnaireId\":\"e4cc6e6c-05fb-4c37-90c8-b7fad6959eb3\",\"sequence\":1}],\"customExtensions\":[{\"name\":\"Test1\",\"value\":\"2\"},{\"name\":\"isCustomExtensionVisit\",\"value\":\"true\"}],\"id\":\"f3555770-91f8-471d-9fd1-626d50cbae2b\",\"name\":\"Visit 1\",\"visitOrder\":0}]";

        protected VisitServiceTestBase()
            : base(GetAllResponse)
        { }

        protected IVisitService GetService()
        {
            var service = new VisitService(
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
