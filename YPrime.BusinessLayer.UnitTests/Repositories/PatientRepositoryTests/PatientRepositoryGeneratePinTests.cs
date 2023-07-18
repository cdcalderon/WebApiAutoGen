using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryGeneratePinTests : PatientRepositoryTestBase
    {
        [TestMethod]
        public async Task GeneratePinTest()
        {
            const int expectedLength = 4;

            var result = await Repository.GeneratePin();

            Assert.AreEqual(expectedLength, result.Length);

            foreach (var individualChar in result)
            {
                int.Parse(individualChar.ToString());
            }
        }
    }
}
