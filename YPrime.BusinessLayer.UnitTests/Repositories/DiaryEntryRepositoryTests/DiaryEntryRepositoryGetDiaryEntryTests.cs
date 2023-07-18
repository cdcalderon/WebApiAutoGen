using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.eCOA.Utilities.Helpers;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DiaryEntryRepositoryTests
{
    [TestClass]
    public class DiaryEntryRepositoryGetDiaryEntryTests : DiaryEntryRepositoryTestBase
    {
        [TestMethod]
        public async Task GetDiaryEntryTest()
        {
            TestDiaryEntry.DataSourceId = DataSource.eCOAApp.Id;

            var repository = GetRepository();

            var result = await repository.GetDiaryEntry(
                TestDiaryEntry.Id,
                false,
                Config.Defaults.Languages.English.CultureName);

            Assert.AreEqual(TestDiaryEntry.Id, result.Id);
            Assert.AreEqual(1, result.Answers.Count);

            foreach (var answer in result.Answers)
            {
                Assert.AreEqual(TestAnswer.Id, answer.Id);
                Assert.AreEqual(TestDiaryPage.Number, answer.PageNumber);
            }

            var expectedVersionString = $"{TestDiaryEntry.SoftwareVersionNumber}-{TestConfigVersion.ConfigurationVersionNumber}-{TestConfigVersion.SrdVersion}";

            Assert.AreEqual(expectedVersionString, result.Version);

            MockVisitService.Verify(
                s => s.Get(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid?>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetDiaryEntryWithTemperaturesTest()
        {
            TestDiaryEntry.DataSourceId = DataSource.eCOAApp.Id;

            var repository = GetRepository(true);

            var result = await repository.GetDiaryEntry(
                TestDiaryEntry.Id,
                false,
                Config.Defaults.Languages.English.CultureName);

            Assert.AreEqual(TestDiaryEntry.Id, result.Id);
            Assert.AreEqual(1, result.Answers.Count);

            foreach (var answer in result.Answers)
            {
                Assert.IsTrue(answer.Suffix == Temperature.DegreesFahrenheit || answer.Suffix == Temperature.DegreesCelsius);
                Assert.AreEqual(InputFieldType.TemperatureSpinner.Id, answer.Question.QuestionType);
                Assert.AreEqual(TestTempAnswer.Id, answer.Id);
                Assert.AreEqual(TestDiaryPage.Number, answer.PageNumber);
            }

            var expectedVersionString = $"{TestDiaryEntry.SoftwareVersionNumber}-{TestConfigVersion.ConfigurationVersionNumber}-{TestConfigVersion.SrdVersion}";

            Assert.AreEqual(expectedVersionString, result.Version);
        }

        [TestMethod]
        public async Task GetDiaryEntryPaperDcfTest()
        {
            TestDiaryEntry.DataSourceId = DataSource.Paper.Id;

            var repository = GetRepository();

            var result = await repository.GetDiaryEntry(
                TestDiaryEntry.Id,
                false,
                Config.Defaults.Languages.English.CultureName);

            Assert.AreEqual(TestDiaryEntry.Id, result.Id);
            Assert.AreEqual(1, result.Answers.Count);

            foreach (var answer in result.Answers)
            {
                Assert.AreEqual(TestAnswer.Id, answer.Id);
                Assert.AreEqual(TestDiaryPage.Number, answer.PageNumber);
            }

            var expectedVersionString = $"{TestConfigVersion.ConfigurationVersionNumber}-{TestConfigVersion.SrdVersion}";

            Assert.AreEqual(expectedVersionString, result.Version);
        }

        [TestMethod]
        public async Task GetDiaryEntryNullVisitTest()
        {
            TestDiaryEntry.VisitId = null;

            var repository = GetRepository();

            var result = await repository.GetDiaryEntry(
                TestDiaryEntry.Id,
                false,
                Config.Defaults.Languages.English.CultureName);

            Assert.AreEqual(TestDiaryEntry.Id, result.Id);

            MockVisitService.Verify(
                s => s.Get(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid?>()),
                Times.Never);
        }

        [TestMethod]
        public async Task GetDiaryEntryEmptyVisitGuidTest()
        {
            TestDiaryEntry.VisitId = Guid.Empty;

            var repository = GetRepository();

            var result = await repository.GetDiaryEntry(
                TestDiaryEntry.Id,
                false,
                Config.Defaults.Languages.English.CultureName);

            Assert.AreEqual(TestDiaryEntry.Id, result.Id);

            MockVisitService.Verify(
                s => s.Get(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid?>()),
                Times.Never);
        }
    }
}
