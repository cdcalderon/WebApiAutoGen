using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryGenerateDefaultPinTests : PatientRepositoryTestBase
    {
        [TestMethod]
        public async Task GenerateDefaultPinTest()
        {
            const string expectedResult = "1234";

            var result = await Repository.GenerateDefaultPin();

            Assert.AreEqual(expectedResult, result);
        }
    }
}
