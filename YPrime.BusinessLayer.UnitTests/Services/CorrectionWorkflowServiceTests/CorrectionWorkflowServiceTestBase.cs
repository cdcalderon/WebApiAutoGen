using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.CorrectionWorkflowServiceTests
{
    public abstract class CorrectionWorkflowServiceTestBase : ConfigServiceTestBase<CorrectionWorkflowSettingsModel>
    {
        public CorrectionWorkflowServiceTestBase()
            : base(string.Empty)
        { }

        protected ICorrectionWorkflowService GetService()
        {
            var service = new CorrectionWorkflowService(
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
