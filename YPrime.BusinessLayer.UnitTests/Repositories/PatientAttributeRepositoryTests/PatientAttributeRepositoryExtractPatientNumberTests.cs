using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientAttributeRepositoryTests
{
    [TestClass]
    public class PatientAttributeRepositoryExtractPatientNumberTests : PatientAttributeRepositoryTestBase
    {
        [TestMethod]
        public async Task PatientAttributeRepositoryExtractPatientNumberWithSeperatorTest()
        {
            SetStudySettingPatientLength(6);

            const string subjectNumberPart = "522356";
            var testValue = $"S-100-{subjectNumberPart}";

            var result = await Repository.ExtractPatientNumber(testValue);

            Assert.AreEqual(subjectNumberPart, result);
        }

        [TestMethod]
        public async Task PatientAttributeRepositoryExtractPatientNumberWithoutSeperatorTest()
        {
            SetStudySettingPatientLength(6, false);

            const string subjectNumberPart = "522356";
            var testValue = $"S-100-{subjectNumberPart}";

            var result = await Repository.ExtractPatientNumber(testValue);

            Assert.AreEqual(subjectNumberPart, result);
        }

        [TestMethod]
        public async Task PatientAttributeRepositoryExtractPatientNumberWithoutSeperatorShortTest()
        {
            SetStudySettingPatientLength(5, false);

            const string subjectNumberPart = "522356";
            var testValue = $"S-100-{subjectNumberPart}";
            const string expectedValue = "22356";

            var result = await Repository.ExtractPatientNumber(testValue);

            Assert.AreEqual(expectedValue, result);
        }
    }
}