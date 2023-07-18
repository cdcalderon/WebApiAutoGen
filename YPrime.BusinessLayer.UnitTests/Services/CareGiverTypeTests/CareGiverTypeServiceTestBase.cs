using System;
using System.Collections.Generic;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.CareGiverTypeTests
{
    public class CareGiverTypeServiceTestBase : ConfigServiceTestBase<CareGiverTypeModel>
    {
        protected string ExpectedHttpAddress = $"{BaseTestHttpAddress}/CareGiverType";

        protected const string DefaultGetAllResponseContent = "[{\"id\": \"ca926028-b343-4825-96a4-f1e0b2fe40bb\",    \"name\": \"Spouse\",    \"translationKey\": \"Spouse\",    \"actionPanel\": true  },  {    \"id\": \"ae4a4c85-fa41-4a48-ba7b-b7464333d3a4\",    \"name\": \"Parent\",    \"translationKey\": \"Parent\",    \"actionPanel\": true  },  {    \"id\": \"194b7735-e538-41ba-82ce-f8cf5388fe5d\",    \"name\": \"Child\",    \"translationKey\": \"Child\",    \"actionPanel\": true  },  {    \"id\": \"c9dd5c83-5f7f-4411-a58a-6b8f4e752060\",    \"name\": \"Other Family Member\",    \"translationKey\": \"OtherFamilyMember\",    \"actionPanel\": true  },  {    \"id\": \"7869ecc1-355d-44ab-8ba0-28b38999403e\",    \"name\": \"Friend\",    \"translationKey\": \"Friend\",    \"actionPanel\": true  },  {    \"id\": \"880900f9-4cbb-4b27-bc00-dd779f7adf0a\",    \"name\": \"Caregiver\",    \"translationKey\": \"Caregiver\",    \"actionPanel\": true  },  {    \"id\": \"660b59c0-1188-43a3-8d9a-c24695fbe317\",    \"name\": \"Other\",    \"translationKey\": \"Other\",    \"actionPanel\": true  }]";
        protected const string DefaultSingleResponseContent = "{\"id\": \"ae4a4c85-fa41-4a48-ba7b-b7464333d3a4\",    \"name\": \"Parent\",    \"translationKey\": \"Parent\",    \"actionPanel\": true  }";
        protected const string DefaultSingleResponseContent2 = "{\"id\": \"194b7735-e538-41ba-82ce-f8cf5388fe5d\",    \"name\": \"Child\",    \"translationKey\": \"Child\",    \"actionPanel\": true }";

        protected static IReadOnlyDictionary<int, CareGiverTypeModel> AlphabeticalCareGiverTypeModels
        {
            get
            {
                return new Dictionary<int, CareGiverTypeModel>
                {
                    { 0, new CareGiverTypeModel { Id = Guid.Parse("880900f9-4cbb-4b27-bc00-dd779f7adf0a"), Name = "Caregiver", TranslationKey = "Caregiver" } },
                    { 1, new CareGiverTypeModel { Id = Guid.Parse("194b7735-e538-41ba-82ce-f8cf5388fe5d"), Name = "Child", TranslationKey = "Child" } },
                    { 2, new CareGiverTypeModel { Id = Guid.Parse("7869ecc1-355d-44ab-8ba0-28b38999403e"), Name = "Friend", TranslationKey = "Friend" } },
                    { 3, new CareGiverTypeModel { Id = Guid.Parse("660b59c0-1188-43a3-8d9a-c24695fbe317"), Name = "Other", TranslationKey = "Other" } },
                    { 4, new CareGiverTypeModel { Id = Guid.Parse("c9dd5c83-5f7f-4411-a58a-6b8f4e752060"), Name = "Other Family Member", TranslationKey = "OtherFamilyMember" } },
                    { 5, new CareGiverTypeModel { Id = Guid.Parse("ae4a4c85-fa41-4a48-ba7b-b7464333d3a4"), Name = "Parent", TranslationKey = "Parent" } },
                    { 6, new CareGiverTypeModel { Id = Guid.Parse("ca926028-b343-4825-96a4-f1e0b2fe40bb"), Name = "Spouse", TranslationKey = "Spouse" } }
                };
            }
        }
       
        protected CareGiverTypeServiceTestBase()
           : base(DefaultGetAllResponseContent)
        { }

        protected ICareGiverTypeService GetService()
        {
            var service = new CareGiverTypeService(
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
