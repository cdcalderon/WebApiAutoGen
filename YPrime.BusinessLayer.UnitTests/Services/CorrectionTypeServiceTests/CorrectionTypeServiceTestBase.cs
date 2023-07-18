using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.CorrectionTypeServiceTests
{
    public abstract class CorrectionTypeServiceTestBase : ConfigServiceTestBase<CorrectionTypeModel>
    {
        protected string ExpectedBaseEndpointAddress = $"{BaseTestHttpAddress}/CorrectionType";

        protected const string GetAllResponse = "[{\"id\":\"10bc9f6f-64ea-42cc-9728-8017c5699614\",\"name\":\"Change questionnaire information\",\"description\":\"This will allow you to modify basic questionnaire information.\",\"hasConfiguration\":false},{\"id\":\"1d458730-843e-4f7b-82fe-c3963457b126\",\"name\":\"Change questionnaire responses\",\"description\":\"This will allow you to modify questionnaire responses.\",\"hasConfiguration\":false},{\"id\":\"a2af05a3-de13-4076-8fba-95d4d04bab99\",\"name\":\"Change subject Visit\",\"description\":\"This will allow you to change a subject''s visit information.\",\"hasConfiguration\":false},{\"id\":\"51ff8e15-9381-4b2c-aaf0-06b88b02dc0e\",\"name\":\"Merge subjects\",\"description\":\"This will allow you to merge duplicate subject that may have been created in error.\",\"hasConfiguration\":false},{\"id\":\"1de27c71-b802-4061-80d4-b03314503f60\",\"name\":\"Change subject Information\",\"description\":\"This will allow you to alter subject information.\",\"hasConfiguration\":false},{\"id\":\"aba77543-7600-47d0-ad1e-699812bfa687\",\"name\":\"Remove a subject\",\"description\":\"This will allow you to remove a subject\",\"hasConfiguration\":false},{\"id\":\"2e8755e4-ce55-4f14-a70e-783e11409420\",\"name\":\"Add Paper Questionnaire\",\"description\":\"PaperDiaryEntryDescription\",\"hasConfiguration\":true}]";

        protected CorrectionTypeServiceTestBase()
            : base(GetAllResponse)
        { }

        protected ICorrectionTypeService GetService()
        {
            var service = new CorrectionTypeService(
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

