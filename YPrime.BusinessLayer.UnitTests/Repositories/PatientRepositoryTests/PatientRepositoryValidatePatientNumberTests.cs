using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryValidatePatientNumberTests : PatientRepositoryTestBase
    {
        private const string SubjectNumberErrorKey = "PatientNumber";
        private const string SubjectNumberErrorMessagePrefix = "Subject number must be ";
        private const string SubjectNumberErrorMessageSuffix = " digits long.";
        private const string SubjectNumberAllZerosErrorMessage = "Subject number must be greater than 0.";

        [TestMethod]
        public async Task PatientRepositoryValidatePatientNumberSuccessTest()
        {
            const string testPatientNumber = "321";
            var testModelStateDictionary = new ModelStateDictionary();

            var result = await Repository.ValidatePatientNumber(testPatientNumber, testModelStateDictionary);

            Assert.IsTrue(result);
            Assert.AreEqual(0, testModelStateDictionary.Count);
        }

        [TestMethod]
        public async Task PatientRepositoryValidatePatientNumberNotEqualTest()
        {
            const string testPatientNumber = "4321";
            const int testLength = 3;
            var testModelStateDictionary = new ModelStateDictionary();
            var expectedError = $"{SubjectNumberErrorMessagePrefix}{testLength}{SubjectNumberErrorMessageSuffix}";

            var result = await Repository.ValidatePatientNumber(testPatientNumber, testModelStateDictionary);

            Assert.IsFalse(result);
            Assert.AreEqual(1, testModelStateDictionary.Count);

            var firstError = testModelStateDictionary.First();

            Assert.AreEqual(SubjectNumberErrorKey, firstError.Key);
            Assert.AreEqual(expectedError, firstError.Value.Errors.First().ErrorMessage);
        }

        [TestMethod]
        public async Task PatientRepositoryValidatePatientNumberAllZerosTest()
        {
            const string testPatientNumber = "000";
            var testModelStateDictionary = new ModelStateDictionary();

            var result = await Repository.ValidatePatientNumber(testPatientNumber, testModelStateDictionary);

            Assert.IsFalse(result);
            Assert.AreEqual(1, testModelStateDictionary.Count);

            var firstError = testModelStateDictionary.First();

            Assert.AreEqual(SubjectNumberErrorKey, firstError.Key);
            Assert.AreEqual(SubjectNumberAllZerosErrorMessage, firstError.Value.Errors.First().ErrorMessage);
        }
    }
}