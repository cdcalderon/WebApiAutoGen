using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Services.SubjectInformationServiceTests
{
    [TestClass]
    public class SubjectInformationServiceGetForCountryTests : SubjectInformationServiceTestBase
    {
        [TestMethod]
        public async Task SubjectInformationService_GetForCountry_Test()
        {
            const int expectedCount = 9;

            var service = GetService();

            var chinaId = Guid.Parse(ChinaCountryId);

            var result = await service.GetForCountry(chinaId);

            Assert.AreEqual(expectedCount, result.Count);
            Assert.IsTrue(result.All(r => r.Countries.Any(c => c.Name == ChinaCountryAbbreviation)));
        }
    }
}
